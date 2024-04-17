using System;
using System.Threading.Tasks;
using APIServer.Model.DTO;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    private readonly IAccountDb _accountDb;
    private readonly IMemoryDb _memoryDb;
    private readonly ILogger<Login> _logger;

    public Login(ILogger<Login> logger, IAccountDb accountDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _accountDb = accountDb;
        _memoryDb = memoryDb;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequest request)
    {
        LoginResponse response = new();

        // ID, PW 검증
        (ErrorCode errorCode, long accountId) = await _accountDb.VerifyUser(request.Email, request.Password);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }


        string authToken = Security.CreateAuthToken();
        errorCode = await _memoryDb.RegistUserAsync(request.Email, authToken, accountId);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"EventType:{EventIdDic[EventType.Login]}, email:{request.Email}, AuthToken:{authToken}, AccountId:{accountId}");

        response.AuthToken = authToken;
        return response;
    }
}
