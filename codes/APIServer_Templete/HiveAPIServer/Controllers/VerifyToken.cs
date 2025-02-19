using System;
using APIServer.Model.DTO;
using APIServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using APIServer.Services;
using Microsoft.Extensions.Configuration;
using ZLogger;
using HiveAPIServer.Services;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<VerifyTokenResponse> VerifyToken([FromBody] VerifyTokenRequest request) 
    {
        VerifyTokenResponse response = new();

        response.Result = await _authService.VerifyToken(request.PlayerId, request.HiveToken);

        return response;
    }
}
