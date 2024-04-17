using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZLogger;

public class CheckUserSessionMiddleWare
{
    ILogger Logger;
    private readonly RequestDelegate _next;
    
    public CheckUserSessionMiddleWare(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var formString = context.Request.Path.Value;
        if (string.Compare(formString, "/api/Login", StringComparison.OrdinalIgnoreCase) == 0 ||
            string.Compare(formString, "/api/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0)
        {
            // Call the next delegate/middleware in the pipeline
            await _next(context);

            return;
        }

        // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
        // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
        context.Request.EnableBuffering();

        string AuthToken;
        string Id;
        string UId;
        string Mode;

        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
        {
            var bodyStr = await reader.ReadToEndAsync();

            // body String에 어떤 문자열도 없을때
            if (await IsValueExist(context, bodyStr) == false)
            {
                return;
            }

            
            /*var document = JsonDocument.Parse(bodyStr);
            
            if (CheckJsonFormat(context, document, out Id, out AuthToken) == false)
            {
                return;
            }*/

            /*var (userInfo, getUserErrorCode) = await _memoryDb.GetUserAsync(Id);

            if (await IdExistCheck(context, getUserErrorCode) == false)
            {
                return;
            }

            if (await UserAuthCheck(context, userInfo, AuthToken) == false)
            {
                return;
            }

            // Redis를 활용한 트랜잭션... 중복 처리 예방... 처리되기 전에 메시지를 2번 보내는 현상을 막기 위함.
            if (await RedisOverlapCheck(context, AuthToken) == false)
            {
                return;
            }

            context.Items[nameof(User)] = userInfo;*/
        }

        context.Request.Body.Position = 0;

        // Call the next delegate/middleware in the pipeline
        await _next(context);

        // 트랜잭션 해제(Redis 동기화 해제)
        //await _memoryDb.DelUserReqLockAsync(AuthToken);
    }

    private async Task<bool> RedisOverlapCheck(HttpContext context, string? AuthToken)
    {
        /*if (!await _memoryDb.SetUserReqLockAsync(AuthToken))
        {
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailSetNx
            });
            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            return false;
        }*/

        return true;
    }

    private static async Task<bool> UserAuthCheck(HttpContext context, User userInfo, string? AuthToken)
    {
        /*if (string.CompareOrdinal(userInfo.AuthToken, AuthToken) != 0)
        {
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailWrongAuthToken
            });
            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return false;
        }*/

        return true;
    }

    private static async Task<bool> IdExistCheck(HttpContext context, ErrorCode getUserErrorCode)
    {
        /*if (getUserErrorCode != ErrorCode.None)
        {
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.GetUserFailWrongId
            });
            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

            return false;
        }*/

        return true;
    }

    private bool CheckJsonFormat(HttpContext context, JsonDocument document, out string? Id,
        out string? authToken)
    {
        /*try
        {
            Id = document.RootElement.GetProperty("ID").GetString();
            authToken = document.RootElement.GetProperty("authToken").GetString();
            return true;
        }
        catch
        {
            Id = "";
            AuthToken = ""; // Error가 났는데 당연히 저장할 것이 없다.

            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailWrongKeyword
            });

            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            context.Response.Body.Write(bytes, 0, bytes.Length);

            return false;
        }*/

        Id = "";
        authToken = "";
        return false;
    }

    private async Task<bool> IsValueExist(HttpContext context, string bodyStr)
    {
        if (string.IsNullOrEmpty(bodyStr))
        {
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenFailNoBody
            });
            var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            return false;
        }

        return true;
    }





    
}

public class MiddlewareResponse
{
    public ErrorCode result { get; set; }
}

public class User
{

}
