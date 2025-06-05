namespace GameServer.Services.Interface;

public interface IAuthService
{
    public Task<(ErrorCode, string)> Verify(Int64 accountUid, string hiveToken);
    public Task<ErrorCode> VerifyTokenToHive(Int64 accountUid, string hiveToken);
    public Task<ErrorCode> VerifyUser(Int64 accountUid);
    public Task<(ErrorCode, string)> RegistToken(Int64 accountUid);
    public Task<ErrorCode> UpdateLastLoginTime(Int64 accountUid);
    public Task LogOut(Int64 accountUid);

}
