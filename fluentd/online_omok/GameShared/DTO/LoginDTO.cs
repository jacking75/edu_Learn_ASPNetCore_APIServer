using System.ComponentModel.DataAnnotations;

namespace GameShared.DTO;

public class LoginRequest
{
	[Required]
	public Int64 PlayerId { get; set; }
	[Required]
	public string Token { get; set; } = "";
}

public class LoginResponse : ErrorCodeDTO
{
	public Int64 Uid { get; set; }
    public string AccessToken { get; set; } = "";
}

public class LogoutRequest;

public class LogoutResponse : ErrorCodeDTO;

