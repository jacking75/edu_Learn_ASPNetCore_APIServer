using ApiServer.Data;
using ApiServer.Model;

namespace ApiServer.Services
{
    public interface IDataStorage
    {
        public void Load(string dbConnString);
        public Monster GetMonsterInfo(Int64 monsterID);
        public DailyInfo GetDailyInfo(Int32 dailyIdx);
        public MonsterUpgrade GetMonsterUpgrade(Int64 monsterIdx);
        public MonsterEvolve GetMonsterEvolve(Int64 monsterIdx);
        public LevelUpInfo GetLevelUpMaxExp(Int32 level);
    }
}
