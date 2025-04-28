using GameAPIServer.Servicies.Interfaces;
using GameAPIServer.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace GameAPIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class AttendanceInfoController : ControllerBase
{
    readonly ILogger<AttendanceInfoController> _logger;
    readonly IAttendanceService _attendanceService;

    public AttendanceInfoController(ILogger<AttendanceInfoController> logger, IAttendanceService attendanceService)
    {
        _logger = logger;
        _attendanceService = attendanceService;
    }

    /// <summary>
    /// 출석 정보 API </br>
    /// 유저의 출석 정보(누적 출석일, 최근 출석 일시)를 전달합니다.
    /// </summary>
    [HttpPost]
    public async Task<AttendanceInfoResponse> GetAttendanceInfo([FromHeader] Header header)
    {
        AttendanceInfoResponse response = new();

        (response.Result, response.AttendanceInfo) = await _attendanceService.GetAttendanceInfo(header.Uid);
        
        _logger.ZLogInformation($"[AttendanceInfo] Uid : {header.Uid}");
        return response;
    }
}
