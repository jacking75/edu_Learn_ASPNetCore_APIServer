using CloudStructures;
using Microsoft.Extensions.Options;
using ZLogger;
using CloudStructures.Structures;
using System.Net.NetworkInformation;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace MatchServer.Repository;

public interface IMemoryDb
{
    Task RegistMatchWaitingInfo(MatchInfo matchInfo);
    Task RegistUserMatchWaiting(MatchInfo matchInfo);
}

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

    public async Task RegistMatchWaitingInfo(MatchInfo matchInfo)
    {
        var key = MemoryDbKeyMaker.MakeMatchWaitingKey(matchInfo.MatchGUID.ToString());
        try
        {
            RedisString<MatchWaiting> redis = new(_redisConn, key, MatchWaitingTimeSpan());
            MatchWaiting matchWaiting = new()
            {
                MatchInfo = matchInfo,
                MatchAcceptUserList = new List<Int64>(matchInfo.UserList)
            };

            await redis.SetAsync(matchWaiting, MatchWaitingTimeSpan());
        }
        catch
        {
            _logger.ZLogError($"[RegistUserMatchInfo] Key = {matchInfo.MatchGUID}, ErrorMessage:Redis Connection Error");
        }
    }

    public async Task RegistUserMatchWaiting(MatchInfo matchInfo)
    {
        foreach (Int64 accountUid in matchInfo.UserList)
        {
            var key = MemoryDbKeyMaker.MakeUserMatchWaitingKey(accountUid.ToString());
            try
            {
                RedisString<Guid> redis = new(_redisConn, key, MatchWaitingTimeSpan());

                await redis.SetAsync(matchInfo.MatchGUID, MatchWaitingTimeSpan());
            }
            catch
            {
                _logger.ZLogError($"[RegistUserMatchInfo] Key = {accountUid}, ErrorMessage:Redis Connection Error");
            }
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
    const string loginUID = "UID_";
    const string userLockKey = "ULOCK_";
    const string userItemKey = "UI_";
    const string userCurrencyKey = "UC_";
    const string userMatchKey = "UM_";
    const string userMatchWaitingKey = "UMW_";
    const string matchWaitingKey = "MW_";


    public static string MakeUIDKey(string id)
    {
        return loginUID + id;
    }

    public static string MakeUserLockKey(string id)
    {
        return userLockKey + id;
    }
    public static string MakeUserItemKey(string id)
    {
        return userItemKey + id;
    }
    public static string MakeUserCurrencyKey(string id)
    {
        return userCurrencyKey + id;
    }
    public static string MakeUserMatchKey(string id)
    {
        return userMatchKey + id;
    }
    public static string MakeMatchWaitingKey(string id)
    {
        return matchWaitingKey + id;
    }
    public static string MakeUserMatchWaitingKey(string id)
    {
        return userMatchWaitingKey + id;
    }
}


