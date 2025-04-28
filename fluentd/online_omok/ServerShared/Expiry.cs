namespace ServerShared;

public static class Expiry
{
	public static readonly TimeSpan UserSessionExpiry = TimeSpan.FromHours(2);
	public static readonly TimeSpan UserGameExpiry = TimeSpan.FromMinutes(2);
	public static readonly TimeSpan UserRequestLockExpiry = TimeSpan.FromSeconds(3);
	public static readonly TimeSpan MailExpiry = TimeSpan.FromDays(7);
	public static readonly TimeSpan MatchExpiry = TimeSpan.FromMinutes(2);
	public static readonly TimeSpan OmokGameExpiry = TimeSpan.FromMinutes(2);
	public static readonly TimeSpan OmokTurnExpiry = TimeSpan.FromSeconds(30);
}
