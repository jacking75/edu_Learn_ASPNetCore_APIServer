using GameClient.Services;
using Microsoft.AspNetCore.Components;

namespace GameClient.Pages;

public enum MenuType
{
	None,
	Profile,
	Mail,
	Inventory,
	Attendance,
	Shop,
	Setting,
}

public partial class Home
{
	private MenuType _currentMenu = MenuType.None;
	[Inject]
	private MailService MailService { get; set; } = null!;

	protected override async Task OnInitializedAsync()
	{
		try
		{
			await MailService.GetMailsAsync();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	private bool IsOpen()
	{
		return _currentMenu != MenuType.None;
	}

	private void CloseMenu()
	{
		_currentMenu = MenuType.None;
		StateHasChanged();
	}

	private void ShowMenu(MenuType menu)
	{
		_currentMenu = menu;
		StateHasChanged();
	}
}