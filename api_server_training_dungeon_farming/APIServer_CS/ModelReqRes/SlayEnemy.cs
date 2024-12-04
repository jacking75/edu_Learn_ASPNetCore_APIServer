using System;
using System.ComponentModel.DataAnnotations;

using APIServer.Services.MasterData;

namespace APIServer.ModelReqRes;

public class SlayEnemyRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH_TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.StageCode))]
    public Int32 StageCode { get; set; }

    [Required]
    [EnumDataType(typeof(MasterDataCode.StageEnemyCode))]
    public Int32 EnemyCode { get; set; }
}

public class SlayEnemyResponse : ResponseBase
{
}