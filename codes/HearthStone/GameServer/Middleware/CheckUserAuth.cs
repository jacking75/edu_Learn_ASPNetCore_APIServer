using GameServer.Repository.Interface;
using GameServer.Repository;
using GameServer.Models;
using System.Text.Json;

namespace GameServer.Middleware;

public class CheckUserAuth
{
    readonly IMemoryDb _memoryDb;
    readonly RequestDelegate _next;

    public CheckUserAuth(RequestDelegate next, IMemoryDb memoryDb)
    {
        _next = next;
        _memoryDb = memoryDb;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method == "OPTIONS")
        {
            // OPTIONS 요청 성공으로 처리하고 추가 체크 없이 통과시킴
            context.Response.StatusCode = 200;
            await _next(context);
            return;
        }

        var formString = context.Request.Path.Value;
        if(string.Compare(formString, "/Auth/Login", StringComparison.OrdinalIgnoreCase) == 0)
        {
            await _next(context);
            return;
        }

        // token이 있는지 검사하고 있다면 저장
        var (isTokenNotExist, token) = await IsTokenNotExistOrReturnToken(context);
        if (isTokenNotExist)
        {
            return;
        }

        //uid가 있는지 검사하고 있다면 저장
        var (isUidNotExist, uid) = await IsUidNotExistOrReturnUid(context);
        if (isUidNotExist)
        {
            return;
        }

        //uid를 키로 하는 데이터 없을 때
        (bool isOk, RdbAuthUserData userInfo) = await _memoryDb.GetUserAsync(uid);
        if (await IsInvalidUserAuthTokenNotFound(context, isOk))
        {
            return;
        }

        //토큰이 일치하지 않을 때
        if (await IsInvalidUserAuthTokenThenSendError(context, userInfo, token))
        {
            return;
        }

        ////이번 api 호출 끝날 때까지 redis키 잠금 만약 이미 잠겨있다면 에러
        var userLockKey = MemoryDbKeyMaker.MakeUserLockKey(userInfo.AccountUid.ToString());
        if (await SetLockAndIsFailThenSendError(context, userLockKey))
        {
            return;
        }

        context.Items[nameof(RdbAuthUserData)] = userInfo;

        // Call the next delegate/middleware in the pipeline
        await _next(context);

        // 트랜잭션 해제(Redis 동기화 해제)
        await _memoryDb.UnLockUserReqAsync(userLockKey);
    }

    async Task<(bool, string)> IsTokenNotExistOrReturnToken(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("token", out var token))
        {
            return (false, token);
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.TokenDoesNotExist
        });
        await context.Response.WriteAsync(errorJsonResponse);

        return (true, "");
    }

    async Task<(bool, string)> IsUidNotExistOrReturnUid(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("accountuid", out var uid))
        {
            return (false, uid);
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.AuthTokenKeyNotFound
        });
        await context.Response.WriteAsync(errorJsonResponse);

        return (true, "");
    }

    async Task<bool> SetLockAndIsFailThenSendError(HttpContext context, string AuthToken)
    {
        if (await _memoryDb.LockUserReqAsync(AuthToken))
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.AuthTokenFailSetNx
        });
        await context.Response.WriteAsync(errorJsonResponse);
        return true;
    }

    async Task<bool> IsInvalidUserAuthTokenThenSendError(HttpContext context, RdbAuthUserData userInfo, string token)
    {
        if (string.CompareOrdinal(userInfo.Token, token) == 0)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            result = ErrorCode.AuthTokenFailWrongAuthToken
        });
        await context.Response.WriteAsync(errorJsonResponse);

        return true;
    }

    async Task<bool> IsInvalidUserAuthTokenNotFound(HttpContext context, bool isOk)
    {
        if (!isOk)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
            {
                result = ErrorCode.AuthTokenKeyNotFound
            });
            await context.Response.WriteAsync(errorJsonResponse);
        }
        return !isOk;
    }

    class MiddlewareResponse
    {
        public ErrorCode result { get; set; }
    }
}
