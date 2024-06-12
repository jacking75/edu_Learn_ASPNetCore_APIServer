using APIServer.Models.GameDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces;

public interface IGameService
{
    public Task<(ErrorCode, IEnumerable<GdbMiniGameInfo>)> GetMiniGameList(int uid);
    public Task<ErrorCode> UnlockMiniGame(int uid, int gameId);
    public Task<(ErrorCode, GdbMiniGameInfo)> GetMiniGameInfo(int uid, int gameId);
    
    public Task<(ErrorCode, int)> InitNewUserGameData(Int64 playerId, string nickname);
    public Task<ErrorCode> SetMiniGamePlayChar(int uid, int gameKey, int charKey);
}
 