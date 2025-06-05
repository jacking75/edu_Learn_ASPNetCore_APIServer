using System.Dynamic;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using HearthStoneClient;
using HearthStoneWeb.Models.Game;
using HearthStoneClient.Services;
using System.Text.Json;

namespace HearthStoneClient.Services;

public class ServerConfig
{
    public string HiveServer { get; set; } = "";
    public string GameServer { get; set; } = "";
    public string MatchServer { get; set; } = "";
}

public class EmptyDTO { }

public class RequestService
{
    readonly StorageService _storageService;
    readonly IHttpClientFactory _httpClientFactory;

    public RequestService(StorageService storageService, IHttpClientFactory httpClientFactory)
    {
        _storageService = storageService;  // StorageService 초기화
        _httpClientFactory = httpClientFactory; // IHttpClientBuilder 초기화 
    }

    public async Task<string> SendRequest<T>
           (
           string serverName,
           string endpoint,
           HttpMethod method,
           T payload
           )
    {
        try
        {
            var client = _httpClientFactory.CreateClient(serverName);

            var request = new HttpRequestMessage(method, endpoint);

            // Fix: Wrap the second argument in an object array to match the expected parameter type
            SessionInfo sessionInfo = _storageService.GetSessionInfo();
            if (sessionInfo != null && !sessionInfo.IsEmpty())
            {
                request.Headers.Add("token", sessionInfo.Token);
                request.Headers.Add("accountuid", sessionInfo.accountUid);
            }

            if (method == HttpMethod.Post || method == HttpMethod.Post && payload != null)
            {
                request.Content = JsonContent.Create(payload);
            }

            var response = await client.SendAsync(request);
            if (response == null)
                return "";

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"SendRequest 오류: {e.Message}, 상세 정보: {e.ToString()}");
            return "";
        }
    }
}

public static class ReceiveResponce 
{
    public static T ConvertToResponse<T>(this string json) where T : new()
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return new T();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(json, options) ?? new T();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON 변환 오류: {ex.Message}");
            return new T();
        }
    }
}