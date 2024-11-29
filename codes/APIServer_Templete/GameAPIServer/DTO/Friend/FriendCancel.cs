using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Friend;

public class FriendCancelRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendCancelResponse : ErrorCodeDTO
{
}


