using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class Attendance : SecureController<Attendance>
{
	private readonly IAttendanceService _attendanceService;

	public Attendance(ILogger<Attendance> logger, IAttendanceService attendanceService) : base(logger)
	{
		_attendanceService = attendanceService;
	}

	/// <summary>
	/// 출석 갱신
	/// </summary>
	[HttpPost]
	public async Task<ErrorCodeDTO> Attend()
	{
		AttendanceResponse response = new();

		Int64 uid = GetUserUid();

		response.Result = await _attendanceService.Attend(uid);

		if (ErrorCode.None != response.Result)
		{
			_logger.ZLogError($"[UpdateUserAttendance] ErrorCode : {response.Result}");
			return response;
		}
		return response;
	}
}
