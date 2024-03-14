using ServerCommon;

namespace ApiServer.Model
{
    public class CatchResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int64 CatchID { get; set; }
        public Int64 MonsterID { get; set; }
        public Int64 StarCount { get; set; }
        public Int64 UpgradeCandy { get; set; }
        public DateTime Date { get; set; }
        public Int32 CombatPoint { get; set; }
    }
}