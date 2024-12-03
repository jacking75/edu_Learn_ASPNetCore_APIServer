using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;
using GameAPIServer.Services;

public class GameDataService : IGameDataService
{
	readonly ILogger<DataLoadService> _logger;
	private readonly IMasterRepository _masterDb;

	public GameDataService(ILogger<DataLoadService> logger, IMasterRepository masterDb)
	{
		_logger = logger;
		_masterDb = masterDb;
	}

	public LoadedGameData LoadGameData()
	{
		LoadedGameData loadGameData = new LoadedGameData
		{
			Attendances = _masterDb._attendances,
			Items = _masterDb._items,
			Rewards = _masterDb._rewards
		};

		return loadGameData;
	}
}
