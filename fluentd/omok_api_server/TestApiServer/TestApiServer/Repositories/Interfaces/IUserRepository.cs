using TestApiServer.Models.Database;

namespace TestApiServer.Repositories.Interfaces;

public interface IUserRepository : IDisposable
{
	public Task<User?> GetUserByIdAsync(Int64 id);
}
