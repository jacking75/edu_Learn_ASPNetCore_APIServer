using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using Microsoft.Extensions.Logging;
using ZLogger;
using HiveAPIServer.Model.DAO;

namespace HiveAPIServer.Repository;

public class HiveDb : IHiveDb
{
	readonly IOptions<ServerConfig> _dbConfig;
	readonly ILogger<HiveDb> _logger;
	IDbConnection _dbConn;
	readonly SqlKata.Compilers.MySqlCompiler _compiler;
	readonly QueryFactory _queryFactory;

	public HiveDb(ILogger<HiveDb> logger, IOptions<ServerConfig> dbConfig)
	{
		_logger = logger;
		_dbConfig = dbConfig;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new QueryFactory(_dbConn, _compiler);
	}

	public void Dispose()
	{
		Close();
	}

	void Open()
	{
		_dbConn = new MySqlConnection(_dbConfig.Value.HiveDb);
		_dbConn.Open();
	}

	void Close()
	{
		_dbConn.Close();
	}

	public async Task<bool> CreateAsync<T>(string table, T data)
	{
		try
		{
			return 1 == await _queryFactory.Query(table).InsertAsync(data);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
			$"[InsertFailException] Table: {table} ErrorCode: {ErrorCode.HiveInsertFailException}");
			return false;
		}
	}

	public async Task<T> SelectAsync<T, S>(string table, string where, S value)
	{
		try
		{
			T result = await _queryFactory.Query(table)
									.Where(where, value)
									.FirstOrDefaultAsync<T>();
			return result;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
			$"[InsertFailException] Table: {table} ErrorCode: {ErrorCode.HiveSelectFailException}");
			return default;
		}
	}

	public async Task<bool> UpsertToken(Token token)
	{
		try
		{
			var tokenExist = await _queryFactory.Query("token")
									.Where("player_id", token.player_id)
									.ExistsAsync();


			if (false == tokenExist)
			{
				return 1 == await _queryFactory.Query("token").InsertAsync(token);
			}

			var result = await _queryFactory.Query("token")
									.Where("player_id", token.player_id)
									.UpdateAsync(new { token.expire_dt });
			return 0 < result;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
			$"[UpsertFailException] UpsertTokenFail ErrorCode: {ErrorCode.HiveUpdateFailException}");
			return false;
		}
	}
}

