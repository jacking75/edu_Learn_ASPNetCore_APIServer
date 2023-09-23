using System;
using System.IO;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using ZLogger;



var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

SetBuilderLogging();

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));
builder.Services.Configure<DirectoryPath>(configuration.GetSection(nameof(DirectoryPath)));
builder.Services.Configure<OptionFlags>(configuration.GetSection(nameof(OptionFlags)));

builder.Services.AddSingleton<APIServer.MasterData.IManager, APIServer.MasterData.Manager>();
builder.Services.AddControllers();

// 필터 추가
builder.Services.AddMvc().AddMvcOptions(options =>
{
    options.Filters.Add<APIServer.Filters.ResponseDataConverter>();
    options.Filters.Add<APIServer.Filters.ResponseDataEncoder>();
});

var app = builder.Build();


app.UseMiddleware<APIServer.Middlewares.Gateway>();

app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
#pragma warning restore ASP0014


var masterDataMgr = app.Services.GetRequiredService<APIServer.MasterData.IManager>();
masterDataMgr.Load(configuration.GetSection("DirectoryPath")["MasterData"]);

app.Run(configuration["ServerAddress"]);



void SetBuilderLogging()
{
    var builerLogging = builder.Logging;
    _ = builerLogging.ClearProviders();

    string fileDir = configuration["LogDirectory"];

    bool exists = Directory.Exists(fileDir);
    if (exists == false)
    {
        _ = Directory.CreateDirectory(fileDir);
    }

    _ = builerLogging.AddZLoggerConsole(options =>
    {
        options.EnableStructuredLogging = true;
        JsonEncodedText time = JsonEncodedText.Encode("EventTime");
        JsonEncodedText timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

        options.StructuredLoggingFormatter = (writer, info) =>
        {
            writer.WriteString(time, timeValue);
            info.WriteToJsonWriter(writer);
        };
    });

    const Int32 RollSizeKB = 1024;

    _ = builerLogging.AddZLoggerRollingFile(
        (dt, x) => $"{fileDir}{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log",
        x => x.ToLocalTime().Date, RollSizeKB,
        options =>
        {
            options.EnableStructuredLogging = true;
            JsonEncodedText time = JsonEncodedText.Encode("Timestamp");
            //DateTime.Now는 UTC+0 이고 한국은 UTC+9이므로 9시간을 더한 값을 출력한다.
            JsonEncodedText timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));

            options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString(time, timeValue);
                info.WriteToJsonWriter(writer);
            };
        });
}

#region Config Classes

public class DbConfig
{
    public string GameDb { get; set; } = string.Empty;

    public string Redis { get; set; } = string.Empty;
}

public class DirectoryPath
{
    public string Log { get; set; } = string.Empty;

    public string MasterData { get; set; } = string.Empty;
}

public class OptionFlags
{
    public bool EnableCrypto { get; set; } = false;
}

#endregion