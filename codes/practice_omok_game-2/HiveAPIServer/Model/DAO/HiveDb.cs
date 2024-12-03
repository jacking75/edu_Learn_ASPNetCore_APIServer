using System;

namespace HiveAPIServer.Model.DAO;

//HiveDb의 객체는 객체 이름 앞에 Hdb를 붙인다.

public class HdbAccountInfo
{
    public Int64 player_id { get; set; }
    public string email { get; set; }
    public string pw { get; set; }
    public string salt { get; set; }
    
    public string create_dt { get; set; }
}

public class HdbTokenInfo
{
	public Int64 player_id { get; set; }

	public string token { get; set; }
	public string expire_dt { get; set; }
}