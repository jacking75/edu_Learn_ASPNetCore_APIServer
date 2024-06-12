using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableMonsterInfo
    {
        public Int64 MID;   // MonsterID
        [StringLength(100)]
        public string MonsterName;
        [StringLength(100)]
        public string Type;
        public Int32 Level;
        public Int32 HP;
        public Int32 Att;
        public Int32 Def;
        public Int32 StarCount;
        public Int32 UpgradeCount;
    }
}