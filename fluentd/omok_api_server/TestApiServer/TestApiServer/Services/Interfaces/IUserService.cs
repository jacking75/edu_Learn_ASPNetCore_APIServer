using TestApiServer.Models.Database;

namespace TestApiServer.Services.Interfaces;

public interface IUserService
{
	public Task<User?> Login(string username, string password);

}
