using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : BaseController<LoginController>
{
	private readonly IUserService _service;
	public LoginController(ILogger<LoginController> logger, IUserService userService) : base(logger)
	{
		_service = userService;
	}

	[HttpPost]
	public async Task<LoginResponse> Login([FromBody] LoginRequest request)
	{
		var response = new LoginResponse();

		(response.Result, var (uid, token)) = await _service.LoginUser(request.PlayerId, request.Token);


		if (response.Result == ErrorCode.None && token != null)
		{
			response.AccessToken = token;
			response.Uid = uid;

            ActionLog(new
			{
				uid
			});
		}
		else
		{
			ErrorLog(ErrorCode.LoginFail, request);
		}

		return response;
	}
}
