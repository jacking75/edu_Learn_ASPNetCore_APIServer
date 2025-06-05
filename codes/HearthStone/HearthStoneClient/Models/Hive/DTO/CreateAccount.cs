using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HearthStoneWeb.Models.Hive;


public class CreateAccountRequest
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

    [Required]
    [MinLength(1, ErrorMessage = "NICKNAME CANNOT BE EMPTY")]
    [StringLength(20, ErrorMessage = "NICKNAME IS TOO LONG")]
    public string NickName { get; set; }
}

public class CreateAccountResponse 
{
    [Required]public ErrorCode Result { get; set; } = ErrorCode.None;
}
