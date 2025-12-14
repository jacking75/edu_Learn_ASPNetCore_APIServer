using GameAPIServer.Models;
using System.Collections.Generic;


namespace GameAPIServer.DTOs;

public class FriendListResponse : ErrorCode
{
    public IEnumerable<GdbFriendInfo> FriendList { get; set; }
}
