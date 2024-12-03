using System.Net.Http.Json;

namespace GameClient.Providers;

public class GameContentProvider
{
	private bool _initialized = false;
	private readonly IHttpClientFactory _httpClientFactory;

	public LoadedGameData? GameData { get; private set; }

	public GameContentProvider(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task LoadContent()
	{
		if (true == _initialized)
			return;
		try
		{
			var gameClient = _httpClientFactory.CreateClient("Game");
			var response = await gameClient.PostAsync("/gamedata" , null);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<GameDataLoadResponse>();

				if (ErrorCode.None != result.Result)
				{
					return;
				}

				GameData = result.GameData;
				_initialized = true;
			}

		}
		catch (Exception ex)
		{

		}
	}

	public List<(Item, int)> GetItemsFromRewardCode(int rewardCode)
	{
		var items = new List<(Item, int)>();

		if (null == GameData?.Items)
			return items;

		if (null == GameData?.Rewards)
			return items;

		var rewards = GameData.Rewards.Where(x => x.RewardCode == rewardCode);

		if (!rewards.Any())
			return items;

		foreach (var reward in rewards)
		{
			var template = GameData.Items.FirstOrDefault(x => x.ItemId == reward.ItemId);

			if (null == template)
				continue;

			items.Add((template, reward.ItemCount));
		}

		return items;
	}
}
