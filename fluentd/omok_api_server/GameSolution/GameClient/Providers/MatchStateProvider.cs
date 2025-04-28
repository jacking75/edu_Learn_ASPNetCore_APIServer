using GameShared.DTO;
using System.Net.Http.Json;

namespace GameClient.Providers;

public class MatchStateProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    public bool IsMatching { get; set; }

    public event Action<ErrorCode> OnMatchCompleted;
    public event Action OnMatchStart;

    public MatchStateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorCode> RequestMatchAsync(CancellationToken cancellationToken)
    {
        if (IsMatching)
        {
            return ErrorCode.GameMatchDuplicate;
        }

        try
        {
            IsMatching = false;

            var gameClient = _httpClientFactory.CreateClient("Game");
            var response = await gameClient.PostAsync("/match/start", null);

            if (!response.IsSuccessStatusCode)
            {
                return ErrorCode.GameMatchBadRequest;
            }

            var result = await response.Content.ReadFromJsonAsync<ErrorCodeDTO>();

            if (null == result)
            {
                return ErrorCode.GameMatchInvalidResponse;
            }

            if (ErrorCode.None == result.Result)
            {
                IsMatching = true;
                _ = MonitorMatchStatusAsync(cancellationToken);
                NotifyMatchStart();
            }

            return result.Result;

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            IsMatching = false;
            return ErrorCode.MatchServerRequestException;
        }
    }

    private async Task MonitorMatchStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await CheckMatch(cancellationToken);

            if (ErrorCode.GameMatchTimeout == result)
            {
                HandleMonitorTimeout();
                return;
            }
            else
            {
                IsMatching = false;
                NotifyMatchCompleted(result);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            IsMatching = false;
            NotifyMatchCompleted(ErrorCode.MatchServerInternalError);
        }

    }

    private async Task<ErrorCode> CheckMatch(CancellationToken cancellationToken)
    {
        try
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(2));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {

                var gameClient = _httpClientFactory.CreateClient("Game");
                var response = await gameClient.PostAsync("/match/check", null, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return ErrorCode.GameMatchCheckFailBadRequest;
                }

                var result = await response.Content.ReadFromJsonAsync<ErrorCodeDTO>(cancellationToken);

                if (null == result)
                {
                    return ErrorCode.GameMatchCheckFailInvalidData;
                }

                if (ErrorCode.None == result.Result)
				{
					return ErrorCode.None;
				}
			}

            return ErrorCode.GameMatchTimeout;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Match checking task was cancelled.");
            return ErrorCode.GameMatchCancelled;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return ErrorCode.MatchServerRequestFail;
        }
    }

    private void HandleMonitorTimeout()
    {
        _ = MonitorMatchStatusAsync(CancellationToken.None);
    }

    private void NotifyMatchCompleted(ErrorCode error)
    {
        OnMatchCompleted.Invoke(error);
    }

    private void NotifyMatchStart()
    {
        OnMatchStart?.Invoke();
    }
}
