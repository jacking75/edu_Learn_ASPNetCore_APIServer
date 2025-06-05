using Microsoft.AspNetCore.Mvc;
using GameServer.Services.Interface;
using GameServer.Models;
using GameServer.Models.DTO;
namespace GameServer.Controllers;

[ApiController]
[Route("contents/shop")]
public class ShopController : ControllerBase
{
    ILogger<ShopController> _logger;
    readonly IShopService _shopService;

    public ShopController(ILogger<ShopController> logger, IShopService shopService)
    {
        _logger = logger;
        _shopService = shopService;
    }

    [HttpPost("buy")]
    public async Task<BuyResponse> Buy([FromHeader] HeaderDTO header, [FromBody] BuyRequest request)
    {
        BuyResponse response = new BuyResponse();
        (response.Result, response.RewardInfo, response.UseAsset)= await _shopService.BuyItem(header.AccountUid, request.ShopId); 
        return response;
    }

}
