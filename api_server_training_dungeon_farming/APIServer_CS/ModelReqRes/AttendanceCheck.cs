using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class AttendanceCheckRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH_TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }
}

public class AttendanceCheckResponse : ResponseBase
{
}