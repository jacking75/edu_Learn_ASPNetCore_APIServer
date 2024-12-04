using System;

namespace APIServer.ModelDB;

public class ItemInfo
{
    public Int32 item_code { get; set; }

    public string item_name { get; set; } = string.Empty;

    public Int32 item_type_code { get; set; }

    public Int64 item_sale_price { get; set; }

    public Int64 item_purchase_price { get; set; }

    public Int32 item_useful_level { get; set; }

    public Int64 item_attack_power { get; set; }

    public Int64 item_defensive_power { get; set; }

    public Int64 item_magic { get; set; }

    public Int32 max_enhance_count { get; set; }
}


public class ItemType
{
    public Int32 item_type_code { get; set; }

    public string item_type_name { get; set; } = string.Empty;
}


public class AttendanceReward
{
    public Int16 days { get; set; }

    public Int32 item_code { get; set; }

    public Int32 item_count { get; set; }
}


public class InAppProduct
{
    public Int32 pid { get; set; }

    public Int32 item_code { get; set; }

    public string item_name { get; set; }

    public Int32 item_count { get; set; }
}


public class StageEnemy
{
    public Int32 stage_code { get; set; }

    public Int32 enemy_code { get; set; }

    public Int32 enemy_count { get; set; }

    public Int32 exp { get; set; }
}


public class StageFarmingItem
{
    public Int32 stage_code { get; set; }

    public Int32 item_code { get; set; }
}