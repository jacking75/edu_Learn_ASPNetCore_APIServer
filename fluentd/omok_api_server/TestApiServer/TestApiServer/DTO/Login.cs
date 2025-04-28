namespace TestApiServer.DTO;

public class LoginRequest
{
	public string Username { get; set; } = "";
	public string Password { get; set; } = "";
}

public class LoginResponse : ErrorCodeDTO
{
}
