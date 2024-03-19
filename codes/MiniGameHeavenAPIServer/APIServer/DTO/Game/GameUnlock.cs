using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class MiniGameUnlockRequest
{
    [Required]
    public int GameKey { get; set; }
}


public class MiniGameUnlockResponse : ErrorCodeDTO
{
}
