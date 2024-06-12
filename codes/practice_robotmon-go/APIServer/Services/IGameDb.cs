using System;
using System.Threading.Tasks;
using ApiServer.Model;
using ServerCommon;
using ApiServer.Model.Data;

namespace ApiServer.Services
{
    public interface IGameDb : IDisposable
    {
        // DB 열기.
        public void Open();
        
        // DB 닫기.
        public void Close();

        // 유저 정보 가져오기
        public Task<Tuple<ErrorCode, UserGameInfo>> GetUserGameInfoAsync(string id);
        public Task<ErrorCode> UpdateUserStarCountAsync(string ID, Int32 starCount);
        // 유저 정보 설정하기
        public Task<Tuple<ErrorCode, Int64>> InitUserGameInfoAsync(string id, UserGameInfo table);
        public Task<ErrorCode> RollbackInitUserGameInfoAsync(Int64 gamedataId);
        public Task<Tuple<ErrorCode, FieldMonsterResponse>> GetMonsterInfoAsync(Int64 monsterUID);
        public Task<Tuple<ErrorCode, Int32>> SetCatchAsync(string id, Int64 monsterID, DateTime catchTime, Int32 combatPoint);
        public Task<ErrorCode> RollbackSetCatchAsync(Int64 catchID);
        // 출석체크 설정하기
        public Task<ErrorCode> InitDailyCheckAsync(string ID);
        public Task<ErrorCode> RollbackInitDailyCheckAsync(string dailyID);
        public Task<Tuple<ErrorCode, DateTime>> TryDailyCheckAsync(string ID);
        public Task<ErrorCode> RollbackDailyCheckAsync(string id, DateTime prevDate);
        public Task<Tuple<ErrorCode, Int32, List<Tuple<Int64,Int32>>?>> CheckMailAsync(string ID, Int32 pageIndex, Int32 pageSize = 10);
        public Task<Tuple<ErrorCode, Int64>> SendMailAsync(string ID, Int32 starCount);
        public Task<ErrorCode> RollbackSendMailAsync(Int64 MailID);
        public Task<Tuple<ErrorCode, Int32, DateTime>> RecvMailAsync(string ID, Int64 MailID);
        public Task<ErrorCode> RollbackRecvMailAsync(string id, Int32 startCount, DateTime date);
        public Task<Tuple<ErrorCode, List<Tuple<Int64, Int64, DateTime, Int32>>>> GetCatchListAsync(string id);
        public Task<Tuple<ErrorCode, Int64, Int64, DateTime, Int32>> DelCatchAsync(Int64 catchID);
        public Task<ErrorCode> RollbackDelCatchAsync(string id, Int64 monsterID, DateTime catchDate);
        public Task<ErrorCode> UpdateUserExpAsync(string id, Int32 gainExp);
        public Task<ErrorCode> UpdateUpgradeCostAsync(string id, Int32 updateCost);
        public Task<ErrorCode> EvolveCatchMonsterAsync(Int64 catchId, Int64 evolveMonsterId);
        public Task<ErrorCode> UpdateCatchCombatPointAsync(Int64 catchId, Int32 combatPoint);
    }
}