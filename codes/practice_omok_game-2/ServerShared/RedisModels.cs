/*
 * Game
 */
using System.ComponentModel.DataAnnotations;

public class RedisMatchData
{
	public string GameGuid { get; set; } = "";
	public Int64 MatchedUserID { get; set; } = 0;
}

public class RedisGameLock
{
	public Int64 CurrentPlayerUid { get; set; } = 0;
}

public class RedisUserCurrentGame
{
	public Int64 Uid { get; set; } = 0;

	public string GameGuid { get; set; } = "";

}


public class MatchRequest
{
	[Required]
    public Int64 Uid { get; set; } = 0;
}