using System.Threading.Tasks;
using System;

namespace GameAPIServer.Servicies.Interfaces;

public interface IAuthService
{
    public Task<(ErrorCode, Int64)> CreateAccount(string userID, string passWord);
        
    public Task<(ErrorCode, Int64, string)> Login(string userID, string passWord);


}
