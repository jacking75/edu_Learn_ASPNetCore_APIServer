using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class ChangeChannelRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "CHAT LOBBY NUMBER IS NOT VAILD")]
    public Int32 ChannelNumber { get; set; }
}

public class ChangeChannelResponse : ResponseBase
{
}