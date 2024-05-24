# Web API 관련 공부 내용 정리

* Web API
  - 특징 및 목적
    1. HTTP 기반 서비스 : ASP.NET Web API는 HTTP 프로토콜을 기반으로 하는 서비스를 제공한다. 이를 통해 다양한 플랫폼 및 디바이스에서 쉽게 소비될 수 있다.
    2. RESTful 서비스 : REST (Representational State Transfer) 아키텍처 원칙을 따르며, 리소스를 고유한 URI로 식별하고 HTTP 메서드를 사용하여 리소스에 대한 작업을 정의한다. 이를 통해 간단하고 확장 가능한 서비스를 구축할 수 있습니다.
    3. 다양한 형식의 데이터 전송 : 주로 JSON 또는 XML 형식을 사용하여 데이터를 전송한다. 이는 클라이언트와 서버 간의 데이터 교환을 용이하게 만든다.
    4. 모델 바인딩 : HTTP 요청을 서비스의 메서드 매개변수에 자동으로 매핑하는 모델 바인딩을 제공한다. 이를 통해 간단한 코드로 데이터를 수신하고 처리할 수 있다.
    5. 라우팅 및 자동 HTTP 상태 코드 : ASP.NET Web API는 URL 라우팅을 지원하여 요청을 적절한 컨트롤러 및 액션으로 매핑한다. 또한, 자동으로 적절한 HTTP 상태 코드를 반환하여 클라이언트에게 응답한다.
    6. 클라이언트 지원 라이브러리 : 다양한 플랫폼 및 언어에서 Web API를 쉽게 사용할 수 있도록 하는 클라이언트 라이브러리를 제공한다.

* 즉시 로드 및 지연 로드
  - 즉시 로드 : 초기 데이터베이스의 쿼리로 관련 엔티티를 로드한다. System.Data.Entity.Include 확장 메서드를 사용한다.
  - 지연 로드 : 해당 엔터티의 탐색 속성이 역참조되면 EF에서 관련 엔터티를 자동으로 로드한다. 탐색 속성을 가상으로 만들면 지연 로드를 사용할 수 있다.
  - 즉시 로드로 인하여 EF가 매우 복잡한 조인을 생성하게 될 경우 지연 로드를 사용할 수 있다.
 
* SQLKata
  1. MySQLConnection와 MySQLCompiler 클래스를 사용하여 연결할 DB를 정해준다.
  2. QueryFactory 클래스의 생성자로 MySQLConnection와 MySQLCompiler의 객체를 넣어 생성해준다.
  3. QueryFactory의 Query 메소드를 사용하여 쿼리문을 실행시킨다.

* CloudStructure
  1. RedisConfig 클래스를 사용하여 Redis 설정을 해준다.
  2. RedisConnection 클래스의 생성자에 위에서 설정한 RedisConfig 객체를 넣어 생성해준다.
  3. RedisString<>의 매소드에 Redis에 보관할 Value 형식을 넣고, 생성자에 RedisConnection과 값과 유지기간을 넣어 생성해준다.
  4. RedisString<>의 객체를 사용하여 Redis에 데이터를 읽거나 쓴다.

* Model Binding
  - Form Values : Request의 Body에서 보낸 값 (HTTP POST 방식의 요청)
  - Route Values : URL 매칭, Default Value
  - Query String Values : URL 끝에 붙이는 방법. ?Name=Rookies (HTTP GET 방식의 요청)
  - 사용 지정
    - [FromHeader] : Header Value에서 찾아라
    - [FromQuery] : Query String에서 찾아라
    - [FromRoute] : Route Parameter에서 찾아라
    - [FromFrom] : Post Body에서 찾아라
    - [FromBody] : 그냥 Body에서 찾아라
  - DataAnnotation
    - [Required] : 무조건 있어야 한다
    - [CreditCard] : 올바른 결제카드 번호인지 확인
    - [EmailAddress] : 올바른 이메일 주소인지 확인
    - [StringLength(max)] : String 길이가 최대 max인지 확인
    - [MinLength(min)] : Collection의 크기가 최소 min인지 확인
    - [Phone] : 올바른 전화번호인지 확인
    - [Range(min, max)] : Min Max 사이의 값인지 확인
    - [Url] : 올바른 URL인지 확인
    - [Compare] : 2개의 Property 비교 (ex. Password, ComfirmPassword)

* Attribute Routing
  - Controller 또는 Action에서 [Route(”경로”)]를 사용해 지정이 가능하다.
  - Action에서 Route에 /가 붙으면 절대 경로, 없으면 상대 경로로 동작하게 된다.
  - 특정 HTTP Verb에 대해 동작하게 하려면 [HttpGet], [HttpPost] 등과 같이 사용하면 된다.
 
* DI (Dependency Injection)
  - ConfigureServices에서 등록해야 한다.
    - Service Type (인터페이스 or 클래스)
    - Implementation Type (클래스)
    - LifeTime (Transient, Scoped, Singleton)
      - Transient - 항상 새로운 서비스 Instance를 만든다. 매번 new
      - Scoped - Scope 동일한 요청 내에서 같음
      - Singleton - 항상 동일한 Instance를 사용 (웹에서의 싱글톤은 thread-safe해야 함)
  - 동일한 인터페이스에 대해 다수의 서비스 등록 가능 (ex. IEnumerable<IBaseLogger>)
  - 보통 생성자를 사용하지만 [FromServices]를 사용할 수도 있다.
 
* Configuration
  - 대부분의 설정들은 CreateDefaultBuilder에서 발생한다.
  - 실제 ConfigureAppConfiguration 코드 동작
    - JSON file provider (appsettings.json)
    - JSON file provider (appsettings.{ENV}.json)
    - UserSecrets ← Development
    - Env Variable (환경변수)
    - CommandLine
  - Reload
    - reloadOnChange = true 으로 해놓으면 동작한다.
    - 프로그램 실행중에 appsetiings.json을 변경하면 바로 적용된다.
  - 사용 방법
    - _configuration["Logging:LogLevel:Default"]
    - 모델링 클래스 (POCO) 하나를 만들어준다 public getter/setter → Startup에 등록 → IOptions<>로 DI (만약 Reload를 사용하지 않는다면 IOptionsSnapshot<> 사용
    - List나 Dictionary도 사용 가능 (List : IReadOnlyList<> IReadOnlyCollection<> ICollection<> IEnumerable<>, Dictionary : IDictionary<,> IReadOnlyDictionary<,>)
  - 개발 환경에 다른 Configuration (관련 헬퍼 클래스들이 이미 만들어져 있기 때문에 보통 셋 중 하나로 사용)
    - Development
    - Staging
    - Production
   
* Filter
  - 종류
    - Authorization Filter : 가장 먼저 실행되며 권한이 있는지 확인. 권한이 없으면 흐름을 가로채서 빠져나가게 한다.
    - Resource Filter : Authorization Filter 다음으로 추가. 맨 마지막에도 가능. 공용 코드를 넣는 등의 작업.
    - Action Filter : Action 호출 전후에 처리
    - Exception Filter : 예외가 일어났을 때
    - Result Filter : IActionResult 전후에 처리
  - 미들웨어 vs 필터
    - 방향성
      - 미들웨어는 항상 양방향 (In/Out)
      - 필터 (Resource, Action, Result 2번, 나머진 1번)
    - Request 종류
      - 미들웨어는 모든 Request에 대해
      - 필터는 MVC 미들웨어와 연관된 Request에 대해서만 실행
    - 적용 범위
      - 필터는 전체 / Controller / Action 적용 범위를 골라서 적용
      - 필터는 MVC의 ModelState, IActionResult 등 세부 정보에 접근 가능
    - 미들웨어는 더 일반적이고 광범위, 필터는 MVC 미들웨어에서 동작하는 2차 미들웨어 정도로 이해 가능
  - 만드는 방법
    - IAuthorizationFilter, IAsyncAuthorizationFilter, IResourceFilter, IAsyncResourceFilter 등
  - Filter의 실행 순서
    - 기본 실행 순서
      - Executing : 넓은 순서로 실행. Global -> Controller -> Action
      - Executed : 좁은 순서로 실행. Action -> Controller -> Global
    - 명시적 순서 지정 가능
      - IOrderedFilter 사용.
      - Order가 작을수록 먼저 실행
      - Order가 같은 경우 기본 실행 순서를 따른다.

* HTTP Method
  - GET : 서버로부터 데이터를 취득
  - POST : 서버에 데이터를 추가, 작성
  - PUT : 서버의 데이터를 갱신
  - DELETE : 서버의 데이터를 삭제
  - HEAD : 서버 리소스의 헤더 (메타 데이터의 취득)
  - OPTIONS : 리소스가 지원하고 있는 메소드의 취득
  - PATCH : 리소스의 일부분을 수정
  - CONNECT : 프록시 동작의 터널 접속을 변경
