using System.ComponentModel.DataAnnotations;

namespace MatchAPIServer.DTO.Friend
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
