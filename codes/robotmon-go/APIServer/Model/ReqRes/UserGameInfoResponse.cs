using System;
using ServerCommon;

namespace ApiServer.Model
{
    public class UserGameInfoResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int64 UserLevel { get; set; }
        public Int64 UserExp { get; set; }
        public Int64 StarPoint { get; set; } // 별의 모래
        public Int64 UpgradeCandy { get; set; } // 별의 모래
    }
}