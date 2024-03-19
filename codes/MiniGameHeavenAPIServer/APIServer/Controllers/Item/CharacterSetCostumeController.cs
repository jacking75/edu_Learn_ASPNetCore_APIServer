using APIServer.DTO;
using APIServer.DTO.Item;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Item;

[ApiController]
[Route("[controller]")]
public class CharacterSetCostume : ControllerBase
{
    readonly ILogger<CharacterSetCostume> _logger;
    readonly IItemService _itemService;

    public CharacterSetCostume(ILogger<CharacterSetCostume> logger, IItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    /// <summary>
    /// 캐릭터 코스튬 변경 API
    /// 캐릭터의 코스튬을 변경합니다.
    /// </summary>
    [HttpPost]
    public async Task<CharacterSetCostumeResponse> SetCharCostume([FromHeader] HeaderDTO header, [FromBody] CharacterSetCostumeRequest request)
    {
        CharacterSetCostumeResponse response = new();

        response.Result = await _itemService.SetCharCostume(header.Uid, request.CharKey, request.CostumeInfo);

        _logger.ZLogInformation($"[CharacterSetCostume] Uid : {header.Uid}, CharKey : {request.CharKey}, CostumeInfo : {request.CostumeInfo}");
        return response;
    }
}
