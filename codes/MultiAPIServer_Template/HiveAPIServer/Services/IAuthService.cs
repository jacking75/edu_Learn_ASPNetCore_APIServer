using System.Threading.Tasks;
using System;

namespace MatchAPIServer.Services;

public interface IAuthService
{
    public Task<ErrorCode> CreateAccount(string userID, string passWord);

    public Task<(ErrorCode, Int64, string)> Login(string userID, string passWord);


    public Task<ErrorCode> VerifyToken(Int64 playerId, string hiveToken);
}
