using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class UserData : SecureController<UserData>
{
	private readonly IDataLoadService _dataLoadService;
	private readonly IAuthService _userService;

	public UserData(ILogger<UserData> logger, IDataLoadService dataLoadService, IAuthService authService) : base(logger)
	{
		_dataLoadService = dataLoadService;
		_userService = authService;
	}

	[HttpGet]
	public async Task<UserDataLoadResponse> GetUserInfo()
	{
		UserDataLoadResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result, response.UserData) = await _dataLoadService.LoadUserData(uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[UserData Load] ErrorCode : {response.Result}");
			return response;
		}

		return response;
	}

	[HttpPost("items")]
	public async Task<UserItemLoadResponse> GetUserItem()
	{
		UserItemLoadResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result, response.ItemData) = await _dataLoadService.LoadItemData(uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[UserData Load] ErrorCode : {response.Result}");
			return response;
		}

		return response;
	}

	[HttpPost("profile")]
	public async Task<UserProfileLoadResponse> GetUserProfile([FromBody] UserProfileLoadRequest request)
	{
		UserProfileLoadResponse response = new();

		(response.Result, response.ProfileData) = await _dataLoadService.LoadUserProfile(request.Uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[UserData Load] ErrorCode : {response.Result}");
			return response;
		}

		return response;
	}

	[HttpPost("update/nickname")]
	public async Task<UpdateNicknameResponse> ChangeNickname([FromBody] UpdateNicknameRequest request)
	{
		UpdateNicknameResponse response = new();

		var uid = GetUserUid();

		response.Result = await _userService.UpdateNickname(uid, request.Nickname);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[UserData Load] ErrorCode : {response.Result}");
			return response;
		}

		return response;
	}
}
