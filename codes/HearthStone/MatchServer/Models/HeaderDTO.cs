using System.ComponentModel.DataAnnotations;

namespace MatchServer.Models;

public class HeaderDTO
{
    [Required]
    public Int64 AccountUid { get; set; }
    [Required]
    public string Token { get; set; }
}
