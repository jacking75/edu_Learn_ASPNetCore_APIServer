using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using WaitingQueue.Server.BackgroundServices;
using WaitingQueue.Server.Hubs;
using WaitingQueue.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. ���� ��� (Dependency Injection)

// Redis ����
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
if (string.IsNullOrEmpty(redisConnectionString))
    throw new InvalidOperationException("Redis:ConnectionString ������ �����Ǿ����ϴ�.");

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString)
);

// ���� ���
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// SignalR ���
builder.Services.AddSignalR();

// ��׶��� ���� ��� (QueueProcessor)
builder.Services.AddHostedService<QueueProcessor>();

// ��Ʈ�ѷ� ���
builder.Services.AddControllers();

// 2. JWT ���� ����
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = builder.Configuration["Jwt:Secret"];
    if (string.IsNullOrEmpty(jwtSecret))
        throw new InvalidOperationException("Jwt:Secret ������ �����Ǿ����ϴ�.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

// 3. CORS ����
builder.Services.AddCors(options =>
{
    // CORS ����
    builder.Services.AddCors(options =>
    {
        var corsOrigin = builder.Configuration["Cors:Origin"];
        if (string.IsNullOrEmpty(corsOrigin))
            throw new InvalidOperationException("Cors:Origin ������ �����Ǿ����ϴ�.");

        options.AddPolicy("AllowAll", policy =>
            policy.WithOrigins(corsOrigin)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
    });
});

var app = builder.Build();

// 4. �̵���� ���������� ����
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors("AllowAll"); // CORS �̵���� ���

app.UseAuthentication(); // ���� �̵����
app.UseAuthorization(); // �ΰ� �̵����

app.MapControllers();
app.MapHub<QueueHub>("/queuehub"); // SignalR ��� ����

app.Run();