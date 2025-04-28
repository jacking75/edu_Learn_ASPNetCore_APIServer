
using GameShared.DTO;
using MatchAPIServer.Service;
using Microsoft.AspNetCore.Mvc;

namespace MatchAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchController : ControllerBase
{
	IMatchService _matchService;
	public MatchController(IMatchService matchService)
	{
		_matchService = matchService;
	}

	[HttpPost]
	public ErrorCodeDTO RequestMatch(MatchStartRequest request)
	{
		ErrorCodeDTO response = new();

		if (false == _matchService.AddUser(request.Uid))
		{
			response.Result = ErrorCode.MatchServerInternalError;
		}

		return response;
	}
}

