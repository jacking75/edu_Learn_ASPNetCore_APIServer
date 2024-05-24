## Configuration - appsettings.json

[Configuration - appsettings.json : 네이버 블로그 (naver.com)](https://m.blog.naver.com/okcharles/222152730413)

- Configuration 클래스: ASP.NET core 앱 빌드 & 실행 시, 앱에 관한 설정값을 제공하는 서비스 역할 수행
- 서비스 자체는 프레임워크 서비스이기 때문에 별도의 서비스 컨테이너 등록 과정 없이 바로 주입받아 사용 가능.(Program.cs)

```csharp
IConfiguration configuration = builder.Configuration;
```

- 서비스가 사용할 설정값들은 설정제공자가 제공.
(https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#cp)
    
    ### appsettings.json
    
- JSONConfigurationProvider를 이용하여 appsetting.json 파일에 정의된 설정값을 앱에 제공.
- 프로젝트 빌드 후, 출력 폴더와 함께 복사.
- 빌드 출력폴더의 다른 파일들과 함께 실행할 시스템에 옮겨짐
→ dotnet run {웹앱 이름} 의 입력 통해 웹앱이 실행될 때, 코드 내의 설정값들이 appsettings.json 파일에 의해 결정
→ 설정값의 변경이 있다면, 그와 관련된 appsettings.json 파일 내용 변경 후, 앱 재실행
- 구조: json 문서의 일반적 구조 but, 설정의 section과 적용될 쌍 값으로 정의

```csharp
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ServerAddress": "http://0.0.0.0:11500",//포트 번호는 임의 설정
  "DBConfig": {
    "RedisDB": "localhost:10001",
    "AccountDB": "Server=localhost; Port=3306;user=root; Password=password1234.;Database=HiveDB;Pooling=true;Min Pool Size=0;Max Pool Size= 40;AllowUserVariables=True;"
  }
}
```

- 위 코드는 4가지 설정 섹션으로 구성됨
: Logging, AllowedHosts, ServerAddress, DBConfig

- DBConfig 섹션은 두 가지의 값을 가진다. 각 값은 값의 이름으로 참조된다.
: RedisDB, AccountDB

### 설정값 사용

코드 내부에서 appsettings.json의 값을 사용하기 위해서는 섹션과 값 이름을 지정해야 한다.

값의 자료형에 따라 아래와 같은 방법을 사용한다.

- 객체(Program.cs)

```csharp
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));
```

이 코드는 appsettings.json에서 DBConfig 섹션을 객체로 가져와서 DBConfig 클래스 객체로 변환해주고 있다. DBConfig 클래스는 Repository에서 다음과 같이 정의한다.

```csharp
public class DBConfig
{
    public string AccountDB { get; set; }
    public string RedisDB { get; set; }
}
```

### JWT(Json Web Token)

https://nowonbun.tistory.com/281

- HiveServer → HiveMySQL에서 정보 조회 후 클라이언트 암호화된 비밀번호와 DB 암호화된 비밀번호가 같으면 id(이메일을 사용한다)와 인증토큰을 Redis에 저장한다.
- 이때 인증토큰을 JWT로 생성하였다.

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/e703f272-c681-4c23-85de-81590eaef8dc/847700ba-b5db-4fe9-b997-7a71d6977633/Untitled.png)

- JWT는 위 이미지처럼 xxxxx.xxxxx.xxxxx의 세 파트의 구조로 되어 있다.
- Header: 토큰 타입, 알고리즘 정보에 대해 설정되어 있다.
- Payload: 세션처럼 사용할 정보 담겨져 있다. 이때, 이름, 아이디 정도는 괜찮지만 패스워드, 개인정보가 있다면 탈취당할 위험이 있다.
- Signature: 토큰 정보가 맞는 정보인지 확인하는 코드가 있다.
- [https://jwt.io](https://jwt.io/) 에서 JWT 토큰 복호화가 가능하다.

```csharp
using System.IdentityModel.Tokens.Jwt;
//로그인: Redis(Memory)에 저장할 토큰 생성 - JWS
public static string GenerateAuthToken(string email)
{
    //JWS Payload 설정
    var claims = new[]
    {
        new Claim(ClaimTypes.Email, email)
    };

    var token = new JwtSecurityToken(
        claims:claims,
        expires: DateTime.UtcNow.AddHours(6),
        notBefore:DateTime.UtcNow
    );
    //Payload에 이메일, 만료시간 등의 정보를 담는다.

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return tokenString;
}
```

## Redis

### 개념

https://ittrue.tistory.com/317

https://docs.google.com/document/d/10mHFq-kTpGBk1-id5Z-zoseiLnTKr_T8N3byBZP5mEg/edit

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/e703f272-c681-4c23-85de-81590eaef8dc/06d49e1b-dc64-42ac-85d3-09d1e687412f/Untitled.png)

- Redis: Remote Dictionary Server의 약자로, Key-Value 쌍의 해시 맵과 같은 구조를 가진 NoSQL 형 데이터베이스 관리 시스템(DBMS)
- 오픈 소스 기반으로, in memory 데이터 구조 저장소(메모리에 데이터를 저장)
→ 별도의 쿼리문이 필요하지 않고, 빠른 속도로 처리 가능
- 장단점:
    - 성능: 메모리에 저장되기 때문에 대기 시간이 적고 처리량이 높다.
    (디스크 기반 데이터베이스보다 빠르다)
    - 유연한 데이터 구조: String, List, Set, Hash, Sorted Set, Bitmap, JSON 등 다양한 데이터 타입 지원
    - 개발 용이성: Redis는 쿼리문이 필요하지 않음. 단순한 명령 구조로 저장, 조회 등이 가능.
    Java, Py, C, C++, C#, JavaScript, PHP, Node.js, Ruby 등 다수의 언어 지원
    - 영속성: 데이터 디스크에 저장 가능. 서버에 치명적 문제가 발생하더라도 디스크 데이터를 통해 복구 가능.
    - Single Thread 방식: 한번에 하나의 명령어만을 처리. Race Condition(경쟁 상태)가 거의 발생하지 않는다.
    but, 멀티 스레드가 아닌 싱글 스레드 방식이기 때문에 O(n) 명령어 사용은 주의해야 함.
        
        
        그래서 Redis를 등록할 때에는 AddSingleton으로 등록해도 무방하다(Program.cs)
        
        ```csharp
        //Repository 등록
        builder.Services.AddScoped<IAccountDB, AccountDB>();
        builder.Services.AddSingleton<IMemoryDB, MemoryDB>();
        ```
        
    
- Redis 사용사례
    - 인증 토큰 저장(세션 스토어): 로그인 메모리DB(캐시 메모리 DB)
    - 채팅, 메시지, 대기열
    - 랭킹 보드
    - 기타: 미디어 스트리밍, 실시간 분석, 위치기반 데이터 타입 사용 등…
    

### C# Redis 라이브러리 - CloudStructures

https://jacking75.github.io/NET_lib_CloudStructures/

https://gist.github.com/jacking75/5f91f8cf975e0bf778508acdf79499c0

- 설치: Nuget
- 플랫폼: .NET core, .NET Framework
- CloudStructures: StackExchange.Redis 사용하기 편하게 랩핑한 라이브러리.
(StackExchange.Redis에서 제공하는 API는 원시 수준, 대부분 반환형이 byte[] 형식.
→ Serialize를 통해 Object 변환 필요
→ CloudStructures는 이 부분을 편리하게 제공)

![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/e703f272-c681-4c23-85de-81590eaef8dc/fb2e94b1-65b5-4ba8-9197-dee5d7dab749/Untitled.png)

```csharp
//사용 예시
public async Task<ErrorCode> RegisterUserAsync(string email, string authToken)
{
    ErrorCode errorCode = ErrorCode.None;

    RedisDBAuthUserData user = new()
    {
        Email = email,
        AuthToken = authToken
    };

    string keyValue = user.Email;

    try
    {
        RedisString<RedisDBAuthUserData> redis = new(_redisConn, keyValue, LoginTimeSpan());

				//id와 인증 토큰 Redis에 삽입
        if(await redis.SetAsync(user, LoginTimeSpan())==false)
        {
            return ErrorCode.LoginFailAddRedis;
        }
    }
    catch (Exception ex)
    {
        return ErrorCode.RedisFailException;
    }

    return errorCode;
}
```

## DI(Dependency Injection): 의존성 주입

[https://velog.io/@jeong-god/DI란-무엇인가](https://velog.io/@jeong-god/DI%EB%9E%80-%EB%AC%B4%EC%97%87%EC%9D%B8%EA%B0%80)

- 의존성이란?
    - 어떤 **Serviece** 호출하려는 그 **Client**는 그 **Service**가 어떻게 구성되었는지 알지 못해야 함.
    - **Client**는 대신 서비스 제공에 대한 책임을 외부 코드(주입자)로부터 위임.
    **Client**는 주입자 코드 호출 불가.
    - 주입자는 이미 존재하거나 주입자에 의해 구성되었을 서비스 클라이언트로 주입
    클라이언트는 서비스 이용
    - 클라이언트가 주입자와 서비스 구성방식 or 사용중인 실제 서비스에 대해 알 필요가 없음
    - **Client**: 서비스의 사용 방식을 정의하고 있는 서비스의 고유한 인터페이스에 대해서만 알면 됨
    - **“구성의 책임”**으로부터 **“사용의 책임”** 구분

### DI 사용 X 예시

```csharp
public class Coffee {...}

public class Programmer {
	private Coffee coffee = new Coffee();

	public startProgramming(){
		this.coffee.drink()
	}
}
```

- Programmer 객체는 Coffee 객체가 필요, 따라서 생성
== Programmer 객체는 Coffee 객체에 의존
- but, Coffee 객체가 아닌 Americano 객체를 사용하고 싶다면 해당 코드 수정해야 함.
- Coupling(결합도) 높아지게 되어 코드의 재활용성 문제가 많아짐

### DI 사용 O 예시

```csharp
public class Coffee {...}
public class Cappuccino extends Coffee {...}
public class Americano extends Coffee {...}

public class Programmer {
	private Coffee coffee;

	public Programmer(Coffee coffee){
		this.coffee = coffee
	}

	public startProgramming(){
		this.coffee.drink()
	}
}
```

- Programmer 객체에 Coffee라는 객체를  **주입**
- Programmer 객체가 Cappuccino 객체를 마시는 중이라면, Cappuccino 객체를 넘기면 됨.
- 코드 재사용 가능

### 생성자 주입

- 객체의 불변성 확보 가능
- 순환 참조 에러 방지
- 객체의 생성과 동시에 의존성 주입이 이루어짐
- Repository에 생성한 인터페이스를 Controller에서 사용하기 위해 주입
    
    ![Untitled](https://prod-files-secure.s3.us-west-2.amazonaws.com/e703f272-c681-4c23-85de-81590eaef8dc/4b84e464-57b6-41e7-b6aa-c536b8433c32/Untitled.png)
    

```csharp
public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDB accountDB)
{
    _logger = logger;
    _accountDB = accountDB; 
}
```

## DAO, DTO, VO

https://melonicedlatte.com/2021/07/24/231500.html

https://ssuamje.tistory.com/85

https://iri-kang.tistory.com/5

### DAO(Data Access Object)

- DB의 data에 접근하기 위한 객체.
- DB 접근 로직과 비즈니스 로직 분리하기 위해 사용
- ex) Repository에서 DB 연동 시 사용

```csharp
//이 정보로 Repository에서 MysqlDatabase로 정보 요청
public class dbAccountInfo
{
    public Int64 uid { get; set; }
    public string email { get; set; }
    public string pw { get; set; }
    public string salt_value { get; set; }
    public string create_date { get; set; }
    public string recent_login_date { get; set;}
}
```

### DTO(Data Transfer Object)

- 계층 간 데이터 교환을 위해 사용하는 객체.
- DTO는 로직을 가지지 않는 순수한 데이터 객체(getter, setter만 가진 클래스)
- **Request & Response:** 클라이언트에서 서버 쪽으로 전송하는 요청 데이터, 서버에서 클라이언트로 전송하는 응답 데이터 형식으로 데이터 전송
- ex) Controller - Repository
    - 유저가 데이터 입력하여 form에 있는 데이터를 DTO에 넣어서 전송
    - 해당 DTO를 받은 서버가 DAO 이용하여 DB로 데이터 넣음
    
    ```csharp
    //로그인 시 필요한 Email, Password 를 정의
    //Client 값을 Controller가 이 객체로 전환하여 Reposititory로 전달한다(정보 조회 위함)
    public class CreateAccountReq
    {
        [Required]
        [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
        [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
    
        [Required]
        [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
        [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    
    public class CreateAccountRes
    {
        public ErrorCode Result { get; set; } = ErrorCode.None;
    }
    ```
    

### VO(Value Object)

- 값 오브젝트, 값을 위해 쓰임
- Read-Only
- DTO와 유사하지만 DTO는 setter가 있으므로 값이 변할 수 있음

## SqlKata

https://docs.google.com/document/u/1/d/e/2PACX-1vTnRYJOXyOagNhTdhpkI_xOQX4DlMu0TRcC9Ehew6wraufgEtBuQiSdGpKzaEmRb-jfsLv43i0nBQsp/pub

- 코드로 쿼리문을 만들어주는 오픈소스 라이브러리
- C#에서 DB 쿼리를 만들고 실행가능

## API Server 작업 시 참고 github

https://github.com/jacking75/edu_Learn_ASPNetCore_APIServer/tree/main/codes/basic/basic_07

https://github.com/jacking75/edu_Learn_ASPNetCore_APIServer/tree/main/codes/MiniGameHeavenAPIServer

https://github.com/jacking75/com2us_edu_GenieFarm/blob/main/GenieFarm/Services/AuthCheckService.cs

https://github.com/jacking75/edu_Learn_ASPNetCore_APIServer/tree/main/codes/net8
