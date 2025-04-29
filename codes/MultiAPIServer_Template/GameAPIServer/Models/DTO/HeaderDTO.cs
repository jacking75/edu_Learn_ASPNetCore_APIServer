using Microsoft.AspNetCore.Mvc;

namespace MatchAPIServer.DTO
{
    public class HeaderDTO
    {
        [FromHeader]
        public int Uid { get; set; }
    }
}
