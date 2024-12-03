namespace GameClient.Providers;

public class LoadingStateProvider
{
	public bool IsLoading { get; set; } = false;

	public Action? OnChange;

	public void SetLoading(bool isLoading)
	{
		IsLoading = isLoading;
		OnChange?.Invoke();
	}
}
