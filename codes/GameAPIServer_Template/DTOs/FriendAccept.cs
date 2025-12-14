using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTOs;

public class FriendAcceptRequest
{
    [Required]
    public int FriendUid { get; set; }
}

public class FriendAcceptResponse : ErrorCode
{
}
