using System.Threading.Tasks;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LogoutController : ControllerBase
{
    readonly IMemoryDb _memoryDb;
    readonly ILogger<LogoutController> _logger;

    public LogoutController(ILogger<LogoutController> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    /// <summary>
    /// 로그아웃 API </br>
    /// 해당 유저의 토큰을 Redis에서 삭제합니다.
    /// </summary>
    [HttpPost]
    public async Task<LogoutResponse> DeleteUserToken([FromHeader] Header request)
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
