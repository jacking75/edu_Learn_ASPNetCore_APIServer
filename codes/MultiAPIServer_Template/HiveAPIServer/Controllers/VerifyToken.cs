using MatchAPIServer.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MatchAPIServer.Services;


namespace MatchAPIServer.Controllers;

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
