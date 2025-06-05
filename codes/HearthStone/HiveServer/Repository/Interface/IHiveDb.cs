namespace HiveServer.Repository.Interface;

public interface IHiveDb : IDisposable
{
    public Task<int> DuplicateNickname(string nickname);
    
    public Task<ErrorCode> CreateAccount(string emailId, string password, string nickname);

    public Task<(ErrorCode, Int64)> VerifyUser(string emailId, string password);
}
