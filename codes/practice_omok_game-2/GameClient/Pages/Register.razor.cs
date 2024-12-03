using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using GameClient.Providers;

namespace GameClient.Pages;

public partial class Register
{
	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
	[Inject]
	protected NavigationManager Navigation { get; set; }
	[Inject]
	protected IToastService ToastService { get; set; }

	protected HiveRegisterRequest User { get; set; } = new HiveRegisterRequest();

	private async Task HandleRegisterAsync()
	{
		try
		{
			var response = await ((CookieStateProvider)AuthenticationStateProvider)
				.RegisterAsync(User.Email, User.Password);

			if (ErrorCode.None != response)
			{
				HandleInvalidResponse(response);
			}
			else
			{
				ToastService.ShowSuccess("Account created successfully!");
				Navigation.NavigateTo("/");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			HandleInvalidSubmit();
		}

	}

	private void HandleInvalidSubmit()
	{
		ToastService.ShowError("Failed to create account. Please try again.");
	}

	private void HandleInvalidResponse(ErrorCode error)
	{
		ToastService.ShowError($"Failed to create account. Error Code:{error}");
	}

}
