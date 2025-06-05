namespace HiveServer.Services.Interface;

public interface IAuthService
{
    public Task<ErrorCode> CreateAccount(string emailId, string password, string nickname);
    public Task<(ErrorCode, Int64, string)> Login(string emailId, string password);
    public Task<ErrorCode> VerifyToken(Int64 accountUid, string hiveToken);
}
