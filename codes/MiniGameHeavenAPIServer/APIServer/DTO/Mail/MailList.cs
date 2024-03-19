using APIServer.DTO.DataLoad;

using System.Collections.Generic;

namespace APIServer.DTO.Mail
{
    public class MailboxInfoResponse : ErrorCodeDTO
    {
        public List<UserMailInfo> MailList { get; set; }
    }

}
