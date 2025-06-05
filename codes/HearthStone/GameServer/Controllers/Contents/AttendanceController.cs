using Microsoft.AspNetCore.Mvc;
using GameServer.Models.DTO;
using GameServer.Services.Interface;
using ZLogger;

namespace GameServer.Controllers;

[ApiController]
[Route("contents/attendance")]
public class AttendanceController : ControllerBase
{
    readonly ILogger<AttendanceController> _logger;
    readonly IAttendanceService _attendanceService;

    public AttendanceController(ILogger<AttendanceController> logger, IAttendanceService attendanceService)
    {
        _logger = logger;
        _attendanceService = attendanceService;
    }

    [HttpPost("load")]
    public async Task<AttendanceInfoResponse> GetAttendanceInfo([FromHeader] HeaderDTO header)
    {
        AttendanceInfoResponse response = new();
        (response.Result, response.AttendanceInfoList) = await _attendanceService.GetAttendanceInfoList(header.AccountUid);
        return response;
    }

    [HttpPost("check")]
    public async Task<AttendanceCheckResponse> CheckAttendanceInfo([FromHeader] HeaderDTO header, [FromBody] AttendanceCheckRequest request)
    {
        AttendanceCheckResponse response = new();
        (response.Result, response.ReceivedReward) = await _attendanceService.CheckAttendanceAndReceiveRewards(header.AccountUid, request.eventKey);
        return response;
    }
}
