using System.Threading.Tasks;
using GameAPIServer.DTO;
using GameAPIServer.DTO.Auth;
using GameAPIServer.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace GameAPIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class Logout : ControllerBase
{
    readonly IMemoryDb _memoryDb;
    readonly ILogger<Logout> _logger;

    public Logout(ILogger<Logout> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    /// <summary>
    /// 로그아웃 API </br>
    /// 해당 유저의 토큰을 Redis에서 삭제합니다.
    /// </summary>
    [HttpPost]
    public async Task<LogoutResponse> DeleteUserToken([FromHeader] HeaderDTO request)
    {
        LogoutResponse response = new();
        var errorCode = await _memoryDb.DelUserAuthAsync(request.Uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[Logout] Uid : {request.Uid}");
        return response;
    }
}
