
using GameServer.Models.DTO;

namespace GameServer.Services.Interface;

public interface IMatchService
{
    public Task<ErrorCode> AddUser(Int64 accountUid);
    public Task<(ErrorCode, Guid)> GetMatchGUID(Int64 accountUid);
    public Task<(ErrorCode, HSGameInfo?)> GetMatch(Guid matchGuid, Int64 accountUid);
}