using GameAPIServer.Models.GameDb;
using GameAPIServer.Models.MasterDb;
using GameAPIServer.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;
using ZLogger;

namespace GameAPIServer.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
	readonly ILogger<AttendanceRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;

	IDbConnection _dbConn;
	SqlKata.Compilers.MySqlCompiler _compiler;
	QueryFactory _queryFactory;

	public AttendanceRepository(ILogger<AttendanceRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_dbConfig = dbConfig;
		_logger = logger;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
	}

	public async Task<IEnumerable<AttendanceInfo>> GetAttendanceList(Int64 uid)
	{
		return await _queryFactory.Query("attendance").Where("user_uid", uid)
													.Select(UserAttendance.SelectColumns)
													.GetAsync<AttendanceInfo>();
	}

	public async Task<(ErrorCode, IEnumerable<AttendanceInfo>)> UpdateAttendanceList(Int64 uid)
	{
		try
		{

			var result = await _queryFactory.Query("attendance")
									.Where("user_uid", uid)
									.IncrementAsync("attendance_seq", 1);

			var rows = await _queryFactory.Query("attendance")
											.Where("user_uid", uid)
											.Select(UserAttendance.SelectColumns)
											.GetAsync<AttendanceInfo>();

			var updateResult = await _queryFactory.Query("user_info").Where("user_uid", uid).UpdateAsync(new
			{
				attendance_update_dt = DateTime.Now,
			});

			return (ErrorCode.None, rows);
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[UpdateAttendence Failed] ErrorMessage: {e.Message}");
			return (ErrorCode.DbAttendanceUpdateException, null);
		}
	}

	public async Task<bool> InsertMissingAttendanceList(Int64 uid, List<Attendance> list)
	{
		if (0 == list.Count)
		{
			return true;
		}

		try
		{
			var cols = new[] { "user_uid", "attendance_code" };
			var data = list.Select(item => new object[] { uid, item.AttendanceCode }).ToArray();
			var result = await _queryFactory.Query("attendance").InsertAsync(cols, data);

			return result > 0;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[InsertAttendance Failed] ErrorMessage: {e.Message}");
			return false;
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

