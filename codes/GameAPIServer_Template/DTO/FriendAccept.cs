using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTO
{
    public class FriendAcceptRequest
    {
        [Required]
        public int FriendUid { get; set; }
    }

    public class FriendAcceptResponse : ErrorCodeDTO
    {
    }
}
