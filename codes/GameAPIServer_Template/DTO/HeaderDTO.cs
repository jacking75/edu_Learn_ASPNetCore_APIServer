using Microsoft.AspNetCore.Mvc;

namespace APIServer.DTO
{
    public class HeaderDTO
    {
        [FromHeader]
        public int Uid { get; set; }
    }
}
