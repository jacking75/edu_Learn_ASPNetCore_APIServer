using ZLogger;
using GameAPIServer.Repositories.Interfaces;
using GameAPIServer.Services.Interfaces;
using GameAPIServer.Services;
using GameAPIServer.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using GameAPIServer.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Cookie Authentication
builder.Services
	.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/login";
		options.LogoutPath = "/logout";
		options.SlidingExpiration = true;                       // 쿠키 자동 갱신
		options.Cookie.HttpOnly = true;
		options.Cookie.SameSite = SameSiteMode.Lax;             // 임시   
		options.Cookie.SecurePolicy = CookieSecurePolicy.None;  // 임시   
		options.Cookie.MaxAge = RedisExpiryTimes.AuthTokenExpiryShort;
		options.Events.OnRedirectToLogin = context =>
		{
			context.Response.StatusCode = 200;
			return Task.CompletedTask;
		};

		options.Events.OnRedirectToAccessDenied = context =>
		{
			context.Response.StatusCode = 200;
			return Task.CompletedTask;
		};
	});

IConfiguration configuration = builder.Configuration;
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", builder =>
	{
		builder.WithOrigins("http://localhost:3000")            // 임시 
			   .AllowAnyHeader()
			   .AllowAnyMethod()
			   .AllowCredentials();
	});
});

builder.Services.Configure<ServerConfig>(configuration.GetSection(nameof(ServerConfig)));

/* DB */
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IGameResultRepository, GameResultRepository>();
builder.Services.AddSingleton<IMemoryRepository, MemoryRepository>();
builder.Services.AddSingleton<IMasterRepository, MasterRepository>();
builder.Services.AddSingleton<IMailRepository, MailRepository>();
builder.Services.AddSingleton<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();

/* Service */
builder.Services.AddTransient<IOmokService, OmokService>();
builder.Services.AddTransient<IMatchService, MatchService>();
builder.Services.AddTransient<IDataLoadService, DataLoadService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IGameDataService, GameDataService>();

/* Http Client */
builder.Services.AddHttpClient<IAuthService, AuthService>();    
builder.Services.AddHttpClient<IMatchService, MatchService>();

builder.Services.AddControllers();

SetLogger();

WebApplication app = builder.Build();

if (false == await app.Services.GetService<IMasterRepository>().Load())
{
	return;
}

ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseCors("AllowSpecificOrigin");

app.UseRouting();
app.UseAuthentication();  // Must come between Routing and Endpoints
app.UseAuthorization();   // Must come after authentication
app.UseMiddleware<VersionCheck>();
app.UseMiddleware<CheckUserAuthAndLoadUserData>();
app.MapDefaultControllerRoute();

IMasterRepository masterDataDb = app.Services.GetRequiredService<IMasterRepository>();
await masterDataDb.Load();

app.Run();

void SetLogger()
{
	ILoggingBuilder logging = builder.Logging;
	logging.ClearProviders();

	var fileDir = ((IConfiguration)builder.Configuration)["logdir"];

	var exists = Directory.Exists(fileDir);

	if (!exists)
	{
		Directory.CreateDirectory(fileDir);
	}

	logging.AddZLoggerRollingFile(
		options =>
		{
			options.UseJsonFormatter();
			options.FilePathSelector = (timestamp, sequenceNumber) => $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
			options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
			options.RollingSizeKB = 1024;
		});

	_ = logging.AddZLoggerConsole(options =>
	{
		options.UseJsonFormatter();
	});
}