using System.Threading.Tasks;
using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;


namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccountController : ControllerBase
{
    readonly ILogger<CreateAccountController> _logger;
    readonly IAuthService _authService;
    readonly IGameService _gameService;

    public CreateAccountController(ILogger<CreateAccountController> logger, IAuthService authService, IGameService gameService)
    {
        _logger = logger;
        _authService = authService;
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<CreateHiveAccountResponse> Create([FromBody]CreateHiveAccountRequest request)
    {
        CreateHiveAccountResponse response = new();
        Int64 uid = 0;

        (response.Result, uid) = await _authService.CreateAccount(request.UserID, request.Password);


        if (response.Result != ErrorCode.None)
        {
            response.Result = await _gameService.InitNewUserGameData(uid);
        }


        return response;
    }

    

    
}

