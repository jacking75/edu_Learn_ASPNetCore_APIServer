using GameServer.Models.GameDb;

namespace GameServer.Services.Interfaces;

public interface IUserService
{
	public  Task<(ErrorCode, User?)> GetUser(Int64 uid);
	public Task<(ErrorCode, (Int64, string))> LoginUser(Int64 playerId, string token);
	public Task<ErrorCode> LogoutUser(Int64 uid);
	public Task<ErrorCode> UpdateLastAttendanceTime(Int64 uid);
	public Task<ErrorCode> UpdateNickname(Int64 uid, string nickname);
}
