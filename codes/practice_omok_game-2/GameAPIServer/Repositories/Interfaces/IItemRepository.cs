using GameAPIServer.Models.GameDb;

namespace GameAPIServer.Repositories.Interfaces;

public interface IItemRepository : IDisposable
{
	public Task<bool> InsertUserItem(UserItem item);
}
