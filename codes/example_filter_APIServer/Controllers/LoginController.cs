using System.Collections.Generic;
using System;
using System.Threading.Tasks;

using APIServer.Models.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ZLogger;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    private readonly ILogger<Login> _logger;
    private readonly MasterData.IManager _masterDataMgr;

    public Login(ILogger<Login> logger, MasterData.IManager masterDataMgr)
    {
        _logger = logger;
        _masterDataMgr = masterDataMgr;
    }

    [HttpPost]
    public async Task<ResLoginDTO> Post(ReqLoginDTO reqDTO)
    {
        // TODO: do something

 
        var resDTO = new ResLoginDTO()
        {
            userUid = 10000,
            sessionKey = "TEST_SESSION_KEY",
            sessionExpireSecond = 3600
        };

        return resDTO;
    }

    
}

