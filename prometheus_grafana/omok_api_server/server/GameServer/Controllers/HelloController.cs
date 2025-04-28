using Microsoft.AspNetCore.Mvc;
using GameServer.Services.Interfaces;
using GameServer.DTO;
using ServerShared;
using System.Text.Json;
using Prometheus;

namespace GameServer.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : BaseController<HelloController> // ControllerBase
{
    private readonly ILogger<HelloController> _logger;
    private static readonly Gauge HelloGauge = Metrics.CreateGauge("game_server_hello", "Hello Metric");

    public HelloController(ILogger<HelloController> logger) : base(logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<HelloResponse> Hello()
    {
        HelloGauge.Inc();
        try
        {

            // 성공 로그를 JSON 형식으로 기록
            ActionLog(new
            {
                action = "hello_success",
                //playerId = request.PlayerId,
                //timestamp = DateTime.UtcNow
            });

            return new HelloResponse { 
                Result = ErrorCode.None,
                Message = "Hello, World!"
            };
        }
        catch (HttpRequestException e)
        {
            // 예외 처리 및 로그 기록
            ActionLog(new
            {
                action = "login_error_http",
                exception = e.Message,
                //timestamp = DateTime.UtcNow
            });
            return new HelloResponse { Result = ErrorCode.ServerError };
        }
        catch (JsonException e)
        {
            // JSON 파싱 예외 처리 및 로그 기록
            ActionLog(new
            {
                action = "login_error_json",
                exception = e.Message,
                //timestamp = DateTime.UtcNow
            });
            return new HelloResponse { Result = ErrorCode.JsonParsingError };
        }
        catch (Exception e)
        {
            // 일반 예외 처리 및 로그 기록
            ActionLog(new
            {
                action = "login_error_unexpected",
                exception = e.Message,
                //timestamp = DateTime.UtcNow
            });
            return new HelloResponse { Result = ErrorCode.InternalError };
        }
    }

}
