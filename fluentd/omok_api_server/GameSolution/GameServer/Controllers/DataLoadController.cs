using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared.ServerCore;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class DataLoadController : BaseController<DataLoadController>
{
	private readonly IDataLoadService _dataLoadService;
	public DataLoadController(ILogger<DataLoadController> logger, IDataLoadService dataLoadService) : base(logger)
	{
		_dataLoadService = dataLoadService;
	}

	[HttpPost]
	public async Task<UserDataResponse> GetUserData()
	{
		UserDataResponse response = new();

		Int64 uid = GetUserUid();

		(response.Result, response.UserData) = await _dataLoadService.LoadUserData(uid, true, true);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, uid);
			return response;
		}

		return response;
	}

	[HttpPost("info")]
	public async Task<UserDataResponse> GetUserInfo([FromBody] UserInfoRequest request)
	{
		UserDataResponse response = new();

		(response.Result, response.UserData) = await _dataLoadService.LoadUserData(request.Uid, false, false);

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, request);
			return response;
		}

		return response;
	}

	[HttpPost("master")]
	public async Task<MasterDataResponse> GetMasterData()
	{
		MasterDataResponse response = new();

		(response.Result, response.MasterData) = _dataLoadService.LoadMasterData();

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result);
			return response;
		}

		return response;
	}
}
