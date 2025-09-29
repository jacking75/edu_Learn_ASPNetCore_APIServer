using System.Collections.Generic;

namespace GameAPIServer.Models.DTO;

public class SocialDataLoadResponse : ErrorCode
{
    public DataLoadSocialInfo SocialData { get; set; }
}

public class DataLoadSocialInfo
{
    public IEnumerable<DAO.GdbFriendInfo> FriendList { get; set; }
    public List<UserMailInfo> MailList { get; set; }

}

public class UserMailInfo
{
    public DAO.GdbMailboxInfo MailInfo { get; set; }
    public IEnumerable<DAO.GdbMailboxRewardInfo> MailItems { get; set; }
}
