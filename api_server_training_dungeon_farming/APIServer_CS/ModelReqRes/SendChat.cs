using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class SendChatRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "CHAT MESSAGE CANNOT BE EMPTY")]
    public string Message { get; set; }
}

public class SendChatResponse : ResponseBase
{
}