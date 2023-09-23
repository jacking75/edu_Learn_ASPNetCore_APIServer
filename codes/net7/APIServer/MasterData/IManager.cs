using System;
using System.Threading.Tasks;

namespace APIServer.MasterData;

public interface IManager : IDisposable
{
    public Task<bool> Load(string connectionString);
}
