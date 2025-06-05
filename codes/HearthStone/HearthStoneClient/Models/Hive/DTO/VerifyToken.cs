using System.ComponentModel.DataAnnotations;

namespace HearthStoneWeb.Models.Hive;



public class VerifyTokenRequest
{
    [Required] public Int64 AccountUId { get; set; }
    [Required] public string HiveToken{ get; set; }
}
public class VerifyTokenResponse
{
   [Required] public ErrorCode Result { get; set; } = ErrorCode.None;
}
