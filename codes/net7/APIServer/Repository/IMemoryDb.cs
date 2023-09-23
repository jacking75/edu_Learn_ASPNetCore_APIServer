using APIServer.Model.DAO;
using System.Threading.Tasks;

namespace APIServer.Repository;

public interface IMemoryDb
{
    public void Init(string address);

    public Task<ErrorCode> RegistUserAsync(string id, string authToken, long accountId);

    public Task<ErrorCode> CheckUserAuthAsync(string id, string authToken);
    public Task<(bool, RdbAuthUserData)> GetUserAsync(string id);

    public Task<bool> SetUserStateAsync(RdbAuthUserData user, UserState userState);

    public Task<bool> SetUserReqLockAsync(string key);

    public Task<bool> DelUserReqLockAsync(string key);

}
