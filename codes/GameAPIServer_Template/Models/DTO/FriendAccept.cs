using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Models.DTO;

public class FriendAcceptRequest
{
    [Required]
    public int FriendUid { get; set; }
}

public class FriendAcceptResponse : ErrorCode
{
}
