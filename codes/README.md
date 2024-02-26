# API 게임 서버 
  
## API 서버간 통신 때 HttpClientFactory 사용하기
[MS Docs: ASP.NET Core에서 IHttpClientFactory를 사용하여 HTTP 요청 만들기](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0 )  
  
```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("Google", httpClient =>{
    httpClient.BaseAddress = new Uri("https://www.google.com/");
});
builder.Services.AddHttpClient("BaiDu", httpClient =>{
    httpClient.BaseAddress = new Uri("https://www.baidu.com/");
});
```
  
```
interface IGoogleService{
    Task<string> GetContentAsync();
}
class GoogleService : IGoogleService{
    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
	
    public async Task<string> GetContentAsync()
    {
        var googleClient = _httpClientFactory.CreateClient("Google");
		var response = await googleClient.GetAsync("some-endpoint");
		...
    }
}
```  
    
    
IHttpClientFactory는 ASP.NET Core에서 HTTP 클라이언트를 관리하는 데 많은 장점을 제공한다.  
- 클라이언트 인스턴스 풀링: IHttpClientFactory는 HttpClient 인스턴스를 풀링하여 재사용합니다. 이로 인해 매 요청마다 새로운 HttpClient를 생성하는 비용을 줄이고 성능을 향상시킵니다.
- 수명 관리: IHttpClientFactory는 HttpClient 인스턴스의 수명을 관리합니다. 요청이 완료되면 HttpClient를 반환하고 재사용할 수 있도록 합니다. 이로 인해 메모리 누수를 방지하고 리소스를 효율적으로 사용할 수 있습니다.
- 구성 가능한 클라이언트: IHttpClientFactory를 사용하면 여러 클라이언트를 구성할 수 있습니다. 위의 코드에서 “Google” 및 “BaiDu” 클라이언트를 등록했습니다. 각 클라이언트는 서로 다른 BaseAddress를 가지며 다른 서비스에 대한 요청을 보낼 수 있습니다.
- DI (의존성 주입) 지원: IHttpClientFactory는 DI 컨테이너와 통합되어 의존성 주입을 통해 HttpClient를 쉽게 주입할 수 있습니다.
- 테스트 용이성: IHttpClientFactory를 사용하면 HttpClient를 목(Mock)으로 대체하여 단위 테스트를 수행할 수 있습니다. 이는 외부 서비스에 의존하는 코드를 테스트할 때 유용합니다.
  
  
요약하자면, IHttpClientFactory는 성능, 메모리 관리, 구성 가능성 및 테스트 용이성 측면에서 HTTP 클라이언트를 효율적으로 관리하는 데 도움이 됩니다.
         
  
<br>    
  
## Poly를 사용한 재 요청 기능
시스템의 안정성과 보안을 보장하기 위해 타사 서비스를 호출할 때 재시도 및 회로 차단기를 추가할 수 있다.   
재시도는 한 번의 호출이 실패한 후 다시 시도하여 다운스트림 서비스의 일시적인 단절로 인해 모든 프로세스가 종료되는 것을 방지한다.   
회로 차단기는 과도한 무효 액세스를 방지하고 시스템에서 알 수 없는 예외가 발생하는 것을 방지하기 위한 것이다.  
Polly는 독립적인 재시도 메커니즘의 서드파티 라이브러리이다.   
  
아래 코드는 httpclient를 사용하여 하류 API에 대한 요청 시 재시도 및 회로 차단기에 대해서만 다룬다.  
NuGet 패키지 Microsoft.Extensions.Http.Polly를 가져와야 한다.  
  
```
using Polly;
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHttpClient("RetryClient", httpclient =>
    {
        httpclient.BaseAddress = new Uri("http://localhost:5258");
    })
    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3));

var app = builder.Build();

// httpclient를 호출한다
app.MapGet("/test", async (IHttpClientFactory httpClientFactory) =>
{
    try
    {
        var httpClient = httpClientFactory.CreateClient("RetryClient");
        var content = await httpClient.GetStringAsync("other-api");
        Console.WriteLine(content);
        return "ok";
    }
    catch (Exception exc)
    {
        if (!Count.Time.HasValue)
        {
            Count.Time = DateTime.Now;
        }
        return $"{exc.Message}    【횟수：{Count.I++}】  【{Count.Time.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}】";
    }
});

// 상태 코드 500을 돌려준다
app.MapGet("/other-api", (ILogger<Program> logger) =>
{
    logger.LogInformation($"실패:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}");
    return Results.StatusCode(500);
});
app.Run();

static class Count
{
    public static int I = 1;
    public static DateTime? Time;
}
```  
   
`.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryAsync(3))` 에 의해 요청에 대해 3번의 재시도가 이루어지며, 첫 번째 재시도까지 총 4번의 재시도가 이루어진다.   
다운스트림 서비스에 장애가 발생하면 이렇게 짧은 시간 내에 자동으로 복구되지 않을 수 있다. 더 좋은 방법은 재시도 횟수에 따라 요청 간 시간을 연장(무작위 또는 자체 지연 알고리즘을 구축)하는 것이다.    
```
.AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.WaitAndRetryAsync(3, retryNumber =>
        {
            switch (retryNumber)
            {
                case 1:
                    return TimeSpan.FromMilliseconds(500);
                case 2:
                    return TimeSpan.FromMilliseconds(1000);
                case 3:
                    return TimeSpan.FromMilliseconds(1500);
                default:
                    return TimeSpan.FromMilliseconds(100);
            }
        }));
```  
  
또 다른 재시도 전략을 소개한다.  
```
// 뮤한으로 재시도 한다
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.RetryForeverAsync());
// 2초 마다 재시도 한다
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryForeverAsync(retryNumber =>
{
    Console.WriteLine(retryNumber);
    return TimeSpan.FromSeconds(2);
}));
// 5초간 4회 요청이 있고, 50%가 실패한 경우 10초간 서킷브레이커가 동작한다
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.AdvancedCircuitBreakerAsync(0.5d, TimeSpan.FromSeconds(5), 4, TimeSpan.FromSeconds(10)));
```  
  
서킷브레이커는 서비스를 보호하는 수단이다. 이 예에서 구체적인 사용 방법은 아래와 같다.  
```
builder.Services
    .AddHttpClient("RetryClient", httpclient =>
    {
        httpclient.BaseAddress = new Uri("http://localhost:5258");
    })
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.WaitAndRetryAsync(3, retryNumber =>
        {
            switch (retryNumber)
            {
                case 1:
                    return TimeSpan.FromMilliseconds(500);
                case 2:
                    return TimeSpan.FromMilliseconds(1000);
                case 3:
                    return TimeSpan.FromMilliseconds(1500);
                default:
                    return TimeSpan.FromMilliseconds(100);
            }
        }))
    // 서킷브레이커
    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(6, TimeSpan.FromSeconds(30)));
```    
CircuitBreaker는 6번의 실패한 요청이 있을 경우, 30초 동안 일시 정지를 제어한다.   
  
     
   
## RateLimit  
RateLimit은 네트워크의 기반 설비에서 설정하여 구현할 수도 있고, 게이트웨이에서 RateLimit을 할 수도 있다. 하지만 서비스 자체의 RateLimit도 빼놓을 수 없다.   
복수의 레플리카가 있는 경우 하나의 레플리카가 장애가 발생하면 다른 레플리카에 대한 트래픽이 증가하게 되고, 이것이 감당할 수 있는 요청량을 초과하면 서비스가 연쇄적으로 크래시될 수 있기 때문이다. 따라서 개별 서비스 자체적으로도 RateLimit을 구현하는 것이 바람직하다.  
  
ASP.NET Core 프로젝트에서는 AspNetCoreRateLimit을 도입하여 RateLimit 처리가 가능하다.  
아래와 같은 방법으로 NuGet 패키지를 도입할 수 있다.   
```
Install-Package AspNetCoreRateLimit
```  
  
클라이언트 RateLimit 설정:    
```
using AspNetCoreRateLimit;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// ClientRateLimiting 설정 파일을 읽는다
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));

// ClientRateLimitPolicies 설정 파일을 읽는다
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

// RateLimit 메모리캐시 서비스를 도입
builder.Services.AddInMemoryRateLimiting();

// RateLimit 설정 파일 서비스를 도입
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// ClientRateLimitPolicies 를 유효화
var clientPolicyStore = app.Services.GetRequiredService<IClientPolicyStore>();
await clientPolicyStore.SeedAsync();

// 클라이언트 RateLimit 미들웨를 사용
app.UseClientRateLimiting();

app.MapGet("/test00", () => "get test00 ok");
app.MapGet("/test01", () => "get test01 ok");
app.MapGet("/test02", () => "get test02 ok");
app.MapPost("/test02", () => "post test02 ok");

app.Run();
```    
     
appsetings.json     
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ClientRateLimiting": {
    "EnableEndpointRateLimiting": false,
    "StackBlockedRequests": false,
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "EndpointWhitelist": [ "get:/test00", "*:/test01" ],
    "ClientWhitelist": [ "dev-id-1", "dev-id-2" ],
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "5s",
        "Limit": 2
      },
      {
        "Endpoint": "*",
        "Period": "10s",
        "Limit": 3
      }
    ]  
  },   
  "ClientRateLimitPolicies": {
    "ClientRules": [
      {
        "ClientId": "client-id-1",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "5s",
            "Limit": 1
          },
          {
            "Endpoint": "*",
            "Period": "15m",
            "Limit": 200
          }
        ]
      },
      {
        "ClientId": "client-id-2",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "1s",
            "Limit": 5
          },
          {
            "Endpoint": "*",
            "Period": "15m",
            "Limit": 150
          },
          {
            "Endpoint": "*",
            "Period": "12h",
            "Limit": 500
          }
        ]
      }
    ]
  }
}
```  
    
설정 설명:   
- EnableEndpointRateLimiting이 false인 경우 모든 요청의 총 수가 임계값을 초과하면 속도 제한을 적용하고, true인 경우 각 요청이 임계 값을 초과하면 속도 제한을 적용한다.
- StackBlockedRequests가 false인 경우 이전 5초 동안 2번 성공하고 1번 실패한 경우 6초 후에 한 번 더 성공할 수 있으며, true인 경우 6초 후의 요청은 성공하지 못한다.
- ClientIdHeader는 속도 제한의 블랙/화이트 리스트를 처리하기 위해 헤더 키 X-ClientId를 지정한다.
- ClientWhitelist는 dev-id-1, dev-id-2이며, 헤더 내 X-ClientId가 이 값이면 통과시킨다.
- EndpointWhitelist는 속도 제한에 포함되지 않는 엔드포인트이다.
- HttpStatusCode는 속도 제한 후 반환되는 상태 코드이다.
- GeneralRules는 일반적인 속도 제한 규칙이다.
- ClientRateLimitPolicies 설정은 서로 다른 X-ClientId에 대해 서로 다른 속도 제한을 설정하기 위한 것으로 클라이언트 ID의 그레이리스트를 의미한다.     
    
또한, ClientID에 의한 속도 제한뿐만 아니라 클라이언트의 요청 IP에 대해서도 속도 제한을 할 수 있으며, 설정 방법은 동일하다.    
     
	 
IP Rate Limit 설정: IP Rate Limit 설정	  
```
using AspNetCoreRateLimit;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// IPRateLimiting 설정 파일을 읽는다
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// IPRateLimitPolicies 설정 파일을 읽는다
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

// RateLimit 메모리캐시 서비스를 주입
builder.Services.AddInMemoryRateLimiting();

// RateLimit 설정 파일 서비스를 주입
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();

// IPRateLimitPolicies 를 유효화
var ipPolicyStore = app.Services.GetRequiredService<IIpPolicyStore>();
await ipPolicyStore.SeedAsync();

// IP RateLimit 미들웨어를 사용
app.UseIpRateLimiting();

app.MapGet("/test00", () => "get test00 ok");
app.MapGet("/test01", () => "get test01 ok");
app.MapGet("/test02", () => "get test02 ok");
app.MapPost("/test02", () => "post test02 ok");
app.MapGet("/test03", () => "get test01 ok");

app.Run();
```  
       
appsettings.json  
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": false,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "IpWhitelist": [ "127.0.0.1"],
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "EndpointWhitelist": [ "get:/test00", "*:/test01" ],
    "ClientWhitelist": [ "dev-id-1", "dev-id-2" ],
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "5s",
        "Limit": 2
      },
      {
        "Endpoint": "*",
        "Period": "10s",
        "Limit": 3
      }
    ]  
  },
  "IpRateLimitPolicies": {
    "IpRules": [
      {
        "Ip": "127.0.0.2",
        "Rules": [
          {
            "Endpoint": "*",
            "Period": "4s",
            "Limit": 1
          },
          {
            "Endpoint": "*",
            "Period": "15m",
            "Limit": 200
          }
        ]
      }
    ]  
  }
}   
```     
   
   
##  FluentValidation: 엔티티 검증
API POST로 전송되는 데이터의 유효성을 검증하기 위해 FluentValidation(자세한 내용은 공식 웹사이트 https://fluentvalidation.net  참조)을 도입할 수 있으며, asp.net mvc에서는 모델의 유효성 검사를 사용하여, 엔티티 클래스 상에 속성을 추가하여 검증 효과를 얻고 있다.   
FluentValidation의 원리는 AbstractValidator의 구현을 통해 T 엔티티 클래스의 검증을 수행하는 것으로, T의 속성을 다양한 규칙을 통해 검증한다(더 많은 검증 규칙은 공식 웹사이트를 참조). 아래 구현을 참고한다:   
```
public class Person{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Tel { get; set; }
    public string Email { get; set; }
    public DateTime Birthday { get; set; }
    public string IDCard { get; set; }
    public PersonAddress Address { get; set; }
}
public class PersonAddress{
    public string Country { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string County { get; set; }
    public string Address { get; set; }
    public string Postcode { get; set; }
}

/// <summary>
/// Person 검증
/// </summary>
public class PersonValidator : AbstractValidator<Person>{
    public PersonValidator(IPersonService personService)
    {
        RuleFor(p => p.Name).NotNull().NotEmpty();
        RuleFor(p => p.Email).NotNull().EmailAddress();
        RuleFor(p => p.Birthday).NotNull();
        RuleFor(p => p.IDCard)
            .NotNull()
            .NotEmpty()
            .Length(18)
            .When(p => (DateTime.Now > p.Birthday.AddYears(1)))
            .WithMessage(p => $"出生日期为{p.Birthday}，现在时间为{DateTime.Now},大于一岁，CardID值必填！");
        RuleFor(p => p.Tel).NotNull().Matches(@"^(\d{3,4}-)?\d{6,8}$|^[1]+[3,4,5,8]+\d{9}$").WithMessage("电话格式为：0000-0000000或13000000000");
        RuleFor(p => p.Address).NotNull();
        RuleFor(p => p.Address).SetValidator(new PersonAddressValidator());
        //외부 메소드를 호출하여 검증한다
        RuleFor(p => p.Id).Must(id => personService.IsExist(id)).WithMessage(p => $"不存在id={p.Id}の用户");
    }
}
/// <summary>
/// Person Address 검증
/// </summary>
public class PersonAddressValidator : AbstractValidator<PersonAddress>{
    public PersonAddressValidator()
    {
        RuleFor(a => a.Country).NotNull().NotEmpty();
        RuleFor(a => a.Province).NotNull().NotEmpty();
        RuleFor(a => a.City).NotNull().NotEmpty();
        RuleFor(a => a.County).NotNull().NotEmpty();
        RuleFor(a => a.Address).NotNull().NotEmpty();
        RuleFor(a => a.Postcode).NotNull().NotEmpty().Length(6);
    }
}
```  
  
FluentValidation을 도입하는 것도 쉬운데, IValidator를 주입하여 구현할 수도 있고, AddFluentValidation으로 주입한 후 IValidatorFactory를 사용하여 Validator를 가져와 검증을 할 수도 있다. 코드는 아래와 같다:    
```
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluentValidation();
builder.Services.AddScoped<IValidator<Person>, PersonValidator>();
builder.Services.AddScoped<IPersonService, PersonService>();
var app = builder.Build();

app.MapPost("/person", async (IValidator<Person> validator, Person person) => {
     var result = await validator.ValidateAsync(person);
     if (!result.IsValid)
     {
         var errors = new StringBuilder();
         foreach (var valid in result.Errors)
         {
             errors.AppendLine(valid.ErrorMessage);
         }
         return errors.ToString();
     }
     return "OK";
});
app.MapPost("/person1", async (IValidatorFactory validatorFactory, Person person) => {
    var result = await validatorFactory.GetValidator<Person>().ValidateAsync(person);
    if (!result.IsValid)
    {
        var errors = new StringBuilder();
        foreach (var valid in result.Errors)
        {
            errors.AppendLine(valid.ErrorMessage);
        }
        return errors.ToString();
    }
    return "OK";
});
app.MapPost("/person2", async (IValidatorFactory validatorFactory, Person person) => {
    var result = await validatorFactory.GetValidator(typeof(Person)).ValidateAsync(new ValidationContext<Person>(person));
    if (!result.IsValid)
    {
        var errors = new StringBuilder();
        foreach (var valid in result.Errors)
        {
            errors.AppendLine(valid.ErrorMessage);
        }
        return errors.ToString();
    }
    return "OK";
});
app.Run();

public interface IPersonService{
    public bool IsExist(int id);
}
public class PersonService : IPersonService{
    public bool IsExist(int id)
    {
        if (DateTime.Now.Second % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
```     