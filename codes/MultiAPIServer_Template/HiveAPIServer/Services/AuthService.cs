using HiveAPIServer.Controllers;
using HiveAPIServer.Repository;
using HiveAPIServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ZLogger;

namespace HiveAPIServer.Services;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IHiveDb _hiveDb;

    public AuthService(ILogger<AuthService> logger, IHiveDb hiveDb)
    {
        _hiveDb = hiveDb;
        _logger = logger;        
    }


    public async Task<ErrorCode> CreateAccount(string userID, string passWord)
    {
        var result = await _hiveDb.CreateAccount(userID, passWord);
        return result;
    }

    public async Task<(ErrorCode, Int64, string)> Login(string userID, string passWord)
    {
        (var result, var playerId) = await _hiveDb.VerifyUser(userID, passWord);
        if (result != ErrorCode.None)
        {
            return (result, 0, "");
        }


        var token = Security.CreateAuthToken();

        //TODO: 토큰을 Redis 저장한다
        
        return (result, playerId, token);
    }

    public async Task<ErrorCode> VerifyToken(Int64 playerId, string token)
    {
        //TODO: 토큰을 Redis에서 조회한 후 결과를 반환한다

        await Task.CompletedTask;
        return ErrorCode.None;        
    }

    
}
