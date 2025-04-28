using GameShared.DTO;

namespace GameServer.Models.GameDb;

public class User 
{
	public Int64 Uid { get; set; }
	public Int64 HivePlayerId { get; set; }
	public string Nickname { get; set; }
	public DateTime CreateDt { get; set; }
	public DateTime LastLoginDt { get; set; }
	public DateTime LastAttendanceDt { get; set; }
	public int WinCount { get; set; }
	public int PlayCount { get; set; }

	public static readonly string Table = "user";
	public static readonly string[] SelectColumns =
	[
		"uid as Uid",
		"hive_player_id as HivePlayerId",
		"nickname as Nickname",
		"create_dt as CreateDt",
		"last_login_dt as LastLoginDt",
		"last_attendance_dt as LastAttendanceDt",
	];

	public GameShared.DTO.UserInfo ToDTO()
	{
		return new GameShared.DTO.UserInfo
		{
			Uid = this.Uid,
			Nickname = this.Nickname,
			CreateDt = this.CreateDt,
			LastLoginDt = this.LastLoginDt,
			WinCount = this.WinCount,
			PlayCount = this.PlayCount,
		};
	}
}

public class UserItem 
{
	public Int64 Uid { get; set; }
	public int ItemId { get; set; }
	public int ItemCount { get; set; }

	public static readonly string Table = "user_item";
	public static readonly string[] SelectColumns =
	[
		"uid as Uid",
		"item_id as ItemId",
		"item_count as ItemCount",
	];

	public GameShared.DTO.UserItemInfo ToDTO()
	{
		return new GameShared.DTO.UserItemInfo
		{
			ItemId = this.ItemId,
			ItemCount = this.ItemCount,
		};
	}
}

public class UserAttendance 
{
	public Int64 Uid { get; set; }
	public int AttendanceCode { get; set; }
	public int AttendanceCount { get; set; }

	public static readonly string Table = "user_attendance";
	public static readonly string[] SelectColumns =
	[
		"uid as Uid",
		"attendance_code as AttendanceCode",
		"attendance_count as AttendanceCount",
	];

	public GameShared.DTO.UserAttendanceInfo ToDTO()
	{
		return new GameShared.DTO.UserAttendanceInfo
		{
			AttendanceCode = this.AttendanceCode,
			AttendanceCount = this.AttendanceCount,
		};
	}
}
