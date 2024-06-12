using System;
using APIServer.Model.DTO;
using APIServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using APIServer.Services;
using Microsoft.Extensions.Configuration;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginHive : ControllerBase
{
    readonly string _saltValue;
    readonly ILogger<LoginHive> _logger;
    readonly IHiveDb _hiveDb;
    
    public LoginHive(ILogger<LoginHive> logger, IHiveDb hiveDb, IConfiguration config)
    {
        _saltValue = config.GetSection("TokenSaltValue").Value;
        _logger = logger;
        _hiveDb = hiveDb;
    }

    [HttpPost]
    public async Task<LoginHiveResponse> Login([FromBody] LoginHiveRequest request)
    {
        LoginHiveResponse response = new();
        (response.Result, response.PlayerId) = await _hiveDb.VerifyUser(request.UserID, request.Password);
        response.HiveToken = Security.MakeHashingToken(_saltValue, response.PlayerId);

        return response;

    }
}
