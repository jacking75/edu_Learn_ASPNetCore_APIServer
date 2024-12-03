using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Match : SecureController<Match>
{
	private readonly IMatchService _matchService;

	public Match(ILogger<Match> logger, IMatchService matchService) : base(logger)
	{
		_matchService = matchService;
	}

	/// <summary>
	/// 매치 요청
	/// </summary>
	/// <remarks>
	/// 매칭을 시작 요청합니다
	/// </remarks>
	[HttpPost("start")]
	public async Task<CheckMatchResponse> StartMatch()
	{
		CheckMatchResponse response = new();

		Int64 Uid = GetUserUid();

		response.Result = await _matchService.StartMatch(Uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[CheckMatch] Error: {response.Result}");
		}

		return response;
	}

    /// <summary>
    /// 매치 상태 체크
    /// </summary>
    /// <remarks>
    /// 매치 완료 여부를 체크합니다.
    /// </remarks>
    [HttpPost("check")]
	public async Task<CheckMatchResponse> CheckMatchCompletion()
	{
		CheckMatchResponse response = new();

		Int64 Uid = GetUserUid();

		(response.Result, var matchData) = await _matchService.CheckMatch(Uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[MatchCheck] Error: {response.Result}");
			return response;
		}

		response.MatchData = matchData;
		return response;
	}

}
