using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ZLogger;
using ServerShared;
using ServerShared.Redis;
using ServerShared.Repository.Interfaces;

namespace MatchAPIServer;

public class MatchWorker : BaseLogger<MatchWorker>
{
	private readonly IMemoryDb _memoryDb;
	private readonly System.Threading.Thread _matchThread;
	private static readonly ConcurrentQueue<Int64> _userQueue = new();

	public MatchWorker(ILogger<MatchWorker> logger, IMemoryDb memoryDb) : base(logger)
	{
		_memoryDb = memoryDb;
		_matchThread = new System.Threading.Thread(MonitorMatchQueue);
		_matchThread.Start();
	}

	public bool AddUser(Int64 uid)
	{
		_userQueue.Enqueue(uid);
		return true;
	}

	private void MonitorMatchQueue()
	{
		while (true)
		{
			if (_userQueue.Count < 2)
			{
				System.Threading.Thread.Sleep(100); 
			}

			if (false == _userQueue.TryDequeue(out Int64 userA))
			{
				continue;
			}

			if (false == _userQueue.TryDequeue(out Int64 userB))
			{
				_userQueue.Enqueue(userA);
				continue;
			}

			if (userA == userB)
			{
				continue;
			}

			var gameGuid = Guid.NewGuid().ToString();

			if (false == StoreMatchData(userA, OmokStone.Black, gameGuid).Result ||
				false == StoreMatchData(userB, OmokStone.White, gameGuid).Result)
			{
				DeleteMatchData(userA, userB).Wait();
				_logger.ZLogError($"[StoreGameData] Rollback Matching for User:{userA} and User:{userB}");
				continue;
			}

			MetricLog("Match", new
			{
				guid = gameGuid
			});

			if (false ==  StoreGameData(gameGuid).Result)
			{
				DeleteMatchData(userA, userB).Wait();
				_logger.ZLogError($"[StoreGameData] Rollback Matching for User:{userA} and User:{userB}");
				continue;
			}
		}
	}

	private async Task<bool> StoreGameData(string guid)
	{
		var gamaDataKey = RedisKeyGenerator.MakeOmokKey(guid);

		var gameData = OmokGame.MakeOmokGame();

		var result = await _memoryDb.SetAsync(gamaDataKey, gameData, Expiry.OmokGameExpiry);

		return result == ErrorCode.None;
	}

	private async Task<bool> StoreMatchData(Int64 uid, OmokStone stone, string guid)
	{
		var key = RedisKeyGenerator.MakeMatchKey(uid.ToString());
		var matchData = new RedisUserMatch
		{
			UserStone = stone,
			GameGuid = guid
		};

		var result = await _memoryDb.SetAsync(key, matchData, Expiry.MatchExpiry);

		if (result != ErrorCode.None)
		{
			return false;
		}

		return true;
	}

	private async Task DeleteMatchData(Int64 userA, Int64 userB)
	{
		var keyA = RedisKeyGenerator.MakeMatchKey(userA.ToString());
		var keyB = RedisKeyGenerator.MakeMatchKey(userB.ToString());

		_ = await _memoryDb.DeleteAsync<Int64>(keyA);
		_ = await _memoryDb.DeleteAsync<Int64>(keyB);
	}
}
