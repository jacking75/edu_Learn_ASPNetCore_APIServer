using GameAPIServer.Models;


namespace GameAPIServer.DTOs;

public class UserDataLoadResponse : ErrorCode
{
    public DataLoadUserInfo UserData { get; set; }
}

public class DataLoadUserInfo
{
    public GdbUserInfo UserInfo { get; set; }
    public GdbUserMoneyInfo MoneyInfo { get; set; }
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}
