using System;
using System.Threading.Tasks;


namespace basic_07.Services;

public interface IAccountDb : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(String id, String pw);
    
    public Task<Tuple<ErrorCode, Int64>> VerifyUser(String email, String pw);
}