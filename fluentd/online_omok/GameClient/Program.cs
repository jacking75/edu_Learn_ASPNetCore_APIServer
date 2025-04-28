using Blazored.LocalStorage;
using GameClient;
using GameClient.Handlers;
using GameClient.Providers;
using GameClient.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var apiConfig = builder.Configuration.GetSection("ClientConfig").Get<ClientConfig>()!;
builder.Services.AddAuthorizationCore();

// Providers
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<LoadingStateProvider>();
builder.Services.AddScoped<OmokStateProvider>();
builder.Services.AddScoped<MatchStateProvider>();

// Services
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DataLoadService>();
builder.Services.AddScoped<MailService>();

// Handlers
builder.Services.AddTransient<RequestHandler>();

// Http
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("Game", client => client.BaseAddress = new Uri(apiConfig.GameServer))
				.AddHttpMessageHandler<RequestHandler>();
builder.Services.AddHttpClient("Hive", client => client.BaseAddress = new Uri(apiConfig.HiveServer));
builder.Services.AddHttpClient("Match", client => client.BaseAddress = new Uri(apiConfig.MatchServer));

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
