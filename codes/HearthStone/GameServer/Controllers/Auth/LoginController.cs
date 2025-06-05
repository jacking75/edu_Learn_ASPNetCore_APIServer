using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTO;
using GameServer.Services.Interface;
using ZLogger;

namespace GameServer.Controllers;

[ApiController]
[Route("auth/login")]
public class LoginController : ControllerBase
{
    readonly ILogger<LoginController> _logger;
    readonly IAuthService _authService;
    readonly IGameService _gameService;

    public LoginController(ILogger<LoginController> logger, IAuthService authService,IGameService gameService)
    {
        _logger = logger;
        _authService = authService;
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<LoginResponse> Login(LoginRequest request) 
    {
        LoginResponse response = new();
        (response.Result, response.Token) = await _authService.Verify(request.AccountUId, request.HiveToken);
        response.AccountUId = request.AccountUId;
        return response;
    }
}
