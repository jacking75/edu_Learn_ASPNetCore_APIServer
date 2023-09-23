using System;

namespace APIServer.Models.DTO;

public class ReqCommonDTO
{
    public Int32 seq { get; set; }

    public UInt64 userUid { get; set; }

    public string sessionKey { get; set; } = string.Empty;

    public string apiName { get; set; } = string.Empty;

    public Int32 apiVer { get; set; }

    public Int64 timestamp { get; set; }
}

public class ResCommonDTO
{
    public Int32 seq { get; set; }

    public UInt64 userUid { get; set; }

    public string sessionKey { get; set; } = string.Empty;

    public string apiName { get; set; } = string.Empty;

    public Int32 apiVer { get; set; }

    public Int64 timestamp { get; set; }

    public APIResult result { get; set; } = new();

    public object returnData { get; set; }
}

public struct APIResult
{
    public Int32 code { get; set; }

    public string msg { get; set; }
}