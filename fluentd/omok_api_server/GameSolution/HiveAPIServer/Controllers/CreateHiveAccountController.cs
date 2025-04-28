using System.Threading.Tasks;
using GameShared.DTO;
using HiveAPIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HiveAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateHiveAccount : ControllerBase
{
    readonly ILogger<CreateHiveAccount> _logger;
    readonly IHiveService _hiveService;
    public CreateHiveAccount(ILogger<CreateHiveAccount> logger, IHiveService hiveService)
    {
        _logger = logger;
		_hiveService = hiveService;
    }

    [HttpPost]
    public async Task<HiveRegisterResponse> Create([FromBody] HiveRegisterRequest request)
    {
		HiveRegisterResponse response = new();

        response.Result = await _hiveService.CreateAccount(request.Email, request.Password);

        return response;
    }
}

