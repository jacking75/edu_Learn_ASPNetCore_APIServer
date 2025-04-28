using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HiveAPIServer.Services;
using HiveAPIServer.Model.DAO;
using GameShared.DTO;

namespace HiveAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginHive : ControllerBase
{
	readonly ILogger<LoginHive> _logger;
	readonly IHiveService _hiveService;
	
	public LoginHive(ILogger<LoginHive> logger, IHiveService hiveService)
	{
		_logger = logger;
		_hiveService = hiveService;
	}

	[HttpPost]
	public async Task<HiveLoginResponse> Login([FromBody] HiveLoginRequest request)
	{
		HiveLoginResponse response = new();

		(response.Result, Token data) = await _hiveService.LoginHive(request.Email, request.Password);

		if (ErrorCode.None == response.Result)
		{
			response.Token = data.token;
			response.PlayerId = data.player_id;
		}

		return response;
	}
}
