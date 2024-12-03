using Microsoft.Extensions.Options;
using CloudStructures;
using CloudStructures.Structures;
using ZLogger;
using GameAPIServer.Repositories.Interfaces;
using StackExchange.Redis;

namespace GameAPIServer.Repositories;

public class MemoryRepository : IMemoryRepository
{
	readonly ILogger<MemoryRepository> _logger;
	readonly RedisConnection _redisConnection;

	public MemoryRepository(ILogger<MemoryRepository> logger,IOptions<ServerConfig> dbConfig)
	{
		_redisConnection = new RedisConnection(new RedisConfig("default", dbConfig.Value.Redis));
		_logger = logger;
	}

	public async Task<bool> DeleteDataAsync<T>(string key)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, null);
			return await redisData.DeleteAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to delete data: Key={Key}", key);
			return false;
		}
	}

	public async Task<bool> StoreDataAsync<T>(string key, T data, TimeSpan expiry)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, expiry);
			return await redisData.SetAsync(data, null, When.NotExists);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to store data: Key={Key}, Data={Data}", key, data);
			return false;
		}
	}

	public async Task<bool> UpdateDataAsync<T>(string key, T data, TimeSpan expiry)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, expiry);
			return await redisData.SetAsync(data);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to update data: Key={Key}, Data={Data}", key, data);
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
				return (ErrorCode.None, result.Value);  // Return the match data if found
			}
			else
			{
				return (ErrorCode.RedisDataNotFound, default(T)); // Return an error if the match is not found
			}

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[FailedToRetrieve] KEY:{key}, ErrorMessage: {e.Message}");
			return (ErrorCode.RedisDataGetException, default(T));
		}
	}

	public async Task<bool> LockDataAsync<T>(string key, T data, TimeSpan expiry)
	{
		try
		{
			var lockKey = SharedKeyGenerator.MakeLockKey(key);
			RedisLock<T> redisData = new(_redisConnection, lockKey);
			return await redisData.TakeAsync(data, expiry);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to store data: Key={Key}, Data={Data}", key, data);
			return false;
		}
	}

	public async Task<bool> UnlockDataAsync<T>(string key, T data)
	{
		try
		{
			var lockKey = SharedKeyGenerator.MakeLockKey(key);
			RedisLock<T> redisData = new(_redisConnection, lockKey);
			return await redisData.ReleaseAsync(data);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to store data: Key={Key}, Data={Data}", key, data);
			return false;
		}
	}

	public async Task<bool> ExtendLockAsync<T>(string key, T data, TimeSpan expiry)
	{
		try
		{
			var lockKey = SharedKeyGenerator.MakeLockKey(key);
			RedisLock<T> redisData = new(_redisConnection, lockKey);
			return await redisData.ExtendAsync(data, expiry);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to extend lock data: Key={Key}, Data={Data}", key, data);
			return false;
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
				return (ErrorCode.None, result.Value);  // Return the match data if found
			}
			else
			{
				return (ErrorCode.RedisDataNotFound, default(T)); // Return an error if the match is not found
			}

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[FailedToRetrieve] KEY:{key}, ErrorMessage: {e.Message}");
			return (ErrorCode.RedisDataGetException, default(T));
		}
	}

	public async Task<(ErrorCode, T?, TimeSpan?)> GetWithExpiry<T>(string key)
	{
		try
		{
			RedisString<T> redisData = new(_redisConnection, key, null);
			RedisResultWithExpiry<T> result = await redisData.GetWithExpiryAsync();

			if (result.HasValue)
			{
				return (ErrorCode.None, result.Value, result.Expiry);  // Return the match data if found
			}
			else
			{
				return (ErrorCode.RedisDataNotFound, default, null ); // Return an error if the match is not found
			}

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[FailedToRetrieve] KEY:{key}, ErrorMessage: {e.Message}");
			return (ErrorCode.RedisDataGetException, default, null);
		}
	}

	public async Task<(ErrorCode, byte[]?)> GetGameAsync(string gameGuid)
	{
		try
		{
			var key = SharedKeyGenerator.MakeGameDataKey(gameGuid);

			var redisData = new RedisString<byte[]>(_redisConnection, key, null);
			var result = await redisData.GetAsync();

			if (result.HasValue)
			{
				return (ErrorCode.None, result.Value);
			}

			return (ErrorCode.RedisGameNotFound, null);

		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get game data: gameGuid={gameGuid}", gameGuid);
			return (ErrorCode.RedisGameEnterException, null);
		}
	}
	public async Task<bool> SetGameAsync(string gameGuid, byte[] gameData)
	{
		try
		{
			var key = SharedKeyGenerator.MakeGameDataKey(gameGuid);
			var redisData = new RedisString<byte[]>(_redisConnection, key, RedisExpiryTimes.GameDataExpiry);

			if (false == await redisData.SetAsync(gameData))
			{
				return false;
			}

			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get game data: gameGuid={gameGuid}", gameGuid);
			return false;
		}
	}

	public async Task<(ErrorCode, RedisMatchData?)> GetMatchInfo(Int64 uid)
	{
		try
		{
			var key = SharedKeyGenerator.MakeMatchDataKey(uid.ToString());
			var redisData = new RedisString<RedisMatchData>(_redisConnection, key, null);
			var result = await redisData.GetAsync();

			if (result.HasValue)
			{
				return (ErrorCode.None, result.Value);
			}

			return (ErrorCode.RedisMatchNotFound, null);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get match: Uid={Uid}", uid);
			return (ErrorCode.RedisMatchGetException, null);
		}
	}

	public async Task<(ErrorCode, RedisUserCurrentGame?)> GetUserGameInfo(Int64 uid)
	{
		try
		{
			var key = SharedKeyGenerator.MakeUserGameKey(uid.ToString());
			var redisData = new RedisString<RedisUserCurrentGame>(_redisConnection, key, RedisExpiryTimes.UserDataExpiry);
			var result = await redisData.GetAsync();

			if (result.HasValue)
			{
				await SetUserGameInfo(result.Value);
				return (ErrorCode.None, result.Value);
			}

			return (ErrorCode.RedisUserGameNotFound, null);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get current user game: Uid={Uid}", uid);
			return (ErrorCode.RedisUserGameGetException, null);
		}
	}

	public async Task<ErrorCode> SetUserGameInfo(RedisUserCurrentGame userInfo)
	{
		try
		{
			var key = SharedKeyGenerator.MakeUserGameKey(userInfo.Uid.ToString());
			var redisData = new RedisString<RedisUserCurrentGame>(_redisConnection, key, RedisExpiryTimes.UserDataExpiry);
			var result = await redisData.SetAsync(userInfo);

			if (true == result)
			{
				return ErrorCode.None;
			}

			return ErrorCode.RedisUserGameStoreFail;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to set current user game: Uid={Uid}", userInfo.Uid);
			return ErrorCode.RedisUserGameSetException;
		}
	}

}


