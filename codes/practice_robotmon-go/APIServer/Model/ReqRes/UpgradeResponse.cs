using ServerCommon;

namespace ApiServer.Model
{
    public class UpgradeResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}