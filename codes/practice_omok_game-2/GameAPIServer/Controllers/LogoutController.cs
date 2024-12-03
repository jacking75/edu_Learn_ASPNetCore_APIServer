using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class Logout : ControllerBase
{
	private readonly ILogger<Logout> _logger;
	private readonly IAuthService _authService;

	public Logout(ILogger<Logout> logger, IAuthService authService)
	{
		_logger = logger;
		_authService = authService;
	}

	/// <summary>
	/// 계정 로그아웃 
	/// <remarks>
	/// 쿠키 및 토큰을 삭제하여 로그아웃합니다.
	/// </remarks>
	[HttpPost]
	public async Task<LogoutResponse> LogoutUser()
	{
		LogoutResponse response = new();

		var uidClaim = User.FindFirst("uid")?.Value;
		if (string.IsNullOrEmpty(uidClaim))
		{
			response.Result = ErrorCode.ClaimAuthTokenUserNotFound;
			return response;
		}

		response.Result = await _authService.Logout(uidClaim);
		if (response.Result != ErrorCode.None)
		{
			_logger.ZLogError($"[User Logout Fail] Useruid : {uidClaim}");
		}

		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		return response;
	}
}
