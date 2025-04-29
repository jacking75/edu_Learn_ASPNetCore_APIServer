using MatchAPIServer.Models.GameDB;

namespace MatchAPIServer.DTO.Attendance;

public class AttendanceInfoResponse : ErrorCodeDTO
{
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}