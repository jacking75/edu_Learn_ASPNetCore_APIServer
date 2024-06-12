using System.Threading.Tasks;
using APIServer.DTO.Auth;
using APIServer.Repository.Interfaces;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    readonly IMemoryDb _memoryDb;
    readonly ILogger<Login> _logger;
    readonly IAuthService _authService;
    readonly IGameService _gameService;
    readonly IDataLoadService _dataLoadService;

    public Login(ILogger<Login> logger, IMemoryDb memoryDb, IAuthService authService, IGameService gameService, IDataLoadService dataLoadService)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _authService = authService;
        _gameService = gameService;
        _dataLoadService = dataLoadService;
    }

    /// <summary>
    /// 로그인 API </br>
    /// 하이브 토큰을 검증하고, 유저가 없다면 생성, 토큰 발급, 로그인 시간 업데이트, 유저 데이터 로드를 합니다. 
    /// </summary>
    [HttpPost]
    public async Task<LoginResponse> LoginAndLoadData(LoginRequest request)
    {
        LoginResponse response = new();

        //하이브 토큰 체크
        var errorCode = await _authService.VerifyTokenToHive(request.PlayerId, request.HiveToken);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        //유저 있는지 확인
        (errorCode, var uid) = await _authService.VerifyUser(request.PlayerId);
        // 유저가 없다면 유저 데이터 생성
        if(errorCode == ErrorCode.LoginFailUserNotExist)
        {
            (errorCode, uid) = await _gameService.InitNewUserGameData(request.PlayerId, request.Nickname);
        }
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        response.Uid = uid;

        //토큰 발급
        (errorCode, var token) = await _authService.RegisterToken(uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        response.Token = token;

        //로그인 시간 업데이트
        errorCode = await _authService.UpdateLastLoginTime(uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        //유저 데이터 로드
        (errorCode, response.userData) = await _dataLoadService.LoadUserData(uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[Login] Uid : {uid}, Token : {token}, PlayerId : {request.PlayerId}");
        return response;
    }
}
