using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;
using GameClient.Providers;
using Microsoft.JSInterop;

namespace GameClient.Pages;

public partial class Login
{
	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
	[Inject]
	protected NavigationManager? Navigation { get; set; }
	[Inject]
	protected IToastService? ToastService { get; set; }
	[Inject]
    protected IJSRuntime? JS { get; set; }

    [Inject]
	protected LoadingStateProvider? LoadingStateProvider { get; set; }

	protected HiveLoginRequest User { get; set; } = new HiveLoginRequest();


    private async Task HandleLoginAsync()
	{
		try
		{
			LoadingStateProvider?.SetLoading(true);

			var response = await ((CookieStateProvider)AuthenticationStateProvider)
				.LoginAsync(User.Email, User.Password);

			if (ErrorCode.None != response)
			{
				HandleInvalidResponse(response);
			}
			else
			{
				ToastService?.ShowSuccess("Login successful!");
				Navigation?.NavigateTo("/");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			HandleInvalidSubmit(ex.Message);
		}
		finally
		{
			LoadingStateProvider?.SetLoading(false);
		}
	}

	private void RedirectToRegister()
	{
		Navigation?.NavigateTo("/register");
	}

	private void HandleInvalidSubmit(string message)
	{
		ToastService?.ShowError($"Failed to login. Please try again: {message} ");
	}

	private void HandleInvalidResponse(ErrorCode error)
	{
		ToastService?.ShowError($"Failed to login. ErrorCode:{error}");
	}

}
