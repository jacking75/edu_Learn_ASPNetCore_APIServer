using GameAPIServer.Models.GameDb;
using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;

namespace GameAPIServer.Services;

public class OmokService : IOmokService
{
	private readonly ILogger<OmokService> _logger;
	private readonly IMemoryRepository _memoryRepository;
	private readonly IGameResultRepository _gameResultRepository;
	private readonly IMailService _mailService;

	public OmokService(ILogger<OmokService> logger, IMemoryRepository memoryRepository, IGameResultRepository gameResultRepository, IMailService mailService)
	{
		_logger = logger;
		_memoryRepository = memoryRepository;
		_gameResultRepository = gameResultRepository;
		_mailService = mailService;
	}

	public async Task<(ErrorCode, byte[]?)> EnterGame(Int64 uid)
	{
		try
		{
			var (errorCode, gameGuid) = await GetGameGuidFromUserGame(uid);

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

			(errorCode, var game) = await _memoryRepository.GetGameAsync(gameGuid);

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

			if (true == OmokGame.IsGameStarted(game))
			{
				return (ErrorCode.None, game);
			}
			
			if (false == OmokGame.TryEnterPlayer(game, uid))
			{
				return (ErrorCode.GameEnterPlayerFail, null);
			}

			if (OmokGame.IsGameReady(game))
			{
                OmokGame.StartGame(game);

            }

            if (false == await _memoryRepository.SetGameAsync(gameGuid, game))
			{
				return (ErrorCode.GameSaveGameFail, null);
			}

			return (ErrorCode.None, game);

		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to enter game: Uid={Uid}", uid);
			return (ErrorCode.GameEnterGameException, null);
		}
	}

	public async Task<(ErrorCode, byte[]?)> SetOmokStone(Int64 uid, int x, int y)
	{
		try
		{
			var (errorCode, userGameInfo) = await _memoryRepository.GetUserGameInfo(uid);

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

            (errorCode, var game) = await _memoryRepository.GetGameAsync(userGameInfo.GameGuid);

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

			var stone = OmokGame.GetPlayerStone(game, uid);

			errorCode = OmokGame.TryPutStone(game, x, y, OmokGame.GetPlayerStone(game, uid));

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

            if (true == OmokGame.IsGameEnded(game))
			{
				_ = await SaveGameResult(game);
				_ = await SendGameReward(game);
			}

			if (false == await _memoryRepository.SetGameAsync(userGameInfo.GameGuid, game))
			{
				return (ErrorCode.GameSaveStoneFail, null);
			}

			return (ErrorCode.None, game);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to set stone: Uid={Uid}", uid);
			return (ErrorCode.GameSaveStoneException, null);
		}
	}

	public async Task<(ErrorCode, byte[]?)> PeekGame(Int64 uid)
	{
		try
		{
			var (errorCode, gameGuid) = await GetGameGuidFromUserGame(uid);

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

			(errorCode, var game) = await _memoryRepository.GetGameAsync(gameGuid);

			if (ErrorCode.None != errorCode)
			{
				return (errorCode, null);
			}

			if (true == OmokGame.IsGameStarted(game) &&
				true == OmokGame.CheckExpiry(game , uid))
			{
				_ = await _memoryRepository.SetGameAsync(gameGuid, game);
			}

			return (ErrorCode.None, game);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get game: Uid={Uid}", uid);
			return (ErrorCode.GameGetException, null);
		}
	}

	private async Task<ErrorCode> SaveGameResult(byte[] game)
	{
		if (true == OmokGame.IsGameResultSaved(game))
		{ 
			return ErrorCode.GameSaveResultFail;
		}

        var gameResult = new GameResult
		{
			result_code = (int)OmokGame.GetGameResultCode(game),
			black_user_uid= OmokGame.GetBlackPlayerUid(game),
			white_user_uid= OmokGame.GetWhitePlayerUid(game),
			start_dt = DateTimeOffset.FromUnixTimeMilliseconds(OmokGame.GetGameStartTime(game)).UtcDateTime
		};

		var saveResult = await _gameResultRepository.InsertGameResult(gameResult);

		if (ErrorCode.None == saveResult)
		{
			OmokGame.SetGameResultSaved(game);
		}

		return saveResult;
	}

	private async Task<ErrorCode> SendGameReward(byte[] game)
	{
		if (true == OmokGame.IsGameRewardSent(game))
		{
			return ErrorCode.GameSendRewardFail;
		}


		var mailResponse = await _mailService.SendReward(
			OmokGame.GetGameWinnerUid(game),
			OmokGame.OmokRewardCode,
			"Omok Game Reward");

		if (ErrorCode.None == mailResponse)
		{
			OmokGame.SetGameRewardSent(game);
		}

		return mailResponse;
	}

	private async Task<(ErrorCode, string)> GetGameGuidFromUserGame(Int64 uid)
	{
		var (errorCode, userGameInfo) = await _memoryRepository.GetUserGameInfo(uid);

		if (ErrorCode.None != errorCode)
		{
			return (errorCode, "");
		}

		if (null == userGameInfo)
		{
			return (ErrorCode.GameEnterFailGameNotFound, "");
		}

		return (ErrorCode.None, userGameInfo.GameGuid);
	}
}
