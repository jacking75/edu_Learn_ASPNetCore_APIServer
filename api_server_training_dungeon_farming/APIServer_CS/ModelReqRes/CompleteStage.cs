using System;
using System.ComponentModel.DataAnnotations;

using APIServer.Services.MasterData;

namespace APIServer.ModelReqRes;

public class CompleteStageRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH_TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.StageCode))]
    public Int32 StageCode { get; set; }
}

public class CompleteStageResponse : ResponseBase
{
}