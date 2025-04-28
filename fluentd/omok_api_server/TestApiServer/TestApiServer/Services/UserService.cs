using TestApiServer.Models.Database;
using TestApiServer.Repositories.Interfaces;
using TestApiServer.Services.Interfaces;

namespace TestApiServer.Services;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	public UserService(IUserRepository userRepository) 
	{
		_userRepository = userRepository;
	}

	public Task<User?> Login(string username, string password)
	{
		var randomNumber = new Random().Next(1, 10);
		// some login logic
		var user = _userRepository.GetUserByIdAsync(randomNumber);

		return user;
	}
}
