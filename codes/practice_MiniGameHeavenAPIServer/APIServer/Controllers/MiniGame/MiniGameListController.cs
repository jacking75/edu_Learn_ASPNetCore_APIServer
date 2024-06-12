using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;
using APIServer.DTO;
using APIServer.DTO.Game;

namespace APIServer.Controllers.Game;

[ApiController]
[Route("[controller]")]
public class MiniGameList : ControllerBase
{
    readonly ILogger<MiniGameList> _logger;
    readonly IGameService _gameService;

    public MiniGameList(ILogger<MiniGameList> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 보유 미니게임 정보 조회 API
    /// 보유한 미니게임의 목록과 정보를 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<MiniGameListResponse> GetMiniGameList([FromHeader] HeaderDTO header)
    {
        MiniGameListResponse response = new();

        (response.Result, response.MiniGameList) = await _gameService.GetMiniGameList(header.Uid);

        _logger.ZLogInformation($"[MiniGameList] Uid : {header.Uid}");
        return response;
    }
}
