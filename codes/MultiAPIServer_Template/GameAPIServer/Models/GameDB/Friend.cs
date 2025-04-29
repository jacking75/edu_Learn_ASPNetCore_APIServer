using System;

namespace MatchAPIServer.Models.GameDB
{

    public class GdbFriendInfo
    {
        public int uid { get; set; }

        public string friend_uid { get; set; }
        public bool friend_yn { get; set; }
        public DateTime create_dt { get; set; }
    }

}
