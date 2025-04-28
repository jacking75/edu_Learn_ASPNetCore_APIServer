using GameServer.Models;
using GameServer.Models.GameDb;
using GameServer.Services.Interfaces;
using GameShared.DTO;
using ServerShared.Redis;
using ServerShared.Repository.Interfaces;
using ServerShared;

namespace GameServer.Services;

public class UserService : BaseLogger<UserService>, IUserService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IGameDb<User> _gameDb;
	private readonly IMemoryDb _memoryDb;

	public UserService(ILogger<UserService> logger, IHttpClientFactory httpClientFactory, IGameDb<User> gameDb, IMemoryDb memoryDb) : base(logger)
	{
		_httpClientFactory = httpClientFactory;
		_gameDb = gameDb;
		_memoryDb = memoryDb;
	}

	public async Task<ErrorCode> LogoutUser(Int64 uid)
	{
		try
		{
			var key = RedisKeyGenerator.MakeUserSessionKey(uid.ToString());
			var result = await _memoryDb.DeleteAsync<string>(key);

			return result;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.UserLogoutException;
		}
	}

	public async Task<(ErrorCode, (Int64, string))> LoginUser(Int64 playerId, string token)
	{
		try
		{
			var errorCode = await VerifyToken(playerId, token);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return (errorCode, (0, string.Empty));
			}

			(errorCode, var user) = await VerifyUser(playerId);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return (errorCode, (0, string.Empty));
			}

			if (user == null)
			{
				return (ErrorCode.UserLoginFail, (0, string.Empty));
			}

			(errorCode, var jwt) = await CreateUserSession(user);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return (errorCode, (0, string.Empty));
			}

			errorCode = await UpdateLastLoginTime(user.Uid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return (errorCode, (0, string.Empty));
			}

			return (errorCode, (user.Uid, jwt));
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.UserLoginException, (0, string.Empty));
		}
	}

	public async Task<(ErrorCode,User?)> GetUser(Int64 uid)
	{
		try
		{
			return await _gameDb.Get(uid);

		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.UserGetException, null);
		}
	}

	public async Task<ErrorCode> UpdateLastAttendanceTime(Int64 uid)
	{
		try
		{
			return await _gameDb.Update(uid, new
			{
				last_attendance_dt = DateTime.Now,
			});
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.UserUpdateLastLoginException;
		}
	}

	public async Task<ErrorCode> UpdateNickname(Int64 uid, string nickname)
	{
		try
		{
			return await _gameDb.Update(uid, new
			{
				nickname,
			});
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.UserUpdateException;
		}
	}

	private async Task<ErrorCode> VerifyToken(Int64 playerId, string token)
	{
		try
		{
			var client = _httpClientFactory.CreateClient("Hive");
			var response = await client.PostAsJsonAsync("/verifytoken", new HiveVerifyRequest
			{
				PlayerId = playerId,
				Token = token
			});

			if (null != response && response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<HiveVerifyResponse>();

				if (result != null && result.Result == ErrorCode.None)
				{
					return ErrorCode.None;
				}
			}

			return ErrorCode.HiveTokenFail;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.HiveTokenException;
		}
	}

	private async Task<(ErrorCode, User?)> VerifyUser(Int64 playerId)
	{
		try
		{
			var (errorCode, user) = await _gameDb.Get(playerId);

			if (errorCode == ErrorCode.DbUserGetFailUserNotFound)
			{
				errorCode = await _gameDb.Set(new User { HivePlayerId = playerId });

				if (errorCode != ErrorCode.None)
				{
					return (errorCode, null);
				}

				(errorCode, user) = await _gameDb.Get(playerId);
			}

			if (errorCode != ErrorCode.None)
			{
				return (errorCode, null);
			}

			return (errorCode, user);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.UserGetException, null);
		}
	}

	private async Task<(ErrorCode, string)> CreateUserSession(User user)
	{
		try
		{
			var jwt = JWTProvider.CreateToken(user);
			var key = RedisKeyGenerator.MakeUserSessionKey(user.Uid.ToString());

			var result = await _memoryDb.SetAsync(key, new RedisUserSession
			{
				Uid = user.Uid,
                Token = jwt,
            }, Expiry.UserSessionExpiry);

			return (result, jwt);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.UserCreateSessionException, "");
		}
	}

	private async Task<ErrorCode> UpdateLastLoginTime(Int64 uid)
	{
		try
		{
			return await _gameDb.Update(uid, new
			{
				last_login_dt = DateTime.Now,
			});
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.UserUpdateLastLoginException;
		}
	}

	public async Task<ErrorCode> Logout(Int64 uid)
	{
		try
		{
			var key = RedisKeyGenerator.MakeUserSessionKey(uid.ToString());
			var result = await _memoryDb.DeleteAsync<string>(key);

			return result;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.UserLogoutException;
		}
	}
}
