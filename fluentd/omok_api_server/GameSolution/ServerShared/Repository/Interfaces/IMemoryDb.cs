using StackExchange.Redis;

namespace ServerShared.Repository.Interfaces;

public interface IMemoryDb
{
	public Task<(ErrorCode, T?)> GetAsync<T>(string key);
	public Task<(ErrorCode, T?)> GetAndDeleteAsync<T>(string key);
	public Task<(ErrorCode, T?)> GetOrSetAsync<T>(string key, T data, TimeSpan? expiry = null);

	public Task<ErrorCode> DeleteAsync<T>(string key);
	public Task<ErrorCode> LockAsync<T>(string key, T data, TimeSpan expiry);
	public Task<ErrorCode> UnlockAsync<T>(string key, T data);
	public Task<ErrorCode> SetAsync<T>(string key, T data, TimeSpan? expiry = null);
	public Task<ErrorCode> SetExAsync<T>(string key, T data, TimeSpan? expiry = null);
	public Task<ErrorCode> SetNxAsync<T>(string key, T data, TimeSpan? expiry = null);

	public Task<ErrorCode> StringSetRangeAsync(string key, int offset, byte[] value);
	public Task<(ErrorCode, RedisValue)> StringGetRangeAsync(string key, int start, int end);
}