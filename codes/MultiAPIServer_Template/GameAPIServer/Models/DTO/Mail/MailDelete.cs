using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTO.Mail
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
