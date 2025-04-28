namespace GameServer.Models;
public class RedisUserSession
{
	public Int64 Uid { get; set; } = 0;
	public string Token { get; set; } = "";
}

public class RedisUserLock
{
	
}