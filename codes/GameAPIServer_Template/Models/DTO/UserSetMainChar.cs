using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Models.DTO;

public class UserSetMainCharRequest
{
    [Required]
    public int CharKey { get; set; }
}

public class UserSetMainCharResponse : ErrorCode
{
}
