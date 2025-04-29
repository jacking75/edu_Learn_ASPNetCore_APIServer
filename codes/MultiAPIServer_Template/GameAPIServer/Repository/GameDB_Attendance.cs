using System;
using System.Threading.Tasks;
using MatchAPIServer.Models.GameDB;
using MatchAPIServer.Repository.Interfaces;
using SqlKata.Execution;

namespace MatchAPIServer.Services;

public partial class GameDb : IGameDb
{
    public async Task<GdbAttendanceInfo> GetAttendanceById(int uid)
    {
        return await _queryFactory.Query("user_attendance").Where("uid", uid)
                                                .FirstOrDefaultAsync<GdbAttendanceInfo>();
    }

    public async Task<int> CheckAttendanceById(int uid)
    {
        return await _queryFactory.StatementAsync($"UPDATE user_attendance " +
                                                  $"SET attendance_cnt = attendance_cnt +1, " +
                                                      $"recent_attendance_dt = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                                                  $"WHERE uid = {uid} AND " +
                                                      $"DATE(recent_attendance_dt) < '{DateTime.Today.ToString("yyyy-MM-dd")}';");
    }
}