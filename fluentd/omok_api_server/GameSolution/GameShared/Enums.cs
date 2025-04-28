public enum MailStatusCode : int
{
	Unread = 0,
	Read = 1,
	Received = 2,
	Expired = 3,
}

public enum MailType : int
{
	System = 0,
	User = 1,
}

public enum OmokResultCode
{
	None = 0,
	BlackWin = 1,
	WhiteWin = 2,
	Expired = 3,
	Draw = 4,
}

public enum OmokStone : byte
{
	None = 0,
	Black = 1,
	White = 2
}
