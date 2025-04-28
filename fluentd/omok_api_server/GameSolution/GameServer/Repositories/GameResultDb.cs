using GameServer.Models.GameDb;
using Microsoft.Extensions.Options;
using ServerShared.Repository;
using SqlKata.Execution;

namespace GameServer.Repositories;

public class GameResultDb : GameDb<GameResult>
{
	public GameResultDb(ILogger<GameResult> logger, IOptions<ServerConfig> dbConfig) : base(logger, dbConfig)
	{
	}

	public override async Task<ErrorCode> Set(GameResult gameResult)
	{
		try
		{
			var count = await _queryFactory.Query(GameResult.Table)
				.InsertAsync(new
				{
					black_user_uid = gameResult.BlackUserUid,
					white_user_uid = gameResult.WhiteUserUid,
					start_dt = gameResult.StartDt,
					end_dt = gameResult.EndDt,
					result_code = gameResult.ResultCode,
				});

			if (count < 1)
			{
				return ErrorCode.DbGameResultInsertFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbGameResultInsertException;
		}
	}
}
