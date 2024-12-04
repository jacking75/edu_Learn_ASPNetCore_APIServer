using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using APIServer.ModelDB;

namespace APIServer.ModelReqRes;

public class LoginRequest : RequestBase
{
    [Required]
    [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
    [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Range(1, Int32.MaxValue, ErrorMessage = "APP_VERSION IS NOT VAILD")]
    public Int32 AppVersion { get; set; }

    [Required]
    [Range(1, Int32.MaxValue, ErrorMessage = "MASTER_DATA_VERSION IS NOT VAILD")]
    public Int32 MasterDataVersion { get; set; }
}

public class LoginResponse : ResponseBase
{
    [Required]
    [MinLength(1, ErrorMessage = "AUTH TOKEN CANNOT BE EMPTY")]
    public string AuthToken { get; set; } = string.Empty;

    [Required]
    [StringLength(512, ErrorMessage = "NOTICE IS TOO LONG")]
    public string GameNotice { get; set; } = string.Empty;

    public UserGameInfoSet GameInfo { get; set; } = new();
}

public class UserGameInfoSet
{
    public UserPlayData PlayData { get; set; }

    public List<UserInventoryItem> InventoryItem { get; set; } = new();
}