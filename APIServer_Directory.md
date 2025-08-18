## API 서버의 디렉토리 구성 예
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
  