namespace GameAPIServer.Models.RedisDb;
/*
 * Authentication
 */
public class RedisUserAuth
{
	public Int64 Uid { get; set; } = 0;
	public string Token { get; set; } = "";
}

public class RedisUserLock
{

}
