using MySqlConnector;
using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Models.MasterDb;
using ZLogger;
using System.Data;
using Microsoft.Extensions.Options;
using SqlKata.Execution;

namespace GameAPIServer.Repositories;

public class MasterRepository : IMasterRepository
{
	readonly ILogger<MasterRepository> _logger;
	readonly IOptions<ServerConfig> _dbConfig;
	IDbConnection _dbConn;
	readonly SqlKata.Compilers.MySqlCompiler _compiler;
	readonly QueryFactory _queryFactory;

	public VersionDAO _version { get; set; }

	public MasterRepository(ILogger<MasterRepository> logger, IOptions<ServerConfig> dbConfig)
	{
		_dbConfig = dbConfig;
		_logger = logger;

		Open();

		_compiler = new SqlKata.Compilers.MySqlCompiler();
		_queryFactory = new QueryFactory(_dbConn, _compiler);
	}

	public List<Item> _items { get; set; }
	public List<Money> _money { get; set; }
	public List<Reward> _rewards { get; set; }
	public List<Attendance> _attendances { get; set; }

	public async Task<bool> Load()
	{
		try
		{
			_version = await _queryFactory.Query("version").FirstOrDefaultAsync<VersionDAO>();

			_items = (await _queryFactory.Query("item").Select(ItemTemplate.SelectColumns).GetAsync<Item>()).ToList();
			_money = (await _queryFactory.Query("money").Select(MoneyTemplate.SelectColumns).GetAsync<Money>()).ToList();
			_rewards = (await _queryFactory.Query("reward").Select(RewardTemplate.SelectColumns).GetAsync<Reward>()).ToList();
			_attendances = (await _queryFactory.Query("attendance").Where("enabled_yn", 1).Select(AttendanceTemplate.SelectColumns).GetAsync<Attendance>()).ToList();

			var attendanceDetails = await _queryFactory.Query("attendance_detail").Select(AttendanceDetailTemplate.SelectColumns).GetAsync<AttendanceDetail>();

			foreach (var attendance in _attendances)
			{
				attendance.AttendanceDetails = attendanceDetails.Where(a => a.AttendanceCode == attendance.AttendanceCode).ToList();
			}

			return true;
		}
		catch (Exception e)
		{
			_logger.ZLogError(e, $"[LoadData Failed] ErrorMessage:{e.Message}");

			return false;
		}
	}

	public int GetAttendanceRewardCode(int attendanceCode, int attendanceCount)
	{

		var attendance = _attendances.FirstOrDefault(a => a.AttendanceCode == attendanceCode);
		if (null == attendance)
		{
			return 0;
		}

		var attendanceDetail = attendance.AttendanceDetails.FirstOrDefault(a => a.AttendanceCount == attendanceCount);
		if (null == attendanceDetail)
		{
			return 0;
		}

		return attendanceDetail.RewardCode;

	}
	public List<(Item?, int)> GetRewardItems(int rewardCode)
	{
		return _rewards.FindAll(r => r.RewardCode == rewardCode)
									.Select(r => (GetItem(r.ItemId), r.ItemCount))
									.ToList();
	}

	public Item? GetItem(int itemId)
	{
		return _items.FirstOrDefault(i => i.ItemId == itemId);
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

	public void Dispose()
	{
		Close();
	}
}
