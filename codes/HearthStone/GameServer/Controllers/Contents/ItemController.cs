using GameServer.Models.DTO; 
using GameServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameServer.Controllers.Contents;

[ApiController]
[Route("contents")]
public class ItemController  : ControllerBase
{
    readonly ILogger<ItemController> _logger;
    readonly IItemService _itemService;
    public ItemController(ILogger<ItemController> logger, IItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    [HttpPost("item/load")]
    public async Task<ItemInfoListResponse> GetItemList([FromHeader] HeaderDTO header)
    {
        ItemInfoListResponse response = new();
        (response.Result, response.ItemList) = await _itemService.GetItemInfoList(header.AccountUid);
        return response;
    }

    [HttpPost("item/gacha")]
    public async Task<ItemRandomResponse> GetItemRandom([FromHeader] HeaderDTO header, ItemRandomRequest request)
    {
        ItemRandomResponse response = new();
        (response.Result, response.ItemList) = await _itemService.GetItemRandom(header.AccountUid, request.GachaId);
        return response;
    }

    [HttpPost("deck/save")]
    public async Task<SaveDeckResponse> SaveDeck([FromHeader] HeaderDTO header, SaveDeckRequest request)
    {
        SaveDeckResponse response = new();
        try
        {
            _logger.ZLogInformation($"[Deck Save] User:{header.AccountUid}, DeckId:{request.DeckId}");
            response.Result = await _itemService.SaveDeck(header.AccountUid, request.DeckId, request.DeckList);
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"[Deck Save Error] User:{header.AccountUid}, DeckId:{request.DeckId}");
            response.Result = ErrorCode.DeckSaveFail;
        }
        return response;
    }

    [HttpPost("deck/main")]
    public async Task<SetMainDeckResponse> SetMainDeck([FromHeader] HeaderDTO header, SetMainDeckRequest request)
    {
        SetMainDeckResponse response = new();
        try
        {
            _logger.ZLogInformation($"[Set Main Deck] User:{header.AccountUid}, DeckId:{request.DeckId}");
            response.Result = await _itemService.SetMainDeck(header.AccountUid, request.DeckId);
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"[Set Main Deck Error] User:{header.AccountUid}, DeckId:{request.DeckId}");
            response.Result = ErrorCode.DeckSetMainFail;
        }
        return response;
    }
}
