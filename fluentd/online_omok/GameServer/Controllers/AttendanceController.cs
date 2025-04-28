using GameServer.Services.Interfaces;
using GameShared.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerShared;

namespace GameServer.Controllers;

[Route("[controller]")]
[ApiController]
public class AttendanceController : BaseController<AttendanceController>
{
	private readonly IAttendanceService _attendanceService;
	public AttendanceController(ILogger<AttendanceController> logger, IAttendanceService attendanceService) : base(logger)
	{
		_attendanceService = attendanceService;
	}

	[HttpPost]
	public async Task<ErrorCodeDTO> Attend()
	{
		AttendanceResponse response = new();

		response.Result = await _attendanceService.UpdateAttendanceList(GetUserUid());

		if (ErrorCode.None != response.Result)
		{
			ErrorLog(response.Result, GetUserUid());
		}

		return response;
	}
}
