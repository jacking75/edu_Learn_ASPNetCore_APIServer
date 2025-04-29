using System.IO;
using GameAPIServer.Repository;
using GameAPIServer.Repository.Interfaces;
using GameAPIServer.Services;
using GameAPIServer.Servicies;
using GameAPIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

builder.Services.AddTransient<IGameDb, GameDb>();
builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<IMasterDb, MasterDb>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IFriendService, FriendService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IDataLoadService, DataLoadService>();
builder.Services.AddControllers();

SettingLogger();

WebApplication app = builder.Build();

if(!await app.Services.GetService<IMasterDb>().Load())
{
    return;
}

//log setting
ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseMiddleware<GameAPIServer.Middleware.VersionCheck>();
app.UseMiddleware<GameAPIServer.Middleware.CheckUserAuthAndLoadUserData>();

app.UseRouting();

app.MapDefaultControllerRoute();

IMasterDb masterDataDB = app.Services.GetRequiredService<IMasterDb>();
await masterDataDB.Load();

app.Run(configuration["ServerAddress"]);

void SettingLogger()
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