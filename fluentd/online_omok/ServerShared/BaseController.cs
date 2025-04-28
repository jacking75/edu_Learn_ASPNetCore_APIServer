using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace ServerShared;

public abstract class BaseController<T> : ControllerBase
{
	private readonly ILogger<T> _logger;

	public BaseController(ILogger<T> logger)
	{
		_logger = logger;
	}

	protected Int64 GetUserUid()
	{
		Int64.TryParse(HttpContext.Request.Headers["uid"], out Int64 uid);
		return uid;
	}

	protected void ActionLog(object context, [CallerMemberName] string? tag = null)
	{
		_logger.ZLogInformation($"[{tag}] {context:json}");
	}

	protected void InformationLog(string message, object? context = default, [CallerMemberName] string? caller = null)
	{
		_logger.ZLogInformation($"[{caller}] {message} {context:json}");
	}

	protected void ErrorLog(ErrorCode errorCode, object? context = default, [CallerMemberName] string? caller = null)
	{
		_logger.ZLogError($"[{caller}] {errorCode:json} {context:json}");
	}

	protected void ExceptionLog(Exception ex, object? context = default, [CallerMemberName] string? caller = null)
	{
		_logger.ZLogError(ex, $"[{caller}] {context:json}");
	}
}
