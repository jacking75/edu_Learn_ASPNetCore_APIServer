using System.Collections.Generic;


namespace GameAPIServer.Models.DTO;

public class FriendListResponse : ErrorCode
{
    public IEnumerable<DAO.GdbFriendInfo> FriendList { get; set; }
}
