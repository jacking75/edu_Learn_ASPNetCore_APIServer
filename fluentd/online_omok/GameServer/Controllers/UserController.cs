using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : BaseController<UserController>
{
	private readonly IUserService _userService;

	public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
	{
		_userService = userService;
	}

    [HttpPost("nickname")]
	public async Task<NicknameUpdateResponse> UpdateNickname(NicknameUpdateRequest request)
	{
		NicknameUpdateResponse response = new()
		{
			Result = await _userService.UpdateNickname(GetUserUid(), request.Nickname)
		};

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, request);
			return response;
		}

		return response;
	}
}

