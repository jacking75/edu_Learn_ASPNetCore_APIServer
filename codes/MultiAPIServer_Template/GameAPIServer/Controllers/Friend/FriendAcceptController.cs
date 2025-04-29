using HiveAPIServer.DTO;
using HiveAPIServer.DTO.Friend;
using HiveAPIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace HiveAPIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendAccept : ControllerBase
{
    readonly ILogger<FriendAccept> _logger;
    readonly IFriendService _friendService;

    public FriendAccept(ILogger<FriendAccept> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 친구 요청을 수락하는 API </br>
    /// 요청이 왔는지, 이미 친구 인지 확인 후 친구 요청을 수락합니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendAcceptResponse> AcceptFriend([FromHeader] HeaderDTO header, FriendAcceptRequest request)
    {
        FriendAcceptResponse response = new();
        
        response.Result = await _friendService.AcceptFriendReq(header.Uid, request.FriendUid);

        _logger.ZLogInformation($"[FriendAccept] Uid : {header.Uid}");
        return response;
    }
}
