using GameServer.Controllers.Contents;
using Microsoft.AspNetCore.Mvc;
using GameServer.Models;
using GameServer.Models.DTO;
using GameServer.Services.Interface;

namespace GameServer.Controllers;

[ApiController]
[Route("auth")]
public class LogOutController : ControllerBase
{
    readonly ILogger<LogOutController> _logger;
    readonly IAuthService _authService;

    [HttpPost("logout")]
    public async Task LogOut([FromHeader] HeaderDTO header)
    {
        await _authService.LogOut(header.AccountUid);
        return;
    }
}

