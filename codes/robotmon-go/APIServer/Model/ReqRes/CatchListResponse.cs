using ServerCommon;

namespace ApiServer.Model
{
    public class CatchListResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        public List<Tuple<Int64, Int64, DateTime, Int32>> MonsterInfoList { get; set; } =
            new List<Tuple<Int64, Int64, DateTime, Int32>>();
    }
}