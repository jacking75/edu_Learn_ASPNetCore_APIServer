using Microsoft.AspNetCore.Mvc;
using TestApiServer.DTO;
using TestApiServer.ServerCore;
using TestApiServer.Services.Interfaces;

namespace TestApiServer.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : Controller<LoginController>
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

		var user = _service.Login(request.Username, request.Password);

		if (user == null)
		{
			response.Result = ErrorCode.LoginInvalidRequest;
			LogError(nameof(Login), "Invalid username or password", 
				ErrorCode.LoginInvalidRequest);

			return response;
		}

		LogInfo(nameof(Login), $"Id: {user.Id} logged in");

		return response;
	}
}

