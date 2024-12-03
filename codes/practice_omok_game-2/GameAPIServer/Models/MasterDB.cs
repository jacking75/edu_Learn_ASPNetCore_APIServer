namespace GameAPIServer.Models.MasterDb;

public class ItemTemplate
{
	public static string[] SelectColumns =
	[
		"item_id as ItemId",
		"item_name as ItemName",
		"item_image as ItemImage",
	];
}

public class MoneyTemplate
{
	public static string[] SelectColumns =
	[
		"money_code as MoneyCode",
		"money_name as MoneyName",
	];
}

public class RewardTemplate
{
	public static string[] SelectColumns =
	[
		"reward_uid as RewardUid",
		"reward_code as RewardCode",
		"item_id as ItemId",
		"item_count as ItemCount",
	];
}

public class AttendanceTemplate
{
	public static string[] SelectColumns =
	[
		"attendance_name as Name",
		"attendance_code as AttendanceCode",
		"enabled_yn as Enabled",
		"repeatable_yn as Repeatable",
	];
}

public class AttendanceDetailTemplate
{
	public static string[] SelectColumns =
	[
		"attendance_code as AttendanceCode",
		"attendance_seq as AttendanceCount",
		"reward_code as RewardCode",
	];
}

public class Shop
{
	public int ShopCode { get; set; }
	public string ShopName { get; set; } = "";
	public bool Enabled { get; set; }

	public static string[] SelectColumns =
	[
		"shop_code as ShopCode",
		"shop_name as ShopName",
		"enabled_yn as Enabled",
	];
}

public class ShopItem
{
	public int ShopCode { get; set; }
	public int RewardCode { get; set; }
	public int CostCode { get; set; }
	public int CostAmount { get; set; }

	public static string[] SelectColumns =
	[
		"shop_code as ShopCode",
		"reward_code as RewardCode",
		"cost_code as CostCode",
		"cost_amount as CostAmount",
	];
}
