using ServerCommon;

namespace ApiServer.Model
{
    public class RemoveCatchResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int64 UpgradeCandy { get; set; }
    }
}