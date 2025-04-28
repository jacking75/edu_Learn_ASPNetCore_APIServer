using GameServer.Services.Interfaces;
using ServerShared;
using ServerShared.Redis;
using ServerShared.Repository.Interfaces;
using ServerShared.ServerCore;

namespace GameServer.Services;

public class MatchService : BaseLogger<MatchService>, IMatchService
{
	private readonly IMemoryDb _memoryDb;
	private readonly IHttpClientFactory _httpClientFactory;

	public MatchService(ILogger<MatchService> logger, IMemoryDb memoryDb, IHttpClientFactory httpClientFactory) : base(logger)
	{
		_memoryDb = memoryDb;
		_httpClientFactory = httpClientFactory;
	}

	public async Task<(ErrorCode, string)> CheckMatch(Int64 uid)
	{
		try
		{
			var (result, match) = await GetMatch(uid);

			if (result != ErrorCode.None || match == null)
			{
				return (ErrorCode.MatchCheckFailMatchNotFound, string.Empty);
			}

			var guid = match.GameGuid;

			result = await _memoryDb.SetAsync(RedisKeyGenerator.MakeUserGameKey(uid.ToString()),
								new RedisUserGame { GameGuid = guid, UserStone = match.UserStone }, 
								Expiry.UserGameExpiry);

			return (result, guid);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.MatchCheckException, string.Empty);
		}
	}

	public async Task<ErrorCode> StartMatch(Int64 uid)
	{
		try
		{
			var httpClient = _httpClientFactory.CreateClient("Match");

			var response = await httpClient.PostAsJsonAsync("/match", new MatchStartRequest { Uid = uid });

			if (response.IsSuccessStatusCode == false)
			{
				return ErrorCode.MatchStartFailMatchServer;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.MatchStartException;
		}
	}

	private async Task<(ErrorCode, RedisUserMatch?)> GetMatch(Int64 uid)
	{
		var key = RedisKeyGenerator.MakeMatchKey(uid.ToString());
		var (result, match) = await _memoryDb.GetAndDeleteAsync<RedisUserMatch>(key);

		if (result != ErrorCode.None)
		{
			ErrorLog(result);
			return (ErrorCode.MatchCheckFailUserNotFound, null);
		}

		return (ErrorCode.None, match);
	} 
}
