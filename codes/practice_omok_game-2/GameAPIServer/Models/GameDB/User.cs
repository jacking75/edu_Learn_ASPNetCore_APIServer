namespace GameAPIServer.Models.GameDb;
public class User : UserInfo
{
	public static string[] SelectColumns =
	[
		"user_uid as Uid",
		"user_name as Nickname",
		"create_dt as CreatedDateTime",
		"recent_login_dt as RecentLoginDateTime",
		"attendance_update_dt as AttendanceUpdateTime",
	];
}

public class UserMoney : UserMoneyInfo
{
	public static string[] SelectColumns =
	[
		"money_code as MoneyCode",
		"money_amount as MoneyAmount",
	];
}

public class UserItem 
{
	public Int64 user_uid{ get; set; }
	public int item_id { get; set; }
	public int item_cnt { get; set; }

	public static string[] SelectColumns =
	[
		"item_id as ItemId",
		"item_cnt as ItemCount",
	];
}
