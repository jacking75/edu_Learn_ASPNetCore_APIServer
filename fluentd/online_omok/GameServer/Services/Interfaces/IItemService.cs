using GameServer.Models.GameDb;

namespace GameServer.Services.Interfaces;

public interface IItemService
{
	public Task<ErrorCode> InsertUserItem(Int64 uid, int itemId, int itemCount = 1);
	public Task<(ErrorCode, IEnumerable<UserItem>?)> GetUserItemsByUid(Int64 uid);
}
