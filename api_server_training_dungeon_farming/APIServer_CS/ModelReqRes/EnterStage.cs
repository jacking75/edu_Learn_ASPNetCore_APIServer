using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using APIServer.ModelDB;
using APIServer.Services.MasterData;

namespace APIServer.ModelReqRes;

public class EnterStageRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.StageCode))]
    public Int32 StageCode { get; set; }
}

public class EnterStageResponse : ResponseBase
{
    public List<StageFarmingItem> AppearanceItem { get; set; } = new();

    public List<StageEnemy> AppearanceEnemy { get; set; } = new();
}