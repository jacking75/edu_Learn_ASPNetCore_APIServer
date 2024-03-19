using APIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class MiniGameInfoRequest
{
    [Required]
    public int GameKey { get; set; }
}


public class MiniGameInfoResponse : ErrorCodeDTO
{
    public GdbMiniGameInfo MiniGameInfo { get; set; }
}
