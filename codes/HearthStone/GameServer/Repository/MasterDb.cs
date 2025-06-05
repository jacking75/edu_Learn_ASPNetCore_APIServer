using GameServer.Models;
using GameServer.Repository.Interface;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;
using ZLogger;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameServer.Repository;

public class MasterDb : IMasterDb
{
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<MasterDb> _logger;
    IDbConnection _dbConn;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;

    public MasterDb(ILogger<MasterDb> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;
        _dbConfig = dbConfig;
        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.MasterDb);

        _dbConn.Open();
    }

    void Close()
    {
        _dbConn.Close();
    }

    public void Dispose()
    {
        Close();
    }
    public MdbVersionInfo _version { get; set; }
    public List<MdbItemInfo> _itemInfoList { get; set; }
    public List<MdbAbilityInfo> _abilityInfoList { get; set; }
    public List<MdbGachaInfo> _gachaInfoList { get; set; }
    public List<MdbGachaRateInfo> _gachaRateInfoList { get; set; }
    public List<MdbAttendanceInfo> _attendanceInfoList { get; set; }
    public List<MdbAttendanceRewardInfo> _attendanceRewardList { get; set; }
    public List<MdbRewardInfo> _rewardInfoList { get; set; }
    public List<ItemInfo> _initItemInfoList { get; set; }
    public List<AssetInfo> _initAssetInfoList { get; set; }
    public List<MdbAttendanceInfo> _initAttendanceInfoList { get; set; }
    public List<MdbMailInfo> _initMailInfoList { get; set; }
    public Dictionary<int, ItemDetailInfo> _itemDetailInfoList { get; set; }
    private List<MdbShopInfo> _shopInfoList { get; set; }
    public Dictionary<int, MdbShopInfo> _shopList { get; set; }
    public async Task<bool> Load() 
    {
        try 
        {
            _version = await _queryFactory.Query($"version").FirstOrDefaultAsync<MdbVersionInfo>();
            _itemInfoList = (await _queryFactory.Query($"item").GetAsync<MdbItemInfo>()).ToList();
            _abilityInfoList = (await _queryFactory.Query($"ability").GetAsync<MdbAbilityInfo>()).ToList();
            _gachaInfoList = (await _queryFactory.Query($"gacha_info").GetAsync<MdbGachaInfo>()).ToList();
            _gachaRateInfoList = (await _queryFactory.Query($"gacha_rate").GetAsync<MdbGachaRateInfo>()).ToList();
            _attendanceInfoList = (await _queryFactory.Query($"attendance_info").GetAsync<MdbAttendanceInfo>()).ToList();
            _attendanceRewardList = (await _queryFactory.Query($"attendance_reward").GetAsync<MdbAttendanceRewardInfo>()).ToList();
            _rewardInfoList = (await _queryFactory.Query($"reward_info").GetAsync<MdbRewardInfo>()).ToList();
            _initItemInfoList = (await _queryFactory.Query($"initial_free_items").GetAsync<ItemInfo>()).ToList();
            _initAssetInfoList = (await _queryFactory.Query($"initial_asset").GetAsync<AssetInfo>()).ToList();
            _initMailInfoList = (await _queryFactory.Query($"initial_mail").GetAsync<MdbMailInfo>()).ToList();
            _shopInfoList = (await _queryFactory.Query($"shop").GetAsync<MdbShopInfo>()).ToList();

            return LoadComplete();
        }
        catch (Exception e)
        {
            return false;
        }
    }

    bool LoadComplete()
    {
        try
        {
            // is_free가 true인 출석 정보만 필터링하여 _initAttendanceInfoList에 할당
            _initAttendanceInfoList = _attendanceInfoList
                .Where(attendance => attendance.free_yn)
                .ToList();

            // 아이템 상세 정보 사전 초기화
            _itemDetailInfoList = new Dictionary<int, ItemDetailInfo>();

            // 능력치를 ability_key로 그룹화하여 조회 효율 향상
            var abilityLookup = _abilityInfoList
                .GroupBy(a => a.ability_key)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var iteminfo in _itemInfoList)
            {
                // 카드 기본 정보 생성
                var cardInfo = new CardInfo() { ItemId = (int)iteminfo.item_id };

                // 이 카드에 해당하는 능력치 목록 가져오기
                if (abilityLookup.TryGetValue(iteminfo.ability_key, out var abilities))
                {
                    // 각 능력치 타입에 따라 카드 속성 설정
                    foreach (var abil in abilities)
                    {
                        if (abil.ability_type.Equals("attack", StringComparison.OrdinalIgnoreCase))
                            cardInfo.Attack = (int)abil.ability_value;
                        else if (abil.ability_type.Equals("mana", StringComparison.OrdinalIgnoreCase))
                            cardInfo.ManaCost = (int)abil.ability_value;
                        else if (abil.ability_type.Equals("hp", StringComparison.OrdinalIgnoreCase))
                            cardInfo.Hp = (int)abil.ability_value;
                    }
                }

                // 아이템 상세 정보 생성
                var itemdetailinfo = new ItemDetailInfo
                {
                    ItemInfo = new MdbItemInfo
                    {
                        item_id = iteminfo.item_id,
                        item_grade_code = iteminfo.item_grade_code,
                        item_type = iteminfo.item_type,
                        ability_key = iteminfo.ability_key
                    },
                    CardInfo = cardInfo
                };

                // item_id를 키로 사용하여 _itemDetailInfoList에 추가
                if (!_itemDetailInfoList.ContainsKey((int)iteminfo.item_id))
                {
                    _itemDetailInfoList.Add((int)iteminfo.item_id, itemdetailinfo);
                }
                else
                {
                    _logger.LogWarning($"Duplicate item_id found: {iteminfo.item_id}");
                }
            }

            _logger.LogInformation($"Loaded {_itemDetailInfoList.Count} items with abilities");

            _shopList = new Dictionary<int, MdbShopInfo>();
            foreach (var shop in _shopInfoList)
            {
                _shopList.Add(shop.shop_id, shop);
            } 
            
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in LoadComplete: {e.Message}", e);
            return false;
        }
    }

}
