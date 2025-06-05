using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace HearthStoneWeb.Models.Game;



public class ErrorCodeDTO
{
    public ErrorCode Result { get; set; } = ErrorCode.MAX;
}
public class HeaderDTO
{
    public Int64 AccountUid { get; set; }

    public string Token { get; set; }
}
