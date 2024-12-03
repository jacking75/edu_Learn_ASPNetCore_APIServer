using System.Net;
using GameAPIServer.Models.GameDb;
using GameAPIServer.Models.RedisDb;
using GameAPIServer.Repositories.Interfaces;

namespace GameAPIServer.Middleware;
public class CheckUserAuthAndLoadUserData
{
	private readonly IMemoryRepository _memoryDb;
	private readonly RequestDelegate _next;

	public CheckUserAuthAndLoadUserData(RequestDelegate next, IMemoryRepository memoryDb)
	{
		_memoryDb = memoryDb;
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			var path = context.Request.Path.Value;
			var uidClaim = GetUidClaim(context);

			if (IsIgnorePath(path))
			{
				await _next(context);
				return;
			}

			if (false == IsAuthenticated(context))
			{
				await SendError(context, ErrorCode.ClaimInvalid);
				return;
			}

			if (true == string.IsNullOrEmpty(uidClaim))
			{
				await SendError(context, ErrorCode.ClaimUidNotFound);
				return;
			}

			var result = await IsTokenValid(uidClaim, context);
			if (ErrorCode.None != result)
			{
				await SendError(context, result);
				return;
			}

			var userLockKey = RedisKeyGenerator.MakeUserLockKey(uidClaim);
			RedisUserLock userLock = new();

			if (false == await IsUserLockSecure(userLockKey, userLock))
			{
				await SendError(context, ErrorCode.RedisUserLockOccupied);
				return;
			}

			if (Int64.TryParse(uidClaim, out var uid))
			{
				context.Items["Uid"] = uid; 
			}
			else
			{
				await SendError(context, ErrorCode.ClaimUidInvalid);
				return;
			}

			await _next(context);
			await _memoryDb.UnlockDataAsync(userLockKey, userLock);
		}
		catch
		{
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;
			await SendError(context, ErrorCode.UnhandledException);
		}	
	}

	private async Task<bool> IsUserLockSecure(string userLockKey, RedisUserLock userLock)
	{
		return await SetLock(userLockKey, userLock);

	}

	private async Task<ErrorCode> IsTokenValid(string uid, HttpContext context)
	{
		var tokenClaim = GetTokenClaim(context);

		var userAuthData = await GetUserAuth(uid);
		if (null == userAuthData)
		{
			return ErrorCode.ClaimAuthTokenUserNotFound;
		}

		if (userAuthData.Token != tokenClaim)
		{
			return ErrorCode.ClaimAuthTokenInvalid;
		}

		return ErrorCode.None;
	}

	private static string? GetTokenClaim(HttpContext context)
	{
		var tokenClaim = context.User.FindFirst("Token")?.Value;
		return tokenClaim;
	}
	private static string? GetUidClaim(HttpContext context)
	{
		var uidClaim = context.User.FindFirst("Uid")?.Value;
		return uidClaim;
	}

	private static bool IsAuthenticated(HttpContext context)
	{
		if (context.User.Identity?.IsAuthenticated == true)
		{
			return true;
		}

		return false;
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

	private async Task<RedisUserAuth?> GetUserAuth(string uidClaim)
	{
		var key = RedisKeyGenerator.MakeUidKey(uidClaim);
		var (errorCode, userInfo) = await _memoryDb.GetDataAsync<RedisUserAuth>(key);
		return errorCode == ErrorCode.None ? userInfo : null;
	}

	private async Task<bool> SetLock(string key, RedisUserLock userLock)
	{
		if (await _memoryDb.LockDataAsync(key, userLock, RedisExpiryTimes.RequestLockExpiry))
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
