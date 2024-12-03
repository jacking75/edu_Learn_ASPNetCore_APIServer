using System.Net.Http.Json;
using Blazored.SessionStorage;
using Microsoft.Extensions.Logging;


namespace OmokClient.Services;

public class FriendService : BaseService
{
    private readonly ILogger<FriendService> _logger;

    public FriendService(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
    : base(httpClientFactory, sessionStorage) { }

    public async Task<GetFriendListResponse> GetFriendListAsync(string playerId)
    {
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/friend/get-list", new GetFriendListRequest { PlayerId = playerId });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<GetFriendListResponse>();
        }
        else
        {
            Console.WriteLine($"Failed to get friend list for playerId: {playerId}");
            return new GetFriendListResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<GetFriendRequestListResponse> GetFriendRequestListAsync(string playerId)
    {
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/friend/get-request-list", new GetFriendRequestListRequest { PlayerId = playerId });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<GetFriendRequestListResponse>();
        }
        else
        {
            Console.WriteLine($"Failed to get friend request list for playerId: {playerId}");
            return new GetFriendRequestListResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<RequestFriendResponse> RequestFriendAsync(string playerId, string friendPlayerId)
    {
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/friend/request", new RequestFriendRequest { PlayerId = playerId, FriendPlayerId = friendPlayerId });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RequestFriendResponse>();
        }
        else
        {
            Console.WriteLine($"Failed to send friend request from playerId: {playerId} to friendPlayerId: {friendPlayerId}");
            return new RequestFriendResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<AcceptFriendResponse> AcceptFriendAsync(string playerId, long friendPlayerUid)
    {
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/friend/accept", new AcceptFriendRequest { PlayerId = playerId, FriendPlayerUid = friendPlayerUid });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AcceptFriendResponse>();
        }
        else
        {
            Console.WriteLine($"Failed to accept friend request for playerId: {playerId} and friendPlayerUid: {friendPlayerUid}");
            return new AcceptFriendResponse { Result = ErrorCode.InternalServerError };
        }
    }
}

public class GetFriendListRequest
{
    public string PlayerId { get; set; }
}

public class GetFriendListResponse
{
    public ErrorCode Result { get; set; }
    public List<String> FriendNickNames { get; set; }
    public List<DateTime> CreateDt { get; set; }
}

public class GetFriendRequestListRequest
{
    public string PlayerId { get; set; }
}

public class GetFriendRequestListResponse
{
    public ErrorCode Result { get; set; }
    public List<String> ReqFriendNickNames { get; set; }
    public List<long> ReqFriendUid { get; set; }
    public List<int> State { get; set; }
    public List<DateTime> CreateDt { get; set; }
}

public class RequestFriendRequest
{
    public string PlayerId { get; set; }
    public string FriendPlayerId { get; set; }
}

public class RequestFriendResponse
{
    public ErrorCode Result { get; set; }
}

public class AcceptFriendRequest
{
    public string PlayerId { get; set; }
    public long FriendPlayerUid { get; set; }
}

public class AcceptFriendResponse
{
    public ErrorCode Result { get; set; }
}

public class Friend
{
    public long PlayerUid { get; set; }
    public long FriendPlayerUid { get; set; }
    public string FriendNickName { get; set; }
    public DateTime CreateDt { get; set; }
}

public class FriendRequest
{
    public long SendPlayerUid { get; set; }
    public long ReceivePlayerUid { get; set; }
    public string SendPlayerNickname { get; set; }
    public string ReceivePlayerNickname { get; set; }
    public int RequestState { get; set; }
    public DateTime CreateDt { get; set; }
}