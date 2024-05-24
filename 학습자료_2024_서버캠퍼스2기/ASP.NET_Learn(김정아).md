# ASP.NET 이론

### .NET / .NET Core

- **.NET 프레임워크** → 윈도우에서 동작
    - 마이크로소프트에서 개발한 플랫폼
    - 여러 언어들을 사용해서 소프트웨어를 만들고 동작시킬 수 있게 도와준다
    - 이 프레임워크는 코드실행, 메모리 관리, DB 접근 등을 쉽게 처리하게 해준다
    - **언어가 다른 환경에서 개발이 되더라도 동일하게 실행 가능하게 한다**
    
    <img src='https://drive.google.com/uc?export=download&id=1FRPj0F3wRmUGR_NOYiX_Kda3pE1MeYWq' width="300" /><br>
    
    1. C#, F# 등 다양한 언어로 코딩
    2. 컴파일러가 번역
    3. 기계가 이해할 수 있는 중간단계인 CIL로 변환 
    4. CIL이 동작할 수 있는 환경인 CLR
    5. CIL과 CLR을 포함한 소프트웨어 = 닷넷 프레임 워크
    
- **.NET Core** → 윈도우, 맥, 리눅스 등 다 가능한 환경

**⇒ .NET Framework는 윈도우만 가능 / .NET Core는 크로스 플랫폼** 

---

### ASP.NET Core 미들웨어

- 미들웨어 : 요청 및 응답을 처리하는 앱 파이프라인으로 어셈블리되는 소프트웨어
    
    **→ 클라이언트와 서버 사이에서(Middle) 요청이나 응답을 처리하면서 다양한 작업을 하는 역할**
    
    > 클라이언트가 서버에 요청을 보내면, 미들웨어는 그 요청을 가로채서 필요한 작업을 처리한 뒤에 다음 단계로 넘기거나 응답을 만들어서 클라이언트에게 보내는 역할을 한다.
    > 
- 미들웨어들은 ASP.NET Core 애플리케이션의 **`Startup.cs`** 파일에서 설정한다
    
    ![Untitled](https://drive.google.com/uc?export=download&id=1lU7PuObMvJAWByUS5VPAYnqNYs3r0qOD)
    
- **`Configure`** 메서드 안에 `app.Use`를 사용하여 미들웨어를 추가하면 순서에 따라 각 미들웨어가 실행된다
- 미들웨어의 구성 요소
    1. **요청의 전달 및 선택:**
        - 클라이언트가 서버에 요청을 보내면, 미들웨어는 이 요청을 파이프라인 상의 다음 구성 요소로 전달할지 여부를 결정 → 어떤 미들웨어가 요청을 처리할지 선택
    2. **작업 수행:**
        - 미들웨어는 파이프라인 상의 다음 구성 요소로 요청을 전달하기 전/후에 작업을 수행할 수 있음
        - 요청을 가로채서 수정하거나, 특정 조건에 따라 요청을 거부하거나, 로그를 남기는 등 다양한 작업 수행 가능
    3. **요청 대리자:**
        - 미들웨어들을 조합하여 HTTP 요청을 처리하는 역할
        - **Run / Map / Use 확장 메서드**를 사용하여 구성
            - 개별 요청 대리자 : 무명 메서드로 인라인에서 지정되거나 다시 사용할 수 있는 클래스에서 정의될 수 있다. 이러한 **다시 사용할 수 있는** 클래스 및 인라인 무명 메서드를 미들웨어 / 미들웨어 구성 요소 라고 함
        - 요청 대리자가 요청 파이프라인을 구성하면, 각 미들웨어가 요청을 받아 처리하고 다음 미들웨어로 전달함
        - 요청 파이프라인의 각 미들웨어 구성 요소는 파이프라인의 그 다음 구성 요소를 호출하거나 파이프라인을 단락(short-circuiting)하는 역할을 담당합니다. 미들웨어가 단락(short-circuit)되는 경우 미들웨어에서 더는 요청을 처리하지 못하도록 하기 때문에 이를 *터미널 미들웨어*라고 합니다.
    
    → 이러한 미들웨어들이 모여서 파이프라인을 형성
    
    **→ 미들웨어는 각 파이프라인 사이에서 일하는 일꾼같은 느낌** 🐝
    

---

### RESTful API

- REST ( **Re**presentational **S**tate **T**ransfer )
    - **HTTP를 잘 사용하기 위해 정한 규칙**
    - API 작동 방식에 조건을 부과하는 소프트웨어 아키텍처
    - REST 기반 아키텍처를 쓰면 대규모의 고성능 통신을 안정적으로 지원 가능
    - 쉽게 구현하고 수정 가능 → 여러 플랫폼에서 사용
- API ( **A**pplication **P**rogramming **I**nterface)
    - 다른 소프트웨어 시스템과 통신하기 위해 따라야 하는 규칙을 정의함
    - 프로그램들 간에 소통할 수 있게 하는 규약
    - **응용 프로그램이 운영 체제나 프로그래밍 언어가 제공하는 기능을 쓸 수 있게 만든 인터페이스**
    - **프로그램들 간의 소통 창구 다리**

**→ RESTful API : REST 규칙을 잘 지켜서 만든 API**

>📏 **HTTP 메서드**
>
>- GET : 데이터 검색
>- POST : 새 데이터 항목 만들기
>- PUT : 데이터 항목 업데이트
>- PATCH : 항목을 수정하는 방법에 대한 지침을 설명하는 방식 = 업데이트
>- DELETE : 데이터 항목 삭제


- 안전 + 신뢰 가능
- 장점
    - 확장성 : REST가 클라-서버 상호작용을 최적화하기 때문에 효율적으로 확장 가능
    - 유연성 : 클라-서버가 분리되어있기 때문에 서버를 바꿔도 클라에 영향이 가지 않아서 유연하게 변경 가능
    - 독립성 : API 설계에 영향을 주지 않고 다양한 언어로 클라 + 서버 어플리케이션 모두 작성 가능

> ✨ REST 아키텍처 스타일 원칙
>
>1. Client-Server 구조
>    1. 클라와 서버는 서로 독립적이어야한다 → 독립적으로 개발되거나 대체될 수 있게 유지해야함
>    2. 클라는 URIs 리소스만 알아야한다
>2. Stateless (무상태성)
>    1. 클라의 모든 요청에는 그 요청을 이해할 수 있는 모든 정보가 포함되어야함
>    2. 서버는 HTTP 요청에 대한 어떤 것도 저장 X
>3. Cacheable
>    1. 서버는 Response cache-control 헤더에 해당 요청이 캐싱이 가능한지에 대한 여부를 제공해야함
>4. Uniform Interface
>    - REST에서 정의한 4가지 인터페이스 규칙
>        1. 요청 시 개별 자원을 식별할 수 있어야 함
>        2. 어떤 자원에 작업하기 위한 적절한 표현 + 메타데이터가 충분하다면 서버는 해당 자원을 변경, 삭제할 수 있는 정보를 가지고 있다는 의미
>        3. 자신을 어떻게 처리해야하는지 정보를 포함해야함
>        4. 결과뿐 아니라 결과에 대한 정보를 포함해야함
>5. Layered System (다중 계층)
>    1. REST는 다중 계층 구조를 가질 수 있음 → API 서버, DB 서버, 인증 서버 따로 가능
>    2. 각 레이어는 자기와 통신하는 컴포넌트 말고 다른 레이어에 대해서는 정보를 얻을 수 없음
>    3. 클라는 REST 서버와만 상호작용 / 다른 레이어들에 직접 요청 X 상호작용도 못봄
>6. Code on demand (이건 옵션)
>    1. 서버가 클라에서 실행이 가능한 로직을 전송해서 클라의 기능 확장 가능


---

### URL / URI

>✨ Resource의 위치까지만 있는 주소 = URL / + 로 뭔가 더 있으면 URI


- URI가 큰 개념 → URL과 URN을 포함하고 있음
- **URL** ( Uniform Resource **Locator** )
    - Resource의 정확한 **위치 정보**를 나타낸다
    
    > https://www.naver.com
    > 
    - 구조
    
    | 이름 | 설명 | 예시 |
    | --- | --- | --- |
    | scheme | 통신 프로토콜 | http://, https:// |
    | host | 위치한 웹 서버의 도메인이나 IP | www.naver.com, 127.0.0.1 |
    | :port | 접속하기 위한 포트번호 | :80, :3000 |
    | /path | 루트 디렉토리로부터 원하는 파일이 위치한 경로 | /search, /Users/username/Desktop |
    | ?query | 리소스의 형식 범위를 좁히기 위한 추가 질문 (key = value 형식) | q=JavaScript |
    | #fragment | URL이 지정하는 자원의 세부 부분 지정 | #section1 |
- **URN** (Uniform Resource **Name** )
    - Resource의 위치와 상관없이 식별 가능한 **고유한 이름** 역할
    - URL의 한계로 생긴 것이라 아직 URL이 더 대중화
        - URL의 한계 = 리소스의 위치를 옮겨버리면 원래 URL을 더 이상 사용 못함
- **URL** ( Uniform Resource **Identifier** )
    - Resource의 위치뿐만 아니라 + 고유 식별자
    - 인터넷에서 요구되는 기본조건
    - 인터넷 프로토콜에 항상 붙어있음

---

### ASP.NET Core에 API를 만들때의 이점

- 동일한 프레임워크와 패턴을 사용해서 웹 페이지 + 서비스 모두 빌드 가능
1. 간단한 직렬화
    - ASP.NET은 최신 웹 환경을 위해 설계되었기 때문에 엔드포인트는 즉시 JSON으로 클래스를 직렬화함
2. 인증 및 권한 부여
    - 보안을 위해 JWT를 기본적으로 지원함
3. 코드와 함께 라우팅
    - 코드와 함께 인라인으로 경로와 동사 정의 가능
4. 기본적으로 HTTPS
    - 기본적으로 HTTPS를 지원함

---

### ControllerBase 클래스

- 일반적으로 웹 API 컨트롤러를 만들때 Controller가 아닌 ControllerBase를 상속받아서 만들어야한다

→ 왜? = API 요청/응답 웹 페이지만을 만들려면 Base로 해야 한다

1. MVC와 API 컨트롤러의 분리
    - Base를 상속받아야 MVC와 API Controller의 역할이 분명하게 나누어진다
    - MVC 컨트롤러처럼 HTML **뷰를 렌더링하는 웹 응용 프로그램이 아니고** 데이터 처리 + API 응답 생성하는 역할을 수행한다
2. API 응답 생성
    - ControllerBase는 다양한 API 응답을 위한 메서드를 제공한다
        
       
        >🎃 ControllerBase에서 제공하는 메서드 예시
        >
        >- OK() : HTTP 200 OK 응답 (성공적인 API 요청)
        >- NotFound() : HTTP 400 Not Found 응답 (해당 리소스 못찾음)
        >- Json() : 주어진 객체를 JSON 형식으로 직렬화해서 반환 (API에서 응답으로 JSON형태의 데이터 전송 시)
        >- CreatedAtAction() : HTTP 201 Created + 주어진 액션 메서드의 URL 응답 (리소스 성공적으로 생성)
        >- BadRequest() : HTTP 400 Bad Request 응답 (클라이언트의 잘못된 요청)
        >- NoContent() : HTTP 204 No Content 응답 (요청은 성공적, 추가적 데이터 반환 X)
        
        
3. 기본 기능에 중점
    - API에서 필요한 기본 기능에 중점을 둘 수 있다
    - 불필요한 기능으로 인한 오버헤드 없이 API의 핵심 로직에 집중

---

### Attribute

> [ApiController] / 
> [Route("[controller]")]
>
> 와 같은 것들
> 
- 어트리뷰트 : 코드의 메타데이터를 나타내는데 사용하는 선언적 태그
- 클래스, 메서드, 속성 등 다양한 곳에 **컴파일타임이나 런타임 시에 추가 정보 제공**으로 사용 가능
- Attribute가 있으면 이 코드?내용이 어떤 용도로 사용되는지 추측 가능

---

### [ApiController]

- 어셈블리 전체에 이 어트리뷰트 적용 가능
    
    ```csharp
    [assembly: ApiController]
    ```
    
- **ApiController를 사용하면, Attribute Routing을 사용해야한다!**
    
   
    >🎲 [Route(”[controller]”)]
    
   
    
    이걸 꼭 사용해야하고, UseEndpoints, UseMvc, UseMvcWithDefaultRoute 같은 **규칙 기반 경로를 이용할 수 없음**
    
- ApiController를 컨트롤러에 적용함으로써 다양한 API 관련 동작 사용 가능
    - 모델 유효성 자동 검사
        - 자동으로 액션 메서드의 매개 변수에 대한 유효성 검사를 해주고, 오류가 발생하면 HTTP 400 응답도 자동으로 생성해준다
    - 자동 HTTP 400 응답
        - 자동으로 HTTP 400 응답을 해주기 때문에 따로 검사할 필요가 없다
    - 자동으로 해당 매개 변수를 어떤 소스에서 바인딩 해야하는지 추론해준다
        - [FromBody] 속성을 명시하지 않아도 프레임워크가 알아서 인식하고 처리해준다
    - Swagger 같은 API 도큐먼트 생성 도구에서 사용될 때 API의 문서화를 쉽게 만든다
    - 기본적인 웹 API 동작이 자동 구성되기 때문에 코드를 간소화할 수 있다
    - 기본적인 라우팅 규칙이 활성화 되어서 리소스 라우팅이 간단해진다

---

### Program.cs

웹 애플리케이션의 진입점

- 앱에서 요구하는 서비스들을 구성
- 앱의 요청 처리 파이프라인이 미들웨어 구성 요소들로 정의되어 있다

```csharp
var builder = WebApplication.CreateBuilder(args); // 의존성 주입 컨테이너 생성

// Add services to the container.
// 만든 컨테이너인 builder에 여러 서비스들 추가하기 -> 이용해서 의존성 주입

builder.Services.AddControllers() // ILogger<TCategoryName> 인스턴스를 검색해서 자동 400 응답에 대한 정보를 기록하는 방법
.ConfigureApiBehaviorOptions(options =>
{
    var builtInFactory = options.InvalidModelStateResponseFactory;

    options.InvalidModelStateResponseFactory = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        return builtInFactory(context);
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.Add~ 한 것들이 모두 컨테이너에 서비스들을 추가한것(나중에 컨테이너가 대신 만들어서 전달해줄 서비스들)(의존성 주입)

var app = builder.Build(); // 의존성 주입 컨테이너에 등록된 서비스들을 포함한 WebApplication을 생성해서 app 변수에 담음

// Configure the HTTP request pipeline.
// HTTP 요청 파이프라인 구성

// app.Use~ 메서드들을 호출해서 이 애플리케이션의 요청 처리 파이프라인에 ~미들웨어를 추가하는 것
if (app.Environment.IsDevelopment()) // 지금이 개발환경이면?
{
    // Swagger 쓰기 위해서 미들웨어 추가
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // HTTP 요청을 HTTPS로 리다이렉션해서 보안을 강화 -> HTTP로 요청이 들어오면 HTTPS로 리다이렉션

app.UseAuthorization(); // 인증 및 권한 부여 기능 활성화 -> 요청 처리 전 인증/권한 부여 확인해줌

app.MapControllers(); // 컨트롤러를 요청에 매핑 -> 요청이 들어온 URL과 그 URL에 해당하는 컨트롤러 + 액션 메서드를 호출해서 요청을 처리하게 해줌

app.Run(); // 요청 처리 파이프라인의 마지막 -> 전에 설정된 미들웨어들에서 처리되지 않은 모든 요청들에 대한 응답을 함
```

- 의존성 주입 컨테이너 생성
    
    ```csharp
    var builder = WebApplication.CreateBuilder(args);
    ```
    
    - CreateBuilder 메서드로 builder라는 변수에 의존성 주입 컨테이너 만들어서 할당

    <br>
    
    >🎁 의존성 주입?
    >
    >어떤 객체가 필요로 하는 다른 객체를 직접 만들어서 쓰는게 아니고 외부에서 제공받아서(주입) 사용하는 디자인 패턴
    >
    >- 의존성 주입을 사용하지 않은 경우
    >    
    >    ```csharp
    >    public class 주문서비스
    >    {
    >        private 결제서비스 결제서비스 = new 결제서비스();
    >    
    >        public void 주문하기()
    >        {
    >            // 주문 로직
    >            결제서비스.결제하기();
    >        }
    >    }
    >    ```
    >    
    >- 의존성 주입을 사용한 경우
    >    
    >    ```csharp
    >    public class 주문서비스
    >    {
    >        private readonly I결제서비스 결제서비스;
    >    
    >        public 주문서비스(I결제서비스 결제서비스)
    >        {
    >            this.결제서비스 = 결제서비스;
    >        }
    >    
    >        public void 주문하기()
    >        {
    >            // 주문 로직
    >            결제서비스.결제하기();
    >        }
    >    }
    >    ```
    >    
    >    → 의존성 주입을 사용한 경우에는, I결제서비스라는 인터페이스형 객체가 변수로 선언되어 있고, 생성자에서 이 결제서비스를 외부에서 제공받아서 변수에 할당한다!!      >    → 이러면 다양한 종류의 결제서비스(I결제서비스 인터페이스를 이용해서 구현한)를 사용할 수 있어서 코드의 유연성도 확보가능

    
- 만든 의존성 주입 컨테이너에 서비스들 추가
    - 나중에 이 컨테이너를 이용해서 다른 객체들이 의존성을 주입받을텐데, 그 때 주입할 서비스들을 추가
    
    ```csharp
    builder.Services.Add~
    ```
    
    - AddControllers, AddSwagger 등 원하는 서비스들을 추가
- 웹 어플리케이션 생성
    
    ```csharp
    var app = builder.Build();
    ```
    
    - 의존성 주입 컨테이너에 등록한 서비스들을 포함한 WebApplication 객체를 만들어서 app 변수에 할당
    - 이제 이 app을 이용해서 HTTP 요청을 받고, 응답을 처리하는 등 다양한 작업 수행 가능
- HTTP 요청 파이프라인 구성 / 미들웨어 추가
    
    ```csharp
    app.Use~
    ```
    
    - 만든 app 변수에 Use~메서드들을 사용해서 요청 처리 파이프라인에 미들웨어들 추가

    <br>
    
    >🔦 **app.UseSwagger()?**
    >
    >Swagger이라는 개발할때 API 문서를 생성하고 브라우저에서 볼 수 있는 UI를 제공하는 미들웨어 → 개발할때만 사용하기 때문에 if문에서 개발중이니? 체크하고 미들웨어를 추가했다
    >

    >🧷 **app.Run()?**
    >
    >**요청 처리 파이프라인의 마지막에 위치하는 미들웨어**
    > 
    >이전의 미들웨어들이 처리하지 않고 **남은 모든 요청들을 응답**하는 미들웨어
    >
    >() 로 하면 아무런 응답없이 지나감
    >
    >그래서! ( ) 사이에 내가 원하는 람다식이라던가, 기능을 넣어서 위에서 처리하지 않은 요청들에 default로 응답할 내용을 넣어서 사용한다
    

### 종속성 주입

- 서비스를 직접 클래스 내에서 만들어서 쓰는게 아니고 밖에서 제공받아서 사용하는 것
- 내부에 서비스 변수 선언(인터페이스) → 생성자에서 매개변수로 받아서 해당 변수에 서비스 할당
- 인터페이스 만들어서 기능 넣어놓고, 서비스들마다 해당하는 인터페이스를 상속받아서 구현 → 여러 종류의 서비스를 종속성 주입 가능
    
    ```csharp
    public interface IMyDependency
    {
        void WriteMessage(string message);
    }
    
    public class MyDependency : IMyDependency
    {
        public void WriteMessage(string message)
        {
            Console.WriteLine($"MyDependency.WriteMessage Message: {message}");
        }
    }
    
    public class Index2Model : PageModel
    {
        private readonly IMyDependency _myDependency;
    
        public Index2Model(IMyDependency myDependency)
        {
            _myDependency = myDependency;            
        }
    
        public void OnGet()
        {
            _myDependency.WriteMessage("Index2Model.OnGet");
        }
    }
    ```
    
    - IMyDependency를 상속받은 여러 종류의 서비스들을 종속성 주입 가능
    - 직접 MyDependency 인스턴스를 만들지 않고 외부에서 DI 컨테이너가 만들어서 넣어준다

---

### 서비스 수명

<img src='https://drive.google.com/uc?export=download&id=1MqeNJ9y_cQOqmBDGhRdLQ6AwtOdieT91' width="800" /><br>

- Transient → 매번 다른 GUID / Scoped → Request 마다 다른 GUID / Singleton → 접속을 다시 해야지만 달라진 GUID
- **Transient** : 일회용
    
    ```csharp
    builder.Services.AddTransient<서비스가상속받은인터페이스,수명을설정할서비스>();
    ```
    
    - **호출될 때마다 새로운 인스턴스로 생성**
    - 매번 다른 인스턴스 사용 → 요청 시마다 새걸로 만들어줌
    - 가벼운 서비스 / 임시 작업 수행 시 사용 → 상태를 저장하지 않는 인스턴스에 사용
- **Scoped** : 범위 지정 → 요청/범위에 따라 공유
    
    ```csharp
    builder.Services.AddScoped<서비스가상속받은인터페이스,수명을설정할서비스>();
    ```
    
    - 각 HTTP **요청이나 범위에 따라 새로운 인스턴스 생성**
    - 그 **범위나 요청 내에서는 같은 인스턴스 사용**(일회용 X) → 요청 내에서 상태를 유지해야 할 때 사용
- **Singleton** : 단일체
    
    ```csharp
    builder.Services.AddSingleton<서비스가상속받은인터페이스,수명을설정할서비스>();
    ```
    
    - **응용 프로그램의 수명 동안 하나의 인스턴스만 생성**되고 이걸 공유해서 사용
    - 이 서비스를 요청할 때마다 같은 인스턴스를 반환해줌
    - 응용 프로그램 전체에서 공유해야하는 상태나 기능을 가진 서비스에 사용 → 앱이 종료될때까지 유지해야 할 때 사용
 
---

**Stateful / Stateless**

- Stateful
    - 서버가 클라이언트의 상태를 계속 기억하고 유지 → 연결 유지?
    - 클라이언트의 로그인 정보, 세션, 현재 상태 등이 서버에 저장되고 관리
    - 클라-서버 간 **지속적인 연결** 필요 → 상태 정보를 저장하고 업데이트하기 위해 많은 리소스 필요할 수 있음 + 확장이 어려움
 

- Stateless
    - 클라이언트의 요청들이 각각 독립적 + 이전 요청들이랑 상관 없음
    - 스테이지 클리어, 아이템 사용 등 **필요시에만 서버에 연결**해서 데이터를 전달하고 결과를 반영함
    - 서버는 이 요청들을 다 독립적으로 처리 → 비동기?
    - 요청들을 처리할때 요청 사이에 서버와 클라 연결이 끊어져도 상관 없음
    - 확장성이 좋음 + 서버 리소스를 효율적으로 사용 가능
    - 장애 발생 시 장애가 발생한 서버를 간단하게 대체 가능

<br>

**왜 소켓 서버 아니고 웹 API 서버를 쓸까?**

- 웹 API는 Stateless
- 소켓 서버는 Stateful
- 실시간 게임이라면? 서버가 클라의 게임 상태를 계속 유지 + 관리해야함 → stateless라면 그때마다 연결하고, 인증하고.. 등 이런걸 계속 해야함 → 구려! → Stateful한 소켓 서버를 사용해서 클라와의 연결을 유지하는게 낫다
- 실시간 아니고 싱글 게임이나, 그냥 게임하고 결과를 등록하거나 갱신하는 게임이라면? 굳이 서버와 클라가 계속 연결되어있을 필요 없음 
