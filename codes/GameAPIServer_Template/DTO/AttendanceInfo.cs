using GameAPIServer.DAO;

namespace GameAPIServer.DTO;

public class AttendanceInfoResponse : ErrorCodeDTO
{
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}