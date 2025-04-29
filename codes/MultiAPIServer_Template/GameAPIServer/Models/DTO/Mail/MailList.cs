using MatchAPIServer.DTO.DataLoad;

using System.Collections.Generic;

namespace MatchAPIServer.DTO.Mail
{
    public class MailboxInfoResponse : ErrorCodeDTO
    {
        public List<UserMailInfo> MailList { get; set; }
    }

}
