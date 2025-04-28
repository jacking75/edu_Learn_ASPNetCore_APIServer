using GameServer.Models.GameDb;
using GameServer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using ServerShared.Repository;
using SqlKata.Execution;

namespace GameServer.Repositories;

public class AttendanceDb : GameDb<UserAttendance>
{
	private readonly IMasterDb _masterDb;
	public AttendanceDb(ILogger<UserAttendance> logger, IOptions<ServerConfig> dbConfig, IMasterDb masterDb) : base(logger, dbConfig)
	{
		_masterDb = masterDb;
	}

	public override async Task<(ErrorCode, IEnumerable<UserAttendance>?)> GetAll(Int64 uid)
	{
		try
		{
			var attendance = await _queryFactory.Query(UserAttendance.Table)
				.Where("uid", uid)
				.Select(UserAttendance.SelectColumns)
				.GetAsync<UserAttendance>();

			if (attendance == null)
			{
				return (ErrorCode.DbAttendanceGetFail, null);
			}

			return (ErrorCode.None, attendance);
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return (ErrorCode.DbAttendanceGetException, null);
		}
	}

	public override async Task<ErrorCode> Update(Int64 uid, object? value = null)
	{
		try
		{
			var cols = new[] { "uid", "attendance_code" };
			var data = _masterDb._attendances.Select(item => new object[] { uid, item.AttendanceCode }).ToArray();
			var insert = await _queryFactory.Query(UserAttendance.Table)
				.InsertAsync(cols, data);

			var update = await _queryFactory.Query(UserAttendance.Table)
				.Where("uid", uid).IncrementAsync("attendance_count");

			return ErrorCode.None;
		}
		catch (Exception e)
		{
			ExceptionLog(e);
			return ErrorCode.DbAttendanceUpdateException;
		}
	}

}
