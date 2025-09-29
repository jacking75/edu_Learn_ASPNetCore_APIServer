using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Models.DTO;


public class MailReceiveRequest
{
    [Required]
    public int MailSeq { get; set; }
}

public class MailReceiveResponse : ErrorCode
{
    public List<ReceivedReward> Rewards { get; set; }
}
