using System.Threading.Tasks;
using System;

namespace APIServer.Servicies.Interfaces;

public interface IAuthService
{
    public Task<ErrorCode> VerifyTokenToHive(Int64 playerId, string hiveToken);
    public Task<(ErrorCode, int)> VerifyUser(Int64 playerId);
    public Task<ErrorCode> UpdateLastLoginTime(int uid);
    public Task<(ErrorCode, string)> RegisterToken(int uid);
}
