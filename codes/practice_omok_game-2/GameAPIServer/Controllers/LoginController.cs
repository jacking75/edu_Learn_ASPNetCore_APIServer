using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class Login : ControllerBase
{
	private readonly ILogger<Login> _logger;
	private readonly IAuthService _authService;

	public Login(ILogger<Login> logger, IAuthService authService)
	{
		_logger = logger;
		_authService = authService;
	}

	/// <summary>
	/// 계정 로그인 
	/// </summary>
	/// <remarks>
	/// Hive 계정을 이용하여 게임 계정에 로그인하거나 새로운 게임 계정을 생성합니다.
	/// </remarks>
	[HttpPost]
	public async Task<LoginResponse> LoginUser(LoginRequest request)
	{
		LoginResponse response = new();

		var (errorCode, result) = await _authService.Login(request.PlayerId, request.HiveToken);
		if (errorCode != ErrorCode.None || result == null)
		{
			response.Result = errorCode;
			return response;
		}

		var (claimsPrincipal, authProperties) = _authService.RegisterUserClaims(result);
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
			claimsPrincipal, authProperties);
		
		_logger.ZLogInformation($"[User Login] UserUid : {result.Uid}");

		return response;
	}
}
