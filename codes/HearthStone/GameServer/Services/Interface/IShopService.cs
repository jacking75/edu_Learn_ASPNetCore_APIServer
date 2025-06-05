using GameServer.Models;
using GameServer.Models.DTO;

namespace GameServer.Services.Interface;

public interface IShopService
{
    public Task<(ErrorCode, ReceivedReward?, UseAsset?)> BuyItem(Int64 accountUid, int shopId);  

}
