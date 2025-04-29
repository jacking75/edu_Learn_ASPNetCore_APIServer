using GameAPIServer.Models.GameDB;

namespace GameAPIServer.DTO.Attendance;

public class AttendanceInfoResponse : ErrorCodeDTO
{
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}