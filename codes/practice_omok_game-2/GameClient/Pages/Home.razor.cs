using GameClient.Providers;
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
	private LoadingStateProvider LoadingStateProvider { get; set; } = null!;
	[Inject]
	private InventoryStateProvider InventoryStateProvider { get; set; } = null!;
	[Inject]
	private MailStateProvider MailStateProvider { get; set; }

	[Inject]
	private GameContentProvider GameContentProvider { get; set; }

	protected override async Task OnInitializedAsync()
	{
		try
		{
			await GameContentProvider.LoadContent();
			await MailStateProvider.GetMailsAsync();
			_ = await InventoryStateProvider.GetUserItemsAsync();
		}
		catch (Exception ex)
		{
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