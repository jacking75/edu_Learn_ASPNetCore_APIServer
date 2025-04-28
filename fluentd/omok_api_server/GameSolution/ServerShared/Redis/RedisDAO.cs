namespace ServerShared.Redis;

public class RedisUserMatch
{
	public OmokStone UserStone { get; set; } = OmokStone.None;
	public string GameGuid { get; set; } = "";
}

public class RedisUserGame
{
    public OmokStone UserStone { get; set; } = OmokStone.None;
    public string GameGuid { get; set; } = "";

}

