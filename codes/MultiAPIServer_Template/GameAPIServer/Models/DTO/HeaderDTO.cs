using Microsoft.AspNetCore.Mvc;

namespace GameAPIServer.DTO
{
    public class HeaderDTO
    {
        [FromHeader]
        public int Uid { get; set; }
    }
}
