using APIServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddSingleton<RedisDb>();
builder.Services.AddControllers();

WebApplication app = builder.Build();

app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
#pragma warning restore ASP0014


RedisDb redisDB = app.Services.GetRequiredService<RedisDb>();
redisDB.Init(configuration.GetSection("DbConfig")["Redis"]);

app.Run(configuration["ServerAddress"]);