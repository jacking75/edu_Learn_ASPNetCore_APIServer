using APIServer.Models;
using CloudStructures.Structures;
using System;
using System.Collections.Generic;

namespace GameAPIServer.DTO
{
    public class RankingResponse : ErrorCodeDTO
    {
        public List<RankData> RankingData { get; set; }
    }

    public class RankData
    {
        public long rank { get; set; }
        public int uid { get; set; }
        public int score { get; set; }
    }
}
