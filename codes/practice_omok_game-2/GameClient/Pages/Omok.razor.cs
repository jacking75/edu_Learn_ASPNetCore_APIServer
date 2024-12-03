using GameClient.Providers;
using GameShared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Pages;

public partial class Omok : IDisposable
{
	private bool _isGameComplete = false;
	private CancellationTokenSource? _cancellationTokenSource;
	private UserInfo? Opponent;

	[Inject]
	protected IToastService ToastService { get; set; }
	[Inject]
	protected LoadingStateProvider LoadingStateProvider { get; set; }
	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }	
	[Inject]
	protected GameStateProvider GameStateProvider { get; set; }

	[Inject]
	private NavigationManager NavigationManager { get; set; }

	protected override async Task OnInitializedAsync()
	{
		GameStateProvider.OnGameStarted += HandleGameStart;
		GameStateProvider.OnGameCompleted += HandleGameComplete;
		GameStateProvider.OnTurnChange += HandleTurnChange;

		await LoadGameDataAsync();
	}

	private async Task LoadGameDataAsync()
	{
		LoadingStateProvider?.SetLoading(true);

		try
		{
			if (null != _cancellationTokenSource)
			{
				DisposeCancelllationToken();
			}

			_cancellationTokenSource = new CancellationTokenSource();

			var errorCode = await GameStateProvider.EnterGameAsync(_cancellationTokenSource.Token);

			if (errorCode != ErrorCode.None)
			{
				ToastService?.ShowError($"Failed to load game data. Error: {errorCode}");
				return;
			}

			await LoadOpponentDataAsync();

			ToastService?.ShowSuccess("Game data loaded successfully");

		}
		catch(Exception e)
		{
			ToastService?.ShowError($"Failed to load game data. Error: {e.Message}");
		}
        finally
        {
			LoadingStateProvider?.SetLoading(false);
		}

		if (false == GameStateProvider.GameStart)
		{
			LoadingStateProvider?.SetLoading(true);
		}
	}

	private async Task LoadOpponentDataAsync()
	{
		if (null == GameStateProvider.Game)
		{
			ToastService?.ShowError("Game data is not loaded yet. Please try again later.");
			return;
		}

		try
		{
			var (result, opponent) = await GameStateProvider.LoadUserInfoAsync(OmokGame.GetOpponentUid(GameStateProvider.Game, GetUid()));

			if (result != ErrorCode.None)
			{
				ToastService?.ShowError($"Failed to load opponent data. Error: {result}");
				return;
			}

			Opponent = opponent;
		}
		catch (Exception e)
		{
			ToastService?.ShowError($"Failed to load game data. Error: {e.Message}");
		}
	}

	private async Task HandleCellClick((int X, int Y) pos)
	{
        if (false == GameStateProvider.IsMyTurn(GetUid()))
        {
            ToastService?.ShowError("Not your turn");
            return;
        }

        try
		{
			LoadingStateProvider?.SetLoading(true);

            var errorCode = await GameStateProvider.PlayGameAsync(pos.X, pos.Y);
            if (errorCode != ErrorCode.None)
            {
                ToastService?.ShowError($"Failed to play move at ({pos.X}, {pos.Y}). Error: {errorCode}");
                return;
            }

            ToastService?.ShowSuccess($"Move played at ({pos.X}, {pos.Y})");
        }
		catch(Exception e)
		{
            ToastService?.ShowError($"Failed to play move at ({pos.X}, {pos.Y}). Error: {e.Message}");
        }
		finally
		{
			if (GameStateProvider.IsMyTurn(GetUid()))
				LoadingStateProvider?.SetLoading(false);
		}
	}

	private void HandleGameComplete(OmokStone winner)
	{
		DisposeCancelllationToken();
		ToastService?.ShowEvent($"Game has been completed, {winner} won!");
		_isGameComplete = true;
		StateHasChanged();
    }

	private void HandleExitGame()
	{
		LoadingStateProvider?.SetLoading(false);
		DisposeCancelllationToken();
		string url = $"/";
		NavigationManager.NavigateTo(url , true);
	}

	private void HandleGameStart()
	{
		ToastService?.ShowEvent("Game has been started");
		StateHasChanged();
	}

	private void HandleTurnChange()
	{
		if (true == GameStateProvider.IsMyTurn(GetUid()))
		{
			LoadingStateProvider?.SetLoading(false);
		}
		else
		{
			LoadingStateProvider?.SetLoading(true);
		}

		StateHasChanged();
	}

	private Int64 GetUid()
	{
		if (AuthenticationStateProvider == null)
		{
			return 0;
		}

		var uid = ((CookieStateProvider)AuthenticationStateProvider).AuthenticatedUser?.Uid;

		if (uid == null)
		{
			return 0;
		}

		return uid.Value;
	}

	public void Dispose()
	{
		GameStateProvider.OnGameStarted -= HandleGameStart;
		GameStateProvider.OnGameCompleted -= HandleGameComplete;
		GameStateProvider.OnTurnChange -= HandleTurnChange;
		LoadingStateProvider?.SetLoading(false);
		DisposeCancelllationToken();
	}

	private void DisposeCancelllationToken()
	{
		if (null != _cancellationTokenSource)
		{
			if (_cancellationTokenSource.Token.CanBeCanceled)
			{
				_cancellationTokenSource.Cancel();
			};

			_cancellationTokenSource.Dispose();
			_cancellationTokenSource = null;
		}
	}
}
