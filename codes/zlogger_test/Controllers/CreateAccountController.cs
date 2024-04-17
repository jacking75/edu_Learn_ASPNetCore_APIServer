using System;
using System.Threading.Tasks;
using APIServer.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccount : ControllerBase
{
    private readonly ILogger<CreateAccount> _logger;

    public CreateAccount(ILogger<CreateAccount> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<CreateAccountRes> Post(CreateAccountReq request)
    {
        var response = new CreateAccountRes();

        /*
         {"Timestamp":"2024-04-17T13:46:25.3337031+09:00","LogLevel":"Information","Category":"APIServer.Controllers.CreateAccount","Message":"EventType:CreateAccount, Email:jacking751@gmail.com","request.Email":"jacking751@gmail.com"}

        {"Timestamp":"2024-04-17T13:46:25.3340705+09:00","LogLevel":"Information","Category":"APIServer.Controllers.CreateAccount","Message":"[EventType:CreateAccount] {\u0022Email\u0022:\u0022jacking751@gmail.com\u0022,\u0022Password\u0022:\u0022123qwe\u0022}","request":{"Email":"jacking751@gmail.com","Password":"123qwe"}}

        {"Timestamp":"2024-04-17T13:46:25.3354687+09:00","LogLevel":"Information","Category":"APIServer.Controllers.CreateAccount","Message":"CreateAccount: {\u0022Email\u0022:\u0022jacking751@gmail.com\u0022}","EventIdDic[EventType.CreateAccount]":{"Id":101,"Name":"CreateAccount"},"new { request.Email }":{"Email":"jacking751@gmail.com"}} 
         */
        _logger.ZLogInformation($"EventType:CreateAccount, Email:{request.Email}");
        _logger.ZLogInformation($"[EventType:CreateAccount] {request:json}");
        _logger.ZLogInformation($"{EventIdDic[EventType.CreateAccount]}: {new { request.Email }:json}");
        return response;
    }
}
