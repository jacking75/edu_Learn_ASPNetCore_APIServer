using System.Collections.Generic;
using GameAPIServer.Models;


namespace GameAPIServer.DTOs;

public class SocialDataLoadResponse : ErrorCode
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
