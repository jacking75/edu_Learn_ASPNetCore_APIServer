using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentDLL.DAO;

public class Account
{
    public Int64 player_id { get; set; }
    public string username { get; set; } = String.Empty;
    public string pw { get; set; } = String.Empty;
    public string salt { get; set; } = String.Empty;
    public DateTime create_dt { get; set; }
    public DateTime last_login_dt { get; set; }
}