using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace OmokClient.Services;

public class MailService : BaseService
{
    public MailService(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
        : base(httpClientFactory, sessionStorage) { }

    public async Task<MailBoxResponse> GetMailboxAsync(string playerId, int pageNum)
    {
        var request = new GetPlayerMailBoxRequest { PlayerId = playerId, PageNum = pageNum };
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/mail/get-mailbox", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MailBoxResponse>();
        }
        else
        {
            return new MailBoxResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<MailDetailResponse> ReadMailAsync(string playerId, long mailId)
    {
        var request = new ReadMailRequest { PlayerId = playerId, MailId = mailId };
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/mail/read", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MailDetailResponse>();
        }
        else
        {
            return new MailDetailResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<ReceiveMailItemResponse> ReceiveMailItemAsync(string playerId, long mailId)
    {
        var request = new ReceiveMailItemRequest { PlayerId = playerId, MailId = mailId };
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/mail/receive-item", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ReceiveMailItemResponse>();
        }
        else
        {
            return new ReceiveMailItemResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<DeleteMailResponse> DeleteMailAsync(string playerId, long mailId)
    {
        var request = new DeleteMailRequest { PlayerId = playerId, MailId = mailId };
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/mail/delete", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<DeleteMailResponse>();
        }
        else
        {
            return new DeleteMailResponse { Result = ErrorCode.InternalServerError };
        }
    }
}


public class GetPlayerMailBoxRequest
{
    public string PlayerId { get; set; }
    public int PageNum { get; set; }
}

public class MailBoxResponse
{
    public ErrorCode Result { get; set; }
    public List<Int64> MailIds { get; set; }
    public List<string> Titles { get; set; }
    public List<int> ItemCodes { get; set; }
    public List<DateTime> SendDates { get; set; }
    public List<int> ReceiveYns { get; set; }
}

public class ReadMailRequest
{
    public string PlayerId { get; set; }
    public Int64 MailId { get; set; }
}

public class MailDetailResponse
{
    public ErrorCode Result { get; set; }
    public Int64 MailId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int ItemCode { get; set; }
    public int ItemCnt { get; set; }
    public DateTime? SendDate { get; set; }
    public DateTime? ExpireDate { get; set; }
    public DateTime? ReceiveDate { get; set; }
    public int ReceiveYn { get; set; }
}
public class ReceiveMailItemRequest
{
    public string PlayerId { get; set; }
    public Int64 MailId { get; set; }
}

public class ReceiveMailItemResponse
{
    public ErrorCode Result { get; set; }
    public int? IsAlreadyReceived { get; set; }
}

public class DeleteMailResponse
{
    public ErrorCode Result { get; set; }
}

public class DeleteMailRequest
{
    public string PlayerId { get; set; }
    public Int64 MailId { get; set; }
}

public class MailDetail
{
    public Int64 MailId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int ItemCode { get; set; }
    public int ItemCnt { get; set; }
    public DateTime SendDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime? ReceiveDate { get; set; }
    public int ReceiveYn { get; set; }
}