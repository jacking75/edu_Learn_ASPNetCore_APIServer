using System.Collections.Generic;

namespace GameAPIServer.DTOs;

public class MailboxInfoResponse : ErrorCode
{
    public List<UserMailInfo> MailList { get; set; }
}
