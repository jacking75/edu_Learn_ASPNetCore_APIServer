using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace GameAPIServer.Repository;

public class MasterDb : IMasterDb
{
    
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<MasterDb> _logger;
    IDbConnection _dbConn;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;
    readonly IMemoryDb _memoryDb;
    readonly IGameDb _gameDb;
    
    public VersionDAO _version { get; set; }
    public List<AttendanceRewardData> _attendanceRewardList { get; set; }    
    public List<GachaRewardData> _gachaRewardList { get; set; }
    public List<ItemLevelData> _itemLevelList { get; set; }


    public MasterDb(ILogger<MasterDb> logger, IOptions<DbConfig> dbConfig, IMemoryDb memoryDb, IGameDb gameDb)
    {
        _logger = logger;
        _dbConfig = dbConfig;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
        _memoryDb = memoryDb;
        _gameDb = gameDb;

    }

    public void Dispose()
    {
        Close();
    }

    public async Task<bool> Load()
    {
        try
        {
            _version = await _queryFactory.Query($"version").FirstOrDefaultAsync<VersionDAO>();
            
            _attendanceRewardList = (await _queryFactory.Query($"master_attendance_reward").GetAsync<AttendanceRewardData>()).ToList();
            
            _itemLevelList = (await _queryFactory.Query($"master_item_level").GetAsync<ItemLevelData>()).ToList();

            var gachaRewards = await _queryFactory.Query($"master_gacha_reward").GetAsync<GachaRewardInfo>();
            
            _gachaRewardList = new();
            
            foreach (var gachaRewardInfo in gachaRewards)
            {
                GachaRewardData gachaRewardData = new();
                gachaRewardData.gachaRewardInfo = gachaRewardInfo;
                gachaRewardData.gachaRewardList = (await _queryFactory.Query("master_gacha_reward_list")
                                                   .Where("gacha_reward_key", gachaRewardInfo.gacha_reward_key)
                                                   .GetAsync<RewardData>())
                                                   .ToList();
                _gachaRewardList.Add(gachaRewardData);
            }

            await LoadUserScore();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            _logger.ZLogError(e,
                $"[MasterDb.Load] ErrorCode: {ErrorCode.MasterDB_Fail_LoadData}");
            return false;
        }

        if (!ValidateMasterData())
        {
            _logger.ZLogError($"[MasterDb.Load] ErrorCode: {ErrorCode.MasterDB_Fail_InvalidData}");
            return false;
        }

        return true;
    }

    public async Task<ErrorCode> LoadUserScore()
    {
        var usersScore = await _gameDb.SelectAllUserScore();
        foreach (var userScore in usersScore)
        {
            await _memoryDb.SetUserScore(userScore.uid, userScore.total_bestscore);
        }

        return ErrorCode.None;
    }

    bool ValidateMasterData()
    {
        if (_version == null || 
            _attendanceRewardList.Count == 0 ||
            _gachaRewardList.Count == 0)
        {
            return false;
        }

        return true;
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
