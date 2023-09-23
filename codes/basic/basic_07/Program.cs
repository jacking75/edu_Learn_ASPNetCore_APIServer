using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
// 아래 코드를 사용하지 않고, launchSettings.json에서 applicationUrl의 포트를 변경해도 된다.
builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(11500));

// Add services to the container.
builder.Services.AddControllers().AddMvcOptions(options => options.Filters.Add(typeof(ResultFilterChangeResponse)));
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();


app.UseRouting();

app.UseLoadRequestDataMiddlerWare();
app.UseCheckUserSessionMiddleWare();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();