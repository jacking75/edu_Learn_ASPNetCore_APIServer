
using System.Net.Http.Json;
using System.Threading;
namespace GameClient.Providers;

public class MatchStateProvider
{
	private readonly IHttpClientFactory _httpClientFactory;

	public bool IsMatching { get; set; }
	public event Action<ErrorCode, MatchData?>? OnMatchCompleted;
	public event Action? OnMatchStart;

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
			IsMatching = false;
			return ErrorCode.MatchServerRequestException;
		}
	}

	private async Task MonitorMatchStatusAsync(CancellationToken cancellationToken)
	{
		try
		{
			var (errorCode, gameGuid) = await CheckMatch(cancellationToken);

			if (ErrorCode.GameMatchTimeout == errorCode)
			{
				HandleMonitorTimeout();
				return;
			}
			else
			{
				IsMatching = false;
				NotifyMatchCompleted(errorCode, gameGuid);
			}


		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			IsMatching = false;
			NotifyMatchCompleted(ErrorCode.MatchServerInternalError, null);
		}

	}

	private async Task<(ErrorCode, MatchData?)> CheckMatch(CancellationToken cancellationToken)
	{
		try
		{
			using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));


			while (await timer.WaitForNextTickAsync(cancellationToken))
			{

				var gameClient = _httpClientFactory.CreateClient("Game");
				var response = await gameClient.PostAsync("/match/check", null, cancellationToken);

				if (!response.IsSuccessStatusCode)
				{
					return (ErrorCode.GameMatchCheckFailBadRequest, null);
				}

				var result = await response.Content.ReadFromJsonAsync<CheckMatchResponse>(cancellationToken);

				if (null == result)
				{
					return (ErrorCode.GameMatchCheckFailInvalidData, null);
				}

				if (ErrorCode.None == result.Result)
				{
					return (result.Result, result.MatchData);
				}
			}

			return (ErrorCode.GameMatchTimeout, null);
		}
		catch (OperationCanceledException)
		{
			Console.WriteLine("Match checking task was cancelled.");
			return (ErrorCode.GameMatchCancelled, null);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return (ErrorCode.MatchServerRequestFail, null);
		}
	}

	private void HandleMonitorTimeout()
	{
		_ = MonitorMatchStatusAsync(CancellationToken.None);
	}

	private void NotifyMatchCompleted(ErrorCode error, MatchData? matchData)
	{
		OnMatchCompleted?.Invoke(error, matchData);
	}

	private void NotifyMatchStart()
	{
		OnMatchStart?.Invoke();
	}

}
