using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes
{
    public class RequestBase
    {
        [Required]
        [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
        [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "EMAIL IS NOT VALID")]
        public string Email { get; set; }
    }

    public class ResponseBase
    {
        [Required]
        [EnumDataType(typeof(ErrorCode))]
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}
