using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class GetCompleteStageListRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH_TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }
}

public class GetCompleteStageListResponse : ResponseBase
{
    public List<Int32> CompletedStage { get; set; } = new();
}