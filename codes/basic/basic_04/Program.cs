var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(); // 로그 추가
builder.Services.AddControllers();

// 로그 설정
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.MapDefaultControllerRoute();

IConfiguration configuration = app.Configuration;
DBManager.Init(configuration);

app.Run(configuration["ServerAddress"]);