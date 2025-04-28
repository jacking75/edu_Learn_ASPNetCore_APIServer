using System.ComponentModel.DataAnnotations;

namespace GameShared.DTO;

public class UserInfoRequest
{
	[Required]
	public Int64 Uid { get; set; }
}

public class UserDataRequest;

public class UserDataResponse : ErrorCodeDTO
{
	public LoadedUserData? UserData { get; set; }
}
public class NicknameUpdateRequest
{
	[Required]
	[MinLength(1)]
	[StringLength(20)]
	public string Nickname { get; set; }
}

public class NicknameUpdateResponse : ErrorCodeDTO;

public class LoadedUserData
{
	public UserInfo UserInfo { get; set; }
	public IEnumerable<UserItemInfo>? UserItemInfo { get; set; }
	public IEnumerable<UserAttendanceInfo>? UserAttendanceInfo { get; set; }
}
