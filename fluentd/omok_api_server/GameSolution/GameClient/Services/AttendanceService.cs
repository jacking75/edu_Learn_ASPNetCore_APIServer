using GameShared.DTO;
using System.Net.Http.Json;

namespace GameClient.Services;

public class AttendanceService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AttendanceService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorCode> AttendAsync()
    {
        try
        {
            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsync("/attendance", null);

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.AttendanceUpdateBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<AttendanceResponse>();
            return result?.Result ?? ErrorCode.AttendanceUpdateFail;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ErrorCode.AttendanceUpdateException;
        }
    }
}
