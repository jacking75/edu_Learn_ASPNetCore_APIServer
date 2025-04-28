using System.Threading.Tasks;
using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccountController : ControllerBase
{
    readonly ILogger<CreateAccountController> _logger;
    readonly IAuthService _authService;

    public CreateAccountController(ILogger<CreateAccountController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost]
    public async Task<CreateHiveAccountResponse> Create([FromBody]CreateHiveAccountRequest request)
    {
        CreateHiveAccountResponse response = new();

        response.Result = await _authService.CreateAccount(request.UserID, request.Password);

        return response;
    }

    

    
}

