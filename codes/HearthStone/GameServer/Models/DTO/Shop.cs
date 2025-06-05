using Microsoft.AspNetCore.Mvc;

namespace GameServer.Models.DTO;

public class BuyRequest
{
    public int ShopId { get; set; }
}

public class UseAsset 
{
    public List<AssetInfo> AssetInfoList{ get; set; }
}


public class BuyResponse : ErrorCodeDTO
{
    public ReceivedReward? RewardInfo { get; set; }
    public UseAsset? UseAsset { get; set; }
}
