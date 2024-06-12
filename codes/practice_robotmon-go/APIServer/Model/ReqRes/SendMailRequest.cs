using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class SendMailRequest
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        [StringLength(200)]
        public string AuthToken { get; set; } = "";
        [StringLength(45)]
        public string sendID { get; set; } = "";
        public Int32 StarCount { get; set; }
    }
}