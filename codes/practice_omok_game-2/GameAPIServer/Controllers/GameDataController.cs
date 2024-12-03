using GameAPIServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class GameData : SecureController<UserData>
{
	private readonly IGameDataService _gameDataService;

	public GameData(ILogger<UserData> logger, IGameDataService gameDataService) : base(logger)
	{
		_gameDataService = gameDataService;
	}

	[HttpPost]
	public GameDataLoadResponse LoadGameData()
	{
		GameDataLoadResponse response = new();
		response.GameData = _gameDataService.LoadGameData();
		return response;
	}
}
