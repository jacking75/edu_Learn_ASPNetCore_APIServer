using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTO
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
