namespace GameShared.DTO;

public class MailInfo 
{
	public Int64 Uid { get; set; }
	public Int64 ReceiverUid { get; set; }
	public Int64 SenderUid { get; set; }
	public string Title { get; set; } = "";
	public string Content { get; set; } = "";
	public MailStatusCode StatusCode { get; set; } = 0;
	public int RewardCode { get; set; } = 0;
	public DateTime CreateDt { get; set; }
	public DateTime UpdateDt { get; set; }
	public DateTime ExpireDt { get; set; }
	public MailType MailType { get; set; }
}

public class UserInfo 
{
	public Int64 Uid { get; set; }
	public string Nickname { get; set; }
	public DateTime CreateDt { get; set; }
	public DateTime LastLoginDt { get; set; }
	public int WinCount { get; set; }
	public int PlayCount { get; set; }
}

public class UserItemInfo
{
	public int ItemId { get; set; }
	public int ItemCount { get; set; }
}

public class UserAttendanceInfo
{
	public int AttendanceCode { get; set; }
	public int AttendanceCount { get; set; }
}