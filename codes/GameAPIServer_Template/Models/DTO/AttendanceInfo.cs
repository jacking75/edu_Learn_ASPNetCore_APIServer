using GameAPIServer.Models.DAO;


namespace GameAPIServer.Models.DTO;

public class AttendanceInfoResponse : ErrorCode
{
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}