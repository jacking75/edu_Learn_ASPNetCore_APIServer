using Microsoft.AspNetCore.Mvc;
using GameServer.Services.Interfaces;
using GameServer.DTO;
using ServerShared;
using System.Text.Json;
using Prometheus;

namespace GameServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : BaseController<LoginController> // ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ILoginService _loginService;

    private static readonly Gauge RealLoginGauge = Metrics.CreateGauge("game_server_real_login", "Real login Metric");


    public LoginController(ILogger<LoginController> logger, ILoginService loginService) : base(logger)
    {
        _logger = logger;
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<GameLoginResponse> Login([FromBody] GameLoginRequest request)
    {
        RealLoginGauge.Inc();

        try
        {
            var verifyTokenRequest = new VerifyTokenRequest
            {
                HiveUserId = request.PlayerId,
                HiveToken = request.Token
            };

            var result = await _loginService.login(request.PlayerId, request.Token, request.AppVersion, request.DataVersion);

            if (result == ErrorCode.None)
            {
                // 성공 로그를 JSON 형식으로 기록
                ActionLog(new
                {
                    action = "login_success",
                    playerId = request.PlayerId,
                    //timestamp = DateTime.UtcNow
                });
            }
            else
            {
                // 실패 로그를 JSON 형식으로 기록
                ActionLog(new
                {
                    action = "login_failed",
                    playerId = request.PlayerId,
                    errorCode = result,
                    //timestamp = DateTime.UtcNow
                });
            }

            return new GameLoginResponse { Result = result };
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
            return new GameLoginResponse { Result = ErrorCode.ServerError };
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
            return new GameLoginResponse { Result = ErrorCode.JsonParsingError };
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
            return new GameLoginResponse { Result = ErrorCode.InternalError };
        }
    }

    //[HttpPost]
    //public async Task<GameLoginResponse> Login([FromBody] GameLoginRequest request)
    //{
    //    try
    //    {
    //        var verifyTokenRequest = new VerifyTokenRequest
    //        {
    //            HiveUserId = request.PlayerId,
    //            HiveToken = request.Token
    //        };

    //        var result = await _loginService.login(request.PlayerId, request.Token, request.AppVersion, request.DataVersion);

    //        if (result == ErrorCode.None)
    //        {
    //            _logger.LogInformation("Login successful for PlayerId={PlayerId}", request.PlayerId);
    //        }
    //        else
    //        {
    //            _logger.LogWarning("Login failed for PlayerId={PlayerId} with ErrorCode={ErrorCode}", request.PlayerId, result);
    //        }

    //        return new GameLoginResponse { Result = result };
    //    }
    //    catch (HttpRequestException e)
    //    {
    //        _logger.LogError(e, "HTTP request to token validation service failed.");
    //        return new GameLoginResponse { Result = ErrorCode.ServerError };
    //    }
    //    catch (JsonException e)
    //    {
    //        _logger.LogError(e, "Error parsing JSON from token validation service.");
    //        return new GameLoginResponse { Result = ErrorCode.JsonParsingError };
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, "Unexpected error occurred during login.");
    //        return new GameLoginResponse { Result = ErrorCode.InternalError };
    //    }
    //}
}
