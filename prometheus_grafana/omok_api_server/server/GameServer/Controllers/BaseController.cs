using Microsoft.AspNetCore.Mvc;
using ZLogger;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace GameServer.Controllers;

public abstract class BaseController<T> : ControllerBase
{
    private readonly ILogger<T> _logger;
    private const string _logPrefix = "Action.";

    protected BaseController(ILogger<T> logger)
    {
        _logger = logger;
    }

    protected void ActionLog(object context, [CallerMemberName] string? tag = null) // 호출된 메서드 이름을 tag로 사용
    {
        tag = _logPrefix + tag;
        _logger.ZLogInformation($"[{tag:json}] {context:json}");
    }
}