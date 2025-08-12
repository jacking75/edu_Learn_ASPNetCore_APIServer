using System.Security.Claims;




namespace WaitingQueue.Server.Services;

public interface ITokenService
{
    string GenerateAccessToken(string userId);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}
