using System.ComponentModel.DataAnnotations;

namespace HearthStoneWeb.Models.Game;



public class LoginRequest
{
    [Required] public Int64 AccountUId { get; set; }
    [Required] public string HiveToken { get; set; }
}
public class LoginResponse : ErrorCodeDTO
{
    [Required] public string Token { get; set; } = "";
    [Required] public Int64 AccountUId { get; set; } = 0;
}
