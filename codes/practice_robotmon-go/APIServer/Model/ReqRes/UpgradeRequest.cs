using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class UpgradeRequest
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        [StringLength(200)]
        public string AuthToken { get; set; } = "";
        public Int64 CatchID { get; set; }
        public Int64 MonsterID { get; set; }
        public Int32 UpgradeSize { get; set; }  // 얼만큼 업그레이드할 것인가?
    }
}