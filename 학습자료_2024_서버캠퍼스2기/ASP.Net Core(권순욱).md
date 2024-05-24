# ASP.Net Core
컴투스 서버 캠퍼스 2기 1 ~ 2주차에 진행하였던 ASP.Net Core에 대해 학습한 내용을 정리한 문서입니다.





  
  
## 목차
1. [ASP.Net Core?]
2. [ASP.Net Core에 대하여]
   * [Program 시작]
   * [Builder]
   * [Routing]
   * [Configuration]
   * [미들웨어]
3. [DI(의존성 주입)]
4. [비동기 제어 Async, Await]
5. [sqlKata]
6. [CloudStructures]




  
---
# [ASP.Net Core?]

ASP.Net Core는 .Net 플랫폼에서 웹 애플리케이션을 구축하기 위한 최신 고성능 웹 개발 프레임워크.
C#으로 API 서버를 개발하기위해 주로 사용되며 아래와 같은 특징을 가졌다.

  
* 오픈 소스 소프트웨어, 멀티플랫폼 지원
* .Net Core 프레임워크 기반으로 하여 빠르고 효율적인 웹 애플리케이션 개발 지원
* 모듈식 아키텍처를 가지고 있어 필요한 기능한 선택하여 사용가능
* 인증, 캐싱, 라우팅, 데이터 액세스 등 웹 애플리케이션 개발에 필요한 다양한 기능 제공
---
# [ASP.Net Core에 대하여]

## [Program 시작]

* ASP.Net Core의 프로그램 진입점
* ASP.Net Core 6버전 부터 main 함수를 선언할 필요 없이 '최상위 문' 기능을 사용하여 스크립트 언어처럼 사용할 수 있다.
* DI를 위한 인터페이스 및 클래스 등록
* 라우팅 및 사용자 정의 미들웨어 등록


-> ASP.Net Core 빈 프로젝트 program.cs

  
```
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
```

1. WebApplication.CreateBuilder(args)
   WebApplicationBuilder 인스턴스를 생성한다. 해당 인스턴스를 통해 ASP.Net Core 애플리케이션을 구성한다.
2. builder.Build()
   앞에서 생성한 builder 인스턴스를 통해 ASP.Net Core 애플리케이션을 빌드한다.  
3. app.MapGet("/", () => "Hello World!")
   지정된 경로(URL)에 따라 GET 요청을 처리하는 엔드포인트를 매핑한다. 해당 애플리케이션에 루트 경로("/")로 요청을 보내면
   "Hello World!" 문자열을 반환한다.
4. app.Run()
   앞의 과정에서 설정된 ASP.Net Core 애플리케이션을 실제로 실행한다.



## [Builder]

ASP.Net Core에서 웹 애플리케이션을 구성하고 빌드하는 데 사용되는 주요 클래스. Builder 클래스를 사용하여 서비스, 미들웨어, 라우팅 등을 등록하고 애플리케이션의 동작 방식을 정의 할 수 있다.

-> builder를 통해 애플리케이션 구성하기
```
var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddSingleton<IMyService, MyService>();

// 미들웨어 등록
builder.UseMiddleware<MyMiddleware>();

// 라우팅 설정
builder.AddControllers();

// 호스트 구성
builder.Host.UseEnvironment("Development");

// 구성이 완료된 builder 객체를 통해 Build
var app = builder.Build();
```

* 서비스 등록: Services 속성을 사용하여 애플리케이션에서 사용할 서비스를 등록할 수 있다. 서비스는 인터페이스와 구현 클래스를 사용하여 등록한다.
* 미들웨어 등록: UseMiddleware() 메서드를 사용하여 HTTP 요청 파이프라인에 사용자 정의 미들웨어를 추가 할 수 있다.
* 라우팅 설정: AddControllers() 또는 MapGet() 등의 메서드를 사용하여 엔드포인트를 라우팅할 수 있다.
* 호스트 구성: Host 속성을 사용하여 호스트 설정을 구성할 수 있다. 여기에는 포트번호 환경 변수 등이 포함된다.
* 애플리케이션 빌드: Build() 메서드를 사용하여 WebApplication 인스턴스를 빌드한다.



## [Routing]

라우팅은 들어오는 HTTP 요청을 적절한 컨트롤러 작업이나 미들웨어에 매핑하는 프로세스이다. 이를 통해 애플리케이션은 다양한 요청을 처리하고 사용자에게 올바른 응답을 제공 할 수 있다.

-> 컨트롤러 선언 후 연결하기 MyContoller.cs
```
[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    readonly ILogger<LoginController> _logger;
    readonly IGameService _gameService;

    public LoginController() 
    {
        _logger = logger;
        _gameService = gameService;
    }

    // 유저 정보가 있다면 같이 동봉해서 보내고, 없다면 신규 유저이므로 새로 만들어서 전달
    [HttpPost]
    public async Task<LoginRes> Login([FromBody] LoginReq req)
    {
        // Login Logic ~


        // 로그인 성공 시 게임 데이터 로드
        res.UserGameData = await _gameService.LoadGameData(req.AccountId);

        return res;
    }

}


```

### [컨트롤러]
컨트롤러는 MVC 패턴에서 사용자의 입력을 처리하는 로직을 실행한다. 이번 ASP.Net Core에서 MVC 패턴은 주된 학습 목표가 아니기 때문에 컨트롤러를 통해 사용자의 요청에 따라 다른 로직을 실행한다는 것 정도만 꼭 기억해두자.  


  
-> LoginController  

  
* '[Route("[controller]")]' : 이 속성은 컨트롤러가 라우팅되는 경로를 의미한다. 컨트롤러의 이름이 'LoginController'인 경우 앞 글자만 따서 '/Login' 으로 라우팅 경로를 지정한다.
* '[HttpPost]' : 이 속성은 해당 메서드가 HTTP POST 요청을 처리하도록 지정함을 의미한다. 위의 LoginContoller에는 'Login' 하나의 메서드만 존재하므로 '서버주소/Login'으로 들어오는 모든 HTTP POST 요청을 해당 메서드가 처리하게 된다.

  
  
이런식으로 선언한 컨트롤러는 '[ApiController]' 속성을 통해 ASP.Net Core에서 관리하는 컨트롤러로 취급되어 builder.AddController() 메서드를 호출하는 시점에 라우팅 경로가 매핑된다. 




  



# [Configuration]
Configuration을 통해 ASP.Net Core의 애플리케이션의 동작 방식을 구성하는 데 사용할 수 있는 설정 값들을 관리할 수 있다.

  
* Connection String - 데이터베이스, 메시지 큐 및 기타 외부 서비스에 대한 연결 정보
* 애플리케이션 설정 - 캐싱, 로깅, 보안 등과 같은 애플리케이션의 동작 방식을 제어하는 값
* 환경별 설정 값 - 개발, 테스트 및 프로덕션 환경에 따라 애플리케이션의 동작을 조정



-> appsettings.json
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mydatabase;User Id=sa;Password=mypassword;"
  },
  "Logging": {
    "LogLevel": "Information"
  }
}
```


  

-> GetSection을 이용해 값 가져오기


  
```
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

var configuration = builder.Configuration;

var connectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
Console.WriteLine($"Connection string: {connectionString}");

var logLevel = configuration.GetSection("Logging")["LogLevel"];
Console.WriteLine($"Log level: {logLevel}");

```


DB 연결 문자열과 로그 레벨 정보를 가져와서 실제 ASP.Net Core 애플리케이션에 적용하고 사용할 수 있다.




 # [미들웨어]



ASP.NET Core 미들웨어는 요청 처리 파이프라인에 연결되는 소프트웨어 구성 요소이다. 각 미들웨어는 요청을 처리하거나 파이프라인에서 다음 미들웨어로 전달하기 전에 작업을 수행할 수 있다.

미들웨어를 사용하면 다음과 같은 작업을 수행할 수 있다.

* 요청 인증 및 권한 부여: 사용자가 요청에 대한 권한이 있는지 확인합니다.
* 캐싱: 요청 결과를 캐싱하여 성능을 향상시킵니다.
* 로그 기록 및 추적: 요청 및 응답에 대한 정보를 기록합니다.
* 요청 및 응답 변환: 요청 및 응답 데이터를 원하는 형식으로 변환합니다.
* 정적 파일 제공: 정적 파일(HTML, CSS, JavaScript 등)을 제공합니다.
* 오류 처리: 오류가 발생하면 적절한 오류 메시지를 반환합니다.



![middleware-pipeline](https://github.com/ks-wook/com2us_omok_server/assets/76806695/9186d5cf-ec8b-4bf6-b022-6cca26e6480b)



ASP.Net Core의 미들웨어 실행 순서는 위의 이미지와 같다. 가장 마지막에 사용자 정의 미들웨어를 실행하게 되어있어 해당 부분에서 요청에 대한 로그나 사용자 인증 등의 기능을 수행하는 것이 바람직하다.


-> 사용자 정의 미들웨어


  
```
public class MyMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // 요청 처리 전 작업 수행

        await next.InvokeAsync(context);

        // 요청 처리 후 작업 수행
    }
}
```

  
-> 미들웨어 등록

  
```
app.UseMiddleware<MyMiddleware>();
```




  
# [DI(의존성 주입)]

의존성 주입은 객체간 결합을 느슨하게 하고 코드 테스트를 용이하게 하기위해 사용하는 기능이다. 의존성 주입을 사용하게 되면 객체 사용시에 필요한 다른 객체들을 직접 생성하는 것이 아니라 객체 생성자를 통해 외부에서 주입받는다.


의존성 주입을 사용하는 이유?
  
* 결합 감소: 객체 간 결합을 느슨하게 하여 코드를 더 유연하고 유지 관리하기 쉽다.
* 테스트 용이성: 객체를 직접 생성하지 않기 때문에 단위 테스트를 작성하기 쉽다.
* 재사용성: 생성된 객체를 여러 곳에서 재사용할 수 있다.
* 변경 관리: 코드를 변경할 때 영향을 받는 객체 수를 줄인다.


의존성 주입 방식
1. Singleton - 애플리케이션 실행 기간 동안 단 하나의 인스턴스만 생성되고 모든 요청에서 공유된다.
   장점: 성능 향상, 특히 값 비싼 리소스(예: 데이터베이스 연결) 사용 시 유용
   단점: 인스턴스 상태 변경 시 모든 요청에 영향을 미칠 수 있다.
2. Transient - 요청마다 새 인스턴스가 생성.
   장점: 요청 간 상태 독립성 보장, 특히 스레드 안전성이 중요한 경우 유용
   단점: 성능 저하 가능성이 있음, 특히 값 비싼 리소스 사용 시.
3. Scoped - HTTP 요청 동안 단 하나의 인스턴스만 생성되고 해당 요청의 모든 컨트롤러 및 미들웨어에서 공유
   장점: 요청 범위 내에서 상태 유지 가능, 특히 웹 애플리케이션에서 사용자 세션 관리에 유용
   단점: 요청 간 공유 불가능, 성능 저하 가능성






-> 의존성 주입 예제


```
services.AddSingleton<IMyService, MyService>();
services.AddTransient<IMyService, MyService>();
services.AddScoped<IMyService, MyService>();
```





# [비동기 제어 Async, Await]

비동기 함수의 제어를 위해 async, await 키워드를 사용하기도한다. asp.net core에서는 특히나 async, await가 비동기 프로그래밍을 위해 많이 쓰인다. async, await를 통해 I/O 함수들을 처리하는 동안 응답 속도를 향상시킬 수 있다.


1. async 키워드:
* 메서드 선언에 사용하여 비동기 메서드임을 나타낸다.
* 이 메서드는 Task 객체를 반환하며, 이 객체는 비동기 작업의 완료를 나타낸다.
* async 메서드는 await 키워드를 사용하여 다른 비동기 작업을 기다릴 수 있다.

2. await 키워드:
* 비동기 작업이 완료될 때까지 현재 메서드 실행을 일시 중단하고 제어권을 반환한다.
* await는 Task 객체 또는 Task을 반환하는 표현식에 사용된다.
* await를 사용하면 비동기 작업이 완료될 때까지 다른 작업을 수행할 수 있다.
  



-> async, await 예제


  
```
public async Task<IActionResult> Index() // 비동기(즉시 반환) 함수
{
    var data = await _myService.GetDataAsync(); // 비동기 작업 기다림
    // ...
    return View(data);
}
```


- async 및 await의 장점:
1) 응답성 향상: I/O 작업 동안 다른 작업 수행 가능, 응답 속도 향상
2) 코드 가독성 개선: 비동기 작업 순서 명확하게 표현, 코드 이해 용이
3) 스레드 풀 효율성 증대: 스레드 차단 최소화, 리소스 활용도 향상
4) 유지 관리 용이: 비동기 코드 테스트 및 디버깅 용이


  
- async 및 await 사용 시 주의 사항:

1) async 메서드는 void를 반환할 수 없음, 반드시 Task 또는 Task을 반환하는 값을 가진 형식을 반환해야 함
2) await는 async 메서드 내에서만 사용 가능
3) await는 Task 객체 또는 Task을 반환하는 표현식에만 사용 가능






  


# [sqlKata]

SQLKata는 MySQL과 같은 관계형 데이터베이스와 상호 작용하는 데 사용되는 강력하고 간편한 SQL 쿼리 생성 도구이다.

SQLKata를 사용하는 이유?
* 간결하고 표현력 있는 쿼리 작성: LINQ와 유사한 구문을 사용하여 쿼리를 작성하여 코드 가독성을 높이고 유지 관리에 용이
* 쿼리 파라미터화: SQL 주입 공격을 방지하고 코드 보안을 강화하기 위해 쿼리에 매개변수를 쉽게 바인딩할 수 있다.
* 다양한 데이터베이스 지원: MySQL, PostgreSQL, SQL Server 등 다양한 데이터베이스와 호환


```
public async Task<ErrorCode> InsertGameResult(PKTInnerReqSaveGameResult packet)
{
    // string -> Int62 parsing
    long blackUserId = long.Parse(packet.BlackUserId);
    long whiteUserId = long.Parse(packet.WhiteUserId);
    long winUserId = long.Parse(packet.WinUserId);

    // 게임 결과 데이터 저장
    var insertSuccess = await _queryFactory.Query("game_result")
            .InsertAsync(new
            {
                black_user_id = blackUserId,
                white_user_id = whiteUserId,
                win_user_id = winUserId,
            });

    // 저장 실패
    if (insertSuccess != 1)
    {
        return ErrorCode.FailInsertGameResult;
    }

    return ErrorCode.None;
}
```





# [CloudStructures]

CloudStructures는 Redis에 접근하기 쉽게 만들어주는 오픈 소스 라이브러리이다.



  
CloudStructures의 장점?
* 개발 생산성 향상: Redis와의 상호 작용을 단순화하여 개발 시간을 단축하고 코드 가독성을 높인다.
* 오류 감소: 객체 매핑 및 자동 직렬화/역직렬화를 통해 오류 가능성을 줄인다.
* 유지 관리 용이성: LINQ 및 기타 C# 기능을 활용하여 코드를 보다 유지 관리하기 쉽게 만든다.
* 확장성: 다양한 데이터 구조 및 기능을 지원하여 다양한 요구 사항에 맞게 사용할 수 있다.


```
// Redis에 문자열 값 저장
var redis = new RedisConnection("localhost");
await redis.SetAsync("my-string", "Hello, World!");

// Redis에 객체 저장
var product = new Product { Id = 1, Name = "Product 1", Price = 10.99m };
await redis.SetAsync<Product>("product:1", product);
```
