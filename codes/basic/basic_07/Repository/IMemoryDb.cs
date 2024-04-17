using System;
using System.Threading.Tasks;

namespace basic_07.Repository;

public interface IMemoryDb : IDisposable
{    
    public Task<ErrorCode> RegistUserAsync(string id, string authToken, long accountId);

    public Task<ErrorCode> CheckUserAuthAsync(string id, string authToken);

    public Task<(bool, RdbAuthUserData)> GetUserAsync(string id);

    
}
