using System.Net.Http.Json;

namespace GameClient.Providers;

public class InventoryStateProvider
{
	private readonly IHttpClientFactory _httpClientFactory;
	
	public List<UserItemInfo> Items { get; private set; }

	public InventoryStateProvider(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}
	public async Task<(ErrorCode, List<UserItemInfo>)> GetUserItemsAsync()
	{
		try
		{
			List<UserItemInfo> items = new List<UserItemInfo>();

			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsync("/userdata/items", null);

			if (!response.IsSuccessStatusCode)
			{
				return (ErrorCode.UserItemGetBadRequest, items);
			}

			var result = await response.Content.ReadFromJsonAsync<UserItemLoadResponse>();

			if (ErrorCode.None != result.Result)
			{
				return (result.Result, items);
			}

			items.AddRange(result.ItemData.UserItem);

			Items = items;

			return (ErrorCode.None, items);
		}
		catch (Exception e)
		{
			return (ErrorCode.UserItemGetException, null);
		}
	}
}
