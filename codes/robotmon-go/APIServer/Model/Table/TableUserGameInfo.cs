using System;
using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class TableUserGameInfo
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        public Int32 UserLevel { get; set; }
        public Int32 UserExp { get; set; }
        public Int32 StarPoint { get; set; } // 별의 모래
        public Int32 UpgradeCandy { get; set; }
    }
}