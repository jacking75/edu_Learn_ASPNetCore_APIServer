using System;
using System.IO;
using System.Text.Json;
using System.Threading;

using APIServer;
using APIServer.Services;
using APIServer.Services.MasterData;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ZLogger;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.Configure<VersionConfig>(configuration.GetSection(nameof(VersionConfig)));
builder.Services.Configure<ChannelConfig>(configuration.GetSection(nameof(ChannelConfig)));

builder.Services.AddTransient<IAccountDb, AccountDb>();
builder.Services.AddTransient<IGameDb, GameDb>();
builder.Services.AddSingleton<IMemoryDb, RedisDb>();
builder.Services.AddSingleton<MasterDataManager>();
builder.Services.AddSingleton<ChannelUserManager>();

builder.Services.AddControllers();

SettingLogger();

var app = builder.Build();

// Setting Logger 
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory, "Global");


// Setting Middleware
app.UseMiddleware<APIServer.Middleware.CertificationMachine>();
app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
#pragma warning restore ASP0014


// Setting MemoryDb
var redisDb = (RedisDb)app.Services.GetRequiredService<IMemoryDb>();
redisDb.Init(configuration.GetSection("DbConfig")["Redis"]);


// Load Game Notice
var success = await redisDb.LoadGameNotice(configuration.GetSection("GameNoticeKey").Value);
if (success == false)
{
    LogManager.Logger.ZLogCriticalWithPayload(
        eventId: LogManager.EventIdDic[LogManager.EventType.CreateAccount],
        payload: $"Key={configuration["NoticeKey"]}",
        message: $"Failed load game notice from {nameof(redisDb)}");

    Thread.Sleep(1000);
    return;
}


// Load Master Data
var masterDataManager = app.Services.GetRequiredService<MasterDataManager>();
var isLoaded = await masterDataManager.Load(configuration.GetSection("DbConfig")["MasterDb"]);
if (!isLoaded)
{
    LogManager.Logger.ZLogCriticalWithPayload(
        eventId: LogManager.EventIdDic[LogManager.EventType.CreateAccount],
        payload: $"Key={configuration["NoticeKey"]}",
        message: $"Failed load master data");

    Thread.Sleep(1000);
    return;
}


app.Run(configuration["ServerAddress"]);


void SettingLogger()
{
    var logging = builder.Logging;
    logging.ClearProviders();

    var fileDir = configuration["logdir"];

    var exists = Directory.Exists(fileDir);
    if (exists == false)
    {
        Directory.CreateDirectory(fileDir);
    }

    logging.AddZLoggerRollingFile(
        (dt, x) => $"{fileDir}{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log",
        x => x.ToLocalTime().Date, 1024,
        options =>
        {
            options.EnableStructuredLogging = true;
            var time = JsonEncodedText.Encode("Timestamp");

            //DateTime.Now는 UTC+0 이고 한국은 UTC+9이므로 9시간을 더한 값을 출력한다.
            var timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

            options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString(time, timeValue);
                info.WriteToJsonWriter(writer);
            };
        }); // 1024KB

    logging.AddZLoggerConsole(options =>
    {
        options.EnableStructuredLogging = true;
        var time = JsonEncodedText.Encode("EventTime");
        var timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

        options.StructuredLoggingFormatter = (writer, info) =>
        {
            writer.WriteString(time, timeValue);
            info.WriteToJsonWriter(writer);
        };
    });

}