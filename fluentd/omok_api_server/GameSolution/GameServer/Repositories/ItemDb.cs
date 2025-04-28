using GameServer.Models;
using GameServer.Models.GameDb;
using GameShared.DTO;
using Microsoft.Extensions.Options;
using ServerShared.Repository;
using SqlKata.Execution;

namespace GameServer.Repositories;

public class ItemDb : GameDb<UserItem>
{
	public ItemDb(ILogger<UserItem> logger, IOptions<ServerConfig> dbConfig) : base(logger, dbConfig)
	{
	}

	public override async Task<ErrorCode> Set(UserItem item)
	{
		try
		{
			string query = $@"
            INSERT INTO user_item (uid, item_id, item_count)
            VALUES ({item.Uid}, {item.ItemId}, {item.ItemCount})
            ON DUPLICATE KEY UPDATE item_count = item_count + {item.ItemCount};";

			var result = await _queryFactory.StatementAsync(query);

			if (result < 1)
			{
				return ErrorCode.DbItemInsertFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbItemInsertException;
		}
	}

	public override async Task<(ErrorCode, IEnumerable<UserItem>?)> GetAll(Int64 uid)
	{
		try
		{
			var result = await _queryFactory.Query("user_item")
								.Where("uid", uid)
								.Select(UserItem.SelectColumns)
								.GetAsync<UserItem>();

			return (ErrorCode.None, result);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.DbItemGetException, null);
		}
	}
}
