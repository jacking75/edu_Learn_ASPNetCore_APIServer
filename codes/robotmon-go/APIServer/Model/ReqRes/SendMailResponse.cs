using ServerCommon;

namespace ApiServer.Model
{
    public class SendMailResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}