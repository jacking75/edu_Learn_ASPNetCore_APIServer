using System;

namespace APIServer.Model.DAO;

//HiveDB의 객체는 객체 이름 앞에 Hdb를 붙인다.

public class HdbAccountInfo
{
    public Int64 player_id { get; set; }
    public string email { get; set; }
    public string pw { get; set; }
    public string salt_value { get; set; }
    public string recent_login_dt { get; set; }
    public string create_dt { get; set; }
}