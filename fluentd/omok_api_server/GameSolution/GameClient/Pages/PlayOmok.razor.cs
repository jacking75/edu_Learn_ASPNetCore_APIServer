using GameClient.Providers;
using GameClient.Services;
using GameShared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Pages;

public partial class PlayOmok : IDisposable
{
	private bool _isGameComplete = false;
	private CancellationTokenSource? _cancellationTokenSource;

	private UserInfo? MyInfo;
	private UserInfo? OpponentInfo;

	private bool isMyTurn;

	[Inject]
	protected IToastService ToastService { get; set; }
	[Inject]
	protected LoadingStateProvider LoadingStateProvider { get; set; }
	[Inject]
	protected DataLoadService DataLoadService { get; set; }
	[Inject]
	protected OmokStateProvider OmokStateProvider { get; set; }
	[Inject]
	private NavigationManager NavigationManager { get; set; }

	protected override async Task OnInitializedAsync()
	{
		OmokStateProvider.OnStateChange += HandleStateChange;
		MyInfo = DataLoadService.UserData?.UserInfo;
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

			var errorCode = await OmokStateProvider.StartOmokAsync(_cancellationTokenSource.Token);

			if (errorCode != ErrorCode.None)
			{
				ToastService?.ShowError($"Failed to load game data. Error: {errorCode}");
				return;
			}

			ToastService?.ShowSuccess("Game data loaded successfully");

		}
		catch (Exception e)
		{
			ToastService?.ShowError($"Failed to load game data. Error: {e.Message}");
		}
	}

	private async Task LoadOpponentDataAsync()
	{
		if (null == OmokStateProvider.OmokData ||
			null == MyInfo)
		{
			ToastService?.ShowError("Game data is not loaded yet. Please try again later.");
			return;
		}

		try
		{
			(var result, OpponentInfo) = await DataLoadService.LoadUserInfo(
				OmokGame.GetOpponentUid(OmokStateProvider.OmokData, MyInfo.Uid));

			if (result != ErrorCode.None)
			{
				ToastService?.ShowError($"Failed to load opponent data. Error: {result}");
				return;
			}

		}
		catch (Exception e)
		{
			ToastService?.ShowError($"Failed to load opponent data. Error: {e.Message}");
		}
	}

	private async Task HandleCellClick((int X, int Y) pos)
	{
		try
		{
			LoadingStateProvider?.SetLoading(true);

			var errorCode = await OmokStateProvider.PutOmokAsync(pos.X, pos.Y);
			if (errorCode != ErrorCode.None)
			{
				ToastService?.ShowError($"Failed to play move at ({pos.X}, {pos.Y}). Error: {errorCode}");
				LoadingStateProvider?.SetLoading(false);
				return;
			}

			ToastService?.ShowSuccess($"Move played at ({pos.X}, {pos.Y})");
		}
		catch (Exception e)
		{
			ToastService?.ShowError($"Failed to play move at ({pos.X}, {pos.Y}). Error: {e.Message}");
			LoadingStateProvider?.SetLoading(false);
		}
	}


	private void OnGameComplete(OmokResultCode result)
	{
		DisposeCancelllationToken();
		ToastService?.ShowEvent($"Game has been completed, {result}!");
		_isGameComplete = true;
	}

	private void HandleExitGame()
	{
		LoadingStateProvider?.SetLoading(false);
		string url = $"/";
		NavigationManager.NavigateTo(url, true);
	}

	private void HandleStateChange()
	{
		if (OmokStateProvider.OmokData != null &&
			OmokGame.IsGameEnded(OmokStateProvider.OmokData))
		{
			var result = OmokGame.GetGameResultCode(OmokStateProvider.OmokData);
			OnGameComplete(result);
		}

		if (null != MyInfo)
		{
			isMyTurn = OmokStateProvider.IsMyTurn(MyInfo.Uid);
			LoadingStateProvider?.SetLoading(!isMyTurn);
		}

		StateHasChanged();
	}

	public void Dispose()
	{
		OmokStateProvider.OnStateChange -= HandleStateChange;
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
