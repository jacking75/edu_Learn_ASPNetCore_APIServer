# ZLogger 사용 가이드

> ZLogger는 `Microsoft.Extensions.Logging` 위 구축된 고성능 로거로,
> C# 10.0의 향상된 String Interpolation을 활용하여 로깅 속도를 높이고 메모리 할당을 최소화합니다.

간편하고 적합한 [ZLogger](https://github.com/Cysharp/ZLogger) 사용을 위하여, 구성 및 사용 방법을 정리합니다.

### 목차

- [ZLogger 시작하기](#zlogger-시작-하기)
  - [설치하기](#설치하기)
  - [프로젝트에 적용하기](#프로젝트에-적용하기)
    - [Dependency Injection](#didependency-injection)
    - [LoggerFactory](#loggerfactory)
    - [Global LoggerFactory](#global-loggerfactory)
- [로그의 기본 구성](#로그의-기본-구성)
- [로그 출력 방법 선택](#logging-providers)
  - [Provider 종류](#provider-types)
- [로그 출력 옵션 ](#provider-options)
  - [공통 옵션](#공통-옵션-zloggeroptions)
  - [콘솔 출력용 옵션](#콘솔-출력-전용-옵션-zloggerconsoleoptions)
- [로그 형식 지정](#zlogger-formatter-configuration)
  - [JSON](#json)
  - [로그 별 사용자 지정](#로그별-형식-사용자-지정하기)
- [로그 구조화](#zlogger를-활용한-로그-구조화)

# ZLogger 시작 하기

## 설치하기

프로젝트 경로에서 아래 커맨드를 실행 또는 NuGet 패키지 매니저를 활용하여 설치 할 수 있습니다.

```bash
dotnet add package ZLogger
```

## 프로젝트에 적용하기

ASP .NET Core에서 ZLogger를 적용하는 가장 간단한 방법은 아래와 같습니다.

```csharp
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

// 기본 로거 설정 제외
builder.Logging.ClearProviders();       // 전체 제외 추천

// 로그 제공자 추가
builder.Logging.AddZLoggerConsole();    // 콘솔 출력
```

위 예시에서는 로그를 콘솔에 출력하는 `ZLoggerConsole`을 사용합니다.

추가적인 로그 제공자 정보는 [Logging Providers](#logging-providers)에서 확인 가능합니다.

### DI(Dependency Injection)

제공자 설정 이후 DI 사용을 통해 원하는 클래스에서 로그 출력 메서드를 쓸 수 있습니다.

```csharp
using Microsoft.Extensions.Logging;
using ZLogger;

public class SomeService(ILogger<MyClass> logger)
{
    public void SomeServiceLog(string name, string city, int age)
    {
        logger.ZLogInformation($"Hello, {name} lives in {city} {age} years old.");
    }
}
```

출력 예시:

```powershell
# name = "Bill", city = "Kumamoto", age = 21
> Hello, Bill lives in Kumamoto 21 years old.
```

`Log*`가 아닌 `ZLog*` 형식의 메서드 사용을 통하여 출력하도록 합니다.

### LoggerFactory

또는 LoggerFactory를 직접 생성하여 만들 수 있습니다.

```csharp
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.SetMinimumLevel(LogLevel.Trace);
});

var logger = loggerFactory.CreateLogger<YourClass>();

var name = "foo";
logger.ZLogInformation($"Hello, {name}!");
```

### Global LoggerFactory

LoggerFactory를 전역 설정하여, DI 없이 타입별 로거를 가져와 사용할 수도 있습니다.

```csharp
// LogManager.cs
public static class LogManager
{
    static ILogger globalLogger = default!;
    static ILoggerFactory loggerFactory = default!;

    public static void SetLoggerFactory(ILoggerFactory factory, string categoryName)
    {
        loggerFactory = factory;
        globalLogger = factory.CreateLogger(categoryName);
    }

    public static ILogger Logger => globalLogger;

    public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
    public static ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
}
```

이렇게 사용자 정의 매니저 클래스를 만든 후

```csharp
// Program.cs
using var host = Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddZLoggerConsole();
    })
    .Build();

var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

LogManager.SetLoggerFactory(loggerFactory, "Global");
```

`Program.cs`에서 위와 같이 설정하고 다음과 같이 사용할 수 있습니다.

```csharp
public class Foo
{
    static readonly ILogger<Foo> logger = LogManager.GetLogger<Foo>();

    public void Foo(int x)
    {
        logger.ZLogDebug($"do do do: {x}");
    }
}
```

## 로그의 기본 구성

Zlogger를 통해 제공받을 수 있는 로그의 구성은 다음과 같습니다.

| Key Name     | Description                                                                                                                                                                                                            |
| :----------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Category `  | 생성된 Logger의 할당된 카테고리 이름, 주로 클래스나 모듈의 전체 이름을 나타냅니다. <br/>**ex:** ILogger<HomeController\> 를 통해 쓰인 로그의 Category는 `App.Controllers.HomeController`가 됩니다                      |
| `Timestamp ` | 로그가 기록된 시간을 나타냅니다.                                                                                                                                                                                       |
| `LogLevel`   | `Microsoft.Extensions.Logging`에서 제공되는 로그의 심각도를 나타냅니다.                                                                                                                                                |
| `EventId `   | `Microsoft.Extensions.Logging`에서 제공되는 이벤트 ID입니다. 각 로그 항목에 대한 고유 식별자 역할을 합니다.                                                                                                            |
| `ScopeState` | `ILogger.BeginScope(...)`를 통해 설정된 추가 속성들을 포함합니다. 이는 로그 항목과 관련된 추가 정보를 저장합니다. (`ZLoggerOptions.IncludeScopes = true`일 경우에만 해당, [Provider Options](#provider-options) 참고). |
| `ThreadInfo` | 로그 항목이 생성될 당시의 쓰레드 ID 및 관련 쓰레드 컨텍스트를 저장합니다.                                                                                                                                              |
| `Context`    | 로그 기록 시 전달된 추가 객체입니다.                                                                                                                                                                                   |
| `MemberName` | 로그 호출 시의 멤버 이름을 나타냅니다. `CallerMemberName`을 통해 자동으로 설정되며, 호출된 메서드의 이름이 기록됩니다.                                                                                                 |
| `FilePath`   | 로그 항목이 호출된 소스 파일의 전체 경로를 나타냅니다.                                                                                                                                                                 |
| `LineNumber` | 로그가 생성된 소스 파일의 줄 번호를 나타냅니다                                                                                                                                                                         |

> 로그 샘플

```csharp
builder.AddZLoggerConsole(options =>
{
	options.IncludeScopes = true;
	options.UseJsonFormatter(formatter =>
	{
		formatter.JsonPropertyNames = JsonPropertyNames.Default with
		{
			Timestamp = JsonEncodedText.Encode("timestamp"),
			MemberName = JsonEncodedText.Encode("membername"),
			Exception = JsonEncodedText.Encode("exception"),
		};
		formatter.JsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
			WriteIndented = true
		};
		formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
		formatter.IncludeProperties = IncludeProperties.All;
	});
});
```

제공자 옵션에서 `IncludeProperties.All`을 활용하여 로그 구성 전체를 출력해보겠습니다.

```csharp
using Microsoft.Extensions.Logging;
using ZLogger;

public class LoggingService
{
    private readonly ILogger<LoggingService> _logger;

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
    }

    public void UserEnter()
    {
        using (_logger.BeginScope(new { GameGuid = Guid.NewGuid() }))
        {
            _logger.ZLogInformation($"User Logged in");
        }
    }

    public void StartGame(string guid)
    {
        using (_logger.BeginScope(new { GameGuid = guid }))
        {
            _logger.ZLogInformation($"Game Started");
        }
    }
}

```

위 코드에서 `UserEnter()` 와 `StartGame()`이 실행 되는 경우,

출력 되는 로그의 예시입니다. (`JSON`)

```json
{
  "timestamp": "2024-11-05T09:23:56.6886296+09:00",
  "LogLevel": "Information",
  "Category": "SampleServer.Services.LoggingService",
  "EventId": 0,
  "EventIdName": null,
  "membername": "UserEnter",
  "FilePath": "C:\\Users\\GSY\\source\\repos\\ZLoggerSolution\\SampleServer\\Services\\LoggingService.cs",
  "LineNumber": 84,
  "Message": "User Logged in ",
  "scope": { "game_guid": "f2abfb13-4795-4f5b-96f7-e4c273c63c33" }
}
```

```json
{
  "timestamp": "2024-11-05T09:23:56.6891689+09:00",
  "LogLevel": "Information",
  "Category": "SampleServer.Services.LoggingService",
  "EventId": 0,
  "EventIdName": null,
  "membername": "StartGame",
  "FilePath": "C:\\Users\\GSY\\source\\repos\\ZLoggerSolution\\SampleServer\\Services\\LoggingService.cs",
  "LineNumber": 89,
  "Message": "Game Started",
  "scope": { "game_guid": "f2abfb13-4795-4f5b-96f7-e4c273c63c33" }
}
```

# Logging Providers

ZLogger 로그 제공자 로그 제공자 선택을 통해 원하는 방식으로 로그를 출력할 수 있습니다.

## Provider Types

| Provider Alias        | Description                                                                                                                                                                                                                                                                                                                                          |
| :-------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `ZLoggerConsole`      | 로그를 콘솔에 출력하도록 합니다.                                                                                                                                                                                                                                                                                                                     |
| `ZLoggerFile`         | 로그를 파일에 기록합니다.                                                                                                                                                                                                                                                                                                                            |
| `ZLoggerRollingFile`  | 특정 간격에 따라 로그파일을 교체(rolling)하여 생성합니다.                                                                                                                                                                                                                                                                                            |
| `ZLoggerStream`       | 로그를 지정 stream으로 출력합니다.                                                                                                                                                                                                                                                                                                                   |
| `ZLoggerInMemory`     | 로그를 메모리에 저장하여 테스트나 지정 프로세스 내 구독 이벤트 처리에 활용할 수 있습니다.                                                                                                                                                                                                                                                            |
| `ZLoggerLogProcessor` | 커스텀 `IAsyncLogProcessor` 활용을 통해 로그 출력을 사용자 정의할 수 있게 합니다. `IZLoggerEntry` 인스턴스를 사용하여 로그를 개별 처리하거나, 로그 일괄 처리를 위한 `BatchingAsyncLogProcessor`를 활용하여 한 번의 HTTP 요청으로 여러 로그를 전송할 수 있습니다. 기본적으로 `IZLoggerEntry`는 풀링되므로 항상 `Return()`을 호출하여 반환해야 합니다. |

ASP .NET Core 에서는 `Add` + **Provider Alias** + `()`형식의 확장 메서드 사용을 통해 손쉽게 원하는 제공자를 구성할 수 있습니다.

```
builder.Logging.AddZLoggerConsole();
```

# Provider Options

옵션은 공통적으로 사용할 수 있는 옵션 `ZLoggerOptions`과, 제공자별 전용 옵션으로 나뉩니다.

## 제공자 옵션 활용 가이드

모든 제공자는 `ZLoggerOptions`를 변경할 수 있는 Action을 제공 받습니다.

예를 들어 본 프로젝트에서는 아래와 같이 `UseJsonFormatter()` 옵션을 사용하여 로그가 JSON 형식으로 출력되도록 합니다.

```csharp
builder.Logging.AddZLoggerConsole(options =>
{
    options.UseJsonFormatter(); // JSON 형식으로 출력
});
```

#### 공통 옵션 `ZLoggerOptions`

\*`ZLoggerInMemoryProvider`는 공통 옵션에서 제외됩니다.

| Option Name                               | Description                                                                                                                                                                                     |
| :---------------------------------------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [`IncludeScopes`](#includescopes)         | `BeginScope` 메서드 활성화 여부를 선택합니다. `BeginScope`를 사용하면 여러 로그 항목에 걸쳐 동일한 컨텍스트 데이터를 포함할 수 있어, 로그 범위 설정과 맥락 파악에 유용합니다. (기본값: `false`) |
| [`TimeProvider`](#timeprovider)           | 로그 출력 시간대 소스를 직접 설정할 수 있습니다. (기본값: `DateTime.UtcNow`)                                                                                                                    |
| `FullMode`                                | 비동기 버퍼가 가득 찼을 때 처리 방식을 설정합니다. `Grow`, `Block`, `Drop` 중 선택 가능하며, 각각 대기, 큐 확장, 또는 초과 항목 삭제의 동작을 정의합니다. (기본값: `Grow`)                      |
| `BackgroundBufferCapacity`                | 비동기 처리에서 사용되는 버퍼의 최대 용량을 설정합니다. FullMode가 `Grow`인 경우 이 옵션은 무시됩니다. (기본값: `10000`)                                                                        |
| `IsFormatLogImmediatelyInStandardLog`     | 로그 포맷을 즉시 적용 여부. 즉시 포맷팅을 선택하면 로그가 기록될 때마다 완전한 포맷으로 즉시 저장되지만, 성능에 부정적인 영향을 줄 수 있습니다. (기본값: `false`)                               |
| `CaptureThreadInfo`                       | 로그에 쓰레드 정보 포함 여부. (기본값: `false`)                                                                                                                                                 |
| `UseFormatter()`                          | 사용자 지정 형식을 정의하여 출력합니다.                                                                                                                                                         |
| `UsePlainTextFormatter()`                 | 기본 텍스트 형식으로 출력합니다.                                                                                                                                                                |
| [`UseJsonFormatter()`](#usejsonformatter) | `System.Text.Json`을 사용하여 JSON 형식으로 출력합니다.                                                                                                                                         |

#### IncludeScopes

```csharp
builder.Logging.AddZLoggerConsole(options =>
{
    options.IncludeScopes = true;
};
```

`IncludeScopes`를 활성화 하면 `BeginScope` 메서드 사용을 통해 특정 범위 내의 모든 로그 항목에 데이터를 추가할 수 있습니다.

예를 들어, Id 값이 해당 범위 내의 각 로그 항목에 포함되도록 작성합니다.

```csharp
using (logger.BeginScope("{Id}", id))
{
    logger.ZLogInformation($"Scoped log {name}");
}
```

출력 예시:

```powershell
# id = 123
# name = "SampleName"
> [Information] Scoped log SampleName {Id=123}
```

스코프는 중첩하여 사용할 수도 있습니다.

```csharp
using (logger.BeginScope("A={A}", 100))
{
    logger.ZLogInformation($"Message 1");
    using (logger.BeginScope("B={B}", 200))
    {
        logger.ZLogInformation($"Message 2");
    }
}
```

출력 예시:

```powershell
# id = 123
# name = "SampleName"
[Information] Message 1 {A=100}
[Information] Message 2 {A=100, B=200}
```

#### TimeProvider

다음은 TimeProvider를 고정 시간으로 설정하는 예제입니다.

```csharp
using Microsoft.Extensions.Logging;
using ZLogger;
using System;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddZLoggerConsole(options =>
    {
        options.TimeProvider = () => DateTime.UtcNow.AddHours(9);       // JST (UTC+9)
        options.UseJsonFormatter((                                      // JSON 형식 사용 (선택 사항)
            formatter.IncludeProperties = IncludeProperties.Timestamp  // Timestamp만 출력되도록 구성
        ));
    });
});

var logger = loggerFactory.CreateLogger<Program>();

logger.ZLogInformation($"Test Log");

```

위 코드에서 TimeProvider는 `DateTime.UtcNow.AddHours(9)`로 설정되어, 일본 표준시(JST, UTC+9)로 로그가 기록되도록 구성됩니다.

출력되는 로그는 다음과 같습니다:

```json
{
  "Timestamp": "2024-11-05T21:34:56.789+09:00"
}
```

#### UseJsonFormatter

`ZLoggerOptions`의 `UseJsonFormatter()` 옵션을 사용하면 로그를 `JSON` 형식으로 출력할 수 있습니다.

이 옵션을 활성화하면 로그 데이터를 구조화된 JSON 형식으로 저장하여 로그 파싱 및 분석에 유용합니다.

```csharp
using Microsoft.Extensions.Logging;
using ZLogger;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddZLoggerConsole(options =>
    {
		options.UseJsonFormatter(formatter =>
		{
			formatter.JsonPropertyNames = JsonPropertyNames.Default with
			{
				Timestamp = JsonEncodedText.Encode("timestamp"),
				MemberName = JsonEncodedText.Encode("membername"),
				Exception = JsonEncodedText.Encode("exception"),
			};

			formatter.JsonSerializerOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
			};

			formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
			formatter.IncludeProperties =
			IncludeProperties.Timestamp |
			IncludeProperties.ParameterKeyValues |
			IncludeProperties.MemberName |
			IncludeProperties.Message |
			IncludeProperties.Exception;
		});
    });
});

var logger = loggerFactory.CreateLogger<Program>();

int userId = 123;
string userName = "Alice";
string action = "UserLoggedIn";
var User = new { UserId = userId, UserName = userName, Action = action };

// 키-값 파라미터를 포함하여 로그 작성
logger.ZLogInformation($"{User}");
```

아래는 해당 설정에서 출력되는 결과입니다.

```json
{
  "timestamp": "2024-11-05T09:13:29.0409654+09:00",
  "membername": "Log",
  "Message": "{ UserId = 123, UserName = Alice, Action = UserLoggedIn }",
  "user": { "user_id": 123, "user_name": "Alice", "action": "UserLoggedIn" }
}
```

## ZLoggerConsole

#### 콘솔 출력 전용 옵션 `ZLoggerConsoleOptions`

콘솔 출력(`ZLoggerConsole`) 전용 옵션입니다.

| Provider Name                   | Description                                                                                                                                                           |
| :------------------------------ | :-------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `OutputEncodingToUtf8`          | `Console.OutputEncoding = new UTF8Encoding(false)` 설정으로 provider를 생성하여, BOM(Byte Order Mark)이 없는 UTF-8 형식으로 로그를 출력합니다. (기본값: `true`)       |
| `ConfigureEnableAnsiEscapeCode` | 콘솔에서 가상 터미널 처리를 구성하여 ANSI 코드를 사용할 수 있게합니다. ANSI 코드를 활성화하면 텍스트 색상, 굵기등의 특수 텍스트 형식을 지원합니다. (기본값: `false​`) |
| `LogToStandardErrorThreshold`   | 설정된 로그 레벨 이상일 경우 표준 오류 출력(`stderr`)으로 로그를 보냅니다. (기본값: `LogLevel.None`)                                                                  |

## JSON

#### JSON 형식 출력 세부 설정 `SystemTextJsonZLoggerFormatter`

앞서 언급한 `UseJsonFormatter()`의 세부 설정을 통해,

필요한 로그가 원하는 형식으로 출력되도록 합니다.

```csharp
builder.Logging.AddZLoggerConsole(options =>
{
	options.UseJsonFormatter(formatter =>
	{
		// 로그 기본 키네임 변경
		formatter.JsonPropertyNames = JsonPropertyNames.Default with
		{
			Timestamp = JsonEncodedText.Encode("timestamp"),
			MemberName = JsonEncodedText.Encode("membername"),
			Exception = JsonEncodedText.Encode("exception"),
		};
		// System.Text.Json 제공 옵션 변경 (JSON 직렬화시 적용되는 설정)
		formatter.JsonSerializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
		};
		// 로그 기본 키네임 컨벤션 변경
		formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
		// 로그 구성 선택
		formatter.IncludeProperties = IncludeProperties.Timestamp | IncludeProperties.ParameterKeyValues;
	});
});
```

설정 가능한 항목은 아래와 같습니다.

| Provider Name                             | Description                                                                          |
| :---------------------------------------- | :----------------------------------------------------------------------------------- |
| [`JsonPropertyNames`](#jsonpropertynames) | ([로그 기본 구성](#로그의-기본-구성)만 해당) 각 속성의 키 이름을 설정할 수 있습니다. |
| [`IncludeProperties`](#includeproperties) | 수집할 속성을 추가하거나 제한합니다.                                                 |
| `JsonSerializerOptions`                   | `System.Text.Json`의 JSON 직렬화 옵션을 설정할 수 있습니다.                          |
| `AdditionalFormatter`                     | 로그 기본 구성외의 추가적인 데이터의 포맷팅을 설정할 수 있습니다.                    |
| `PropertyKeyValuesObjectName`             | 전달되는 `ParameterKeyValues` 를 지정된 이름 아래에 중첩하여 출력하도록 합니다.      |
| [`KeyNameMutator`](#keynamemutator)       | 각 속성의 키 네이밍 컨벤션을 지정합니다.                                             |
| `UseUtcTimestamp`                         | 타임스탬프를 UTC 형식으로 출력합니다. (기본값: `false`)                              |

이전 단계에서 서버에 ZLogger 적용을 완료 하였으면, 아래와 같이 `ZLogInformation` 메서드를 사용할 수 있습니다.

```csharp
var user = new User(1, "Alice");
logger.ZLogInformation($"Name: {user.Name}");
```

### KeyNameMutator

위의 예시를 별도의 설정없이 `JSON` 형식으로만 출력할 경우, 위 코드는 아래와 같이 출력됩니다.

```json
{ "user.Name": "Alice" }
```

기본설정으로 `JSON` 형식 출력 시, 입력 당시의 객체의 키값을 그대로 사용하기 때문에

`KeyNameMutator`옵션 사용을 통해 기본 네이밍 규칙을 변경할 수 있습니다.

예를 들어, `KeyNameMutator`을 `LastMemberName`으로 설정하면 해당 값의 키로 마지막 멤버 이름만을 가져오게 합니다.

```json
{ "Name": "Alice" }
```

여기서 `LastMemberNameLowerFirstCharacter` 옵션을 통해 소문자 형식을 추가로 지정할 수 있습니다.

```csharp
formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
```

해당 옵션을 적용할경우 동일한 로그가 아래와 같이 출력됩니다.

```json
{ "name": "Alice" }
```

### JsonPropertyNames

`JsonPropertyNames` 는 로그 속성 키네임 전체를 대체할 때 사용됩니다.

네이밍 규칙 변경만으로 원하는 키값을 출력하기 어려울때 사용하면 됩니다.

예를 들어, 아래와 같이 `TimeStamp` 와 `MemberName`을 변경합니다.

```csharp
formatter.JsonPropertyNames = JsonPropertyNames.Default with
{
    Timestamp = JsonEncodedText.Encode("timestamp"),
    MemberName = JsonEncodedText.Encode("membername"),
};
```

이렇게 설정한 속성들의 키 값은 아래와 같이 출력됩니다.

```json
{
  "timestamp": "2024-10-02T08:29:50.7544882+00:00",
  "membername": "ActionLog"
}
```

### IncludeProperties

`IncludeProperties`는 [로그 기본 구성](#로그의-기본-구성)키 값으로 이루어진 `enum`입니다.

> ZLogger.IncludeProperties

```csharp
[Flags]
public enum IncludeProperties
{
    None = 0,
    Timestamp = 1,
    LogLevel = 2,
    CategoryName = 4,
    EventIdValue = 8,
    EventIdName = 0x10,
    Message = 0x20,
    Exception = 0x40,
    ScopeKeyValues = 0x80,
    ParameterKeyValues = 0x100,
    MemberName = 0x200,
    FilePath = 0x400,
    LineNumber = 0x800,
    Default = 0x1E7,
    All = 0xFFF
}
```

옵션 값 지정을 통해 원하는 로그의 구성을 선택할 수 있습니다.

예를들어 이렇게 설정할 경우,

```csharp
formatter.IncludeProperties = IncludeProperties.Timestamp;
```

아래와 같이 로그는 `Timestamp` 속성만 출력되게 됩니다.

```json
{
  "Timestamp": "2024-10-02T08:29:50.7544882+00:00"
}
```

## 로그별 형식 사용자 지정하기

로그별로 메서드 호출시에 syntax 활용을 통해 로그 형식을 변경할 수 있습니다.

### Types of syntax

#### 사용자 정의 형식 `:`

`:` 구문은 변수에 사용자 정의 형식을 적용하여 로그 메시지에 표시할 때 사용됩니다.

예를 들어 아래 `ActionLog` 메서드에서 사용되는 context는 object 타입이지만, 아래처럼 `:json`을 활용해 객체를 JSON으로 직렬화합니다.

```csharp
protected void ActionLog(object context, [CallerMemberName] string? tag = null)
{
    _logger.ZLogInformation($"[{tag:json}] {context:json}");
}
```

#### 명시적 이름 변경 `:@`

`:@` 구문은 구조화된 데이터로 로깅할 때 변수의 이름을 명시적으로 변경할 수 있습니다.

기본적으로 ZLogger는 변수 이름을 구조화된 로그의 속성 키로 사용하지만, `:@newname`을 사용하면 해당 변수의 key name을 로그 출력에서 `newname`으로 지정할 수 있습니다.

#### 사용자 정의 형식과 명시적 이름 변경 조합

`:@`(명시적 이름 변경)과 `:`(사용자 정의 형식)을 함께 사용하여 변수의 이름을 지정하고, 동시에 형식을 적용할 수도 있습니다.

```csharp
logger.ZLogDebug($"Today is {DateTime.Now:@date:yyyy-MM-dd}.");
```

위 예시에서는 property 이름을 `date`로 변경하고, 날짜 형식을 `yyyy-MM-dd`로 지정합니다

# Zlogger를 활용한 로그 구조화

통일화된 로그 형태를 위해 ZLogger를 사용하는 서버의 메서드들을 추상화 합니다.

### ActionLog on Controllers

```csharp
public abstract class BaseController<T> : ControllerBase
{
    private readonly ILogger<T> _logger;

    ...

    protected void ActionLog(object context, [CallerMemberName] string? tag = null)
    {
        _logger.ZLogInformation($"[{tag:json}] {context:json}");
    }
}
```

유저 기반 이벤트가 발생할때에는 컨트롤러에서 `ActionLog`를 호출하여

`[CallerMemberName]`을 통해 실행된 메서드 이름과 함께 로그 상세정보(`context`)를 기록합니다.

```csharp

[Route("[controller]")]
[ApiController]
public class LoginController : BaseController<LoginController>
{
    ...

    [HttpPost]
    public async Task<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var response = new LoginResponse();
        (response.Result, var (uid, token)) = await _service.LoginUser(request.PlayerId, request.Token);

        if (response.Result == ErrorCode.None)
        {

            ActionLog(new
            {
                uid
            });
        }

        ...
    }
}
```

예를 들어, 로그인에 성공했을 경우, 위와 같이 ActionLog가 실행됩니다.

해당 메서드는 아래와 같이 콘솔에 출력합니다.

```json
{
  "timestamp": "2024-10-02T08:29:50.7544882+00:00",
  "membername": "ActionLog",
  "tag": "Login",
  "context": {
    "uid": 1
  }
}
```

### Metric Log in Services

Metric Log는 유저 액션 기반이 아닌 시스템 이벤트 발생 시점으로 부터 기록합니다.

```csharp
public abstract class BaseLogger<T>
{
    private readonly ILogger<T> _logger;

    ...

    protected void MetricLog(string tag, object context)
    {
        _logger.ZLogInformation($"[{tag:json}] {context:json}");
    }
}
```

매칭 성사 통계의 경우, MatchServer 에서 매칭 데이터를 생성하고 저장하는 기점을 매칭 성사로 간주하고 MetricLog를 호출합니다

```csharp
// MatchWorker.cs
    private void MonitorMatchQueue()
    {
        while (true)
        {
            if (_userQueue.Count < 2)
            {
                System.Threading.Thread.Sleep(100);
            }

            // Queue 확인 후 저장 .. 생략

            MetricLog("Match", new
            {
                guid = gameGuid
            });
        }
    }
```

해당 메서드 실행을 통해 아래와 같은 로그가 출력됩니다.

```json
{
  "timestamp": "2024-10-02T08:29:50.7544882+00:00",
  "membername": "MetricLog",
  "tag": "Match",
  "context": {
    "guid": "..."
  }
}
```

### 출력용 메서드

디버깅 목적으로 출력되는 로그 메서드 입니다.

공통 항목

| Parameter    | Description                                                                                             |
| :----------- | :------------------------------------------------------------------------------------------------------ |
| `caller`     | 로깅 메서드를 호출한 함수 이름입니다. 자동으로 메서드명이 기록됩니다.                                   |
| `context`    | 로그와 관련된 추가적인 객체나 데이터를 포함할 수 있습니다.                                              |
| `membername` | ZLogger 메서드를 호출한 함수 이름입니다. 로깅 메서드 종류를 구분합니다. 자동으로 메서드명이 기록됩니다. |

#### InformationLog

```csharp
protected void InformationLog(string message, object? context = default, [CallerMemberName] string? caller =null)
{
	_logger.ZLogInformation($"[{caller}] {message} {context:json}");
}
```

정상적인 흐름이나 특정 동작이 수행되었음을 알리고자 할때 사용됩니다.

이전에 로그인 성공시 호출되던 ActionLog 와 함께 Information Log를 호출하여 보겠습니다.

```csharp

	public async Task<LoginResponse> Login([FromBody] LoginRequest request)
	{
        // .. 로그인.. 생략

		if (response.Result == ErrorCode.None && token != null)
		{
			response.AccessToken = token;
			response.Uid = uid;

            // context: 유저 UID
            ActionLog(new
			{
				uid
			});

            // context: 처리 결과 전체, 디버깅용
			InformationLog("User Logged in", response);
		}

    }
```

RESULT:

```json
{
  "timestamp": "2024-10-10T07:46:08.9486195+00:00",
  "membername": "InformationLog",
  "caller": "Login",
  "message": "User Logged in",
  "context": {
    "uid": 1,
    "access_token": "...",
    "result": 0
  }
}
```

`InformationLog`에 첨부된 `response`를 `context`에서 확인할 수 있습니다.

#### ExceptionLog

```csharp
protected void ExceptionLog(Exception ex, object? context = default, [CallerMemberName] string? caller = null)
{
	_logger.ZLogError(ex, $"[{caller}]", context);
}
```

시스템 오류 예외 처리 과정에서 사용됩니다.

Exception 객체(스택 추적 포함)를 통해 문제가 발생한 위치를 파악하고 진단하는 데 도움을 줍니다.

다음은 MemoryDb에서 Redis CloudStructures를 활용하여 RedisConnection으로부터 값을 가져오는 `GetAsync` 메서드입니다.

```csharp
public async Task<(ErrorCode, T?)> GetAsync<T>(string key)
{
    try
    {
        RedisString<T> redisData = new(_redisConnection, key, null);
        RedisResult<T> result = await redisData.GetAsync();
		return (ErrorCode.None, result.Value);
	}
    catch (Exception e)
    {
        ExceptionLog(e, $"{typeof(T).Name}:{key}");
        return (ErrorCode.RedisGetException, default(T));
    }
}
```

MemoryDb에서 존재하지 않는 값을 조회하려 할 때 발생하는 `InvalidOperationException`에 대한 출력 예시입니다.

```json
{
  "timestamp": "2024-10-10T08:15:24.8677434+00:00",
  "exception": {
    "Name": "System.InvalidOperationException",
    "Message": "has no value.",
    "StackTrace": "   at CloudStructures.RedisResult\u00601.get_Value()\n   at ServerShared.Repository.MemoryDb.GetAsync[T](String key)",
    "InnerException": null
  },
  "membername": "ExceptionLog",
  "caller": "GetAsync",
  "context": "RedisUserSession:US_1"
}
```

RedisKeyValue `US_1`를 이용하여 `RedisUserSession`을 조회하려 했을 때 발생한 것을 확인할 수 있습니다.

#### ErrorLog

```csharp
protected void ErrorLog(ErrorCode errorCode, object? context = default, [CallerMemberName] string? caller =null)
{
	_logger.ZLogError($"[{caller}] {errorCode}", context);
}
```

서비스 오류 처리 과정에서 사용됩니다.

오류가 발생 했을때 `ErrorCode`(오류 유형)과 관련 정보를 통해 디버깅을 돕습니다.

로그인 요청시 발생하는 오류를 예시로 살펴보겠습니다.

```csharp

	public async Task<LoginResponse> Login([FromBody] LoginRequest request)
	{
		var response = new LoginResponse();

		(response.Result, var (uid, token)) = await _service.LoginUser(request.PlayerId, request.Token);

		if (response.Result == ErrorCode.None && token != null)
		{
			response.AccessToken = token;
			response.Uid = uid;

            ActionLog(new
			{
				uid
			});

			InformationLog("User Logged in", response);
		}
        else
		{
			ErrorLog(ErrorCode.LoginFail, request);
		}

    }
```

이전에 명시되었던 로그인 요청 함수에, 실패시 `ErrorLog`를 남기는 부분이 추가되었습니다.

오목게임 `UserService`의 `LoginUser`함수는 `VerifyUser`를 통해 아래와 같이 유저정보를 불러오거나 생성합니다.

```csharp
private async Task<(ErrorCode, User?)> VerifyUser(Int64 playerId)
{
	try
	{
		var (errorCode, user) = await _gameDb.Get(playerId);
		if (errorCode == ErrorCode.DbUserGetFailUserNotFound)
		{
			errorCode = await _gameDb.Set(new User { HivePlayerId = playerId });
			if (errorCode != ErrorCode.None)
			{
				return (errorCode, null);
			}
			(errorCode, user) = await _gameDb.Get(playerId);
		}
        // ...
	}
   // ...생략
}
```

`VerifyUser`가 실패할 경우, `LoginUser`는 전달받은 오류코드와 함께 `ErrorLog`를 남깁니다.

```csharp
public async Task<(ErrorCode, (Int64, string))> LoginUser(Int64 playerId, string token)
{
	try
	{
        // ...
		(errorCode, var user) = await VerifyUser(playerId);
		if (errorCode != ErrorCode.None)
		{
			ErrorLog(errorCode);
			return (errorCode, (0, string.Empty));
		}
        // ..
	}
    // ...생략
}
```

해당 과정은 다음과 같이 출력됩니다.

중복된 유저 정보 생성 시도시 `_gameDb.Set`내부에서 호출 되는 `ExceptionLog` 출력물입니다.

```json
{
  "timestamp": "2024-10-10T08:21:58.0954021+00:00",
  "exception": {
    "Name": "MySqlConnector.MySqlException",
    "Message": "Duplicate entry \u00273\u0027 for key \u0027user.hive_player_id\u0027",
    "StackTrace": "...  at GameServer.Repositories.UserDb.Set(User user) in /src/GameServer/Repositories/UserDb.cs:line 44",
    "InnerException": null
  },
  "membername": "ExceptionLog",
  "caller": "Set",
  "context": null
}
```

중복된 `hive_player_id`로 인해 `MySqlException` 이 발생한 것을 확인할 수 있습니다.

이후 `LoginUser`는 아래와 같은 `ErrorLog`를 남깁니다.

```json
{
  "timestamp": "2024-10-10T08:21:58.0954751+00:00",
  "membername": "ErrorLog",
  "caller": "LoginUser",
  "errorCode": 10003,
  "context": null
}
```

```csharp
public enum ErrorCode
{
	DbUserInsertException = 10003
}
```

DB 예외처리가 발생하였을 알려주는 에러코드와 함께 호출 위치(`caller`)를 알려주고 있습니다.

이렇게 로그인 요청에 실패하면, `LoginUser`를 요청했던 `LoginController` 에서 요청 실패에 대한 정보를 다음과 같이 남깁니다.

```json
{
  "timestamp": "2024-10-10T08:21:58.0954838+00:00",
  "membername": "ErrorLog",
  "caller": "Login",
  "errorCode": 1300,
  "context": {
    "player_id": 3,
    "token": "..."
  }
}
```

```csharp
public enum ErrorCode
{
	LoginFail = 1300,
}
```

실패한 요청 정보와 함께 요청 실패 오류코드가 출력된것을 확인할 수 있습니다.
