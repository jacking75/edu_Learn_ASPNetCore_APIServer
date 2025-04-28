using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;
using HiveAPIServer.Repository;
using HiveAPIServer.Services;
using Prometheus;
using Prometheus.DotNetRuntime;
using System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});

	options.AddPolicy("AllowSpecificOrigin", builder =>
	{
		builder.WithOrigins("http://localhost:3000") // Blazor WebAssembly client origin
			   .AllowAnyHeader()
			   .AllowAnyMethod()
			   .AllowCredentials();
	});
});
builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));
builder.Services.AddTransient<IHiveDb, HiveDb>();
builder.Services.AddTransient<IHiveService, HiveService>();
builder.Services.AddControllers();
builder.Services.AddMetricServer(options => options.Port = 8081);

IDisposable collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
SetLogger();

WebApplication app = builder.Build();
app.UseCors("AllowSpecificOrigin");
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

    var fileDir = configuration["logdir"];

    var exists = Directory.Exists(fileDir);

    if (!exists)
    {
        Directory.CreateDirectory(fileDir);
    }

    logging.AddZLoggerRollingFile(
        options =>
        {
            options.UseJsonFormatter();
            options.FilePathSelector = (timestamp, sequenceNumber) => $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
            options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
            options.RollingSizeKB = 1024;
        });

    _ = logging.AddZLoggerConsole(options =>
    {
        options.UseJsonFormatter();
    });
}
