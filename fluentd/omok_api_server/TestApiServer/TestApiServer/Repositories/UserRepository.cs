using Microsoft.Extensions.Options;
using System.Data;
using TestApiServer.Models.Database;
using TestApiServer.Repositories.Interfaces;
using TestApiServer.ServerCore;
using ZLogger;
using MySqlConnector;
using SqlKata.Execution;

namespace TestApiServer.Repositories;

public class UserRepository : IUserRepository
{
	readonly ILogger<UserRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;

	IDbConnection _dbConn;
	SqlKata.Compilers.MySqlCompiler _compiler;
	QueryFactory _queryFactory;

	public UserRepository(ILogger<UserRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_logger = logger;
		_dbConfig = dbConfig;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new QueryFactory(_dbConn, _compiler);
	}

	public async Task<User?> GetUserByIdAsync(Int64 id)
	{
		try
		{
			return await _queryFactory.Query("user_info")
								.Where("user_uid", id)
								.Select(User.SelectColumns)
								.FirstOrDefaultAsync<User>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetUserByUid Failed] Uid:{id}, ErrorMessage:{e.Message}");
			return null;
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
