
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
using Prometheus;
using Prometheus.DotNetRuntime;
using System;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));
builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddSingleton<MatchWorker>();

builder.Services.AddControllers();
builder.Services.AddMetricServer(options => options.Port = 9001);

IDisposable collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
SetLogger();

WebApplication app = builder.Build();

app.UseRouting();
app.UseHttpMetrics(options =>
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