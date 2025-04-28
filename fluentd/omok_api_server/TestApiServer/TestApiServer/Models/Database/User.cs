namespace TestApiServer.Models.Database;

public class User
{
	public Int64 Uid { get; set; }
	public Int64 PlayerId { get; set; }
	public string Nickname { get; set; } = "";
	public DateTime CreatedDateTime { get; set; }
	public DateTime RecentLoginDateTime { get; set; }
	public DateTime AttendanceUpdateTime { get; set; }

	public int WinCount { get; set; }
	public int PlayCount { get; set; }

	public static string[] SelectColumns =
	[
		"user_uid as Uid",
		"user_name as Nickname",
		"create_dt as CreatedDateTime",
		"recent_login_dt as RecentLoginDateTime",
		"attendance_update_dt as AttendanceUpdateTime",
	];
}
