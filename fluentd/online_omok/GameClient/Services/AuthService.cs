using Blazored.LocalStorage;
using GameShared.DTO;
using System.Net.Http.Json;

namespace GameClient.Services;

public class AuthService
{
	private readonly IHttpClientFactory _httpClientFactory;
    public AuthService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
    }

	public async Task<(ErrorCode, LoginResponse)> LoginAsync(string email, string password)
	{
		var response = new LoginResponse();
		try
		{
            var hiveClient = _httpClientFactory.CreateClient("Hive");
			var hiveResult = await hiveClient.PostAsJsonAsync(
				"/LoginHive", new HiveLoginRequest
				{
					Email = email,
					Password = password
				});

			if (hiveResult.IsSuccessStatusCode == false)
			{
				return (ErrorCode.LoginFailInvalidResponse, response);
			}

			var hiveResponse = await hiveResult.Content.ReadFromJsonAsync<HiveLoginResponse>();

			if (hiveResponse == null)
			{
				return (ErrorCode.LoginFailInvalidResponse, response);
			}

			if (hiveResponse.Result != ErrorCode.None)
			{
				return (hiveResponse.Result, response);
			}

			var gameClient = _httpClientFactory.CreateClient("Game");

			var gameResult = await gameClient.PostAsJsonAsync(
				"/Login", new LoginRequest
				{
					PlayerId = hiveResponse.PlayerId,
					Token = hiveResponse.Token
				});

			if (false == gameResult.IsSuccessStatusCode)
			{
				return (ErrorCode.LoginFailInvalidResponse, response);
			}

            var result = await gameResult.Content.ReadFromJsonAsync<LoginResponse>();

			if (result == null)
			{
				return (ErrorCode.LoginFailInvalidResponse, response);
			}

            return (result.Result, result);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return (ErrorCode.LoginFailException, response);
		}
	}

	public async Task<ErrorCode> RegisterAsync(string email, string password)
	{
		try
		{
			var hiveClient = _httpClientFactory.CreateClient("Hive");
			var hiveResult = await hiveClient.PostAsJsonAsync(
				"/CreateHiveAccount", new HiveRegisterRequest
				{
					Email = email,
					Password = password
				});

			if (true == hiveResult.IsSuccessStatusCode)
			{
				return ErrorCode.None;
			}

			return ErrorCode.RegisterFail;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.RegisterFailException;
		}
	}

	public async Task LogoutAsync()
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			await gameClient.PostAsync("/logout", null);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}