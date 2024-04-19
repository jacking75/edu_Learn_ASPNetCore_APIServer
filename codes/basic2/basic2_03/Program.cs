using basic2_03.Repository;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.MapControllers();

app.Run();
