using Microsoft.AspNetCore.Mvc;

namespace GameAPIServer.DTOs;

public class Header
{
    [FromHeader]
    public int Uid { get; set; }
}
