using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using ZLogger;
using StackExchange.Redis;

namespace MatchAPIServer.Repository;


public class MemoryRepository : IMemoryRepository
{
	private readonly RedisConnection _redisConnection;
	private readonly ILogger<MemoryRepository> _logger;

	public MemoryRepository(IOptions<ServerConfig> dbConfig, ILogger<MemoryRepository> logger)
	{
		_redisConnection = new RedisConnection(new RedisConfig("default", dbConfig.Value.Redis));
		_logger = logger;
	}

	public async Task DeleteDataAsync<T>(string key)
	{
		try
		{
			RedisString<T> redisString = new(_redisConnection, key, null);
			await redisString.DeleteAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to delete data: Key={Key}", key);
		}
	}

	public async Task<bool> StoreDataAsync<T>(string key, T data, TimeSpan expiry)
	{
		try
		{
			RedisString<T> redisString = new(_redisConnection, key, expiry);
			return await redisString.SetAsync(data, null, When.NotExists);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to store data: Key={Key}, Data={Data}", key, data);
			return false;
		}
	}
	public async Task<(ErrorCode, T?)> GetDataAsync<T>(string key)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, null);
			RedisResult<T> result = await redisData.GetAsync();

			if (result.HasValue)
			{
				_logger.ZLogInformation(
				 $"[[Redis.GetDataAsync] Key = {key} Retrieved");
				return (ErrorCode.None, result.Value);  // Return the match data if found
			}
			else
			{
				return (ErrorCode.RedisMatchNotFound, default(T)); // Return an error if the match is not found
			}

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[FailedToRetrieve] KEY:{key}, ErrorMessage: {e.Message}");
			return (ErrorCode.RedisMatchGetException, default(T));
		}
	}

	public async Task<bool> LockDataAsync<T>(string key, T token, TimeSpan expiry)
	{
		try
		{
			var lockKey = SharedKeyGenerator.MakeLockKey(key);
			RedisLock<T> redisLock = new(_redisConnection, lockKey);
			return await redisLock.TakeAsync(token, expiry);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to lock data: Key={Key}, token={token}", key, token);
			return false;
		}
	}

	public async Task<bool> UnlockDataAsync<T>(string key, T token)
	{
		try
		{
			var lockKey = SharedKeyGenerator.MakeLockKey(key);
			RedisLock<T> redisData = new(_redisConnection, lockKey);
			return await redisData.ReleaseAsync(token);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to unlock data: Key={Key}, token={token}", key, token);
			return false;
		}
	}
	public async Task<bool> ExtendLockAsync<T>(string key, T token, TimeSpan expiry)
	{
		try
		{
			var lockKey = SharedKeyGenerator.MakeLockKey(key);
			RedisLock<T> redisData = new(_redisConnection, lockKey);
			return await redisData.ExtendAsync(token, expiry);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to extend lock data: Key={Key}, Data={token}", key, token);
			return false;
		}
	}
}
