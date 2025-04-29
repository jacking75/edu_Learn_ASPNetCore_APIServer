using HiveAPIServer.DTO;
using HiveAPIServer.DTO.Ranking;
using HiveAPIServer.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace HiveAPIServer.Controllers.Ranking;

[ApiController]
[Route("[controller]")]
public class UserRank : ControllerBase
{
    readonly ILogger<UserRank> _logger;
    readonly IMemoryDb _memoryDb;
    
    public UserRank(ILogger<UserRank> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    /// <summary>
    /// 유저 랭킹 API
    /// 자신의 등수를 가져옵니다.
    /// </summary>
    [HttpPost]
    public async Task<UserRankResponse> GetUserRank([FromHeader] HeaderDTO request)
    {
        var response = new UserRankResponse();

        (response.Result, response.Rank) = await _memoryDb.GetUserRankAsync(request.Uid);

        _logger.ZLogInformation($"[UserRank] Uid:{request.Uid}, Result:{response.Result}, Rank:{response.Rank}");
        return response;
    }
}
