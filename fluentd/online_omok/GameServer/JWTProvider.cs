using GameServer.Models.GameDb;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace GameServer;

public class JWTProvider
{
	private static string secret = "MySuperSecretKey12345678901234567890";
	public static string CreateToken(User user)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var tokenHandler = new JsonWebTokenHandler();
		return tokenHandler.CreateToken(JsonSerializer.Serialize(user), credentials);
	}

	public static async Task<bool> ValidateToken(string token)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

		var tokenHandler = new JsonWebTokenHandler();

		var validationParameters = new TokenValidationParameters
		{
			RequireExpirationTime = false,
			RequireAudience = false,
			ValidateAudience = false,
			ValidateIssuer = false,
			ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
			IssuerSigningKey = key,
		};

		try
		{
			var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);
			return result.IsValid;
		}
		catch (Exception e)
		{
			return false;
		}
	}
}

