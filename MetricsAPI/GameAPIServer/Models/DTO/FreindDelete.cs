using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Models.DTO;

public class FriendDeleteRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendDeleteResponse : ErrorCode
{
}


