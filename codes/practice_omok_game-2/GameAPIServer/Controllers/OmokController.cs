using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class Omok : SecureController<Omok>
{
	private readonly IOmokService _omokService;

	public Omok(ILogger<Omok> logger, IOmokService gameService) : base(logger)
	{
		_omokService = gameService;
	}

	/// <summary>
	/// 게임 참여하기
	/// </summary>
	/// <remarks>
	/// 게임룸에 입장 합니다
	/// </remarks>
	[HttpPost("enter")]
	public async Task<EnterGameResponse> EnterGame()
	{
		var response = new EnterGameResponse();

		Int64 uid = GetUserUid();

		(response.Result, response.GameData) = await _omokService.EnterGame(uid);

		if (response.Result != ErrorCode.None)
		{
			_logger.ZLogError($"[Omok EnterGame] Error: {response.Result}");
			return response;
		}

		return response;
	}

	/// <summary>
	/// 게임 확인
	/// </summary>
	/// <remarks>
	/// 게임 확인을 통해 게임 참여 가능여부 등을 체크 합니다
	/// </remarks>
	[HttpPost("peek")]
	public async Task<PeekGameResponse> CheckGame()
	{
		var response = new PeekGameResponse();

		Int64 uid = GetUserUid();

		(response.Result, response.GameData) = await _omokService.PeekGame(uid);

		if (response.Result != ErrorCode.None)
		{
			_logger.ZLogError($"[Omok CheckGame] Error: {response.Result}");
		}

		return response;
	}

	/// <summary>
	/// 돌 두기
	/// </summary>
	/// <remarks>
	/// 진행중인 게임에서 돌을 둡니다
	/// </remarks>
	[HttpPost("stone")]
	public async Task<PlayOmokResponse> PutGameStone([FromBody] PlayOmokRequest request)
	{
		var response = new PlayOmokResponse();

		Int64 uid = GetUserUid();

		(response.Result, response.GameData) = await _omokService.SetOmokStone(uid, request.PosX, request.PosY);

		if (response.Result != ErrorCode.None)
		{
			_logger.ZLogError($"[Omok PutGameStone] Error: {response.Result}");
		}

		return response;
	}

}
