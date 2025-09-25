using Microsoft.AspNetCore.Mvc;

namespace GameAPIServer.Models.DTO;

public class Header
{
    [FromHeader]
    public int Uid { get; set; }
}
