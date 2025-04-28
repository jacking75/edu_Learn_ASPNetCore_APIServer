using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerShared.Repository.Interfaces;
using StackExchange.Redis;

namespace ServerShared.Repository;

public class MemoryDb : BaseLogger<MemoryDb>, IMemoryDb
{
    private readonly RedisConnection _redisConnection;

    public MemoryDb(ILogger<MemoryDb> logger, IOptions<ServerConfig> dbConfig) : base(logger)
    {
        _redisConnection = new RedisConnection(new RedisConfig("default", dbConfig.Value.Redis));
    }

    public async Task<ErrorCode> DeleteAsync<T>(string key)
    {
        try
        {
            RedisString<T> redisData = new(_redisConnection, key, null);
            var result = await redisData.DeleteAsync();

			return result ? ErrorCode.None : ErrorCode.RedisDeleteFail;
		}
        catch (Exception e)
        {
            ExceptionLog(e, $"{typeof(T).Name}:{key}");
            return ErrorCode.RedisDeleteException;
        }
    }

    public async Task<(ErrorCode, T?)> GetAndDeleteAsync<T>(string key)
    {
        try
        {
            RedisString<T> redisData = new(_redisConnection, key, null);
            RedisResult<T> result = await redisData.GetAndDeleteAsync();

            if (result.HasValue)
			{
				return (ErrorCode.None, result.Value);
			}
			else
			{
				ErrorLog(ErrorCode.RedisGetAndDeleteFail, $"{typeof(T).Name}:{key} {result}");
				return (ErrorCode.RedisGetAndDeleteFail, default(T));
			}
		}
        catch (Exception e)
        {
            ExceptionLog(e, $"{typeof(T).Name}:{key}");
            return (ErrorCode.RedisGetAndDeleteException, default(T));
        }

    }

    public async Task<(ErrorCode, T?)> GetAsync<T>(string key)
    {
        try
        {
            RedisString<T> redisData = new(_redisConnection, key, null);
            RedisResult<T> result = await redisData.GetAsync();

			return (ErrorCode.None, result.Value);
		}
        catch (Exception e)
        {
            ExceptionLog(e, $"{typeof(T).Name}:{key}");
            return (ErrorCode.RedisGetException, default(T));
        }
    }

	public async Task<ErrorCode> SetAsync<T>(string key, T data, TimeSpan? expiry = null)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, expiry);
			var result = await redisData.SetAsync(data);

			return result ? ErrorCode.None : ErrorCode.RedisUpdateFail;
		}
		catch (Exception e)
		{
			ExceptionLog(e, $"{typeof(T).Name}:{key}, Data: {data}");
			return ErrorCode.RedisUpdateException;
		}
	}

	public async Task<ErrorCode> SetExAsync<T>(string key, T data, TimeSpan? expiry = null)
    {
        try
        {
            RedisString<T> redisData = new(_redisConnection, key, expiry);
            var result = await redisData.SetAsync(data, null, When.Exists);

            return result ? ErrorCode.None : ErrorCode.RedisSetEXFail;

        }
        catch (Exception e)
        {
            ExceptionLog(e, $"{typeof(T).Name}:{key}, Data: {data}");
            return ErrorCode.RedisSetEXException;
        }
    }

    public async Task<ErrorCode> SetNxAsync<T>(string key, T data, TimeSpan? expiry = null)
    {
        try
        {
            RedisString<T> redisData = new(_redisConnection, key, expiry);
            var result = await redisData.SetAsync(data, null, When.NotExists);

            return result ? ErrorCode.None : ErrorCode.RedisSetNXFail;
        }
        catch (Exception e)
        {
            ExceptionLog(e, $"{typeof(T).Name}:{key}, Data: {data}");
            return ErrorCode.RedisSetNXException;
        }
    }

	public async Task<(ErrorCode, T?)> GetOrSetAsync<T>(string key, T data, TimeSpan? expiry = null)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, expiry);
			var result = await redisData.GetOrSetAsync(() => data, null);

			return result != null ? (ErrorCode.None, result) : (ErrorCode.RedisGetOrSetFail, default);
		}
		catch (Exception e)
		{
			ExceptionLog(e, $"{typeof(T).Name}:{key}, Data: {data}");
			return (ErrorCode.RedisGetOrSetException, default);
		}
	}

	public async Task<ErrorCode> LockAsync<T>(string key, T data, TimeSpan expiry)
	{
		try
		{
			RedisLock<T> redisData = new(_redisConnection, key);
			var result = await redisData.TakeAsync(data, expiry);

			return result ? ErrorCode.None : ErrorCode.RedisLockFail;

		}
		catch (Exception e)
		{
			ExceptionLog(e, $"{typeof(T).Name}:{key}, Data: {data}");
			return ErrorCode.RedisLockException;
		}
	}

	public async Task<ErrorCode> UnlockAsync<T>(string key, T data)
    {
        try
        {
            RedisLock<T> redisData = new(_redisConnection, key);
            var result = await redisData.ReleaseAsync(data);

            return result ? ErrorCode.None : ErrorCode.RedisUnlockFail;
        }
        catch (Exception e)
        {
            ExceptionLog(e, $"{typeof(T).Name}:{key}, Data: {data}");
            return ErrorCode.RedisUnlockException;
        }
    }

    public async Task<(ErrorCode, bool)> ExistsAsync<T>(string key)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, null);
			var result = await redisData.GetAsync();

			return (ErrorCode.None, result.HasValue);
		}
		catch (Exception e)
		{
			ExceptionLog(e, key);
			return (ErrorCode.RedisGetException, false);
		}
	}

	public async Task<ErrorCode> StringSetRangeAsync(string key, int offset, byte[] value)
    {
        try
        {
            IDatabase db = _redisConnection.GetConnection().GetDatabase();
            RedisValue result = await db.StringSetRangeAsync(key, offset, value);

            return result.HasValue ? ErrorCode.None : ErrorCode.RedisSetRangeFail;
		}
		catch (Exception e)
		{
            ExceptionLog(e, $" Key: {key}, Offset: {offset}");
            return ErrorCode.RedisSetRangeException;
        }
    }

	public async Task<(ErrorCode, RedisValue)> StringGetRangeAsync(string key, int start, int end)
	{
		try
		{
			IDatabase db = _redisConnection.GetConnection().GetDatabase();
			RedisValue result = await db.StringGetRangeAsync(key, start, end);

            if (result.HasValue)
            {
				return (ErrorCode.None, result);
			}

			return (ErrorCode.RedisGetRangeFail, RedisValue.Null);
		}
		catch (Exception e)
		{
			ExceptionLog(e, $"Key:{key}, Offset: {start}");
			return (ErrorCode.RedisGetRangeException, RedisValue.Null);
		}
	}
}
