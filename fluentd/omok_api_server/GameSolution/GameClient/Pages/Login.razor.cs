using Blazored.LocalStorage;
using GameClient.Providers;
using GameClient.Services;
using GameShared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Pages;

public partial class Login
{
	[Inject]
	IToastService ToastService { get; set; }
	[Inject]
    ILocalStorageService LocalStorage { get; set; }

	[Inject]
    AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
	AuthService AuthService { get; set; }

	[Inject]
	NavigationManager Navigation { get; set; }

	[Inject]
	protected LoadingStateProvider LoadingStateProvider { get; set; }

	protected HiveLoginRequest User { get; set; } = new HiveLoginRequest();

	private async Task HandleLoginAsync()
	{
		try
		{
			LoadingStateProvider?.SetLoading(true);

			var (result, response) = await AuthService.LoginAsync(User.Email, User.Password);

			if (ErrorCode.None != result)
			{
				HandleInvalidResponse(result);
			}
			else
			{
                await LocalStorage.SetItemAsStringAsync("accesstoken", response.AccessToken);
				await LocalStorage.SetItemAsStringAsync("uid", response.Uid.ToString());
                //await ((CustomAuthenticationStateProvider)AuthenticationStateProvider).GetAuthenticationStateAsync();
                ToastService.ShowSuccess("Login successful!");
                Navigation.NavigateTo("/", true);
            }
		}
		catch (Exception ex)
		{
			ToastService.ShowError(ex.Message);
		}
		finally
		{
			LoadingStateProvider.SetLoading(false);
		}
	}
	private void RedirectToRegister()
	{
		Navigation.NavigateTo("/register");
	}

	private void HandleInvalidResponse(ErrorCode error)
	{
		ToastService.ShowError($"Failed to login. ErrorCode:{error}");
	}

}
