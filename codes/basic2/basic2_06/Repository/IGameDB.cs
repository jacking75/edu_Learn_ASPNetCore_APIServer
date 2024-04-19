using basic2_06.Controllers;
using System;


namespace basic2_03.Repository;

public interface IGameDB : IDisposable
{
    public Task<Tuple<ErrorCode, Int64>> AuthCheck(String email, String pw);


    public Task<GdbAttendanceInfo> GetAttendance(Int64 uid);
}


public class GdbAttendanceInfo
{
    public int uid { get; set; }
    public int attendance_cnt { get; set; }
    public DateTime recent_attendance_dt { get; set; }
}