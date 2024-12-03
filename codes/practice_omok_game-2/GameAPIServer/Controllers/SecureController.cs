using Microsoft.AspNetCore.Mvc;
namespace GameAPIServer.Controllers;

[Route("[controller]")]
[ApiController]
public class SecureController<T> : ControllerBase
{
	protected readonly ILogger<T> _logger;

	public SecureController(ILogger<T> logger)
	{
		_logger = logger;
	}

	protected Int64 GetUserUid()
	{
		Int64 uid = 0;

		string? uidClaim = User.FindFirst("Uid")?.Value;
		if (string.IsNullOrEmpty(uidClaim))
		{
			return uid;
		}

		Int64.TryParse(uidClaim, out uid);

		return uid;
	}
}
