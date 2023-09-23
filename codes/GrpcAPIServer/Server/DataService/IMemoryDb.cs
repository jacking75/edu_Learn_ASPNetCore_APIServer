public interface IMemoryDb
{
    public void Init(string address);

    public Task<ErrorCode> RegistUserAsync(string id, string authToken, Int64 accountId);

    public Task<ErrorCode> CheckUserAuthAsync(string id, string authToken);
    public Task<(bool, ModelMemoryDB.AuthUser)> GetUserAsync(string id);

    public Task<bool> SetUserStateAsync(ModelMemoryDB.AuthUser user, ModelMemoryDB.UserState userState);

    public Task<bool> SetUserReqLockAsync(string key);

    public Task<bool> DelUserReqLockAsync(string key);

}