# zLogger

---
## 목차
1. [ZLogger](#ZLogger)
2. [설치](#설치)
3. [설정](#설정)
4. [Log File](#Log-File)
   * [AddZLoggerFile](#AddZLoggerFile)
   * [AddZLoggerRollingFile](#AddZLoggerRollingFile)
   * [출력 공급자](#출력-공급자)
   * [여러 공급자 사용](#여러-공급자-사용)
   * [메서드](#메서드)


---
## ZLogger
![jpg_1](./99Resource/00Image/ZLogger/1.png)
* 사용 이유
```
일반적으로사용하는 Logger / Console.WriteLine 방식은 object 를 boxing하고 string을 UTF8로 endcoding하는데 추가적인 비용이 들었지만
ZLogger는 제로 할당 문자열 빌더 ZString에 의해 UTF8로 직접 버퍼영역에 쓰게되고 ConsoleStream에 정리해서 보내주기 때문에 
Boxing도 발생하지 않고 비동기적으로 단번에 쓰기 때문에 애플리케이션에 부하를 주지 않는다.
```
* ZLogger 설명
````
설정 및 로거 가져오기는 Microsoft.Extensions.Logging 을 따릅니다 . 
그러나 로그를 작성할 때는 접두사 ZLog 와 함께 , ZLogDebug, ZLogException등을 사용합니다 .

모든 로깅 방법은 Microsoft.Extensions.Logging.LoggerExtensions 와 완전히 유사 하지만
Z접두사가 있고 boxing 할당을 피하기 위해 많은 제네릭 오버로드가 있습니다.
 
즉 ZLogger에 대한 학습기에 아래의 msdn 링크도 같이 참고해야한다.
````
참고 : https://jacking75.github.io/NET_lib_ZLogger/  
참고 : https://github.com/Cysharp/ZLogger

MSDN : https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-6.0  
MSDN : https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions?view=dotnet-plat-ext-6.0

---

## 설치
![jpg_1](./99Resource/00Image/ZLogger/2.png)
```
Nuget에서 ZLogger를 다운받아 사용한다.
```

## 설정
1. ConfigureLogging  
![jpg_1](./99Resource/00Image/ZLogger/3.png)
```C#
Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        // 기본 제공자를 재정의해준다.
        logging.ClearProviders();
        
        // optional(MS.E.Logging): default값은 Info이며 option을 바꿔 최소 로그레벨을 지정해 줄 수 있다.
        logging.SetMinimumLevel(LogLevel.Debug);
        
        // 콘솔에 출력한다.
        logging.AddZLoggerConsole();

        // 지정된 파일에 출력한다.
        logging.AddZLoggerFile("fileName.log");

        // 날짜-시간 또는 파일 크기에 따라 출력 파일 경로를 변경한다.
        logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024);
       
        // 구조화된 로깅 지원
        logging.AddZLoggerConsole(options =>
        {
            options.EnableStructuredLogging = true;
        });
    })
```

2. ILogger, ILoggerFactory  
```C#
// ILogger
ILogger인터페이스는 실제 저장소에 로그를 기록하는 역할을 하며 로깅을 위해 생성자에 주입하는 용도로 사용합니다 
public TestController(ILogger<TestController> logger)
{
    _logger = logger;
}
```
```C#
// ILoggerFactory
ILoggerFactory유형 의 인스턴스를 만들고 ILogger로깅 공급자를 등록하는 데 사용할 수 있는 팩토리 인터페이스이며
등록된 모든 로거 공급자에 대한 래퍼 역할을 하며 생성하는 로거는 모든 로거 공급자에 한 번에 쓸 수 있습니다.

// Program.cs
var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

LogManager.SetLoggerFactory(loggerFactory, "Global");

// LogManager.cs
public static class LogManager
{
    static ILogger globalLogger;
    static ILoggerFactory loggerFactory;

    
    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
        LogManager.loggerFactory = loggerFactory;
        LogManager.globalLogger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger Logger => globalLogger;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}

자세한 내용은 아래 링크참조
```
https://github.com/Cysharp/ZLogger#global-loggerfactory  
https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerfactoryextensions.createlogger?view=dotnet-plat-ext-6.0#microsoft-extensions-logging-loggerfactoryextensions-createlogger-1(microsoft-extensions-logging-iloggerfactory)  
https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger?view=dotnet-plat-ext-6.0  
https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.iloggerfactory?view=dotnet-plat-ext-6.0  
---
## Log File
### AddZLoggerFile
```C#
AddZLoggerFile의 옵션을 세팅하여 Log의 포멧을 바꿀 수 있다.
예를들어

AddZLoggerFile을 아래와 같이 세팅하게 된다면 로그는 "[{0}][WebAPI_EDU] -> " + 출력값 이 되게 된다.

logging.AddZLoggerFile("WebAPI_EDU.log", x =>
{
    x.PrefixFormatter = (writer, info) =>
        ZString.Utf8Format(writer, "[{0}][WebAPI_EDU] -> ", info.Timestamp.ToLocalTime().DateTime);
});
// prefixFormatter 옵션을 사용하면 아래 출력된 로그와 같이 로그 이전에 출력될 포멧을 정의할 수 있습니다.


{       //실제 로그 파일 내용
    [08/11/2022 18:11:44][WebAPI_EDU] -> Now listening on: https://localhost:7024
    [08/11/2022 18:11:44][WebAPI_EDU] -> Now listening on: http://localhost:5024
    [08/11/2022 18:11:44][WebAPI_EDU] -> Application started. Press Ctrl+C to shut down.
    [08/11/2022 18:11:44][WebAPI_EDU] -> Hosting environment: Development
    [08/11/2022 18:11:44][WebAPI_EDU] -> Content root path: C:\Users\user\Documents\GitHub\C2S_Internship\WebAPIServer_edu_test\WebAPIServer_edu\
    [08/11/2022 18:11:48][WebAPI_EDU] -> test format
    [08/11/2022 18:11:48][WebAPI_EDU] -> error
    [08/12/2022 10:08:04][WebAPI_EDU] -> test format
    [08/12/2022 10:08:04][WebAPI_EDU] -> error
}


----------------------------------------------------------------------------------------------------------------------------------------------------------
// 구조화 된 로깅
/ To setup, `EnableStructuredLogging = true`.
logging.AddZLoggerConsole(options =>
{
    options.EnableStructuredLogging = true;
});


// 출력방식
// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00",
//          "Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":null}
logger.ZLogInformation("Registered User: Id = {0}, UserName = {1}", id, userName);

// 출력방식
// {"CategoryName":"ConsoleApp.Program","LogLevel":"Information","EventId":0,"EventIdName":null,"Timestamp":"2020-04-07T11:53:22.3867872+00:00",
//          "Exception":null,"Message":"Registered User: Id = 10, UserName = Mike","Payload":{"Id":10,"Name":"Mike"}}
logger.ZLogInformationWithPayload(new UserRegisteredLog { Id = id, Name = userName }, "Registered User: Id = {0}, UserName = {1}", id, userName);

Amazon Athena, Google BigQuery, Azure Data Lake, etc...에서는 json으로 데이터를 읽기 때문에 parsing하는 과정이 필요한데
구조화된 로깅 방식에서는 System.Text.Json.JsonSerializer를 사용하여 log를 json화 하는데 추가적인 할당을 하지 않는다.
자세한 내용은 아래 링크 참조
```
https://github.com/Cysharp/ZLogger#options-for-structured-logging    

### AddZLoggerRollingFile
```C#
AddZLoggerRollingFile의 매개변수는 아래와 같이 3가지 입니다.
------------------------------------------------------------------------------------------------------------------------
1. Func<DateTimeOffset, int, string> fileNameSelector

    fileNameSelector생성된 파일 경로의 선택자입니다. DateTimeOffset는 생성 시간의 UTC, int시퀀스 번호, 0 원점입니다. 
    fileNameSelector의 형식은 int(시퀀스 번호)가 마지막이어야 합니다.
------------------------------------------------------------------------------------------------------------------------
2. Func<DateTimeOffset, DateTimeOffset> timestampPattern

    timestampPattern새 파일을 생성해야 함의 술어입니다. 인수는 로그가 작성되는 현재 시간(UTC)이며, 
    반환 값이 마지막으로 작성된 시간과 다를 경우 fileNameSelector를 호출하여 새 파일에 씁니다.

------------------------------------------------------------------------------------------------------------------------
3. int rollSizeKB

    파일 크기 제한이며, 오버플로가 있는 경우 fileNameSelector를 호출하고 새 파일에 씁니다.
------------------------------------------------------------------------------------------------------------------------
```
````c#
logging.AddZLoggerRollingFile(
    fileNameSelector: (dt, x) => $"{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", 
    timestampPattern: x => x.ToLocalTime().Date, 
    rollSizeKB: 1024);

아래 사진은 실제로 11,12일에 AddZLoggerRollingFile을 통해 생성된 로그 파일이다.  
````


![jpg_1](./99Resource/00Image/ZLogger/5.png)

```C#
public class MyClass
{
   readonly ILogger<MyClass> logger;

   // 기본적으로 DI에 의해 주입된다.
   public class MyClass(ILogger<MyClass> logger)
   {
       this.logger = logger;
   }

   public void Foo()
   {
       logger.ZLogDebug("foo{0} bar{1}", 10, 20);

       logger.ZLogDebugWithPayload(new { Foo = 10, Bar = 20 }, "foo{0} bar{1}", 10, 20);
   }
}
```
---
## 출력 공급자

ZLogger에는 기본적으로 다음 공급자가 있습니다.  
![jpg_1](./99Resource/00Image/ZLogger/4.png)  

### 여러 공급자 사용
```C#
ZLogger를 사용하면 동일한 유형의 공급자를 여러 개 추가할 수 있습니다. 이 경우 아래와 같이 문자열 optionName에 다른 이름을 지정해야 합니다.
logging.AddZLoggerFile("plain-text.log", "file-plain", ...
logging.AddZLoggerFile("json.log", "file-structured", ...
```
---
## 메서드
| LogLevel    | Value | Method         | Description                                                                                                 |
|-------------|-------|----------------|-------------------------------------------------------------------------------------------------------------|
| Trace       | 0     | LogTrace       | 가장 자세한 메시지를 포함합니다. 이러한 메시지에는 민감한 앱 데이터가 포함될 수 있습니다. 이러한 메시지는 기본적으로 비활성화되어 있으며 <br/>프로덕션 환경에서 활성화하면 안 됩니다 .  |
| Debug       | 1     | LogDebug       | 디버깅 및 개발용. 대용량이므로 생산시 주의하여 사용하십시오.                                                                          |
| Information | 2     | LogInformation | 앱의 일반적인 흐름을 추적합니다. 장기적인 가치를 가질 수 있습니다.                                                                      |
| Warning     | 3     | LogWarning     | 비정상적이거나 예상치 못한 사건의 경우. 일반적으로 앱 실패를 유발하지 않는 오류 또는 조건이 포함됩니다.                                                 |
| Error       | 4     | LogError       | 처리할 수 없는 오류 및 예외의 경우. 이러한 메시지는 앱 전체의 오류가 아니라 현재 작업 또는 요청의 오류를 나타냅니다.                                        |
| Critical    | 5     | LogCritical    | 즉각적인 주의가 필요한 오류의 경우. 예: 데이터 손실 시나리오, 디스크 공간 부족.                                                             |
| None        | 6     | LogNone        | 로깅 범주가 메시지를 쓰지 않도록 지정합니다.                                                                                   |
---

* Log
```C#
해당 메서드의 LogLevel 매개변수는 로그의 심각도를 나타낸다.
그러므로 다음의 두 예시는 동일한 기능을 한다.

_logger.Log(LogLevel.Information, MyLogEvents.TestItem, routeInfo);

_logger.LogInformation(MyLogEvents.TestItem, routeInfo);

Log 출력 메서드에 관해서는 아래의 링크를 참조하고
Log + LogLevel -> (ex) LogInformation)의 형태라는 것만 알아두도록 한다.
```
Log Mathod - https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions?view=dotnet-plat-ext-6.0

* EventID를 사용한 출력
```C#
public class MyLogEvents
{
    public const int GenerateItems = 1000;
    public const int ListItems     = 1001;
    public const int GetItem       = 1002;
    public const int InsertItem    = 1003;
    public const int UpdateItem    = 1004;
    public const int DeleteItem    = 1005;

    public const int TestItem      = 3000;

    public const int GetItemNotFound    = 4000;
    public const int UpdateItemNotFound = 4001;
}

Func()
{
    _logger.LogInformation(MyLogEvents.GetItem, "Getting item {Id}", id);
    _logger.LogWarning(MyLogEvents.GetItemNotFound, "Get({Id}) NOT FOUND", id);
}

해당 방식을 통해 어떤 evnet에 출력된 Log인지 판단할 수 있는데 예시로 든 Func함수가 실행되면 다음과 같이 출력된다(출력도 예시)

info: TodoApi.Controllers.TodoItemsController[1002]Getting item 1
warn: TodoApi.Controllers.TodoItemsController[4000]Get(1) NOT FOUND

```
