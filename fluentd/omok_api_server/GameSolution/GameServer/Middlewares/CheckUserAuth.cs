using GameServer.Models;
using GameShared.DTO;
using ServerShared;
using ServerShared.Redis;
using ServerShared.Repository.Interfaces;


namespace GameServer.Middlewares;

public class CheckUserAuthAndLoadUserData
{
	private readonly IMemoryDb _memoryDb;
	private readonly RequestDelegate _next;

	public CheckUserAuthAndLoadUserData(RequestDelegate next, IMemoryDb memoryDb)
	{
		_memoryDb = memoryDb;
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			var path = context.Request.Path.Value;

			if (IsIgnorePath(path))
			{
				await _next(context);
				return;
			}

			var (result, token) = GetTokenFromRequest(context);

			if (false == result)
			{
				await SendError(context, ErrorCode.BearerTokenNotFound);
				return;
			}

			(result, var uid) = GetUidFromRequest(context);

			if (false == result)
			{
				await SendError(context, ErrorCode.UidNotFound);
				return;
			}

			var errorCode = await IsTokenValid(token, uid);
			if (ErrorCode.None != errorCode)
			{
				await SendError(context, errorCode);
				return;
			}

			var userLockKey = RedisKeyGenerator.MakeUserLockKey(uid);
			RedisUserLock userLock = new();

			if (false == await IsUserLockSecure(userLockKey, userLock))
			{
				await SendError(context, ErrorCode.UserLockOccupied);
				return;
			}

			context.Items["uid"] = uid;

			await _next(context);
			await _memoryDb.UnlockAsync(userLockKey, userLock);
		}
		catch
		{
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await SendError(context, ErrorCode.UnhandledException);
		}
	}

	private (bool, string) GetUidFromRequest(HttpContext context)
	{
		if (false == context.Request.Headers.TryGetValue("Uid", out var uid))
		{
			return (false, string.Empty);
		}

		return (true, uid);
	}

	private async Task<ErrorCode> IsTokenValid(string token, string uid)
	{
		try
		{
			var result = await JWTProvider.ValidateToken(token);
		
			if (false == result)
			{
				return ErrorCode.BearerTokenInvalid;
			}

			var userAuthData = await GetUserAuth(uid);

			if (null == userAuthData)
			{
				return ErrorCode.UserSessionExpired;
			}

			if (userAuthData.Token != token)
			{
				return ErrorCode.BearerTokenMismatch;
			}

			return ErrorCode.None;
		}
		catch
		{
			return ErrorCode.BearerTokenException;
		}
	}

	private async Task<bool> IsUserLockSecure(string userLockKey, RedisUserLock userLock)
	{
		return await SetLock(userLockKey, userLock);
	}

	private (bool, string) GetTokenFromRequest(HttpContext context)
	{
		var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
		if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
		{
			return (true, authorizationHeader.Substring("Bearer ".Length).Trim()); 
		}

		return (false , string.Empty);
	}

	private static bool IsIgnorePath(string? path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}

		if (string.Compare(path, "/Login", StringComparison.OrdinalIgnoreCase) == 0)
		{
			return true;
		}

		return false;
	}

	private async Task<RedisUserSession?> GetUserAuth(string uid)
	{
		var key = RedisKeyGenerator.MakeUserSessionKey(uid);
		var (errorCode, userInfo) = await _memoryDb.GetAsync<RedisUserSession>(key);
		return errorCode == ErrorCode.None ? userInfo : null;
	}

	private async Task<bool> SetLock(string key, RedisUserLock userLock)
	{
		var result = await _memoryDb.LockAsync(key, userLock, Expiry.UserRequestLockExpiry);

		if (result == ErrorCode.None)
		{
			return true;
		}

		return false;
	}

	private static async Task<bool> SendError(HttpContext context, ErrorCode errorCode,
		int statusCode = StatusCodes.Status401Unauthorized)
	{
		context.Response.StatusCode = statusCode;
		await context.Response.WriteAsJsonAsync(new ErrorCodeDTO
		{
			Result = errorCode
		});
		return true;
	}
}