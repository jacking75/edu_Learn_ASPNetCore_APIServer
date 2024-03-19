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
public class MiniGameSave : ControllerBase
{
    readonly ILogger<MiniGameSave> _logger;
    readonly IGameService _gameService;

    public MiniGameSave(ILogger<MiniGameSave> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 미니게임 플레이 정보 저장 API
    /// 미니게임 플레이 정보(아이템 사용량, 점수, 획득 보상 등)를 저장합니다.
    /// </summary>
    [HttpPost]
    public async Task<MiniGameSaveResponse> SaveMiniGame([FromHeader] HeaderDTO header, MiniGameSaveRequest request)
    {
        MiniGameSaveResponse response = new();

        response.Result = await _gameService.SaveMiniGame(header.Uid, request.GameKey, request.Score, request.Foods);
    
        _logger.ZLogInformation($"[MiniGameSave] Uid : {header.Uid}, GameKey : {request.GameKey}, Score : {request.Score}");
        return response;
    }
}
