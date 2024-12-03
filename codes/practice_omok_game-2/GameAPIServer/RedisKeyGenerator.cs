namespace GameAPIServer;

public class RedisKeyGenerator
{
	const string loginUid = "Uid_";
	const string userLockKey = "ULOCK_";

	public static string MakeUidKey(string id)
	{
		return loginUid + id;
	}

	public static string MakeUserLockKey(string id)
	{
		return userLockKey + id;
	}

}