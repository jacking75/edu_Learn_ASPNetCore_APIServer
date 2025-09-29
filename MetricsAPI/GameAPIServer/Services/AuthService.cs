using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DAO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;
using GameAPIServer.Repository;

namespace GameAPIServer.Servicies;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IGameDb _gameDb;
    string _hiveServerAPIAddress;

    public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IGameDb gameDb)
    {
        _gameDb = gameDb;
        _logger = logger;
        _hiveServerAPIAddress = configuration.GetSection("HiveServerAddress").Value + "/verifytoken";
    }

    public async Task<ErrorCode> CreateAccount(string userID, string passWord)
    {
        var result = await _gameDb.CreateAccount(userID, passWord);
        return result;
    }
    
    public async Task<(ErrorCode, Int64, string)> Login(string userID, string passWord)
    {
        (var result, var uid) = await _gameDb.VerifyUser(userID, passWord);
        if (result != ErrorCode.None)
        {
            return (result, 0, "");
        }

        var token = Security.CreateAuthToken();
        result = ErrorCode.None;

        return (result, uid, token);
    }

    

}
