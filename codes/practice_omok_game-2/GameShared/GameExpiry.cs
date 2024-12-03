namespace GameShared;

public static class GameExpiry
{
	/* Match */
	public static readonly TimeSpan MatchWaitExpiry = TimeSpan.FromMinutes(2);

	/* Game */
	public static readonly TimeSpan GameWaitExpiry = TimeSpan.FromMinutes(2);
	public static readonly TimeSpan GameTurnExpiry = TimeSpan.FromSeconds(30);
}
