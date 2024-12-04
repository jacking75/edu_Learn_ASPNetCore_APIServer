using System;
using System.ComponentModel.DataAnnotations;

using APIServer.Services.MasterData;

namespace APIServer.ModelReqRes;

public class FarmingItemInDungeonRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.StageCode))]
    public Int32 StageCode { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.ItemCode))]
    public Int32 ItemCode { get; set; }
}

public class FarmingItemInDungeonResponse : ResponseBase
{
}