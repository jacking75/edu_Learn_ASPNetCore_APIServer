using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;
using HiveAPIServer.Repository;
using HiveAPIServer.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", builder =>
	{
		builder.WithOrigins("http://localhost:3000") 
			   .AllowAnyHeader()
			   .AllowAnyMethod()
			   .AllowCredentials();
	});
});
builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));
builder.Services.AddTransient<IHiveDb, HiveDb>();
builder.Services.AddTransient<IHiveService, HiveService>();
builder.Services.AddControllers();

SetLogger();

WebApplication app = builder.Build();
app.UseCors("AllowSpecificOrigin");
app.UseRouting();

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

    _ = logging.AddZLoggerConsole(options =>
    {
        options.UseJsonFormatter();
    });
}
