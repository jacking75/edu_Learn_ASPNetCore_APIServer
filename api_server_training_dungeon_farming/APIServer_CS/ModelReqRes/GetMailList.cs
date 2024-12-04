using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using APIServer.ModelDB;

namespace APIServer.ModelReqRes;

public class GetMailListRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH_TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [Range(1, Int64.MaxValue, ErrorMessage = "OPEN_PAGE IS NOT VAILD")]
    public Int32 OpenPage { get; set; }
}

public class GetMailListResponse : ResponseBase
{
    [Required]
    [Range(0, Int64.MaxValue, ErrorMessage = "PAGE_COUNT IS NOT VALID")]
    public Int32 PageCount { get; set; }

    [Required]
    public List<UserMail> MailList { get; set; } = new();
}