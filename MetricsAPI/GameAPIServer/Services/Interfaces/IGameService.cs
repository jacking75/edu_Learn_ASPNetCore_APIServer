using GameAPIServer.Models.DAO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameAPIServer.Servicies.Interfaces;

public interface IGameService
{
    public Task<(ErrorCode, int)> InitNewUserGameData(Int64 playerId, string nickname);
   
}
 