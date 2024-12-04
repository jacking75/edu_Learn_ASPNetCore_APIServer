using System;
using System.Threading.Tasks;

using APIServer.ModelDB;

namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<bool> CreateAccount(string email, string password, Int64 userId);


    public Task<(ErrorCode, Account)> VerifyAccount(string email, string password);


    public Task<ErrorCode> DeleteAccount(Int64 accountId, string email);
}