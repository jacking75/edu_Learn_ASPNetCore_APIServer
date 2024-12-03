using System.Data;
using GameAPIServer.Controllers;
using GameAPIServer.Models.GameDb;
using GameAPIServer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace GameAPIServer.Repositories;

public class ItemRepository : IItemRepository
{
	readonly ILogger<ItemRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;

	IDbConnection _dbConn;
	SqlKata.Compilers.MySqlCompiler _compiler;
	QueryFactory _queryFactory;

	public ItemRepository(ILogger<ItemRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_dbConfig = dbConfig;
		_logger = logger;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
	}
	public async Task<bool> InsertUserItem(UserItem item)
	{
		try
		{
			string query = $@"
            INSERT INTO user_item (user_uid, item_id, item_cnt)
            VALUES ({item.user_uid}, {item.item_id}, {item.item_cnt})
            ON DUPLICATE KEY UPDATE item_cnt = item_cnt + {item.item_cnt};";

			var result = await _queryFactory.StatementAsync(query);
			return result > 0;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[InsertUserItem Failed] ErrorMessage: {e.Message}");
			return false;
		}
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

	public void Dispose()
	{
		Close();
	}
}
