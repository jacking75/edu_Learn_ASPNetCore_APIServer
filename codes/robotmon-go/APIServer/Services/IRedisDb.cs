using ApiServer.Model;

namespace ApiServer.Services
{
    public interface IRedisDb
    {
        public void Init(string address);
        public Task<bool> SetUserInfo(string key, RedisLoginData redisLoginData);
        public Task<RedisLoginData> GetUserInfo(string key);
        public Task<bool> DelUserInfo(string key);
        public Task<RedisLoginData> GetUserAuthToken(string key);
        public Task<bool> SetNxAsync(string key);
        public Task<bool> DelNxAsync(string key);
        public Task<bool> ZSetAddAsync(string member, Int32 score);
        public Task<bool> UpdateRankAsync(string member, Int32 score);
        public Task<string[]?> GetRangeRankAsync(Int32 start, Int32 range);
        public Task<Int32> GetRankSizeAsync();
    }
}
