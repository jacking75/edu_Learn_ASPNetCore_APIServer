using System;

namespace GameAPIServer.DTO
{
    public class UserRankResponse : ErrorCodeDTO
    {
        public long Rank { get; set; }
    }
}
