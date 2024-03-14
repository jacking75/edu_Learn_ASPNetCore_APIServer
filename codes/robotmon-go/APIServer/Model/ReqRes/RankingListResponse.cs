using ServerCommon;

namespace ApiServer.Model
{
    public class RankingListResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int32 TotalSize { get; set; }
        public string[] RankingIdList { get; set; } = null;
    }
}