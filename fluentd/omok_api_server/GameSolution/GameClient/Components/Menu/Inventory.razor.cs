using GameClient.Models;
using GameClient.Services;
using GameShared.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.Menu;

public partial class Inventory
{
	private Item? _selectedItem { get; set; }

	[Inject]
	IToastService ToastService { get; set; } = default!;
	[Inject]
	DataLoadService DataLoadService { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		var response = await DataLoadService.LoadUserDataAsync();
	}
	private void OnClickItem(Item item)
	{
		if (item == _selectedItem)
		{
			_selectedItem = null;
			return;
		}

		_selectedItem = item;
		ToastService.ShowSuccess($"Item {item.ItemName} clicked");
	}
}
