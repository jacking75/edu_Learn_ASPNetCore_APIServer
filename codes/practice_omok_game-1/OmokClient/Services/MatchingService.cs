using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace OmokClient.Services;

public class MatchingService : BaseService
{
    public MatchingService(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
            : base(httpClientFactory, sessionStorage) 
    {
    }

    public async Task<MatchResponse?> RequestMatchingAsync(string playerId)
    {
        var matchRequest = new MatchRequest { PlayerID = playerId };
        var gameClient = await CreateClientWithHeadersAsync("GameAPI");

        var response = await gameClient.PostAsJsonAsync("matching/request", matchRequest);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MatchResponse>();
        }
        return null;
    }

    public async Task<MatchResponse?> CheckMatchingAsync(string playerId)
    {
        var checkRequest = new MatchRequest { PlayerID = playerId };
        var gameClient = await CreateClientWithHeadersAsync("GameAPI");

        var response = await gameClient.PostAsJsonAsync("matching/check", checkRequest);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MatchResponse>();
        }
        return null;
    }
}

// Match DTO
public class MatchRequest
{
    public string PlayerID { get; set; }
}

public class MatchResponse
{
    public ErrorCode Result { get; set; }
    public int Success { get; set; }
}
