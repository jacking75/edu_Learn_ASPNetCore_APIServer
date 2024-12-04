using APIServer.Services.MasterData;
using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.ModelReqRes;

public class ReceiveInAppProductRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.InAppPID))]
    [Range(1, Int32.MaxValue, ErrorMessage = "PID IS NOT VALID")]
    public Int32 PID { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "RECEIPT CANNOT BE EMPTY")]
    [StringLength(100, ErrorMessage = "RECEIPT IS TOO LONG")]
    public string Receipt { get; set; }
}

public class ReceiveInAppProductResponse : ResponseBase
{
}