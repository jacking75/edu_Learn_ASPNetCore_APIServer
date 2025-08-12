# 대기열 서버 (Waiting Queue Server)
트래픽 관리를 위한 대기열 시스템이다. 동시 접속자 수를 제한하고, 대기 중인 사용자에게 실시간으로 순번을 알려주는 서버이다.
[이 저장소](https://github.com/pjt3591oo/waiting-server) 에 있는 코드를 C#으로 포팅한 것이다.  
  
  
## 주요 기능
- 대기열 관리: Redis Sorted Set을 사용한 효율적인 대기열 관리
- 실시간 업데이트: SugnalR를 통한 실시간 대기 순번 알림
- 토큰 기반 인증: JWT를 사용한 접근 권한 관리
- 자동 대기열 처리: 5초마다 자동으로 대기열을 처리하여 사용자 입장
- 동시 접속자 제한: 설정 가능한 최대 동시 접속자 수
  

## 프로젝트 생성
  
### 📝 전체 과정 요약
1.  솔루션 폴더를 생성한다.
2.  **ASP.NET Core Web API** 프로젝트(서버)를 생성하고 필요한 NuGet 패키지를 설치한다.
3.  **Blazor WebAssembly** 프로젝트(클라이언트)를 생성하고 필요한 NuGet 패키지를 설치한다.
4.  솔루션에 두 프로젝트를 추가한다.

-----

### ⚙️ 1단계: 프로젝트 폴더 및 솔루션 생성
먼저, 모든 프로젝트를 담을 최상위 폴더와 솔루션 파일을 생성한다.

```bash
# 1. 프로젝트를 모두 담을 폴더를 생성하고 이동합니다.
mkdir WaitingQueueSolution
cd WaitingQueueSolution

# 2. 솔루션 파일을 생성한다. (솔루션은 여러 프로젝트를 관리하는 컨테이너이다.)
dotnet new sln -n WaitingQueue
```

-----

### сервер 2단계: ASP.NET Core 웹 API (서버) 생성
이제 백엔드 서버 프로젝트를 생성하고 필요한 라이브러리(NuGet 패키지)를 설치한다.

1.  **Web API 프로젝트 생성**

      * `-n` : 프로젝트 이름을 지정한다.
      * `-o` : 프로젝트가 생성될 폴더(출력 경로)를 지정한다.

    <!-- end list -->

    ```bash
    dotnet new webapi -n WaitingQueue.Server -o src/Server
    ```

2.  **필요한 NuGet 패키지 설치**
    생성된 서버 프로젝트 폴더로 이동하여 다음 패키지들을 설치합니다.

    ```bash
    cd src/Server
    ```

      * **StackExchange.Redis**: Redis 데이터베이스와 통신하기 위한 클라이언트 라이브러리입니다.
        ```bash
        dotnet add package StackExchange.Redis
        ```
      * **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT(JSON Web Token)를 사용한 인증 기능을 구현하기 위해 필요합니다.
        ```bash
        dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
        ```
      * **Microsoft.AspNetCore.SignalR**: 실시간 웹 통신(WebSocket)을 위한 SignalR 라이브러리입니다.
        ```bash
        dotnet add package Microsoft.AspNetCore.SignalR
        ```
      
    서버 설정이 끝났습니다. 이제 다시 최상위 폴더로 이동합니다.

    ```bash
    cd ../..
    ```

-----

### 🌐 3단계: Blazor WebAssembly (클라이언트) 생성

다음으로 사용자에게 보여질 프론트엔드 Blazor 프로젝트를 생성하고 필요한 패키지를 설치합니다.

1.  **Blazor WebAssembly 프로젝트 생성**

    ```bash
    dotnet new blazorwasm -n WaitingQueue.Client -o src/Client
    ```

2.  **필요한 NuGet 패키지 설치**
    생성된 클라이언트 프로젝트 폴더로 이동하여 패키지를 설치합니다.

    ```bash
    cd src/Client
    ```

      * **Microsoft.AspNetCore.SignalR.Client**: 서버의 SignalR 허브와 실시간 통신을 하기 위해 필요합니다.
        ```bash
        dotnet add package Microsoft.AspNetCore.SignalR.Client
        ```
      * **Microsoft.AspNetCore.Components.WebAssembly.Authentication**: Blazor 앱에서 사용자의 인증 상태(로그인/로그아웃)를 관리합니다.
        ```bash
        dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Authentication
        ```

    클라이언트 설정이 끝났습니다. 다시 최상위 폴더로 이동합니다.

    ```bash
    cd ../..
    ```

-----

### 🔗 4단계: 솔루션에 프로젝트 추가 및 실행

마지막으로, 생성한 서버와 클라이언트 프로젝트를 솔루션 파일에 추가하여 Visual Studio나 VS Code에서 쉽게 관리할 수 있도록 합니다.

1.  **솔루션에 두 프로젝트 추가**

    ```bash
    dotnet sln add src/Server/WaitingQueue.Server.csproj
    dotnet sln add src/Client/WaitingQueue.Client.csproj
    ```

2.  **프로젝트 실행**

      * 먼저 **서버**를 실행합니다.
        ```bash
        cd src/Server
        dotnet run
        ```
      * 새 터미널을 열어 **클라이언트**를 실행합니다.
        ```bash
        cd src/Client # (다른 터미널에서 실행)
        dotnet run
        ```

이제 모든 설정이 완료되었습니다\! 이 명령어들을 순서대로 실행하면 제시해 드린 코드를 실행할 수 있는 개발 환경이 완벽하게 구축됩니다. 👍


## 서버  
변환된 코드는 기존 Node.js 프로젝트의 핵심 기능인 **Redis를 활용한 대기열 관리**, **실시간 업데이트를 위한 WebSocket(SignalR) 통신**, **JWT 기반 인증**, **백그라운드 대기열 처리** 등을 모두 포함하고 있습니다.

### 1\. 프로젝트 구조
변환된 ASP.NET Core 프로젝트의 구조는 다음과 같습니다.

```
/WaitingQueueServer
|
├── Controllers
│   └── QueueController.cs
├── Hubs
│   └── QueueHub.cs
├── Services
│   ├── IQueueService.cs
│   ├── QueueService.cs
│   ├── ITokenService.cs
│   └── TokenService.cs
├── BackgroundServices
│   └── QueueProcessor.cs
├── Models
│   ├── QueueStatus.cs
│   ├── UserData.cs
│   └── ... (기타 DTO 모델)
├── appsettings.json
└── Program.cs
```

### 2\. `appsettings.json` - 설정 파일

Node.js 프로젝트의 `src/config/index.js` 파일에 해당하는 부분입니다. Redis, JWT, CORS, 대기열 관련 설정을 관리합니다.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-that-is-long-enough",
    "Issuer": "WaitingQueueServer",
    "Audience": "WaitingQueueUsers",
    "ExpiresInMinutes": 1440 // 24 hours
  },
  "Queue": {
    "MaxConcurrentUsers": 2,
    "TimeoutMinutes": 30,
    "EstimatedServiceTimeSeconds": 180
  },
  "Cors": {
    "Origin": "*"
  }
}
```

### 3\. `Program.cs` - 애플리케이션 설정 및 실행

Node.js 프로젝트의 `src/index.js`와 같이 애플리케이션의 시작점입니다. 서비스 등록, 미들웨어 설정, 라우팅, SignalR, JWT 인증 설정 등을 담당합니다.

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using WaitingQueueServer.BackgroundServices;
using WaitingQueueServer.Hubs;
using WaitingQueueServer.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. 서비스 등록 (Dependency Injection)

// Redis 연결
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"])
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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});

// 3. CORS 설정
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins(builder.Configuration["Cors:Origin"])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
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
```

### 4\. `Services` - 핵심 비즈니스 로직

#### `IQueueService.cs` & `QueueService.cs`

Node.js의 `src/services/queueService.js`에 해당하며, Redis를 이용한 대기열의 핵심 로직을 처리합니다.

```csharp
// IQueueService.cs
using WaitingQueueServer.Models;

public interface IQueueService
{
    Task<QueueAddResult> AddToQueueAsync(string userId, UserData userData);
    Task<QueueStatus> GetQueueStatusAsync(string userId);
    Task<List<string>> ProcessQueueAsync();
    Task<bool> RemoveFromQueueAsync(string userId);
    Task<QueueInfo> GetQueueInfoAsync();
    Task ClearQueueAsync();
    Task UpdateQueuePositionsAsync();
}

// QueueService.cs (일부 핵심 메서드)
public class QueueService : IQueueService
{
    private readonly IDatabase _redis;
    private readonly IConfiguration _config;
    private readonly IHubContext<QueueHub> _hubContext;
    private const string QueueKey = "waiting:queue";
    private const string ActiveUsersKey = "active:users";
    private const string UserDataPrefix = "user:data:";

    public QueueService(IConnectionMultiplexer redis, IConfiguration config, IHubContext<QueueHub> hubContext)
    {
        _redis = redis.GetDatabase();
        _config = config;
        _hubContext = hubContext;
    }

    public async Task<QueueAddResult> AddToQueueAsync(string userId, UserData userData)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var queueToken = Guid.NewGuid().ToString();

        // 사용자 데이터 저장 (Hash)
        var userKey = $"{UserDataPrefix}{userId}";
        await _redis.HashSetAsync(userKey, new HashEntry[]
        {
            new("userId", userId),
            new("queueToken", queueToken),
            new("joinedAt", timestamp.ToString()),
            new("email", userData.Email ?? ""),
            new("metadata", JsonConvert.SerializeObject(userData.Metadata ?? new object()))
        });
        await _redis.KeyExpireAsync(userKey, TimeSpan.FromMinutes(_config.GetValue<int>("Queue:TimeoutMinutes")));

        // 대기열에 추가 (Sorted Set)
        await _redis.SortedSetAddAsync(QueueKey, userId, timestamp);

        var position = await GetQueuePositionAsync(userId);
        return new QueueAddResult { /* ... */ };
    }

    public async Task<List<string>> ProcessQueueAsync()
    {
        var activeUsers = await _redis.SetLengthAsync(ActiveUsersKey);
        var availableSlots = _config.GetValue<int>("Queue:MaxConcurrentUsers") - activeUsers;

        if (availableSlots <= 0) return new List<string>();

        // 대기열에서 사용자 가져오기
        var nextUsers = await _redis.SortedSetRangeByRankAsync(QueueKey, 0, availableSlots - 1);
        var processedUsers = new List<string>();

        foreach (var userId in nextUsers)
        {
            await _redis.SortedSetRemoveAsync(QueueKey, userId);
            await _redis.SetAddAsync(ActiveUsersKey, userId);
            processedUsers.Add(userId.ToString());
        }
        return processedUsers;
    }
    // ... 기타 메서드 구현
}
```

#### `ITokenService.cs` & `TokenService.cs`

Node.js의 `src/services/tokenService.js`에 해당하며, JWT 생성 및 검증을 담당합니다.

```csharp
// ITokenService.cs
using System.Security.Claims;

public interface ITokenService
{
    string GenerateAccessToken(string userId);
    ClaimsPrincipal GetPrincipalFromToken(string token);
}

// TokenService.cs
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(string userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresInMinutes")),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    // ... 기타 메서드
}
```

### 5\. `Controllers/QueueController.cs` - API 엔드포인트

Node.js의 `src/controllers/queueController.js`와 `src/routes/queue.js`의 역할을 하며, HTTP 요청을 받아 처리합니다.

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaitingQueueServer.Models;
using WaitingQueueServer.Services;

[ApiController]
[Route("api/queue")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly ITokenService _tokenService;

    public QueueController(IQueueService queueService, ITokenService tokenService)
    {
        _queueService = queueService;
        _tokenService = tokenService;
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinQueue([FromBody] JoinQueueRequest request)
    {
        // ... 로직 구현 ...
        var currentStatus = await _queueService.GetQueueStatusAsync(request.UserId);
        if(currentStatus.Status == "waiting" || currentStatus.Status == "active")
        {
            // ...
        }

        var result = await _queueService.AddToQueueAsync(request.UserId, new UserData { /* ... */ });
        await _queueService.ProcessQueueAsync(); // 즉시 처리 시도
        // ...
        return Created("", result);
    }

    [HttpGet("status/{userId}")]
    public async Task<IActionResult> GetQueueStatus(string userId)
    {
        var status = await _queueService.GetQueueStatusAsync(userId);
        if (status.Status == "not_in_queue") return NotFound("User not found in queue");
        return Ok(status);
    }

    [Authorize] // JWT 인증 필요
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAccess()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // ...
        return Ok();
    }

    // ... 기타 엔드포인트 (leave, info, clear)
}
```

### 6\. `Hubs/QueueHub.cs` - 실시간 통신

Node.js의 `socket.io` 로직에 해당하며, SignalR을 사용하여 클라이언트와 실시간으로 통신합니다.

```csharp
using Microsoft.AspNetCore.SignalR;

public class QueueHub : Hub
{
    // 사용자가 자신의 고유 room(그룹)에 참여
    public async Task JoinQueue(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
    }

    // 클라이언트에게 대기열 업데이트 정보 전송
    public async Task SendQueueUpdate(string userId, object data)
    {
        await Clients.Group($"user-{userId}").SendAsync("queue-update", data);
    }

    // 대기열 처리가 완료된 사용자에게 알림 전송
    public async Task SendQueueReady(string userId, object data)
    {
        await Clients.Group($"user-{userId}").SendAsync("queue-ready", data);
    }
}
```

### 7\. `BackgroundServices/QueueProcessor.cs` - 백그라운드 작업

Node.js의 `src/utils/queueProcessor.js`에 해당하며, 5초마다 주기적으로 대기열을 처리하는 백그라운드 서비스를 구현합니다.

```csharp
using Microsoft.AspNetCore.SignalR;
using WaitingQueueServer.Hubs;
using WaitingQueueServer.Services;

public class QueueProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueueProcessor> _logger;

    public QueueProcessor(IServiceProvider serviceProvider, ILogger<QueueProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queue Processor is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();
                var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<QueueHub>>();

                var processedUsers = await queueService.ProcessQueueAsync();

                if (processedUsers.Any())
                {
                    _logger.LogInformation($"Processing {processedUsers.Count} users from queue");

                    foreach (var userId in processedUsers)
                    {
                        var accessToken = tokenService.GenerateAccessToken(userId);
                        await hubContext.Clients.Group($"user-{userId}").SendAsync("queue-ready", new { status = "active", accessToken }, stoppingToken);
                    }

                    // 남은 사용자들의 대기 순번 업데이트
                    await queueService.UpdateQueuePositionsAsync();
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("Queue Processor is stopping.");
    }
}
```
  
    
## 클라이언트  
이 Blazor 앱은 다음 기능을 포함합니다.

  * **사용자 ID 입력 및 대기열 참여**
  * **SignalR을 통한 실시간 대기 상태 업데이트** (현재 순번, 예상 대기 시간 등)
  * 대기열 통과 시 **JWT 액세스 토큰 수신 및 저장**
  * 발급받은 토큰으로 **보호된 API 엔드포인트에 접근하여 접근 권한 확인**

-----

### 1\. Blazor 프로젝트 생성 및 NuGet 패키지 설치

먼저, `WaitingQueue.Client`라는 이름의 Blazor WebAssembly 프로젝트를 생성합니다. 그리고 다음 NuGet 패키지들을 설치해야 합니다.

  * `Microsoft.AspNetCore.SignalR.Client`: SignalR 허브와 통신하기 위해 필요합니다.
  * `Microsoft.AspNetCore.Components.WebAssembly.Authentication`: JWT 기반 인증 상태를 관리하기 위해 필요합니다.

<!-- end list -->

```bash
dotnet new blazorwasm -n WaitingQueue.Client
cd WaitingQueue.Client
dotnet add package Microsoft.AspNetCore.SignalR.Client
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Authentication
```

-----

### 2\. `appsettings.json` 설정

`wwwroot` 폴더에 `appsettings.json` 파일을 만들고 백엔드 API 서버의 주소를 설정합니다.

**`wwwroot/appsettings.json`**

```json
{
  "ApiBaseUrl": "https://localhost:7123" // 본인 환경의 ASP.NET Core 서버 주소로 변경
}
```

-----

### 3\. 인증 서비스 구현

JWT 토큰을 관리하고 사용자의 인증 상태를 유지하는 서비스를 구현합니다.

**`Auth/ApiAuthenticationStateProvider.cs`**

```csharp
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WaitingQueue.Client.Auth;

// JWT 토큰을 분석하여 사용자의 인증 상태를 알려주는 클래스
public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Lazy<Task<ClaimsPrincipal>> _anonymousUser;

    public ApiAuthenticationStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _anonymousUser = new Lazy<Task<ClaimsPrincipal>>(() => Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity())));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // 로컬 스토리지에서 토큰을 가져옴
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(await _anonymousUser.Value);
            }

            // 토큰이 있으면 ClaimsPrincipal을 생성하여 인증된 사용자로 설정
            var claims = ParseClaimsFromJwt(token);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(await _anonymousUser.Value);
        }
    }

    // 토큰을 받아 인증 상태를 업데이트하고 로컬 스토리지에 저장
    public async Task MarkUserAsAuthenticated(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", token);
    }

    // 로그아웃 처리 및 로컬 스토리지에서 토큰 제거
    public async Task MarkUserAsLoggedOut()
    {
        var authState = Task.FromResult(new AuthenticationState(await _anonymousUser.Value));
        NotifyAuthenticationStateChanged(authState);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
    }

    // JWT 토큰의 payload를 파싱하여 클레임(사용자 정보) 목록을 반환
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out var userId);
            if (userId != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()!));
            }
        }
        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
```

-----

### 4\. SignalR 서비스 구현

백엔드의 `QueueHub`와 통신을 담당하는 서비스를 구현합니다.

**`Services/QueueHubService.cs`**

```csharp
using Microsoft.AspNetCore.SignalR.Client;

namespace WaitingQueue.Client.Services;

public class QueueHubService : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    public event Action<string, object> OnQueueUpdate;

    public QueueHubService(IConfiguration config)
    {
        var apiBaseUrl = config["ApiBaseUrl"];
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{apiBaseUrl}/queuehub")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            // 'queue-ready', 'queue-update' 등 서버에서 보내는 이벤트를 수신
            _hubConnection.On<object>("queue-ready", (data) => OnQueueUpdate?.Invoke("queue-ready", data));
            _hubConnection.On<object>("queue-update", (data) => OnQueueUpdate?.Invoke("queue-update", data));
            _hubConnection.On<object>("queue-joined", (data) => OnQueueUpdate?.Invoke("queue-joined", data));
            _hubConnection.On<object>("queue-left", (data) => OnQueueUpdate?.Invoke("queue-left", data));

            await _hubConnection.StartAsync();
        }
    }

    // 서버의 JoinQueue 메서드를 호출하여 특정 그룹에 조인
    public async Task JoinQueue(string userId)
    {
        await _hubConnection.InvokeAsync("JoinQueue", userId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
```

-----

### 5\. `Program.cs` DI 설정

`Program.cs` 파일에 `HttpClient`, 인증 서비스, SignalR 서비스를 등록합니다.

**`Program.cs`**

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WaitingQueue.Client;
using WaitingQueue.Client.Auth;
using WaitingQueue.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient 등록 (API 서버와 통신용)
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!) });

// 인증 관련 서비스 등록
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

// SignalR 허브 서비스 등록
builder.Services.AddSingleton<QueueHubService>();

await builder.Build().RunAsync();
```

-----

### 6\. UI 컴포넌트 작성

사용자가 상호작용할 UI를 만듭니다.

**`Pages/Index.razor`**

```razor
@page "/"
@inject HttpClient Http
@inject QueueHubService QueueHub
@inject ApiAuthenticationStateProvider AuthStateProvider
@using System.Text.Json
@using System.Text

<PageTitle>Waiting Queue</PageTitle>

<h1>Waiting Queue System</h1>

<div class="card my-4">
    <div class="card-body">
        <h5 class="card-title">Join Queue</h5>
        @if (string.IsNullOrEmpty(AccessToken))
        {
            <div class="input-group">
                <input @bind="userId" class="form-control" placeholder="Enter Your User ID" />
                <button @onclick="HandleJoinQueue" class="btn btn-primary" disabled="@isConnecting">
                    @if (isConnecting)
                    {
                        <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                    }
                    else
                    {
                        <span>Join Queue</span>
                    }
                </button>
            </div>
        }
        else
        {
            <p class="text-success">You are already active!</p>
        }
    </div>
</div>

<div class="card my-4">
    <div class="card-body">
        <h5 class="card-title">Status</h5>
        <p>@statusMessage</p>
        @if (!string.IsNullOrEmpty(AccessToken))
        {
            <div class="alert alert-success">
                <strong>Access Token:</strong> <small class="text-break">@AccessToken</small>
            </div>
            <button @onclick="VerifyAccess" class="btn btn-info">Verify Access Token</button>
            <button @onclick="Logout" class="btn btn-secondary">Logout</button>
        }
        @if (verificationResult != null)
        {
            <div class="alert @(verificationResult.Value ? "alert-success" : "alert-danger") mt-3">
                Verification Result: @(verificationResult.Value ? "Valid" : "Invalid")
            </div>
        }
    </div>
</div>

@code {
    private string? userId;
    private string statusMessage = "Enter your ID and join the queue.";
    private string? AccessToken;
    private bool? verificationResult;
    private bool isConnecting = false;

    protected override async Task OnInitializedAsync()
    {
        // SignalR 허브 서비스 이벤트 핸들러 등록
        QueueHub.OnQueueUpdate += HandleQueueUpdate;
        await QueueHub.StartAsync();
    }

    // 대기열 참여 처리
    private async Task HandleJoinQueue()
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            statusMessage = "Please enter a User ID.";
            return;
        }

        isConnecting = true;
        statusMessage = "Connecting to queue...";

        await QueueHub.JoinQueue(userId);

        var response = await Http.PostAsync("api/queue/join",
            new StringContent(JsonSerializer.Serialize(new { userId }), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            // 응답 처리 로직은 HandleQueueUpdate에서 담당
        }
        else
        {
            statusMessage = $"Error joining queue: {response.ReasonPhrase}";
        }
        isConnecting = false;
    }

    // SignalR 허브로부터 받은 메시지 처리
    private void HandleQueueUpdate(string eventName, object data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        switch (eventName)
        {
            case "queue-joined":
                var joinedData = JsonSerializer.Deserialize<JsonElement>(json);
                var position = joinedData.GetProperty("data").GetProperty("position").GetInt32();
                statusMessage = $"Successfully joined queue! Your position is {position}.";
                break;

            case "queue-update":
                var updateData = JsonSerializer.Deserialize<JsonElement>(json);
                var newPosition = updateData.GetProperty("position").GetInt32();
                statusMessage = $"Your queue position has been updated. You are now at position {newPosition}.";
                break;

            case "queue-ready":
                var readyData = JsonSerializer.Deserialize<JsonElement>(json);
                AccessToken = readyData.GetProperty("accessToken").GetString();
                statusMessage = "It's your turn! You can now access the service.";
                // 인증 상태 업데이트
                InvokeAsync(() => AuthStateProvider.MarkUserAsAuthenticated(AccessToken!));
                break;
        }
        StateHasChanged(); // UI 갱신
    }

    // 접근 토큰 유효성 검사
    private async Task VerifyAccess()
    {
        if (string.IsNullOrEmpty(AccessToken)) return;

        Http.DefaultRequestHeaders.Authorization = new("Bearer", AccessToken);
        var response = await Http.PostAsync("api/queue/verify", null);

        verificationResult = response.IsSuccessStatusCode;
    }

    // 로그아웃
    private async Task Logout()
    {
        AccessToken = null;
        userId = null;
        verificationResult = null;
        statusMessage = "You have been logged out.";
        await AuthStateProvider.MarkUserAsLoggedOut();
    }
}
```

### 7\. 실행 및 테스트

1.  **백엔드 서버 실행**: 먼저 ASP.NET Core Web API 프로젝트를 실행합니다.
2.  **프론트엔드 앱 실행**: Blazor 프로젝트를 실행합니다 (`dotnet run`).
3.  브라우저에서 Blazor 앱에 접속하여 사용자 ID를 입력하고 'Join Queue' 버튼을 클릭합니다.
4.  백엔드 `QueueProcessor`의 설정에 따라(기본 5초) 대기열이 처리되면서, 화면의 상태 메시지가 실시간으로 변경되는 것을 확인합니다.
5.  순서가 되면 액세스 토큰이 화면에 나타나고, 'Verify Access' 버튼으로 토큰의 유효성을 테스트할 수 있습니다.

이 Blazor 애플리케이션을 통해 직접 구현하신 대기열 서버의 모든 기능을 효과적으로 테스트하고 시각적으로 확인할 수 있습니다.