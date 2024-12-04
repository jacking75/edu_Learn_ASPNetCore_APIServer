using System;
using System.Collections.Generic;

namespace APIServer.ModelDB;


public class UserPlayData
{
    public Int64 user_id { get; set; }

    public Int16 level { get; set; }

    public Int32 exp { get; set; }

    public DateTime create_date { get; set; }
}


public class UserInventoryItem
{
    public Int64 inventory_item_id { get; set; }

    public Int32 item_code { get; set; }

    public Int64 item_attack_power { get; set; }

    public Int64 item_defensive_power { get; set; }

    public Int64 item_magic { get; set; }

    public Int16 enhance_stage { get; set; }

    public Int32 item_count { get; set; }
}


public class UserMail
{
    public Int64 mail_id { get; set; }

    public Int16 mail_type { get; set; } = -1;

    public string mail_title { get; set; } = string.Empty;

    public Int32 item_code { get; set; }

    public Int32 item_count { get; set; } = -1;

    public bool is_receive { get; set; } = false;

    public string send_date { get; set; } = string.Empty;

    public string receive_date { get; set; } = string.Empty;

    public DateTime expire_date { get; set; }
}


public class UserAttendanceBook
{
    public Int16 last_attendance_day { get; set; }

    public DateTime start_update_date { get; set; }

    public DateTime last_update_date { get; set; }
}


// Redis 관련

public class UserBattleInfo
{
    public Int32 StageCode { get; set; }

    /// <summary>
    /// Key     : ItemCode
    /// Value   : FarmedCount
    /// </summary>
    public Dictionary<Int32, Int32> FarmedItems { get; set; } = new();

    /// <summary>
    /// Key     : ItemCode
    /// Value   : FarmingMaxCount
    /// </summary>
    public Dictionary<Int32, Int32> ItemFarmingCounterRef { get; set; }

    /// <summary>
    /// Key     : EnemyCode
    /// Value   : SlainCount
    /// /// </summary>
    public Dictionary<Int32, Int32> SlainEnemies { get; set; } = new();
    /// <summary>
    /// Key     : EnemyCode
    /// Value   : SlayMaxCount
    /// </summary>
    public Dictionary<Int32, Int32> EnemySlayCounterRef { get; set; }

    public Int32 CompleteRewardExp { get; set; }

    public bool Validate(Int32 stageCode, Int32 enemyCode = -1)
    {
        if (StageCode != stageCode)
        {
            return false;
        }

        if (enemyCode != -1)
        {
            if (SlainEnemies.ContainsKey(enemyCode) == false)
            {
                return false;
            }

            if (IsOverSlainEnemy(enemyCode) == true)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsOverSlainEnemy(Int32 enemyCode) => EnemySlayCounterRef[enemyCode] <= SlainEnemies[enemyCode];

    public Int32 GetSlainEnemyCount(Int32 enemyCode) => SlainEnemies[enemyCode];

    public void IncrementSlainEnemyCount(Int32 enemyCode) => SlainEnemies[enemyCode]++;

    public bool IsAllSlainEnemies()
    {
        foreach (var slainEnemy in SlainEnemies)
        {
            if (slainEnemy.Value == EnemySlayCounterRef[slainEnemy.Key])
            {
                return false;
            }
        }

        return true;
    }

    public bool AddFarmingItem(Int32 itemCode)
    {
        if (FarmedItems.ContainsKey(itemCode) == false)
        {
            FarmedItems[itemCode] = 1;
            return true;
        }

        if (IsOverFarmingItem(itemCode) == true)
        {
            return false;
        }

        FarmedItems[itemCode]++;
        return true;
    }
    private bool IsOverFarmingItem(Int32 itemCode) => ItemFarmingCounterRef[itemCode] <= FarmedItems[itemCode];
}



public class ChatInfo
{
    public string MessageId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}