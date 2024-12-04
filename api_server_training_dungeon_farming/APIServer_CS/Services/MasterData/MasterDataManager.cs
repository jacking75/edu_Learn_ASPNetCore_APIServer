using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.Services.MasterData.Storages;

using MySqlConnector;

using SqlKata.Execution;

namespace APIServer.Services.MasterData;


public class MasterDataManager
{
    private readonly ItemInfoStorage _itemInfos = new();
    private readonly ItemTypeStorage _itemTypes = new();
    private readonly StageEnemyStorage _stageEnemies = new();
    private readonly InAppProductStorage _inAppProducts = new();
    private readonly StageFarmingItemStorage _stageFarmingItems = new();
    private readonly AttendanceRewardStorage _attendanceRewards = new();


    public async Task<bool> Load(string connectionString)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var queryFactory = CraeteQueryFactory(connection);
            if (await LoadByStorage(queryFactory) == false)
            {
                return false;
            }

            SetDefaultGiveItemList();

            SetInAppProductInfos();

            SetEnemySlayCounter();

            SetItemFarmingCounter();

            SetStageCompleteRewardExp();

            return true;
        }
    }

    private QueryFactory CraeteQueryFactory(MySqlConnection connection)
    {
        var compiler = new SqlKata.Compilers.MySqlCompiler();
        return new QueryFactory(connection, compiler);
    }

    private async Task<bool> LoadByStorage(QueryFactory queryFactory)
    {
        if (await _itemInfos.Load(queryFactory) == false)
        {
            return false;
        }

        if (await _itemTypes.Load(queryFactory) == false)
        {
            return false;
        }

        if (await _attendanceRewards.Load(queryFactory) == false)
        {
            return false;
        }

        if (await _inAppProducts.Load(queryFactory) == false)
        {
            return false;
        }

        if (await _stageEnemies.Load(queryFactory) == false)
        {
            return false;
        }

        if (await _stageFarmingItems.Load(queryFactory) == false)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Key     :   StageCode
    /// Value   :   Exp
    /// </summary>
    /// 
    public Dictionary<Int32, Int32> StageCompleteRewardExp { get; private set; } = new();
    private void SetStageCompleteRewardExp()
    {
        foreach (var stageEnemies in _stageEnemies.Datas.Values)
        {
            foreach (var stageEnemyInfo in stageEnemies)
            {
                if (StageCompleteRewardExp.ContainsKey(stageEnemyInfo.stage_code) == false)
                {
                    StageCompleteRewardExp[stageEnemyInfo.stage_code] = 0;
                }
                StageCompleteRewardExp[stageEnemyInfo.stage_code] += stageEnemyInfo.exp;
            }
        }
    }



    /// <summary>
    /// Key     : StageCode
    /// Value   :
    /// {
    ///     Key     : EnemyCode
    ///     Value   : MaxCount
    /// }
    /// </summary>
    /// 
    public Dictionary<Int32, Dictionary<Int32, Int32>> EnemySlayCounter { get; private set; } = new();
    private void SetEnemySlayCounter()
    {
        foreach (var stageEnemies in _stageEnemies.Datas.Values)
        {
            foreach (var stageEnemyInfo in stageEnemies)
            {
                if (EnemySlayCounter.ContainsKey(stageEnemyInfo.stage_code) == false)
                {
                    EnemySlayCounter[stageEnemyInfo.stage_code] = new();
                }

                EnemySlayCounter[stageEnemyInfo.stage_code][stageEnemyInfo.enemy_code] = stageEnemyInfo.enemy_count;
            }
        }
    }


    /// <summary>
    /// Key     : StageCode
    /// Value   :
    /// {
    ///     Key     : ItemCode
    ///     Value   : MaxFarmingCount
    /// }
    /// </summary>
    public Dictionary<Int32, Dictionary<Int32, Int32>> ItemFarmingCounter { get; private set; } = new();
    private void SetItemFarmingCounter()
    {
        foreach (var stageFarmingItems in _stageFarmingItems.Datas.Values)
        {
            foreach (var stageItemInfo in stageFarmingItems)
            {
                // 스테이지 정보가 없다면 생성.
                if (ItemFarmingCounter.ContainsKey(stageItemInfo.stage_code) == false)
                {
                    ItemFarmingCounter[stageItemInfo.stage_code] = new Dictionary<Int32, Int32>();
                }

                // 아이템 정보가 없다면 초기화
                var counter = ItemFarmingCounter[stageItemInfo.stage_code];
                if (counter.ContainsKey(stageItemInfo.item_code) == false)
                {
                    counter[stageItemInfo.item_code] = 0;
                }

                // 해당 아이템 최대 파밍 개수 증가
                counter[stageItemInfo.item_code]++;
            }
        }
    }



    /// <summary>
    /// Key     : PID
    /// Value   
    /// {
    ///     Item1   :   ItemInfo
    ///     Item2   :   GiveCount
    /// }
    /// </summary>
    public Dictionary<Int32, List<(ItemInfo, Int32)>> InAppProductInfos { get; private set; }
    private void SetInAppProductInfos()
    {
        InAppProductInfos = new(_inAppProducts.Datas.Count);

        foreach (var inAppProduct in _inAppProducts.Datas)
        {
            // 인앱 상품별 정보 읽어서 지급 개수에 짝지어 저장한다.
            var pid = inAppProduct.Key;
            var inAppProductItems = inAppProduct.Value;

            foreach (var itemInfo in inAppProductItems)
            {
                if (InAppProductInfos.ContainsKey(pid) == false)
                {
                    InAppProductInfos[pid] = new(inAppProductItems.Count);
                }

                InAppProductInfos[pid].Add((GetItemInfo(itemInfo.item_code), itemInfo.item_count));
            }
        }
    }

    public List<(ItemInfo, Int32)> GetInAppProductItemList(Int32 pid)
    {
        return InAppProductInfos[pid];
    }


    public List<(ItemInfo, Int32)> DefaultGiveItemList { get; private set; } = new();
    private void SetDefaultGiveItemList()
    {
        DefaultGiveItemList.Add((GetItemInfo((Int32)MasterDataCode.ItemCode.게임돈), 1000));
        DefaultGiveItemList.Add((GetItemInfo((Int32)MasterDataCode.ItemCode.작은칼), 1));
        DefaultGiveItemList.Add((GetItemInfo((Int32)MasterDataCode.ItemCode.나무방패), 1));
    }


    public ItemInfo GetItemInfo(Int32 itemCode)
    {
        return _itemInfos.Datas[itemCode];
    }

    public Int32 GetMaxEnhanceCount(Int32 itemCode)
    {
        return _itemInfos.Datas[itemCode].max_enhance_count;
    }

    public Int32 GetItemType(Int32 itemCode)
    {
        return _itemInfos.Datas[itemCode].item_type_code;
    }

    public AttendanceReward GetAttendanceReward(Int16 day)
    {
        return _attendanceRewards.Datas[day];
    }

    public List<StageEnemy> GetStageEnemyList(Int32 stageCode)
    {
        return _stageEnemies.Datas[stageCode];
    }

    public List<StageFarmingItem> GetStageFarmingItemList(Int32 stageCode)
    {
        return _stageFarmingItems.Datas[stageCode];
    }

    public bool IsExistStageFarmingItem(Int32 stageCode, Int32 itemCode)
    {
        var infos = GetStageFarmingItemList(stageCode);

        foreach (var info in infos)
        {
            if (info.item_code == itemCode)
            {
                return true;
            }
        }

        return false;
    }



}