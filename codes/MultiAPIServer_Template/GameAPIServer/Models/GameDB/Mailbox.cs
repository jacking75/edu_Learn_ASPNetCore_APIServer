using System;

namespace MatchAPIServer.Models.GameDB
{
    public class GdbMailboxInfo
    {
        public int mail_seq { get; set; }
        public int uid { get; set; }
        public string mail_title { get; set; }
        public DateTime create_dt { get; set; }
        public DateTime expire_dt { get; set; }
        public DateTime receive_dt { get; set; }
        public bool receive_yn { get; set; }
    }

    public class GdbMailboxRewardInfo : RewardData
    {
        public int mail_seq { get; set; }
    }
}
