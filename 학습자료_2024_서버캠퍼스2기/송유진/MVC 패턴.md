# MVC 패턴

## MVC 정의
- **모델-뷰-컨트롤러 (MVC)** 패턴은 소프트웨어 설계 패턴 중 하나로, 애플리케이션을 세 가지 주요 구성 요소인 모델(Model), 뷰(View), 컨트롤러(Controller)로 분리합니다. 이 구조는 애플리케이션의 데이터 처리(모델), 사용자 인터페이스(뷰), 입력 처리(컨트롤러)를 분리하여 각 부분의 독립적인 개발과 테스트를 가능하게 합니다.

## ASP.NET Core에서 MVC 사용하기
- **ASP.NET Core에서의 MVC 구현**: ASP.NET Core는 MVC 패턴을 통합하여, 웹 애플리케이션과 API의 개발을 용이하게 합니다. ASP.NET Core의 MVC는 라우팅, 모델 바인딩, 모델 유효성 검사, 의존성 주입 등을 지원하여 개발자가 보다 쉽게 강력하고 테스트 가능한 애플리케이션을 구축할 수 있도록 돕습니다.
```csharp
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```
위 코드는 ASP.NET Core에서 MVC 컨트롤러의 기본 구조를 보여줍니다. 
HomeController는 컨트롤러 클래스이며, Index 메서드는 뷰를 반환합니다.

## MVC의 주요 구성 요소
### 모델 (Model)
- 모델은 애플리케이션의 데이터와 비즈니스 로직을 포함합니다.
- 모델은 데이터 저장소와의 상호 작용을 처리하고, 데이터의 가공 및 관리를 담당합니다.
```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}
```
### 뷰 (View)
- 뷰는 사용자에게 보여지는 UI 요소입니다. 
- ASP.NET Core에서는 주로 Razor 뷰 엔진을 사용하여 HTML 마크업과 C# 코드를 결합하여 동적인 웹 페이지를 생성합니다.
- 우리의 경우 게임 서버 개발을 진행하기 때문에, 뷰는 크게 신경쓰지 않아도 됩니다.

### 컨트롤러 (Controller)
컨트롤러는 사용자의 입력과 상호작용을 처리하고, 모델을 사용하여 데이터를 요청하거나 변경합니다. 
또한, 적절한 뷰를 선택하고 데이터를 전달합니다.
```csharp
public class UserController : Controller
{
    public IActionResult Profile()
    {
        User user = GetUserFromDatabase();
        return View(user);
    }
}
```
# MVC 패턴의 확장된 이해와 구현

## MVC 패턴 구성

- MVC (모델-뷰-컨트롤러) 패턴은 웹 애플리케이션을 효율적으로 구조화하는 데 사용되는 인기 있는 디자인 패턴입니다. 
- 여기에서는 Controller, Model, DTO, DAO, Repository, 그리고 Service 레이어의 역할에 대해 자세히 설명합니다.

### Controller
- **Controller**는 사용자의 요청을 받아 처리하는 역할을 담당합니다. 각 요청은 특정 URI와 연결되며, HTTP 메소드(get, post 등)에 따라 적절한 작업을 실행합니다.
```csharp
[HttpGet]
public ActionResult<List<User>> GetAllUsers()
{
    return userService.GetAllUsers();
}

[HttpPost]
public IActionResult AddUser(User user)
{
    userService.AddUser(user);
    return CreatedAtAction(nameof(AddUser), new { id = user.Id }, user);
}
```
### Model, DTO, and DAO
Model은 애플리케이션의 데이터 구조를 정의합니다. 데이터 전송 객체(DTO)와 데이터 접근 객체(DAO)는 모델 아래에 위치합니다.
- **DTO (Data Transfer Object)**: 다양한 계층 간 데이터 전송을 위해 사용됩니다.
  + DB의 데이터에 접근하기 위한 객체
  + “DB에 접근하기 위한 로직”과 “비지니스 로직”을 분리하기 위해 사용
- **DAO (Data Access Object)**: 데이터베이스와의 상호작용을 처리합니다.
  + 계층(controller, view, business layer) 간 데이터 교환을 위한
  + 데이터 로직을 가지지 않는 순수한 데이터 객체
    * 따라서 getter, setter만 가짐
          - 여기서 setter가 있기에 값이 변할 수 있음

  - → [유저가 입력한 데이터를 DB에 넣는 과정]
        + 유저가 데이터를 입력하여 form에 있는 데이터를 DTO에 넣어서 전송
        + 해당 DTO를 받은 서버가 DAO를 이용하여 데이터베이스로 데이터를 집어넣음
- VO (Value Object)
    - Read-only 속성을 가진 **값 오브젝트**
    - getter

### Repository
Repository는 데이터베이스와의 직접적인 상호작용을 캡슐화합니다. 이곳에서는 데이터의 CRUD (생성, 읽기, 업데이트, 삭제) 연산을 처리합니다.
Repository는 클래스와 인터페이스로 구성되며, 데이터베이스 연결 및 데이터 조작을 담당합니다.
```csharp
public class UserRepository : IUserRepository
{
    private readonly DbContext context;

    public UserRepository(DbContext context)
    {
        this.context = context;
    }

    public User GetUserById(int id)
    {
        return context.Users.Find(id);
    }
}
```

### Service Layer
- Service Layer는 Repository에서 제공하는 메소드를 사용하여 비즈니스 로직을 구현합니다. 
- 복잡한 비즈니스 규칙이나 트랜잭션 관리, 예외 처리 등을 이 계층에서 처리합니다.
``` csharp
public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public void AddUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException("User is null");

        userRepository.Add(user);
    }
}
```
