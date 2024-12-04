using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class ReceiveMailRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [Range(1, Int32.MaxValue, ErrorMessage = "MAIL_ID IS NOT VALID")]
    public Int64 MailId { get; set; }
}

public class ReceiveMailResponse : ResponseBase
{
    public List<Int64> ReceivedItemList { get; set; } = new();

    [Required]
    [Range(0, Int64.MaxValue, ErrorMessage = "RECEIVED_ITEM_COUNT IS NOT VALID")]
    public Int64 ReceivedItemCount { get; set; }
}