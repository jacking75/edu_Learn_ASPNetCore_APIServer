using System.ComponentModel.DataAnnotations;
using ServerCommon;

namespace ApiServer.Model
{
    public class LoginResponse
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
        [StringLength(200)]
        public string Authtoken { get; set; } = "";
    }
}