using GameServer.Models;

namespace GameServer.Repository.Interface;

public interface IMasterDb : IDisposable
{
    public MdbVersionInfo _version { get; }
    public List<MdbItemInfo> _itemInfoList { get; }
    public List<MdbAbilityInfo> _abilityInfoList { get;}
    public List<MdbGachaInfo> _gachaInfoList { get; }
    public List<MdbGachaRateInfo> _gachaRateInfoList { get;}
    public List<MdbAttendanceInfo> _attendanceInfoList { get;}
    public List<MdbAttendanceRewardInfo> _attendanceRewardList { get;}
    public List<MdbRewardInfo> _rewardInfoList { get; }
    public List<ItemInfo> _initItemInfoList { get;}
    public List<AssetInfo> _initAssetInfoList { get;}
    public List<MdbAttendanceInfo> _initAttendanceInfoList { get; }
    public List<MdbMailInfo> _initMailInfoList { get; set; }
    public Dictionary<int, ItemDetailInfo> _itemDetailInfoList { get; set; }
    public Dictionary<int, MdbShopInfo> _shopList { get; set; }

    public Task<bool> Load();
}
