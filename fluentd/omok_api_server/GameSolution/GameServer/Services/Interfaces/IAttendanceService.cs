using GameServer.Models.GameDb;

namespace GameServer.Services.Interfaces;

public interface IAttendanceService
{
	public Task<(ErrorCode, IEnumerable<UserAttendance>?)> GetAttendanceList(Int64 uid);
	public Task<ErrorCode> UpdateAttendanceList(Int64 uid);
	public Task<ErrorCode> UpdateAttendance(Int64 uid , UserAttendance attendance);
}
