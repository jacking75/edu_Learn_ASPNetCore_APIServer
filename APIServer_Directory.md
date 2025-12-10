# API 서버의 디렉토리 구성 예
- Controllers: 컨트롤러 클래스를 저장하는 곳
- Dto: 클라이언트와 데이터를 주고받을 때 모델 클래스를 저장하는 곳
- Infrastructure: DB, 메일, 기타 시스템 연동 등의 설정파일을 저장하는 곳
- Libraries: 프로젝트 내 공통 처리를 저장하는 곳
- Repositories: DB 접근 처리를 저장하는 곳
- Services : 비즈니스 로직을 저장하는 곳  

<pre>
│   ├── Controllers
│   │   └── TodoController.cs
│   ├── Dto
│   │   ├── CreateRequestDto.cs
│   │   └── CreateResponseDto.cs
│   ├── Infrastructure
│   │   ├── Configure
│   │   │   ├── DatabaseConfigure.cs
│   │   │   └── ServiceConfigure.cs
│   │   └── Database
│   │       └── TodoEntity.cs
│   ├── Libraries
│   │   ├── Exception
│   │   │   └── ValidationException.cs
│   │   └── Middleware
│   │       └── CustomExceptionMiddleware.cs
│   ├── Program.cs
│   ├── Properties
│   │   └── launchSettings.json
│   ├── Repositories
│   │   └── Todo
│   │       ├── ITodoRepository.cs
│   │       └── TodoRepository.cs
│   ├── Services
│   │   └── Todo
│   │       ├── ITodoService.cs
│   │       └── TodoService.cs
│   ├── TodoApi.csproj
│   ├── TodoApi.sln	
│   ├── TodoApi.http
│   ├── appsettings.Development.json
│   └── appsettings.json
├── Api.Tests
│   ├── Api.Tests.csproj
│   ├── Api.Tests.sln	
│   ├── GlobalUsings.cs
│   └── UnitTests
│       └── Services
│           └── TodoServiceTests.cs
└── dotnet-todo-web-api.sln	
</pre>   
    
---  
  
</br>   
</br>    

# 디렉토리 구조는 어떻게 구성하는 것이 좋은가? 
from: ChatGPT  

정석 하나가 딱 있는 건 아니지만, **ASP.NET Core Web API에서 많이 쓰이는 패턴**은 몇 가지로 거의 수렴한다.
특히 질문한 것처럼 `Models` 안에 `DTOs`를 넣느냐, `DTOs`를 `Models`와 동급으로 두느냐는 **“Models에 뭐가 들어가느냐”**에 따라 달라진다.

아래를 기준으로 보면 된다.

---

## 1. 먼저 개념부터 정리: Entity vs DTO

* **Entity / Domain Model**

  * DB와 매핑되거나 비즈니스 규칙을 담는 핵심 모델
  * 예: `User`, `Order`, `InventoryItem` 등
* **DTO (Data Transfer Object)**

  * API 입출력용, “전달용” 모델
  * Request DTO: `CreateUserRequest`, `UpdateUserRequest`
  * Response DTO: `UserResponse`, `PagedResult<T>` 등
  * 외부 계약(API 스펙)에 가깝고, 도메인 모델과는 독립적으로 변경될 수 있음

이 둘은 **책임과 라이프사이클이 다르다**
→ 폴더도 분리하는 게 보편적이다.

---

## 2. 단일 Web API 프로젝트에서의 흔한 구조

프로젝트 하나만 두는 비교적 단순한 구조라면, 보통 이렇게 많이 나눈다.

```text
MyApp
 ├─ Controllers
 ├─ Domain          // 도메인 모델, 엔티티
 │   ├─ Entities
 │   └─ ValueObjects
 ├─ Services        // 비즈니스 서비스 (Application 서비스)
 ├─ Dtos            // API용 DTO
 │   ├─ Requests
 │   └─ Responses
 ├─ Repositories    // 인터페이스 + 구현(간단한 경우)
 └─ Infrastructure  // DB 관련 구현, 외부 시스템 등
```

### 여기서 질문에 대한 답

* `Models`라는 폴더에 **Entity**를 두고 있다면:

  * `Models/DTOs` 로 넣는 것보다
  * `Dtos`를 **`Models`와 동급**으로 빼는 쪽이 더 일반적이다.
  * 이유: DTO는 “API 레벨 계약”, Entity는 “도메인 레벨 모델”이라 책임이 다르기 때문이다.

예를 들면 이렇게:

```text
 ├─ Models          // 도메인 엔티티
 │   ├─ User.cs
 │   └─ Item.cs
 ├─ Dtos            // API 입출력용
 │   ├─ User
 │   │   ├─ UserResponse.cs
 │   │   └─ CreateUserRequest.cs
 │   └─ Item
 │       └─ ItemResponse.cs
```

`Models/DTOs` 로 넣으면 “DTO도 모델의 한 종류”라는 느낌이 강해져서
나중에 **Entity와 DTO가 뒤섞이는 코드**가 되기 쉽다.

---

## 3. 멀티 프로젝트(계층형) 구조에서의 보편적인 패턴

조금 규모가 있으면 보통 이렇게 나눈다.

```text
src
 ├─ MyApp.Api             // Web API (Controller, API DTO)
 ├─ MyApp.Application     // 유스케이스/서비스, Application DTO
 ├─ MyApp.Domain          // Entity, ValueObject, Domain 서비스
 └─ MyApp.Infrastructure  // EF Core, Repository 구현, 외부 연동
```

각 프로젝트 내부는 대략 이렇게 간다.

### `MyApp.Domain`

```text
MyApp.Domain
 ├─ Entities
 ├─ ValueObjects
 └─ Services   // 도메인 서비스(규칙)
```

### `MyApp.Application`

```text
MyApp.Application
 ├─ Interfaces  // IUserRepository, IEmailSender 등
 ├─ Services    // UseCase(Service) 레벨
 └─ Dtos
     ├─ UserDto.cs
     └─ ...
```

### `MyApp.Api`

```text
MyApp.Api
 ├─ Controllers
 └─ Contracts or Dtos
     ├─ Requests
     └─ Responses
```

여기서도 DTO는 **Api/Contracts** 폴더에 두고,
Domain의 Entity와는 분리하는 것이 거의 표준에 가깝다.

---

## 4. 타입별이 아니라 기능별(Vertical Slice / Feature 폴더) 구조도 많이 쓴다

최근에는 폴더를 타입별이 아니라 “기능별”로 묶는 형태도 많이 쓴다.

```text
MyApp.Api
 ├─ Features
 │   ├─ Users
 │   │   ├─ UsersController.cs
 │   │   ├─ UserDto.cs
 │   │   ├─ CreateUserRequest.cs
 │   │   └─ CreateUserHandler.cs (또는 Service)
 │   └─ Items
 │       ├─ ItemsController.cs
 │       ├─ ItemDto.cs
 │       └─ ...
 └─ Program.cs
```

이 패턴의 특징은 다음과 같다.

* “User 관련 코드”가 한 폴더에 모여서

  * Controller
  * DTO
  * Handler / Service
    를 한 번에 보기 쉬움
* 규모 커질수록 **팀/기능 단위로 나누기 좋다**

이 방식에서도 DTO는 Entity와 폴더를 섞지 않고,
각 Feature 폴더 안에 `UserDto`, `CreateUserRequest` 등을 둔다.

---

## 5. 정리: DTO 폴더를 어떻게 두는 게 좋나
질문에 대한 결론만 다시 정리하면 다음과 같다.

1. **보편적인 패턴**

   * 도메인 Entity(Model)와 DTO를 **물리적으로 분리**한다.
   * DTO는 보통 `/Dtos`, `/Contracts`, `/Features/...` 등에 두고
     도메인 모델이 있는 `/Models` 또는 `/Domain`과는 별도로 관리한다.

2. **Models/DTOs처럼 Models 하위에 두는 경우**

   * 작은/단기 프로젝트에서 “일단 폴더 하나 더 만들기 귀찮다” 수준이면 할 수는 있지만
   * 책임이 섞이기 쉬워서 규모가 커지면 후회하기 쉽다.

3. **API 스펙 중심으로 생각하는 게 좋다**

   * DTO = 클라이언트와의 계약(API)
   * Entity = 내부 도메인 모델
     이 관점으로 보면 자연스럽게 폴더를 나눌 수 있다.

---
  
</br>    
</br>     
     
## 1. 왜 굳이 프로젝트를 나누는가?
솔직히 말해서 **초기에는 안 나눠도 된다**
그런데도 다들 어느 시점부터는 나누는 이유는 대략 다음 네 가지 때문이다.

### 1) 참조 방향을 강제로 고정할 수 있다
예를 들어 이렇게 나눈다고 하자.

* `Game.Api` (ASP.NET Core Web API)
* `Game.Application` (서비스, 유즈케이스)
* `Game.Domain` (엔티티, 도메인 로직)
* `Game.Infrastructure` (EF Core, Redis, 외부 시스템 구현 등)

이때 참조 방향을 이렇게만 허용한다.

* `Game.Api` → `Game.Application`
* `Game.Application` → `Game.Domain`
* `Game.Infrastructure` → `Game.Application`, `Game.Domain`
* **반대로는 참조 불가**

프로젝트를 나누면 csproj 참조 덕분에 **물리적으로 역참조를 막을 수 있다**
단일 프로젝트에서는:

* Controller에서 바로 DbContext 들고 와서 쿼리하고
* 도메인 모델이 HttpContext나 DTO를 알고
  이런 “편하지만 나중에 골치 아픈” 참조가 쉽게 생긴다.

즉, 나누는 이유 중 하나는 **“팀원이(나 포함) 막 짓지 못하게 레일을 깔아놓는 것”**이다.

---

### 2) 테스트 방식과 실행 환경을 나누기 쉽다

* `Game.Domain`, `Game.Application`은 별도 콘솔 없이도 테스트 가능
  → 순수 로직 테스트(유닛 테스트) 돌리기 좋다
* `Game.Api`는 실제 HTTP 파이프라인, 미들웨어까지 포함한 통합 테스트에 집중
* `Game.Infrastructure`는 진짜 DB/Redis 붙인 통합 테스트 전용으로 쓸 수 있다

한 프로젝트에 모든 걸 다 넣으면,
레퍼런스/설정/테스트가 다 섞이면서 점점 손 대기 싫어지는 코드가 된다.

---

### 3) 모듈/도메인 재사용성이 올라간다

예를 들어:

* 수집형 게임 A
* 차기 프로젝트 수집형 게임 B

둘 다 공통으로 쓸 수 있는 게 있을 수 있다.

* `Domain` 레벨: 돈/재화, 인벤토리, 우편, 보상 분배, 쿨타임, 기간제 아이템 등
* `Application` 레벨: “보상 지급 유즈케이스”, “출석 보상 처리”, “메일 보내기” 등

이걸 `Game.Common.Domain`, `Game.Common.Application` 같은 형태로 잘 분리해두면
**다음 프로젝트에서 그대로 가져다 쓸 수 있다**

단일 Web API 프로젝트에 섞여 있으면, 재사용하려고 할 때
컨트롤러, ASP.NET Core 의존성, DB 설정 등과 엉겨서 떼어내기 매우 귀찮다.

---

### 4) 팀 규모가 커질수록 “경계”가 필요하다
사람이 많아지면 구조가 느슨하면 느슨한 대로 다들 본인 스타일대로 건드리기 시작한다.

* 어떤 사람은 Controller에서 바로 DbContext 사용
* 어떤 사람은 Service를 만들지만, 그 안에서 또 HTTP 의존성 사용
* 어떤 사람은 도메인 엔티티 안에 로깅/캐시 코드 넣음

프로젝트를 나누고, 각 프로젝트의 역할을 정해두면:

* Domain 쪽 담당자는 HTTP/DB 모르고, 도메인 규칙에 집중
* Application 담당자는 유즈케이스, 트랜잭션, 여러 도메인 조합에 집중
* API 담당자는 라우팅, 인증/인가, 버전 관리에 집중

이렇게 **관심사/역할을 사람 단위로도 나누기 쉬워진다**.

---

## 2. 수집형(가챠, 캐릭터/카드 수집) 모바일 게임 서버 구조라면?

온라인 수집형 게임 서버 기준으로, 현실적인 구조를 하나 추천하겠다.
“처음부터 너무 잘게 쪼개기보단, 중간 정도 복잡도”를 목표로 둔다.

### 2-1. 솔루션 구조 예시

```text
src
 ├─ Game.Api               // ASP.NET Core Web API
 ├─ Game.Application       // 유즈케이스/서비스
 ├─ Game.Domain            // 도메인 모델/규칙
 └─ Game.Infrastructure    // DB, Redis, 외부 연동 구현
```

각 프로젝트 역할은 이렇게 잡는다.

---

### 2-2. Game.Domain – 게임 규칙의 심장

여기에 들어가는 것들이다.

* 엔티티

  * `Player`, `Inventory`, `Item`, `Hero`, `Stage`, `Quest`, `Mail`, `GachaPool` 등
* 값 객체(Value Object)

  * `Currency`, `DropRate`, `LevelInfo`, `Cooldown`, `Reward` 등
* 도메인 서비스

  * 가챠 롤 연산
  * 보상 계산 로직
  * 경험치/레벨업 계산
  * 스태미나 소모/회복 로직 등

여기서는 **ASP.NET Core, EF Core, HttpContext, ILogger** 같은 걸 절대 모르게 만든다.
“게임 규칙만 아는 순수한 C# 라이브러리” 느낌으로 유지하는 것이 중요하다.

---

### 2-3. Game.Application – 유즈케이스(플로우/오케스트레이션)

여기에는 이런 코드가 들어간다.

* 유즈케이스/서비스

  * `LoginService`
  * `GachaService`
  * `StageService`
  * `InventoryService`
  * `MailService`
* 인터페이스

  * `IPlayerRepository`, `IInventoryRepository`, `IGachaRepository`
  * `IMailSender`, `INotificationService`
  * `IRandomProvider` (테스트를 위한 RNG 추상화 등)
* Application 레벨 DTO (내부 이동용)

  * `PlayerDto`, `InventoryDto`, `RewardDto`

여기서 하는 일은 “한 요청에서 어떤 도메인 기능들을 조합해서 실행할지”이다.

예를 들어 `GachaService.RollAsync()`는:

1. Player 로드
2. 재화 부족 여부 확인
3. 재화 차감
4. 도메인 `GachaDomainService.Roll()` 호출하여 결과 생성
5. 인벤토리에 아이템/캐릭터 추가
6. 로그 작성/이벤트 발행
7. 트랜잭션 커밋

흐름은 Application이, 세부 규칙은 Domain이 맡는다.

---

### 2-4. Game.Infrastructure – 실제 구현체 모음
여기에는 인프라 의존 코드가 들어간다.

* EF Core DbContext 및 엔티티 매핑
* `IPlayerRepository`의 MySQL 구현
* Redis 캐시 구현
* Kafka, RabbitMQ, gRPC 클라이언트, 외부 결제 서버 연동 등

이 프로젝트만 `Microsoft.EntityFrameworkCore`, `StackExchange.Redis` 같은 라이브러리를 의존하도록 제한한다.

---

### 2-5. Game.Api – ASP.NET Core Web API (입출구)
여기에 들어갈 것들이다.

* Controllers (`PlayerController`, `GachaController`, `StageController` 등)
* API DTO (Request/Response)

  * `LoginRequest`, `LoginResponse`
  * `RollGachaRequest`, `RollGachaResponse`
* 필터, 미들웨어 (인증, 로깅, 에러 처리 등)
* DI 등록 (`IPlayerRepository -> PlayerRepository`, `IGachaService -> GachaService`)

컨트롤러는 최대한 얇게 만든다.

```csharp
[ApiController]
[Route("api/gacha")]
public class GachaController : ControllerBase
{
    private readonly IGachaService _gacha;

    public GachaController(IGachaService gacha)
    {
        _gacha = gacha;
    }

    [HttpPost("roll")]
    public async Task<ActionResult<RollGachaResponse>> Roll([FromBody] RollGachaRequest request)
    {
        var result = await _gacha.RollAsync(request.PlayerId, request.PoolId, request.Count);

        return Ok(new RollGachaResponse
        {
            Rewards = result.Rewards.Select(r => new RewardDto { ... }).ToList()
        });
    }
}
```

컨트롤러는 **HTTP → Application 호출 → HTTP 응답으로 변환** 정도에만 집중하고,
비즈니스 규칙/DB 접근/트랜잭션 등은 전부 아래 레이어에 위임한다.

---

## 3. 현실적인 조언
마지막으로 현실적인 쪽으로 요약하겠다.

1. **처음부터 무조건 4개 프로젝트로 쪼개라는 건 아니다**

   * 혼자 개발하거나 프로토타입이라면 `Game.Api` 하나에 다 넣고 시작해도 된다.
   * 다만 Entity, DTO, Service 정도만이라도 폴더로 구분해두는 걸 추천한다.

2. **어느 정도 규모가 보여갈 때 쯤 나누는게 좋다**

   * 기능이 많아지고, 팀원이 2~3명 이상이 되면
     Domain을 분리하고, Application/Infrastructure를 떼어내는 걸 고려할 만하다.

3. **수집형 모바일 게임 서버라면**

   * 도메인이 풍부하고 규칙이 복잡해지기 쉬운 장르라서,
   * Domain/Application/Infrastructure 분리가 가져다주는 이득이 크다.
   * 특히 가챠, 보상, 재화, 쿨타임 등은 재사용성과 규칙 검증의 가치가 높다.

---   
   
</br>     

## 예: 수집형 RPG 게임 서버  
**수집형 RPG 하나 만든다고 가정**하고 각 프로젝트에 *무엇을* 넣을지, 그리고 *하나의 유즈케이스가 어떻게 4개 프로젝트를 타고 흐르는지*까지 구체적으로 적어보겠다.

프로젝트 구조는 이것을 기준으로 한다.

```text
src
 ├─ MyApp.Api             // Web API (Controller, API DTO)
 ├─ MyApp.Application     // 유즈케이스/서비스, Application DTO
 ├─ MyApp.Domain          // Entity, ValueObject, Domain 서비스
 └─ MyApp.Infrastructure  // EF Core, Repository 구현, 외부 연동
```

예시는 대표적인 수집형 RPG 도메인들(플레이어, 영웅, 가챠, 인벤토리, 스테이지, 우편 등)을 기준으로 한다.

---

## 1. MyApp.Domain – 게임 규칙의 심장

**핵심 개념**

* “이 게임이 어떤 규칙으로 돌아가는가?”를 표현하는 레이어다.
* HTTP, DB, EF Core, 컨트롤러, 로깅 같은 건 전혀 몰라야 한다.
* 여긴 그냥 **순수 C# 라이브러리**라고 생각하면 된다.

### 1.1. 폴더 구조 예시

```text
MyApp.Domain
 ├─ Players
 │   ├─ Player.cs
 │   └─ PlayerLevelPolicy.cs
 ├─ Heroes
 │   ├─ Hero.cs
 │   └─ HeroRarity.cs
 ├─ Inventory
 │   ├─ Item.cs
 │   ├─ ItemInstance.cs
 │   └─ Inventory.cs
 ├─ Gacha
 │   ├─ GachaPool.cs
 │   ├─ GachaResult.cs
 │   └─ GachaDomainService.cs
 ├─ Stage
 │   ├─ Stage.cs
 │   ├─ StageResult.cs
 │   └─ StageDomainService.cs
 ├─ Mail
 │   ├─ Mail.cs
 │   └─ MailAttachment.cs
 ├─ Common
 │   ├─ Currency.cs          // ValueObject
 │   ├─ Reward.cs            // ValueObject
 │   ├─ TimeSpanRange.cs     // 기간제 아이템 등
 │   └─ IRandomGenerator.cs  // RNG 추상화(도메인에서 쓸 인터페이스)
 └─ DomainExceptions
     ├─ NotEnoughCurrencyException.cs
     └─ InvalidStateException.cs
```

### 1.2. 예시 코드

#### Player / Currency

```csharp
// Value Object
public readonly struct Currency
{
    public int Gems { get; }
    public int Gold { get; }

    public Currency(int gems, int gold)
    {
        if (gems < 0 || gold < 0)
            throw new ArgumentException("Currency cannot be negative.");

        Gems = gems;
        Gold = gold;
    }

    public Currency Add(Currency other)
        => new Currency(Gems + other.Gems, Gold + other.Gold);

    public Currency Subtract(Currency other)
    {
        if (Gems < other.Gems || Gold < other.Gold)
            throw new NotEnoughCurrencyException();
        return new Currency(Gems - other.Gems, Gold - other.Gold);
    }
}
```

#### Player 엔티티

```csharp
public class Player
{
    public long Id { get; private set; }
    public int Level { get; private set; }
    public Currency Currency { get; private set; }
    public int Stamina { get; private set; }
    public DateTime LastLoginAt { get; private set; }

    private readonly List<Hero> _heroes = new();
    public IReadOnlyCollection<Hero> Heroes => _heroes;

    protected Player() { } // ORM용 기본 생성자

    public Player(long id)
    {
        Id = id;
        Level = 1;
        Currency = new Currency(0, 0);
        Stamina = 100;
        LastLoginAt = DateTime.UtcNow;
    }

    public void AddCurrency(Currency delta) => Currency = Currency.Add(delta);

    public void SpendCurrency(Currency cost) => Currency = Currency.Subtract(cost);

    public void AddHero(Hero hero) => _heroes.Add(hero);

    public void ChangeLevel(int newLevel)
    {
        if (newLevel < Level)
            throw new InvalidOperationException("Level cannot decrease.");
        Level = newLevel;
    }
}
```

#### GachaDomainService

```csharp
public class GachaDomainService
{
    private readonly IRandomGenerator _random;

    public GachaDomainService(IRandomGenerator random)
    {
        _random = random;
    }

    public IReadOnlyList<Hero> Roll(GachaPool pool, int count)
    {
        var results = new List<Hero>(count);
        for (int i = 0; i < count; i++)
        {
            var entry = pool.Pick(_random.NextDouble());
            var hero = new Hero(entry.HeroId, entry.Rarity);
            results.Add(hero);
        }

        return results;
    }
}
```

여기서는 “주어진 풀 안에서 확률에 따라 영웅을 뽑는다”는 **규칙만** 표현하고,
DB 저장, 로그, HTTP 응답 같은 건 전혀 몰라야 한다.

---

## 2. MyApp.Application – 유즈케이스 / 서비스

**핵심 개념**

* “한 요청에서 어떤 도메인 기능을 조합해서 무엇을 할지”를 정의하는 레이어다.
* 트랜잭션, 여러 Aggregate 조합, 도메인 서비스 호출을 묶어서 처리한다.
* DB 구현은 모르는 대신, **Repository 인터페이스**만 알고 의존한다.

### 2.1. 폴더 구조 예시

```text
MyApp.Application
 ├─ Interfaces
 │   ├─ Repositories
 │   │   ├─ IPlayerRepository.cs
 │   │   ├─ IHeroRepository.cs
 │   │   ├─ IInventoryRepository.cs
 │   │   └─ IGachaRepository.cs
 │   ├─ Services
 │   │   ├─ ITimeProvider.cs
 │   │   └─ IUnitOfWork.cs
 │   └─ External
 │       ├─ IAuthService.cs
 │       └─ INotificationService.cs
 ├─ Dtos
 │   ├─ PlayerDto.cs
 │   ├─ HeroDto.cs
 │   └─ RewardDto.cs
 ├─ Gacha
 │   ├─ RollGachaCommand.cs
 │   ├─ RollGachaResult.cs
 │   └─ GachaService.cs
 ├─ Stage
 │   ├─ ClearStageCommand.cs
 │   ├─ ClearStageResult.cs
 │   └─ StageService.cs
 ├─ Player
 │   ├─ LoginCommand.cs
 │   └─ PlayerService.cs
 └─ Mail
     ├─ ReceiveMailCommand.cs
     └─ MailService.cs
```

### 2.2. Repository 인터페이스

```csharp
public interface IPlayerRepository
{
    Task<Player?> GetAsync(long id, CancellationToken ct = default);
    Task AddAsync(Player player, CancellationToken ct = default);
    Task UpdateAsync(Player player, CancellationToken ct = default);
}
```

### 2.3. Gacha 유즈케이스 예시

#### Command / Result DTO

```csharp
public class RollGachaCommand
{
    public long PlayerId { get; init; }
    public int PoolId { get; init; }
    public int Count { get; init; }
}

public class RollGachaResult
{
    public PlayerDto Player { get; init; } = null!;
    public IReadOnlyList<HeroDto> Heroes { get; init; } = Array.Empty<HeroDto>();
}
```

#### GachaService

```csharp
public class GachaService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGachaRepository _gachaRepository;
    private readonly IUnitOfWork _uow;
    private readonly GachaDomainService _gachaDomainService;

    public GachaService(
        IPlayerRepository playerRepository,
        IGachaRepository gachaRepository,
        IUnitOfWork uow,
        GachaDomainService gachaDomainService)
    {
        _playerRepository = playerRepository;
        _gachaRepository = gachaRepository;
        _uow = uow;
        _gachaDomainService = gachaDomainService;
    }

    public async Task<RollGachaResult> RollAsync(RollGachaCommand command, CancellationToken ct = default)
    {
        // 1. 유저/가챠풀 로드
        var player = await _playerRepository.GetAsync(command.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");
        var pool = await _gachaRepository.GetPoolAsync(command.PoolId, ct)
            ?? throw new InvalidOperationException("Gacha pool not found");

        // 2. 재화 차감 (도메인 규칙)
        var cost = pool.GetCostFor(command.Count);
        player.SpendCurrency(cost);

        // 3. 도메인 서비스로 뽑기
        var heroes = _gachaDomainService.Roll(pool, command.Count);
        foreach (var hero in heroes)
        {
            player.AddHero(hero);
        }

        // 4. 저장 + 트랜잭션 커밋
        await _playerRepository.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        // 5. 결과 DTO 매핑
        return new RollGachaResult
        {
            Player = PlayerDto.From(player),
            Heroes = heroes.Select(HeroDto.From).ToList()
        };
    }
}
```

여기서 Application은:

* 도메인 규칙을 호출하고
* 여러 Aggregate를 묶어 트랜잭션 처리하며
* 결과를 상위(API)가 쓰기 쉬운 DTO로 변환하는 역할을 한다.

---

## 3. MyApp.Infrastructure – EF Core / 외부 시스템 구현

**핵심 개념**

* Application에 정의된 인터페이스를 **실제로 MySQL/Redis/외부 서비스에 붙이는 구현체** 레이어다.
* 이 프로젝트만 EF Core, Redis, gRPC, HTTP 클라이언트 같은 라이브러리를 안다.

### 3.1. 폴더 구조 예시

```text
MyApp.Infrastructure
 ├─ Persistence
 │   ├─ GameDbContext.cs
 │   ├─ Configurations
 │   │   ├─ PlayerConfiguration.cs
 │   │   └─ HeroConfiguration.cs
 │   ├─ Repositories
 │   │   ├─ PlayerRepository.cs
 │   │   ├─ HeroRepository.cs
 │   │   └─ GachaRepository.cs
 │   └─ UnitOfWork.cs
 ├─ External
 │   ├─ AuthService.cs
 │   └─ NotificationService.cs
 └─ Random
     └─ DefaultRandomGenerator.cs
```

### 3.2. DbContext 예시

```csharp
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<GachaPool> GachaPools => Set<GachaPool>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new HeroConfiguration());
        // ...
    }
}
```

### 3.3. Repository 구현

```csharp
public class PlayerRepository : IPlayerRepository
{
    private readonly GameDbContext _db;

    public PlayerRepository(GameDbContext db)
    {
        _db = db;
    }

    public Task<Player?> GetAsync(long id, CancellationToken ct = default)
    {
        return _db.Players
            .Include(p => p.Heroes)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task AddAsync(Player player, CancellationToken ct = default)
        => _db.Players.AddAsync(player, ct).AsTask();

    public Task UpdateAsync(Player player, CancellationToken ct = default)
    {
        _db.Players.Update(player);
        return Task.CompletedTask;
    }
}
```

### 3.4. UnitOfWork

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly GameDbContext _db;

    public UnitOfWork(GameDbContext db)
    {
        _db = db;
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
```

### 3.5. Domain에서 쓰는 Random 구현체

```csharp
public class DefaultRandomGenerator : IRandomGenerator
{
    private readonly Random _random = new();

    public double NextDouble() => _random.NextDouble();
}
```

---

## 4. MyApp.Api – Web API (컨트롤러 / API DTO)

**핵심 개념**

* “HTTP 요청 ↔ Application 호출 ↔ HTTP 응답”을 담당하는 최상위 레이어다.
* Swagger, 인증, 미들웨어, 버전 관리, 필터 등은 여기서 처리한다.
* 도메인 모델을 직접 노출하지 않고, **API 전용 DTO**를 쓰는 것이 일반적이다.

### 4.1. 폴더 구조 예시

```text
MyApp.Api
 ├─ Controllers
 │   ├─ PlayerController.cs
 │   ├─ GachaController.cs
 │   └─ StageController.cs
 ├─ Dtos
 │   ├─ Gacha
 │   │   ├─ RollGachaRequest.cs
 │   │   └─ RollGachaResponse.cs
 │   └─ Player
 │       ├─ PlayerResponse.cs
 │       └─ LoginRequest.cs
 ├─ Filters
 │   └─ ApiExceptionFilter.cs
 ├─ Startup (Program.cs / DI 설정)
 └─ Mapping
     └─ ApiMappingProfile.cs (필요하면 AutoMapper 등)
```

### 4.2. API Request/Response DTO

```csharp
public class RollGachaRequest
{
    public int PoolId { get; set; }
    public int Count { get; set; } = 1;
}

public class RollGachaResponse
{
    public PlayerResponse Player { get; set; } = null!;
    public List<HeroResponse> Heroes { get; set; } = new();
}

public class PlayerResponse
{
    public long Id { get; set; }
    public int Level { get; set; }
    public int Gems { get; set; }
    public int Gold { get; set; }
}

public class HeroResponse
{
    public long HeroId { get; set; }
    public string Rarity { get; set; } = "";
    public int Level { get; set; }
}
```

### 4.3. Controller 예시

```csharp
[ApiController]
[Route("api/gacha")]
public class GachaController : ControllerBase
{
    private readonly GachaService _gachaService;
    private readonly ICurrentPlayerProvider _currentPlayer; // 토큰에서 playerId 꺼내는 용도라고 가정

    public GachaController(GachaService gachaService, ICurrentPlayerProvider currentPlayer)
    {
        _gachaService = gachaService;
        _currentPlayer = currentPlayer;
    }

    [HttpPost("roll")]
    public async Task<ActionResult<RollGachaResponse>> Roll([FromBody] RollGachaRequest request)
    {
        var cmd = new RollGachaCommand
        {
            PlayerId = _currentPlayer.PlayerId,
            PoolId = request.PoolId,
            Count = request.Count
        };

        var result = await _gachaService.RollAsync(cmd);

        var response = new RollGachaResponse
        {
            Player = new PlayerResponse
            {
                Id = result.Player.Id,
                Level = result.Player.Level,
                Gems = result.Player.Gems,
                Gold = result.Player.Gold
            },
            Heroes = result.Heroes.Select(h => new HeroResponse
            {
                HeroId = h.HeroId,
                Rarity = h.Rarity.ToString(),
                Level = h.Level
            }).ToList()
        };

        return Ok(response);
    }
}
```

### 4.4. Program.cs / DI 구성

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Application
builder.Services.AddScoped<GachaService>();

// Domain
builder.Services.AddScoped<GachaDomainService>();
builder.Services.AddSingleton<IRandomGenerator, DefaultRandomGenerator>();

// Infrastructure
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("GameDb"),
        new MySqlServerVersion(new Version(8, 0, 25))));

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IGachaRepository, GachaRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();
app.MapControllers();
app.Run();
```

---

## 5. 한 유즈케이스(가챠 롤)가 4개 프로젝트를 타는 흐름

1. **클라이언트 → API**

   * `/api/gacha/roll` POST + `{ poolId, count }`
   * `GachaController`가 `RollGachaRequest` 바인딩

2. **API → Application**

   * Controller가 `RollGachaCommand` 생성
   * `GachaService.RollAsync(cmd)` 호출

3. **Application → Domain + Repository 인터페이스 사용**

   * `IPlayerRepository.GetAsync`, `IGachaRepository.GetPoolAsync`로 도메인 엔티티 로드
   * `player.SpendCurrency`, `GachaDomainService.Roll` 호출
   * `player.AddHero(hero)` 등 도메인 메서드 호출
   * `IPlayerRepository.UpdateAsync`, `IUnitOfWork.CommitAsync` 호출
   * 결과를 `RollGachaResult`로 반환

4. **Infrastructure에서 실제 동작**

   * `PlayerRepository`가 EF Core로 MySQL에서 Player 도큐먼트/테이블 로드
   * `GameDbContext.SaveChangesAsync`로 실제 DB에 반영
   * `DefaultRandomGenerator`가 실제 랜덤값 생성

5. **Application → API → 클라이언트**

   * Application의 Result를 API의 Response DTO로 매핑
   * JSON으로 내려간다

---

## 요약

질문한 구조에서 **각 프로젝트에 구체적으로 들어가야 할 것**을 정리하면 다음과 같다.

* **MyApp.Domain**

  * Entity, ValueObject, Domain Service, 도메인 예외
  * 게임 규칙(가챠 확률, 보상 계산, 레벨업 규칙, 스태미나 처리 등)

* **MyApp.Application**

  * 유즈케이스/서비스 (`GachaService`, `StageService`, `MailService` 등)
  * Repository/외부 서비스 인터페이스(`IPlayerRepository`, `IGachaRepository` 등)
  * Application용 DTO / Command / Result
  * 트랜잭션 단위 정의

* **MyApp.Infrastructure**

  * EF Core DbContext, Entity 매핑
  * Repository 구현, UnitOfWork 구현
  * Redis, gRPC, 외부 결제/푸시 서버 연동 구현
  * Domain에서 쓰는 기술 구현체(`IRandomGenerator` 구현 등)

* **MyApp.Api**

  * Controllers, API Request/Response DTO
  * 필터, 미들웨어, 인증/인가, Swagger
  * DI 구성, 앱 시작/라우팅 설정

이 정도 구조면 수집형 RPG 서버에서

* 로직 복잡도
* 기능 확장
* 테스트(유닛/통합)
* 차기 프로젝트 재사용
  을 모두 고려했을 때 꽤 현실적인 밸런스를 가져가는 형태가 된다.

---   
  
</br>  
  
좋다, 말한 세 가지 유즈케이스

1. 스테이지 클리어
2. 우편 받기(보상 수령)
3. 출석체크 보상 받기

가 **4개 레이어(MyApp.Api / Application / Domain / Infrastructure)** 를 어떻게 타는지 전체 흐름을 단계별로 적어보겠다.

앞에서 정리한 도메인 타입들(Reward, Player, Currency 등)은 이미 존재한다고 가정하고, **핵심 코드와 흐름** 위주로 보여준다.

---

## 공통 전제: 프로젝트 구조

```text
src
 ├─ MyApp.Api             // Web API (Controller, API DTO)
 ├─ MyApp.Application     // 유즈케이스/서비스, Application DTO
 ├─ MyApp.Domain          // Entity, ValueObject, Domain 서비스
 └─ MyApp.Infrastructure  // EF Core, Repository 구현, 외부 연동
```

---

# 1. 유즈케이스 1: 스테이지 클리어

## 1-1. Domain 레이어 (MyApp.Domain)

### 1) 엔티티/값 객체

```csharp
// 스테이지 자체 정의
public class Stage
{
    public int Id { get; private set; }
    public int RequiredStamina { get; private set; }
    public Reward BaseReward { get; private set; }  // 골드/재화, 아이템 등
    public int BaseExp { get; private set; }

    protected Stage() { }

    public Stage(int id, int requiredStamina, Reward baseReward, int baseExp)
    {
        Id = id;
        RequiredStamina = requiredStamina;
        BaseReward = baseReward;
        BaseExp = baseExp;
    }
}

// 클리어 결과
public class StageClearResult
{
    public Reward Reward { get; }
    public int GainedExp { get; }

    public StageClearResult(Reward reward, int gainedExp)
    {
        Reward = reward;
        GainedExp = gainedExp;
    }
}
```

### 2) 도메인 서비스

```csharp
public class StageDomainService
{
    public StageClearResult Clear(Player player, Stage stage, bool isFirstClear)
    {
        // 스태미나 소모는 Player가 책임짐
        player.ConsumeStamina(stage.RequiredStamina);

        var reward = stage.BaseReward;

        if (isFirstClear)
        {
            // 첫 클리어 보너스가 있다면 합산
            reward = reward.Merge(Reward.FirstClearBonus());
        }

        player.AddReward(reward);
        player.AddExperience(stage.BaseExp);

        return new StageClearResult(reward, stage.BaseExp);
    }
}
```

여기까지는 **HTTP도 DB도 모르고, 오직 규칙만** 표현한다.

---

## 1-2. Application 레이어 (MyApp.Application)

### 1) Command / Result DTO

```csharp
public class ClearStageCommand
{
    public long PlayerId { get; init; }
    public int StageId { get; init; }
    public bool IsSuccess { get; init; }
}

public class ClearStageResultDto
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}
```

### 2) 레포지토리/서비스 인터페이스

```csharp
public interface IPlayerRepository
{
    Task<Player?> GetAsync(long id, CancellationToken ct = default);
    Task UpdateAsync(Player player, CancellationToken ct = default);
}

public interface IStageRepository
{
    Task<Stage?> GetAsync(int id, CancellationToken ct = default);
    Task<bool> HasClearedAsync(long playerId, int stageId, CancellationToken ct = default);
    Task MarkClearedAsync(long playerId, int stageId, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
```

### 3) StageService 유즈케이스

```csharp
public class StageService
{
    private readonly IPlayerRepository _players;
    private readonly IStageRepository _stages;
    private readonly StageDomainService _stageDomainService;
    private readonly IUnitOfWork _uow;

    public StageService(
        IPlayerRepository players,
        IStageRepository stages,
        StageDomainService stageDomainService,
        IUnitOfWork uow)
    {
        _players = players;
        _stages = stages;
        _stageDomainService = stageDomainService;
        _uow = uow;
    }

    public async Task<ClearStageResultDto> ClearAsync(ClearStageCommand command, CancellationToken ct = default)
    {
        if (!command.IsSuccess)
            throw new InvalidOperationException("Stage not cleared");

        var player = await _players.GetAsync(command.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");

        var stage = await _stages.GetAsync(command.StageId, ct)
            ?? throw new InvalidOperationException("Stage not found");

        var isFirstClear = !await _stages.HasClearedAsync(command.PlayerId, command.StageId, ct);

        var result = _stageDomainService.Clear(player, stage, isFirstClear);

        await _players.UpdateAsync(player, ct);
        if (isFirstClear)
            await _stages.MarkClearedAsync(player.Id, stage.Id, ct);

        await _uow.CommitAsync(ct);

        return new ClearStageResultDto
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(result.Reward)
        };
    }
}
```

---

## 1-3. Infrastructure 레이어 (MyApp.Infrastructure)

### 1) DbContext & 매핑

```csharp
public class GameDbContext : DbContext
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<PlayerStageClear> PlayerStageClears => Set<PlayerStageClear>();

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
}

public class PlayerStageClear
{
    public long PlayerId { get; set; }
    public int StageId { get; set; }
    public DateTime ClearedAt { get; set; }
}
```

### 2) Repository 구현

```csharp
public class StageRepository : IStageRepository
{
    private readonly GameDbContext _db;

    public StageRepository(GameDbContext db) => _db = db;

    public Task<Stage?> GetAsync(int id, CancellationToken ct = default)
        => _db.Stages.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<bool> HasClearedAsync(long playerId, int stageId, CancellationToken ct = default)
        => _db.PlayerStageClears
              .AnyAsync(x => x.PlayerId == playerId && x.StageId == stageId, ct);

    public async Task MarkClearedAsync(long playerId, int stageId, CancellationToken ct = default)
    {
        _db.PlayerStageClears.Add(new PlayerStageClear
        {
            PlayerId = playerId,
            StageId = stageId,
            ClearedAt = DateTime.UtcNow
        });
        await Task.CompletedTask;
    }
}
```

`PlayerRepository`, `UnitOfWork`는 이전에 보여준 것과 동일한 패턴으로 구현하면 된다.

---

## 1-4. Api 레이어 (MyApp.Api)

### 1) Request / Response DTO

```csharp
public class ClearStageRequest
{
    public int StageId { get; set; }
    public bool IsSuccess { get; set; }
}

public class ClearStageResponse
{
    public PlayerResponse Player { get; set; } = null!;
    public RewardResponse Reward { get; set; } = null!;
}
```

### 2) Controller

```csharp
[ApiController]
[Route("api/stage")]
public class StageController : ControllerBase
{
    private readonly StageService _stageService;
    private readonly ICurrentPlayerProvider _currentPlayer;

    public StageController(StageService stageService, ICurrentPlayerProvider currentPlayer)
    {
        _stageService = stageService;
        _currentPlayer = currentPlayer;
    }

    [HttpPost("clear")]
    public async Task<ActionResult<ClearStageResponse>> Clear([FromBody] ClearStageRequest request)
    {
        var cmd = new ClearStageCommand
        {
            PlayerId = _currentPlayer.PlayerId,
            StageId = request.StageId,
            IsSuccess = request.IsSuccess
        };

        var result = await _stageService.ClearAsync(cmd);

        var response = new ClearStageResponse
        {
            Player = PlayerResponse.From(result.Player),
            Reward = RewardResponse.From(result.Reward)
        };

        return Ok(response);
    }
}
```

---

## 1-5. 스테이지 클리어 전체 흐름 요약

1. 클라이언트 → `POST /api/stage/clear` (`ClearStageRequest`)
2. Api → `StageService.ClearAsync(ClearStageCommand)` 호출
3. Application → Player/Stage 로드, `StageDomainService.Clear` 호출, 트랜잭션 커밋
4. Domain → 규칙대로 스태미나 차감, 보상 계산, 경험치 증가
5. Infrastructure → 실제 DB 읽기/쓰기
6. Application → `ClearStageResultDto` 반환
7. Api → `ClearStageResponse` JSON으로 응답

---

# 2. 유즈케이스 2: 우편 받기(보상 수령)

## 2-1. Domain 레이어

### 1) Mail 엔티티

```csharp
public class Mail
{
    public long Id { get; private set; }
    public long PlayerId { get; private set; }
    public string Title { get; private set; } = "";
    public string Body { get; private set; } = "";
    public Reward Reward { get; private set; }
    public bool IsClaimed { get; private set; }
    public DateTime ExpireAt { get; private set; }

    protected Mail() { }

    public Mail(long playerId, string title, string body, Reward reward, DateTime expireAt)
    {
        PlayerId = playerId;
        Title = title;
        Body = body;
        Reward = reward;
        ExpireAt = expireAt;
        IsClaimed = false;
    }

    public Reward Claim(DateTime now)
    {
        if (IsClaimed)
            throw new InvalidOperationException("Mail already claimed.");
        if (now > ExpireAt)
            throw new InvalidOperationException("Mail expired.");

        IsClaimed = true;
        return Reward;
    }
}
```

---

## 2-2. Application 레이어

### 1) 인터페이스

```csharp
public interface IMailRepository
{
    Task<Mail?> GetAsync(long mailId, CancellationToken ct = default);
    Task UpdateAsync(Mail mail, CancellationToken ct = default);
}
```

### 2) Command / Result DTO

```csharp
public class ClaimMailCommand
{
    public long PlayerId { get; init; }
    public long MailId { get; init; }
}

public class ClaimMailResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}
```

### 3) MailService

```csharp
public class MailService
{
    private readonly IMailRepository _mails;
    private readonly IPlayerRepository _players;
    private readonly IUnitOfWork _uow;
    private readonly ITimeProvider _time;

    public MailService(
        IMailRepository mails,
        IPlayerRepository players,
        IUnitOfWork uow,
        ITimeProvider time)
    {
        _mails = mails;
        _players = players;
        _uow = uow;
        _time = time;
    }

    public async Task<ClaimMailResult> ClaimAsync(ClaimMailCommand cmd, CancellationToken ct = default)
    {
        var player = await _players.GetAsync(cmd.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");

        var mail = await _mails.GetAsync(cmd.MailId, ct)
            ?? throw new InvalidOperationException("Mail not found");

        if (mail.PlayerId != player.Id)
            throw new InvalidOperationException("Mail does not belong to player");

        var reward = mail.Claim(_time.UtcNow); // Domain 로직 호출
        player.AddReward(reward);

        await _mails.UpdateAsync(mail, ct);
        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimMailResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(reward)
        };
    }
}
```

---

## 2-3. Infrastructure 레이어

```csharp
public class MailRepository : IMailRepository
{
    private readonly GameDbContext _db;

    public MailRepository(GameDbContext db) => _db = db;

    public Task<Mail?> GetAsync(long mailId, CancellationToken ct = default)
        => _db.Set<Mail>().FirstOrDefaultAsync(m => m.Id == mailId, ct);

    public Task UpdateAsync(Mail mail, CancellationToken ct = default)
    {
        _db.Update(mail);
        return Task.CompletedTask;
    }
}
```

`ITimeProvider`는 여기서도 구현해줄 수 있다.

---

## 2-4. Api 레이어

### 1) DTO

```csharp
public class ClaimMailRequest
{
    public long MailId { get; set; }
}

public class ClaimMailResponse
{
    public PlayerResponse Player { get; set; } = null!;
    public RewardResponse Reward { get; set; } = null!;
}
```

### 2) Controller

```csharp
[ApiController]
[Route("api/mail")]
public class MailController : ControllerBase
{
    private readonly MailService _mailService;
    private readonly ICurrentPlayerProvider _currentPlayer;

    public MailController(MailService mailService, ICurrentPlayerProvider currentPlayer)
    {
        _mailService = mailService;
        _currentPlayer = currentPlayer;
    }

    [HttpPost("claim")]
    public async Task<ActionResult<ClaimMailResponse>> Claim([FromBody] ClaimMailRequest request)
    {
        var cmd = new ClaimMailCommand
        {
            PlayerId = _currentPlayer.PlayerId,
            MailId = request.MailId
        };

        var result = await _mailService.ClaimAsync(cmd);

        var response = new ClaimMailResponse
        {
            Player = PlayerResponse.From(result.Player),
            Reward = RewardResponse.From(result.Reward)
        };

        return Ok(response);
    }
}
```

---

## 2-5. 우편 받기 전체 흐름 요약

1. 클라이언트 → `POST /api/mail/claim` (`MailId`)
2. Api → `MailService.ClaimAsync(ClaimMailCommand)` 호출
3. Application → Player/Mail 로드, Mail.Claim(now), Player.AddReward, 커밋
4. Domain → 중복 수령, 만료 체크, Reward 반환
5. Infrastructure → Mail/Player 상태 업데이트 DB 반영
6. Application → ClaimMailResult 반환
7. Api → ClaimMailResponse JSON 응답

---

# 3. 유즈케이스 3: 출석체크 보상 받기

## 3-1. Domain 레이어

### 1) AttendanceInfo 엔티티

```csharp
public class AttendanceInfo
{
    public long PlayerId { get; private set; }
    public int CurrentDay { get; private set; }  // 1-based
    public DateTime LastCheckedAt { get; private set; }

    protected AttendanceInfo() { }

    public AttendanceInfo(long playerId)
    {
        PlayerId = playerId;
        CurrentDay = 0;
        LastCheckedAt = DateTime.MinValue;
    }

    public int GetTodayDay(DateTime now)
    {
        // 예시: 매일 00:00 UTC 기준으로 다음 날로 침
        // 실제 구현에서는 기간/이벤트별 로직을 넣을 수 있다
        // 여기서는 단순하게 CurrentDay + 1 로 간다고 가정
        return CurrentDay + 1;
    }

    public void MarkChecked(DateTime now, int day)
    {
        if (day != CurrentDay + 1)
            throw new InvalidOperationException("Invalid attendance day");

        CurrentDay = day;
        LastCheckedAt = now;
    }
}
```

### 2) AttendanceRewardPolicy 도메인 서비스

```csharp
public class AttendanceRewardPolicy
{
    public Reward GetRewardForDay(int day)
    {
        // 예시: 간단한 switch
        return day switch
        {
            1 => Reward.Gold(1000),
            2 => Reward.Gems(10),
            3 => Reward.Item(1001, 1),
            _ => Reward.Gold(500)
        };
    }
}
```

---

## 3-2. Application 레이어

### 1) 인터페이스

```csharp
public interface IAttendanceRepository
{
    Task<AttendanceInfo?> GetAsync(long playerId, CancellationToken ct = default);
    Task AddAsync(AttendanceInfo attendance, CancellationToken ct = default);
    Task UpdateAsync(AttendanceInfo attendance, CancellationToken ct = default);
}
```

### 2) Command / Result DTO

```csharp
public class ClaimAttendanceRewardCommand
{
    public long PlayerId { get; init; }
}

public class ClaimAttendanceRewardResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
    public int Day { get; init; }
}
```

### 3) AttendanceService

```csharp
public class AttendanceService
{
    private readonly IPlayerRepository _players;
    private readonly IAttendanceRepository _attendances;
    private readonly AttendanceRewardPolicy _rewardPolicy;
    private readonly ITimeProvider _time;
    private readonly IUnitOfWork _uow;

    public AttendanceService(
        IPlayerRepository players,
        IAttendanceRepository attendances,
        AttendanceRewardPolicy rewardPolicy,
        ITimeProvider time,
        IUnitOfWork uow)
    {
        _players = players;
        _attendances = attendances;
        _rewardPolicy = rewardPolicy;
        _time = time;
        _uow = uow;
    }

    public async Task<ClaimAttendanceRewardResult> ClaimAsync(ClaimAttendanceRewardCommand cmd, CancellationToken ct = default)
    {
        var now = _time.UtcNow;

        var player = await _players.GetAsync(cmd.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");

        var attendance = await _attendances.GetAsync(cmd.PlayerId, ct)
                        ?? new AttendanceInfo(cmd.PlayerId);

        var todayDay = attendance.GetTodayDay(now);
        var reward = _rewardPolicy.GetRewardForDay(todayDay);

        attendance.MarkChecked(now, todayDay);
        player.AddReward(reward);

        if (attendance.CurrentDay == 1 && attendance.LastCheckedAt == now)
            await _attendances.AddAsync(attendance, ct);
        else
            await _attendances.UpdateAsync(attendance, ct);

        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimAttendanceRewardResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(reward),
            Day = todayDay
        };
    }
}
```

---

## 3-3. Infrastructure 레이어

간단 예시만 적는다.

```csharp
public class AttendanceRepository : IAttendanceRepository
{
    private readonly GameDbContext _db;

    public AttendanceRepository(GameDbContext db) => _db = db;

    public Task<AttendanceInfo?> GetAsync(long playerId, CancellationToken ct = default)
        => _db.Set<AttendanceInfo>().FirstOrDefaultAsync(a => a.PlayerId == playerId, ct);

    public Task AddAsync(AttendanceInfo attendance, CancellationToken ct = default)
        => _db.Set<AttendanceInfo>().AddAsync(attendance, ct).AsTask();

    public Task UpdateAsync(AttendanceInfo attendance, CancellationToken ct = default)
    {
        _db.Update(attendance);
        return Task.CompletedTask;
    }
}
```

---

## 3-4. Api 레이어

### 1) DTO

```csharp
public class ClaimAttendanceRequest
{
    // 보통 요청 바디는 비어있고 인증 정보로 Player를 식별한다
}

public class ClaimAttendanceResponse
{
    public PlayerResponse Player { get; set; } = null!;
    public RewardResponse Reward { get; set; } = null!;
    public int Day { get; set; }
}
```

### 2) Controller

```csharp
[ApiController]
[Route("api/attendance")]
public class AttendanceController : ControllerBase
{
    private readonly AttendanceService _attendanceService;
    private readonly ICurrentPlayerProvider _currentPlayer;

    public AttendanceController(AttendanceService attendanceService, ICurrentPlayerProvider currentPlayer)
    {
        _attendanceService = attendanceService;
        _currentPlayer = currentPlayer;
    }

    [HttpPost("claim")]
    public async Task<ActionResult<ClaimAttendanceResponse>> Claim([FromBody] ClaimAttendanceRequest request)
    {
        var cmd = new ClaimAttendanceRewardCommand
        {
            PlayerId = _currentPlayer.PlayerId
        };

        var result = await _attendanceService.ClaimAsync(cmd);

        var response = new ClaimAttendanceResponse
        {
            Player = PlayerResponse.From(result.Player),
            Reward = RewardResponse.From(result.Reward),
            Day = result.Day
        };

        return Ok(response);
    }
}
```

---

## 3-5. 출석체크 전체 흐름 요약

1. 클라이언트 → `POST /api/attendance/claim`
2. Api → `AttendanceService.ClaimAsync(ClaimAttendanceRewardCommand)` 호출
3. Application → Player/Attendance 로드, 도메인 로직 호출, 보상/날짜 계산, 커밋
4. Domain → 오늘 출석 가능 여부, Day 증가, Reward 결정(Policy)
5. Infrastructure → Attendance/Player 상태 DB 반영
6. Application → ClaimAttendanceRewardResult 반환
7. Api → ClaimAttendanceResponse JSON 응답

---

## 전체적으로 보면

세 유즈케이스 모두 공통 패턴을 따른다.

* **Domain**: 규칙만 알고, 환경을 모른다
* **Application**: 여러 도메인 객체를 조합해 유즈케이스를 완성하고, 트랜잭션을 관리한다
* **Infrastructure**: Repository/UnitOfWork로 실제 DB와 외부 시스템에 붙는다
* **Api**: HTTP ↔ Application 변환만 한다

이 구조를 유지하면:

* 테스트하기 좋고
* 규칙/플로우/인프라가 잘 분리되고
* 수집형 RPG에서 자주 나오는 유즈케이스(가챠, 우편, 출석, 미션, 업적 등)를 계속 추가해도 구조가 크게 흐트러지지 않는다.

원하면 여기에서 하나 골라서 **테스트 코드(유닛/통합)까지 포함한 예제**를 이어서 만들어 줄 수 있다.
  

좋다.
요구한 대로 **4개 유즈케이스(가챠, 스테이지 클리어, 우편 수령, 출석체크 보상)**에 대해

* 도메인 유닛 테스트
* 애플리케이션 유닛 테스트
* Dapper 기반 통합 테스트(간단 버전)

까지 한 번에 정리하겠다.

> 전제: 아래 코드는 “패턴 참고용” 예제라서
> 실제 프로젝트에서는 네임스페이스/파일 분리해서 쓰는 게 좋다.

---

## 0. 전제 인터페이스/도메인 간단 정의

### 0-1. 공통 도메인

```csharp
// Value Object
public readonly struct Currency
{
    public int Gems { get; }
    public int Gold { get; }

    public Currency(int gems, int gold)
    {
        if (gems < 0 || gold <  0)
            throw new ArgumentException();

        Gems = gems;
        Gold = gold;
    }

    public Currency Add(Currency other) => new Currency(Gems + other.Gems, Gold + other.Gold);

    public Currency Subtract(Currency other)
    {
        if (Gems < other.Gems || Gold < other.Gold)
            throw new InvalidOperationException("Not enough currency");
        return new Currency(Gems - other.Gems, Gold - other.Gold);
    }
}

public class Reward
{
    public int Gold { get; }
    public int Gems { get; }

    public Reward(int gold, int gems)
    {
        Gold = gold;
        Gems = gems;
    }

    public Reward Merge(Reward other) => new Reward(Gold + other.Gold, Gems + other.Gems);

    public static Reward GoldOnly(int gold) => new Reward(gold, 0);
    public static Reward GemsOnly(int gems) => new Reward(0, gems);
}
```

### 0-2. Player 도메인 (공통)

```csharp
public class Player
{
    public long Id { get; private set; }
    public int Level { get; private set; }
    public Currency Currency { get; private set; }
    public int Stamina { get; private set; }

    private readonly List<Hero> _heroes = new();
    public IReadOnlyCollection<Hero> Heroes => _heroes;

    protected Player() { }

    public Player(long id)
    {
        Id = id;
        Level = 1;
        Currency = new Currency(0, 0);
        Stamina = 100;
    }

    public void AddReward(Reward reward)
    {
        Currency = Currency.Add(new Currency(reward.Gems, reward.Gold));
    }

    public void ConsumeStamina(int cost)
    {
        if (Stamina < cost)
            throw new InvalidOperationException("Not enough stamina");

        Stamina -= cost;
    }

    public void AddHero(Hero hero) => _heroes.Add(hero);
}

public class Hero
{
    public long Id { get; private set; }
    public string Rarity { get; private set; }
    public int Level { get; private set; }

    protected Hero() { }

    public Hero(long id, string rarity, int level = 1)
    {
        Id = id;
        Rarity = rarity;
        Level = level;
    }
}
```

---

## 1. 도메인 유닛 테스트 (Domain Layer Tests)

xUnit 기준으로 작성한다.

### 1-1. 가챠 도메인

#### 도메인

```csharp
public interface IRandomGenerator
{
    double NextDouble();
}

public class GachaEntry
{
    public long HeroId { get; }
    public string Rarity { get; }
    public double Weight { get; }

    public GachaEntry(long heroId, string rarity, double weight)
    {
        HeroId = heroId;
        Rarity = rarity;
        Weight = weight;
    }
}

public class GachaPool
{
    private readonly List<GachaEntry> _entries;

    public GachaPool(IEnumerable<GachaEntry> entries)
    {
        _entries = entries.ToList();
    }

    public GachaEntry Pick(double r)
    {
        var total = _entries.Sum(e => e.Weight);
        var target = r * total;
        double acc = 0;
        foreach (var e in _entries)
        {
            acc += e.Weight;
            if (target <= acc)
                return e;
        }
        return _entries.Last();
    }
}

public class GachaDomainService
{
    private readonly IRandomGenerator _random;

    public GachaDomainService(IRandomGenerator random) => _random = random;

    public IReadOnlyList<Hero> Roll(GachaPool pool, int count)
    {
        var result = new List<Hero>(count);
        for (int i = 0; i < count; i++)
        {
            var r = _random.NextDouble();
            var entry = pool.Pick(r);
            result.Add(new Hero(entry.HeroId, entry.Rarity));
        }
        return result;
    }
}
```

#### 테스트

```csharp
public class FixedRandom : IRandomGenerator
{
    private readonly Queue<double> _values;

    public FixedRandom(IEnumerable<double> values)
    {
        _values = new Queue<double>(values);
    }

    public double NextDouble() => _values.Dequeue();
}

public class GachaDomainTests
{
    [Fact]
    public void Roll_GivesHeroesCountEqualToRequested()
    {
        var pool = new GachaPool(new[]
        {
            new GachaEntry(1, "R", 1),
            new GachaEntry(2, "SR", 1)
        });

        var random = new FixedRandom(new[] { 0.1, 0.9 });
        var service = new GachaDomainService(random);

        var result = service.Roll(pool, 2);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, h => h.Id == 1);
        Assert.Contains(result, h => h.Id == 2);
    }
}
```

---

### 1-2. 스테이지 클리어 도메인

```csharp
public class Stage
{
    public int Id { get; private set; }
    public int RequiredStamina { get; private set; }
    public Reward BaseReward { get; private set; }
    public int BaseExp { get; private set; }

    protected Stage() { }

    public Stage(int id, int requiredStamina, Reward baseReward, int baseExp)
    {
        Id = id;
        RequiredStamina = requiredStamina;
        BaseReward = baseReward;
        BaseExp = baseExp;
    }
}

public class StageClearResult
{
    public Reward Reward { get; }
    public int GainedExp { get; }

    public StageClearResult(Reward reward, int gainedExp)
    {
        Reward = reward;
        GainedExp = gainedExp;
    }
}

public class StageDomainService
{
    public StageClearResult Clear(Player player, Stage stage, bool isFirstClear)
    {
        player.ConsumeStamina(stage.RequiredStamina);

        var reward = stage.BaseReward;
        if (isFirstClear)
            reward = reward.Merge(Reward.GemsOnly(10)); // 임의 첫 클리어 보너스

        player.AddReward(reward);

        return new StageClearResult(reward, stage.BaseExp);
    }
}
```

#### 테스트

```csharp
public class StageDomainTests
{
    [Fact]
    public void Clear_FirstClear_GivesBonusRewardAndConsumesStamina()
    {
        var player = new Player(1);
        var stage = new Stage(1, requiredStamina: 10, baseReward: Reward.GoldOnly(100), baseExp: 50);
        var service = new StageDomainService();

        var result = service.Clear(player, stage, isFirstClear: true);

        Assert.Equal(90, player.Stamina); // 100 - 10
        Assert.Equal(100, result.Reward.Gold);
        Assert.Equal(10, result.Reward.Gems); // 첫 클리어 보너스
    }
}
```

---

### 1-3. 우편 도메인

```csharp
public class Mail
{
    public long Id { get; private set; }
    public long PlayerId { get; private set; }
    public Reward Reward { get; private set; }
    public bool IsClaimed { get; private set; }
    public DateTime ExpireAt { get; private set; }

    protected Mail() { }

    public Mail(long playerId, Reward reward, DateTime expireAt)
    {
        PlayerId = playerId;
        Reward = reward;
        ExpireAt = expireAt;
        IsClaimed = false;
    }

    public Reward Claim(DateTime now)
    {
        if (IsClaimed)
            throw new InvalidOperationException("Already claimed");
        if (now > ExpireAt)
            throw new InvalidOperationException("Mail expired");

        IsClaimed = true;
        return Reward;
    }
}
```

#### 테스트

```csharp
public class MailDomainTests
{
    [Fact]
    public void Claim_FirstTime_ReturnsRewardAndMarksClaimed()
    {
        var mail = new Mail(1, Reward.GoldOnly(1000), DateTime.UtcNow.AddDays(1));

        var reward = mail.Claim(DateTime.UtcNow);

        Assert.True(mail.IsClaimed);
        Assert.Equal(1000, reward.Gold);
    }

    [Fact]
    public void Claim_Twice_Throws()
    {
        var mail = new Mail(1, Reward.GoldOnly(1000), DateTime.UtcNow.AddDays(1));
        mail.Claim(DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => mail.Claim(DateTime.UtcNow));
    }
}
```

---

### 1-4. 출석체크 도메인

```csharp
public class AttendanceInfo
{
    public long PlayerId { get; private set; }
    public int CurrentDay { get; private set; }
    public DateTime LastCheckedAt { get; private set; }

    protected AttendanceInfo() { }

    public AttendanceInfo(long playerId)
    {
        PlayerId = playerId;
        CurrentDay = 0;
        LastCheckedAt = DateTime.MinValue;
    }

    public int GetNextDay() => CurrentDay + 1;

    public void MarkChecked(DateTime now, int day)
    {
        if (day != CurrentDay + 1)
            throw new InvalidOperationException("Invalid day");

        CurrentDay = day;
        LastCheckedAt = now;
    }
}

public class AttendanceRewardPolicy
{
    public Reward GetRewardForDay(int day) =>
        day switch
        {
            1 => Reward.GoldOnly(1000),
            2 => Reward.GemsOnly(10),
            _ => Reward.GoldOnly(500)
        };
}
```

#### 테스트

```csharp
public class AttendanceDomainTests
{
    [Fact]
    public void MarkChecked_IncrementsDay()
    {
        var attendance = new AttendanceInfo(1);

        var nextDay = attendance.GetNextDay();
        attendance.MarkChecked(DateTime.UtcNow, nextDay);

        Assert.Equal(1, attendance.CurrentDay);
    }

    [Fact]
    public void GetRewardForDay_ReturnsDifferentRewards()
    {
        var policy = new AttendanceRewardPolicy();

        var r1 = policy.GetRewardForDay(1);
        var r2 = policy.GetRewardForDay(2);

        Assert.Equal(1000, r1.Gold);
        Assert.Equal(10, r2.Gems);
    }
}
```

---

## 2. 애플리케이션 유닛 테스트 (Application Layer Tests)

Moq를 사용해 레포지토리/UnitOfWork만 Mock 한다.

### 공통 인터페이스

```csharp
public interface IPlayerRepository
{
    Task<Player?> GetAsync(long id, CancellationToken ct = default);
    Task UpdateAsync(Player player, CancellationToken ct = default);
}

public interface IGachaRepository
{
    Task<GachaPool?> GetPoolAsync(int poolId, CancellationToken ct = default);
}

public interface IStageRepository
{
    Task<Stage?> GetAsync(int id, CancellationToken ct = default);
    Task<bool> HasClearedAsync(long playerId, int stageId, CancellationToken ct = default);
    Task MarkClearedAsync(long playerId, int stageId, CancellationToken ct = default);
}

public interface IMailRepository
{
    Task<Mail?> GetAsync(long mailId, CancellationToken ct = default);
    Task UpdateAsync(Mail mail, CancellationToken ct = default);
}

public interface IAttendanceRepository
{
    Task<AttendanceInfo?> GetAsync(long playerId, CancellationToken ct = default);
    Task AddAsync(AttendanceInfo attendance, CancellationToken ct = default);
    Task UpdateAsync(AttendanceInfo attendance, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}
```

DTO는 간단하게 만든다.

```csharp
public class PlayerDto
{
    public long Id { get; set; }
    public int Level { get; set; }
    public int Gems { get; set; }
    public int Gold { get; set; }

    public static PlayerDto From(Player p) =>
        new PlayerDto { Id = p.Id, Level = p.Level, Gems = p.Currency.Gems, Gold = p.Currency.Gold };
}

public class HeroDto
{
    public long HeroId { get; set; }
    public string Rarity { get; set; } = "";
    public int Level { get; set; }

    public static HeroDto From(Hero h) =>
        new HeroDto { HeroId = h.Id, Rarity = h.Rarity, Level = h.Level };
}

public class RewardDto
{
    public int Gold { get; set; }
    public int Gems { get; set; }

    public static RewardDto From(Reward r) => new RewardDto { Gold = r.Gold, Gems = r.Gems };
}
```

---

### 2-1. 가챠 Application

```csharp
public class RollGachaCommand
{
    public long PlayerId { get; init; }
    public int PoolId { get; init; }
    public int Count { get; init; }
}

public class RollGachaResult
{
    public PlayerDto Player { get; init; } = null!;
    public IReadOnlyList<HeroDto> Heroes { get; init; } = Array.Empty<HeroDto>();
}

public class GachaService
{
    private readonly IPlayerRepository _players;
    private readonly IGachaRepository _gacha;
    private readonly IUnitOfWork _uow;
    private readonly GachaDomainService _domain;

    public GachaService(IPlayerRepository players, IGachaRepository gacha, IUnitOfWork uow, GachaDomainService domain)
    {
        _players = players;
        _gacha = gacha;
        _uow = uow;
        _domain = domain;
    }

    public async Task<RollGachaResult> RollAsync(RollGachaCommand cmd, CancellationToken ct = default)
    {
        var player = await _players.GetAsync(cmd.PlayerId, ct) ?? throw new InvalidOperationException("Player not found");
        var pool = await _gacha.GetPoolAsync(cmd.PoolId, ct) ?? throw new InvalidOperationException("Pool not found");

        // 비용은 간단히 Count * 300 Gems 라고 가정
        var cost = new Currency(cmd.Count * 300, 0);
        player.Currency = player.Currency.Subtract(cost);

        var heroes = _domain.Roll(pool, cmd.Count);
        foreach (var h in heroes)
            player.AddHero(h);

        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new RollGachaResult
        {
            Player = PlayerDto.From(player),
            Heroes = heroes.Select(HeroDto.From).ToList()
        };
    }
}
```

#### 테스트

```csharp
public class GachaServiceTests
{
    [Fact]
    public async Task RollAsync_SubtractsCurrency_AndAddsHeroes()
    {
        var player = new Player(1);
        player.Currency = new Currency(1000, 0);

        var pool = new GachaPool(new[]
        {
            new GachaEntry(1, "R", 1)
        });

        var playerRepo = new Mock<IPlayerRepository>();
        playerRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(player);

        var gachaRepo = new Mock<IGachaRepository>();
        gachaRepo.Setup(r => r.GetPoolAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(pool);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        var random = new FixedRandom(new[] { 0.1 });
        var domain = new GachaDomainService(random);

        var service = new GachaService(playerRepo.Object, gachaRepo.Object, uow.Object, domain);

        var result = await service.RollAsync(new RollGachaCommand
        {
            PlayerId = 1,
            PoolId = 1,
            Count = 1
        });

        Assert.Single(result.Heroes);
        Assert.Equal(700, result.Player.Gems); // 1000 - 300
        playerRepo.Verify(r => r.UpdateAsync(player, It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

### 2-2. 스테이지 Application

앞에서 만든 StageDomainService 재사용.

```csharp
public class ClearStageCommand
{
    public long PlayerId { get; init; }
    public int StageId { get; init; }
    public bool IsSuccess { get; init; }
}

public class ClearStageResultDto
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}

public class StageService
{
    private readonly IPlayerRepository _players;
    private readonly IStageRepository _stages;
    private readonly StageDomainService _domain;
    private readonly IUnitOfWork _uow;

    public StageService(IPlayerRepository players, IStageRepository stages, StageDomainService domain, IUnitOfWork uow)
    {
        _players = players;
        _stages = stages;
        _domain = domain;
        _uow = uow;
    }

    public async Task<ClearStageResultDto> ClearAsync(ClearStageCommand cmd, CancellationToken ct = default)
    {
        if (!cmd.IsSuccess)
            throw new InvalidOperationException("Stage not cleared");

        var player = await _players.GetAsync(cmd.PlayerId, ct) ?? throw new InvalidOperationException("Player not found");
        var stage = await _stages.GetAsync(cmd.StageId, ct) ?? throw new InvalidOperationException("Stage not found");

        var firstClear = !await _stages.HasClearedAsync(cmd.PlayerId, cmd.StageId, ct);

        var result = _domain.Clear(player, stage, firstClear);

        await _players.UpdateAsync(player, ct);
        if (firstClear)
            await _stages.MarkClearedAsync(player.Id, stage.Id, ct);

        await _uow.CommitAsync(ct);

        return new ClearStageResultDto
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(result.Reward)
        };
    }
}
```

#### 테스트

```csharp
public class StageServiceTests
{
    [Fact]
    public async Task ClearAsync_FirstClear_MarksClearedAndUpdatesPlayer()
    {
        var player = new Player(1);
        var stage = new Stage(1, 10, Reward.GoldOnly(100), 50);

        var playerRepo = new Mock<IPlayerRepository>();
        playerRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(player);

        var stageRepo = new Mock<IStageRepository>();
        stageRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(stage);
        stageRepo.Setup(r => r.HasClearedAsync(1, 1, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        var domain = new StageDomainService();
        var service = new StageService(playerRepo.Object, stageRepo.Object, domain, uow.Object);

        var result = await service.ClearAsync(new ClearStageCommand
        {
            PlayerId = 1,
            StageId = 1,
            IsSuccess = true
        });

        Assert.Equal(90, result.Player.Stamina);
        Assert.Equal(100, result.Reward.Gold);
        stageRepo.Verify(r => r.MarkClearedAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

### 2-3. 우편 Application

```csharp
public class ClaimMailCommand
{
    public long PlayerId { get; init; }
    public long MailId { get; init; }
}

public class ClaimMailResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}

public class MailService
{
    private readonly IMailRepository _mails;
    private readonly IPlayerRepository _players;
    private readonly ITimeProvider _time;
    private readonly IUnitOfWork _uow;

    public MailService(IMailRepository mails, IPlayerRepository players, ITimeProvider time, IUnitOfWork uow)
    {
        _mails = mails;
        _players = players;
        _time = time;
        _uow = uow;
    }

    public async Task<ClaimMailResult> ClaimAsync(ClaimMailCommand cmd, CancellationToken ct = default)
    {
        var player = await _players.GetAsync(cmd.PlayerId, ct) ?? throw new InvalidOperationException("Player not found");
        var mail = await _mails.GetAsync(cmd.MailId, ct) ?? throw new InvalidOperationException("Mail not found");

        if (mail.PlayerId != player.Id)
            throw new InvalidOperationException("Mail does not belong to player");

        var reward = mail.Claim(_time.UtcNow);
        player.AddReward(reward);

        await _mails.UpdateAsync(mail, ct);
        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimMailResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(reward)
        };
    }
}
```

#### 테스트

```csharp
public class MailServiceTests
{
    [Fact]
    public async Task ClaimAsync_AddsRewardAndMarksMailClaimed()
    {
        var player = new Player(1);
        var mail = new Mail(1, Reward.GemsOnly(10), DateTime.UtcNow.AddDays(1));

        var playerRepo = new Mock<IPlayerRepository>();
        playerRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(player);

        var mailRepo = new Mock<IMailRepository>();
        mailRepo.Setup(r => r.GetAsync(100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mail);

        var time = new Mock<ITimeProvider>();
        time.SetupGet(t => t.UtcNow).Returns(DateTime.UtcNow);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        var service = new MailService(mailRepo.Object, playerRepo.Object, time.Object, uow.Object);

        var result = await service.ClaimAsync(new ClaimMailCommand { PlayerId = 1, MailId = 100 });

        Assert.Equal(10, result.Reward.Gems);
        Assert.True(mail.IsClaimed);
    }
}
```

---

### 2-4. 출석체크 Application

```csharp
public class ClaimAttendanceRewardCommand
{
    public long PlayerId { get; init; }
}

public class ClaimAttendanceRewardResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
    public int Day { get; init; }
}

public class AttendanceService
{
    private readonly IPlayerRepository _players;
    private readonly IAttendanceRepository _attendances;
    private readonly AttendanceRewardPolicy _policy;
    private readonly ITimeProvider _time;
    private readonly IUnitOfWork _uow;

    public AttendanceService(
        IPlayerRepository players,
        IAttendanceRepository attendances,
        AttendanceRewardPolicy policy,
        ITimeProvider time,
        IUnitOfWork uow)
    {
        _players = players;
        _attendances = attendances;
        _policy = policy;
        _time = time;
        _uow = uow;
    }

    public async Task<ClaimAttendanceRewardResult> ClaimAsync(ClaimAttendanceRewardCommand cmd, CancellationToken ct = default)
    {
        var now = _time.UtcNow;

        var player = await _players.GetAsync(cmd.PlayerId, ct) ?? throw new InvalidOperationException("Player not found");
        var attendance = await _attendances.GetAsync(cmd.PlayerId, ct) ?? new AttendanceInfo(cmd.PlayerId);

        var day = attendance.GetNextDay();
        var reward = _policy.GetRewardForDay(day);

        attendance.MarkChecked(now, day);
        player.AddReward(reward);

        if (day == 1 && attendance.LastCheckedAt == now)
            await _attendances.AddAsync(attendance, ct);
        else
            await _attendances.UpdateAsync(attendance, ct);

        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimAttendanceRewardResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(reward),
            Day = day
        };
    }
}
```

#### 테스트

```csharp
public class AttendanceServiceTests
{
    [Fact]
    public async Task ClaimAsync_FirstDay_AddsRewardAndCreatesAttendance()
    {
        var player = new Player(1);

        var playerRepo = new Mock<IPlayerRepository>();
        playerRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(player);

        var attRepo = new Mock<IAttendanceRepository>();
        attRepo.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
               .ReturnsAsync((AttendanceInfo?)null);

        var policy = new AttendanceRewardPolicy();

        var time = new Mock<ITimeProvider>();
        time.SetupGet(t => t.UtcNow).Returns(DateTime.UtcNow);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1);

        var service = new AttendanceService(playerRepo.Object, attRepo.Object, policy, time.Object, uow.Object);

        var result = await service.ClaimAsync(new ClaimAttendanceRewardCommand { PlayerId = 1 });

        Assert.Equal(1, result.Day);
        Assert.Equal(1000, result.Reward.Gold); // 정책상 1일차 1000 골드
        attRepo.Verify(r => r.AddAsync(It.IsAny<AttendanceInfo>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

## 3. Dapper 기반 통합 테스트 (Integration Tests with Dapper)

실제 DB 대신 **SQLite In-Memory**를 사용해 Dapper로 통합 테스트 예제를 보여준다.
(실제 MySQL로 바꾸려면 연결 문자열과 DDL만 바꾸면 된다.)

### 3-1. Dapper용 Repository 예시 (Player + Stage)

```csharp
using System.Data;
using Dapper;

public class DapperPlayerRepository : IPlayerRepository
{
    private readonly IDbConnection _conn;

    public DapperPlayerRepository(IDbConnection conn) => _conn = conn;

    public async Task<Player?> GetAsync(long id, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT Id, Level, Gems, Gold, Stamina FROM Players WHERE Id = @Id",
            new { Id = id });

        if (row == null) return null;

        var p = new Player((long)row.Id);
        p.GetType().GetProperty("Level")!.SetValue(p, (int)row.Level);
        p.GetType().GetProperty("Currency")!.SetValue(p, new Currency((int)row.Gems, (int)row.Gold));
        p.GetType().GetProperty("Stamina")!.SetValue(p, (int)row.Stamina);
        return p;
    }

    public async Task UpdateAsync(Player player, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            "UPDATE Players SET Level = @Level, Gems = @Gems, Gold = @Gold, Stamina = @Stamina WHERE Id = @Id",
            new
            {
                Id = player.Id,
                Level = player.Level,
                Gems = player.Currency.Gems,
                Gold = player.Currency.Gold,
                Stamina = player.Stamina
            });
    }
}

public class DapperStageRepository : IStageRepository
{
    private readonly IDbConnection _conn;

    public DapperStageRepository(IDbConnection conn) => _conn = conn;

    public async Task<Stage?> GetAsync(int id, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT Id, RequiredStamina, RewardGold, RewardGems, BaseExp FROM Stages WHERE Id = @Id",
            new { Id = id });

        if (row == null) return null;

        var reward = new Reward((int)row.RewardGold, (int)row.RewardGems);
        return new Stage((int)row.Id, (int)row.RequiredStamina, reward, (int)row.BaseExp);
    }

    public async Task<bool> HasClearedAsync(long playerId, int stageId, CancellationToken ct = default)
    {
        var count = await _conn.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM PlayerStageClears WHERE PlayerId = @PlayerId AND StageId = @StageId",
            new { PlayerId = playerId, StageId = stageId });
        return count > 0;
    }

    public async Task MarkClearedAsync(long playerId, int stageId, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            "INSERT INTO PlayerStageClears(PlayerId, StageId, ClearedAt) VALUES(@PlayerId, @StageId, @ClearedAt)",
            new { PlayerId = playerId, StageId = stageId, ClearedAt = DateTime.UtcNow });
    }
}

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _conn;
    private readonly IDbTransaction _tx;

    public DapperUnitOfWork(IDbConnection conn, IDbTransaction tx)
    {
        _conn = conn;
        _tx = tx;
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        _tx.Commit();
        return Task.FromResult(0);
    }
}
```

> 통합 테스트에서는 보통 트랜잭션을 테스트마다 열고 끝에 롤백하는 패턴을 많이 쓴다.
> 여기서는 단순하게 Commit만 보이도록 만들었다.

---

### 3-2. 스테이지 클리어 통합 테스트 예시 (Dapper + SQLite InMemory)

```csharp
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using Xunit;

public class StageIntegrationTests : IAsyncLifetime
{
    private IDbConnection _conn = null!;
    private IDbTransaction _tx = null!;
    private StageService _service = null!;

    public async Task InitializeAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        _conn = connection;

        // 스키마 생성
        await _conn.ExecuteAsync(@"
CREATE TABLE Players(
    Id INTEGER PRIMARY KEY,
    Level INTEGER NOT NULL,
    Gems INTEGER NOT NULL,
    Gold INTEGER NOT NULL,
    Stamina INTEGER NOT NULL
);
CREATE TABLE Stages(
    Id INTEGER PRIMARY KEY,
    RequiredStamina INTEGER NOT NULL,
    RewardGold INTEGER NOT NULL,
    RewardGems INTEGER NOT NULL,
    BaseExp INTEGER NOT NULL
);
CREATE TABLE PlayerStageClears(
    PlayerId INTEGER NOT NULL,
    StageId INTEGER NOT NULL,
    ClearedAt TEXT NOT NULL
);");

        // 초기 데이터
        await _conn.ExecuteAsync(
            "INSERT INTO Players(Id, Level, Gems, Gold, Stamina) VALUES(1, 1, 0, 0, 100);");
        await _conn.ExecuteAsync(
            "INSERT INTO Stages(Id, RequiredStamina, RewardGold, RewardGems, BaseExp) VALUES(1, 10, 100, 0, 50);");

        _tx = _conn.BeginTransaction();

        var playerRepo = new DapperPlayerRepository(_conn);
        var stageRepo = new DapperStageRepository(_conn);
        var uow = new DapperUnitOfWork(_conn, _tx);
        var domain = new StageDomainService();

        _service = new StageService(playerRepo, stageRepo, domain, uow);
    }

    public Task DisposeAsync()
    {
        _tx.Dispose();
        _conn.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ClearStage_UpdatesPlayerAndInsertsClearRecord()
    {
        var result = await _service.ClearAsync(new ClearStageCommand
        {
            PlayerId = 1,
            StageId = 1,
            IsSuccess = true
        });

        // 플레이어 상태 확인
        var playerRow = await _conn.QuerySingleAsync<dynamic>("SELECT * FROM Players WHERE Id = 1");
        Assert.Equal(90, (int)playerRow.Stamina);
        Assert.Equal(100, (int)playerRow.Gold);

        // 클리어 기록 확인
        var clearCount = await _conn.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM PlayerStageClears WHERE PlayerId = 1 AND StageId = 1");
        Assert.Equal(1, clearCount);
    }
}
```

---

### 3-3. 나머지 유즈케이스 통합 테스트 패턴

가챠 / 우편 / 출석체크도 패턴은 같다.

1. SQLite InMemory + Dapper로 테이블/데이터 준비
2. Dapper 기반 Repository 구현 (Mail, Attendance, GachaPool 등)
3. UnitOfWork/트랜잭션 시작
4. Application 서비스 호출
5. DB 상태를 직접 SELECT해서 검증

예를 들어 우편 수령 통합 테스트는:

* `Mails` 테이블, `Players` 테이블 생성
* `Mail` 행에 `IsClaimed = 0`, `RewardGold/Gems` 등 세팅
* `MailService.ClaimAsync` 호출
* `Mails`에서 `IsClaimed`가 1로 바뀌었는지, `Players`의 골드/젬이 증가했는지 확인

출석체크도:

* `AttendanceInfos` 테이블 준비
* 기존 레코드 없는 상태에서 호출 → 1일차 추가 + 보상 지급
* 기존 레코드 있는 상태에서 호출 → 일 수 증가 + 보상 지급

이런 식으로 Dapper Repository + 실제 DB 상태 검증으로 가져가면 된다.

---

여기까지가

* **4개 유즈케이스(가챠, 스테이지, 우편, 출석)**에 대한

  * 도메인 유닛 테스트
  * 애플리케이션 유닛 테스트
  * Dapper 기반 통합 테스트 패턴 예시

전체 흐름이다.

원하면 특정 유즈케이스 하나를 골라서,
**API Controller까지 포함한 end-to-end 테스트(WebApplicationFactory 이용)** 예제도 이어서 만들어 줄 수 있다.

  
좋다, 바로 **우편 수령 + 출석체크 보상** Dapper 통합테스트 예제를 만들어보겠다.
전제는 앞에서 만든 도메인/애플리케이션 레이어(Mail, AttendanceInfo, MailService, AttendanceService, IPlayerRepository 등)가 이미 정의되어 있다는 것으로 두겠다.

여기서는 **Infrastructure(Dapper Repo) + 통합 테스트 코드**만 집중해서 보여준다.
DB는 SQLite In-Memory + Dapper를 사용한다.

---

## 1. 공통: Player용 Dapper Repository & UnitOfWork

우편/출석 둘 다 Player 재화를 갱신해야 하므로 공통으로 쓸 수 있는 Dapper 구현을 먼저 둔다.

```csharp
using System.Data;
using Dapper;

public class DapperPlayerRepository : IPlayerRepository
{
    private readonly IDbConnection _conn;

    public DapperPlayerRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<Player?> GetAsync(long id, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT Id, Level, Gems, Gold, Stamina FROM Players WHERE Id = @Id",
            new { Id = id });

        if (row == null) return null;

        var p = new Player((long)row.Id);
        p.GetType().GetProperty("Level")!.SetValue(p, (int)row.Level);
        p.GetType().GetProperty("Currency")!.SetValue(p, new Currency((int)row.Gems, (int)row.Gold));
        p.GetType().GetProperty("Stamina")!.SetValue(p, (int)row.Stamina);

        return p;
    }

    public async Task UpdateAsync(Player player, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE Players
              SET Level = @Level,
                  Gems  = @Gems,
                  Gold  = @Gold,
                  Stamina = @Stamina
              WHERE Id = @Id",
            new
            {
                Id = player.Id,
                Level = player.Level,
                Gems = player.Currency.Gems,
                Gold = player.Currency.Gold,
                Stamina = player.Stamina
            });
    }
}

public class DapperUnitOfWork : IUnitOfWork
{
    private readonly IDbTransaction _tx;

    public DapperUnitOfWork(IDbTransaction tx)
    {
        _tx = tx;
    }

    public Task<int> CommitAsync(CancellationToken ct = default)
    {
        _tx.Commit();
        return Task.FromResult(0);
    }
}
```

테스트마다 트랜잭션을 열고, 끝나면 `Dispose`에서 롤백하는 패턴을 써도 되고, 여기처럼 `Commit`만 해도 된다(예제 단순화를 위해 Commit만 둔다).

---

## 2. 우편 수령: Dapper Repository + 통합 테스트

### 2.1. 우편 테이블 스키마

SQLite In-Memory에서 사용할 간단한 스키마다.

```sql
CREATE TABLE Players(
    Id      INTEGER PRIMARY KEY,
    Level   INTEGER NOT NULL,
    Gems    INTEGER NOT NULL,
    Gold    INTEGER NOT NULL,
    Stamina INTEGER NOT NULL
);

CREATE TABLE Mails(
    Id          INTEGER PRIMARY KEY,
    PlayerId    INTEGER NOT NULL,
    RewardGold  INTEGER NOT NULL,
    RewardGems  INTEGER NOT NULL,
    IsClaimed   INTEGER NOT NULL,
    ExpireAt    TEXT    NOT NULL
);
```

* `IsClaimed`는 0/1 정수로 두고, Dapper에서 bool처럼 다룬다
* `ExpireAt`는 TEXT(ISO8601)로 저장한다

### 2.2. DapperMailRepository 구현

앞에서 만든 `Mail` 도메인 클래스를 DB와 연결하는 Repository다.

```csharp
public class DapperMailRepository : IMailRepository
{
    private readonly IDbConnection _conn;

    public DapperMailRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<Mail?> GetAsync(long mailId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT Id, PlayerId, RewardGold, RewardGems, IsClaimed, ExpireAt
              FROM Mails
              WHERE Id = @Id",
            new { Id = mailId });

        if (row == null) return null;

        var reward = new Reward((int)row.RewardGold, (int)row.RewardGems);
        var mail = new Mail((long)row.PlayerId, reward, DateTime.Parse((string)row.ExpireAt));
        mail.GetType().GetProperty("Id")!.SetValue(mail, (long)row.Id);
        mail.GetType().GetProperty("IsClaimed")!.SetValue(mail, ((long)row.IsClaimed) != 0);

        return mail;
    }

    public async Task UpdateAsync(Mail mail, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE Mails
              SET IsClaimed = @IsClaimed
              WHERE Id = @Id",
            new
            {
                Id = mail.Id,
                IsClaimed = mail.IsClaimed ? 1 : 0
            });
    }
}
```

필요하면 RewardGold/RewardGems도 업데이트하지만, 여기서는 수령 후 reward 자체는 안 바뀐다고 보고 `IsClaimed`만 갱신한다.

### 2.3. TimeProvider 구현 (테스트용)

```csharp
public class FixedTimeProvider : ITimeProvider
{
    public DateTime UtcNow { get; }

    public FixedTimeProvider(DateTime utcNow)
    {
        UtcNow = utcNow;
    }
}
```

### 2.4. 우편 수령 통합 테스트

```csharp
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using Xunit;

public class MailIntegrationTests : IAsyncLifetime
{
    private IDbConnection _conn = null!;
    private IDbTransaction _tx = null!;
    private MailService _service = null!;

    public async Task InitializeAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        _conn = connection;

        // 스키마 생성
        await _conn.ExecuteAsync(@"
CREATE TABLE Players(
    Id      INTEGER PRIMARY KEY,
    Level   INTEGER NOT NULL,
    Gems    INTEGER NOT NULL,
    Gold    INTEGER NOT NULL,
    Stamina INTEGER NOT NULL
);
CREATE TABLE Mails(
    Id          INTEGER PRIMARY KEY,
    PlayerId    INTEGER NOT NULL,
    RewardGold  INTEGER NOT NULL,
    RewardGems  INTEGER NOT NULL,
    IsClaimed   INTEGER NOT NULL,
    ExpireAt    TEXT    NOT NULL
);");

        // 초기 데이터
        await _conn.ExecuteAsync(
            "INSERT INTO Players(Id, Level, Gems, Gold, Stamina) VALUES(1, 1, 0, 0, 100);");

        var expireAt = DateTime.UtcNow.AddDays(1).ToString("O");
        await _conn.ExecuteAsync(
            @"INSERT INTO Mails(Id, PlayerId, RewardGold, RewardGems, IsClaimed, ExpireAt)
              VALUES(100, 1, 500, 10, 0, @ExpireAt);",
            new { ExpireAt = expireAt });

        _tx = _conn.BeginTransaction();

        var playerRepo = new DapperPlayerRepository(_conn);
        var mailRepo = new DapperMailRepository(_conn);
        var time = new FixedTimeProvider(DateTime.UtcNow);
        var uow = new DapperUnitOfWork(_tx);

        _service = new MailService(mailRepo, playerRepo, time, uow);
    }

    public Task DisposeAsync()
    {
        _tx.Dispose();
        _conn.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ClaimMail_UpdatesMailAndPlayer()
    {
        var result = await _service.ClaimAsync(new ClaimMailCommand
        {
            PlayerId = 1,
            MailId = 100
        });

        // 플레이어 재화 검증
        var playerRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT Gems, Gold FROM Players WHERE Id = 1");
        Assert.Equal(10, (int)playerRow.Gems);
        Assert.Equal(500, (int)playerRow.Gold);

        // 우편 상태 검증
        var mailRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT IsClaimed FROM Mails WHERE Id = 100");
        Assert.Equal(1L, (long)mailRow.IsClaimed);
    }
}
```

이 테스트는 실제 흐름이 이렇게 된다.

* Dapper로 DB에 Player/Mail 입력
* `MailService.ClaimAsync` 호출
* Mail 도메인의 `Claim` 호출 + Player.AddReward
* DapperMailRepository/DapperPlayerRepository가 DB 상태 갱신
* 마지막에 DB에서 SELECT 해서 변경 결과 검증

---

## 3. 출석체크: Dapper Repository + 통합 테스트

### 3.1. 출석 테이블 스키마

```sql
CREATE TABLE Players(
    Id      INTEGER PRIMARY KEY,
    Level   INTEGER NOT NULL,
    Gems    INTEGER NOT NULL,
    Gold    INTEGER NOT NULL,
    Stamina INTEGER NOT NULL
);

CREATE TABLE AttendanceInfos(
    PlayerId      INTEGER PRIMARY KEY,
    CurrentDay    INTEGER NOT NULL,
    LastCheckedAt TEXT    NOT NULL
);
```

### 3.2. DapperAttendanceRepository 구현

```csharp
public class DapperAttendanceRepository : IAttendanceRepository
{
    private readonly IDbConnection _conn;

    public DapperAttendanceRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<AttendanceInfo?> GetAsync(long playerId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT PlayerId, CurrentDay, LastCheckedAt
              FROM AttendanceInfos
              WHERE PlayerId = @PlayerId",
            new { PlayerId = playerId });

        if (row == null) return null;

        var info = new AttendanceInfo((long)row.PlayerId);
        info.GetType().GetProperty("CurrentDay")!.SetValue(info, (int)row.CurrentDay);
        info.GetType().GetProperty("LastCheckedAt")!.SetValue(info, DateTime.Parse((string)row.LastCheckedAt));

        return info;
    }

    public async Task AddAsync(AttendanceInfo attendance, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"INSERT INTO AttendanceInfos(PlayerId, CurrentDay, LastCheckedAt)
              VALUES(@PlayerId, @CurrentDay, @LastCheckedAt)",
            new
            {
                PlayerId = attendance.PlayerId,
                CurrentDay = attendance.CurrentDay,
                LastCheckedAt = attendance.LastCheckedAt.ToString("O")
            });
    }

    public async Task UpdateAsync(AttendanceInfo attendance, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE AttendanceInfos
              SET CurrentDay = @CurrentDay,
                  LastCheckedAt = @LastCheckedAt
              WHERE PlayerId = @PlayerId",
            new
            {
                PlayerId = attendance.PlayerId,
                CurrentDay = attendance.CurrentDay,
                LastCheckedAt = attendance.LastCheckedAt.ToString("O")
            });
    }
}
```

### 3.3. 출석체크 보상 통합 테스트

```csharp
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using Xunit;

public class AttendanceIntegrationTests : IAsyncLifetime
{
    private IDbConnection _conn = null!;
    private IDbTransaction _tx = null!;
    private AttendanceService _service = null!;

    public async Task InitializeAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        _conn = connection;

        await _conn.ExecuteAsync(@"
CREATE TABLE Players(
    Id      INTEGER PRIMARY KEY,
    Level   INTEGER NOT NULL,
    Gems    INTEGER NOT NULL,
    Gold    INTEGER NOT NULL,
    Stamina INTEGER NOT NULL
);
CREATE TABLE AttendanceInfos(
    PlayerId      INTEGER PRIMARY KEY,
    CurrentDay    INTEGER NOT NULL,
    LastCheckedAt TEXT    NOT NULL
);");

        await _conn.ExecuteAsync(
            "INSERT INTO Players(Id, Level, Gems, Gold, Stamina) VALUES(1, 1, 0, 0, 100);");

        _tx = _conn.BeginTransaction();

        var playerRepo = new DapperPlayerRepository(_conn);
        var attRepo = new DapperAttendanceRepository(_conn);
        var policy = new AttendanceRewardPolicy();
        var time = new FixedTimeProvider(DateTime.UtcNow);
        var uow = new DapperUnitOfWork(_tx);

        _service = new AttendanceService(playerRepo, attRepo, policy, time, uow);
    }

    public Task DisposeAsync()
    {
        _tx.Dispose();
        _conn.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ClaimAttendance_FirstDay_InsertsRecordAndGivesReward()
    {
        var result = await _service.ClaimAsync(new ClaimAttendanceRewardCommand
        {
            PlayerId = 1
        });

        // Player 재화 확인 (1일차 = 1000 골드라고 가정)
        var playerRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT Gems, Gold FROM Players WHERE Id = 1");
        Assert.Equal(0, (int)playerRow.Gems);
        Assert.Equal(1000, (int)playerRow.Gold);

        // Attendance 레코드 확인
        var attRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT CurrentDay FROM AttendanceInfos WHERE PlayerId = 1");
        Assert.Equal(1, (int)attRow.CurrentDay);
    }

    [Fact]
    public async Task ClaimAttendance_SecondDay_UpdatesRecordAndGivesReward()
    {
        // 1일차 선지급
        await _service.ClaimAsync(new ClaimAttendanceRewardCommand { PlayerId = 1 });

        // 트랜잭션 안에서 같은 커넥션을 쓰고 있으니 AttendanceInfos에 이미 1일차가 들어가 있다
        var result = await _service.ClaimAsync(new ClaimAttendanceRewardCommand { PlayerId = 1 });

        // 2일차 = 10 Gems라고 가정
        var playerRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT Gems, Gold FROM Players WHERE Id = 1");
        Assert.Equal(10, (int)playerRow.Gems);
        Assert.Equal(1000 + 0, (int)playerRow.Gold); // 1일차 골드 + 2일차 젬

        var attRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT CurrentDay FROM AttendanceInfos WHERE PlayerId = 1");
        Assert.Equal(2, (int)attRow.CurrentDay);
    }
}
```

---

## 4. 정리

요청한 것처럼 **우편 수령 / 출석체크 보상** 유즈케이스에 대해:

* **Dapper 기반 Repository 구현**

  * `DapperMailRepository`
  * `DapperAttendanceRepository`
* **공통 Player용 DapperRepository + UnitOfWork**
* **SQLite In-Memory + Dapper를 사용하는 통합 테스트 코드**

까지 한 세트로 만들었다.

이 패턴을 그대로 복사해서
가챠, 스테이지, 기타 유즈케이스(예: 미션, 업적, 이벤트 보상)도 같은 방식으로 확장하면 된다.


좋다. 이번에는 **미션, 업적, 이벤트 보상**까지 포함해서,

* 도메인 엔티티
* 애플리케이션 서비스(유즈케이스)
* Dapper 기반 Repository
* 간단한 통합 테스트 예제

를 한 번에 만들어보겠다.

이미 앞에서 `Player`, `Reward`, `Currency`, `IPlayerRepository`, `IUnitOfWork`, `ITimeProvider` 등은 정의되어 있다고 가정한다
(없다면 이전 답변 코드와 그대로 이어서 보면 된다).

---

# 1. 미션 (Mission) 유즈케이스

## 1-1. 도메인 (MyApp.Domain)

### Mission 엔티티

* 특정 미션은 “진행도(Progress)”를 올리고, 완료되면 보상을 받을 수 있다고 가정한다
* 미션 보상은 한 번만 수령 가능하다고 본다

```csharp
public enum MissionType
{
    Daily,
    Weekly,
    Normal
}

public class Mission
{
    public long Id { get; private set; }
    public long PlayerId { get; private set; }
    public MissionType Type { get; private set; }
    public int TargetCount { get; private set; }
    public int CurrentCount { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsClaimed { get; private set; }
    public Reward Reward { get; private set; }

    protected Mission() { }

    public Mission(long playerId, MissionType type, int targetCount, Reward reward)
    {
        PlayerId = playerId;
        Type = type;
        TargetCount = targetCount;
        Reward = reward;
        CurrentCount = 0;
        IsCompleted = false;
        IsClaimed = false;
    }

    public void AddProgress(int amount)
    {
        if (IsCompleted)
            return;

        CurrentCount += amount;
        if (CurrentCount >= TargetCount)
            IsCompleted = true;
    }

    public Reward Claim()
    {
        if (!IsCompleted)
            throw new InvalidOperationException("Mission not completed");

        if (IsClaimed)
            throw new InvalidOperationException("Mission already claimed");

        IsClaimed = true;
        return Reward;
    }
}
```

---

## 1-2. 애플리케이션 (MyApp.Application)

### 인터페이스

```csharp
public interface IMissionRepository
{
    Task<Mission?> GetAsync(long missionId, CancellationToken ct = default);
    Task UpdateAsync(Mission mission, CancellationToken ct = default);
}
```

### Command / Result DTO

```csharp
public class ClaimMissionRewardCommand
{
    public long PlayerId { get; init; }
    public long MissionId { get; init; }
}

public class ClaimMissionRewardResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}
```

### MissionService

```csharp
public class MissionService
{
    private readonly IPlayerRepository _players;
    private readonly IMissionRepository _missions;
    private readonly IUnitOfWork _uow;

    public MissionService(IPlayerRepository players, IMissionRepository missions, IUnitOfWork uow)
    {
        _players = players;
        _missions = missions;
        _uow = uow;
    }

    public async Task<ClaimMissionRewardResult> ClaimAsync(ClaimMissionRewardCommand cmd, CancellationToken ct = default)
    {
        var player = await _players.GetAsync(cmd.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");

        var mission = await _missions.GetAsync(cmd.MissionId, ct)
            ?? throw new InvalidOperationException("Mission not found");

        if (mission.PlayerId != player.Id)
            throw new InvalidOperationException("Mission does not belong to player");

        var reward = mission.Claim();
        player.AddReward(reward);

        await _missions.UpdateAsync(mission, ct);
        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimMissionRewardResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(reward)
        };
    }
}
```

---

## 1-3. Infrastructure – Dapper Repository

### 테이블 스키마 예시

```sql
CREATE TABLE Missions(
    Id           INTEGER PRIMARY KEY,
    PlayerId     INTEGER NOT NULL,
    Type         INTEGER NOT NULL,
    TargetCount  INTEGER NOT NULL,
    CurrentCount INTEGER NOT NULL,
    IsCompleted  INTEGER NOT NULL,
    IsClaimed    INTEGER NOT NULL,
    RewardGold   INTEGER NOT NULL,
    RewardGems   INTEGER NOT NULL
);
```

### DapperMissionRepository

```csharp
using System.Data;
using Dapper;

public class DapperMissionRepository : IMissionRepository
{
    private readonly IDbConnection _conn;

    public DapperMissionRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<Mission?> GetAsync(long missionId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT Id, PlayerId, Type, TargetCount, CurrentCount,
                     IsCompleted, IsClaimed, RewardGold, RewardGems
              FROM Missions
              WHERE Id = @Id",
            new { Id = missionId });

        if (row == null)
            return null;

        var reward = new Reward((int)row.RewardGold, (int)row.RewardGems);
        var mission = new Mission((long)row.PlayerId, (MissionType)(int)row.Type, (int)row.TargetCount, reward);

        mission.GetType().GetProperty("Id")!.SetValue(mission, (long)row.Id);
        mission.GetType().GetProperty("CurrentCount")!.SetValue(mission, (int)row.CurrentCount);
        mission.GetType().GetProperty("IsCompleted")!.SetValue(mission, (long)row.IsCompleted != 0);
        mission.GetType().GetProperty("IsClaimed")!.SetValue(mission, (long)row.IsClaimed != 0);

        return mission;
    }

    public async Task UpdateAsync(Mission mission, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE Missions
              SET CurrentCount = @CurrentCount,
                  IsCompleted  = @IsCompleted,
                  IsClaimed    = @IsClaimed
              WHERE Id = @Id",
            new
            {
                Id = mission.Id,
                CurrentCount = mission.CurrentCount,
                IsCompleted = mission.IsCompleted ? 1 : 0,
                IsClaimed = mission.IsClaimed ? 1 : 0
            });
    }
}
```

---

## 1-4. 미션 보상 통합 테스트 예시 (SQLite + Dapper)

```csharp
using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using Xunit;

public class MissionIntegrationTests : IAsyncLifetime
{
    private IDbConnection _conn = null!;
    private IDbTransaction _tx = null!;
    private MissionService _service = null!;

    public async Task InitializeAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        _conn = connection;

        await _conn.ExecuteAsync(@"
CREATE TABLE Players(
    Id      INTEGER PRIMARY KEY,
    Level   INTEGER NOT NULL,
    Gems    INTEGER NOT NULL,
    Gold    INTEGER NOT NULL,
    Stamina INTEGER NOT NULL
);
CREATE TABLE Missions(
    Id           INTEGER PRIMARY KEY,
    PlayerId     INTEGER NOT NULL,
    Type         INTEGER NOT NULL,
    TargetCount  INTEGER NOT NULL,
    CurrentCount INTEGER NOT NULL,
    IsCompleted  INTEGER NOT NULL,
    IsClaimed    INTEGER NOT NULL,
    RewardGold   INTEGER NOT NULL,
    RewardGems   INTEGER NOT NULL
);");

        await _conn.ExecuteAsync(
            "INSERT INTO Players(Id, Level, Gems, Gold, Stamina) VALUES(1, 1, 0, 0, 100);");

        // 이미 완료된 미션이라고 가정 (CurrentCount >= TargetCount, IsCompleted=1)
        await _conn.ExecuteAsync(
            @"INSERT INTO Missions(Id, PlayerId, Type, TargetCount, CurrentCount, IsCompleted, IsClaimed, RewardGold, RewardGems)
              VALUES(100, 1, 0, 10, 10, 1, 0, 1000, 5);");

        _tx = _conn.BeginTransaction();

        var playerRepo = new DapperPlayerRepository(_conn);
        var missionRepo = new DapperMissionRepository(_conn);
        var uow = new DapperUnitOfWork(_tx);

        _service = new MissionService(playerRepo, missionRepo, uow);
    }

    public Task DisposeAsync()
    {
        _tx.Dispose();
        _conn.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ClaimMissionReward_UpdatesMissionAndPlayer()
    {
        var result = await _service.ClaimAsync(new ClaimMissionRewardCommand
        {
            PlayerId = 1,
            MissionId = 100
        });

        var playerRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT Gold, Gems FROM Players WHERE Id = 1");
        Assert.Equal(1000, (int)playerRow.Gold);
        Assert.Equal(5, (int)playerRow.Gems);

        var missionRow = await _conn.QuerySingleAsync<dynamic>(
            "SELECT IsClaimed FROM Missions WHERE Id = 100");
        Assert.Equal(1L, (long)missionRow.IsClaimed);
    }
}
```

---

# 2. 업적 (Achievement) 유즈케이스

업적은 **한 번 달성되면 언제든 보상 수령 가능**하다고 가정한다.

## 2-1. 도메인

```csharp
public class Achievement
{
    public int Id { get; private set; }
    public string Name { get; private set; } = "";
    public Reward Reward { get; private set; }

    protected Achievement() { }

    public Achievement(int id, string name, Reward reward)
    {
        Id = id;
        Name = name;
        Reward = reward;
    }
}

// 플레이어별 업적 상태
public class PlayerAchievement
{
    public long PlayerId { get; private set; }
    public int AchievementId { get; private set; }
    public bool IsUnlocked { get; private set; }
    public bool IsClaimed { get; private set; }

    protected PlayerAchievement() { }

    public PlayerAchievement(long playerId, int achievementId)
    {
        PlayerId = playerId;
        AchievementId = achievementId;
        IsUnlocked = false;
        IsClaimed = false;
    }

    public void Unlock()
    {
        if (!IsUnlocked)
            IsUnlocked = true;
    }

    public void MarkClaimed()
    {
        if (!IsUnlocked)
            throw new InvalidOperationException("Achievement not unlocked");
        if (IsClaimed)
            throw new InvalidOperationException("Already claimed");
        IsClaimed = true;
    }
}
```

실제로는 “언제 Unlock 되는지”는 다른 도메인 로직(예: 누적 킬 수, 스테이지 클리어 수 등)이 결정하고,
여기서는 **이미 Unlock 된 상태에서 보상만 받는 유즈케이스**만 다룬다.

---

## 2-2. 애플리케이션

### 인터페이스

```csharp
public interface IAchievementRepository
{
    Task<Achievement?> GetAsync(int achievementId, CancellationToken ct = default);
}

public interface IPlayerAchievementRepository
{
    Task<PlayerAchievement?> GetAsync(long playerId, int achievementId, CancellationToken ct = default);
    Task AddAsync(PlayerAchievement pa, CancellationToken ct = default);
    Task UpdateAsync(PlayerAchievement pa, CancellationToken ct = default);
}
```

### Command / Result DTO

```csharp
public class ClaimAchievementRewardCommand
{
    public long PlayerId { get; init; }
    public int AchievementId { get; init; }
}

public class ClaimAchievementRewardResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}
```

### AchievementService

```csharp
public class AchievementService
{
    private readonly IPlayerRepository _players;
    private readonly IAchievementRepository _achievements;
    private readonly IPlayerAchievementRepository _playerAchievements;
    private readonly IUnitOfWork _uow;

    public AchievementService(
        IPlayerRepository players,
        IAchievementRepository achievements,
        IPlayerAchievementRepository playerAchievements,
        IUnitOfWork uow)
    {
        _players = players;
        _achievements = achievements;
        _playerAchievements = playerAchievements;
        _uow = uow;
    }

    public async Task<ClaimAchievementRewardResult> ClaimAsync(ClaimAchievementRewardCommand cmd, CancellationToken ct = default)
    {
        var player = await _players.GetAsync(cmd.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");

        var achievement = await _achievements.GetAsync(cmd.AchievementId, ct)
            ?? throw new InvalidOperationException("Achievement not found");

        var pa = await _playerAchievements.GetAsync(cmd.PlayerId, cmd.AchievementId, ct)
                 ?? throw new InvalidOperationException("Player has not unlocked this achievement");

        if (!pa.IsUnlocked)
            throw new InvalidOperationException("Achievement not unlocked");

        pa.MarkClaimed();
        player.AddReward(achievement.Reward);

        await _playerAchievements.UpdateAsync(pa, ct);
        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimAchievementRewardResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(achievement.Reward)
        };
    }
}
```

---

## 2-3. Infrastructure – Dapper Repositories

### 테이블 스키마 예시

```sql
CREATE TABLE Achievements(
    Id         INTEGER PRIMARY KEY,
    Name       TEXT    NOT NULL,
    RewardGold INTEGER NOT NULL,
    RewardGems INTEGER NOT NULL
);

CREATE TABLE PlayerAchievements(
    PlayerId       INTEGER NOT NULL,
    AchievementId  INTEGER NOT NULL,
    IsUnlocked     INTEGER NOT NULL,
    IsClaimed      INTEGER NOT NULL,
    PRIMARY KEY(PlayerId, AchievementId)
);
```

### Dapper 구현

```csharp
public class DapperAchievementRepository : IAchievementRepository
{
    private readonly IDbConnection _conn;

    public DapperAchievementRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<Achievement?> GetAsync(int achievementId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT Id, Name, RewardGold, RewardGems
              FROM Achievements
              WHERE Id = @Id",
            new { Id = achievementId });

        if (row == null) return null;

        var reward = new Reward((int)row.RewardGold, (int)row.RewardGems);
        return new Achievement((int)row.Id, (string)row.Name, reward);
    }
}

public class DapperPlayerAchievementRepository : IPlayerAchievementRepository
{
    private readonly IDbConnection _conn;

    public DapperPlayerAchievementRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<PlayerAchievement?> GetAsync(long playerId, int achievementId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT PlayerId, AchievementId, IsUnlocked, IsClaimed
              FROM PlayerAchievements
              WHERE PlayerId = @PlayerId AND AchievementId = @AchievementId",
            new { PlayerId = playerId, AchievementId = achievementId });

        if (row == null) return null;

        var pa = new PlayerAchievement((long)row.PlayerId, (int)row.AchievementId);
        pa.GetType().GetProperty("IsUnlocked")!.SetValue(pa, (long)row.IsUnlocked != 0);
        pa.GetType().GetProperty("IsClaimed")!.SetValue(pa, (long)row.IsClaimed != 0);

        return pa;
    }

    public async Task AddAsync(PlayerAchievement pa, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"INSERT INTO PlayerAchievements(PlayerId, AchievementId, IsUnlocked, IsClaimed)
              VALUES(@PlayerId, @AchievementId, @IsUnlocked, @IsClaimed)",
            new
            {
                PlayerId = pa.PlayerId,
                AchievementId = pa.AchievementId,
                IsUnlocked = pa.IsUnlocked ? 1 : 0,
                IsClaimed = pa.IsClaimed ? 1 : 0
            });
    }

    public async Task UpdateAsync(PlayerAchievement pa, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE PlayerAchievements
              SET IsUnlocked = @IsUnlocked,
                  IsClaimed  = @IsClaimed
              WHERE PlayerId = @PlayerId AND AchievementId = @AchievementId",
            new
            {
                PlayerId = pa.PlayerId,
                AchievementId = pa.AchievementId,
                IsUnlocked = pa.IsUnlocked ? 1 : 0,
                IsClaimed = pa.IsClaimed ? 1 : 0
            });
    }
}
```

통합 테스트는 미션/우편과 거의 동일 패턴으로 만들 수 있다

* Achievements/PlayerAchievements/Players 테이블 생성
* 데이터 insert
* `AchievementService.ClaimAsync` 호출
* Players/PlayerAchievements 상태 SELECT 후 검증

---

# 3. 이벤트 보상 (Event Reward) 유즈케이스

이벤트는 “기간 제한 + 1회성 보상 수령” 정도로 단순하게 가정한다
(실제로는 누적 포인트, 누적 접속일 등 여러 유형이 있지만 패턴은 동일하다).

## 3-1. 도메인

```csharp
public class EventDefinition
{
    public int Id { get; private set; }
    public string Name { get; private set; } = "";
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public Reward Reward { get; private set; }

    protected EventDefinition() { }

    public EventDefinition(int id, string name, DateTime startAt, DateTime endAt, Reward reward)
    {
        Id = id;
        Name = name;
        StartAt = startAt;
        EndAt = endAt;
        Reward = reward;
    }

    public bool IsActive(DateTime now) => now >= StartAt && now <= EndAt;
}

public class PlayerEventReward
{
    public long PlayerId { get; private set; }
    public int EventId { get; private set; }
    public bool IsClaimed { get; private set; }

    protected PlayerEventReward() { }

    public PlayerEventReward(long playerId, int eventId)
    {
        PlayerId = playerId;
        EventId = eventId;
        IsClaimed = false;
    }

    public void MarkClaimed()
    {
        if (IsClaimed)
            throw new InvalidOperationException("Event reward already claimed");
        IsClaimed = true;
    }
}
```

---

## 3-2. 애플리케이션

### 인터페이스

```csharp
public interface IEventDefinitionRepository
{
    Task<EventDefinition?> GetAsync(int eventId, CancellationToken ct = default);
}

public interface IPlayerEventRewardRepository
{
    Task<PlayerEventReward?> GetAsync(long playerId, int eventId, CancellationToken ct = default);
    Task AddAsync(PlayerEventReward per, CancellationToken ct = default);
    Task UpdateAsync(PlayerEventReward per, CancellationToken ct = default);
}
```

### Command / Result DTO

```csharp
public class ClaimEventRewardCommand
{
    public long PlayerId { get; init; }
    public int EventId { get; init; }
}

public class ClaimEventRewardResult
{
    public PlayerDto Player { get; init; } = null!;
    public RewardDto Reward { get; init; } = null!;
}
```

### EventRewardService

```csharp
public class EventRewardService
{
    private readonly IPlayerRepository _players;
    private readonly IEventDefinitionRepository _events;
    private readonly IPlayerEventRewardRepository _playerEvents;
    private readonly ITimeProvider _time;
    private readonly IUnitOfWork _uow;

    public EventRewardService(
        IPlayerRepository players,
        IEventDefinitionRepository events,
        IPlayerEventRewardRepository playerEvents,
        ITimeProvider time,
        IUnitOfWork uow)
    {
        _players = players;
        _events = events;
        _playerEvents = playerEvents;
        _time = time;
        _uow = uow;
    }

    public async Task<ClaimEventRewardResult> ClaimAsync(ClaimEventRewardCommand cmd, CancellationToken ct = default)
    {
        var now = _time.UtcNow;

        var player = await _players.GetAsync(cmd.PlayerId, ct)
            ?? throw new InvalidOperationException("Player not found");

        var ev = await _events.GetAsync(cmd.EventId, ct)
            ?? throw new InvalidOperationException("Event not found");

        if (!ev.IsActive(now))
            throw new InvalidOperationException("Event is not active");

        var per = await _playerEvents.GetAsync(cmd.PlayerId, cmd.EventId, ct)
                  ?? new PlayerEventReward(cmd.PlayerId, cmd.EventId);

        if (per.IsClaimed)
            throw new InvalidOperationException("Already claimed");

        per.MarkClaimed();
        player.AddReward(ev.Reward);

        if (per.IsClaimed && per.PlayerId == cmd.PlayerId && per.EventId == cmd.EventId)
        {
            // 신규면 Add, 기존이면 Update라고 가정하고 간단히 처리한다
            await _playerEvents.AddAsync(per, ct); // 실제로는 존재 여부 보고 분기하는게 안전하다
        }

        await _players.UpdateAsync(player, ct);
        await _uow.CommitAsync(ct);

        return new ClaimEventRewardResult
        {
            Player = PlayerDto.From(player),
            Reward = RewardDto.From(ev.Reward)
        };
    }
}
```

실제 구현에서는 `GetAsync` 결과 null / not null에 따라 Add/Update를 나눠야 한다
여기서는 예제를 단순하게 유지하기 위해 새로 생성한 경우만 Add 한다고 보았다.

---

## 3-3. Infrastructure – Dapper Repositories

### 테이블 스키마

```sql
CREATE TABLE EventDefinitions(
    Id         INTEGER PRIMARY KEY,
    Name       TEXT    NOT NULL,
    StartAt    TEXT    NOT NULL,
    EndAt      TEXT    NOT NULL,
    RewardGold INTEGER NOT NULL,
    RewardGems INTEGER NOT NULL
);

CREATE TABLE PlayerEventRewards(
    PlayerId  INTEGER NOT NULL,
    EventId   INTEGER NOT NULL,
    IsClaimed INTEGER NOT NULL,
    PRIMARY KEY(PlayerId, EventId)
);
```

### Dapper 구현

```csharp
public class DapperEventDefinitionRepository : IEventDefinitionRepository
{
    private readonly IDbConnection _conn;

    public DapperEventDefinitionRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<EventDefinition?> GetAsync(int eventId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT Id, Name, StartAt, EndAt, RewardGold, RewardGems
              FROM EventDefinitions
              WHERE Id = @Id",
            new { Id = eventId });

        if (row == null) return null;

        var reward = new Reward((int)row.RewardGold, (int)row.RewardGems);
        return new EventDefinition(
            (int)row.Id,
            (string)row.Name,
            DateTime.Parse((string)row.StartAt),
            DateTime.Parse((string)row.EndAt),
            reward);
    }
}

public class DapperPlayerEventRewardRepository : IPlayerEventRewardRepository
{
    private readonly IDbConnection _conn;

    public DapperPlayerEventRewardRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<PlayerEventReward?> GetAsync(long playerId, int eventId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT PlayerId, EventId, IsClaimed
              FROM PlayerEventRewards
              WHERE PlayerId = @PlayerId AND EventId = @EventId",
            new { PlayerId = playerId, EventId = eventId });

        if (row == null) return null;

        var per = new PlayerEventReward((long)row.PlayerId, (int)row.EventId);
        per.GetType().GetProperty("IsClaimed")!.SetValue(per, (long)row.IsClaimed != 0);

        return per;
    }

    public async Task AddAsync(PlayerEventReward per, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"INSERT INTO PlayerEventRewards(PlayerId, EventId, IsClaimed)
              VALUES(@PlayerId, @EventId, @IsClaimed)",
            new
            {
                PlayerId = per.PlayerId,
                EventId = per.EventId,
                IsClaimed = per.IsClaimed ? 1 : 0
            });
    }

    public async Task UpdateAsync(PlayerEventReward per, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE PlayerEventRewards
              SET IsClaimed = @IsClaimed
              WHERE PlayerId = @PlayerId AND EventId = @EventId",
            new
            {
                PlayerId = per.PlayerId,
                EventId = per.EventId,
                IsClaimed = per.IsClaimed ? 1 : 0
            });
    }
}
```

통합 테스트는 다른 것들과 동일한 패턴으로 작성하면 된다

* Players, EventDefinitions, PlayerEventRewards 테이블 생성
* 활성 상태 이벤트를 insert
* Player 1 생성
* `EventRewardService.ClaimAsync` 호출
* Player 재화와 PlayerEventRewards의 IsClaimed를 SELECT해서 검증

---

## 마무리

지금까지

* **미션**: 도메인 `Mission`, `MissionService`, `DapperMissionRepository`, 통합 테스트
* **업적**: 도메인 `Achievement`, `PlayerAchievement`, `AchievementService`, Dapper Repo
* **이벤트 보상**: 도메인 `EventDefinition`, `PlayerEventReward`, `EventRewardService`, Dapper Repo

까지 다 구성했으니, 앞에서 만든

* 가챠
* 스테이지 클리어
* 우편
* 출석체크

까지 합치면 웬만한 수집형 RPG의 기본 보상/진행 유즈케이스는 거의 다 커버되는 셈이다.

원하면 이 구조들을 실제 `MyApp.Api / Application / Domain / Infrastructure` 프로젝트로 나눴을 때의 csproj 참조 관계나, DI 등록 코드(Startup/Program.cs)를 한 번에 정리해서 보여줄 수도 있다.

   
좋다, 이제까지 만든 도메인/유즈케이스들을 실제 솔루션 구조와 DI 구성까지 한 번에 정리해보겠다.

아래 기준으로 설명하겠다.

```text
src
 ├─ MyApp.Api             // Web API (Controller, API DTO)
 ├─ MyApp.Application     // 유즈케이스/서비스, Application DTO, 인터페이스
 ├─ MyApp.Domain          // Entity, ValueObject, Domain 서비스
 └─ MyApp.Infrastructure  // Dapper, Repository 구현, 외부 연동
```

---

## 1. 프로젝트 참조 구조

의존성 방향을 이렇게 고정한다고 보면 된다.

* `MyApp.Domain`

  * 아무 것도 참조하지 않음 (순수 C#)

* `MyApp.Application`

  * `MyApp.Domain`만 참조

* `MyApp.Infrastructure`

  * `MyApp.Domain`
  * `MyApp.Application`

* `MyApp.Api`

  * `MyApp.Application`
  * `MyApp.Infrastructure`

의도는:

* Domain은 제일 안쪽이라 아무 것도 모름
* Application은 규칙/유즈케이스만 알고 인프라를 인터페이스로 추상화함
* Infrastructure는 실제 구현(Dapper, DB 연결)을 제공함
* Api는 유즈케이스를 HTTP에 노출하고, DI만 구성함

---

## 2. 각 .csproj 예시

### 2.1. MyApp.Domain.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <!-- 외부 패키지 의존성 없음: 순수 도메인 코드만 둔다 -->

</Project>
```

---

### 2.2. MyApp.Application.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyApp.Domain\MyApp.Domain.csproj" />
  </ItemGroup>

  <!-- 필요하다면 MediatR 등 추가 가능하지만,
       예제에서는 순수 C# + Domain 의존만 둔다 -->

</Project>
```

---

### 2.3. MyApp.Infrastructure.csproj

Dapper + DB Provider를 쓰는 프로젝트다.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyApp.Domain\MyApp.Domain.csproj" />
    <ProjectReference Include="..\MyApp.Application\MyApp.Application.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.35" />
    <PackageReference Include="MySqlConnector" Version="2.3.7" />
    <!-- 개발/테스트용으로 SQLite를 쓴다면
    <PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" /> -->
  </ItemGroup>

</Project>
```

여기에는:

* `DapperPlayerRepository`, `DapperStageRepository`,
  `DapperMailRepository`, `DapperAttendanceRepository`,
  `DapperMissionRepository`, `DapperAchievementRepository`,
  `DapperEventDefinitionRepository`, `DapperPlayerEventRewardRepository`
* `DapperUnitOfWork`, `FixedTimeProvider`(테스트용 말고 실제용 TimeProvider 구현)
* DB Connection/Transaction을 사용하는 각종 구현체

들을 둔다고 보면 된다.

---

### 2.4. MyApp.Api.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MyApp.Application\MyApp.Application.csproj" />
    <ProjectReference Include="..\MyApp.Infrastructure\MyApp.Infrastructure.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

</Project>
```

여기에는:

* `GachaController`, `StageController`, `MailController`, `AttendanceController`,
  `MissionController`, `AchievementController`, `EventController` 등
* API DTO (Request/Response)
* Program.cs / DI 구성, 미들웨어, Swagger 설정

이 들어간다고 보면 된다.

---

## 3. Infrastructure에 DI 확장 메서드 만들기

`MyApp.Infrastructure` 프로젝트에 DI 확장 메서드를 만들어두면 Api 쪽 Program.cs가 깔끔해진다.

예시로 `DependencyInjection.cs` 파일을 만든다고 하겠다.

```csharp
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using MyApp.Application;
using MyApp.Domain;

namespace MyApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GameDb");

        // IDbConnection 등록 (Request 스코프)
        services.AddScoped<IDbConnection>(sp =>
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        });

        // IDbTransaction + IUnitOfWork 등록 (Request 스코프)
        services.AddScoped<IDbTransaction>(sp =>
        {
            var conn = sp.GetRequiredService<IDbConnection>();
            return conn.BeginTransaction();
        });

        services.AddScoped<IUnitOfWork>(sp =>
        {
            var tx = sp.GetRequiredService<IDbTransaction>();
            return new DapperUnitOfWork(tx);
        });

        // TimeProvider
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();

        // Dapper Repository들 등록
        services.AddScoped<IPlayerRepository, DapperPlayerRepository>();
        services.AddScoped<IStageRepository, DapperStageRepository>();
        services.AddScoped<IMailRepository, DapperMailRepository>();
        services.AddScoped<IAttendanceRepository, DapperAttendanceRepository>();
        services.AddScoped<IMissionRepository, DapperMissionRepository>();
        services.AddScoped<IAchievementRepository, DapperAchievementRepository>();
        services.AddScoped<IPlayerAchievementRepository, DapperPlayerAchievementRepository>();
        services.AddScoped<IEventDefinitionRepository, DapperEventDefinitionRepository>();
        services.AddScoped<IPlayerEventRewardRepository, DapperPlayerEventRewardRepository>();

        return services;
    }
}

// 실제 시간 제공용 구현
public class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
```

> 주의: Dapper Repository 구현에서 `IDbConnection`만 쓰고 `IDbTransaction`을 안 넘겨주면
> 트랜잭션이 적용되지 않을 수 있으니, 실제 구현에서는 생성자에 `IDbTransaction`도 주입하거나,
> Dapper 호출 시 `transaction: _tx`를 넘기는 식으로 통일하는 편이 좋다.

---

## 4. Application 서비스 DI 등록

Application 프로젝트에 있는 서비스들도 DI로 등록해야 한다.

보통은 `MyApp.Api` 쪽에서 직접 등록하거나,
`MyApp.Application`에 `DependencyInjection` 확장 메서드를 하나 더 두고 거기서 묶는다.

여기서는 Application쪽에 `DependencyInjection`을 둔다고 가정하겠다.

### MyApp.Application.DependencyInjection.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using MyApp.Domain;

namespace MyApp.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 도메인 서비스 (도메인에 있지만 DI가 필요하므로 여기서 등록)
        services.AddScoped<GachaDomainService>();
        services.AddScoped<StageDomainService>();
        services.AddSingleton<AttendanceRewardPolicy>();

        // Application 서비스 (유즈케이스)
        services.AddScoped<GachaService>();
        services.AddScoped<StageService>();
        services.AddScoped<MailService>();
        services.AddScoped<AttendanceService>();
        services.AddScoped<MissionService>();
        services.AddScoped<AchievementService>();
        services.AddScoped<EventRewardService>();

        return services;
    }
}
```

이렇게 해두면 Api 쪽 Program.cs에서 한 줄로 Application 서비스들을 등록할 수 있다.

---

## 5. Program.cs / DI 구성 전체 예시

`MyApp.Api` 프로젝트의 `Program.cs`를 다음처럼 구성할 수 있다.

```csharp
using MyApp.Application;
using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controller + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application 레이어 등록
builder.Services.AddApplication();

// Infrastructure(Dapper, Repositories, UnitOfWork, TimeProvider) 등록
builder.Services.AddInfrastructure(builder.Configuration);

// 인증/인가, 기타 미들웨어 설정도 여기서 추가한다
// builder.Services.AddAuthentication(...);
// builder.Services.AddAuthorization(...);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
```

이 상태에서 Controller들은 전부 DI로 서비스들을 받을 수 있다.

예를 들어 `MailController`는 이렇게 생긴다.

```csharp
[ApiController]
[Route("api/mail")]
public class MailController : ControllerBase
{
    private readonly MailService _mailService;
    private readonly ICurrentPlayerProvider _currentPlayer;

    public MailController(MailService mailService, ICurrentPlayerProvider currentPlayer)
    {
        _mailService = mailService;
        _currentPlayer = currentPlayer;
    }

    [HttpPost("claim")]
    public async Task<ActionResult<ClaimMailResponse>> Claim([FromBody] ClaimMailRequest request)
    {
        var cmd = new ClaimMailCommand
        {
            PlayerId = _currentPlayer.PlayerId,
            MailId = request.MailId
        };

        var result = await _mailService.ClaimAsync(cmd);

        var response = new ClaimMailResponse
        {
            Player = PlayerResponse.From(result.Player),
            Reward  = RewardResponse.From(result.Reward)
        };

        return Ok(response);
    }
}
```

다른 컨트롤러들도 마찬가지로:

* `GachaController` → `GachaService`
* `StageController` → `StageService`
* `AttendanceController` → `AttendanceService`
* `MissionController` → `MissionService`
* `AchievementController` → `AchievementService`
* `EventController` → `EventRewardService`

를 주입받아서 사용하면 된다.

---

## 6. 요약

정리하면:

1. **프로젝트 구조**

   * `MyApp.Domain`
   * `MyApp.Application` (Domain 참조)
   * `MyApp.Infrastructure` (Domain + Application 참조, Dapper/DB 구현)
   * `MyApp.Api` (Application + Infrastructure 참조, Controller/Program)

2. **DI 분리 전략**

   * Application에 `AddApplication` 확장 메서드 → 도메인 서비스 + 유즈케이스 서비스 등록
   * Infrastructure에 `AddInfrastructure` 확장 메서드 → Dapper Repos + UnitOfWork + IDbConnection + TimeProvider 등록
   * Api의 Program.cs에서는 `AddApplication()`, `AddInfrastructure()`만 호출

3. **유즈케이스들**

   * 가챠, 스테이지 클리어, 우편 수령, 출석체크, 미션 보상, 업적 보상, 이벤트 보상
   * 전부 동일한 패턴으로

     * Domain: 규칙
     * Application: 흐름/트랜잭션
     * Infrastructure: DB/Dapper 구현
     * Api: HTTP 입출력

까지 연결된다.

지금 단계에서 실제로 구현을 시작하고 싶은 클래스(예: MissionController, EventController)의 구체적인 코드 골격이 필요하면, 그 부분만 골라서 컨트롤러 + DTO까지 쭉 만들어줄 수 있다.


좋다, 이제까지 만든 구조에 이어서 **“우편함(메일 목록 조회)” 기능**을 4레이어 전체로 추가해보겠다.

이미 있는 전제들 그대로 쓴다

* 레이어 구조

  * `MyApp.Domain`
  * `MyApp.Application`
  * `MyApp.Infrastructure` (Dapper)
  * `MyApp.Api`

* 도메인 `Mail`은 이미 있고, `MailService.ClaimAsync` 같은 “수령” 유즈케이스도 구현돼 있다고 가정한다.

여기서는 **“우편함 목록 조회 + 단건 조회”** 기준으로 만든다:

* 우편함 목록: `/api/mail`

  * 페이지네이션
  * 정렬(보통 최신순)
  * 기본 필터: 아직 만료 안 된 메일
  * 옵션: 수령 완료/미수령 필터
* 단건 조회: `/api/mail/{id}`

---

## 1. Domain: Mail에 약간만 정보 추가

우편함 구현 자체는 거의 애플리케이션/인프라 레이어 일이기 때문에
도메인은 크게 바꿀 필요는 없고, **CreatedAt** 정도만 추가하면 충분하다.

```csharp
// MyApp.Domain.Mail

public class Mail
{
    public long Id { get; private set; }
    public long PlayerId { get; private set; }
    public string Title { get; private set; } = "";
    public string Body { get; private set; } = "";
    public Reward Reward { get; private set; }
    public bool IsClaimed { get; private set; }
    public DateTime ExpireAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Mail() { }

    public Mail(long playerId, string title, string body, Reward reward, DateTime expireAt)
    {
        PlayerId = playerId;
        Title = title;
        Body = body;
        Reward = reward;
        ExpireAt = expireAt;
        CreatedAt = DateTime.UtcNow;
        IsClaimed = false;
    }

    public bool IsExpired(DateTime now) => now > ExpireAt;

    public Reward Claim(DateTime now)
    {
        if (IsClaimed)
            throw new InvalidOperationException("Mail already claimed.");
        if (IsExpired(now))
            throw new InvalidOperationException("Mail expired.");

        IsClaimed = true;
        return Reward;
    }
}
```

우편함 자체는 별 Aggregate를 만들 필요 없고,
“플레이어의 Mail 리스트”가 곧 우편함이라 보고 Repository에서 목록을 가져오면 된다.

---

## 2. Application: Mailbox 유즈케이스 설계

### 2.1. 쿼리/DTO 정의

우편함은 보통 목록용 DTO가 필요하니 **요약용 + 상세용**으로 나눈다.

```csharp
// MyApp.Application.Dtos.MailDto들

public class MailSummaryDto
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsClaimed { get; set; }
    public bool IsExpired { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public static MailSummaryDto From(Mail mail, DateTime now) => new()
    {
        Id = mail.Id,
        Title = mail.Title,
        IsClaimed = mail.IsClaimed,
        IsExpired = mail.IsExpired(now),
        ExpireAt = mail.ExpireAt,
        CreatedAt = mail.CreatedAt
    };
}

public class MailDetailDto
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public bool IsClaimed { get; set; }
    public bool IsExpired { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public RewardDto Reward { get; set; } = null!;

    public static MailDetailDto From(Mail mail, DateTime now) => new()
    {
        Id = mail.Id,
        Title = mail.Title,
        Body = mail.Body,
        IsClaimed = mail.IsClaimed,
        IsExpired = mail.IsExpired(now),
        ExpireAt = mail.ExpireAt,
        CreatedAt = mail.CreatedAt,
        Reward = RewardDto.From(mail.Reward)
    };
}

public class PagedMailResultDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IReadOnlyList<MailSummaryDto> Items { get; set; } = Array.Empty<MailSummaryDto>();
}
```

### 2.2. Repository 인터페이스 확장

기존 `IMailRepository`에 “목록 조회” 기능을 추가한다.

```csharp
// MyApp.Application.Interfaces.Repositories

public interface IMailRepository
{
    Task<Mail?> GetAsync(long mailId, CancellationToken ct = default);
    Task UpdateAsync(Mail mail, CancellationToken ct = default);

    // 우편함 목록 조회
    Task<IReadOnlyList<Mail>> GetListByPlayerAsync(
        long playerId,
        int skip,
        int take,
        bool includeClaimed,
        bool includeExpired,
        DateTime now,
        CancellationToken ct = default);

    Task<int> CountByPlayerAsync(
        long playerId,
        bool includeClaimed,
        bool includeExpired,
        DateTime now,
        CancellationToken ct = default);
}
```

필터 기준은 이렇게 두었다.

* `includeClaimed`: true면 수령한 메일도 포함, false면 미수령만
* `includeExpired`: true면 만료된 메일도 포함, false면 `ExpireAt > now` 만

### 2.3. MailboxService 유즈케이스

이건 “우편함 조회 전용” 서비스라고 생각하면 된다.

```csharp
// MyApp.Application.Mailbox.MailboxService

public class MailboxQuery
{
    public long PlayerId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool IncludeClaimed { get; init; } = false;
    public bool IncludeExpired { get; init; } = false;
}

public class MailboxService
{
    private readonly IMailRepository _mails;
    private readonly ITimeProvider _time;

    public MailboxService(IMailRepository mails, ITimeProvider time)
    {
        _mails = mails;
        _time = time;
    }

    public async Task<PagedMailResultDto> GetMailboxAsync(MailboxQuery query, CancellationToken ct = default)
    {
        var now = _time.UtcNow;
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;

        var skip = (page - 1) * pageSize;
        var take = pageSize;

        var mails = await _mails.GetListByPlayerAsync(
            query.PlayerId,
            skip,
            take,
            query.IncludeClaimed,
            query.IncludeExpired,
            now,
            ct);

        var total = await _mails.CountByPlayerAsync(
            query.PlayerId,
            query.IncludeClaimed,
            query.IncludeExpired,
            now,
            ct);

        var items = mails.Select(m => MailSummaryDto.From(m, now)).ToList();

        return new PagedMailResultDto
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<MailDetailDto> GetMailDetailAsync(long playerId, long mailId, CancellationToken ct = default)
    {
        var now = _time.UtcNow;

        var mail = await _mails.GetAsync(mailId, ct)
            ?? throw new InvalidOperationException("Mail not found");

        if (mail.PlayerId != playerId)
            throw new InvalidOperationException("Mail does not belong to player");

        return MailDetailDto.From(mail, now);
    }
}
```

---

## 3. Infrastructure: Dapper 기반 MailRepository 구현

### 3.1. 테이블 스키마 상정

앞에서 썼던 것에 컬럼 조금 확장:

```sql
CREATE TABLE Mails(
    Id          BIGINT PRIMARY KEY,
    PlayerId    BIGINT NOT NULL,
    Title       TEXT   NOT NULL,
    Body        TEXT   NOT NULL,
    RewardGold  INT    NOT NULL,
    RewardGems  INT    NOT NULL,
    IsClaimed   INT    NOT NULL,
    ExpireAt    TEXT   NOT NULL,
    CreatedAt   TEXT   NOT NULL
);

CREATE INDEX IX_Mails_PlayerId_CreatedAt
    ON Mails(PlayerId, CreatedAt DESC);
```

### 3.2. DapperMailRepository 구현

```csharp
// MyApp.Infrastructure.Persistence.DapperMailRepository

using System.Data;
using Dapper;
using MyApp.Application;
using MyApp.Domain;

public class DapperMailRepository : IMailRepository
{
    private readonly IDbConnection _conn;

    public DapperMailRepository(IDbConnection conn)
    {
        _conn = conn;
    }

    public async Task<Mail?> GetAsync(long mailId, CancellationToken ct = default)
    {
        var row = await _conn.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT Id, PlayerId, Title, Body,
                     RewardGold, RewardGems,
                     IsClaimed, ExpireAt, CreatedAt
              FROM Mails
              WHERE Id = @Id",
            new { Id = mailId });

        if (row == null)
            return null;

        return MapToMail(row);
    }

    public async Task UpdateAsync(Mail mail, CancellationToken ct = default)
    {
        await _conn.ExecuteAsync(
            @"UPDATE Mails
              SET IsClaimed = @IsClaimed
              WHERE Id = @Id",
            new
            {
                Id = mail.Id,
                IsClaimed = mail.IsClaimed ? 1 : 0
            });
    }

    public async Task<IReadOnlyList<Mail>> GetListByPlayerAsync(
        long playerId,
        int skip,
        int take,
        bool includeClaimed,
        bool includeExpired,
        DateTime now,
        CancellationToken ct = default)
    {
        // WHERE 조건 구성
        var sql = @"
SELECT Id, PlayerId, Title, Body,
       RewardGold, RewardGems,
       IsClaimed, ExpireAt, CreatedAt
FROM Mails
WHERE PlayerId = @PlayerId";

        if (!includeClaimed)
            sql += " AND IsClaimed = 0";

        if (!includeExpired)
            sql += " AND ExpireAt > @Now";

        sql += @"
ORDER BY CreatedAt DESC
LIMIT @Take OFFSET @Skip;";

        var rows = await _conn.QueryAsync<dynamic>(
            sql,
            new
            {
                PlayerId = playerId,
                Now = now.ToString("O"),
                Skip = skip,
                Take = take
            });

        var list = rows.Select(MapToMail).ToList();
        return list;
    }

    public async Task<int> CountByPlayerAsync(
        long playerId,
        bool includeClaimed,
        bool includeExpired,
        DateTime now,
        CancellationToken ct = default)
    {
        var sql = @"
SELECT COUNT(*)
FROM Mails
WHERE PlayerId = @PlayerId";

        if (!includeClaimed)
            sql += " AND IsClaimed = 0";

        if (!includeExpired)
            sql += " AND ExpireAt > @Now";

        var count = await _conn.ExecuteScalarAsync<long>(
            sql,
            new
            {
                PlayerId = playerId,
                Now = now.ToString("O")
            });

        return (int)count;
    }

    private Mail MapToMail(dynamic row)
    {
        var reward = new Reward((int)row.RewardGold, (int)row.RewardGems);

        var mail = new Mail(
            (long)row.PlayerId,
            (string)row.Title,
            (string)row.Body,
            reward,
            DateTime.Parse((string)row.ExpireAt));

        mail.GetType().GetProperty("Id")!.SetValue(mail, (long)row.Id);
        mail.GetType().GetProperty("IsClaimed")!.SetValue(mail, (long)row.IsClaimed != 0);
        mail.GetType().GetProperty("CreatedAt")!.SetValue(mail, DateTime.Parse((string)row.CreatedAt));

        return mail;
    }
}
```

---

## 4. Api: 우편함용 Controller & DTO

### 4.1. API DTO

```csharp
// MyApp.Api.Dtos.Mailbox

public class MailboxRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeClaimed { get; set; } = false;
    public bool IncludeExpired { get; set; } = false;
}

public class MailSummaryResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsClaimed { get; set; }
    public bool IsExpired { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public static MailSummaryResponse From(MailSummaryDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        IsClaimed = dto.IsClaimed,
        IsExpired = dto.IsExpired,
        ExpireAt = dto.ExpireAt,
        CreatedAt = dto.CreatedAt
    };
}

public class PagedMailboxResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IReadOnlyList<MailSummaryResponse> Items { get; set; } = Array.Empty<MailSummaryResponse>();
}

public class MailDetailResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public bool IsClaimed { get; set; }
    public bool IsExpired { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public RewardResponse Reward { get; set; } = null!;

    public static MailDetailResponse From(MailDetailDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        Body = dto.Body,
        IsClaimed = dto.IsClaimed,
        IsExpired = dto.IsExpired,
        ExpireAt = dto.ExpireAt,
        CreatedAt = dto.CreatedAt,
        Reward = RewardResponse.From(dto.Reward)
    };
}
```

### 4.2. Controller

기존 `MailController`에 우편함 API를 추가한다고 하겠다.

```csharp
// MyApp.Api.Controllers.MailController

[ApiController]
[Route("api/mail")]
public class MailController : ControllerBase
{
    private readonly MailService _mailService;       // 수령용 기존 서비스
    private readonly MailboxService _mailboxService; // 새로 만든 우편함 서비스
    private readonly ICurrentPlayerProvider _currentPlayer;

    public MailController(
        MailService mailService,
        MailboxService mailboxService,
        ICurrentPlayerProvider currentPlayer)
    {
        _mailService = mailService;
        _mailboxService = mailboxService;
        _currentPlayer = currentPlayer;
    }

    // 우편함 목록
    [HttpGet]
    public async Task<ActionResult<PagedMailboxResponse>> GetMailbox([FromQuery] MailboxRequest request)
    {
        var query = new MailboxQuery
        {
            PlayerId = _currentPlayer.PlayerId,
            Page = request.Page,
            PageSize = request.PageSize,
            IncludeClaimed = request.IncludeClaimed,
            IncludeExpired = request.IncludeExpired
        };

        var result = await _mailboxService.GetMailboxAsync(query);

        var response = new PagedMailboxResponse
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            Items = result.Items.Select(MailSummaryResponse.From).ToList()
        };

        return Ok(response);
    }

    // 우편 상세
    [HttpGet("{mailId:long}")]
    public async Task<ActionResult<MailDetailResponse>> GetMail(long mailId)
    {
        var playerId = _currentPlayer.PlayerId;

        var dto = await _mailboxService.GetMailDetailAsync(playerId, mailId);

        return Ok(MailDetailResponse.From(dto));
    }

    // 이미 만들어 둔 우편 수령 API (예시)
    [HttpPost("claim")]
    public async Task<ActionResult<ClaimMailResponse>> Claim([FromBody] ClaimMailRequest request)
    {
        var cmd = new ClaimMailCommand
        {
            PlayerId = _currentPlayer.PlayerId,
            MailId = request.MailId
        };

        var result = await _mailService.ClaimAsync(cmd);

        return Ok(new ClaimMailResponse
        {
            Player = PlayerResponse.From(result.Player),
            Reward = RewardResponse.From(result.Reward)
        });
    }
}
```

---

## 5. Application / Infrastructure DI에 우편함 서비스 추가

### 5.1. Application DI

```csharp
// MyApp.Application.DependencyInjection

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 도메인 서비스
        services.AddScoped<GachaDomainService>();
        services.AddScoped<StageDomainService>();
        services.AddSingleton<AttendanceRewardPolicy>();

        // Application 서비스
        services.AddScoped<GachaService>();
        services.AddScoped<StageService>();
        services.AddScoped<MailService>();
        services.AddScoped<MailboxService>();      // ← 여기
        services.AddScoped<AttendanceService>();
        services.AddScoped<MissionService>();
        services.AddScoped<AchievementService>();
        services.AddScoped<EventRewardService>();

        return services;
    }
}
```

### 5.2. Infrastructure DI는 이미 `IMailRepository -> DapperMailRepository`를 등록하고 있으니 그대로 사용하면 된다.

---

## 6. 요약

지금까지 내용대로라면 **우편함 기능**은 이렇게 구성된다.

* **Domain**

  * `Mail` 엔티티에 `CreatedAt`, `IsExpired(now)` 등 최소 정보/행동 추가

* **Application**

  * `MailSummaryDto`, `MailDetailDto`, `PagedMailResultDto`
  * `MailboxQuery`
  * `MailboxService`
  * `IMailRepository`에 `GetListByPlayerAsync`, `CountByPlayerAsync` 추가

* **Infrastructure(Dapper)**

  * `Mails` 테이블에 Title/Body/Reward/IsClaimed/ExpireAt/CreatedAt 저장
  * `DapperMailRepository`에서 목록/카운트 쿼리 구현

* **Api**

  * `GET /api/mail` → 우편함 목록 (페이지, 수령/만료 필터 가능)
  * `GET /api/mail/{mailId}` → 우편 상세
  * 기존 `POST /api/mail/claim` → 메일 수령

이 구조대로 구현하면, 클라이언트 쪽에서는

* 우편함 리스트 요청 → 목록/페이지 정보 수신
* 아이템 선택 시 상세 요청 → 내용 + 보상 확인
* “받기” 버튼 → 수령 API 호출 → 인벤 업데이트

이렇게 깔끔하게 흐름을 구성할 수 있다.

