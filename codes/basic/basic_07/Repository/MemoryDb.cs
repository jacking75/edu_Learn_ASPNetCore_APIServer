using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;
using ZLogger;

namespace basic_07.Repository;

public class MemoryDb : IMemoryDb
{
    private readonly ILogger<AccountDb> _logger;
    public RedisConnection _redisConn;
  
    public MemoryDb(ILogger<AccountDb> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;

        RedisConfig config = new("default", dbConfig.Value.Redis);
        _redisConn = new RedisConnection(config);

        _logger.ZLogDebug($"userDbAddress:{dbConfig.Value.Redis}");
    }

    public void Dispose()
    {
    }

    public async Task<ErrorCode> RegistUserAsync(string email, string authToken, long accountId)
    {
        string key = MemoryDbKeyMaker.MakeUIDKey(email);
        ErrorCode result = ErrorCode.None;

        RdbAuthUserData user = new()
        {
            Email = email,
            AuthToken = authToken,
            AccountId = accountId,
            State = UserState.Default.ToString()
        };

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
            if (await redis.SetAsync(user, LoginTimeSpan()) == false)
            {
                result = ErrorCode.LoginFailAddRedis;
                return result;
            }
        }
        catch
        {
            result = ErrorCode.LoginFailAddRedis;
            return result;
        }

        return result;
    }

    public async Task<ErrorCode> CheckUserAuthAsync(string id, string authToken)
    {
        string key = MemoryDbKeyMaker.MakeUIDKey(id);
        ErrorCode result = ErrorCode.None;

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();

            if (!user.HasValue)
            {
                result = ErrorCode.CheckAuthFailNotExist;
                return result;
            }

            if (user.Value.Email != id || user.Value.AuthToken != authToken)
            {
                result = ErrorCode.CheckAuthFailNotMatch;
                return result;
            }
        }
        catch
        {
            result = ErrorCode.CheckAuthFailException;
            return result;
        }


        return result;
    }

   
    public async Task<(bool, RdbAuthUserData)> GetUserAsync(string id)
    {
        string uid = MemoryDbKeyMaker.MakeUIDKey(id);

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, uid, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();
            if (!user.HasValue)
            {
                return (false, null);
            }

            return (true, user.Value);
        }
        catch
        {
            return (false, null);
        }
    }
    
    

    public TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromMinutes(RediskeyExpireTime.LoginKeyExpireMin);
    }

    public TimeSpan TicketKeyTimeSpan()
    {
        return TimeSpan.FromSeconds(RediskeyExpireTime.TicketKeyExpireSecond);
    }

    public TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromSeconds(RediskeyExpireTime.NxKeyExpireSecond);
    }
}


public class RdbAuthUserData
{
    public string Email { get; set; } = "";
    public string AuthToken { get; set; } = "";
    public long AccountId { get; set; } = 0;
    public string State { get; set; } = ""; // enum UserState    
}