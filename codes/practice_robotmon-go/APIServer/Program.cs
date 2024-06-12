using ApiServer;
using ApiServer.Options;
using ApiServer.Services;
using Microsoft.Extensions.Logging.Console;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

// 내가 만든 Config 파일 가져오기
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory());
    config.AddJsonFile("MyConfig.json",
                       optional: true,
                       reloadOnChange: true);
});

// Config 파일 가져오기
IConfiguration Configuration = builder.Configuration;

// 환경 세팅하기
if (Configuration["Environment"] == "Production")
{
    builder.Environment.EnvironmentName = Environments.Production;
}
else
{
    builder.Environment.EnvironmentName = Environments.Development;
}

// Config 파일 추가하기
//builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Config 파일 추가 등록하기
builder.Services.Configure<DbConfig>(Configuration.GetSection(nameof(DbConfig)));

// 서비스 등록
builder.Services.AddTransient<IAccountDb, AccountDb>();
builder.Services.AddTransient<IGameDb, GameDb>();
builder.Services.AddSingleton<IRedisDb, RedisDb>();
builder.Services.AddSingleton<IDataStorage, DataStorage>();
builder.Services.AddSingleton<IRankingManager, RankManager>();
builder.Services.AddControllers();

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();

    if (builder.Environment.EnvironmentName == Environments.Production)
    {
        var fileDir = Configuration["logdir"];
        bool exists = Directory.Exists(fileDir);
        if (!exists)
        {
            Directory.CreateDirectory(fileDir);
        }

        exists = File.Exists($"{fileDir}logTest.pos");
        if (!exists)
        {
            File.Create($"{fileDir}logTest.pos").Dispose();
        }

        logging.AddZLoggerRollingFile(
            (dt, x) => $"{fileDir}{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", 
            x => x.ToLocalTime().Date, 1024); // 1024KB
    }
    else
    {
        logging.AddZLoggerConsole();
    }
});

// app build
var app = builder.Build();

// 미들웨어 추가
app.UseMiddleware<AuthTokenCheckMiddleware>();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// Redis Singleton 가져오기
var redisDb = app.Services.GetService<IRedisDb>();
var datastorage = app.Services.GetService<IDataStorage>();
var rankingManager = app.Services.GetService<IRankingManager>();
var logger = app.Services.GetService<ILogger<Program>>();

if (redisDb is null || datastorage is null || rankingManager is null || logger is null)
{
    Console.WriteLine("singleton is null");
}
else
{
    // 사용자 초기화
    redisDb.Init(Configuration["SessionConfig:SessionCacheRedisIp"]);
    datastorage.Load(Configuration["DbConfig:GameConnStr"]);
    rankingManager.Init(Configuration["DbConfig:GameConnStr"], redisDb);

    // 현재 모드 확인하기
    logger.ZLogInformation($"Program env Mode : {app.Environment.EnvironmentName}");
    logger.ZLogInformation($"My appsetting Mode : {Configuration["Mode"]}");
    logger.ZLogInformation($"urls : {Configuration["urls"]}");
    logger.ZLogInformation(Environment.Version.ToString());
    // 앱 실행 - IP Port 설정
    app.Run(Configuration["urls"]);
}