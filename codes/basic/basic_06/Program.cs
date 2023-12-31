using ZLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(); // 로그 추가
builder.Services.AddControllers();

// 로그 설정
builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();

var app = builder.Build();

app.UseRouting();


app.UseLoadRequestDataMiddlerWare();
app.UseCheckUserSessionMiddleWare();


app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

IConfiguration configuration = app.Configuration;
DBManager.Init(configuration);

app.Run(configuration["ServerAddress"]);