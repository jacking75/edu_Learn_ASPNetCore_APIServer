using GameServer.Services.Interface;
using GameServer.Models.DTO;
using GameServer.Repository.Interface;
using GameServer.Models;

namespace GameServer.Services;

public class ShopService : IShopService
{
    private readonly IGameDb _gameDb;
    private readonly IMasterDb _masterDb;
    private readonly IItemService _itemService;
    private readonly ILogger<ShopService> _logger;
    public ShopService(IGameDb gameDb, IItemService itemService, ILogger<ShopService> logger, IMasterDb masterDb)
    {
        _gameDb = gameDb;
        _itemService = itemService;
        _logger = logger;
        _masterDb = masterDb;
    }

    public async Task<(ErrorCode, ReceivedReward?, UseAsset?)> BuyItem(Int64 accountUid, int shopId)
    {
        _masterDb._shopList.TryGetValue(shopId, out MdbShopInfo? shopInfo);
        if (shopInfo == null)
            return (ErrorCode.BuyError, null, null);

        AssetInfo assetInfo= await _gameDb.GetAssetInfo(accountUid, shopInfo.asset_name);
        if(assetInfo == null)
            return (ErrorCode.BuyError, null, null);

        if (assetInfo.asset_amount < shopInfo.asset_amount)
            return (ErrorCode.BuyError, null, null);

        (var errorCode, var itemList) = await _itemService.GetItemRandom(accountUid, shopInfo.gacha_key);
        if(errorCode != ErrorCode.None)
            return (errorCode, null, null);

        ReceivedReward reward = new ReceivedReward() { ItemList = new List<ItemInfo>()};
        foreach (var item in itemList)
        {
            reward.ItemList.Add(new ItemInfo
            {
                item_id = item.item_id,
                item_cnt = item.item_cnt
            });
        }

        var useAsset = new UseAsset() { AssetInfoList = new List<AssetInfo>() };
        useAsset.AssetInfoList.Add(new AssetInfo
        {
            asset_name = shopInfo.asset_name,
            asset_amount = -shopInfo.asset_amount
        });

        return (ErrorCode.None, reward, useAsset);
    }
}
