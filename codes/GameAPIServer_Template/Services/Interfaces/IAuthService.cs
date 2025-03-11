using System.Threading.Tasks;
using System;

namespace APIServer.Servicies.Interfaces;

public interface IAuthService
{
    public Task<ErrorCode> CreateAccount(string userID, string passWord);
        
    public Task<(ErrorCode, Int64, string)> Login(string userID, string passWord);

    //public Task<ErrorCode> UpdateLastLoginTime(int uid);

    //public Task<(ErrorCode, string)> RegisterToken(int uid);
}
