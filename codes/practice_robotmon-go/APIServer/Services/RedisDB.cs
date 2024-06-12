using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using ApiServer.Model;
using CloudStructures;
using CloudStructures.Structures;
using StackExchange.Redis;

namespace ApiServer.Services
{
    public class RedisDb : IRedisDb
    {
        private RedisConnection s_connection;

        public void Init(string address)
        {
            var config = new RedisConfig("redisDb", address);
            s_connection = new RedisConnection(config);
        }

        public async Task<bool> SetUserInfo(string key, RedisLoginData redisLoginData)
        {
            try
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, TimeSpan.FromDays(1));
                if (await redis.SetAsync(redisLoginData, TimeSpan.FromDays(1)) == false)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<RedisLoginData> GetUserInfo(string key)
        {
            try
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, null);
                var redisResult = await redis.GetAsync();
                if (!redisResult.HasValue)
                {
                    return null;
                }

                return redisResult.Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DelUserInfo(string key)
        {
            try
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, null);
                var redisResult = await redis.DeleteAsync();
                return redisResult;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RedisLoginData> GetUserAuthToken(string key)
        {
            try
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, null);
                var redisResult = await redis.GetAsync();
                if (!redisResult.HasValue)
                {
                    return null;
                }
            
                var redisLogin = redisResult.Value;
                return redisLogin;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> SetNxAsync(string key)
        {
            try
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, TimeSpan.FromMinutes(1));
                if (await redis.SetAsync(new RedisLoginData()
                    {
                        ID = key,
                        AuthToken = ""
                    }, TimeSpan.FromMinutes(1), When.NotExists) == false)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DelNxAsync(string key)
        {
            try
            {
                var redis = new RedisString<RedisLoginData>(s_connection, key, TimeSpan.FromMinutes(1));
                var redisResult = await redis.DeleteAsync();
                return redisResult;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ZSetAddAsync(string member, Int32 score)
        {
            try
            {
                // 기존에 키가 없다면 자동으로 생성된다.
                var redis = new RedisSortedSet<string>(s_connection, "Rank", null);
                var success = await redis.AddAsync(member, score, null,When.Always);
                return success;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateRankAsync(string member, Int32 score)
        {
            try
            {
                // 기존에 키가 없다면 자동으로 생성된다.
                var redis = new RedisSortedSet<string>(s_connection, "Rank", null);
                var diff = await redis.IncrementAsync(member, score, null);
                // 변화된 마지막 값...이 diff
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<string[]?> GetRangeRankAsync(Int32 start, Int32 range)
        {
            try
            {
                var redis = new RedisSortedSet<string>(s_connection, "Rank", null);
                var redisList = await redis.RangeByRankAsync(start, range, Order.Descending);
                return redisList;
            }
            catch
            {
                return null;
            }
        }
        
        public async Task<Int32> GetRankSizeAsync()
        {
            try
            {
                var redis = new RedisSortedSet<string>(s_connection, "Rank", null);
                var rankSize = await redis.LengthAsync();
                return (Int32)rankSize;
            }
            catch
            {
                return -1;
            }
        }
    }
}