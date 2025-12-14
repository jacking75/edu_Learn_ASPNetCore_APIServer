using System;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IGameService
{
    public Task<ErrorCode> InitNewUserGameData(Int64 uid);
   
}
 