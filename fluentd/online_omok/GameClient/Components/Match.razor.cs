using GameClient.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components;

public partial class Match : IDisposable
{
	private bool IsMatched = false;
	private CancellationTokenSource? _cancellationTokenSource;

	[Inject]
	protected IToastService ToastService { get; set; }
	[Inject]
	protected MatchStateProvider MatchStateProvider { get; set; }
	[Inject]
	protected LoadingStateProvider LoadingStateProvider { get; set; }
	[Inject]
	protected NavigationManager NavigationManager { get; set; }

	public void Dispose()
	{
		MatchStateProvider.OnMatchCompleted -= HandleMatchComplete;
		MatchStateProvider.OnMatchStart -= HandleMatchStart;
		_cancellationTokenSource?.Dispose();
		DisposeCancelllationToken();
	}

	protected override void OnInitialized()
	{
		MatchStateProvider.OnMatchCompleted += HandleMatchComplete;
		MatchStateProvider.OnMatchStart += HandleMatchStart;
	}

	protected async void HandleMatchRequest()
	{
		LoadingStateProvider?.SetLoading(true);

		try
		{
			if (null != _cancellationTokenSource)
			{
				DisposeCancelllationToken();
			}

			_cancellationTokenSource = new CancellationTokenSource();

			var response = await MatchStateProvider.RequestMatchAsync(_cancellationTokenSource.Token);

			if (ErrorCode.None != response)
			{
				ToastService?.ShowError($"Match request failed, ERROR: {response}");
			}
		}
		catch (Exception ex)
		{
			ToastService?.ShowError($"Match request failed, ERROR: {ex.Message}");
			LoadingStateProvider?.SetLoading(false);
		}
	}

	private void HandleMatchStart()
	{
		LoadingStateProvider?.SetLoading(true);
	}

	private void HandleMatchComplete(ErrorCode errorCode)
	{
		if (errorCode == ErrorCode.None)
		{
			ShowMatchResult();
		}
		else
		{
			ToastService?.ShowError($"Match has been completed with error: {errorCode}");
		}

		LoadingStateProvider?.SetLoading(false);
		DisposeCancelllationToken();
	}

	private void ShowMatchResult()
	{
		IsMatched = true;
		StateHasChanged();
	}

	private void AcceptGame()
	{
		DisposeCancelllationToken();

		string url = $"/playomok";
		NavigationManager.NavigateTo(url);
	}

	private void CancelGame()
	{
		DisposeCancelllationToken();
		LoadingStateProvider?.SetLoading(false);
		ToastService?.ShowError("Game has been cancelled.");
		StateHasChanged();
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
