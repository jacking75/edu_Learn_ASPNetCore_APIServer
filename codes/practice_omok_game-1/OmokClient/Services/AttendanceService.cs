using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace OmokClient.Services;

public class AttendanceService : BaseService
{
    public AttendanceService(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorage)
        : base(httpClientFactory, sessionStorage) { }

    public async Task<AttendanceInfoResponse> GetAttendanceInfoAsync(string playerId)
    {
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/attendance/get-info", new AttendanceInfoRequest { PlayerId = playerId });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AttendanceInfoResponse>();
        }
        else
        {
            return new AttendanceInfoResponse { Result = ErrorCode.InternalServerError };
        }
    }

    public async Task<AttendanceCheckResponse> CheckAttendanceAsync(string playerId)
    {
        var client = await CreateClientWithHeadersAsync("GameAPI");
        var response = await client.PostAsJsonAsync("/attendance/check", new AttendanceCheckRequest { PlayerId = playerId });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AttendanceCheckResponse>();
        }
        else
        {
            return new AttendanceCheckResponse { Result = ErrorCode.InternalServerError };
        }
    }
}


public class AttendanceInfoRequest
{
    public string PlayerId { get; set; }
}

public class AttendanceInfoResponse
{
    public ErrorCode Result { get; set; }
    public int AttendanceCnt { get; set; }
    public DateTime? RecentAttendanceDate { get; set; }
}

public class AttendanceCheckRequest
{
    public string PlayerId { get; set; }
}

public class AttendanceCheckResponse
{
    public ErrorCode Result { get; set; }
}

public class AttendanceInfo
{
    public int AttendanceCnt { get; set; }
    public DateTime? RecentAttendanceDate { get; set; }
}