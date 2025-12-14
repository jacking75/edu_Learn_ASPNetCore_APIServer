using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTOs;

public class SendFriendReqRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class SendFriendReqResponse : ErrorCode
{
}


