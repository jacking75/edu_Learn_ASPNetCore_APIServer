using System.ComponentModel.DataAnnotations;

namespace MatchAPIServer.DTO.User
{
    public class UserSetMainCharRequest
    {
        [Required]
        public int CharKey { get; set; }
    }

    public class UserSetMainCharResponse : ErrorCodeDTO
    {
    }
}
