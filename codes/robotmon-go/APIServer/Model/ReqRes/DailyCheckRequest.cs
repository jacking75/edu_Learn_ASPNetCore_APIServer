using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class DailyCheckRequest
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        [StringLength(200)]
        public string AuthToken { get; set; } = "";
    }
}