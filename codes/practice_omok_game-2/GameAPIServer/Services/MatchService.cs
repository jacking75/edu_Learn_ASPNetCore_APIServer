
using GameAPIServer.Services.Interfaces;
using GameAPIServer.Repositories.Interfaces;
using ZLogger;
using Microsoft.Extensions.Options;

namespace GameAPIServer.Services;

public class MatchService : IMatchService
{
	readonly ILogger<MatchService> _logger;
	private readonly IMemoryRepository _memoryDb;
	private readonly HttpClient _httpClient;

	public MatchService(ILogger<MatchService> logger, IMemoryRepository memoryDb, IOptions<ServerConfig> dbConfig, HttpClient httpClient)
	{
		_logger = logger;
		_memoryDb = memoryDb;
		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri(dbConfig.Value.MatchServer);
	}

	public async Task<(ErrorCode, MatchData?)> CheckMatch(Int64 uid)
	{
		try
		{
			var matchKey = SharedKeyGenerator.MakeMatchDataKey(uid.ToString());
			var (errorCode, result, expiry) =  await _memoryDb.GetWithExpiry<RedisMatchData>(matchKey);

			if (errorCode != ErrorCode.None)
			{
				return (errorCode, null);
			}

			if (result.MatchedUserID == 0)
			{
				return (ErrorCode.GameMatchUserNotFound, null);
			}

			var response = new MatchData
			{
				MatchedUserID = result.MatchedUserID,
				GameGuid = result.GameGuid,
				RemainTime = expiry
			};

            var userGameKey = SharedKeyGenerator.MakeUserGameKey(uid.ToString());
			if (false == await _memoryDb.StoreDataAsync(userGameKey, new RedisUserCurrentGame
				{
					Uid = uid,
					GameGuid = result.GameGuid,
				}, RedisExpiryTimes.UserDataExpiry))
			{
				return (ErrorCode.GameMatchCreateUserDataFail, null);
			}

            var deleteResult = await _memoryDb.DeleteDataAsync<RedisMatchData>(matchKey);

			return (errorCode, response);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[CheckMatch] Uid:{uid}, ErrorMessage:{e.Message}");
			return (ErrorCode.RedisMatchGetException, null);
		}
	}

    public async Task<ErrorCode> StartMatch(Int64 uid)
	{
		try
		{
            if (false == await CheckUserStatus(uid))
				return ErrorCode.GameMatchInvalidUserStatus;

            var response = await _httpClient.PostAsJsonAsync("/RequestMatching", new MatchRequest
			{
				Uid = uid
			});

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<ErrorCodeDTO>();

				if (null == result)
				{
					return ErrorCode.MatchServerInternalError;
				}

				return result.Result;
			}

			return ErrorCode.MatchServerRequestFail;

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[StartMatch] Uid:{uid}, ErrorMessage:{e.Message}");
			return ErrorCode.MatchServerRequestException;
		}
	}

    private async Task<bool> CheckUserStatus(Int64 uid)
    {
        var userGameKey = SharedKeyGenerator.MakeUserGameKey(uid.ToString());
        var (errorCode, userGame) = await _memoryDb.GetDataAsync<RedisUserCurrentGame>(userGameKey);

        if (ErrorCode.None == errorCode)
		{
			(errorCode, var game) = await _memoryDb.GetDataAsync<byte[]>(SharedKeyGenerator.MakeGameDataKey(userGame.GameGuid));
			
			if (null == game ||
				true == OmokGame.IsGameEnded(game))
			{
				_ = await _memoryDb.DeleteDataAsync<RedisUserCurrentGame>(userGameKey);
				return true;
			}

        }

        var matchKey = SharedKeyGenerator.MakeMatchDataKey(uid.ToString());
        (errorCode, var match) = await _memoryDb.GetDataAsync<RedisMatchData>(matchKey);

        if (errorCode == ErrorCode.None)
            return false;

        return true;
    }
}
