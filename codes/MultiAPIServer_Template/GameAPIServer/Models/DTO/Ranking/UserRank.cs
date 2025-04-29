using System;

namespace GameAPIServer.DTO.Ranking
{
    public class UserRankResponse : ErrorCodeDTO
    {
        public Int64 Rank { get; set; }
    }
}
