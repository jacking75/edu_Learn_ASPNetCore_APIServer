using System;


namespace basic2_03.Repository;

public interface IMemoryDB : IDisposable
{
    public Task<ErrorCode> RegistUserAsync(string id, string authToken, long accountId);
}
