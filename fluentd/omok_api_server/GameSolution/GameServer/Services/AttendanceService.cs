using GameServer.Models.GameDb;
using GameServer.Repositories.Interfaces;
using GameServer.Services.Interfaces;
using ServerShared;
using ServerShared.Repository.Interfaces;
using ServerShared.ServerCore;

namespace GameServer.Services;

public class AttendanceService : BaseLogger<AttendanceService>, IAttendanceService
{
	private readonly IGameDb<UserAttendance> _attendanceDb;
	private readonly IMasterDb _masterDb;
	private readonly IMailService _mailService;
	private readonly IUserService _userService;
	public AttendanceService(ILogger<AttendanceService> logger, IGameDb<UserAttendance> attendanceDb,IMasterDb masterDb, IMailService mailService, IUserService userService) : base(logger)
	{
		_attendanceDb = attendanceDb;
		_masterDb = masterDb;
		_mailService = mailService;
		_userService = userService;
	}

	public async Task<(ErrorCode, IEnumerable<UserAttendance>?)> GetAttendanceList(Int64 uid)
	{
		try
		{
			var (errorCode, rows) = await _attendanceDb.GetAll(uid);

			if (errorCode != ErrorCode.None)
			{
				return (ErrorCode.AttendanceGetFail, null);
			}

			return (ErrorCode.None, rows);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.AttendanceGetException, null);
		}
	}

	public async Task<ErrorCode> UpdateAttendanceList(Int64 uid)
	{
		try
		{
			var errorCode = await CheckAlreadyAttended(uid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			errorCode = await _attendanceDb.Update(uid, null);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.AttendanceUpdateListFail;
			}

			errorCode = await UpdateLastAttendanceTime(uid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			(errorCode, var rows) = await GetAttendanceList(uid);

			if (errorCode != ErrorCode.None || rows == null)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			if (rows.Any())
			{
				foreach (var row in rows)
				{				
					errorCode = await SendAttendanceReward(uid, row);
				}
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.AttendanceUpdateListException;
		}
	}

	public async Task<ErrorCode> UpdateAttendance(Int64 uid, UserAttendance attendance)
	{
		try
		{
			var errorCode = await CheckAlreadyAttended(uid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			errorCode = await _attendanceDb.Update(uid, attendance);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return ErrorCode.AttendanceUpdateFail;
			}

			errorCode = await UpdateLastAttendanceTime(uid);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			errorCode = await SendAttendanceReward(uid, attendance);

			if (errorCode != ErrorCode.None)
			{
				ErrorLog(errorCode);
				return errorCode;
			}

			return errorCode;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.AttendanceUpdateException;
		}
	}

	private async Task<ErrorCode> SendAttendanceReward(Int64 uid, UserAttendance attendance)
	{
		try
		{
			var rewardCode = _masterDb.GetRewardCodeByAttendance(attendance.AttendanceCode, attendance.AttendanceCount);

			if (rewardCode == 0)
			{
				return ErrorCode.AttendanceRewardFailRewardNotFound;
			}

			var result = await _mailService.SendMail(new Mail
			{
				ReceiverUid = uid,
				Title = $"{attendance.AttendanceCount}일째 출석 선물",
				Content = $"출석 보상입니다",
				ExpireDt = DateTime.Now.Add(Expiry.MailExpiry),
				RewardCode = rewardCode,
				StatusCode = MailStatusCode.Unread,
				MailType = MailType.System,
			});

			return result;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.AttendanceRewardException;
		}
	}

	private async Task<ErrorCode> CheckAlreadyAttended(Int64 uid)
	{
		try
		{
			var (errorCode, user) = await _userService.GetUser(uid);

			if (errorCode != ErrorCode.None)
			{
				return ErrorCode.AttendanceCheckFail; 
			}

			if (user?.LastAttendanceDt >= DateTime.Today)
			{
				return ErrorCode.AttendanceCheckFailAlreadyAttended;
			}

			return ErrorCode.None;

		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.AttendanceCheckException;
		}
	}

	private async Task<ErrorCode> UpdateLastAttendanceTime(Int64 uid)
	{
		try
		{
			var errorCode = await _userService.UpdateLastAttendanceTime(uid);

			if (errorCode != ErrorCode.None)
			{
				return ErrorCode.AttendanceUpdateUserFail;
			}

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.AttendanceUpdateUserException;
		}
	}

}
