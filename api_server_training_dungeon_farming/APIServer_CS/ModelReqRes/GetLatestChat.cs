using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using APIServer.ModelDB;

namespace APIServer.ModelReqRes;

public class GetLatestChatRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    public string MessageId { get; set; } = string.Empty;
}

public class GetLatestChatResponse : ResponseBase
{
    public List<ChatInfo> MessageList { get; set; } = new();
}