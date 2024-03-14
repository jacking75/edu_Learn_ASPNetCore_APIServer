using ServerCommon;

namespace ApiServer.Model
{
    public class MailListResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public Int32 TotalSize { get; set; }
        public List<Tuple<Int64,Int32>> MailInfo { get; set; }
    }
}