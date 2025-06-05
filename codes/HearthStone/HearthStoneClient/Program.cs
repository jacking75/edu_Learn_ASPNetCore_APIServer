using HearthStoneClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using HearthStoneClient.Services;
using Microsoft.Extensions.Options;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.Configure<ServerConfig>(builder.Configuration.GetSection(nameof(ServerConfig))); ;

        builder.Services.AddHttpClient("HiveServer", (sp, client) =>
        {
            var config = sp.GetRequiredService<IOptions<ServerConfig>>().Value;
            client.BaseAddress = new Uri(config.HiveServer);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddHttpClient("GameServer", (sp, client)=>
        {
            var config = sp.GetRequiredService<IOptions<ServerConfig>>().Value;
            client.BaseAddress = new Uri(config.GameServer);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("GameServer"));

        builder.Services.AddScoped<RequestService>();
        builder.Services.AddScoped<StorageService>();
        builder.Services.AddTransient<PollingService>();

        var host = builder.Build();

        await host.RunAsync();
    }
}

