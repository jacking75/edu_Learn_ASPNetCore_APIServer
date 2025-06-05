using GameServer.Services.Interface;
using GameServer.Models.DTO;
using GameServer.Repository.Interface;
using ZLogger;
using static Humanizer.In;
using GameServer.Models;
using System.Net.Http;

namespace GameServer.Services;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IGameService _gameService;
    readonly IGameDb _gameDb;
    readonly IMemoryDb _memoryDb;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IGameDb gameDb, IMemoryDb memoryDb, IGameService gameService, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
        _gameService = gameService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(ErrorCode, string)> Verify(Int64 accountUid, string hiveToken)
    {
        var result = await VerifyTokenToHive(accountUid, hiveToken);
        if (result != ErrorCode.None)
        {
            return (result, "");
        }

        result = await VerifyUser(accountUid);
        if (result == ErrorCode.UserNotFound)
        {
            result = await _gameService.InitNewUserGameData(accountUid);
        }

        if (result != ErrorCode.None)
        {
            return (result, "");
        }

        (result, string token) = await RegistToken(accountUid);
        if (result != ErrorCode.None)
        {
            return (result, "");
        }

        result = await UpdateLastLoginTime(accountUid);
        if (result != ErrorCode.None)
        {
            return (result, "");
        }
        return (ErrorCode.None, token);
    }

    public async Task<ErrorCode> VerifyTokenToHive(Int64 accountUid, string hiveToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("HiveServer");
            var endpoint = "auth/verifytoken";
            var hiveResponse = await client.PostAsJsonAsync(endpoint, new { accountUid = accountUid, HiveToken = hiveToken });

            if (hiveResponse == null || !ValidateHiveResponse(hiveResponse))
            {
                _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.HiveTokenInvalid}, accountUid = {accountUid}, Token = {hiveToken}");

                return ErrorCode.HiveTokenInvalid;
            }

            var authResult = await hiveResponse.Content.ReadFromJsonAsync<ErrorCodeDTO>();
            if (!ValidateHiveAuthErrorCode(authResult))
            {
                return ErrorCode.HiveTokenInvalid;
            }

            return ErrorCode.None;
        }
        catch
        {
            _logger.ZLogError($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.HiveTokenInvalid}, accountUid = {accountUid}, Token = {hiveToken}");

            return ErrorCode.HiveTokenInvalid;
        }
    }

    public bool ValidateHiveResponse(HttpResponseMessage? response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return false;
        }
        return true;
    }

    bool ValidateHiveAuthErrorCode(ErrorCodeDTO? authResult)
    {
        if (authResult == null || authResult.Result != ErrorCode.None)
        {
            return false;
        }

        return true;
    }

    public async Task<ErrorCode> VerifyUser(Int64 accountUid)
    {
        try
        {
            GdbUserInfo userInfo = await _gameDb.GetUserByAccountUid(accountUid);
            if (userInfo == null)
            {
                _logger.ZLogError($"[VerifyUser Service] ErrorCode:{ErrorCode.UserNotFound}, accountUid = {accountUid}");
                return ErrorCode.UserNotFound;
            }

            return ErrorCode.None;
        }
        catch
        {
            _logger.ZLogError($"[VerifyUser Service] ErrorCode:{ErrorCode.LoginFailException}, accountUid = {accountUid}");
            return ErrorCode.LoginFailException;
        }
    }

    public async Task<(ErrorCode, string)> RegistToken(Int64 accountUid)
    {
        try
        {
            string userToken = Security.CreateAuthToken();
            ErrorCode result = await _memoryDb.ResistUserAsync(userToken, accountUid);

            return (result, userToken);
        }
        catch
        {
            _logger.ZLogError($"[VerifyUser Service] ErrorCode:{ErrorCode.UserNotFound}, accountUid = {accountUid}");
            return (ErrorCode.RegistTokenFail, "");
        }
    }
    public async Task<ErrorCode> UpdateLastLoginTime(Int64 accountUid)
    {
        try
        {
            var count = await _gameDb.UpdateRecentLogin(accountUid);
            if (count < 1)
            {
                _logger.ZLogError($"[UpdateLastLoginTime] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFail}, count : {count}");
                return ErrorCode.LoginUpdateRecentLoginFail;
            }

            return ErrorCode.None;
        }
        catch
        {
            _logger.ZLogError($"[UpdateLastLoginTime] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFail}");
            return ErrorCode.LoginUpdateRecentLoginFail;
        }
    }

    public async Task LogOut(Int64 accountUid) 
    {
        try
        {
            await _memoryDb.DeleteUserAsync(accountUid);
            await _gameDb.UpdateRecentLogin(accountUid);
            return;
        }
        catch (Exception e)
        {
            _logger.ZLogError($"[UpdateLastLoginTime] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFail}");
            return;
        }
    }
}
