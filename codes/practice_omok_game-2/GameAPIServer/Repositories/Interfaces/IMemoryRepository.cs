namespace GameAPIServer.Repositories.Interfaces;

public interface IMemoryRepository
{
	/* Generic */
	public Task<(ErrorCode, T?)> GetDataAsync<T>(string key);
	public Task<(ErrorCode, T?, TimeSpan?)> GetWithExpiry<T>(string key);
	public Task<(ErrorCode, T?)> GetAndDeleteAsync<T>(string key);
	public Task<bool> DeleteDataAsync<T>(string key);
	public Task<bool> StoreDataAsync<T>(string key, T data, TimeSpan expiry);
	public Task<bool> UpdateDataAsync<T>(string key, T data, TimeSpan expiry);

	/* Lock */
	public Task<bool> LockDataAsync<T>(string key, T data, TimeSpan expiry);
	public Task<bool> ExtendLockAsync<T>(string key, T data, TimeSpan expiry);
	public Task<bool> UnlockDataAsync<T>(string key, T data);

	/* User */
	public Task<(ErrorCode, RedisMatchData?)> GetMatchInfo(Int64 uid);
	public Task<(ErrorCode, RedisUserCurrentGame?)> GetUserGameInfo(Int64 uid);
	public Task<ErrorCode> SetUserGameInfo(RedisUserCurrentGame userInfo);

	/* Game */
	public Task<(ErrorCode, byte[]?)> GetGameAsync(string gameGuid);
	public Task<bool> SetGameAsync(string gameGuid, byte[] gameData);
}

