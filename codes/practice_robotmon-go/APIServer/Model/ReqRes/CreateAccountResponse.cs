using ServerCommon;

namespace ApiServer.Model
{
    public class CreateAccountResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}