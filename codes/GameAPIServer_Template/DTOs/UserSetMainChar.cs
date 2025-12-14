using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTOs;

public class UserSetMainCharRequest
{
    [Required]
    public int CharKey { get; set; }
}

public class UserSetMainCharResponse : ErrorCode
{
}
