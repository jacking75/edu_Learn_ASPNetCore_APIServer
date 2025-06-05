using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models.DTO;

public class VerifyTokenRequest
{
    [Required] public Int64 AccountUId { get; set; }
    [Required] public string HiveToken{ get; set; }
}
public class VerifyTokenResponse
{
   [Required] public ErrorCode Result { get; set; } = ErrorCode.None;
}