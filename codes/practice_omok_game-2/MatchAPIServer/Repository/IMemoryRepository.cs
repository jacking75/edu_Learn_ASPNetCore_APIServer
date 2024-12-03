using System;
using System.Threading.Tasks;

namespace MatchAPIServer.Repository;

public interface IMemoryRepository
{
	public Task DeleteDataAsync<T>(string key);
	public Task<bool> StoreDataAsync<T>(string key, T data, TimeSpan expiry);
	public Task<(ErrorCode, T)> GetDataAsync<T>(string key);
	
	public Task<bool> LockDataAsync<T>(string key, T data, TimeSpan expiry);
	public Task<bool> ExtendLockAsync<T>(string key, T data, TimeSpan expiry);
	public Task<bool> UnlockDataAsync<T>(string key, T data);

}
