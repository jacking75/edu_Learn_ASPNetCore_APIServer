namespace ServerShared.Redis;

public class RedisKeyGenerator
{
    const string userSessionKey = "US_";
    const string userLockKey = "UL_";
    const string userGameKey = "UG_";

    const string matchKey = "M_";
    const string omokKey = "O_";

    public static string MakeUserSessionKey(string id)
    {
        return userSessionKey + id;
    }

    public static string MakeUserLockKey(string id)
    {
        return userLockKey + id;
    }
    public static string MakeUserGameKey(string id)
    {
        return userGameKey + id;
    }

    public static string MakeMatchKey(string id)
    {
        return matchKey + id;
    }

    public static string MakeOmokKey(string id)
    {
        return omokKey + id;
    }

}
