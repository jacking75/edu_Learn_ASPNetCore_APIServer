using MatchServer.Models;
using MatchServer.Services.Interface;
using MatchServer.Services;
using MatchServer.Repository;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
// 설정 로드
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
    
// 서비스 등록
builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<IMatchService, MatchService>();

builder.Services.AddControllers();

builder.Services.Configure<DbConfig>(builder.Configuration.GetSection("DbConfig"));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(configuration["GameServerAddress"])
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

SettingZlogger();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

void SettingZlogger() 
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
       options => {
           options.UseJsonFormatter();
           options.FilePathSelector = (timestamp, sequenceNumber) => $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
           options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
           options.RollingSizeKB = 1024;
       });
    logging.AddZLoggerConsole(
        options =>
        {
            options.UseJsonFormatter();
        });
}