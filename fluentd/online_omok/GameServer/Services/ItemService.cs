using GameServer.Models.GameDb;
using GameServer.Services.Interfaces;
using ServerShared.Repository.Interfaces;
using ServerShared;

namespace GameServer.Services;

public class ItemService : BaseLogger<ItemService>, IItemService
{
	private readonly IGameDb<UserItem> _itemDb;

	public ItemService(ILogger<ItemService> logger, IGameDb<UserItem> itemDb) : base(logger)
	{
		_itemDb = itemDb;
	}

	public async Task<(ErrorCode, IEnumerable<UserItem>?)> GetUserItemsByUid(Int64 uid)
	{
		try
		{
			var (result, items) = await _itemDb.GetAll(uid);

			if (result != ErrorCode.None)
			{
				ErrorLog(result);
				return (ErrorCode.ItemGetFail, null);
			}

			return (ErrorCode.None, items);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.ItemGetException, null);
		}
	}

	public async Task<ErrorCode> InsertUserItem(Int64 uid, int itemId, int itemCount = 1)
	{
		try
		{
			var item = new UserItem
			{
				Uid = uid,
				ItemId = itemId,
				ItemCount = itemCount
			};

			var errorCode = await _itemDb.Set(item);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.ItemInsertFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.ItemInsertException;
		}
	}
}