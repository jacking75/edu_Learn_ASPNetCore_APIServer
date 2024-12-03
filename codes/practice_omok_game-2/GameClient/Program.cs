using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.FluentUI.AspNetCore.Components;
using GameClient.Providers;
using GameClient;
using GameClient.Handlers;
using Microsoft.FluentUI.AspNetCore.Components.Components.Tooltip;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var apiConfig = builder.Configuration.GetSection("ClientConfig").Get<ClientConfig>()!;

// Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Providers
builder.Services.AddScoped<AuthenticationStateProvider, CookieStateProvider>();
builder.Services.AddScoped<MatchStateProvider>();
builder.Services.AddScoped<AttendanceProvider>();
builder.Services.AddScoped<LoadingStateProvider>();
builder.Services.AddScoped<GameStateProvider>();
builder.Services.AddScoped<MailStateProvider>();
builder.Services.AddScoped<InventoryStateProvider>();
builder.Services.AddScoped<GameContentProvider>();
builder.Services.AddScoped<CookieStateProvider>();

// Handlers
builder.Services.AddTransient<CookieHandler>();
builder.Services.AddTransient<VersionHandler>();

// Http
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient("Game", client => client.BaseAddress = new Uri(apiConfig.GameServer))
				.AddHttpMessageHandler<CookieHandler>()
				.AddHttpMessageHandler<VersionHandler>();
builder.Services.AddHttpClient("Hive", client => client.BaseAddress = new Uri(apiConfig.HiveServer));

// UI
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
