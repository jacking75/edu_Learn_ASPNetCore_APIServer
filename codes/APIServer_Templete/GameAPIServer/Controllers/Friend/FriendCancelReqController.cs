using APIServer.DTO;
using APIServer.DTO.Friend;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendCancelReq : ControllerBase
{
    readonly ILogger<FriendCancelReq> _logger;
    readonly IFriendService _friendService;

    public FriendCancelReq(ILogger<FriendCancelReq> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 친구 요청 취소 API
    /// 보낸 친구 요청을 취소합니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendCancelResponse> CancelFriendReq([FromHeader] HeaderDTO header, FriendCancelRequest request)
    {
        FriendCancelResponse response = new();

        response.Result = await _friendService.CancelFriendReq(header.Uid, request.FriendUid);

        _logger.ZLogInformation($"[FriendCancelReq] Uid : {header.Uid}");
        return response;
    }
}
