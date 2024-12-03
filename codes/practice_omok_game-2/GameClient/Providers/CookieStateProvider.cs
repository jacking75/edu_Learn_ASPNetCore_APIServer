using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GameClient.Providers;
public class CookieStateProvider : AuthenticationStateProvider
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly AttendanceProvider _attendanceProvider; 
	private ClaimsPrincipal _anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
	private bool _authenticated = false;

	public UserInfo? AuthenticatedUser { get; private set; }

	public CookieStateProvider(IHttpClientFactory httpClientFactory, AttendanceProvider attendanceProvider)
	{
		_httpClientFactory = httpClientFactory;
		_attendanceProvider = attendanceProvider;
	}

	public async Task<bool> CheckAuthenticatedAsync()
	{
		try
		{
			await GetAuthenticationStateAsync();
			return _authenticated;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return false;
		}
	}

	public void NotifyAuthenticationStateChanged()
	{
		NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		_authenticated = false;
		var user = _anonymousUser;
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
		
			var response = await gameClient.GetAsync("/userdata");

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<UserDataLoadResponse>();

				if (null == result)
				{
					return new AuthenticationState(user);
				}

				var userInfo = result.UserData?.User;
				var attendanceInfo = result.UserData?.UserAttendances;

				if (null == userInfo)
				{
					return new AuthenticationState(user);
				}

				if (null != attendanceInfo)
				{
					_attendanceProvider.SetAttendanceInfos(attendanceInfo);
				}

				var claims = new List<Claim>
				{
					new(ClaimTypes.Name, userInfo.Nickname),
					new(ClaimTypes.NameIdentifier, userInfo.Uid.ToString()),
				};

				var identity = new ClaimsIdentity(claims, "ServerCookie");
				user = new ClaimsPrincipal(identity);

				AuthenticatedUser = userInfo;
				_authenticated = true;
				return new AuthenticationState(user);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			_authenticated = false;
			return new AuthenticationState(user);
		}

		return new AuthenticationState(user);
	}

	public async Task<ErrorCode> LoginAsync(string email, string password)
	{
		try
		{
			var hiveClient = _httpClientFactory.CreateClient("Hive");
			var hiveResult = await hiveClient.PostAsJsonAsync(
				"/LoginHive", new HiveLoginRequest
				{
					Email = email,
					Password = password
				});

			var hiveResponse = await hiveResult.Content.ReadFromJsonAsync<HiveLoginResponse>();

			if (null == hiveResponse)
			{
				return ErrorCode.HiveLoginFail;
			}

			if (ErrorCode.None != hiveResponse.Result)
			{
				return hiveResponse.Result;
			}

			var gameClient = _httpClientFactory.CreateClient("Game");

			var gameResult = await gameClient.PostAsJsonAsync(
				"/Login", new LoginRequest
				{
					PlayerId = hiveResponse.PlayerId,
					HiveToken = hiveResponse.HiveToken
				});

			if (false == gameResult.IsSuccessStatusCode)
			{
				return ErrorCode.LoginFailBadRequest;
			}

			var result = await gameResult.Content.ReadFromJsonAsync<LoginResponse>();

			if (null == result)
			{
				return ErrorCode.LoginFailInvalidResponse;
			}

			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

			return result.Result;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.LoginFailException;
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

			return ErrorCode.LoginFail;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.LoginFailException;
		}
	}

	public async Task<ErrorCode> UpdateNicknameAsync(string nickname)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("userdata/update/nickname", new UpdateNicknameRequest
			{
				Nickname = nickname
			});

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.UpdateUserFailBadRequest;
			}

			var result = await response.Content.ReadFromJsonAsync<UpdateNicknameResponse>();
			return result.Result;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.UpdateUserException;
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