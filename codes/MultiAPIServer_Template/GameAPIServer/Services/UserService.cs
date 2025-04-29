using GameAPIServer.DTO.User;
using GameAPIServer.Models.GameDB;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace GameAPIServer.Servicies;

public class UserService : IUserService
{
    readonly ILogger<UserService> _logger;
    readonly IGameDb _gameDb;
    readonly IMemoryDb _memoryDb;

    public UserService(ILogger<UserService> logger, IGameDb gameDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _memoryDb = memoryDb;
    }

    public async Task<(ErrorCode, GdbUserInfo)> GetUserInfo(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetUserByUid(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[User.GetUserInfo] ErrorCode: {ErrorCode.UserInfoFailException}, Uid: {uid}");
            return (ErrorCode.UserInfoFailException, null);
        }
    }

    public async Task<(ErrorCode, GdbUserMoneyInfo)> GetUserMoneyInfo(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetUserMoneyById(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[User.GetUserMoneyInfo] ErrorCode: {ErrorCode.UserMoneyInfoFailException}, Uid: {uid}");
            return (ErrorCode.UserMoneyInfoFailException, null);
        }
    }

    public async Task<ErrorCode> SetUserMainChar(int uid, int charKey)
    {
        try
        {
            var charInfo = await _gameDb.GetCharInfo(uid, charKey);
            if (charInfo == null)
            {
                _logger.ZLogError($"[User.SetMainChar] ErrorCode: {ErrorCode.CharNotExist}, CharKey: {charKey}");
                return ErrorCode.CharNotExist;
            }

            await _gameDb.UpdateMainChar(uid, charKey);

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[User.SetMainChar] ErrorCode: {ErrorCode.SetMainCharFailException}, Uid: {uid}, CharKey: {charKey}");
            return ErrorCode.SetMainCharFailException;
        }
    }

    public async Task<(ErrorCode, OtherUserInfo)> GetOtherUserInfo(int uid)
    {
        try
        {
            var userInfo = await _gameDb.GetUserByUid(uid);
            if (userInfo == null)
            {
                _logger.ZLogError($"[User.GetOtherUserInfo] ErrorCode: {ErrorCode.UserNotExist}, Uid: {uid}");
                return (ErrorCode.UserNotExist, null);
            }

            var charInfo = await _gameDb.GetCharInfo(uid, userInfo.main_char_key);

            var (errorCode, rank) = await _memoryDb.GetUserRankAsync(uid);

            if(errorCode != ErrorCode.None)
            {
                _logger.ZLogError($"[User.GetOtherUserInfo] ErrorCode: {errorCode}, Uid: {uid}");
                return (errorCode, null);
            }

            return (ErrorCode.None, new OtherUserInfo
            {
                uid = uid,
                nickname = userInfo.nickname,
                total_bestscore = userInfo.total_bestscore,
                total_bestscore_cur_season = userInfo.total_bestscore_cur_season,
                total_bestscore_prev_season = userInfo.total_bestscore_prev_season,
                main_char_key = userInfo.main_char_key,
                main_char_skin_key = charInfo.skin_key,
                main_char_costume_json = charInfo.costume_json,
                rank = rank,
            });

        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[User.GetOtherUserInfo] ErrorCode: {ErrorCode.GetOtherUserInfoFailException}, Uid: {uid}");
            return (ErrorCode.GetOtherUserInfoFailException, null);
        }
    }
}
