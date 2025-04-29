using GameAPIServer.DTO;
using GameAPIServer.DTO.Attendance;
using GameAPIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace GameAPIServer.Controllers.Attendance;

[ApiController]
[Route("[controller]")]
public class AttendanceCheck : ControllerBase
{
    readonly ILogger<AttendanceCheck> _logger;
    readonly IAttendanceService _attendanceService;

    public AttendanceCheck(ILogger<AttendanceCheck> logger, IAttendanceService attendanceService)
    {
        _logger = logger;
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// 출석 체크 API </br>
    /// 출석 체크를 하고 받은 보상을 반환합니다.
    /// </summary>
    [HttpPost]
    public async Task<AttendanceCheckResponse> CheckAttendance([FromHeader] HeaderDTO header)
    {
        AttendanceCheckResponse response = new();
                              
        (response.Result, response.Rewards) = await _attendanceService.CheckAttendanceAndReceiveRewards(header.Uid);

        _logger.ZLogInformation($"[AttendanceCheck] Uid : {header.Uid}");
        return response;
    }

}
