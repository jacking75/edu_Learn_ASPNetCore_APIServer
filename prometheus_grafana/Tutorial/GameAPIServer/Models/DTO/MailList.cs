using System.Collections.Generic;

namespace GameAPIServer.Models.DTO;

public class MailboxInfoResponse : ErrorCode
{
    public List<UserMailInfo> MailList { get; set; }
}
