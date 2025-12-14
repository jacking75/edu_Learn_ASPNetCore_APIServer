using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTOs;

public class MailDeleteRequest
{
    [Required]
    public int MailSeq { get; set; }
}
public class MailDeleteResponse : ErrorCode
{
}
