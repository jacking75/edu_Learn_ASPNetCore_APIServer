using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared.ServerCore;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class MatchController : BaseController<MatchController>
{
	private readonly IMatchService _matchService;
	public MatchController(ILogger<MatchController> logger, IMatchService matchService) : base(logger)
	{
		_matchService = matchService;
	}

	[HttpPost("start")]
	public async Task<ErrorCodeDTO> RequestMatch()
	{
		var response = new ErrorCodeDTO();

		Int64 uid = GetUserUid();

		response.Result = await _matchService.StartMatch(uid);

		if (response.Result == ErrorCode.None)
		{
			ActionLog(new
			{
				uid
			}, "MatchStart");
		}

		return response;
	}

	[HttpPost("check")]
	public async Task<ErrorCodeDTO> CheckMatch()
	{
		var response = new ErrorCodeDTO();

		Int64 uid = GetUserUid();

		(response.Result, var guid )= await _matchService.CheckMatch(uid);

		if (response.Result == ErrorCode.None)
		{
			ActionLog(new
			{
				uid
			}, "MatchComplete");
		}

		return response;
	}
}
