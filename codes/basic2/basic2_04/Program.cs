using basic2_04.Repository;


var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

// Add services to the container.
builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.MapControllers();

app.Run();
