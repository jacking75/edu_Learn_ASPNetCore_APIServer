using MatchServer.Models;
using MatchServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace MatchServer.Controllers;

[ApiController]
[Route("match")]
public class MatchController : ControllerBase
{
    readonly IMatchService matchService;

    public MatchController(IMatchService matchService)
    {
        this.matchService = matchService;
    }

    [HttpPost("add")]
    public async Task<MatchAddResponse> AddMatch([FromBody] MatchAddReqeust request)
    {
        MatchAddResponse result = new MatchAddResponse();
        result.Result = matchService.AddUser(request.AccountUid);
        return result;
    }   
}

