using GameAPIServer.Models.GameDB;
using System.Collections.Generic;

namespace GameAPIServer.DTO.DataLoad
{
    public class UserDataLoadResponse : ErrorCodeDTO
    {
        public DataLoadUserInfo UserData { get; set; }
    }

    public class DataLoadUserInfo
    {
        public GdbUserInfo UserInfo { get; set; }
        public GdbUserMoneyInfo MoneyInfo { get; set; }
        public GdbAttendanceInfo AttendanceInfo { get; set; }
    }
}
