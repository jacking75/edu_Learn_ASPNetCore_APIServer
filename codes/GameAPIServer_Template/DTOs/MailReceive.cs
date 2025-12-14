using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GameAPIServer.Models;


namespace GameAPIServer.DTOs;


public class MailReceiveRequest
{
    [Required]
    public int MailSeq { get; set; }
}

public class MailReceiveResponse : ErrorCode
{
    public List<ReceivedReward> Rewards { get; set; }
}
