using GameServer.Models.GameDb;
using Microsoft.Extensions.Options;
using ServerShared.Repository;
using SqlKata.Execution;

namespace GameServer.Repositories;

public class UserDb : GameDb<User>
{
	public UserDb(ILogger<User> logger, IOptions<ServerConfig> dbConfig) : base(logger, dbConfig)
	{
	}

	public override async Task<(ErrorCode, User?)> Get(Int64 uid)
	{
		try
		{
			var user = await _queryFactory.Query(User.Table)
				.Where("uid", uid)
				.Select(User.SelectColumns)
				.FirstOrDefaultAsync<User>();

			if (user == null)
			{
				return (ErrorCode.DbUserGetFailUserNotFound, null);
			}

			user.WinCount = await GetTotalUserWinCountByUid(user.Uid);
			user.PlayCount = await GetTotalUserPlayCountByUid(user.Uid);

			return (ErrorCode.None, user);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.DbUserGetException, null);
		}
	}

	public override async Task<ErrorCode> Set(User user)
	{
		try
		{
			var count = await _queryFactory.Query(User.Table)
				.InsertAsync(new { 
					hive_player_id = user.HivePlayerId,
					nickname = $"USER#{user.HivePlayerId}",
				});

			if (count < 1)
			{
				return ErrorCode.DbUserInsertFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbUserInsertException;
		}
	}

	public override async Task<ErrorCode> Update(Int64 uid, object? value = null)
	{
		try
		{
			var count = await _queryFactory.Query(User.Table)
				.Where("uid", uid)
				.UpdateAsync(value);

			if (count < 1)
			{
				return ErrorCode.DbUserUpdateFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbUserUpdateException;
		}
	}

	private async Task<int> GetTotalUserPlayCountByUid(Int64 uid)
	{
		try
		{
			return await _queryFactory.Query("game_result")
								.Where("black_user_uid", uid)
								.OrWhere("white_user_uid", uid)
								.CountAsync<int>();
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return 0;
		}
	}

	private async Task<int> GetTotalUserWinCountByUid(Int64 uid)
	{
		try
		{
			return await _queryFactory.Query("game_result")
				.Where("black_user_uid", uid).Where("result_code", (int)OmokResultCode.BlackWin)
				.OrWhere("white_user_uid", uid).Where("result_code", (int)OmokResultCode.WhiteWin)
				.CountAsync<int>();
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return 0;
		}
	}
}
