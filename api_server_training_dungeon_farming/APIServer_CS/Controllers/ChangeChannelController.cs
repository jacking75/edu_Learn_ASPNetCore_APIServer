using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class ChangeChannel : ControllerBase
{
    private readonly ILogger<ChangeChannel> _logger;

    private readonly IMemoryDb _memoryDb;

    private readonly ChannelUserManager _channelUserMgr;

    public ChangeChannel(ILogger<ChangeChannel> logger, IMemoryDb memoryDb, ChannelUserManager channelUserMgr)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _channelUserMgr = channelUserMgr;
    }


    [HttpPost]
    public async Task<ChangeChannelResponse> Post(ChangeChannelRequest request)
    {
        var response = new ChangeChannelResponse();
        var userInfo = (CertifiedUser)HttpContext.Items[nameof(CertifiedUser)]!;


        // 1. 요청 채널 입장 시도
        var error = _channelUserMgr.Enter(request.ChannelNumber, userInfo.UserId);
        if (error != ErrorCode.None)
        {
            response.Result = error;
            return response;
        }


        // 2. 유저 채널 업데이트
        var existChannelNumber = userInfo.ChannelNumber;
        userInfo.ChannelNumber = request.ChannelNumber;
        if (await UpdateUserInfo(request.Email, userInfo) == false)
        {
            // 채널 매니저에 추가했던 유저 정보 삭제
            _channelUserMgr.Leave(request.ChannelNumber, userInfo.UserId);

            response.Result = ErrorCode.FailedRedisRegist;
            return response;
        }


        // 3. 기존 채널에서 유저 정보 삭제
        _channelUserMgr.Leave(existChannelNumber, userInfo.UserId);




        return response;
    }


    private async Task<bool> UpdateUserInfo(string email, CertifiedUser info)
    {
        var error = await _memoryDb.UpdateCertifiedUserInfo(email, info);
        if (error != ErrorCode.None)
        {
            return false;
        }

        return true;
    }



}