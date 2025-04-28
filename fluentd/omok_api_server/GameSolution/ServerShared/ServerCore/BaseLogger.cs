using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using ZLogger;

namespace ServerShared.ServerCore;

public abstract class BaseLogger<T>
{
	protected readonly ILogger<T> _logger;

	public BaseLogger(ILogger<T> logger)
	{
		_logger = logger;
	}

	protected void MetricLog(string tag, object context)
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
