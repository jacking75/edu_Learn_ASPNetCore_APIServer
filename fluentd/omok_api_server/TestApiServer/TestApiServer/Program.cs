using TestApiServer.Repositories.Interfaces;
using TestApiServer.Repositories;
using TestApiServer.Services.Interfaces;
using TestApiServer.Services;
using TestApiServer.ServerCore;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddControllers();

SetLogger();
var app = builder.Build();
ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
app.UseRouting();
app.MapDefaultControllerRoute();
app.Run();

void SetLogger()
{
	ILoggingBuilder logging = builder.Logging;
	logging.ClearProviders();

	var fileDir = ((IConfiguration)builder.Configuration)["LogDirectory"];

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