using GameAPIServer.Models.GameDb;
using GameAPIServer.Models.MasterDb;
using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;
using ZLogger;

namespace GameAPIServer.Services;

public class ItemService: IItemService
{
	private readonly ILogger<ItemService> _logger;
	private readonly IItemRepository _itemRepository;

	public ItemService(ILogger<ItemService> logger,  IItemRepository postGameRepository)
	{
		_logger = logger;
		_itemRepository = postGameRepository;
	}

	public async Task<ErrorCode> InsertUserItem(Int64 uid, int itemId, int itemCount = 1)
	{
		try
		{
			UserItem item = new UserItem
			{
				user_uid = uid,
				item_id = itemId,
				item_cnt = itemCount
			};

			if (false == await _itemRepository.InsertUserItem(item))
			{
				return ErrorCode.DbItemInsertFail;
			}

			return ErrorCode.None;

		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[InsertUserItem] ErrorCode: {ErrorCode.DbItemInsertException}, Uid: {uid}");
			return ErrorCode.DbItemInsertException;
		}
	}
}
