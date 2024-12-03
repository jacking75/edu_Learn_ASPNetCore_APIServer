using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;
using ZLogger;

namespace GameAPIServer.Services;

public class AttendanceService : IAttendanceService
{
	readonly ILogger<AttendanceService> _logger;
	private readonly IAttendanceRepository _attendanceDb;
	private readonly IMasterRepository _masterDb;
	private readonly IUserRepository _userDb;
	private readonly IMailService _mailService;

	public AttendanceService(ILogger<AttendanceService> logger, IAttendanceRepository attendanceDb,
		IMailService mailService,
		IUserRepository userDb,
		IMasterRepository masterDb)
	{
		_logger = logger;
		_attendanceDb = attendanceDb;
		_masterDb = masterDb;
		_userDb = userDb;
		_mailService = mailService;
	}

	public async Task<ErrorCode> Attend(Int64 uid)
	{
		try
		{
			var user = await _userDb.GetUserByUid(uid);

			if (user == null)
			{
				return ErrorCode.AttendanceUpdateFailUserNotFound;
			}

			if (user.AttendanceUpdateTime >= DateTime.Today)
			{
				_logger.ZLogError($"[Attend] Already attend. Uid: {uid}");
				return ErrorCode.AttendanceUpdateFailAlreadyAttended;
			}

			var (errorCode, updatedRows) = await _attendanceDb.UpdateAttendanceList(uid);

			if (ErrorCode.None != errorCode)
			{
				_logger.ZLogError($"[UpdateAttendanceList] Uid: {uid}");
				return ErrorCode.AttendanceUpdateFail;
			}

			if (updatedRows.Any())
			{
				foreach (var row in updatedRows)
				{
					var rewardCode = _masterDb.GetAttendanceRewardCode(row.AttendanceCode, row.AttendanceCount);
					errorCode = await SendReward(uid, rewardCode);
				}
			}

			return errorCode;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[Attend] ErrorCode: {ErrorCode.AttendanceUpdateException}, Uid: {uid}");
			return ErrorCode.AttendanceUpdateException;
		}
	}

	public async Task<(ErrorCode, IEnumerable<AttendanceInfo>?)> GetAttendance(Int64 uid)
	{
		try
		{
			var result = await _attendanceDb.InsertMissingAttendanceList(uid, _masterDb._attendances);

			if (false == result)
			{
				_logger.ZLogError($"[InsertMissingAttendanceList] No data to insert. Uid: {uid}");
			}

			var attendance = await _attendanceDb.GetAttendanceList(uid);

			if (attendance == null)
			{
				_logger.ZLogError($"[GetAttendance] Uid: {uid}");
				return (ErrorCode.AttendanceGetFail, null);
			}

			return (ErrorCode.None, attendance);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[GetAttendance] ErrorCode: {ErrorCode.AttendanceGetException}, Uid: {uid}");
			return (ErrorCode.AttendanceGetException, null);
		}
	}
	private async Task<ErrorCode> SendReward(Int64 uid, int rewardCode)
	{
		if (0 == rewardCode)
		{
			return ErrorCode.None;
		}

		var result = await _mailService.SendReward(uid, rewardCode);

		if (ErrorCode.None != result)
		{
			_logger.ZLogError($"[Fail Send Attendance Reward] Uid: {uid}, RewardCode: {rewardCode}");
		}
		else
		{
			_logger.ZLogInformation($"[Success Send Attendance Reward] Uid: {uid}, RewardCode: {rewardCode}");
		}

		return result;
	}
}
