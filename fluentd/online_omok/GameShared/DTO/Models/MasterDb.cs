namespace GameShared.DTO;

public class Item
{
	public int ItemId { get; set; }
	public string ItemName { get; set; }
	public string ItemImage { get; set; }
}
public class Reward
{
	public int RewardCode { get; set; }
	public int ItemId { get; set; }
	public int ItemCount { get; set; }
}

public class Attendance
{
	public string Name { get; set; }
	public int AttendanceCode { get; set; }
	public bool Enabled { get; set; }
	public bool Repeatable { get; set; }

	public IEnumerable<AttendanceReward> AttendanceRewards { get; set; }

}

public class AttendanceReward
{
	public int AttendanceCode { get; set; }
	public int RewardCode { get; set; }
	public int AttendanceCount { get; set; }
}