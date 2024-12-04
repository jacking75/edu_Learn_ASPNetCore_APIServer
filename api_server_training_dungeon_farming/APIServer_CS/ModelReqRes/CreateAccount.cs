using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class CreateAccountRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
    [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class CreateAccountResponse : ResponseBase
{
}