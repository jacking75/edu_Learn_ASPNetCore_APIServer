using Microsoft.AspNetCore.Mvc;
using System.Data;
using GameServer.Models;
using GameServer.Repository.Interface;

namespace GameServer.Models.DTO;

public class ErrorCodeDTO
{
    public ErrorCode Result { get; set; } = ErrorCode.MAX;
}
public class HeaderDTO
{
    [FromHeader]
    public Int64 AccountUid { get; set; }

    [FromHeader]
    public string Token { get; set; }
}
