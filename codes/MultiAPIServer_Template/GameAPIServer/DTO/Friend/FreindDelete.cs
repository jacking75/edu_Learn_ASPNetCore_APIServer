using System.ComponentModel.DataAnnotations;

namespace MatchAPIServer.DTO.Friend;

public class FriendDeleteRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendDeleteResponse : ErrorCodeDTO
{
}


