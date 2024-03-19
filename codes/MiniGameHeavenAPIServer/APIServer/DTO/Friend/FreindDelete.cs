using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Friend;

public class FriendDeleteRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendDeleteResponse : ErrorCodeDTO
{
}


