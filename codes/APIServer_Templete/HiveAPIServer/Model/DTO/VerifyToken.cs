using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO
{
    public class VerifyTokenBody
    {
        [Required]
        public string HiveToken { get; set; }
        [Required]
        public Int64 PlayerId { get; set; }
    }

    public class VerifyTokenResponse
    {
        [Required]
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
}
