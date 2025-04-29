using GameAPIServer.DTO.Ranking;
using GameAPIServer.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace GameAPIServer.Controllers.Ranking;

[ApiController]
[Route("[controller]")]
public class TopRanking : ControllerBase
{
    readonly ILogger<TopRanking> _logger;
    readonly IMemoryDb _memoryDb;

    public TopRanking(ILogger<TopRanking> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    /// <summary>
    /// 상위 랭킹 조회 API
    /// 상위 100명의 랭킹을 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<RankingResponse> GetTopRanking()
    {
        RankingResponse response = new();
    
        (response.Result, response.RankingData) = await _memoryDb.GetTopRanking();

        _logger.ZLogInformation($"[TopRanking] GetTopRanking");
        return response;
    }
}
