using System;
using System.Threading.Tasks;
using CloudStructures;
using CloudStructures.Structures;


namespace APIServer.Services;

public class RedisDb
{
    private RedisConnection _redisConn;

    public void Init(string address)
    {
        RedisConfig config = new("default", address);
        _redisConn = new RedisConnection(config);

        Console.WriteLine($"userDbAddress:{address}");
    }


    public async Task<bool> CreateAccountAsync(string userID, string userPW)
    {
        string key = MemoryDbKeyMaker.MakeUserAccountKey(userID);

        try
        {
            RedisString<string> redis = new(_redisConn, key, null);
            if (await redis.SetAsync(userPW, null) == false)
            {
                Console.WriteLine($"[Redis CreateAccountAsync] UserID:{userID}, PW:{userPW},ErrorMessage: Save Account Info");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Redis CreateAccountAsync] UserID:{userID}, Exception: {ex.Message}");
            return false;
        }

        return true;
    }


}