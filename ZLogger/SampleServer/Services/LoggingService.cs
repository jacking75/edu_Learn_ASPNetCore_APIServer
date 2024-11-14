namespace SampleServer.Services;

using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Formatters;

public class LoggingService : ILoggingService
{
	private readonly ILogger _consolelogger;
	private readonly ILogger _filelogger;

	public LoggingService()
	{
		var factory = LoggerFactory.Create(builder =>
		{
			builder.AddZLoggerConsole(options =>
			{
				options.IncludeScopes = true;
				options.UseJsonFormatter(formatter =>
				{
					formatter.JsonPropertyNames = JsonPropertyNames.Default with
					{
						Timestamp = JsonEncodedText.Encode("timestamp"),
						MemberName = JsonEncodedText.Encode("membername"),
						Exception = JsonEncodedText.Encode("exception"),
					};

					formatter.JsonSerializerOptions = new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
						WriteIndented = true
					};

					formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
					formatter.IncludeProperties = IncludeProperties.All;
				});

			});
		});

		_consolelogger = factory.CreateLogger<LoggingService>();

		var factory2 = LoggerFactory.Create(builder =>
		{
			builder.AddZLoggerRollingFile(
				options =>
				{
					options.UseJsonFormatter();
					options.FilePathSelector = (timestamp, sequenceNumber) => $"logs/{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
					options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
					options.RollingSizeKB = 1024;
				}
			);
		});

		_filelogger = factory2.CreateLogger<LoggingService>();

	}

	public void Log(string message, LogType logType = LogType.Console)
	{
		switch (logType)
		{
			case LogType.Console:
				_consolelogger.ZLogInformation($"{message}");
				break;
			case LogType.File:
				_filelogger.ZLogInformation($"{message}");
				break;
		}
	}


}