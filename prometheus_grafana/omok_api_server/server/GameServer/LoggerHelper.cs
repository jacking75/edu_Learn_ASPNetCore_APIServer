using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace GameServer;

public static class LoggerHelper
{
    // 공통 ActionLog 메서드
    public static void ActionLog<T>(ILogger<T> logger, object context, [CallerMemberName] string? tag = null)
    {
        string logPrefix = "Action.";
        tag = logPrefix + tag;
        logger.ZLogInformation($"[{tag:json}] {context:json}");
    }
}
