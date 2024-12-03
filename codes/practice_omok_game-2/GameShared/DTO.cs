using System.ComponentModel.DataAnnotations;

/**
 * 
 * Common 
 * 
 */

public class ErrorCodeDTO
{
	public ErrorCode Result { get; set; } = ErrorCode.None;
}


/**
 * 
 * User 
 * 
 */
public class UserDataLoadResponse : ErrorCodeDTO
{
	public LoadedUserData? UserData { get; set; }

}

public class UserItemLoadResponse : ErrorCodeDTO
{
	public LoadedItemData? ItemData { get; set; }
}

public class GameDataLoadResponse : ErrorCodeDTO
{
	public LoadedGameData? GameData { get; set; }
}

public class UserProfileLoadRequest
{
	public Int64 Uid { get; set; }
}

public class UserProfileLoadResponse : ErrorCodeDTO
{
	public LoadedProfileData? ProfileData { get; set; }
}

public class UpdateNicknameRequest
{
	public string Nickname { get; set; } = "";
}

public class UpdateNicknameResponse : ErrorCodeDTO;

public class LoadedUserData
{
	public UserInfo? User { get; set; }
	public IEnumerable<UserMoneyInfo>? UserMoney { get; set; }

	public IEnumerable<AttendanceInfo>? UserAttendances { get; set; }
}

public class LoadedGameData
{
	public List<Item>? Items { get; set; }
	public List<Attendance>? Attendances { get; set; }
	public List<Reward>? Rewards { get; set; }
}

public class LoadedItemData
{
	public IEnumerable<UserItemInfo>? UserItem { get; set; }
}

public class LoadedProfileData
{
	public UserInfo? User { get; set; }
}

public class UserInfo
{
	public Int64 Uid { get; set; }
	public Int64 PlayerId { get; set; }
	public string Nickname { get; set; } = "";
	public DateTime CreatedDateTime { get; set; }
	public DateTime RecentLoginDateTime { get; set; }
	public DateTime AttendanceUpdateTime { get; set; }

	public int WinCount { get; set; }
	public int PlayCount { get; set; }
}

public class UserMoneyInfo
{
	public int MoneyCode { get; set; }
	public int MoneyAmount { get; set; }
}
public class UserItemInfo
{
	public int ItemId { get; set; }
	public int ItemCount { get; set; }

}

/**
 * 
 * Game Content 
 * 
 */

public class Item
{
	public int ItemId { get; set; }
	public string ItemName { get; set; } = "";
	public string ItemImage { get; set; } = "";
}

public class Money
{
	public int MoneyCode { get; set; }
	public string MoneyName { get; set; } = "";
}

public class Reward
{
	public int RewardUid { get; set; }
	public int RewardCode { get; set; }
	public int ItemId { get; set; }
	public int ItemCount { get; set; }
}

public class Attendance
{
	public string Name { get; set; } = "";
	public int AttendanceCode { get; set; }
	public bool Enabled { get; set; }
	public bool Repeatable { get; set; }

	public List<AttendanceDetail> AttendanceDetails { get; set; } = new List<AttendanceDetail>();
}

public class AttendanceDetail
{
	public int AttendanceCode { get; set; }
	public int AttendanceCount { get; set; }
	public int RewardCode { get; set; }

}

/**
 * 
 * Mail 
 * 
 */

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

public class GetMailResponse : ErrorCodeDTO
{
	public IEnumerable<MailInfo>? MailData { get; set; }
}
public class ReadMailRequest
{
	public Int64 MailUid { get; set; }
}
public class ReadMailResponse : ErrorCodeDTO
{
	public MailInfo? MailInfo { get; set; }
	public List<(Item, int)>? Items { get; set; }
}


public class ReceiveMailRequest
{
	public Int64 MailUid { get; set; }
}


public class ReceiveMailResponse : ErrorCodeDTO
{
}

public class SendMailRequest
{
	public Int64 ReceiveUid { get; set; }
	public string Title { get; set; } = "";
	public string Content { get; set; } = "";
}

public class SendMailResponse : ErrorCodeDTO
{
}

public class DeleteMailRequest
{
	public Int64 MailUid { get; set; }
}


public class DeleteMailResponse : ErrorCodeDTO
{
}


public class MailInfo
{
	public Int64 MailUid { get; set; }
	public Int64 ReceiveUid { get; set; }
	public Int64 SendUid { get; set; }
	public string Title { get; set; } = "";
	public string Content { get; set; } = "";
	public MailStatusCode StatusCode { get; set; } = 0;
	public MailType Type { get; set; } = 0;
	public int RewardCode { get; set; } = 0;
	public DateTime CreatedDateTime { get; set; }
	public DateTime UpdatedDateTime { get; set; }
	public DateTime ExpireDateTime { get; set; }
}

/**
 * 
 * Attendance 
 * 
 */
public class AttendanceInfo
{
	public Int64 Uid { get; set; }
	public int AttendanceCode { get; set; }
	public int AttendanceCount { get; set; }
}


public class AttendanceResponse : ErrorCodeDTO
{
}



/**
 * 
 * Game
 * 
 */


public class GameResponse : ErrorCodeDTO
{
	public byte[]? GameData { get; set; }
}

public class GetGameResponse : GameResponse
{
}

public class EnterGameResponse : GameResponse
{
}

public class PlayOmokRequest
{
	public int PosX { get; set; }

	public int PosY { get; set; }
}

public class PlayOmokResponse : GameResponse
{
}

public class PeekGameRequest
{
}

public class PeekGameResponse : GameResponse
{
}
public class GameResultInfo
{
	public Int64 GameResultUid { get; set; }
	public Int64 BlackPlayerUid { get; set; }
	public Int64 WhitePlayerUid { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public int ResultCode { get; set; }
}




/**
 * 
 * Auth  
 * 
 */

public class LoginRequest
{
	[Required]
	public Int64 PlayerId { get; set; }

	[Required]
	public string HiveToken { get; set; }
}

public class LoginResponse : ErrorCodeDTO
{
}

public class LogoutResponse : ErrorCodeDTO
{
}

/**
 * 
 * Match
 * 
 */

public class StartMatchRequest
{

}
public class PeekMatchRequest
{

}

public class StartMatchResponse : ErrorCodeDTO
{
}

public class CheckMatchRequest
{

}

public class MatchData
{
	public Int64 MatchedUserID { get; set; }
	public string GameGuid { get; set; } = "";

	public TimeSpan? RemainTime { get; set; }
}


public class CheckMatchResponse : ErrorCodeDTO
{
	public MatchData? MatchData { get; set; }
}

/**
 * 
 * Hive  
 * 
 */

public class HiveRegisterRequest : HiveCredentials { }

public class HiveRegisterResponse : ErrorCodeDTO { }

public class HiveLoginRequest : HiveCredentials { }

public class HiveLoginResponse : ErrorCodeDTO
{
	[Required]
	public Int64 PlayerId { get; set; }
	[Required]
	public string HiveToken { get; set; }
}

public class HiveVerifyTokenRequest : HiveLoginResponse { }

public class HiveVerifyTokenResponse : ErrorCodeDTO { }

public class HiveCredentials
{
	[Required]
	[MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
	[StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
	[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
	public string Email { get; set; }
	[Required]
	[MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
	[StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
	[DataType(DataType.Password)]
	public string Password { get; set; }
}