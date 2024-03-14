using ServerCommon;

namespace ApiServer.Model
{
    public class EvolveResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int64 EvolveMonsterID { get; set; }
    }
}