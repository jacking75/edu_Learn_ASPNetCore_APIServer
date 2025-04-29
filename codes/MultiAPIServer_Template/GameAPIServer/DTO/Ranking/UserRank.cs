using System;

namespace MatchAPIServer.DTO.Ranking
{
    public class UserRankResponse : ErrorCodeDTO
    {
        public Int64 Rank { get; set; }
    }
}
