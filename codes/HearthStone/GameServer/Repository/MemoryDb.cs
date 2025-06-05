using CloudStructures;
using Microsoft.Extensions.Options;
using GameServer.Repository.Interface;
using ZLogger;
using GameServer.Models;
using GameServer.Models.DTO;
using CloudStructures.Structures;
using System.Net.NetworkInformation;
using GameServer.Services.Interface;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace GameServer.Repository;

public class MemoryDb : IMemoryDb
{
    readonly RedisConnection _redisConn;
    readonly ILogger<MemoryDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;

    public MemoryDb(ILogger<MemoryDb> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;
        _dbConfig = dbConfig;
        RedisConfig config = new("default", _dbConfig.Value.Redis);
        _redisConn = new RedisConnection(config);
    }

    public async Task<ErrorCode> ResistUserAsync(string token, Int64 accountUid) 
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(accountUid.ToString());
        ErrorCode result = ErrorCode.None;

        RdbAuthUserData user = new()
        {
            AccountUid = accountUid,
            Token = token
        };

        try 
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
            if (await redis.SetAsync(user, LoginTimeSpan()) == false)
            {
                _logger.ZLogError($"[RegistUserAsync] Uid:{accountUid}, Token:{token},ErrorMessage:UserBasicAuth, RedisString set Error");
                result = ErrorCode.LoginFail;
                return result;
             }
        }
        catch 
        {
            _logger.ZLogError($"[RegistUserAsync] Uid:{accountUid}, Token:{token},ErrorMessage:UserBasicAuth, RedisString set Error");
            result = ErrorCode.LoginFail;
            return result;
        }

        return result;
    }

    public async Task<(bool, RdbAuthUserData)> GetUserAsync(string uid)
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(uid);

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();
            if (!user.HasValue)
            {
                _logger.ZLogError(
                    $"[GetUserAsync] UID = {uid}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (false, null);
            }

            return (true, user.Value);
        }
        catch 
        {
            _logger.ZLogError($"[GetUserAsync] UID:{uid},ErrorMessage:ID does Not Exist");
            return (false, null);
        }
    }

    public async Task<bool> LockUserReqAsync(string key) 
    {
        try 
        {
            RedisString<RdbAuthUserData> redist = new(_redisConn, key, NxKeyTimeSpan());
            if (await redist.SetAsync(new RdbAuthUserData {}, NxKeyTimeSpan(), StackExchange.Redis.When.NotExists) == false) 
            {
                return false;
            }

            return true;
        } 
        catch 
        {
            _logger.ZLogError($"[SetUserReqLockAsync] Key = {key}, ErrorMessage:Redis Connection Error");

            return false;
        }
    }

    public async Task<bool> UnLockUserReqAsync(string key) 
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            var redisResult = await redis.DeleteAsync();
            return redisResult;
        }
        catch
        {
            _logger.ZLogError($"[DelUserReqLockAsync] Key = {key}, ErrorMessage:Redis Connection Error");
            return false;
        }
    }
    public async Task<(bool, Guid)> GetMatchGUID(Int64 accountUid)
    {
        var key = MemoryDbKeyMaker.MakeUserMatchWaitingKey(accountUid.ToString());
        try
        {
            RedisString<Guid> redis = new(_redisConn, key, null);

            var result = await redis.GetAsync();
            return (result.HasValue, result.HasValue ? result.Value : Guid.Empty);
        }
        catch
        {
            _logger.ZLogError($"[RegistUserMatchInfo] Key = {accountUid}, ErrorMessage:Redis Connection Error");
            return (false, Guid.Empty);
        }
    }

    async Task<bool> CheckMatchComplete(RedisString<MatchWaiting> redis, Int64 accountUid, MatchWaiting matchWaiting)
    {
        try
        {
            matchWaiting.MatchAcceptUserList.Remove(accountUid);

            await redis.SetAsync(matchWaiting, MatchWaitingTimeSpan());
            return (matchWaiting.MatchAcceptUserList.Count <= 0);
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[CheckMatchComplete] Key = {accountUid}, Error: {ex.Message}");
            return false;
        }
    }

    public async Task<HSGameInfo?> UpdateMatchInfo(Guid matchGUID, Int64 accountUid)
    {
        var key = MemoryDbKeyMaker.MakeMatchWaitingKey(matchGUID);
        try
        {
            RedisString<MatchWaiting> redis = new(_redisConn, key, null);
            var result = await redis.GetAsync();
            if (result.HasValue)
            { 
               if (await CheckMatchComplete(redis, accountUid,result.Value))
                {
                    await CleanMatchWaiting(result.Value.MatchInfo.MatchGUID);
                    await CleanUserMatchWaiting(result.Value.MatchInfo.UserList);

                    return  await RegistGameInfo(CreateGameInfo(result.Value.MatchInfo));
                }  
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[UpdateUserMatchWaitingInfo] Key = {matchGUID}, Error: {ex.Message}");
            return null;
        }
    }

    async Task CleanMatchWaiting(Guid matchGuid)
    {
        try
        {
            var key = MemoryDbKeyMaker.MakeMatchWaitingKey(matchGuid);
            RedisString<MatchWaiting> redis = new(_redisConn, key, null);
            await redis.DeleteAsync();
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[CleanupMatchWaitingInfo] 매치 대기 정보 삭제 실패. MatchGUID = {matchGuid}, Error: {ex.Message}");
        }
    }
    async Task CleanUserMatchWaiting(List<Int64> userList)
    {
        try
        {
            foreach (var userId in userList)
            {
                var userWaitingKey = MemoryDbKeyMaker.MakeUserMatchWaitingKey(userId.ToString());
                RedisString<Guid> userRedis = new(_redisConn, userWaitingKey, null);
                await userRedis.DeleteAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[CleanupMatchWaitingInfo] 사용자 매치 대기 정보 삭제 실패. UserId = , Error: {ex.Message}");
        }
    }

    async Task<HSGameInfo> RegistGameInfo(HSGameInfo gameInfo)
    {
        var key = MemoryDbKeyMaker.MakeMatchKey(gameInfo.MatchGUID);
        try
        {
            RedisString<HSGameInfo> redis = new(_redisConn, key, MatchTimeSpan());
            await redis.SetAsync(gameInfo, MatchTimeSpan());
            return gameInfo;
        }
        catch
        {
            _logger.ZLogError($"[DelUserReqLockAsync] Key = {key}, ErrorMessage:Redis Connection Error");
            return null;
        }
    }

    HSGameInfo CreateGameInfo(MatchInfo matchInfo)
    {
        HSGameInfo gameInfo = new HSGameInfo
        {
            MatchGUID = matchInfo.MatchGUID,
            MatchType = matchInfo.MatchType,
            StartTime = DateTime.UtcNow.AddSeconds(Global.START_TIME),
            CurrentTurnUid = matchInfo.UserList[0],
            GameUserList = new List<HSGameUserInfo>()
        };

        // 각 유저의 게임 정보 설정
        foreach (var uid in matchInfo.UserList)
        {
            // 각 유저에게 Global에서 설정한 기본 HP와 Mana 값을 할당
            HSGameUserInfo userInfo = new HSGameUserInfo
            {
                AccountUid = uid,
                Hp = Global.SET_HP, // 기본 체력 설정 (10)
                Mana = Global.SET_MANA // 기본 마나 설정 (1)
            };

            gameInfo.GameUserList.Add(userInfo);
        }

        return gameInfo;
    }

    // heartstone
    public async Task<HSGameInfo?> GetMatchInfo(Guid matchGUID)
    {
        var key = MemoryDbKeyMaker.MakeMatchKey(matchGUID);
        try
        {
            RedisString<HSGameInfo> redis = new(_redisConn, key, null);
            var result = await redis.GetAsync();

            return result.HasValue ? result.Value : null;
        }
        catch
        {
            _logger.ZLogError($"[GetMatchInfo] Key = {matchGUID}, ErrorMessage:Redis Connection Error");
            return null;
        }
    }

    public async Task<bool> UpdateGameInfo(HSGameInfo gameInfo)
    {
        var key = MemoryDbKeyMaker.MakeMatchKey(gameInfo.MatchGUID);
        try
        {
            RedisString<HSGameInfo> redis = new(_redisConn, key, MatchTimeSpan());
            return await redis.SetAsync(gameInfo, MatchTimeSpan());
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[UpdateGameInfo] matchGUID={gameInfo.MatchGUID}, Error:{ex.Message}");
            return false;
        }
    }

    public async Task<HSPlayerState?> GetPlayerState(Guid matchGUID, Int64 accountUid)
    {
        var key = MemoryDbKeyMaker.MakePlayerStateKey(matchGUID.ToString(), accountUid.ToString());
        try
        {
            RedisString<HSPlayerState> redis = new(_redisConn, key, null);
            var result = await redis.GetAsync();

            return (result.HasValue) ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[GetPlayerState] Error getting player state: {accountUid}, Match: {matchGUID}, Error: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdatePlayerState(Guid matchGUID, Int64 accountUid, HSPlayerState playerState)
    {
        var key = MemoryDbKeyMaker.MakePlayerStateKey(matchGUID.ToString(), accountUid.ToString());
        try
        {
            RedisString<HSPlayerState> redis = new(_redisConn, key, MatchTimeSpan());
            return await redis.SetAsync(playerState, MatchTimeSpan());
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[UpdatePlayerState] Error updating player state: {accountUid}, Match: {matchGUID}, Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> MarkMatchCompleted(HSGameInfo gameInfo)
    {
        try
        {
            // 1. 게임 결과 저장
            if (!await SaveMatchResult(gameInfo))
                return false;

            // 2. 플레이어 상태 데이터 정리
            await CleanPlayerStates(gameInfo);

            // 3. 게임 정보 TTL 조정 (클라이언트가 잠시 더 조회할 수 있도록)
            await UpdateGameInfoTTL(gameInfo);

            _logger.ZLogInformation($"[MarkMatchCompleted] Match completed and data cleanup performed. matchGUID={gameInfo.MatchGUID}, winner={gameInfo.WinnerUid}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[MarkMatchCompleted] Failed to mark match as completed. matchGUID={gameInfo.MatchGUID}, Error:{ex.Message}");
            return false;
        }
    }

    private async Task<bool> SaveMatchResult(HSGameInfo gameInfo)
    {
        // 필요한 결과 데이터만 추출
        var gameResult = new HSGameResult
        {
            MatchGUID = gameInfo.MatchGUID,
            MatchType = gameInfo.MatchType,
            StartTime = gameInfo.StartTime,
            EndTime = DateTime.UtcNow,
            PlayerUIDs = gameInfo.GameUserList.Select(u => u.AccountUid).ToList(),
            WinnerUid = gameInfo.WinnerUid,
            TurnCount = gameInfo.TurnCount,
            IsGameOver = true
        };

        // 결과 데이터는 1일간 보존
        var resultKey = MemoryDbKeyMaker.MakeMatchResultKey(gameInfo.MatchGUID);
        var resultRedis = new RedisString<HSGameResult>(_redisConn, resultKey, TimeSpan.FromDays(1));

        if (!await resultRedis.SetAsync(gameResult, TimeSpan.FromDays(1)))
        {
            _logger.ZLogError($"[SaveMatchResult] Failed to save match result. matchGUID={gameInfo.MatchGUID}");
            return false;
        }

        return true;
    }

    private async Task CleanPlayerStates(HSGameInfo gameInfo)
    {
        var tasks = gameInfo.GameUserList.Select(player =>
        {
            var playerStateKey = MemoryDbKeyMaker.MakePlayerStateKey(
                gameInfo.MatchGUID.ToString(),
                player.AccountUid.ToString());
            var playerRedis = new RedisString<HSPlayerState>(_redisConn, playerStateKey, null);
            return playerRedis.DeleteAsync();
        });

        await Task.WhenAll(tasks);
    }

    private async Task UpdateGameInfoTTL(HSGameInfo gameInfo)
    {
        var key = MemoryDbKeyMaker.MakeMatchKey(gameInfo.MatchGUID);
        var redis = new RedisString<HSGameInfo>(_redisConn, key, TimeSpan.FromMinutes(5));
        await redis.SetAsync(gameInfo, TimeSpan.FromMinutes(5));
    }

    public async Task<HSGameResult> GetMatchResult(Guid matchGUID)
    {
        var key = MemoryDbKeyMaker.MakeMatchResultKey(matchGUID);
        try
        {
            RedisString<HSGameResult> redis = new(_redisConn, key, null);
            var result = await redis.GetAsync();
            return result.HasValue ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[GetMatchResult] Error retrieving match result: {matchGUID}, Error: {ex.Message}");
            return null;
        }
    }

    public async Task DeleteUserAsync(Int64 accountUid)  
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(accountUid.ToString());
        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            await redis.DeleteAsync();
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[DeleteUserAsync] Error : {accountUid} Error: {ex.Message}");
            return;
        }
    }

    public TimeSpan LoginTimeSpan() 
    {
        return TimeSpan.FromMinutes(RediskeyExpireTime.LoginKeyExpireMin);
    }

    public TimeSpan NxKeyTimeSpan() 
    {
        return TimeSpan.FromSeconds(RediskeyExpireTime.NxKeyExpireSecond);
    }
    public TimeSpan MatchTimeSpan()
    {
        return TimeSpan.FromMinutes(RediskeyExpireTime.MatchKeyExpireMin);
    }
    public TimeSpan MatchWaitingTimeSpan()
    {
        return TimeSpan.FromMinutes(RediskeyExpireTime.MatchWaitingKeyExpireMin);
    }
}

public class RediskeyExpireTime
{
    public const ushort LoginKeyExpireMin = 60;
    public const ushort NxKeyExpireSecond = 3;  
    public const ushort MatchKeyExpireMin = 30;
    public const ushort MatchWaitingKeyExpireMin = 5;
}

public class MemoryDbKeyMaker
{
    public static string MakeUIDKey(string id)
    {
        return "UID_" + id;
    }

    public static string MakeUserLockKey(string id)
    {
        return "ULOCK_" + id;
    }
    public static string MakeMatchWaitingKey(Guid matchGUID)
    {
        return "MW_" + matchGUID.ToString();
    }
    public static string MakeUserMatchWaitingKey(string id)
    {
        return "UMW_" + id;
    }
    public static string MakeMatchKey(Guid matchGUID)
    {
        return "M_" + matchGUID.ToString();
    }
    public static string MakePlayerStateKey(string matchGUID, string accountUid)
    {
        return "PS_" + matchGUID + "_" + accountUid;
    }
    public static string MakeMatchResultKey(Guid matchGUID)
    {
        return "MR_" + matchGUID.ToString();
    }
}


