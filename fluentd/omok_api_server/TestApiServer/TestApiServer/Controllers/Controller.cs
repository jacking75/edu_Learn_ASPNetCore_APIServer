using Microsoft.AspNetCore.Mvc;
using TestApiServer.ServerCore;
using ZLogger;

namespace TestApiServer.Controllers;

public class Controller<T> : ControllerBase 
{
	protected readonly ILogger<T> _logger;

	public Controller(ILogger<T> logger)
	{
		_logger = logger;
	}

	protected void LogError(string tag, string message, ErrorCode errorCode)
	{
		_logger.ZLogError($"[{tag}], ErrorCode: {errorCode}, Message: {message}");
	}

	protected void LogInfo(string tag, string message)
	{
		_logger.ZLogInformation($"[{tag}], {message}");
	}
}
