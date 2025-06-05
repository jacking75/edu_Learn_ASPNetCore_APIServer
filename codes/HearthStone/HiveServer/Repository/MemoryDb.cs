using CloudStructures;
using CloudStructures.Structures;
using HiveServer.Models;
using HiveServer.Repository.Interface;
using Microsoft.Extensions.Options;
using ZLogger;
using static Humanizer.In;

namespace HiveServer.Repository;

public class MemoryDb : IMemoryDb
{
    readonly RedisConnection _redisConn;
    readonly ILogger<MemoryDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;
    
    public MemoryDb(IOptions<DbConfig> dbConfig, ILogger<MemoryDb> logger)
    {
        _dbConfig = dbConfig;
        _logger = logger;
        RedisConfig redisConfig = new ("default", dbConfig.Value.Redis);
        _redisConn = new RedisConnection(redisConfig);
    }


    public async Task<ErrorCode> RegistUserAsync(Int64 accountUid, string hiveToken) 
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(accountUid.ToString());
        ErrorCode result = ErrorCode.None;

        RdbAuthUserData userdata = new()
        {
            accountUid = accountUid,
            hiveToken = hiveToken
        };

        try 
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
            if(await redis.SetAsync(userdata, LoginTimeSpan()) == false)
            {
                _logger.ZLogError($"[RegistUserAsync] Redis SetAsync Error. key : {key}");
                result = ErrorCode.LoginFail;
                return result;
            }
        } 
        catch 
        {
            _logger.ZLogError($"[RegistUserAsync] Redis SetAsync Error. key : {key}");
            result = ErrorCode.LoginFail;
            return result;
        }

        return result;
    }
    public TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromMinutes(RediskeyExpireTime.LoginKeyExpireMin);
    }

    public async Task<ErrorCode> CheckUserAuthAsync(Int64 accountUid, string hiveToken)
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(accountUid.ToString());
        ErrorCode result = ErrorCode.None;

        try 
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();

            if (!user.HasValue)
            {
                _logger.ZLogError($"[CheckUserAuthAsync] player_id = {accountUid}, hiveToken = {hiveToken}, ErrorMessage:ID does Not Exist");
                result = ErrorCode.HiveTokenInvalid;
                return result;
            }

            if (user.Value.accountUid != accountUid || user.Value.hiveToken != hiveToken)
            {
                _logger.ZLogError($"[CheckUserAuthAsync] player_id = {accountUid}, hiveToken = {hiveToken}, ErrorMessage:ID does Not Exist");
                result = ErrorCode.HiveTokenInvalid;
                return result;
            }
        } 
        catch
        {
            _logger.ZLogError($"[CheckUserAuthAsync] player_id = {accountUid}, hiveToken = {hiveToken}, ErrorMessage:ID does Not Exist");
            result = ErrorCode.HiveTokenInvalid;
            return result;
        }

        return result;
    }
}

public class MemoryDbKeyMaker
{
    const string loginUID = "UID_";
    const string userLockKey = "ULock_";

    public static string MakeUIDKey(string id)
    {
        return loginUID + id;
    }

    public static string MakeUserLockKey(string id)
    {
        return userLockKey + id;
    }
}

public class RediskeyExpireTime
{
    public const ushort NxKeyExpireSecond = 3;
    public const ushort RegistKeyExpireSecond = 6000;
    public const ushort LoginKeyExpireMin = 60;
    public const ushort TicketKeyExpireSecond = 6000; // 현재 테스트를 위해 티켓은 10분동안 삭제하지 않는다. 
}
