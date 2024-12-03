using System.Net.Http;
using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace OmokClient.Services;

public class BaseService // TODO 로그인 토큰 보내는 부분 추가하기? (모둔 Service의 공통 작업)
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISessionStorageService _sessionStorage;

    public BaseService(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
    {
        _httpClientFactory = httpClientFactory;
        _sessionStorage = sessionStorage;
    }

    protected async Task<HttpClient> CreateClientWithHeadersAsync(string clientName)
    {
        var client = _httpClientFactory.CreateClient(clientName);

        var playerId = await _sessionStorage.GetItemAsync<string>("UserId");
        var token = await _sessionStorage.GetItemAsync<string>("Token");
        var appVersion = "0.1.0";
        var dataVersion = "0.1.0";

        if (!string.IsNullOrEmpty(playerId) && !string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Add("PlayerId", playerId);
            client.DefaultRequestHeaders.Add("Token", token);
        }
        client.DefaultRequestHeaders.Add("AppVersion", appVersion);
        client.DefaultRequestHeaders.Add("DataVersion", dataVersion);

        // 디버깅 - 헤더가 제대로 추가되었는지 확인
        Console.WriteLine("HttpClient Headers:");
        foreach (var header in client.DefaultRequestHeaders)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        return client;
    }
}
