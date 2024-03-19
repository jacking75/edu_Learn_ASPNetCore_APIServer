
using APIServer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class MiniGameSaveRequest
{
    [Required]
    public int GameKey { get; set; }
    [Required]
    public int Score { get; set; }
    public List<UsedFoodData> Foods { get; set; }
}

public class UsedFoodData
{
    public int FoodKey { get; set; }
    public int FoodQty { get; set; }
}


public class MiniGameSaveResponse : ErrorCodeDTO
{
}
