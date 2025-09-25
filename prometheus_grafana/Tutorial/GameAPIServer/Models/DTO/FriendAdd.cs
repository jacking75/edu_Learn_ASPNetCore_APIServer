using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Models.DTO;

public class SendFriendReqRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class SendFriendReqResponse : ErrorCode
{
}


