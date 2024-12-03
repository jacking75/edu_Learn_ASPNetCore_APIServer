
public class SharedKeyGenerator
{
    const string matchKey = "MATCH_";
    const string gameKey = "GAME_";
	const string lockKey = "LOCK:";
	const string userGameKey = "UGAME_";

	public static string MakeMatchDataKey(string id)
    {
        return matchKey + id;
    }

    public static string MakeGameDataKey(string id)
    {
        return gameKey + id;
    }

	public static string MakeUserGameKey(string id)
	{
		return userGameKey + id;
	}

	public static string MakeLockKey(string key)
	{
		return lockKey + key;
	}
}
