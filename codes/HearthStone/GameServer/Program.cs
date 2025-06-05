using GameServer.Repository.Interface;
using GameServer.Repository;
using GameServer.Services.Interface;
using GameServer.Services;
using GameServer.Middleware;
using ZLogger;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));

builder.Services.AddSingleton<IMasterDb, MasterDb>();
builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddSingleton<IGameDb, GameDb>();

builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IMatchService, MatchService>();
builder.Services.AddTransient<IDataLoadService, DataLoadService>();
builder.Services.AddTransient<IShopService, ShopService>();
builder.Services.AddScoped<IHearthStoneService, HearthStoneService>();

builder.Services.AddControllers();


builder.Services.AddHttpClient("HiveServer", (sp, client)=>
{
    var config = sp.GetRequiredService<IOptions<ServerConfig>>().Value;
    client.BaseAddress = new Uri(config.HiveServer);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("MatchServer", (sp, client)=>
{
    var config = sp.GetRequiredService<IOptions<ServerConfig>>().Value;
    client.BaseAddress = new Uri(config.MatchServer);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddCors(options=>
    {
        options.AddDefaultPolicy(policy=>
        {
            policy.WithOrigins(configuration["ClientAddress"])
               .AllowAnyMethod()
               .AllowAnyHeader();
        });
    });

builder.WebHost.UseUrls(configuration["ServerAddress"]);


SettingLogger();

var app = builder.Build();

app.UseMiddleware<GameServer.Middleware.CheckUserAuth>();

app.UseRouting();

app.MapDefaultControllerRoute();

IMasterDb masterDb = app.Services.GetRequiredService<IMasterDb>();
await masterDb.Load();
    
app.UseCors();
app.Run();

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