using Microsoft.AspNetCore.Mvc;

namespace HiveAPIServer.DTO
{
    public class HeaderDTO
    {
        [FromHeader]
        public int Uid { get; set; }
    }
}
