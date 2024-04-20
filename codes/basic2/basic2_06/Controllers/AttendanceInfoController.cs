using basic2_03.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace basic2_06.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceInfoController : ControllerBase
{
    readonly ILogger<AttendanceInfoController> _logger;
    private readonly IGameDB _gameDB;

    public AttendanceInfoController(ILogger<AttendanceInfoController> logger, IGameDB gameDB)
    {
        _logger = logger;
        _gameDB = gameDB;
    }

    /// <summary>
    /// 출석 정보 API </br>
    /// 유저의 출석 정보(누적 출석일, 최근 출석 일시)를 전달합니다.
    /// </summary>
    [HttpPost]
    public async Task<AttendanceInfoResponse> Post([FromHeader] HeaderDTO header)
    {
        AttendanceInfoResponse response = new();

        MdbUserData userInfo = (MdbUserData)HttpContext.Items[nameof(MdbUserData)]!;

        // DB 관련 처리를 했다고 가정하고...
        await Task.Delay(32);
        
        _logger.ZLogInformation($"[AttendanceInfo] Uid : {userInfo.UId} , Result:{response.Result}");
        return response;
    }
}

public class HeaderDTO
{
    [FromHeader]
    public string UserID { get; set; }
    public string AuthToken { get; set; }
}




public class AttendanceInfoResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;

    public GdbAttendanceInfo AttendanceInfo { get; set; }
}