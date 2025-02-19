using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using APIServer.Model.DTO;
using APIServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZLogger;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateHiveAccount : ControllerBase
{
    readonly ILogger<CreateHiveAccount> _logger;
    readonly IHiveDb _hiveDb;

    public CreateHiveAccount(ILogger<CreateHiveAccount> logger, IHiveDb hiveDb)
    {
        _logger = logger;
        _hiveDb = hiveDb;
    }

    [HttpPost]
    public async Task<CreateHiveAccountResponse> Create([FromBody]CreateHiveAccountRequest request)
    {
        CreateHiveAccountResponse response = new();

        response.Result = await _hiveDb.CreateAccountAsync(request.UserID, request.Password);

        return response;
    }

    

    
}

