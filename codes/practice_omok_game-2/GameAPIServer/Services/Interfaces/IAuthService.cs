using GameAPIServer.Models.RedisDb;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GameAPIServer.Services.Interfaces;

public interface IAuthService
{
	public Task<ErrorCode> VerifyTokenToHive(Int64 playerId, string hiveToken);
	public Task<(ErrorCode, Int64)> VerifyUser(Int64 playerId);
	public Task<ErrorCode> UpdateLastLoginTime(Int64 uid);
	public Task<ErrorCode> UpdateNickname(Int64 uid, string nickname);
	public Task<(ErrorCode, RedisUserAuth?)> RegisterToken(Int64 uid);
	public Task<(ErrorCode, RedisUserAuth?)> Login(Int64 playerId, string hiveToken);
	public Task<ErrorCode> Logout(string uidClaim);
	public (ClaimsPrincipal, AuthenticationProperties) RegisterUserClaims(RedisUserAuth userAuth);
}
