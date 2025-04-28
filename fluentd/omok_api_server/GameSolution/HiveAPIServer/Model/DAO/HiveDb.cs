using System;
namespace HiveAPIServer.Model.DAO;

public class Account
{
    public Int64 player_id { get; set; }
    public string email { get; set; }
    public string pw { get; set; }
    public string salt { get; set; }
    public string create_dt { get; set; }
}

public class Token
{
	public Int64 player_id { get; set; }
	public string token { get; set; }
	public string expire_dt { get; set; }
}