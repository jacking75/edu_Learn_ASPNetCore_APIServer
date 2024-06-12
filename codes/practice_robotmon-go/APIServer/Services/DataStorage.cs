using System.Collections.Concurrent;
using ApiServer.Data;
using ApiServer.Model;
using Dapper;
using MySqlConnector;

namespace ApiServer.Services
{
    public class DataStorage : IDataStorage
    {
        private ConcurrentDictionary<Int64, Monster> s_monsterDic = new();
        private ConcurrentDictionary<Int32, DailyInfo> s_dailyCheckDic = new();
        private ConcurrentDictionary<Int64, MonsterUpgrade> s_monsterUpgradeDic = new();
        private ConcurrentDictionary<Int64, MonsterEvolve> s_monsterEvolveDic = new();
        private ConcurrentDictionary<Int32, LevelUpInfo> s_levelUpInfoDic = new();

        public void Load(string dbConnString)
        {
            using var dBConn = new MySqlConnection(dbConnString);
            {
                dBConn.Open();

                var monsterList = dBConn.Query<TableMonsterInfo>("select * from monsterinfo");
                foreach (var value in monsterList)
                {
                    s_monsterDic.TryAdd(value.MID, new Monster()
                    {
                        Att = value.Att,
                        Def = value.Def,
                        HP = value.HP,
                        Level = value.Level,
                        MonsterName = value.MonsterName,
                        StarCount = value.StarCount,
                        UpgradeCount = value.UpgradeCount,
                        Type = value.Type
                    });
                }

                var dailyCheckList = dBConn.Query<TableDailyInfo>("select * from dailyinfo");
                foreach (var value in dailyCheckList)
                {
                    s_dailyCheckDic.TryAdd(value.DayCount, new DailyInfo()
                    {
                        StarCount = value.StarCount
                    });
                }
                
                var updgradeList = dBConn.Query<TableMonsterUpgrade>("select * from monsterupgrade");
                foreach (var value in updgradeList)
                {
                    s_monsterUpgradeDic.TryAdd(value.MID, new MonsterUpgrade()
                    {
                        UpdateCost = value.UpgradeCost,
                        StarCost = value.StarCount,
                        Exp = value.Exp
                    });
                }
                
                var evolveList = dBConn.Query<TableMonsterEvolve>("select * from monsterevolve");
                foreach (var value in evolveList)
                {
                    s_monsterEvolveDic.TryAdd(value.MID, new MonsterEvolve()
                    {
                        EvolveMonsterID = value.EvolveMID,
                        CandyCount = value.StarCount
                    });
                }

                var levelUpList = dBConn.Query<TableUserLevelInfo>("select * from userlevelinfo");
                foreach (var value in levelUpList)
                {
                    s_levelUpInfoDic.TryAdd(value.Level, new LevelUpInfo()
                    {
                        MaxExpForLevelUp = value.LevelUpExp
                    });
                }
            }
        }
        
        public Monster GetMonsterInfo(Int64 monsterID)
        {
            if(s_monsterDic.TryGetValue(monsterID, out var value))
            {
                return value;
            }

            return null;
        }
        
        public DailyInfo GetDailyInfo(Int32 dailyIdx)
        {
            if(s_dailyCheckDic.TryGetValue(dailyIdx, out var value))
            {
                return value;
            }

            return null;
        }
        
        public MonsterUpgrade GetMonsterUpgrade(Int64 monsterIdx)
        {
            if(s_monsterUpgradeDic.TryGetValue(monsterIdx, out var value))
            {
                return value;
            }

            return null;
        }   
        
        public MonsterEvolve GetMonsterEvolve(Int64 monsterIdx)
        {
            if(s_monsterEvolveDic.TryGetValue(monsterIdx, out var value))
            {
                return value;
            }

            return null;
        }   
        
        public LevelUpInfo GetLevelUpMaxExp(Int32 level)
        {
            if(s_levelUpInfoDic.TryGetValue(level, out var value))
            {
                return value;
            }

            return null;
        }  
    }
}