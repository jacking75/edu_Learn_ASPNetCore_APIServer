
using GameServer.Models.GameDb;
using GameServer.Services.Interfaces;
using ServerShared;
using ServerShared.Redis;
using ServerShared.Repository.Interfaces;
using ServerShared.ServerCore;

namespace GameServer.Services;

public class OmokService : BaseLogger<OmokService>, IOmokService
{
	private readonly IMemoryDb _memoryDb;
	private readonly IMailService _mailService;
	private readonly IGameDb<GameResult> _gameDb;

	public OmokService(ILogger<OmokService> logger, IMemoryDb memoryDb, IGameDb<GameResult> gameDb, IMailService mailService) : base(logger)
	{
		_memoryDb = memoryDb;
		_gameDb = gameDb;
		_mailService = mailService;
	}

	public async Task<(ErrorCode, (int, byte[]?))> PeekTurn(Int64 uid, int lastCount)
	{
		try
		{
			var (result, user) = await GetUserGame(uid);
			 
			if (result != ErrorCode.None || user == null)
			{
				return (result, (0, null));
			}

			(result, var turnCount) = await GetTurnCount(user.GameGuid);

			if (result != ErrorCode.None)
			{
				return (result, (0, null));
			}

			if (turnCount == lastCount)
			{
				return (result, (turnCount, null));
			}

			// 턴수가 업데이트 되면 게임 정보를 보내준다
			(result, var game) = await GetGame(user.GameGuid);

			return (result, (turnCount, game));
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.OmokPeekException, (0, null));
		}

	}

	public async Task<ErrorCode> EnterOmok(Int64 uid)
	{
		try
		{
			var (result, user) = await GetUserGame(uid);

			if (result != ErrorCode.None || user == null)
			{
				return result;
			}

			// 게임 입장
			result = await Entergame(uid, user);
			 
			if (result != ErrorCode.None)
			{
				return result;
			}

			// 게임 시작
			if (await IsGameReady(user))
			{
				result = await StartGame(user);

				MetricLog("Game", new
				{
					guid = user.GameGuid
				});
			}

			return result;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.OmokEnterException;
		}
	}

	public async Task<ErrorCode> PutOmok(Int64 uid, int posX, int posY)
	{	
		try
		{
			var (result, user) = await GetUserGame(uid);

			if (result != ErrorCode.None || user == null)
			{
				return result;
			}

			// 게임 정보 가져오기
			(result, var game) = await GetGame(user.GameGuid);

			if (result != ErrorCode.None || game == null)
			{
				return result;
			}

			// 돌두기
			if (false == OmokGame.PlaceStone(game, user.UserStone, posX, posY))
			{
				return ErrorCode.OmokPutFail;
			}

			// 승리시 결과 저장 및 리워드 전송
			if (true == OmokGame.CheckWinAndEndGame(game, user.UserStone, posX, posY))
			{
				_ = await SendGameRewardMail(uid);
				_ = await SaveGameResult(game);
			}

			// 게임 정보 저장
			result = await SetGame(user.GameGuid, game);

			return result;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.OmokPutException;
		}
	}

	private async Task<ErrorCode> SaveGameResult(byte[] game)
	{
		var gameResult = new GameResult
		{
			ResultCode = OmokGame.GetGameResultCode(game),
			BlackUserUid = OmokGame.GetBlackPlayerUid(game),
			WhiteUserUid = OmokGame.GetWhitePlayerUid(game),
			StartDt = DateTimeOffset.FromUnixTimeSeconds(BitConverter.ToInt64(game, (int)OmokIndex.GameStartTime)).DateTime,
			EndDt = DateTimeOffset.UtcNow.DateTime
		};

		return await _gameDb.Set(gameResult);
	}

	private async Task<ErrorCode> SendGameRewardMail(Int64 uid)
	{
		var mail = new Mail
		{
			Title = "오목 게임 보상",
			Content = "오목 게임 보상입니다.",
			ReceiverUid = uid,
			ExpireDt = DateTime.Now.Add(Expiry.MailExpiry),
			RewardCode = OmokGame.OmokRewardCode
		};

		return await _mailService.SendMail(mail);
	}

	private async Task<ErrorCode> Entergame(Int64 uid, RedisUserGame user)
	{
		var gameKey = RedisKeyGenerator.MakeOmokKey(user.GameGuid);

		return await _memoryDb.StringSetRangeAsync(gameKey, OmokGame.GetPlayerIndex(user.UserStone), BitConverter.GetBytes(uid));
	}

	private async Task<bool> IsGameReady(RedisUserGame user)
	{
		var gameKey = RedisKeyGenerator.MakeOmokKey(user.GameGuid);
		var index = OmokGame.GetOpponentIndex(user.UserStone);
		var (result, value) = await _memoryDb.StringGetRangeAsync(gameKey, index, index + 7);

		var uid = BitConverter.ToInt64((byte[])value);

		if (result != ErrorCode.None || uid == 0)
		{
			return false;
		}

		return true;
	}

	private async Task<ErrorCode> StartGame(RedisUserGame user)
	{
		var gameKey = RedisKeyGenerator.MakeOmokKey(user.GameGuid);

		byte turnCount = 1;

		byte[] currentTime = BitConverter.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		byte[] combinedBytes = new byte[17];

		// 시작 시간, 턴 수 초기화
		combinedBytes[0] = turnCount;
		Buffer.BlockCopy(currentTime, 0, combinedBytes, 1, currentTime.Length);
		Buffer.BlockCopy(currentTime, 0, combinedBytes, 9, currentTime.Length);

		return await _memoryDb.StringSetRangeAsync(gameKey, (int)OmokIndex.TurnCount,
			combinedBytes);
	}

	private async Task<(ErrorCode, RedisUserGame?)> GetUserGame(Int64 uid)
	{
		var key = RedisKeyGenerator.MakeUserGameKey(uid.ToString());
		var (result, user) = await _memoryDb.GetAsync<RedisUserGame>(key);

		if (result != ErrorCode.None)
		{
			return (ErrorCode.OmokGetGameFailUserNotFound, null);
		}

		return (ErrorCode.None, user);
	}

	private async Task<(ErrorCode, byte[])> GetGame(string gameGuid)
	{
		var key = RedisKeyGenerator.MakeOmokKey(gameGuid);
		var (result, game) = await _memoryDb.GetAsync<byte[]>(key);
		 
		if (result != ErrorCode.None || game == null)
		{
			return (ErrorCode.OmokGetGameFailGameNotFound, Array.Empty<byte>());
		}

		return (result, game);
	}

	private async Task<ErrorCode> SetGame(string gameGuid, byte[] data)
	{
		var key = RedisKeyGenerator.MakeOmokKey(gameGuid);
		var result = await _memoryDb.SetAsync(key, data, Expiry.OmokGameExpiry);

		return result;
	}

	private async Task<(ErrorCode, int)> GetTurnCount(string gameGuid)
	{
		var key = RedisKeyGenerator.MakeOmokKey(gameGuid);
		var (result, value) = await _memoryDb.StringGetRangeAsync(key, (int)OmokIndex.TurnCount, (int)OmokIndex.TurnCount);

		if (result != ErrorCode.None)
		{
			return (result, 0);
		}

		byte[] byteValue = (byte[])value;

		return (result, byteValue[0]);
	}
}
