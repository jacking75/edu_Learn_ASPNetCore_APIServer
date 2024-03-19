using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.User
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
