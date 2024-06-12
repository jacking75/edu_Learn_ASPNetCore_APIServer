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
public class CharacterList : ControllerBase
{
    readonly ILogger<CharacterList> _logger;
    readonly IItemService _itemService;

    public CharacterList(ILogger<CharacterList> logger, IItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    /// <summary>
    /// 캐릭터 목록 조회 API
    /// 유저의 캐릭터 목록을 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<CharacterListResponse> GetCharacterList([FromHeader] HeaderDTO header)
    {
        CharacterListResponse response = new();

        (response.Result, response.CharList) = await _itemService.GetCharList(header.Uid);

        _logger.ZLogInformation($"[CharacterList] Uid : {header.Uid}");
        return response;
    }
}
