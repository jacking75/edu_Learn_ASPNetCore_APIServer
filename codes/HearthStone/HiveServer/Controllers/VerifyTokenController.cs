using HiveServer.Repository.Interface;
using HiveServer.Models.DTO;
using HiveServer.Services.Interface;
using ZLogger;
using Microsoft.AspNetCore.Mvc;

namespace HiveServer.Controllers;

[ApiController]
[Route("Auth/VerifyToken")]
public class VerifyTokenController : ControllerBase
{
    readonly ILogger<VerifyTokenController> _logger;
    readonly IAuthService _authService;
    
    public VerifyTokenController(ILogger<VerifyTokenController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    public async Task<VerifyTokenResponse> VerifyToken(VerifyTokenRequest request)
    {
        VerifyTokenResponse res = new();
        res.Result = await _authService.VerifyToken(request.AccountUId, request.HiveToken);
        return res;
    }
}
