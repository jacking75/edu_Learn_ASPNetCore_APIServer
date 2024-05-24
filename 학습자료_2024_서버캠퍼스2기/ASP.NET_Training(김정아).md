# ASP.NET 실습

## 목표 : ASP.NET Core 웹 API 컨트롤러와 .NET 및 C#을 사용한 플랫폼 간 RESTful 서비스 작성 살펴보기

<aside>
🍗 따라하기

[ASP.NET Core 컨트롤러를 사용하여 웹 API 만들기 - Training](https://learn.microsoft.com/ko-kr/training/modules/build-web-api-aspnet-core/)

</aside>

### 환경 세팅

1. .NET 8.0 버전 다운로드
    - 윈도우
        
        [.NET 8.0 SDK (v8.0.101) - Windows x64 Installer 다운로드](https://dotnet.microsoft.com/ko-kr/download/dotnet/thank-you/sdk-8.0.101-windows-x64-installer)
        
    - 맥
        
        [.NET 다운로드(Linux, macOS 및 Windows)](https://dotnet.microsoft.com/ko-kr/download)
        
2. VS Code와 Extension 설치
    - C# Dev Kit
        
        <img src='https://drive.google.com/uc?export=download&id=1qy1AMChjflxZCtTqY2atHZvN2XNjOwIh' width="500" /><br>
        
    - REST Client
        
        <img src='https://drive.google.com/uc?export=download&id=1AHlcS2MLkigFGH4OPpmGslzuonda54D8' width="500" /><br>
3. 웹 API 프로젝트 만들기 (.NET 8.0 SDK 사용)
    1. 터미널에서 설치 확인
        
        ```cli
        dotnet --list-sdks
        ```
        
        <img src='https://drive.google.com/uc?export=download&id=1Cd3LlY61XP0MhGPb9G-bQZGAAa-_XYLz' width="500" /><br>
        
        - 8로 시작하는 버전 있는지 확인
        
    2. VS Code 이용해서 프로젝트 만들기
    3. ContosoPizza 라는 폴더 만들고 폴더 선택하기
    4. VS Code에서 터미널 아래에 열고 아래 명령 입력
        
        ```cli
        dotnet new webapi -controllers -f net8.0
        ```
        
        - 날씨 예측 목록을 반환하는 *ContosoPizza.csproj*라는 C# 프로젝트 파일과 함께 컨트롤러를 사용하는 기본 웹 API 프로젝트용 파일을 만듬
            
            <img src='https://drive.google.com/uc?export=download&id=1N9ZZ3862Q4ShmvTS9ndVkaUSqWKv2TlI' width="300" /><br>
        - Controllers/ : HTTP 엔드포인트로 노출되는 공용 메서드가 있는 클래스를 포함
        - Program.cs : 애플리케이션의 관리되는 진입점을 포함하는 서비스와 앱의 HTTP 요청 파이프라인을 구성
        - ContosPizzd.csproj : 프로젝트에 대한 구성 메타데이터 포함
        - ContosoPizza.http : VS Code에서 직접 REST API를 테스트하는 구성 포함
        
        
       >🪓 워크로드를 확인하라는 노란색 경고문이 뜸
       >```cli
       >dotnet workload update
       >```
       >명령을 치니까 관리자 권한으로 해야한다고 안됨        
       >→ 맥에서 관리자 권한으로 터미널 열기 ( sudo -i 입력하고 비밀번호 치고 들어가기 )       
       >관리자 권한에서 명령 입력      
       >⇒ 워크로드 업데이트 완료
        
        
        
4. 웹 API 빌드 및 테스트
    1. 명령 쉘에서 .NET Core CLI 명령 실행
        
        ```cli
        dotnet run
        ```        
        <img src='https://drive.google.com/uc?export=download&id=1iiBHMvcFPVs1Y3lSBRjw5Xk4t1-ZLe6f' width="500" /><br>
        
        - 현재 디렉토리에 있는 프로젝트 파일 찾기
        - 이 프로젝트에 필요한 종속성을 검색해서 설치
        - 프로젝트 코드 컴파일
        - HTTP / HTTPS 엔드포인트 모두에서 ASP.NET Core의 Kestel 웹 서버를 이용해서 웹 API 호스팅
       <br>
       
          >🪓 HTTP는 5000 ~ 5300, HTTPS는 7000 ~ 7300 사이의 포트가 선택된다
          >이 포트를 변경하려면 프로젝트의 launchSettings.json 파일에서 쉽게 변경 가능

        
    
    2. 웹 브라우저를 열고 URI로 이동
    
        > http://localhost:5119/weatherforecast
    
        <img src='https://drive.google.com/uc?export=download&id=1hN1kCZwjruJVG-UfAba0RllWUM8G_2Uo' width="500" /><br>

    
5. .http 파일 이용해서 테스트
    1. ContosoPizza.http 파일 열기
    2. GET 위의 Send Request 클릭
        
        <img src='https://drive.google.com/uc?export=download&id=1TWzm2_G5cDi9T3ddtXKshMnlDbeEQtdo' width="500" /><br>
        
    3. 브라우저에서 본 것과 비슷한 출력이 있는 창이 열림
        
        <img src='https://drive.google.com/uc?export=download&id=1q3Uz8pwO_Q1eHBy-MAhXdREYqMYgRTox' width="500" /><br>
        

---

## 웹 API 컨트롤러

### WeatherController 코드 분석

```csharp
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers; // Web API 컨트롤러 및 작업 메서드 구성에 사용할 수 있는 attribute를 제공해주는 네임스페이스

[ApiController] // attribute -> 이제부터 ApiController다
[Route("[controller]")]
public class WeatherForecastController : ControllerBase // Controller가 아닌 ControllerBase 상속받기
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")] 
    // Get 메서드에 대한 attribute -> HTTP Get 요청을 IEnumerable<WeatherForecast> Get() 메서드로 라우팅해줌
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
```

- API 컨트롤러 클래스 특성
    
    ```csharp
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    ```
    
    - [ApiController] : 웹 API를 쉽게 빌드할 수 있게 해준다
    - [Route(”[controller]”)] : 라우팅 패턴 [controller]를 정의
        - 컨트롤러의 이름이 [controller] 토큰을 바꾼다
            
            **→ 이 컨트롤러의 이름이 WeatherForecastController 니까 저 [ ] 안이 WeatherForecastController 로 바뀐다는 뜻**
            
        - 이제 이 컨트롤러는 `https://localhost:{port}/weatherforecast` 에 대한 요청을 처리한다 **(컨트롤러 이름 대소문자 구분 없애고, controller 접미사가 없어진다)**
- Get() 메서드 라우팅 + 메서드 내용
    
    ```csharp
    [HttpGet(Name = "GetWeatherForecast")] 
    public IEnumerable<WeatherForecast> Get()
    {
    	...
    }
    ```
    
    - [HttpGet(Name = "GetWeatherForecast")] : 위에서 지정한 URI로 들어온 **HTTP Get 요청을** 아래의 Inumerable<WeatherForecast> **Get() 메서드로 들어오도록 라우팅**한다
    - **`[ApiController]`** 어트리뷰트를 사용한 컨트롤러는 주로 하나의 액션 메서드로 하나의 HTTP 동작을 처리하게 된다
        
        → 각각의 액션 메서드가 특정 HTTP 메서드(GET, POST, PUT, DELETE 등)에 대응하여 동작하도록 구현되기 때문
        
    
    >🙉 하나의 경로 URL에서도 Get, Post, Delete 등 다양한 종류의 요청이 있을 수 있기 때문에 컨트롤러 내부에 각각의 메서드를 만들어줘야한다

    

---

## 데이터 저장소 추가

### 피자 모델 만들기

1. Models 폴더 생성 → MVC 아키텍처에서 따온 이름
   ```cli
    mkdir Models
    ```
3. Pizza.cs 파일 생성
  
    
4. Pizza.cs 내용 작성
    - 피자를 정의하는 클래스

   <br>
    
    ```csharp
    namespace ContosoPizza.Models; // ContosoPizza.Models 라는 네임스페이스 정의
    
    public class Pizza // Pizza 클래스 정의
    {
        public int ID { get; set; } // 정수형 ID 속성
        public string? Name { get; set; } // 문자열 Name 속성 -> ? 는 이 속성이 null 일 수 있다는 뜻
        public bool IsGlutenFree { get; set; } // bool형 IsGlutenFree 속성
    }
    ```
    
    <br>
    
    >🌭 string?의 ?
    >
    >**해당 변수가 null 값을 가질 수 있다! 가져도 된다**! → 컴파일러가 해당 변수가 null 일 때의 가능성을 인식하게 한다 → NullReferenceException 오류 사저너 방지 가능
    >
    >C# 8.0버전부터 도입된 “nullable reference types”
    >
    >원래 C#에서는 참조 타입 변수는 null 값을 가질 수 있긴 한데, 코드에서 발생 할 수 있는 null 관련 오류를 줄이려고 도입되었다
    >
    

---

### 데이터 서비스 추가

1. Services 폴더 생성
    
    ```cli
    mkdir Services
    ```
    
2. PizzaService.cs 파일 생성
  
    
3. PizzaService.cs 내용 작성
    - 메모리 내 피자 데이터 서비스를 만든다
    - 기본적으로 2개의 피자가 있는 캐시 제공 (생성자에서 List에 피자 2개를 만들어서 초기화하기 때문)
    - 웹 API를 중지하고 다시 시작하면 생성자때문에 다시 두 개의 기본 피자로 설정된다

    <br>

    ```csharp
    using ContosoPizza.Models; // 전에 만든 Model 네임스페이스 가져와서 사용
    
    namespace ContosoPizza.Services; // ContosoPizza.Services 라는 네임스페이스 정의
    
    public static class PizzaService // PizzaService 클래스 정의 -> static으로 선언해서 단일한 인스턴스로 공유하게 + static class 라서 멤버들도 다 static 이어야한다
    {
        static List<Pizza> Pizzas { get; } // 피자 리스트
        static int nextId = 3; // 피자 2개까지 처음에 만들어두니까 다음 피자 id는 3으로 미리 지정
        static PizzaService() // 정적 생성자 -> 이 정적 클래스 인스턴스를 처음 사용할 때 호출
        {
            Pizzas = new List<Pizza> // 피자 리스트 초기화
            {
                new Pizza {Id = 1, Name = "Classic Italian", IsGlutenFree = false}, // 1번 피자 내용
                new Pizza {Id = 2, Name = "Veggie", IsGlutenFree = true} // 2번 피자 내용
            };
        }
        public static List<Pizza> GetAll() => Pizzas; // 피자 리스트의 모든 피자를 얻어오는 메소드
        public static Pizza? Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id); // LINQ 쿼리를 이용해서 원하는 id의 피자 찾아서 반환
        public static void Add(Pizza pizza) // 피자 추가 메소드
        {
            pizza.Id = nextId++; // 현재 nextId 값을 먼저 매개변수 pizza의 Id 에 할당하고, 그 후에 nextId를 1 증가시킴
            Pizzas.Add(pizza); // 피자 리스트에 새로 매개변수로 받은 pizza 추가
        }
        public static void Delete(int id) // 피자 삭제 메소드
        {
            var pizza = Get(id); // var 형식의 pizza 변수에 매개변수로 받은 id로 해당 피자 찾아서 넣기
            if(pizza is null)
            {
                return; // 피자가 null이면 삭제 할 필요 없으니까 return
            }
    
            Pizzas.Remove(pizza); // 피자 리스트에서 해당 피자 삭제
        }
        public static void Update(Pizza pizza) // 피자 내용 업데이트 메소드
        {
            var index = Pizzas.FindIndex(p=>p.Id == pizza.Id); // 매개변수로 받은 피자의 id 사용해서 피자 리스트에서 인덱스 몇인지 찾기
            // FindIndex 메소드는 못 찾으면 -1 을 반환한다
            if(index == -1) // 인덱스가 -1 이면 없는 거니까 업데이트 안하고 return
            {
                return;
            }
    
            Pizzas[index] = pizza; // 해당 인덱스의 피자를 새로 변경할 피자로 수정하기
        }
    }
    ```
    
    <br>
    
    >🍤 **static class?**
    >
    >정적 클래스 = 하나만 존재! 인스턴스화 해서 쓰는 클래스가 아니고 그냥 이 클래스 이름으로 접근해서 **인스턴스화 안하고** 사용 가능
    >
    >C++ 에는 없음
    >
    >싱글턴이랑 비슷! 
    >→ 싱글턴 : 이 객체는 하나만 만들고 우리 공유해서 쓰자~
    >→ static class : 이 객체는 하나만 있어! 다 이거 참조해서 사용해야해!
    >
    >static class 내부의 모든 멤버 변수, 멤버 메소드는 static 이다

    <br>
    
    >🥨 **var** 형식 변수
    >
    >C++의 auto와 동일
    >
    >변수의 형식을 컴파일러가 알아서 추론하라고 하기
    >
    >Get 메소드를 사용했을때 반환값이 Pizza? 이기 때문에 Pizza일지, null 일지 몰라서 사용
    >
    >FindIndex를 했을 때 찾아서 Pizza를 반환할지, 못찾아서 -1 을 반환할지 몰라서 사용
    
    

---

### 웹 API 프로젝트 빌드

```cli
dotnet build
```

  <img src='https://drive.google.com/uc?export=download&id=1w6kkffbQex8z6wpNy_axC1cdQ27N2-EM' width="400" /><br>

- 빌드 성공

---

## 컨트롤러 추가

### 컨트롤러 만들기

1. Controllers 폴더에 PizzaController.cs 파일 생성 → MVC 아키텍처에서 따온 이름
  
    
2. PizzaController.cs 내용 작성
    - 컨트롤러 기본 틀 작성

    <br>

    ```csharp
    using System.Reflection.Metadata;
    using ContosoPizza.Models; // 만든 ContosoPizza.Models 네임스페이스 가져와서 사용
    using ContosoPizza.Services; // 만든 Services 네임스페이스 가져와서 사용
    using Microsoft.AspNetCore.Mvc; // Microsoft.AspNetCore.Mvc 네임스페이스 사용 -> MVC 패턴 구현을 위해 사용
    
    namespace ContosoPizza.Controllers; // 네임스페이스 정의
    
    [ApiController] // ApiController attribute 사용
    [Route("[controller]")] // ApiController attribute 사용하니까 Attribute Routing 써주기
    // 클래스 이름에서 대소문자 무시 + 접미사 Controller 제거 = https://localhost:{port}/pizza 로 들어오는 요청 처리
    public class PizzaController : ControllerBase // controller 클래스 정의
    {
        public PizzaController() // 생성자
        {
            
        }
    }
    ```

<br>

>💫 **CRUD**
>
>| HTTP 동작 동사 | CRUD 작업 | ASP.NET Core 특성 |
>| --- | --- | --- |
>| GET | 읽기 | [HttpGet] |
>| POST | 생성 | [HttpPost] |
>| PUT | 주어진 경로에 리소스 저장 = 리소스 업데이트 또는 생성 | [HttpPut] |
>| PATCH | 리소스 일부 업데이트 (PUT은 전체 업데이트) | [HttpPatch] |
>| DELETE | 삭제 | [HttpDelete] |

---

### GET

### 모든 피자 Get하기

- 첫번째로 구현해야할 REST 동사는 클라이언트가 API에서 모든 피자를 가져올 수 있게 하는 `GET`
- **[HttpGet] 특성 사용**해서 메서드를 정의
- PizzaController 클래스 내부에 Get 메서드 정의
    
    ```csharp
    [HttpGet] // 이 아래의 메서드는 Http Get 동사에 응답
    public ActionResult<List<Pizza>> GetAll() => PizzaService.GetAll(); 
    // List<Pizza> 형태의 ActionResult 인스턴스를 반환 -> PizzaService에서 구현한 GetAll 메서드 사용
    ```
    
    - PizzaService는 static class 이므로 인스턴스화 하지않고 바로 클래스이름으로 내부 메서드(GetAll) 사용 가능

    <br>

    >🥞 **ActionResult?**
    >
    >ASP.NET Core에서 사용하는 MVC 패턴에서 Controller의 액션 메서드가 클라이언트에게 반환할 결과
    >
    >위의 GetAll 메서드는 PizzaService.GetAll()을 호출해서 List<Pizza> 형태의 데이터를 가져오고, 이 **데이터를 ActionResult<List<Pizza>> 형태로 다시 감싸서 클라이언트에게 반환함**
    

### 단일 피자 찾아서 Get하기

- 클라이언트가 전체 목록이 아니라 특정 피자에 대한 정보를 요청할 수도 있음 → 피자를 특정하기 위해 **id를 매개 변수로 받는** 다른 `GET` 구현
- **[HttpGet(”{id}”)] 특성 사용**해서 메서드를 정의
- 동작 결과 종류
    - Ok (200)
        - 받은 Id 매개변수와 일치하는 피자가 있다! → 응답 본문에 요청한 데이터 포함
    - NotFound (404)
        - 받은 Id 매개변수와 일치하는 피자가 없다
- PizzaController 클래스 내부에 Get 메서드 정의
    
    ```csharp
    [HttpGet("{id}")] // Get 동사에 응답 + ("{id}") 있으니까 클래스 선언 위에서 정의한 Route + /id 가 매개변수로 와야한다
    public ActionResult<Pizza> Get(int id) // URL로 전달받은 매개변수 id를 이용해서 특정 피자를 얻어오는 Get 메서드
    {
        var pizza = PizzaService.Get(id); // PizzaService Static class에서 정의한 Get 메서드 사용해서 id가 일치하는 Pizza를 받아서 변수에 담는다
    
        if(pizza == null) // 만약 받아온 변수가 null이면? -> 해당하는 피자가 없기 때문에 못찾음
        {
            return NotFound(); // 없으니까 반환할거 없어서 NotFound 반환
        }
    
        return pizza; // 찾은 피자를 ActionResult에 담아서 반환
    }
    ```
    
    
    >🍿 **메서드에 명시된 반환값**과 **실제 return하는 자료형이 다르다**?
    >
    >메서드에서는 ActionResult<Pizza>를 반환한다고 되어있는데 실제 메서드 내부에서 return 되는 값은 pizza 인데?!(var 형태의 변수에 담겨있지만 할당 받는 값은 pizza?이다) + NotFound도 마찬가지! ActionResult<Pizza>가 아니잖아?!
    >
    >**→ 결론 : ASP.NET Core가 자동으로 적절하게 바꿔준다!**
    >
    >실제 동작 : Pizza객체를 반환하면 ASP.NET Core가 이 받은 값을 OkObjectResult<Pizza>로 감싸서 HTTP 응답으로 보냄 → 이 OkObjectResult<Pizza>는 사실 ActionResult<Pizza>의 하위 클래스! HTTP 상태 코드를 200 (OK)로 설정하고 이 객체를 응답함
    
    
    

### 새 컨트롤러 빌드 및 실행

```cli
dotnet run
```

### .http 파일을 사용해서 컨트롤러 테스트

1. .http 파일 열기
2. 전체 가져오는 GET
    
    ```csharp
    GET {{ContosoPizza_HostAddress}}/pizza/
    Accept: application/json
    ```
    
    <img src='https://drive.google.com/uc?export=download&id=1BsPC0sR30qBCvHHofB2r8WcHMWGB4zn0' width="300" /><br>
    
    - 모든 피자 목록 반환
    
3. id를 매개변수로 받아서 특정 피자 가져오는 GET
    
    ```csharp
    GET {{ContosoPizza_HostAddress}}/pizza/1
    Accept: application/json
    ```
    
    <img src='https://drive.google.com/uc?export=download&id=17XQOn6OHmcDUJGCemeOk_3dql8XxD6IB' width="300" /><br>
    
    - id가 1인 피자만 반환
    
4. 없는 항목을 GET
    
    ```csharp
    GET {{ContosoPizza_HostAddress}}/pizza/5
    Accept: application/json
    ```
    
    <img src='https://drive.google.com/uc?export=download&id=1WyzJH70nJ_WCREWqeJ7t1TZjCGqK4-6j' width="300" /><br>
    
    - id가 5인 피자는 없어서 Not Found 반환
    
---
### POST

### 피자 추가

- **[HttpPost] 특성 사용**해서 메서드 구현
- GET에서는 피자 목록이나 특정 피자를 반환했지만 POST에서는 `IActionResult` 응답을 반환한다
- `IActionResult`는 클라이언트가 요청이 성공했는지 알 수 있게 해주고, 새로 만든 피자의 ID를 제공함
- 동작 결과 종류
    - CreatedAtAction (201)
        - 피자가 잘 추가되었다
    - BadRequest (400)
        - 요청의 pizza 개체가 뭔가 잘못되었다
- PizzaController 클래스 내부에 Create 메서드 정의
    
    ```csharp
    [HttpPost] // Http Post 동사에 응답
    public IActionResult Create(Pizza pizza) // 매개변수로 받은 피자를 생성하는 메서드 + 새로 생성한 객체의 위치를 클라이언트에게 반환
    {
        PizzaService.Add(pizza); // 매개변수로 받은 피자를 피자 리스트에 추가해서 내부 저장소에 저장
        return CreatedAtAction(nameof(Get), new { id = pizza.Id }, pizza); // HTTP Created(201) 응답 반환 + CreatedAtAction 메서드로 클라이언트에게 새로 생성된 리소스의 위치를 알려줌
        // CreatedAtAction이 해당 Get 액션 메서드로 이동할 수 있는 URL을 생성해서 반환해줌
    }
    ```
    
    
    >🍮 **CreatedAtAction**
    >
    >(string actionName, object routeValues, object value);
    >
    >  - actionName : URL을 생성할 액션 메서드의 이름 / 보통 ‘nameof()’를 사용해서 액션 메서드의 이름을 문자열로 전달해서 사용함
    >  - routeValues : 액션 메서드에 전달될 route 값
    >  - value : 응답으로 클라이언트에게 반환할 객체
    >    
    > → POST 요청이 성공해서 새 리소스가 생기면, 해당 리소스에 대한 Get URL을 생성해서 클라이언트에게 반환하기 때문에 클라이언트가 이 URL로 새로운 리소스에 접근이 가능하다
    >
    >>🎣 왜 2번째 매개변수가 new { id = pizza.Id } ?
    >>
    >>피자를 새로 한개 만들었기 때문에 여기로 접근하는 Get URL을 만들어주려면 특정 피자에 Get 하기때문에 매개변수로 Id가 필요하다 → new로 익명 객체를 만들어서 그 route 값에 새로 만들어진 pizza의 Id값을 할당해준것
    
    
---
### PUT

### 피자 수정

- **[HttpPut] 특성 사용**해서 메서드 구현
- POST와 유사하지만 업데이트해야 하는 피자의 id + 업데이트 될 새 모습의 Pizza 개체가 필요함
- 동작 결과 종류
    - NoContent (204)
        - 피자가 잘 업데이트 되었다
    - BadRequest (400)
        - 매개변수로 받은 Id 값이 경로의 Id 값과 일치하지 않는다 / 요청의 pizza 개체가 뭔가 잘못되었다
- PizzaController 클래스 내부에 Update 메서드 정의
    
    ```csharp
    [HttpPut("{id}")] // HTTP Put 동사에 응답 + URL에 원래 Route /id 가 와야한다
    public IActionResult Update(int id, Pizza pizza) // 매개변수로 받은 id로 피자를 식별하고, 매개변수로 받은 피자로 업데이트 하는 메서드
    {
        if(id != pizza.Id) // 식별자로 받은 id와 업데이트할 피자의 id가 다르면 -> 잘못된 요청
        {
            return BadRequest(); // BadRequest 응답하기
        }
    
        var existingPizza = PizzaService.Get(id); // PizzaService static class의 Get에 id를 매개변수로 넣어서 특정 피자 객체 가져와서 var 변수에 담기
        if(existingPizza is null) // 가져온 피자가 null이면 -> 피자가 없다
        {
            return NotFound(); // 해당 피자 없으니까 NotFound 응답하기
        }
    
        PizzaService.Update(pizza); // 위의 상황을 모두 지났으면 피자 식별 + 피자 존재하므로 원하는 새 피자의 id로 가서 그 피자를 새로운 피자로 업데이트하기
    
        return NoContent(); // 성공적으로 업데이트해서 추가적으로 반환할 정보가 필요 없으니까 NoContent 응답 -> 업데이트가 성공적으로 이루어졌다는 뜻
    }
    ```
    
---
### DELETE

### 피자 제거

- **[HttpDelete] 특성 사용**해서 메서드 구현
- 어떤 객체를 지울지 특정해야하기 때문에 [HttpDelete(”{id}”] 처럼 매개변수를 전달받아야한다
- 동작 결과 종류
    - NoContent (204)
        - 피자가 잘 삭제되었다
    - NotFound (404)
        - 매개변수로 받은 Id 값에 해당하는 피자가 존재하지 않는다

```csharp
[HttpDelete("{id}")] // HTTP Delete 동사에 응답 + URL에 원래 Route /id가 와야한다
public IActionResult Delete(int id) // 매개변수로 받은 id로 식별한 특정 피자를 삭제하는 메서드
{
    var pizza = PizzaService.Get(id); // 매개변수로 받은 id로 피자를 가져와서 var 변수에 할당

    if(pizza is null) // 변수가 null이면 -> 피자가 없으면
    {
        return NotFound(); // 없으니까 NotFound 응답
    }

    PizzaService.Delete(id); // null이 아닌거니까 피자는 있으므로 해당 id의 피자 삭제

    return NoContent(); // 성공해서 추가로 반환할 정보가 필요 없으므로 NoContent 응답 -> Delete가 성공했다는 뜻
}
```

### 완료된 웹 API 빌드 및 실행

```cli
dotnet run
```

### .http 파일을 사용하여 완성된 웹 API 테스트

1. ContosoPizza.http 파일 열기
2. 새로운 피자 POST
    
    ```csharp
    POST {{ContosoPizza_HostAddress}}/pizza/
    Content-Type: application/json
    
    {
        "name":"Hawaii",
        "isGlutenFree":false
    }
    
    ###
    ```
    
    <img src='https://drive.google.com/uc?export=download&id=1lTUbX3KEiB78oM6Y9ztd8y4w832AzlJG' width="200" /><br>

    
    >🛎️ 꼭 Content-Type: ~ 입력 후 json 을 body에 입력 할때 한줄 띄기
    
    
3. 만든 피자를 PUT으로 업데이트하기
    
    ```csharp
    PUT {{ContosoPizza_HostAddress}}/pizza/3
    Content-Type: application/json
    
    {
        "id":3,
        "name":"Hawaiian",
        "isGlutenFree":false
    }
    
    ###
    ```
    
    - id가 3인 피자를 넣은 json 내용으로 수정하기
    
        <img src='https://drive.google.com/uc?export=download&id=1kZFnSjTcdM7n_9MvJP5AlCvw9Jj5BVve' width="300" /><br>
    
    - PUT이 잘 동작해서 No Content라 응답 내용은 없다
    
    - 잘 PUT된건지 GET으로 확인
        
        <img src='https://drive.google.com/uc?export=download&id=1X9yMh0gHSfypeYkfB0g4xUAlADUriJ5P' width="300" /><br>
        
        - 3번 피자 이름이 Hawaiian으로 바뀐 모습
        
4. 만든 피자 DELETE 하기
    
    ```csharp
    DELETE {{ContosoPizza_HostAddress}}/pizza/3
    
    ###
    ```
    
      <img src='https://drive.google.com/uc?export=download&id=1JzcJK9nA0R_Sbehq1_ZXso_jB6RWJOWz' width="300" /><br>
    
    - 잘 DELETE 되어서 No Content 응답 내용 없음
    
    - 잘 DELETE된건지 GET으로 확인
        
        <img src='https://drive.google.com/uc?export=download&id=1cVEMBG0dPVJUL0DWOBrDNcfKWu6bE58d' width="300" /><br>
        - 3번 피자가 삭제된 모습
