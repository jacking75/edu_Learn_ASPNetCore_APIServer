using HiveServer.Models.DTO;
using HiveServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace HiveServer.Controllers;

[ApiController]
[Route("Auth/Login")]
public class LoginController
{
    readonly ILogger<LoginController> _logger;
    readonly IAuthService _authService;

    public LoginController(ILogger<LoginController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    public async Task<LoginResponse> Login(LoginRequest request) 
    {
        LoginResponse res = new();
        (res.Result, res.AccountUId, res.HiveToken) = await _authService.Login(request.EmailID, request.Password);
        return res;
    }
}
