using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Friend
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
