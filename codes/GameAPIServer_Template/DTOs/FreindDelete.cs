using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTOs;

public class FriendDeleteRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendDeleteResponse : ErrorCode
{
}


