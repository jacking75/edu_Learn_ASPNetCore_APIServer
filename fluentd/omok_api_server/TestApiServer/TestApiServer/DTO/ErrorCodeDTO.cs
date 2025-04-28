using TestApiServer.ServerCore;

namespace TestApiServer.DTO;

public class ErrorCodeDTO
{
	public ErrorCode Result { get; set; } = ErrorCode.None;
}
