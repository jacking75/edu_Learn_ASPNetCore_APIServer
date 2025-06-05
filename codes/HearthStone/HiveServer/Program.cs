using HiveServer.Repository.Interface;
using HiveServer.Repository;
using HiveServer.Services.Interface;
using HiveServer.Services;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));   

builder.Services.AddSingleton<IMemoryDb, MemoryDb>();
builder.Services.AddScoped<IHiveDb, HiveDb>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(configuration["ClientAddress"])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("token", "accountuid"); // 응답 헤더 노출 - HTTPONLY 쿠키 사용 고려
    });
});

builder.WebHost.UseUrls(configuration["ServerAddress"]);

SettingLogger();

var app = builder.Build();

app.UseRouting();

app.MapDefaultControllerRoute();

app.UseCors();

app.Run();

void SettingLogger() 
{
    ILoggingBuilder logging = builder.Logging;
    logging.ClearProviders();

    var fileDir = configuration["logdir"];
    var exists = Directory.Exists(fileDir);
    if(!exists)
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