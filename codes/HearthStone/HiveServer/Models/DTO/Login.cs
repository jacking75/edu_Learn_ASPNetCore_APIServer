using System.ComponentModel.DataAnnotations;

namespace HiveServer.Models.DTO;

public class LoginRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
    [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string EmailID { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
    [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
public class LoginResponse 
{
    [Required]public ErrorCode Result { get; set; } = ErrorCode.None;
    [Required]public Int64 AccountUId { get; set; }
    [Required]public string HiveToken{ get; set; }
}