using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTO.Friend;

public class SendFriendReqRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class SendFriendReqResponse : ErrorCodeDTO
{
}


