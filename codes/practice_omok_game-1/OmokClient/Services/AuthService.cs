using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace OmokClient.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISessionStorageService _sessionStorage;

    public AuthService(HttpClient httpClient, IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _sessionStorage = sessionStorage;
    }

    public async Task<AccountResponse?> RegisterUserAsync(string email, string password)
    {
        var registerData = new AccountRequest
        {
            HiveUserId = email,
            HiveUserPw = password
        };

        var response = await _httpClient.PostAsJsonAsync("register", registerData);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AccountResponse>();
        }
        return null;
    }

    public async Task<LoginResponse?> LoginUserAsync(string email, string password)
    {
        var loginData = new LoginRequest
        {
            HiveUserId = email,
            HiveUserPw = password
        };

        var response = await _httpClient.PostAsJsonAsync("login", loginData);
        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse != null && loginResponse.Result == ErrorCode.None)
            {
                await _sessionStorage.SetItemAsync("UserId", loginResponse.HiveUserId);
                await _sessionStorage.SetItemAsync("Token", loginResponse.HiveToken);
            }
            return loginResponse;
        }
        return null;
    }

    public async Task<GameLoginResponse?> GameLoginAsync(LoginResponse loginResponse)
    {
        var gameLoginData = new GameLoginRequest
        {
            PlayerId = loginResponse.HiveUserId,
            Token = loginResponse.HiveToken,
            AppVersion = "0.1.0",
            DataVersion = "0.1.0"
        };

        var gameClient = _httpClientFactory.CreateClient("GameAPI");
        gameClient.DefaultRequestHeaders.Add("AppVersion", "0.1.0");
        gameClient.DefaultRequestHeaders.Add("DataVersion", "0.1.0");
        var response = await gameClient.PostAsJsonAsync("login", gameLoginData);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<GameLoginResponse>();
        }
        return null;
    }
}

// Login Register Auth DTO 
public class AccountRequest
{
    public required string HiveUserId { get; set; }
    public required string HiveUserPw { get; set; }
}

public class AccountResponse
{
    public required ErrorCode Result { get; set; }
}

public class LoginRequest
{
    public required string HiveUserId { get; set; }
    public required string HiveUserPw { get; set; }
}

public class LoginResponse
{
    public required ErrorCode Result { get; set; }
    public required string HiveUserId { get; set; }
    public string HiveToken { get; set; } = string.Empty;
}

public class GameLoginRequest
{
    public required string PlayerId { get; set; }
    public required string Token { get; set; }
    public string AppVersion { get; set; } = "0.1.0";
    public string DataVersion { get; set; } = "0.1.0";
}

public class GameLoginResponse
{
    public required ErrorCode Result { get; set; }
}
