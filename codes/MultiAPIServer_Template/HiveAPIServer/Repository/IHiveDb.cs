using System;
using System.Threading.Tasks;

namespace MatchAPIServer.Repository;

public interface IHiveDb : IDisposable
{
    public Task<ErrorCode> CreateAccount(string userID, string pw);

    public Task<(ErrorCode, Int64)> VerifyUser(string userID, string pw);
}
