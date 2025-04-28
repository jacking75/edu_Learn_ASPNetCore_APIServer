using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared.ServerCore;

namespace GameServer.Controllers;


[Route("[controller]")]
[ApiController]
public class LogoutController : BaseController<LogoutController>
{
	private readonly IUserService _service;

	public LogoutController(ILogger<LogoutController> logger, IUserService userService) : base(logger)
	{
		_service = userService;
	}

	[HttpPost]
	public async Task<LogoutResponse> Logout()
	{
		var response = new LogoutResponse();

		response.Result = await _service.LogoutUser(GetUserUid());

		return response;
	}
}
