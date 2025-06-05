using CloudStructures.Structures;
using GameServer.Models.DTO;
using GameServer.Models;
namespace GameServer.Repository.Interface;

public interface IMemoryDb
{
    public Task<ErrorCode> ResistUserAsync(string token, Int64 accountUid);
    public Task<(bool, RdbAuthUserData)> GetUserAsync(string id);
    public Task DeleteUserAsync(Int64 accountUid);
    public Task<bool> LockUserReqAsync(string key);
    public Task<bool> UnLockUserReqAsync(string key);

    // matching
    public Task<(bool, Guid)> GetMatchGUID(Int64 accountUid);
    public Task<HSGameInfo?> UpdateMatchInfo(Guid matchGUID, Int64 accountUid);
    
    // hearthstone
    public Task<HSGameInfo?> GetMatchInfo(Guid matchGUID);
    public Task<bool> MarkMatchCompleted(HSGameInfo gameInfo);
    public Task<bool> UpdateGameInfo(HSGameInfo gameInfo);
    public Task<HSPlayerState?> GetPlayerState(Guid matchGUID, Int64 accountUid);
    public Task<bool> UpdatePlayerState(Guid matchGUID, Int64 accountUid, HSPlayerState playerState);
    public Task<HSGameResult> GetMatchResult(Guid matchGUID);
}

