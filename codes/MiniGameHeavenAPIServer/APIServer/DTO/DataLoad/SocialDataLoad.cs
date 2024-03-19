using APIServer.Models.GameDB;
using System.Collections.Generic;

namespace APIServer.DTO.DataLoad
{
    public class SocialDataLoadResponse : ErrorCodeDTO
    {
        public DataLoadSocialInfo SocialData { get; set; }
    }

    public class DataLoadSocialInfo
    {
        public IEnumerable<GdbFriendInfo> FriendList { get; set; }
        public List<UserMailInfo> MailList { get; set; }

    }

    public class UserMailInfo
    {
        public GdbMailboxInfo MailInfo { get; set; }
        public IEnumerable<GdbMailboxRewardInfo> MailItems { get; set; }
    }
}
