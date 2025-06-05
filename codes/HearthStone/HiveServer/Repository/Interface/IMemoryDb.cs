
namespace HiveServer.Repository.Interface;

public interface IMemoryDb
{
    public Task<ErrorCode> RegistUserAsync(Int64 accountUid, string hiveToken);
    public Task<ErrorCode> CheckUserAuthAsync(Int64 accountUid, string hiveToken);
}
