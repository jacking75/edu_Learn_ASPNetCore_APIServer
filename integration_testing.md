# 통합테스트
from: ChatGPT  
  
통합테스트(integration test)는 **애플리케이션의 여러 컴포넌트를 실제로 연결해서 전체 흐름이 제대로 동작하는지 검증하는 테스트**다.
즉, 유닛 테스트가 “각각의 단위 로직”을 검증한다면, 통합 테스트는 다음을 확인한다:

* 실제 DB와 잘 통신하는가
* Repository + Service + DB + ORM 흐름이 전체적으로 맞는가
* 트랜잭션이 제대로 동작하는가
* 쿼리가 실제 MySQL에서 의도대로 실행되는가
* 스키마 제약(Unique, FK, 인덱스)이 제대로 적용되는가

즉, **“현실 환경과 최대한 비슷하게 돌려서 전체 기능이 진짜로 돌아가는지”를 확인하는 테스트**다.

---

# 1. 통합테스트가 왜 필요한가?

예를 들어 이런 코드가 있다고 하자:

```csharp
var user = _db.Users.FirstOrDefault(u => u.Name == name);
```

유닛 테스트에서는 Mock으로 Users 리스트를 만들어서 테스트할 수 있다.
하지만 현실에서는 다음 문제들이 발생할 수 있다:

* 실제 MySQL에서는 문자열 비교가 대소문자 구분됨
* 인덱스가 없어서 성능이 느려짐
* 컬럼 타입이 다르면 EF 매핑에서 예외가 발생
* 트랜잭션 스코프가 실제 DB에서 의도와 다르게 동작

이런 문제는 **유닛 테스트로 절대 못 잡고, 통합 테스트로만 잡을 수 있다**.

---

# 2. 통합테스트 구현 방법(ASP.NET Core + xUnit 기준)

ASP.NET Core에서는 기본적으로 다음 두 가지 방식이 많이 사용된다.

---

## 2-1. **실제 DB(MySQL)와 연결해서 테스트**

가장 현실적인 방식이다.

테스트 전용 DB를 하나 만든 뒤:

1. 테스트 시작할 때 DB 초기화
2. 테스트 데이터 Insert
3. 실제 Repository/Service 로직 호출
4. DB에 반영된 상태 또는 반환값 검증
5. 테스트 끝나면 데이터 삭제(또는 테스트용 DB 자체를 초기화)

예: xUnit + EF Core + MySQL

```csharp
public class UserRepositoryTests
{
    private readonly AppDbContext _db;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql("Server=localhost;Database=test;User=root;Password=1234;",
                      new MySqlServerVersion(new Version(8,0,25)))
            .Options;

        _db = new AppDbContext(options);
    }

    [Fact]
    public async Task AddUser_ShouldInsertIntoDatabase()
    {
        var repo = new UserRepository(_db);

        var user = new User { Name = "test" };
        await repo.Add(user);

        var savedUser = await _db.Users.FirstOrDefaultAsync(u => u.Name == "test");

        Assert.NotNull(savedUser);
    }
}
```

장점

* 실제 환경과 가장 가까워서 신뢰도 높음

단점

* DB 초기화가 필요 → 느림
* 테스트 실행 환경에서 MySQL이 필요

그래서 다음 방법이 자주 쓰인다:

---

## 2-2. **Docker TestContainer 사용**

테스트 실행 시 Docker로 MySQL 컨테이너를 자동 실행하고
테스트 끝나면 자동으로 삭제하는 방식이다.

→ 개발 PC, CI/CD 환경에서 항상 동일한 DB 환경으로 테스트 가능.

간단 예(테스트 컨테이너 라이브러리 사용):

```csharp
public class MySqlIntegrationTest : IAsyncLifetime
{
    private MySqlContainer _container;
    public AppDbContext Db { get; private set; }

    public async Task InitializeAsync()
    {
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithUsername("root")
            .WithPassword("1234")
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(_container.GetConnectionString(),
                    new MySqlServerVersion("8.0"))
                .Options;

        Db = new AppDbContext(options);
        await Db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
```

이 방식은 널리 사용되고 유지보수도 편하다.

---

# 3. ASP.NET Core 전체 API 통합 테스트도 가능

Repository 레벨이 아니라 API 전체 흐름도 테스트할 수 있다.

`WebApplicationFactory<TEntryPoint>` 사용:

```csharp
public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_users_id_ReturnsOk()
    {
        var res = await _client.GetAsync("/users/1");

        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

이건 **Controller → Service → Repository → DB** 전체 플로우를 검증한다.

---

# 4. 통합 테스트 구성 요약

| 테스트 타입               | 특징                           | DB 사용            |
| -------------------- | ---------------------------- | ---------------- |
| **유닛 테스트**           | 로직 단위 검증                     | ❌ 없음 (Mock/Fake) |
| **통합 테스트**           | Service/Repository/DB 함께 테스트 | ⭕ 있음             |
| **엔드 투 엔드 테스트(E2E)** | API 전체 흐름 테스트                | ⭕ 있음             |

---

# 5. 실제 프로젝트에서는 보통 이렇게 구성한다

* **유닛 테스트**: 빠르고 로직 중심 (70%)
* **통합 테스트**: 느리지만 정확한 DB 기반 테스트 (20%)
* **E2E 테스트**: 실제 API 검증 (10%)

특히 DB 기반 서비스라면 통합 테스트 비율이 더 높아지는 것도 일반적이다.

---

</br>  
</br>  
  
  
## 1. 실제 MySQL + EF Core 통합 테스트 예제

### 1-1. 예제 도메인

아주 단순하게 **유저가 아이템을 구매하는** 도메인이라고 하자.

```csharp
// Domain
public class User
{
    public long Id { get; set; }
    public int Cash { get; set; }
    public List<UserItem> Items { get; set; } = new();
}

public class UserItem
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public int ItemId { get; set; }
}

public class ShopItem
{
    public int Id { get; set; }
    public int Price { get; set; }
}
```

DbContext는 이렇게 둔다.

```csharp
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserItem> UserItems { get; set; }
    public DbSet<ShopItem> ShopItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Items)
            .WithOne()
            .HasForeignKey(i => i.UserId);
    }
}
```

Repository 예제:

```csharp
public interface IUserRepository
{
    Task<User?> GetAsync(long id);
    Task SaveAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly GameDbContext _db;

    public UserRepository(GameDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetAsync(long id)
    {
        return _db.Users
            .Include(u => u.Items)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task SaveAsync(User user)
    {
        _db.Update(user);
        await _db.SaveChangesAsync();
    }
}
```

Service 예제:

```csharp
public class PurchaseService
{
    private readonly GameDbContext _db;
    private readonly IUserRepository _users;

    public PurchaseService(GameDbContext db, IUserRepository users)
    {
        _db = db;
        _users = users;
    }

    public async Task<bool> PurchaseAsync(long userId, int itemId)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        var user = await _users.GetAsync(userId);
        var item = await _db.ShopItems.FindAsync(itemId);

        if (user == null || item == null)
            return false;

        if (user.Cash < item.Price)
            return false;

        user.Cash -= item.Price;
        user.Items.Add(new UserItem { ItemId = item.Id });

        await _users.SaveAsync(user);
        await tx.CommitAsync();
        return true;
    }
}
```

---

### 1-2. 실제 MySQL 통합 테스트 환경 구성

전제: `test_game` 같은 **테스트 전용 DB**를 만들어둔다.

`appsettings.Test.json` 같은 데에 연결 문자열을 분리해 두거나, 테스트 코드에서 직접 설정해도 된다.

```csharp
// Test의 공통 Fixture
public class MySqlFixture : IAsyncLifetime
{
    public GameDbContext Db { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=test_game;User=root;Password=1234;",
                new MySqlServerVersion(new Version(8, 0, 25)))
            .Options;

        Db = new GameDbContext(options);

        // 스키마 없으면 생성
        await Db.Database.EnsureCreatedAsync();

        // 매 테스트 전 초기화가 필요하면 여기서 TRUNCATE 등의 작업을 수행해도 된다
    }

    public Task DisposeAsync()
    {
        Db.Dispose();
        return Task.CompletedTask;
    }
}
```

테스트 클래스:

```csharp
public class PurchaseServiceIntegrationTests : IClassFixture<MySqlFixture>
{
    private readonly GameDbContext _db;

    public PurchaseServiceIntegrationTests(MySqlFixture fixture)
    {
        _db = fixture.Db;
    }

    [Fact]
    public async Task Purchase_Succeeds_And_Persists_To_Database()
    {
        // Arrange: 깨끗한 상태를 위해 기존 데이터 삭제
        _db.Users.RemoveRange(_db.Users);
        _db.ShopItems.RemoveRange(_db.ShopItems);
        await _db.SaveChangesAsync();

        var user = new User { Cash = 1000 };
        var item = new ShopItem { Id = 1, Price = 500 };

        await _db.Users.AddAsync(user);
        await _db.ShopItems.AddAsync(item);
        await _db.SaveChangesAsync();

        var repo = new UserRepository(_db);
        var service = new PurchaseService(_db, repo);

        // Act
        var result = await service.PurchaseAsync(user.Id, item.Id);

        // Assert
        result.Should().BeTrue();

        var saved = await _db.Users
            .Include(u => u.Items)
            .FirstAsync(u => u.Id == user.Id);

        saved.Cash.Should().Be(500);
        saved.Items.Should().Contain(i => i.ItemId == item.Id);
    }
}
```

이렇게 하면 **실제 MySQL + 실제 EF 쿼리 + 실제 트랜잭션**까지 다 검증하는 통합 테스트가 된다.

---

## 2. Docker + Testcontainers 기반 추천 구조

로컬 MySQL 설치 안 하고, 테스트 돌릴 때마다 컨테이너를 띄워서 테스트하는 방식이다.

### 2-1. 필요한 NuGet 패키지

```plaintext
dotnet add package Testcontainers
dotnet add package Testcontainers.MySql
```

### 2-2. MySql 컨테이너 Fixture

```csharp
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

public class MySqlContainerFixture : IAsyncLifetime
{
    public MySqlContainer Container { get; private set; } = null!;
    public GameDbContext Db { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Container = new TestcontainersBuilder<MySqlContainer>()
            .WithDatabase(new MySqlTestcontainerConfiguration
            {
                Database = "test_game",
                Username = "root",
                Password = "1234"
            })
            .WithImage("mysql:8.0")
            .WithCleanUp(true)
            .Build();

        await Container.StartAsync();

        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseMySql(
                Container.GetConnectionString(),
                new MySqlServerVersion(new Version(8, 0, 25)))
            .Options;

        Db = new GameDbContext(options);
        await Db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}
```

### 2-3. 통합 테스트

```csharp
public class PurchaseServiceWithContainerTests : IClassFixture<MySqlContainerFixture>
{
    private readonly GameDbContext _db;

    public PurchaseServiceWithContainerTests(MySqlContainerFixture fixture)
    {
        _db = fixture.Db;
    }

    [Fact]
    public async Task Purchase_Works_On_Real_MySql_In_Container()
    {
        _db.Users.RemoveRange(_db.Users);
        _db.ShopItems.RemoveRange(_db.ShopItems);
        await _db.SaveChangesAsync();

        var user = new User { Cash = 1000 };
        var item = new ShopItem { Id = 1, Price = 500 };

        await _db.Users.AddAsync(user);
        await _db.ShopItems.AddAsync(item);
        await _db.SaveChangesAsync();

        var repo = new UserRepository(_db);
        var service = new PurchaseService(_db, repo);

        var result = await service.PurchaseAsync(user.Id, item.Id);

        result.Should().BeTrue();

        var saved = await _db.Users
            .Include(u => u.Items)
            .FirstAsync(u => u.Id == user.Id);

        saved.Cash.Should().Be(500);
        saved.Items.Should().Contain(i => i.ItemId == item.Id);
    }
}
```

이 구조의 장점은 다음과 같다.

* 로컬/CI 어디에서도 **동일한 DB 버전**으로 테스트 가능하다
* DB를 따로 설치/관리할 필요가 없다
* 테스트 종료 시 컨테이너와 데이터가 모두 삭제된다

---

## 3. 게임 서버에서 레포지토리/서비스 통합 테스트를 어떻게 나누나

게임 서버 기준으로 대략 이렇게 레벨을 나누는 게 좋다.

### 3-1. Repository 통합 테스트

목표: **쿼리와 매핑이 제대로 되는지** 확인하는 테스트다.

* `UserRepository.GetByNickname`이 실제로 올바른 WHERE/JOIN을 쓰는지
* `InventoryRepository.GetEquipments`가 실제 DB 구조와 맞는지
* 샤딩 키, 파티션 키가 의도대로 적용되는지
* Lazy/Eager Loading 전략으로 N+1 문제를 피하고 있는지 등

예:

```csharp
public class UserRepositoryIntegrationTests : IClassFixture<MySqlContainerFixture>
{
    private readonly GameDbContext _db;
    private readonly IUserRepository _repo;

    public UserRepositoryIntegrationTests(MySqlContainerFixture fixture)
    {
        _db = fixture.Db;
        _repo = new UserRepository(_db);
    }

    [Fact]
    public async Task GetAsync_Returns_User_With_Items()
    {
        _db.Users.RemoveRange(_db.Users);
        _db.UserItems.RemoveRange(_db.UserItems);
        await _db.SaveChangesAsync();

        var user = new User { Cash = 1000 };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        await _db.UserItems.AddAsync(new UserItem { UserId = user.Id, ItemId = 10 });
        await _db.SaveChangesAsync();

        var result = await _repo.GetAsync(user.Id);

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);
        result.Items.First().ItemId.Should().Be(10);
    }
}
```

여기서는 **Repository + DbContext + MySQL**만 테스트한다.
Service, Controller는 개입하지 않는다.

---

### 3-2. Service 통합 테스트

목표: **비즈니스 흐름(트랜잭션, 여러 Repository 사용, 상태 변경)을 검증**하는 테스트다.

* 여러 Repository/도메인 객체를 조합하는 Purchase, Matchmaking, Reward 지급 등
* 트랜잭션 안에서 여러 엔티티가 같이 바뀌는 로직
* 외부 시스템(메시지 큐, Redis, gRPC)과 상호작용하는 부분

패턴은 보통 이렇게 나눈다.

* DB는 실제 사용
* 외부 시스템(예: Kafka, Redis)은 Mock 또는 In-memory 구현 사용
* 진짜로 외부까지 붙이는 건 e2e 테스트에서 소량만 수행

Service 통합 테스트 예:

```csharp
public class PurchaseServiceIntegrationTests2 : IClassFixture<MySqlContainerFixture>
{
    private readonly GameDbContext _db;
    private readonly PurchaseService _service;

    public PurchaseServiceIntegrationTests2(MySqlContainerFixture fixture)
    {
        _db = fixture.Db;

        var userRepo = new UserRepository(_db);
        _service = new PurchaseService(_db, userRepo);
    }

    [Fact]
    public async Task Purchase_Withdraws_Cash_And_Adds_Item_In_Same_Transaction()
    {
        _db.Users.RemoveRange(_db.Users);
        _db.UserItems.RemoveRange(_db.UserItems);
        _db.ShopItems.RemoveRange(_db.ShopItems);
        await _db.SaveChangesAsync();

        var user = new User { Cash = 500 };
        var item = new ShopItem { Id = 1, Price = 500 };

        await _db.Users.AddAsync(user);
        await _db.ShopItems.AddAsync(item);
        await _db.SaveChangesAsync();

        var result = await _service.PurchaseAsync(user.Id, item.Id);

        result.Should().BeTrue();

        var saved = await _db.Users
            .Include(u => u.Items)
            .FirstAsync(u => u.Id == user.Id);

        saved.Cash.Should().Be(0);
        saved.Items.Should().Contain(i => i.ItemId == item.Id);
    }
}
```

---

## 4. 통합 테스트에서 트랜잭션 롤백 처리하는 기술

통합 테스트의 큰 문제는 **데이터가 계속 쌓이면서 서로 영향을 주는 것**이다.
대표적인 해결 패턴은 세 가지 정도가 있다.

### 4-1. 테스트마다 트랜잭션 열고 끝나면 롤백

각 테스트가 **자기 트랜잭션 안에서만 연산**하고, 테스트 끝날 때 롤백하는 방식이다.

xUnit에서 Fixture + IAsyncLifetime을 써서 구현할 수 있다.

```csharp
public class TransactionalTestBase : IAsyncLifetime
{
    protected readonly GameDbContext Db;
    private IDbContextTransaction _tx = null!;

    public TransactionalTestBase()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseMySql("Server=localhost;Database=test_game;User=root;Password=1234;",
                new MySqlServerVersion(new Version(8, 0, 25)))
            .Options;

        Db = new GameDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await Db.Database.OpenConnectionAsync();
        _tx = await Db.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _tx.RollbackAsync();
        await Db.DisposeAsync();
    }
}
```

그리고 테스트 클래스는 이렇게 상속한다.

```csharp
public class PurchaseServiceTxTests : TransactionalTestBase
{
    [Fact]
    public async Task Purchase_Rollsback_After_Test()
    {
        var user = new User { Cash = 1000 };
        var item = new ShopItem { Id = 1, Price = 500 };
        await Db.Users.AddAsync(user);
        await Db.ShopItems.AddAsync(item);
        await Db.SaveChangesAsync();

        var repo = new UserRepository(Db);
        var service = new PurchaseService(Db, repo);

        var result = await service.PurchaseAsync(user.Id, item.Id);

        result.Should().BeTrue();
        // 여기까지는 DB 안에 실제로 들어가 있지만,
        // 테스트가 끝나면 트랜잭션 롤백으로 깨끗해진다.
    }
}
```

주의할 점은 다음과 같다.

* Service 내부에서 별도의 트랜잭션을 열면(**Nested Transaction**) 약간 꼬일 수 있다
  → 가능하면 “테스트 코드에서 트랜잭션을 관리”하고, Service는 DbContext의 Transaction을 그대로 사용하도록 설계하는 것이 깔끔하다.

---

### 4-2. 테스트마다 DB 초기화 (TRUNCATE, Respawn 등)

테스트 앞/뒤로 모든 테이블을 TRUNCATE하거나, `Respawn` 같은 라이브러리로 DB 상태를 초기화하는 방식이다.

장점

* 트랜잭션/Isolation 걱정이 적다
* 테스트 코드에서 트랜잭션을 크게 신경 쓸 필요가 없다

단점

* 테이블 수가 많으면 초기화가 느려질 수 있다

단순 TRUNCATE 패턴:

```csharp
public static class DbTestHelper
{
    public static async Task ResetDatabaseAsync(GameDbContext db)
    {
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE UserItems;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Users;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ShopItems;");
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
    }
}
```

테스트에서:

```csharp
[Fact]
public async Task Purchase_Works_With_Clean_Db()
{
    await DbTestHelper.ResetDatabaseAsync(Db);

    // Arrange, Act, Assert...
}
```

---

### 4-3. 테스트 전용 DB를 아예 매번 새로 만들고, 끝나면 삭제

* Testcontainers + MySQL에서 **컨테이너 하나 = 테스트 세션 하나**
* 각 테스트 클래스마다 새로운 DB 컨테이너를 띄워서 사용하고, 끝나면 날리는 방식이다

이 방식은 “테스트 격리”가 가장 확실하다.
단, 성능 비용이 크기 때문에 보통은 **테스트 클래스 단위로 컨테이너 하나**를 공유하게 만든다.

---

## 마무리 정리

* **실제 MySQL + EF Core 통합 테스트**

  * 테스트 전용 DB를 만들고, DbContext를 실제 MySQL에 붙인다
  * Repository/Service를 그대로 사용해 CRUD + 트랜잭션 동작을 검증한다

* **Docker + Testcontainers**

  * 테스트 시작 시 MySQL 컨테이너를 띄우고, 끝나면 제거한다
  * 환경 의존성을 줄이고 CI에도 동일한 환경을 제공한다

* **게임 서버에서 레이어별 통합 테스트**

  * Repository 통합 테스트: 쿼리/매핑/샤딩/스키마 검증
  * Service 통합 테스트: 트랜잭션, 여러 Repository/도메인 조합, 비즈니스 흐름 검증

* **트랜잭션 롤백 패턴**

  * 테스트마다 트랜잭션 시작 → 테스트 끝날 때 롤백
  * TRUNCATE/Respawn 등으로 DB를 매 테스트마다 초기화
  * 컨테이너/테스트 DB를 테스트 세션마다 새로 만들고 삭제
  
---   

</br>   
</br>   
  

## 유닛테스트와 통합테스트 프로젝트를 각각 분리해야 하는가?
  
1. **유닛테스트 / 통합테스트를 반드시 다른 프로젝트로 분리해야 하는 건 아니다**
2. 하지만 **규모가 조금만 커져도 분리하는 편이 훨씬 편하다**
3. **통합테스트 실행 시점은 “비용 대비”로 나눠서**

   * 유닛 테스트: *매 빌드, 매 커밋, 매 PR*
   * 통합 테스트: *PR 단계 또는 merge 이후 / 주기적(CI 파이프라인)*
     정도로 가져가는 게 보통 좋다.

아래에서 좀 더 구체적으로 설명한다.
  

## 1. 프로젝트를 분리해야 하는가?

### 1-1. 선택지 2개

일반적으로 두 가지 패턴이 있다.

1. **테스트 프로젝트 하나에 유닛 + 통합을 다 넣는 패턴**

   * `MyApp.Tests`

     * `Unit/…`
     * `Integration/…`

2. **테스트 프로젝트를 타입별로 분리하는 패턴**

   * `MyApp.UnitTests`
   * `MyApp.IntegrationTests`

두 방식 모두 가능하다. 프레임워크에서 강제하는 건 없다.

---

### 1-2. “하나로 쓰는” 경우 장단점

**장점**

* 솔루션 구조가 단순하다
* 공용 테스트 유틸(빌더, 헬퍼 등) 공유가 쉽다
* 작은/중간 규모 프로젝트는 이 정도로도 충분하다

**단점**

* 유닛/통합이 물리적으로 섞여 있어서

  * 어떤 테스트가 느리고, 어떤 테스트가 빠른지 구분이 흐려진다
  * `dotnet test` 한 번 돌리면 느린 통합테스트까지 같이 돌아간다
* 실행 필터링을 Trait/Category로 해야 해서 설정이 약간 귀찮다

이 방식이면 보통 xUnit 기준으로 이렇게 태깅한다.

```csharp
public class PurchaseServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Some_unit_test() { ... }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Some_integration_test() { ... }
}
```

그리고 실행할 때:

* 유닛 테스트만:
  `dotnet test --filter "Category=Unit"`
* 통합 테스트만:
  `dotnet test --filter "Category=Integration"`

---

### 1-3. “프로젝트를 분리하는” 경우 장단점

**장점**

1. **실행 전략 분리가 쉬움**

   * CI에서 `MyApp.UnitTests`는 항상, `MyApp.IntegrationTests`는 선택적으로 실행
   * 로컬에서도 솔루션 탐색기에서 프로젝트 단위로 바로 실행 가능

2. **참조/의존성 분리가 깔끔함**

   * 통합 테스트 프로젝트는

     * `WebApplicationFactory<Program>` 사용
     * Docker / Testcontainers, 실제 DB 연결 문자열, appsettings.Integration.json 등
   * 유닛 테스트 프로젝트는

     * Mock 라이브러리(Moq, NSubstitute 등) 위주만 참조

3. **환경 설정이 다름**

   * 통합 테스트는 보통 별도 `appsettings.Integration.json`,
     실제 DB 주소, 테스트용 Redis/Kafka 등 필요
   * 이걸 유닛테스트와 같이 쓰면 config가 꼬이기 쉽다

**단점**

* 프로젝트/솔루션이 늘어난다
* 공용 테스트 유틸을 다른 프로젝트에 공유하려면

  * 공용 테스트 라이브러리(`MyApp.TestCommon`)를 하나 더 만들 수도 있다

**ASP.NET Core에서는**
`MyApp.Api` (실제 웹 프로젝트)
`MyApp.UnitTests`
`MyApp.IntegrationTests`
이 구조가 꽤 흔한 패턴이다.

---

## 2. 통합 테스트는 언제 실행하는 게 좋은가?

핵심은 “느리고, 외부 의존성이 있으니까” **유닛만큼 자주 돌리기 힘들다**는 점이다. 보통 이렇게 나눈다.

### 2-1. 유닛 테스트

* 실행 비용: 매우 빠름 (ms~수십 ms 단위)
* 의존성: 없음 (DB, 네트워크 X)
* 역할: 개발자가 안심하고 리팩토링할 수 있게 해주는 최소 안전망

**실행 타이밍**

* 로컬:

  * 큰 변경 전에 한번, 커밋 전에 한번 돌리는 걸 추천
* CI:

  * *모든 PR / 모든 push*에서 항상 실행

---

### 2-2. 통합 테스트

* 실행 비용: 상대적으로 느림 (초 단위, DB 초기화 포함)
* 의존성: 실제 DB, Docker, 외부 시스템 등
* 역할:

  * ORM + DB 스키마 + 트랜잭션 + 실제 HTTP 파이프라인이 잘 붙는지 검증
  * “로직은 맞는데, 실 서버에서는 왜 깨지냐?”를 방지

**실행 전략 몇 가지 케이스**

1. **규모가 작은 팀 / 프로젝트 초기**

   * CI에서 PR마다 유닛 + 통합 테스트 모두 실행해도 된다
   * 테스트 수가 적고 DB도 가벼워서 시간 부담이 크지 않다

2. **규모가 커질 때(통합테스트가 느려지는 시점)**
   많이 쓰는 패턴은:

   * PR 생성/업데이트 시:

     * 유닛 테스트 **항상**
     * 통합 테스트는

       * 특정 브랜치만(`main`, `develop`) 실행
       * 혹은, label을 달았을 때만 (예: `run-integration-tests`)
   * 일정 주기:

     * 밤마다 / 몇 시간마다 전체 통합 테스트 실행 (스케줄 CI)

3. **로컬 개발자 워크플로우**

   * 유닛 테스트:

     * 뭔가 로직을 만졌으면 자주 돌리는 편이 좋다
   * 통합 테스트:

     * 인프라나 DB 쪽을 건드린 날, PR 올리기 전 등 *중요한 변경 전*에 한 번 돌리는 정도로도 충분하다

---

## 3. 정리: 어떻게 하는 걸 추천하나?

현실적인 추천안을 적어 본다.

### 3-1. 솔루션 구조 추천

프로젝트가 이제 막 시작이거나 중간 규모라면:

* `MyGame.Api` (ASP.NET Core 프로젝트)
* `MyGame.UnitTests`
* `MyGame.IntegrationTests`

이렇게 **분리하는 쪽**을 추천한다.

이유:

* 통합 테스트 쪽은 반드시

  * 실제 DB 연결
  * Docker / Testcontainers
  * `appsettings.Integration.json`
    같은 걸 잡게 되는데,
    이걸 유닛테스트와 같은 프로젝트에 섞어두면 나중에 반드시 꼬인다.

(이미 `MyGame.Tests` 하나만 있는 상태라면, 당장 갈아엎지는 말고, 시간이 될 때 분리하는 방식으로 진행해도 된다)

---

### 3-2. 실행 정책 추천

1. **유닛 테스트**

   * 로컬: 자주, 커밋 전에 실행
   * CI: 모든 PR / 모든 push에서 실행 (필수 통과로 설정)

2. **통합 테스트**

   * CI:

     * 기본: `develop` / `main` 브랜치에 push 시 실행
     * 또는 모든 PR에서 실행하되, 특정 job으로 분리해서 느리면 나중에 결과를 보게 할 수도 있다
   * 로컬:

     * DB, 레포지터리, 컨트롤러 레벨에 영향을 주는 큰 변경을 했을 때
     * PR 올리기 전 “한 번은 돌려본다” 정도의 문화로 가져가면 좋다

이렇게 하면:

* 유닛 테스트 → 빠르고 자주, 개발자 리듬을 해치지 않게
* 통합 테스트 → 느린 대신, 인프라/실제 환경과의 호환성을 보장하는 안전망으로 사용  