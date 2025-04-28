using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.Models.DTO;

public class MailDeleteRequest
{
    [Required]
    public int MailSeq { get; set; }
}
public class MailDeleteResponse : ErrorCode
{
}
