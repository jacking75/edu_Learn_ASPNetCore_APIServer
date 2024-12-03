using GameAPIServer.Models.GameDb;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;
using ZLogger;

namespace GameAPIServer.Repositories.Interfaces;

public  class UserRepository : IUserRepository
{
	readonly ILogger<UserRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;

	IDbConnection _dbConn;
	SqlKata.Compilers.MySqlCompiler _compiler;
	QueryFactory _queryFactory;

	public UserRepository(ILogger<UserRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_dbConfig = dbConfig;
		_logger = logger;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
	}

	public async Task<UserInfo?> GetUserByNickname(string nickname)
	{
		try
		{
			return await _queryFactory.Query("user_info")
								.Where("user_name", nickname)
								.Select(User.SelectColumns)
								.FirstOrDefaultAsync<User>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetUserByNickname Failed] Nickname:{nickname}, ErrorMessage:{e.Message}");
			return null;
		}
	}

	public async Task<UserInfo?> GetUserByPlayerId(Int64 playerId)
	{
		try
		{
			return await _queryFactory.Query("user_info")
								  .Where("hive_player_id", playerId)
								  .Select(User.SelectColumns)
								  .FirstOrDefaultAsync<UserInfo>();

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetUserByPlayerId Failed] Player ID:{playerId}, ErrorMessage:{e.Message}");
			return null;
		}
	}

	public async Task<UserInfo?> GetUserByUid(Int64 uid)
	{
		try
		{
			return  await _queryFactory.Query("user_info")
								.Where("user_uid", uid)
								.Select(User.SelectColumns)
								.FirstOrDefaultAsync<UserInfo>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetUserByUid Failed] Uid:{uid}, ErrorMessage:{e.Message}");
			return null;
		}
	}

	public async Task<IEnumerable<UserMoneyInfo>?> GetUserMoneyByUid(Int64 uid)
	{
		try
		{
			return await _queryFactory.Query("user_money")
								.Where("user_uid", uid)
								.Select(UserMoney.SelectColumns)
								.GetAsync<UserMoneyInfo>();

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetUserMoneyByUid Failed] Uid:{uid}, ErrorMessage:{e.Message}");
			return null;
		}
	}

	public async Task<IEnumerable<UserItemInfo>?> GetUserItemByUid(Int64 uid)
	{
		try
		{
			return await _queryFactory.Query("user_item")
								.Where("user_uid", uid)
								.Select(UserItem.SelectColumns)
								.GetAsync<UserItemInfo>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetUserItemByUid Failed] Uid:{uid}, ErrorMessage:{e.Message}");
			return null;
		}
	}

	public async Task<(ErrorCode, int)> InsertUser(Int64 playerId)
	{
		try
		{
			var result = await _queryFactory.Query("user_info")
								   .InsertGetIdAsync<int>(new
								   {
									   hive_player_id = playerId,
									   create_dt = DateTime.Now,
									   recent_login_dt = DateTime.Now,
									   user_name = $"Player_{playerId}",
								   });
			return (ErrorCode.None, result);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[InsertUser Failed] Player ID:{playerId}, ErrorMessage: {e.Message}");
			return (ErrorCode.DbUserInsertException, 0);
		}
	}

	public async Task<bool> UpdateRecentLoginTime(Int64 uid)
	{
		try
		{
			var result = await _queryFactory.Query("user_info").Where("user_uid", uid).UpdateAsync(new
			{
				recent_login_dt = DateTime.Now,
			});

			return true;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[UpdateRecentLoginTime Failed] user_uid:{uid}, ErrorMessage: {e.Message}");
			return false;
		}
	}

	public async Task<bool> UpdateUserNickname(Int64 uid, string nickname)
	{
		try
		{
			var result = await _queryFactory.Query("user_info").Where("user_uid", uid).UpdateAsync(new
			{
				user_name = nickname,
			});

			return true;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[UpdateUserNickname Failed] user_uid:{uid}, ErrorMessage: {e.Message}");
			return false;
		}
	}

	public async Task<int> GetTotalUserPlayCountByUid(long uid)
	{
		try
		{
			return await _queryFactory.Query("game_result")
								.Where("black_user_uid", uid)
								.OrWhere("white_user_uid",uid)
								.CountAsync<int>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetTotalUserGameByUid Failed] Uid:{uid}, ErrorMessage:{e.Message}");
			return 0;
		}
	}

	public async Task<int> GetTotalUserWinCountByUid(long uid)
	{
		try
		{
			return await _queryFactory.Query("game_result")
				.Where("black_user_uid", uid).Where("result_code", (int)GameResultCode.BlackWin)
				.OrWhere("white_user_uid", uid).Where("result_code", (int)GameResultCode.WhiteWin)
				.CountAsync<int>();
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetTotalUserGameByUid Failed] Uid:{uid}, ErrorMessage:{e.Message}");
			return 0;
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
