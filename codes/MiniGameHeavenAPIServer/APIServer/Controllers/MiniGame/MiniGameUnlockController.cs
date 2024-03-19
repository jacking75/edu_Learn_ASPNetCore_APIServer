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
public class MiniGameUnlock : ControllerBase
{
    readonly ILogger<MiniGameUnlock> _logger;
    readonly IGameService _gameService;

    public MiniGameUnlock(ILogger<MiniGameUnlock> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    ///<summary>
    /// 미니게임 잠금 해제 API
    /// 미니게임을 잠금 해제합니다.
    /// </summary>
    [HttpPost]
    public async Task<MiniGameUnlockResponse> UnlockMiniGame([FromHeader] HeaderDTO header, MiniGameUnlockRequest request)
    {
        MiniGameUnlockResponse response = new();

        response.Result = await _gameService.UnlockMiniGame(header.Uid, request.GameKey);
    
        _logger.ZLogInformation($"[MiniGameUnlock] Uid : {header.Uid}, GameKey : {request.GameKey}");
        return response;
    }
}
