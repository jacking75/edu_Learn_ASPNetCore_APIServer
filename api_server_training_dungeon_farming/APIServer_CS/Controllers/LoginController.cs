using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using APIServer.ModelDB;
using APIServer.ModelReqRes;
using APIServer.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    readonly ILogger<Login> _logger;

    readonly IAccountDb _accountDb;

    readonly IGameDb _gameDb;

    readonly IMemoryDb _memoryDb;

    readonly VersionConfig _versionConfig;

    readonly ChannelUserManager _channelUserMgr;


    public Login(ILogger<Login> logger, IAccountDb accountDb, IGameDb gameDb, IMemoryDb memoryDb, IOptions<VersionConfig> versionConfig, ChannelUserManager channelUserMgr)
    {
        _logger = logger;
        _accountDb = accountDb;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
        _versionConfig = versionConfig.Value;
        _channelUserMgr = channelUserMgr;
    }


    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequest request)
    {
        var response = new LoginResponse();


        // 버전 확인
        var error = VerifyVersion(request);
        if (error != ErrorCode.None)
        {
            response.Result = error;
            return response;
        }


        // 유저 계정 정보 확인
        var accountInfo = await VerifyAccount(request.Email, request.Password);
        if (accountInfo is null)
        {
            response.Result = ErrorCode.NotCertifiedUser;
            return response;
        }


        // 유저 게임데이터 읽기
        var userId = accountInfo.user_id;
        var userPlayData = await LoadUserPlayData(userId);
        if (userPlayData is null)
        {
            response.Result = ErrorCode.NotExistUserPlayData;
            return response;
        }


        // 유저 아이템 정보 읽기
        var itemList = await LoadUserItemList(userId);
        if (itemList is null)
        {
            response.Result = ErrorCode.NotExistUserItem;
            return response;
        }


        // 유저 정보 캐싱
        var certifiedUser = await CachingUserInfo(request.Email, accountInfo.account_id, userId, request.AppVersion, request.MasterDataVersion);
        if (certifiedUser is null)
        {
            response.Result = ErrorCode.FailedRedisRegist;
            return response;
        }



        response.AuthToken = certifiedUser.AuthToken;
        response.GameInfo.PlayData = userPlayData;
        response.GameInfo.InventoryItem = itemList;
        response.GameNotice = _memoryDb.GetGameNotice();
        return response;
    }


    private ErrorCode VerifyVersion(LoginRequest request)
    {
        if (_versionConfig.App != request.AppVersion)
        {
            return ErrorCode.InvalidAppVersion;
        }

        if (_versionConfig.MasterData != request.MasterDataVersion)
        {
            return ErrorCode.InvalidMasterDataVersion;
        }

        return ErrorCode.None;
    }


    private async Task<Account> VerifyAccount(string email, string password)
    {
        var (error, loadedData) = await _accountDb.VerifyAccount(email, password);
        if (error != ErrorCode.None)
        {
            return null;
        }

        return loadedData;
    }


    private async Task<UserPlayData> LoadUserPlayData(Int64 userId)
    {
        var (error, loadedData) = await _gameDb.GetUserPlayData(userId);
        if (error != ErrorCode.None)
        {
            return null;
        }
        return loadedData;
    }


    private async Task<List<UserInventoryItem>> LoadUserItemList(Int64 userId)
    {
        var (error, loadedData) = await _gameDb.GetUserInventoryItemList(userId);
        if (error != ErrorCode.None)
        {
            return null;
        }

        return loadedData;
    }


    private async Task<(bool, Int32)> IsRelogin(string email)
    {
        var (_, info) = await _memoryDb.GetCertifiedUser(email);
        if (info is not null)
        {
            return (true, info.ChannelNumber);
        }
        return (false, 0);
    }


    private async Task<CertifiedUser> CachingUserInfo(string email, Int64 accountId, Int64 userId, Int32 appVersion, Int32 masterDataVersion)
    {
        // 재로그인이라면, 채널에서 나와야한다.
        var (isRelogin, channelNumber) = await IsRelogin(email);
        if (isRelogin == true)
        {
            _channelUserMgr.Leave(channelNumber, userId);
        }

        var enterdChannelNumber = _channelUserMgr.RandomEnter(userId);
        var info = CreateCertifiedUserInfo(email, accountId, userId, appVersion, masterDataVersion, enterdChannelNumber);
        if (await _memoryDb.RegistCertifiedUserInfo(email, info) != ErrorCode.None)
        {
            _channelUserMgr.Leave(enterdChannelNumber, userId);
            return null;
        }

        return info;
    }


    private CertifiedUser CreateCertifiedUserInfo(string email, Int64 accountId, Int64 userId, Int32 appVersion, Int32 masterDataVersion, Int32 channelNumber)
    {
        return new CertifiedUser
        {
            Email = email,
            AuthToken = Security.CreateAuthToken(),
            AccountId = accountId,
            UserId = userId,
            AppVersion = appVersion,
            MasterDataVersion = masterDataVersion,
            ChannelNumber = channelNumber
        };
    }



}
