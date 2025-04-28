using GameServer.Models;
using GameServer.Repositories.Interfaces;
using GameShared;
using Microsoft.Extensions.Options;
using MySqlConnector;
using ServerShared.ServerCore;
using SqlKata.Execution;
using System.Data;
using ZLogger;

namespace GameServer.Repositories;

public class MasterDb : BaseLogger<MasterDb>, IMasterDb
{
	readonly IOptions<ServerConfig> _dbConfig;
	IDbConnection _dbConn;
	readonly SqlKata.Compilers.MySqlCompiler _compiler;
	readonly QueryFactory _queryFactory;

	public VersionDAO _version { get; set; }

	public MasterDb(ILogger<MasterDb> logger, IOptions<ServerConfig> dbConfig) : base(logger)
	{
		_dbConfig = dbConfig;
		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new QueryFactory(_dbConn, _compiler);
	}

	public List<Item> _items { get; set; }
	public List<Reward> _rewards { get; set; }
	public List<Attendance> _attendances { get; set; }

	public async Task<bool> Load()
	{
		try
		{
			_version = await _queryFactory.Query(VersionDAO.Table).FirstOrDefaultAsync<VersionDAO>();
			_items = (await _queryFactory.Query(Item.Table).Select(Item.SelectColumns).GetAsync<Item>()).ToList();
			_rewards = (await _queryFactory.Query(Reward.Table).Select(Reward.SelectColumns).GetAsync<Reward>()).ToList();
			_attendances = (await _queryFactory.Query(Attendance.Table).Where("enabled_yn", 1).Select(Attendance.SelectColumns).GetAsync<Attendance>()).ToList();

			var attendanceDetails = await _queryFactory.Query(AttendanceReward.Table).Select(AttendanceReward.SelectColumns).GetAsync<AttendanceReward>();

			foreach (var attendance in _attendances)
			{
				attendance.AttendanceRewards = attendanceDetails.Where(a => a.AttendanceCode == attendance.AttendanceCode);
			}

			return true;
		}
		catch (Exception e)
		{
			ExceptionLog(e, "Load Master Data Failed");
			return false;
		}
		finally
		{
			Close();
		}
	}

	public Item? GetItemById(int itemId)
	{
		return _items.FirstOrDefault(i => i.ItemId == itemId);
	}

	public List<(Item?, int)> GetItemsByRewardCode(int rewardCode)
	{
		return _rewards.FindAll(r => r.RewardCode == rewardCode)
									.Select(r => (GetItemById(r.ItemId), r.ItemCount))
									.ToList();
	}

	public int GetRewardCodeByAttendance(int attendanceCode, int attendanceCount)
	{
		var attendance = _attendances.FirstOrDefault(a => a.AttendanceCode == attendanceCode);
		if (null == attendance)
		{
			return 0;
		}

		var attendanceDetail = attendance.AttendanceRewards.FirstOrDefault(a => a.AttendanceCount == attendanceCount);
		if (null == attendanceDetail)
		{
			return 0;
		}

		return attendanceDetail.RewardCode;
	}


	void Open()
	{
		_dbConn = new MySqlConnection(_dbConfig.Value.MasterDb);
		_dbConn.Open();
	}

	void Close()
	{
		_dbConn.Close();
	}

}
