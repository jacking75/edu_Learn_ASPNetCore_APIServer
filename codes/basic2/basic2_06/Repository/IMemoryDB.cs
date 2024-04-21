

namespace basic2_06.Repository;

public interface IMemoryDB : IDisposable
{
    public Task<ErrorCode> RegistUserAsync(string id, string authToken, long accountId);

    public Task<(bool, MdbUserData)> GetUserAsync(string userID);
}
