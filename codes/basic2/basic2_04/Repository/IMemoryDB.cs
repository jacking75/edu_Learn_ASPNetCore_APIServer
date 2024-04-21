using System;


namespace basic2_04.Repository;

public interface IMemoryDB : IDisposable
{
    public Task<ErrorCode> RegistUserAsync(string id, string authToken, long accountId);
}
