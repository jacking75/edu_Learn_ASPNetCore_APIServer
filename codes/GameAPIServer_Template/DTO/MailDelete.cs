using System.ComponentModel.DataAnnotations;

namespace GameAPIServer.DTO
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
