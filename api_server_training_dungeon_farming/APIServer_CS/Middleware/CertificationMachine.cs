using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using APIServer.ModelDB;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace APIServer.Middleware;

public class CertificationMachine
{
    private readonly VersionConfig _versionConfig;

    private readonly Services.IMemoryDb _memoryDb;

    private readonly RequestDelegate _next;

    public CertificationMachine(RequestDelegate next, Services.IMemoryDb memoryDb, IOptions<VersionConfig> versionConfig)
    {
        _versionConfig = versionConfig.Value;
        _memoryDb = memoryDb;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var formString = context.Request.Path.Value;
        if (string.Compare(formString, "/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0
            || string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0)

        {
            // Call the next delegate/middleware in the pipeline
            await _next(context);
            return;
        }

        // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
        // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
        context.Request.EnableBuffering();

        string authToken;
        string email;

        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
        {
            var bodyStr = await reader.ReadToEndAsync();

            // body String에 어떤 문자열도 없을때
            if (await IsNullBodyDataThenSendError(context, bodyStr))
            {
                return;
            }

            var document = JsonDocument.Parse(bodyStr);

            if (IsInvalidJsonFormatThenSendError(context, document, out email, out authToken))
            {
                return;
            }

            var (isOk, userInfo) = await _memoryDb.GetCertifiedUser(email);
            if (isOk == false)
            {
                return;
            }

            if (await IsInvalidUserAuthTokenThenSendError(context, userInfo, authToken) == true)
            {
                return;
            }

            if (await IsInvalidAppVersionThenSendError(context, userInfo.AppVersion) == true)
            {
                return;
            }

            if (await IsInvalidMasterDataVersionThenSendError(context, userInfo.MasterDataVersion) == true)
            {
                return;
            }

            if (await SetLockAndIsFailThenSendError(context, email))
            {
                return;
            }

            context.Items[nameof(CertifiedUser)] = userInfo;
        }

        context.Request.Body.Position = 0;

        // Call the next delegate/middleware in the pipeline
        await _next(context);

        // Reids로 만든 트랜잭션 해제
        await _memoryDb.UnlockUserRequest(email);
    }

    private async Task<bool> SetLockAndIsFailThenSendError(HttpContext context, string email)
    {
        if (await _memoryDb.TryLockUserRequest(email))
        {
            return false;
        }

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.AuthTokenFailSetNx
        });

        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);

        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

        return true;
    }

    private async Task SendErrorCode(HttpContext context, ErrorCode errorCode)
    {
        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = errorCode
        });

        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);

        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
    }

    private async Task<bool> IsInvalidUserAuthTokenThenSendError(HttpContext context, CertifiedUser userInfo, string authToken)
    {
        if (string.CompareOrdinal(userInfo.AuthToken, authToken) == 0)
        {
            return false;
        }

        await SendErrorCode(context, ErrorCode.AuthTokenFailWrongAuthToken);
        return true;
    }

    private async Task<bool> IsInvalidAppVersionThenSendError(HttpContext context, Int32 appVersion)
    {
        if (_versionConfig.App == appVersion)
        {
            return false;
        }

        await SendErrorCode(context, ErrorCode.InvalidAppVersion);
        return true;
    }

    private async Task<bool> IsInvalidMasterDataVersionThenSendError(HttpContext context, Int32 masterDataVersion)
    {
        if (_versionConfig.MasterData == masterDataVersion)
        {
            return false;
        }

        await SendErrorCode(context, ErrorCode.InvalidMasterDataVersion);
        return true;
    }

    private bool IsInvalidJsonFormatThenSendError(HttpContext context, JsonDocument document, out string email, out string authToken)
    {
        try
        {
            email = document.RootElement.GetProperty("Email").GetString();
            authToken = document.RootElement.GetProperty("AuthToken").GetString();

            return false;
        }
        catch
        {
            email = "";
            authToken = "";

            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailWrongKeyword
            });

            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            context.Response.Body.Write(bytes, 0, bytes.Length);

            return true;
        }
    }

    async Task<bool> IsNullBodyDataThenSendError(HttpContext context, string bodyStr)
    {
        if (string.IsNullOrEmpty(bodyStr) == false)
        {
            return false;
        }

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.InvalidRequestHttpBody
        });

        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

        return true;
    }

    public class MiddlewareResponse
    {
        public ErrorCode result { get; set; }
    }
}
