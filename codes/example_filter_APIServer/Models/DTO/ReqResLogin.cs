using System;

namespace APIServer.Models.DTO;

public class ReqLoginDTO
{
    public UInt64 hiveVid { get; set; }

    public UInt64 hiveUid { get; set; }

    public UInt64 hiveDid { get; set; }

    public string hiveSessionKey { get; set; } = string.Empty;

    public string appId { get; set; } = string.Empty;

    public string appVer { get; set; } = string.Empty;

    public string country { get; set; } = string.Empty;

    public string language { get; set; } = string.Empty;

    public string osVer { get; set; } = string.Empty;

    public string deviceName { get; set; } = string.Empty;

    public string pushToken { get; set; } = string.Empty;

    public string bundleType { get; set; } = string.Empty;
}

public class ResLoginDTO
{
    public UInt64 userUid { get; set; }

    public string sessionKey { get; set; } = string.Empty;

    public Int32 sessionExpireSecond { get; set; }
}