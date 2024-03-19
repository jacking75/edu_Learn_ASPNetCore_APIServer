using System;
using APIServer.Model.DTO;
using APIServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using APIServer.Services;
using Microsoft.Extensions.Configuration;
using ZLogger;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class VerifyToken : ControllerBase
{
    readonly string _saltValue;
    readonly ILogger<VerifyToken> _logger;
    readonly IHiveDb _hiveDb;

    public VerifyToken(ILogger<VerifyToken> logger, IHiveDb hiveDb, IConfiguration config)
    {
        _saltValue = config.GetSection("TokenSaltValue").Value;
        _logger = logger;
        _hiveDb = hiveDb;
    }

    [HttpPost]
    public VerifyTokenResponse Verify([FromBody] VerifyTokenBody request) {
        VerifyTokenResponse response = new();

        if (Security.MakeHashingToken(_saltValue, request.PlayerId)!=request.HiveToken)
        {
            _logger.ZLogDebug(
                $"[AccoutDb.CreateAccount] ErrorCode: {ErrorCode.VerifyTokenFail}");
            response.Result =  ErrorCode.VerifyTokenFail;
        }
        return response;
    }
}
