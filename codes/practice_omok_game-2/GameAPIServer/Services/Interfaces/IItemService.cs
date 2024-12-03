using GameAPIServer.Repositories.Interfaces;

namespace GameAPIServer.Services.Interfaces;

public interface IItemService
{
	public Task<ErrorCode> InsertUserItem(Int64 uid, int itemId, int itemCount = 1);

}
