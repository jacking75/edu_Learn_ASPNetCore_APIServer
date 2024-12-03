
using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;
using GameAPIServer.Models.RedisDb;
using Microsoft.Extensions.Options;
using ZLogger;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace GameAPIServer.Services;

public class AuthService : IAuthService
{
	readonly ILogger<AuthService> _logger;
	private readonly IUserRepository _userDb;
	private readonly IMemoryRepository _memoryDb;
	private readonly HttpClient _httpClient;

	public AuthService(ILogger<AuthService> logger, IOptions<ServerConfig> dbConfig, IUserRepository userDb, IMemoryRepository memoryDb, HttpClient httpClient)
	{
		_logger = logger;
		_userDb = userDb;
		_memoryDb = memoryDb;
		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri(dbConfig.Value.HiveServer);
	}

	public async Task<(ErrorCode, RedisUserAuth?)> RegisterToken(Int64 uid)
	{
		var key = RedisKeyGenerator.MakeUidKey(uid.ToString());
		var token = Security.CreateAuthToken();
		RedisUserAuth userAuth = new() { Token = token, Uid = uid };

		if (true == await _memoryDb.StoreDataAsync(key, userAuth, RedisExpiryTimes.AuthTokenExpiryShort))
			return (ErrorCode.None, userAuth);

		return (ErrorCode.RedisTokenStoreFail, null);
	}

	public async Task<ErrorCode> UpdateLastLoginTime(Int64 uid)
	{
		try
		{
			if (false == await _userDb.UpdateRecentLoginTime(uid))
			{
				_logger.ZLogError($"[UpdateLastLoginTime] ErrorCode: {ErrorCode.DbUserRecentLoginUpdateFail}");
				return ErrorCode.DbUserRecentLoginUpdateFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
				$"[UpdateLastLoginTime] ErrorCode: {ErrorCode.DbUserRecentLoginUpdateException}, Uid: {uid}");

			return ErrorCode.DbUserRecentLoginUpdateException;
		}
	}

	public async Task<(ErrorCode, Int64)> VerifyUser(Int64 playerId)
	{
		try
		{
			var user = await _userDb.GetUserByPlayerId(playerId);
			if (null == user)
			{
				return (ErrorCode.DbUserNotFound, 0);
			}

			return (ErrorCode.None, user.Uid);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
				$"[VerifyUser] ErrorCode: {ErrorCode.DbUserFindPlayerIdException}, PlayerId: {playerId}");

			return (ErrorCode.DbUserFindPlayerIdException, 0);
		}
	}

	public async Task<ErrorCode> VerifyTokenToHive(Int64 playerId, string hiveToken)
	{
		try
		{
			var response = await _httpClient.PostAsJsonAsync("/verifytoken", new
			{
				PlayerId = playerId,
				HiveToken = hiveToken
			});

			if (null != response && response.IsSuccessStatusCode)
			{
				return ErrorCode.None;
			}

			return ErrorCode.DbUserHiveTokenNotFound;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
				$"[GDbUser.VerifyTokenToHive] ErrorCode: {ErrorCode.DbUserHiveTokenException}, PlayerId: {playerId}");

			return ErrorCode.DbUserHiveTokenException;
		}
	}

	public async Task<(ErrorCode, RedisUserAuth?)> Login(Int64 playerId, string hiveToken)
	{
		var errorCode = await VerifyTokenToHive(playerId, hiveToken);
		if (ErrorCode.None != errorCode)
		{
			return (errorCode , null);
		}

		(errorCode, Int64 uid) = await VerifyUser(playerId);

		if (errorCode == ErrorCode.DbUserNotFound)
		{
			(errorCode, uid) = await _userDb.InsertUser(playerId);
		}

		if (ErrorCode.None != errorCode)
		{
			return (errorCode, null);
		}

		errorCode = await UpdateLastLoginTime(uid);

		if (ErrorCode.None != errorCode)
		{
			return (errorCode, null);
		}

		var key = RedisKeyGenerator.MakeUidKey(uid.ToString());
		(errorCode, RedisUserAuth? userAuth) = await _memoryDb.GetDataAsync<RedisUserAuth>(key);

		if (null != userAuth)
		{
			return (ErrorCode.None, userAuth);
		}

		(errorCode, var result) = await RegisterToken(uid);
		if (ErrorCode.None != errorCode)
		{
			return (errorCode, null);
		}

		return (ErrorCode.None, result);
	}

	public (ClaimsPrincipal, AuthenticationProperties) RegisterUserClaims(RedisUserAuth userAuth)
	{
		var claims = new List<Claim>
		{
			new("Uid", userAuth.Uid.ToString()),
			new("Token", userAuth.Token),
			new(ClaimTypes.Role, "User")
		};

		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		var authProperties = new AuthenticationProperties
		{
		};

		return (new ClaimsPrincipal(claimsIdentity), authProperties);
	}

	public async Task<ErrorCode> Logout(string uidClaim)
	{
		var key = RedisKeyGenerator.MakeUidKey(uidClaim);
		if (false == await _memoryDb.DeleteDataAsync<RedisUserAuth>(key))
		{
			return ErrorCode.RedisDataDeleteFail;
		}

		return ErrorCode.None;
	}

	public async Task<ErrorCode> UpdateNickname(long uid, string nickname)
	{
		try
		{
			if (false == await _userDb.UpdateUserNickname(uid, nickname))
			{
				_logger.ZLogError($"[UpdateNickname] ErrorCode: {ErrorCode.DbUserNicknameUpdateFail}");
				return ErrorCode.DbUserNicknameUpdateFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
				$"[UpdateNickname] ErrorCode: {ErrorCode.DbUserNicknameUpdateException}, Uid: {uid}");

			return ErrorCode.DbUserNicknameUpdateException;
		}
	}
}
