using System;


namespace basic2_03.Repository;

public interface IGameDB : IDisposable
{
    public Task<Tuple<ErrorCode, Int64>> AuthCheck(String email, String pw);
}
