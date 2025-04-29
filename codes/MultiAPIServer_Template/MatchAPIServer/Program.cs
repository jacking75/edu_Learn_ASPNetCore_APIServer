using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MatchAPIServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<MatchingConfig>(configuration.GetSection(nameof(MatchingConfig)));


builder.Services.AddSingleton<IMatchWoker, MatchWoker>();


builder.Services.AddControllers();

WebApplication app = builder.Build();

app.MapDefaultControllerRoute();


app.Run(configuration["ServerAddress"]);


