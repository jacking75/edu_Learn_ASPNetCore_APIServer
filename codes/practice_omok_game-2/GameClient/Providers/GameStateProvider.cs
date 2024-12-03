using System.Net.Http.Json;
namespace GameClient.Providers;

public class GameStateProvider
{
	private readonly IHttpClientFactory _httpClientFactory;

	public byte[]? Game { get; set; }
	public LoadedProfileData? Opponent { get; set; }
	public bool GameStart = false;
	public OmokStone CurrentTurn = OmokStone.Empty;
	public OmokStone Winner = OmokStone.Empty;

	public event Action? OnGameStarted;
	public event Action<OmokStone>? OnGameCompleted;
	public event Action? OnTurnChange;

	public GameStateProvider(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}
	public async Task<ErrorCode> EnterGameAsync(CancellationToken cancellationToken)
	{
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/omok/enter", new { });

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.GameGetFail;
			}

			var result = await response.Content.ReadFromJsonAsync<EnterGameResponse>();

			if (null == result)
			{
				return ErrorCode.GameGetFailGameNotFound;
			}

			if (ErrorCode.None != result.Result)
			{
				return result.Result;
			}

			if (null == result.GameData)
			{
				return ErrorCode.GameGetFailInvalidGameData;
			}

			Game = result.GameData;

			CheckGameUpdate();
			_ = MonitorGameAsync(cancellationToken);
			return result.Result;

		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.GameGetException;
		}
	}

	public async Task<ErrorCode> PlayGameAsync(int x, int y)
	{
		if (null == Game)
		{
			return ErrorCode.GamePlayFailGameLoadFail;
		}

		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/omok/stone", new PlayOmokRequest
			{
				PosX = x,
				PosY = y
			});

			if (!response.IsSuccessStatusCode)
			{
				return ErrorCode.GamePlayFail;
			}

			var result = await response.Content.ReadFromJsonAsync<PlayOmokResponse>();

			if (null == result)
			{
				return ErrorCode.GamePlayFailGameNotFound;
			}

			if (ErrorCode.None != result.Result)
			{
				return result.Result;
			}

			if (null == result.GameData)
			{
				return ErrorCode.GamePlayFailInvalidData;
			}

			Game = result.GameData;
			CheckGameUpdate();

			return result.Result;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return ErrorCode.GamePlayException;
		}
	}

	private void HandleMonitorTimeout(CancellationToken cancellationToken)
	{
		_ = MonitorGameAsync(cancellationToken);
	}

	private async Task MonitorGameAsync(CancellationToken cancellationToken)
	{
		try
		{
			var errorCode = await PeekGameAsync(cancellationToken);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}

		if (null != Game && cancellationToken.IsCancellationRequested == false)
		{
			await Task.Delay(1000, cancellationToken);
			HandleMonitorTimeout(cancellationToken);
		}
	}

	private async Task<ErrorCode> PeekGameAsync(CancellationToken cancellationToken)
	{
		try
		{
			using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

			while (await timer.WaitForNextTickAsync(cancellationToken))
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return ErrorCode.GamePeekCancelled;
				}

				var gameClient = _httpClientFactory.CreateClient("Game");
				var response = await gameClient.PostAsync("/omok/peek", null);

				if (!response.IsSuccessStatusCode)
				{
					return (ErrorCode.GamePeekFailInvalidData);
				}

				var result = await response.Content.ReadFromJsonAsync<PeekGameResponse>();

				if (null == result || null == result.GameData)
				{
					return (ErrorCode.GamePeekFailInvalidData);
				}

				Game = result.GameData;
				CheckGameUpdate();

				return result.Result;
			}

			return ErrorCode.GamePeekFail;
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

	public async Task<(ErrorCode, UserInfo?)> LoadUserInfoAsync(Int64 uid)
	{
		if (null == Game)
		{
			return (ErrorCode.GameLoadOpponentFailGameNotFound, null);
		}

		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsJsonAsync("/userdata/profile", new UserProfileLoadRequest {
			
				Uid = uid
			});

			if (!response.IsSuccessStatusCode)
			{
				return ((ErrorCode.GamePeekFailInvalidData, null));
			}

			var result = await response.Content.ReadFromJsonAsync<UserProfileLoadResponse>();

			if (null == result)
			{
				return (ErrorCode.GamePeekFailInvalidData, null);
			}

			return (result.Result, result.ProfileData?.User);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			return (ErrorCode.GameLoadOpponentProfileException, null);
		}
	}

	private void CheckGameUpdate()
	{
		if (null == Game)
			return;

		if (OmokGame.IsGameEnded(Game))
		{
			Winner = OmokGame.GetGameWinner(Game);

			NotifyGameCompleted(Winner);
			return;
		}

		if (false == GameStart)
		{
			if (true == OmokGame.IsGameStarted(Game))
			{
				GameStart = true;
				NotifyGameStarted();
			}
		}
		else if (CurrentTurn != OmokGame.GetCurrentTurn(Game))
		{
			CurrentTurn = OmokGame.GetCurrentTurn(Game);
			NotifyTurnChange();
		}
	}

	private void NotifyGameStarted()
	{
		OnGameStarted?.Invoke();
	}

	public OmokStone GetOmokStone(Int64 uid)
	{
		if (null == Game)
		{
			return OmokStone.Empty;
		}

		return OmokGame.GetPlayerStone(Game, uid);
	}

	public bool IsMyTurn(Int64 uid)
	{
		if (null == Game)
		{
			return false;
		}

		return OmokGame.GetCurrentTurn(Game) == GetOmokStone(uid);
	}

	private void NotifyTurnChange()
	{
		OnTurnChange?.Invoke();
	}

	private void NotifyGameCompleted(OmokStone winner)
	{
		OnGameCompleted?.Invoke(winner);
	}

}
