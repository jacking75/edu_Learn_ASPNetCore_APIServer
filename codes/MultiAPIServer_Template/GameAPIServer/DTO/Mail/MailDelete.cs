using System.ComponentModel.DataAnnotations;

namespace MatchAPIServer.DTO.Mail
{
    public class MailDeleteRequest
    {
        [Required]
        public int MailSeq { get; set; }
    }
    public class MailDeleteResponse : ErrorCodeDTO
    {
    }
}
