using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableCatch
    {
        public Int64 CatchID { get; set; }
        [StringLength(45)]
        public string UserID { get; set; }
        public Int64 MonsterID { get; set; }
        public DateTime CatchTime { get; set; }
        public Int32 CombatPoint { get; set; }
    }
}