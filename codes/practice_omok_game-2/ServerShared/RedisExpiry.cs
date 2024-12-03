

public static class RedisExpiryTimes
{
	public static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(2);

	/* Auth */
	public static readonly TimeSpan AuthTokenExpiryLong = TimeSpan.FromDays(7);
	public static readonly TimeSpan AuthTokenExpiryShort = TimeSpan.FromHours(2);
	public static readonly TimeSpan RequestLockExpiry = TimeSpan.FromSeconds(3);

    /* User */
    public static readonly TimeSpan UserDataExpiry = TimeSpan.FromMinutes(1);

    /* Match */
    public static readonly TimeSpan MatchDataExpiry = TimeSpan.FromSeconds(30);

	/* Game */
	public static readonly TimeSpan GameDataExpiry = TimeSpan.FromMinutes(5);
}