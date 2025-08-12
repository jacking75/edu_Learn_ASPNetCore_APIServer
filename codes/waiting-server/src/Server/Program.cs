using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using WaitingQueue.Server.BackgroundServices;
using WaitingQueue.Server.Hubs;
using WaitingQueue.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. 서비스 등록 (Dependency Injection)

// Redis 연결
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];
if (string.IsNullOrEmpty(redisConnectionString))
    throw new InvalidOperationException("Redis:ConnectionString 설정이 누락되었습니다.");

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString)
);

// 서비스 등록
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton<ITokenService, TokenService>();

// SignalR 등록
builder.Services.AddSignalR();

// 백그라운드 서비스 등록 (QueueProcessor)
builder.Services.AddHostedService<QueueProcessor>();

// 컨트롤러 등록
builder.Services.AddControllers();

// 2. JWT 인증 설정
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = builder.Configuration["Jwt:Secret"];
    if (string.IsNullOrEmpty(jwtSecret))
        throw new InvalidOperationException("Jwt:Secret 설정이 누락되었습니다.");

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

// 3. CORS 설정
builder.Services.AddCors(options =>
{
    // CORS 설정
    builder.Services.AddCors(options =>
    {
        var corsOrigin = builder.Configuration["Cors:Origin"];
        if (string.IsNullOrEmpty(corsOrigin))
            throw new InvalidOperationException("Cors:Origin 설정이 누락되었습니다.");

        options.AddPolicy("AllowAll", policy =>
            policy.WithOrigins(corsOrigin)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
    });
});

var app = builder.Build();

// 4. 미들웨어 파이프라인 설정
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors("AllowAll"); // CORS 미들웨어 사용

app.UseAuthentication(); // 인증 미들웨어
app.UseAuthorization(); // 인가 미들웨어

app.MapControllers();
app.MapHub<QueueHub>("/queuehub"); // SignalR 허브 매핑

app.Run();