using ServerCommon;

namespace ApiServer.Services
{
    public interface IRankingManager
    {
        public void Init(string dbConnString, IRedisDb redisDb);

        public Task<ErrorCode> UpdateStarCount(string id, Int32 starCount, IGameDb gameDb, IRedisDb redisDb);
        public Task<ErrorCode> RollbackUpdateStarCount(string id, Int32 minusStarCount, IGameDb gameDb, IRedisDb redisDb);

        public Task<Tuple<ErrorCode, Int64, string[]>> CheckRankingInfo(Int32 pageIndex, IRedisDb redisDb, Int32 range = 10);

    }
}
