using GameAPIServer.DTO.DataLoad;

using System.Collections.Generic;

namespace GameAPIServer.DTO.Mail
{
    public class MailboxInfoResponse : ErrorCodeDTO
    {
        public List<UserMailInfo> MailList { get; set; }
    }

}
