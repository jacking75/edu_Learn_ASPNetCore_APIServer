using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class RemoveCatchRequest
    {
        [StringLength(45)]
        public string ID { get; set; } = "";
        [StringLength(200)]
        public string AuthToken { get; set; } = "";
        public Int64 RemoveID { get; set; }
    }
}