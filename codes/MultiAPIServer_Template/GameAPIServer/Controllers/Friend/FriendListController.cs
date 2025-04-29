using MatchAPIServer.DTO;
using MatchAPIServer.DTO.Friend;
using MatchAPIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace MatchAPIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendList : ControllerBase
{
    readonly ILogger<FriendList> _logger;
    readonly IFriendService _friendService;

    public FriendList(ILogger<FriendList> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 친구 목록 조회 API
    /// 보낸 친구 요청, 받은 친구 요청, 친구 목록을 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendListResponse> GetFriendList([FromHeader] HeaderDTO header)
    {
        FriendListResponse response = new();

        (response.Result, response.FriendList) = await _friendService.GetFriendList(header.Uid);

        _logger.ZLogInformation($"[FriendList] Uid : {header.Uid}");
        return response;
    } 
}
