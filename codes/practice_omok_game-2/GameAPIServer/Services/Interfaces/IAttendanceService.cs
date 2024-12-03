namespace GameAPIServer.Services.Interfaces;

public interface IAttendanceService
{
	public Task<ErrorCode> Attend(Int64 uid);
	public Task<(ErrorCode, IEnumerable<AttendanceInfo>?)> GetAttendance(Int64 uid);
}
