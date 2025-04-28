using GameServer.Middlewares;
using GameServer.Models.GameDb;
using GameServer.Repositories;
using GameServer.Repositories.Interfaces;
using GameServer.Services;
using GameServer.Services.Interfaces;
using Prometheus;
using Prometheus.DotNetRuntime;
using ServerShared.Repository;
using ServerShared.Repository.Interfaces;
using System.Text.Json;
using ZLogger;
using ZLogger.Formatters;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
var serverConfig = builder.Configuration.GetSection(nameof(ServerConfig));

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", builder =>
	{
		builder.WithOrigins("http://localhost:3000") // Blazor WebAssembly client origin
			   .AllowAnyHeader()
			   .AllowAnyMethod()
			   .AllowCredentials();
	});
});

builder.Services.Configure<ServerConfig>(serverConfig);

builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<IMasterDb, MasterDb>();
builder.Services.AddTransient<IGameDb<UserAttendance>, AttendanceDb>();
builder.Services.AddTransient<IGameDb<UserItem>, ItemDb>();
builder.Services.AddTransient<IGameDb<Mail>, MailDb>();
builder.Services.AddTransient<IGameDb<User>, UserDb>();
builder.Services.AddTransient<IGameDb<GameResult>, GameResultDb>();

builder.Services.AddTransient<IDataLoadService, DataLoadService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMatchService, MatchService>();
builder.Services.AddTransient<IOmokService, OmokService>();

builder.Services.AddHttpClient("Hive", client => client.BaseAddress = new Uri(serverConfig.Get<ServerConfig>()!.HiveServer));
builder.Services.AddHttpClient("Match", client => client.BaseAddress = new Uri(serverConfig.Get<ServerConfig>()!.MatchServer));

builder.Services.AddControllers();
builder.Services.AddMetricServer(options => options.Port = 8001);

IDisposable collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();

SetLogger();

WebApplication app = builder.Build();
app.UseCors("AllowSpecificOrigin");

if (false == await app.Services.GetService<IMasterDb>().Load())
{
	return;
}

ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseRouting();
app.UseMiddleware<VersionCheck>();
app.UseMiddleware<CheckUserAuthAndLoadUserData>();
app.UseHttpMetrics( options =>
		{
			options.CaptureMetricsUrl = true;
			options.RequestCount.Counter = Metrics.CreateCounter(
			"http_requests_total",
			"HTTP Requests Total",
			new CounterConfiguration { LabelNames = new[] { "controller", "endpoint", "code" } });
			options.RequestDuration.Histogram = Metrics.CreateHistogram(
			"http_request_duration_seconds",
			"HTTP 요청 처리 시간(초)",
			new HistogramConfiguration { LabelNames = new[] { "controller", "endpoint", "code" } });
		}
	);
app.MapDefaultControllerRoute();

IMasterDb masterDataDb = app.Services.GetRequiredService<IMasterDb>();
await masterDataDb.Load();
app.Run();

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
				Exception = JsonEncodedText.Encode("exception"),
			};

			formatter.JsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
				WriteIndented = true
			};

			formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
			//formatter.IncludeProperties = IncludeProperties.Timestamp | IncludeProperties.ParameterKeyValues | IncludeProperties.MemberName | IncludeProperties.Exception;
		});
	}); 
}