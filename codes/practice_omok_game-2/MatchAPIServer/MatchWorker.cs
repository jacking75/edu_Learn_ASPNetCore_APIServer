using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using MatchAPIServer.Repository;
using System.Collections.Concurrent;
using ZLogger;

namespace MatchAPIServer;

public class MatchWorker
{
	private readonly ILogger<MatchWorker> _logger;
	private readonly IMemoryRepository _memoryDb;
	private static readonly ConcurrentQueue<Int64> _userQueue = new();

	private readonly System.Threading.Thread _matchThread;

	public MatchWorker(ILogger<MatchWorker> logger, IMemoryRepository memoryDb)
	{
		_logger = logger;
		_memoryDb = memoryDb;

		_matchThread = new System.Threading.Thread(MonitorMatchQueue);
		_matchThread.Start();
	}

	private void MonitorMatchQueue()
	{
		while (true)
		{
			if (_userQueue.Count < 2)
			{
				System.Threading.Thread.Sleep(100); // 잠시 대기
				continue;
			}

			if (false == _userQueue.TryDequeue(out Int64 userA))
				continue;
			;

			if (false == _userQueue.TryDequeue(out Int64 userB))
			{
				_userQueue.Enqueue(userA);
				continue;
			}

			if (userA == userB)
			{
				_userQueue.Enqueue(userA);
				continue;
			}

			var gameGuid = Guid.NewGuid().ToString();

			if (false ==  StoreMatchData(userA, gameGuid).Result)
			{
				continue;
	
			}

			if (false ==  StoreMatchData(userB, gameGuid).Result)
			{
				continue;
			}

			if (false ==  StoreGameData(userA, userB, gameGuid).Result)
			{
				DeleteMatchData(userA, userB).Wait();
				_logger.ZLogError($"[StoreGameData] Rollback Matching for User:{userA} and User:{userB}");
				return;
			}
		}
	}
	public bool AddUser(Int64 uid)
	{
		_userQueue.Enqueue(uid);
		return true;
	}


	private async Task<bool> StoreGameData(Int64 userA, Int64 userB, string gameGuid)
	{
		var gamaDataKey = SharedKeyGenerator.MakeGameDataKey(gameGuid);

		var gameData = OmokGame.MakeOmokGame(userA, userB);
		if (gameData == null)
		{
			return false;
		}

		if (false == await _memoryDb.StoreDataAsync(gamaDataKey, gameData, RedisExpiryTimes.GameDataExpiry))
		{
			return false;
		}

		return true;
	}

	private async Task<bool> StoreMatchData(Int64 userUid, string gameGuid)
	{
		var key = SharedKeyGenerator.MakeMatchDataKey(userUid.ToString());
		var matchData = new RedisMatchData
		{
			MatchedUserID = userUid,
			GameGuid = gameGuid
		};

		return await _memoryDb.StoreDataAsync(key, matchData, RedisExpiryTimes.MatchDataExpiry);
	}

	private async Task DeleteMatchData(Int64 userA, Int64 userB)
	{
		var keyA = SharedKeyGenerator.MakeMatchDataKey(userA.ToString());
		var keyB = SharedKeyGenerator.MakeMatchDataKey(userB.ToString());

		await _memoryDb.DeleteDataAsync<Int64>(keyA);
		await _memoryDb.DeleteDataAsync<Int64>(keyB);
	}

}
