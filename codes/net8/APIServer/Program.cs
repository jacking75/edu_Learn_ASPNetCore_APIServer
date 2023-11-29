using System;
using System.IO;
using System.Text.Json;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

builder.Services.AddTransient<IAccountDb, AccountDb>();
builder.Services.AddTransient<IGameDb, GameDb>();
builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<APIServer.MasterData.IManager, APIServer.MasterData.Manager>();
builder.Services.AddControllers();

SettingLogger();

WebApplication app = builder.Build();


//log setting
ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory, "Global");


app.UseMiddleware<APIServer.Middleware.CheckUserAuthAndLoadUserData>();


app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
#pragma warning restore ASP0014


IMemoryDb redisDB = app.Services.GetRequiredService<IMemoryDb>();
redisDB.Init(configuration.GetSection("DbConfig")["Redis"]);

APIServer.MasterData.IManager masterDataDB = app.Services.GetRequiredService<APIServer.MasterData.IManager>();
masterDataDB.Load(configuration.GetSection("DbConfig")["MasterDataDb"]);


app.Run(configuration["ServerAddress"]);



void SettingLogger()
{
    ILoggingBuilder logging = builder.Logging;
    _ = logging.ClearProviders();

    string fileDir = configuration["logdir"];

    bool exists = Directory.Exists(fileDir);

    if (!exists)
    {
        _ = Directory.CreateDirectory(fileDir);
    }

    _ = logging.AddZLoggerRollingFile(
        (dt, x) => $"{fileDir}{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log",
        x => x.ToLocalTime().Date, 1024,
        options =>
        {
            options.EnableStructuredLogging = true;
            JsonEncodedText time = JsonEncodedText.Encode("Timestamp");
            //DateTime.Now는 UTC+0 이고 한국은 UTC+9이므로 9시간을 더한 값을 출력한다.
            JsonEncodedText timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

            options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString(time, timeValue);
                info.WriteToJsonWriter(writer);
            };
        }); // 1024KB

    _ = logging.AddZLoggerConsole(options =>
    {
        options.EnableStructuredLogging = true;
        JsonEncodedText time = JsonEncodedText.Encode("EventTime");
        JsonEncodedText timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

        options.StructuredLoggingFormatter = (writer, info) =>
        {
            writer.WriteString(time, timeValue);
            info.WriteToJsonWriter(writer);
        };
    });

}