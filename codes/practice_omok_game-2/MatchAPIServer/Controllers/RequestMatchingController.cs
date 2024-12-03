
using MatchAPIServer.Service;
using Microsoft.AspNetCore.Mvc;

namespace MatchAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestMatching : ControllerBase
{
	IMatchService _matchService;
	public RequestMatching(IMatchService matchService)
	{
		_matchService = matchService;
	}

	[HttpPost]
	public ErrorCodeDTO Post(MatchRequest request)
	{
		ErrorCodeDTO response = new();

		if (false == _matchService.AddUser(request.Uid))
		{
			response.Result = ErrorCode.MatchServerInternalError;
		}

		return response;
	}
}

