using GameServer.Repositories.Interfaces;
using GameServer.Services.Interfaces;
using GameShared.DTO;
using ServerShared;

namespace GameServer.Services;

public class DataLoadService : BaseLogger<DataLoadService>, IDataLoadService
{
	private readonly IUserService _userService;
	private readonly IAttendanceService _attendanceService;
	private readonly IItemService _itemService;
	private readonly IMasterDb _masterDb;
	public DataLoadService(ILogger<DataLoadService> logger, IUserService userService, IAttendanceService attendanceService, IItemService itemService, IMasterDb masterDb) : base(logger)
	{
		_userService = userService;
		_attendanceService = attendanceService;
		_itemService = itemService;
		_masterDb = masterDb;
	}

	public async Task<(ErrorCode, LoadedUserData?)> LoadUserData(Int64 uid, bool loadItems = false, bool loadAttendance = false)
	{
		try
		{
			var (errorCode, user) = await _userService.GetUser(uid);

			if (errorCode != ErrorCode.None || user == null)
			{
				ErrorLog(errorCode);
				return (errorCode, null);
			}

			LoadedUserData loadedUserData = new() { UserInfo = user.ToDTO() };

			if (loadItems == true)
			{
				(errorCode, var items) = await _itemService.GetUserItemsByUid(uid);

				if (errorCode != ErrorCode.None)
				{
					ErrorLog(errorCode);
					return (errorCode, null);
				}

				loadedUserData.UserItemInfo = items?.Select(i => i.ToDTO());
			}


			if (loadAttendance == true)
			{
				(errorCode, var attendance) = await _attendanceService.GetAttendanceList(uid);

				if (errorCode != ErrorCode.None)
				{
					ErrorLog(errorCode);
					return (errorCode, null);
				}

				loadedUserData.UserAttendanceInfo = attendance?.Select(i => i.ToDTO());
			}

			return (ErrorCode.None, loadedUserData);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.UserDataLoadException, new LoadedUserData());
		}
	}

	public (ErrorCode, LoadedMasterData?) LoadMasterData()
	{

		LoadedMasterData masterData = new LoadedMasterData
		{
			Items = _masterDb._items.Select(i => i.ToDTO()),
			Rewards = _masterDb._rewards.Select(i => i.ToDTO()),
			Attendances = _masterDb._attendances.Select(i => i.ToDTO())
		};

		return (ErrorCode.None, masterData);
	}
}
