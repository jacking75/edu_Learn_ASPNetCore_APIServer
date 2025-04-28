namespace GameAPIServer.Models.DTO;

public class UserDataLoadResponse : ErrorCode
{
    public DataLoadUserInfo UserData { get; set; }
}

public class DataLoadUserInfo
{
    public DAO.GdbUserInfo UserInfo { get; set; }
    public DAO.GdbUserMoneyInfo MoneyInfo { get; set; }
    public DAO.GdbAttendanceInfo AttendanceInfo { get; set; }
}
