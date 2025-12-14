using GameAPIServer.Models;

namespace GameAPIServer.DTOs;

public class AttendanceInfoResponse : ErrorCode
{
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}