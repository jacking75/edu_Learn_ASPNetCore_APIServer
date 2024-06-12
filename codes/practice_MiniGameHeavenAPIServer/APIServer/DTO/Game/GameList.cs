using APIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class MiniGameListResponse : ErrorCodeDTO
{
    public IEnumerable<GdbMiniGameInfo> MiniGameList { get; set; }
}
