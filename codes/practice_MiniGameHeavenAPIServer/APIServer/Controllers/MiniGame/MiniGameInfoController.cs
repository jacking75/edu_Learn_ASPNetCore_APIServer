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
public class MiniGameInfo : ControllerBase
{
    readonly ILogger<MiniGameInfo> _logger;
    readonly IGameService _gameService;

    public MiniGameInfo(ILogger<MiniGameInfo> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 미니게임 정보 조회 API
    /// 미니게임의 정보(아이템 보유 현황, 플레이 캐릭터(코스튬,스킨), 최고점수 등)을 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<MiniGameInfoResponse> GetMiniGameInfo([FromHeader] HeaderDTO header, MiniGameInfoRequest request)
    {
        MiniGameInfoResponse response = new();

        (response.Result, response.MiniGameInfo) = await _gameService.GetMiniGameInfo(header.Uid, request.GameKey);
    
        _logger.ZLogInformation($"[MiniGameInfo] Uid : {header.Uid}, GameKey : {request.GameKey}");
            return response;
    }
}
