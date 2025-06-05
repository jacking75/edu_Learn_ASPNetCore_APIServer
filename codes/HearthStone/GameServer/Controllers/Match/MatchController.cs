using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTO;
using GameServer.Services.Interface;

namespace GameServer.Controllers;

[ApiController]
[Route("match")]
public class MatchController : ControllerBase
{
    readonly IMatchService _matchService;

    public MatchController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpPost("add")]
    public async Task<MatchAddResponse> AddMatch([FromHeader] HeaderDTO header)
    {
        MatchAddResponse result = new MatchAddResponse();
        result.Result = await _matchService.AddUser(header.AccountUid);
        return result;
    }

    [HttpPost("waiting")]
    public async Task<MatchWaitingResponse> GetWaiting([FromHeader] HeaderDTO header)
    {
        MatchWaitingResponse response = new MatchWaitingResponse();
        (response.Result, response.MatchGUID) = await _matchService.GetMatchGUID(header.AccountUid);
        return response;
    }

    [HttpPost("status")]
    public async Task<MatchStatusResponse> GetMatchStatus([FromHeader] HeaderDTO header, [FromBody] MatchStatusRequest request)
    {
        MatchStatusResponse result = new MatchStatusResponse();
        (result.Result, result.GameInfo) = await _matchService.GetMatch(request.MatchGUID, header.AccountUid);
        return result;
    }
}
