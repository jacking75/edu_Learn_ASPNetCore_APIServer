using MatchAPIServer.Models;
using CloudStructures.Structures;
using System;
using System.Collections.Generic;

namespace MatchAPIServer.DTO.Ranking
{
    public class RankingResponse : ErrorCodeDTO
    {
        public List<RankData> RankingData { get; set; }
    }

    public class RankData
    {
        public Int64 rank { get; set; }
        public int uid { get; set; }
        public int score { get; set; }
    }
}
