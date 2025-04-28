namespace ServerShared.Repository.Interfaces;

public interface IGameDb<T> : IDisposable
{
	public Task<(ErrorCode, IEnumerable<T>?)> GetAll(Int64 uid);
	public Task<(ErrorCode, T?)> Get(Int64 uid);
	public Task<ErrorCode> Set(T data);
	public Task<ErrorCode> Update(Int64 uid, object value);
	public Task<ErrorCode> Delete(Int64 uid);
}
