using System.Data;
using GameAPIServer.Models.GameDb;
using GameAPIServer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace GameAPIServer.Repositories;

public class GameResultRepository : IGameResultRepository
{
	readonly ILogger<GameResultRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;

	IDbConnection _dbConn;
	SqlKata.Compilers.MySqlCompiler _compiler;
	QueryFactory _queryFactory;

	public GameResultRepository(ILogger<GameResultRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_dbConfig = dbConfig;
		_logger = logger;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
	}

	public async Task<IEnumerable<GameResult>?> GetGameResultByUserUid(Int64 uid)
	{
		try
		{
			return await _queryFactory.Query("game_result")
								.Where("black_user_uid", uid)
								.OrWhere("white_user_uid", uid)
								.GetAsync<GameResult>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetGameResultByUserUid Failed] Uid:{uid}, ErrorMessage:{e.Message}");
			return null;
		}
	}

	public async Task<ErrorCode> InsertGameResult(GameResult gameResult)
	{
		try
		{
			var result = await _queryFactory.Query("game_result")
								   .InsertGetIdAsync<int>(new
								   {
									   gameResult.black_user_uid,
									   gameResult.white_user_uid,
									   gameResult.result_code,
									   gameResult.start_dt,
								   });

			if (result > 0)
				return ErrorCode.None;

			return ErrorCode.DbGameResultInsertFail;

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[InsertGameResult Failed] ErrorMessage: {e.Message}");
			return ErrorCode.DbGameResultInsertException;
		}
	}

	public void Dispose()
	{
		Close();
	}

	void Open()
	{
		_dbConn = new MySqlConnection(_dbConfig.Value.GameDb);
		_dbConn.Open();
	}

	void Close()
	{
		_dbConn.Close();
	}
}
