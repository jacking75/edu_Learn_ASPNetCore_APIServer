
using ZLogger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MatchAPIServer.Service;
using MatchAPIServer;
using ServerShared.Repository;
using ServerShared.Repository.Interfaces;
using System.Text.Json;
using ZLogger.Formatters;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));
builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddSingleton<MatchWorker>();
builder.Services.AddControllers();
SetLogger();

WebApplication app = builder.Build();

app.UseRouting();
app.MapDefaultControllerRoute();
app.Run(configuration["ServerAddress"]);

void SetLogger()
{
	ILoggingBuilder logging = builder.Logging;
	logging.ClearProviders();

	_ = logging.AddZLoggerConsole(options =>
	{
		options.UseJsonFormatter(formatter =>
		{
			formatter.JsonPropertyNames = JsonPropertyNames.Default with
			{
				Timestamp = JsonEncodedText.Encode("timestamp"),
				MemberName = JsonEncodedText.Encode("membername"),
			};
			formatter.JsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
			formatter.IncludeProperties = IncludeProperties.Timestamp | IncludeProperties.ParameterKeyValues | IncludeProperties.MemberName;
		});
	});
}