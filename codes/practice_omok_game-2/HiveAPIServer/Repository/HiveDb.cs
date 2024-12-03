using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using Microsoft.Extensions.Logging;
using ZLogger;

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

	public async Task<bool> UpsertAsync<T>(string table, string primaryKey, T data)
	{
		try
		{
			var pkValue = data.GetType().GetProperty(primaryKey).GetValue(data, null);
			var record = await _queryFactory.Query(table).Where(primaryKey, pkValue).FirstOrDefaultAsync<T>();

			if (null == record)
			{
				return 1 == await _queryFactory.Query(table).InsertAsync(data);
			}

			return 1 == await _queryFactory.Query(table).Where(primaryKey, pkValue).UpdateAsync(data);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e,
			$"[InsertFailException] Table: {table} ErrorCode: {ErrorCode.HiveUpdateFailException}");
			return false;
		}
	}
}

