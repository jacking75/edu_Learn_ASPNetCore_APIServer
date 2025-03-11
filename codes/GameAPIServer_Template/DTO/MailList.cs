using System.Collections.Generic;

namespace GameAPIServer.DTO
{
    public class MailboxInfoResponse : ErrorCodeDTO
    {
        public List<UserMailInfo> MailList { get; set; }
    }

}
