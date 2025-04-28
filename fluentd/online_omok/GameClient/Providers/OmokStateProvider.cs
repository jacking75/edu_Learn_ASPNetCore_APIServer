using GameShared.DTO;
using System.Net.Http.Json;

namespace GameClient.Providers;

public class OmokStateProvider
{
	private readonly IHttpClientFactory _httpClientFactory;

	public byte[] OmokData { get; set; }
	public int TurnCount { get; set; } = 0;

	public event Action? OnStateChange;

	public OmokStateProvider(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<ErrorCode> StartOmokAsync(CancellationToken cancellationToken)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/omok/start", new { });

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.StartOmokFail;
			}

			var result = await response.Content.ReadFromJsonAsync<ErrorCodeDTO>();

			if (null == result)
			{
				return ErrorCode.StartOmokFailInvalidData;
			}

			if (ErrorCode.None != result.Result)
			{
				return result.Result;
			}

			_ = MonitorGameAsync(cancellationToken);
			return result.Result;

		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.StartOmokException;
		}
	}

	private async Task<ErrorCode> PeekTurnAsync(CancellationToken cancellationToken)
	{
		try
		{
			using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));
			var gameClient = _httpClientFactory.CreateClient("Game");

			while (await timer.WaitForNextTickAsync(cancellationToken))
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return ErrorCode.GamePeekCancelled;
				}

				var response = await gameClient.PostAsJsonAsync("/omok/peek", 
					new OmokPeekRequest { TurnCount = TurnCount });

				if (!response.IsSuccessStatusCode)
				{
					return (ErrorCode.GamePeekFailInvalidData);
				}

				var result = await response.Content.ReadFromJsonAsync<OmokPeekResponse>();

				if (null == result)
				{
					return (ErrorCode.GamePeekFailInvalidData);
				}

				if (TurnCount != result.TurnCount &&
				   null != result.OmokData)
				{
					OmokData = result.OmokData;
					TurnCount = result.TurnCount;
					NotifyStateChange();
				}

				return result.Result;
			}

			return ErrorCode.GamePeekTimeout;
		}
		catch (OperationCanceledException)
		{
			return ErrorCode.GamePeekCancelled;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.GamePeekException;
		}
	}

	public bool IsMyTurn(Int64 uid)
	{
		Console.WriteLine($"uid: {uid}");
		return OmokGame.IsMyTurn(OmokData, uid);
	}

	private void HandleMonitorTimeout(CancellationToken cancellationToken)
	{
		_ = MonitorGameAsync(cancellationToken);
	}

	private async Task MonitorGameAsync(CancellationToken cancellationToken)
	{
		try
		{
			var errorCode = await PeekTurnAsync(cancellationToken);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}

		if (cancellationToken.IsCancellationRequested == false)
		{
			await Task.Delay(1000, cancellationToken);
			HandleMonitorTimeout(cancellationToken);
		}
	}

	public async Task<ErrorCode> PutOmokAsync(int posX, int posY)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/omok/put", new OmokPutRequest { PosX = posX, PosY = posY });

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.PutOmokFail;
			}

			var result = await response.Content.ReadFromJsonAsync<OmokPutResponse>();

			if (null == result)
			{
				return ErrorCode.PutOmokFailInvalidResponse;
			}

			if (ErrorCode.None != result.Result)
			{
				return result.Result;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.PutOmokException;
		}
	}   

	private void NotifyStateChange()
	{
		OnStateChange?.Invoke();
	}
}
