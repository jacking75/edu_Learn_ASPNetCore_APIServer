var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultControllerRoute();

IConfiguration configuration = app.Configuration;
DBManager.Init(configuration);

app.Run(configuration["ServerAddress"]);