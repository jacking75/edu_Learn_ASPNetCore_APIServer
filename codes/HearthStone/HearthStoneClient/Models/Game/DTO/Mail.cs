
using System.ComponentModel.DataAnnotations;

namespace HearthStoneWeb.Models.Game;



public class MailInfoResponse : ErrorCodeDTO
{
    public List<MailInfo> MailList { get; set; }
}
public class MailReadRequest
{
    [Required]
    public Int64 MailId { get; set; }
}

public class MailReadResponse : ErrorCodeDTO
{
    ReceivedReward ReceivedReward { get; set; }
}
public class MailDeleteRequest
{
    [Required]
    public Int64 MailId { get; set; }
}
public class MailDeleteResponse : ErrorCodeDTO
{
}
