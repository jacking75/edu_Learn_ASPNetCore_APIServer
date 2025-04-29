using GameAPIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTO.Friend;

public class FriendListResponse : ErrorCodeDTO
{
    public IEnumerable<GdbFriendInfo> FriendList { get; set; }
}
