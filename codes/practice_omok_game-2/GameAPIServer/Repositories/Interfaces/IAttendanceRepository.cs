namespace GameAPIServer.Repositories.Interfaces;

public interface IAttendanceRepository : IDisposable
{
	public Task<IEnumerable<AttendanceInfo>> GetAttendanceList(Int64 uid);
	public Task<(ErrorCode, IEnumerable<AttendanceInfo>)> UpdateAttendanceList(Int64 uid);
	public Task<bool> InsertMissingAttendanceList(Int64 uid, List<Attendance> list);
}
