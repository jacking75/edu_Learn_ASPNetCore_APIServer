using APIServer.DTO;
using APIServer.DTO.Game;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Game;

[ApiController]
[Route("[controller]")]
public class MiniGameSetPlayChar : ControllerBase
{
    readonly ILogger<MiniGameSetPlayChar> _logger;
    readonly IGameService _gameService;

    public MiniGameSetPlayChar(ILogger<MiniGameSetPlayChar> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 미니게임 플레이 캐릭터 설정 API
    /// 미니게임 플레이에 사용할 캐릭터를 설정합니다.
    /// </summary>
    [HttpPost]
    public async Task<MiniGameSetPlayCharResponse> SetMiniGamePlayChar([FromHeader] HeaderDTO header, MiniGameSetPlayCharRequest request)
    {
        MiniGameSetPlayCharResponse response = new();

        response.Result = await _gameService.SetMiniGamePlayChar(header.Uid, request.GameKey, request.CharKey);

        _logger.ZLogInformation($"[MiniGameSetPlayChar] Uid : {header.Uid}, CharKey : {request.CharKey}");
        return response;
    }

}
