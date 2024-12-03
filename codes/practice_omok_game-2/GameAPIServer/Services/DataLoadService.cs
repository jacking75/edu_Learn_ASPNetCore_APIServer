using System;
using System.Security.Cryptography;
using GameAPIServer.Models.GameDb;
using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;
using ZLogger;

namespace GameAPIServer.Services;

public class DataLoadService : IDataLoadService
{
	readonly ILogger<DataLoadService> _logger;
	private readonly IUserRepository _userDb;
	private readonly IAttendanceService _attendanceService;

	public DataLoadService(ILogger<DataLoadService> logger, IUserRepository userDb, IAttendanceService attendanceService)
	{
		_logger = logger;
		_userDb = userDb;
		_attendanceService = attendanceService;
	}

	public async Task<(ErrorCode, LoadedUserData?)> LoadUserData(Int64 uid)
	{
		try
		{
			var user = await _userDb.GetUserByUid(uid);
			if (null == user)
			{
				_logger.ZLogError($"[LoadUserData] Uid: {uid}");
				return (ErrorCode.DbLoadUserInfoFail, null);
			}

			user.WinCount = await _userDb.GetTotalUserWinCountByUid(uid);
			user.PlayCount = await _userDb.GetTotalUserPlayCountByUid(uid);

			var userMoney = await _userDb.GetUserMoneyByUid(uid);
			if (null == userMoney)
			{
				_logger.ZLogError($"[LoadUserMoney] Uid: {uid}");
				return (ErrorCode.DbLoadUserMoneyFail, new LoadedUserData());
			}

			var (errorCode, attendances) = await LoadAttendanceData(uid, user.AttendanceUpdateTime);

			var userData = new LoadedUserData
			{
				User = user,
				UserMoney = userMoney,
				UserAttendances = attendances
			};

			return (ErrorCode.None, userData);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[LoadUserData] ErrorCode: {ErrorCode.DbLoadUserException}, Uid: {uid}");
			return (ErrorCode.None, null);
		}
	}

	private async Task<(ErrorCode, IEnumerable<AttendanceInfo>?)> LoadAttendanceData(Int64 uid, DateTime updateTime)
	{
		try
		{
			var (errorCode, attendance) = await _attendanceService.GetAttendance(uid);

			if (ErrorCode.None != errorCode)
			{
				_logger.ZLogError($"[LoadAttendanceData] ErrorCode: {errorCode}, Uid: {uid}");
				return (errorCode, null);
			}

			return (errorCode, attendance);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[LoadAttendanceData] ErrorCode: {ErrorCode.AttendanceGetException}, Uid: {uid}");
			return (ErrorCode.AttendanceGetException, null);
		}
	}
	public async Task<(ErrorCode, LoadedItemData?)> LoadItemData(Int64 uid)
	{
		try
		{
			var result = await _userDb.GetUserItemByUid(uid);

			if (null == result)
			{
				_logger.ZLogError($"[LoadUserItem] Uid: {uid}");
				return (ErrorCode.DbLoadUserItemFail, null);
			}

			var itemData = new LoadedItemData
			{
				UserItem = result,
			};

			return (ErrorCode.None, itemData);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[LoadAttendanceData] ErrorCode: {ErrorCode.AttendanceGetException}, Uid: {uid}");
			return (ErrorCode.AttendanceGetException, null);
		}

	}

	public async Task<(ErrorCode, LoadedProfileData?)> LoadUserProfile(Int64 uid)
	{
		try
		{
			var user = await _userDb.GetUserByUid(uid);
			if (null == user)
			{
				_logger.ZLogError($"[LoadUserData] Uid: {uid}");
				return (ErrorCode.DbLoadUserNotFound, null);
			}

			var userData = new LoadedProfileData
			{
				User = user,
			};

			return (ErrorCode.None, userData);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[LoadUserData] ErrorCode: {ErrorCode.DbLoadUserException}, Uid: {uid}");
			return (ErrorCode.None, null);
		}
	}
}