
using ZLogger;

using basic2_06.Repository;
using basic2_06.Middleware;



var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

// Add services to the container.
builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();


builder.Services.AddControllers();


SettingLogger();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<CheckUserAuthAndLoadUserData>();


app.MapControllers();

app.Run();




void SettingLogger()
{
    ILoggingBuilder logging = builder.Logging;
    _ = logging.ClearProviders();

    string? fileDir = configuration["logdir"];
    if(fileDir == null)
    {
        throw new Exception("logdir is not set in appsettings.json");
    }


    bool exists = Directory.Exists(fileDir);

    if (!exists)
    {
        _ = Directory.CreateDirectory(fileDir);
    }

    _ = logging.AddZLoggerRollingFile(
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