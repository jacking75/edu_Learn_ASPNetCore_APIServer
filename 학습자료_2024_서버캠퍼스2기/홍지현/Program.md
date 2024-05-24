# Program.cs

## 전체적인 흐름
Request -> Middleware -> 특정 Filter -> 처리 -> Middleware -> 응답 형식으로 진행됨.

## 객체 DI
- 디자인 패턴에서 코드간 종속성을 줄이는 것을 중요하게 생각함(loosely coupled)
- 인터페이스에 대해서 자동으로 인자를 넣어주는 것(연결)

1) Request
2) Routing
3) Controller ACtivator (DI Contatiner한테 Controller 생성 + 알맞는 Dependency 연결 위탁)
4) DI Contatiner 임무 실시
5) Controller가 생성 끝

``` cs
var builder = WebApplication.CreateBuilder(args);

builder.AddSingleton();
builder.AddScoped();
builder.AddTransient();
```

- Singleton : 싱글톤임.
- Scopped : Request -> Response 까지 객체가 생명주기를 가짐.
- Transient : 요청, 응답과 상관없이 DI된 객체가 사용될 때 마다 생성 및 초기화되는 생명주기를 가짐.

## 설정
- 기본적으로 appsettings.json을 config로 주입하며, 추가적으로 설정을 통해 설정 파일을 가져올 수 있음.
``` cs
builder.Configuration.AddJsonFile("./SecretZuZu/secretconfig.json", optional: true, reloadOnChange: true);
```
- 다음 코드를 통해 객체로 만들어서 설정을 넘겨줄 수 있음.
``` cs
public class TestObject
{
    int id {get;set;}
    string name {get;set;} = null!;
}

builder.Services.Configure<TestObject>(builder.Configuration.GetSection("~~"));
```

## CORS 설정
- 프론트와의 연동의 경우, CORS 설정이 필수임. 따라서 CORS 설정은 다음과 같이 설정하면 됨.
``` cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", builder =>
    {
        builder.WithOrigins("http://localhost:3000")        // 리액트 앱의 주소
                .AllowAnyHeader()
                .AllowAnyMethod();
    });

    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

app.UseCors("AllowReact");
```

## Exception 설정
- Exception의 경우, ModelState에서 발생하는 Exception과 Middelware에서 처리하는 Error Handler 두 가지가 존재한다.
- ModelSate Exception
- ModelState는 [Requeired] 등과 같은 Attribute 조건 측정에서 발생하는 에러들을 말한다.
``` cs
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = CustomErrorHandler.ModelStateErrorHandler;
});
```
- Middleware Exception
``` cs
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(CustomErrorHandler.MyRequestDelegate);
});
```

## 개발 환경 테스트 설정
``` cs
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
```

## Middleware
- 자세한 내용은 [Middleware](Middleware.md)에서 다룰 예정.
- 미들웨어 추가하는 코드
``` cs
app.UseAuthorization();

app.UseMiddleware<TestMiddleware>();
```

## 전체 예제 코드
- Controller에서 Attirubte로 [APiController], [Route]를 사용하는 경우
- 다음과 같이 AddController, MapControllers를 추가해야 한다.
``` cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
```

- 나머지 자세한 내용들은 아래 공식 사이트 참고
- https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/?view=aspnetcore-8.0&tabs=windows