using APIServer.DTO.Game;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class GameService :IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    readonly IMemoryDb _memoryDb;
    int initCharKey;

    public GameService(ILogger<GameService> logger, IGameDb gameDb, IMasterDb masterDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        initCharKey = _masterDb._characterList[0].char_key;
        _memoryDb = memoryDb;
    }

    public async Task<(ErrorCode, IEnumerable<GdbMiniGameInfo>)> GetMiniGameList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetMiniGameList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.GetMiniGameList] ErrorCode: {ErrorCode.MiniGameListFailException}, Uid: {uid}");
            return (ErrorCode.MiniGameListFailException, null);
        }
    }

    public async Task<ErrorCode> UnlockMiniGame(int uid, int gameKey)
    {
        try
        {
            var gameInfo = await _gameDb.GetMiniGameInfo(uid, gameKey);
            if (gameInfo != null)
            {
                _logger.ZLogDebug($"[Game.UnlockMiniGame] ErrorCode: { ErrorCode.MiniGameUnlockFailAlreadyUnlocked}, Uid: { uid}");
                return ErrorCode.MiniGameUnlockFailAlreadyUnlocked;
            }

            var rowCount = await _gameDb.InsertMiniGame(uid, initCharKey, gameKey);
            if(rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Game.UnlockMiniGame] ErrorCode: {ErrorCode.MiniGameUnlockFailInsert}, Uid: {uid}");
                return ErrorCode.MiniGameUnlockFailInsert;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.UnlockMiniGame] ErrorCode: {ErrorCode.MiniGameUnlockFailException}, Uid: {uid}");
            return ErrorCode.MiniGameUnlockFailException;
        }
    }

    public async Task<(ErrorCode, GdbMiniGameInfo)> GetMiniGameInfo(int uid, int gameKey)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetMiniGameInfo(uid, gameKey));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.GetMiniGameInfo] ErrorCode: {ErrorCode.MiniGameInfoFailException}, Uid: {uid}");
            return (ErrorCode.MiniGameInfoFailException, null);
        }
    }

    public async Task<ErrorCode> SaveMiniGame(int uid, int gameKey, int score, List<UsedFoodData> foods)
    {
        try
        {
            var gameInfo = await _gameDb.GetMiniGameInfo(uid, gameKey);

            if (gameInfo == null)
            {
                _logger.ZLogError($"[Game.SaveMiniGame] ErrorCode: {ErrorCode.MiniGameSaveFailGameLocked}, Uid: {uid}");
                return ErrorCode.MiniGameSaveFailGameLocked;
            }

            //점수 업데이트
            //게임 최고점수 갱신
            var rowCount = await _gameDb.UpdateBestscore(uid, gameKey, score);
            if(rowCount == 0)
            {
                //게임 시즌 최고 점수 갱신
                rowCount = await _gameDb.UpdateBestscoreCurSeason(uid, gameKey, score);
                if(rowCount == 0)
                {
                    // 최고 점수 갱신이 없을 때
                    await _gameDb.UpdateRecentPlayDt(uid, gameKey);
                }
                else
                {
                    await _gameDb.UpdateUserTotalBestScoreCurSeason(uid);
                }
            }
            else
            {
                await _gameDb.UpdateUserTotalBestScore(uid);
                var totalBestscore = await _gameDb.GetTotalBestscore(uid);
                //랭크 갱신
                await _memoryDb.SetUserScore(uid, totalBestscore);
            }

            //사용된 푸드 감소
            if(foods != null)
            {
                foreach (var food in foods)
                {
                    rowCount = await _gameDb.FoodDecrement(uid, food.FoodKey, food.FoodQty);
                    if (rowCount != 1)
                    {
                        _logger.ZLogError($"[Game.SaveMiniGame] ErrorCode: {ErrorCode.MiniGameSaveFailFoodDecrement}, Uid: {uid}");
                        return ErrorCode.MiniGameSaveFailFoodDecrement;
                    }
                }
            }
            
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Game.SaveMiniGame] ErrorCode: {ErrorCode.MiniGameSaveFailException}, Uid: {uid}");
            return ErrorCode.MiniGameSaveFailException;
        }
    }

    public async Task<(ErrorCode, int)> InitNewUserGameData(Int64 playerId, string nickname)
    {
        var transaction = _gameDb.GDbConnection().BeginTransaction();
        try
        {
            var (errorCode, uid) = await CreateUserAsync(playerId, nickname, transaction);
            if(errorCode != ErrorCode.None)
            {
                transaction.Rollback();
                return (errorCode,0);
            }

            var rowCount = await _gameDb.InsertInitCharacter(uid, initCharKey, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailCharacter, 0);
            }

            rowCount = await _gameDb.InsertInitGameList(uid, initCharKey, transaction);
            if (rowCount != 3)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailGameList, 0);
            }

            rowCount = await _gameDb.InsertInitMoneyInfo(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailMoney, 0);
            }

            rowCount = await _gameDb.InsertInitAttendance(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailAttendance, 0);
            }

            transaction.Commit();
            return (ErrorCode.None, uid);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.ZLogError(e,
                $"[Game.InitNewUserGameData] ErrorCode: {ErrorCode.InitNewUserGameDataFailException}, PlayerId : {playerId}");
            return (ErrorCode.GameSetNewUserListFailException, 0);
        }
        finally
        {
            transaction.Dispose();
        }
    }

    async Task<(ErrorCode,int)> CreateUserAsync(Int64 playerId, string nickname, IDbTransaction transaction)
    {
        try
        {
            if (string.IsNullOrEmpty(nickname))
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, nickname : {nickname}");
                return (ErrorCode.CreateUserFailNoNickname,0);
            }
            //nickname 중복 체크
            var existUser = await _gameDb.GetUserByNickname(nickname, transaction);
            if (existUser is not null)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailDuplicateNickname}, nickname : {nickname}");
                return (ErrorCode.CreateUserFailDuplicateNickname,0);
            }

            //유저 생성
            return (ErrorCode.None, await _gameDb.InsertUser(playerId, nickname, transaction));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailException}, PlayerId: {playerId}");
            return (ErrorCode.CreateUserFailException, 0);
        }
    }

    public async Task<ErrorCode> SetMiniGamePlayChar(int uid, int gameKey, int charKey)
    {
        try
        {
            var charInfo = await _gameDb.GetCharInfo(uid, charKey);
            if(charInfo == null)
            {
                return ErrorCode.CharNotExist;
            }

            var rowCount = await _gameDb.UpdateMiniGamePlayChar(uid, gameKey, charKey);
            if (rowCount != 1)
            {
                _logger.ZLogError($"[Game.SetMiniGamePlayChar] ErrorCode: {ErrorCode.MiniGameSetPlayCharFailUpdate}, Uid: {uid}");
                return ErrorCode.MiniGameSetPlayCharFailUpdate;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Game.SetMiniGamePlayChar] ErrorCode: {ErrorCode.MiniGameSetPlayCharFailException}, Uid: {uid}");
            return ErrorCode.MiniGameSetPlayCharFailException;
        }
    }
}
