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
public class FriendSendReq : ControllerBase
{
    readonly ILogger<FriendSendReq> _logger;
    readonly IFriendService _friendService;

    public FriendSendReq(ILogger<FriendSendReq> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 친구 요청 API </br>
    /// 상대방에게 친구 요청을 보냅니다.
    /// </summary>
    [HttpPost]
    public async Task<SendFriendReqResponse> SendFriendReq([FromHeader] HeaderDTO header, SendFriendReqRequest request)
    {
        SendFriendReqResponse response = new();

        response.Result = await _friendService.SendFriendReq(header.Uid, request.FriendUid);

        _logger.ZLogInformation($"[FriendSendReq] Uid : {header.Uid}");
        return response;
    }
}


