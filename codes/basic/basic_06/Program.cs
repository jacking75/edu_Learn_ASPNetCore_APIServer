using ZLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(); // 로그 추가
builder.Services.AddControllers();

// 로그 설정
builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();

var app = builder.Build();

app.UseRouting();

// 구현이 올바르게 되지 않았으므로 주석 처리 한다
//app.UseLoadRequestDataMiddlerWare();
//app.UseCheckUserSessionMiddleWare();


app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

IConfiguration configuration = app.Configuration;
DBManager.Init(configuration);

app.Run(configuration["ServerAddress"]);