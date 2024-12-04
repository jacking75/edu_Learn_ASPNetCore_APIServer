namespace APIServer.Services;

public class MemoryDbKeyMaker
{
    private const string loginUID = "UID_";
    private const string userLockKey = "ULock_";

    public static string MakeUserAccountKey(string id)
    {
        return "UA_" + id;
    }

    public static string MakeUIDKey(string id)
    {
        return loginUID + id;
    }

    public static string MakeUserLockKey(string id)
    {
        return userLockKey + id;
    }
}