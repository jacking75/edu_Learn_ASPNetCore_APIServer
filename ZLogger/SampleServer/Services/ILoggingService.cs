namespace SampleServer.Services;

public enum LogType
{
	Console,
	File,
}
public interface ILoggingService
{
	public void Log(string message, LogType logType = LogType.Console);
}
