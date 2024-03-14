using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableDailyCheck
    {
        [StringLength(45)] 
        public string ID { get; set; }
        public Int32 RewardCount { get; set; }  // 보상 받은 횟수
        public DateTime RewardDate { get; set; }   // 마지막 보상 받은 날짜
    }
}