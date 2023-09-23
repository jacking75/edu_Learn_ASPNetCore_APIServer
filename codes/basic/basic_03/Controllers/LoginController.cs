using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;


[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    [HttpPost]
    public async Task<PkLoginResponse> Post(PkLoginRequest request)
    {
        var response = new PkLoginResponse();
        response.Result = ErrorCode.None;

        using (var db = await DBManager.GetGameDBQuery())
        {
            var userInfo = await db.Query("account").Where("Email", request.Email).FirstOrDefaultAsync<DBUserInfo>();

            if (userInfo == null || string.IsNullOrEmpty(userInfo.HashedPassword))
            {
                response.Result = ErrorCode.Login_Fail_NotUser;
                return response;
            }
                        
            var hashingPassword = Security.MakeHashingPassWord(userInfo.SaltValue, request.Password);

            Console.WriteLine($"[Request Login] Email:{request.Email}, request.Password:{request.Password},  saltValue:{userInfo.SaltValue}, hashingPassword:{hashingPassword}");
            if (userInfo.HashedPassword != hashingPassword)
            {
                response.Result = ErrorCode.Login_Fail_PW;
                return response;
            }

            db.Dispose();
        }

        
        string tokenValue = Security.CreateAuthToken();
        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<string>(DBManager.RedisConn, request.Email, idDefaultExpiry);
        await redisId.SetAsync(tokenValue);

        response.Authtoken = tokenValue;
        return response;
    }
}


public class PkLoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class PkLoginResponse
{
    public ErrorCode Result { get; set; }
    public string Authtoken { get; set; }
}

class DBUserInfo
{
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public string SaltValue { get; set; }
}
