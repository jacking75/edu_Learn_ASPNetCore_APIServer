using System;
using System.Threading.Tasks;
using APIServer.ModelReqRes;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccount : ControllerBase
{
    private readonly RedisDb _redisDb;
    private readonly ILogger<CreateAccount> _logger;

    public CreateAccount(ILogger<CreateAccount> logger, RedisDb redisDb)
    {
        _logger = logger;
        _redisDb = redisDb;
    }

    [HttpPost]
    public async Task<PkCreateAccountRes> Post(PkCreateAccountReq request)
    {
        PkCreateAccountRes response = new();

        bool ret = await _redisDb.CreateAccountAsync(request.Email, request.Password);
        if (ret == false)
        {
            response.Result = 1;
            return response;
        }

        Console.WriteLine($"CreateAccount Success");
        return response;
    }
}
