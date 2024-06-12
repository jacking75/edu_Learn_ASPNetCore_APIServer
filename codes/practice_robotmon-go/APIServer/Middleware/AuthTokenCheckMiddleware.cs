using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ServerCommon;

namespace ApiServer.Services
{
    public class AuthTokenCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedisDb _redisDb;
        
        public AuthTokenCheckMiddleware(RequestDelegate next, IRedisDb redisDb)
        {
            _next = next;
            _redisDb = redisDb;
        }

        public async Task Invoke(HttpContext context)
        {
            var formString = context.Request.Path.Value;
            if (String.CompareOrdinal(formString, "/Login") == 0 ||
                String.CompareOrdinal(formString, "/CreateAccount") == 0)
            {
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
                {
                    var bodyStr = await reader.ReadToEndAsync();

                    // body String에 어떤 문자열도 없다면...
                    if (string.IsNullOrEmpty(bodyStr))
                    {
                        // http Response Code
                        context.Response.StatusCode = (int)ErrorCode.AuthTokenFailNoBody;
                        return;
                    }

                    var document = JsonDocument.Parse(bodyStr);

                    try
                    {
                        document.RootElement.GetProperty("ID").GetString();
                        document.RootElement.GetProperty("PW").GetString();
                    }
                    catch
                    {
                        context.Response.StatusCode = (int)ErrorCode.AuthTokenFailWrongKeyword;
                        return;
                    }
                }
                context.Request.Body.Position = 0;

                // Call the next delegate/middleware in the pipeline
                await _next(context);
                return;
            }

            // https://devblogs.microsoft.com/dotnet/re-reading-asp-net-core-request-bodies-with-enablebuffering/
            // 다중 읽기 허용 함수 -> 파일 형식으로 임시 변환
            context.Request.EnableBuffering();
            string userAuthToken;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))
            {
                var bodyStr = await reader.ReadToEndAsync();

                // body String에 어떤 문자열도 없다면...
                if (string.IsNullOrEmpty(bodyStr))
                {
                    // http Response Code
                    context.Response.StatusCode = (int) ErrorCode.AuthTokenFailNoBody;
                    return;
                }

                var document = JsonDocument.Parse(bodyStr);

                string userId;
                try
                {
                    userId = document.RootElement.GetProperty("ID").GetString();
                    userAuthToken = document.RootElement.GetProperty("AuthToken").GetString();
                }
                catch
                {
                    context.Response.StatusCode = (int)ErrorCode.AuthTokenFailWrongKeyword;
                    return;
                }
                
                // redis에서 로그인 유저 정보 받아오기... 없으면 로그인 성공한 유저가 아님.
                var userInfo = await _redisDb.GetUserInfo(userId);
                if (userInfo == null)
                {
                    // http Response Code
                    context.Response.StatusCode = (int) ErrorCode.AuthTokenFailNoUser;
                    return;
                }

                // id, AuthToken 일치 여부 확인...
                if (String.CompareOrdinal(userInfo.AuthToken, userAuthToken) != 0)
                {
                    // http Response Code
                    context.Response.StatusCode = (int) ErrorCode.AuthTokenFailWrongAuthToken;
                    return;
                }

                // Redis를 활용한 트랜잭션... 중복 처리 예방... 처리되기 전에 메시지를 2번 보내는 현상을 막기 위함.
                if (!await _redisDb.SetNxAsync(userAuthToken))
                {
                    // http Response Code
                    context.Response.StatusCode = (int) ErrorCode.AuthTokenFailSetNx;
                    return;
                }
            }

            context.Request.Body.Position = 0;
            
            // Call the next delegate/middleware in the pipeline
            await _next(context);

            // 트랜잭션 해제(Redis 동기화 해제)
            await _redisDb.DelNxAsync(userAuthToken);
        }
    }
}