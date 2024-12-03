using GameClient.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace GameClient.Components.User;

public partial class Inventory
{
	private UserItemInfo? _selectedItem { get; set; }

	[Inject]
	private IToastService ToastService { get; set; }
	[Inject]
	private InventoryStateProvider InventoryStateProvider { get; set; }
	[Inject]
	private GameContentProvider GameContentProvider { get; set; }

	private List<UserItemInfo>? _list { get; set; }

	protected override async Task OnInitializedAsync()
	{
		_list = InventoryStateProvider.Items;
	}

	private Item? GetItem(int itemId)
	{
		if (null == GameContentProvider.GameData)
			return null;

		var items = GameContentProvider.GameData.Items;

		if (null == items)
			return null;

		return items.Find(x => x.ItemId == itemId);
	}

	private string GetItemImg(int itemId)
	{
		return GetItem(itemId)?.ItemImage ?? "";
	}

	private string GetItemName(int itemId)
	{
		return GetItem(itemId)?.ItemName ?? "";
	}

	private void OnClickItem(UserItemInfo item)
	{
		if (item == _selectedItem)
		{
			_selectedItem = null;
			return;
		}

		_selectedItem = item;
		ToastService.ShowSuccess($"Item {item.ItemId} clicked");
	}
}
