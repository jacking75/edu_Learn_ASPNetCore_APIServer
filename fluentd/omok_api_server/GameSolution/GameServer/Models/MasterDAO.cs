namespace GameServer.Models;

public class Item  
{
	public int ItemId { get; set; }
	public string ItemName { get; set; } = "";
	public string ItemImage { get; set; } = "";

	public static readonly string Table = "item";

	public static readonly string[] SelectColumns =
	[
		"item_id as ItemId",
		"item_name as ItemName",
		"item_image as ItemImage",
	];

	public GameShared.DTO.Item ToDTO()
	{
		return new GameShared.DTO.Item
		{
			ItemId = this.ItemId,
			ItemName = this.ItemName,
			ItemImage = this.ItemImage,
		};
	}
}

public class Reward
{
	public int RewardCode { get; set; }
	public int ItemId { get; set; }
	public int ItemCount { get; set; }

	public static readonly string Table = "reward";

	public static readonly string[] SelectColumns =
	[
		"reward_code as RewardCode",
		"item_id as ItemId",
		"item_count as ItemCount",
	];

	public GameShared.DTO.Reward ToDTO()
	{
		return new GameShared.DTO.Reward
		{
			RewardCode = this.RewardCode,
			ItemId = this.ItemId,
			ItemCount = this.ItemCount,
		};
	}
}

public class Attendance
{
	public string Name { get; set; } = "";
	public int AttendanceCode { get; set; }
	public bool Enabled { get; set; }
	public bool Repeatable { get; set; }

	public static readonly string Table = "attendance";

	public static readonly string[] SelectColumns =
	[
		"name as Name",
		"attendance_code as AttendanceCode",
		"enabled_yn as Enabled",
		"repeatable_yn as Repeatable",
	];

	public IEnumerable<AttendanceReward> AttendanceRewards { get; set; }

	public GameShared.DTO.Attendance ToDTO()
	{
		return new GameShared.DTO.Attendance
		{
			Name = this.Name,
			AttendanceCode = this.AttendanceCode,
			Enabled = this.Enabled,
			Repeatable = this.Repeatable,
			AttendanceRewards = this.AttendanceRewards.Select(ar => ar.ToDTO()),
		};
	}
}

public class AttendanceReward 
{
	public int AttendanceCode { get; set; }
	public int AttendanceCount { get; set; }
	public int RewardCode { get; set; }

	public static readonly string Table = "attendance_reward";

	public static readonly string[] SelectColumns =
	[
		"attendance_code as AttendanceCode",
		"attendance_count as AttendanceCount",
		"reward_code as RewardCode",
	];

	public GameShared.DTO.AttendanceReward ToDTO()
	{
		return new GameShared.DTO.AttendanceReward
		{
			AttendanceCode = this.AttendanceCode,
			AttendanceCount = this.AttendanceCount,
			RewardCode = this.RewardCode,
		};
	}
}