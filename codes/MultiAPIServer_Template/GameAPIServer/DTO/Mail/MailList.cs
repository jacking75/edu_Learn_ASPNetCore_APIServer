using HiveAPIServer.DTO.DataLoad;

using System.Collections.Generic;

namespace HiveAPIServer.DTO.Mail
{
    public class MailboxInfoResponse : ErrorCodeDTO
    {
        public List<UserMailInfo> MailList { get; set; }
    }

}
