using SampleServer.Services;
using System.Text.Json;
using ZLogger;
using ZLogger.Formatters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.Logging.ClearProviders();

//builder.Logging.AddZLoggerConsole(options =>
//{
//	options.UseJsonFormatter(formatter =>
//	{
//		formatter.JsonPropertyNames = JsonPropertyNames.Default with
//		{
//			Timestamp = JsonEncodedText.Encode("timestamp"),
//			MemberName = JsonEncodedText.Encode("membername"),
//			Exception = JsonEncodedText.Encode("exception"),
//		};

//		formatter.JsonSerializerOptions = new JsonSerializerOptions
//		{
//			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
//			WriteIndented = true
//		};



//		formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
//		formatter.IncludeProperties = 
//		IncludeProperties.Timestamp | 
//		IncludeProperties.ParameterKeyValues | 
//		IncludeProperties.MemberName |
//		IncludeProperties.Message |
//		IncludeProperties.Exception;
//	});			
//});

//builder.Logging.AddZLoggerRollingFile(
//	options =>
//	{
//		options.UseJsonFormatter();
//		options.FilePathSelector = (timestamp, sequenceNumber) => $"logs/{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
//		options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
//		options.RollingSizeKB = 1024;
//	}
//);
builder.Services.AddSingleton<ILoggingService, LoggingService>();
builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseRouting();
app.MapDefaultControllerRoute();
app.Run();
