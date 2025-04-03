# Serilog 사용 가이드

Serilog는 .NET 애플리케이션에서 구조화된 로깅을 지원하는 강력한 라이브러리입니다.
다양한 기능을 통해 개발자는 로그를 효율적으로 관리하고 분석할 수 있습니다.
본 저장소는 Serilog의 주요 기능과 각 기능의 사용 예시를 코드와 함께 설명합니다.

## 목차

- [Serilog 소개](#serilog-소개)
- [기본 활용법](#기본-활용법)
  - [설치](#설치)
  - [구성](#구성)
    - [기본 구성](#1-기본적인-설정과-콘솔-로깅)
      - [설정 파일 구성](#2-appsettingsjson-파일-기반-구성)
      - [싱크(Sink)추가 및 동시 출력](#3-로그-동시-출력-및-sink-추가)
    - [출력 구성](#출력-구성)
      - [Output Template으로 포맷팅 설정](#1-output-template을-통한-포맷팅-설정)
      - [JSON 형식으로 출력하기](#2-json형식으로-출력하기)
        - [Serilog.Formatting.Json.JsonFormatter (기본 제공)](#serilogformattingjsonjsonformatter)
        - [Serilog.Formatting.Compact.CompactJsonFormatter (압축형)](#serilogformattingcompactcompactjsonformatter)
      - [Enrich로 추가 정보 설정](#3-enrich를-활용한-추가-정보-설정)
      - [Filter로 조건부 로깅](#4-filter를-이용한-조건부-로깅)
    - [Sink 구성](#sink-구성)
      - [파일 싱크 (Serilog.Sinks.File)](#serilogsinksfile)
      - [콘솔 싱크 (Serilog.Sinks.Console)](#serilogsinksconsole)
      - [디버그 싱크 (Serilog.Sinks.Debug)](#serilogsinksdebug)
      - [비동기 싱크 (Serilog.Sinks.Async)](#serilogsinksasync)
      - [Elasticsearch 싱크 (Serilog.Sinks.Elasticsearch)](#serilogsinkselasticsearch)
      - [HTTP 싱크 (Serilog.Sinks.Http)](#serilogsinkshttp)
      - [SQL Server 싱크 (Serilog.Sinks.MSSqlServer)](#serilogsinksmssqlserver)
      - [SQLite 싱크 (Serilog.Sinks.SQLite)](#serilogsinkssqlite)
      - [MongoDB 싱크 (Serilog.Sinks.MongoDB)](#serilogsinksmongodb)
- [로그 구조화](#로그-구조화)
  - [기본 직렬화 방식](#기본-동작)
  - [콜렉션 처리](#콜렉션-처리)
  - [객체 처리](#객체-처리)
  - [메시지 템플릿 (Message Template)](#message-template)
- [활용 예제]()
  - [ASP .NET Core 9 Web API Server](#asp-net-core-9-web-api-server)
  - [.NET 9 Socket Server using SuperSocket](#net-9-socket-server-using-supersocket)

## Serilog 소개

[Serilog](https://serilog.net/)는 .NET 플랫폼을 위한 구조화된 로깅 라이브러리로, 로그 데이터를 구조화된 형식으로 기록하여 효율적인 검색과 분석을 가능하게 합니다.

다양한 싱크([Sink](https://github.com/serilog/serilog/wiki/Provided-Sinks)) 통해 콘솔, 파일, 데이터베이스 등 여러 출력 대상으로 로그를 전송할 수 있습니다.

# 기본 활용법

## 설치

Serilog를 사용하려면 NuGet 패키지를 통해 필요한 라이브러리를 설치해야 합니다.

```
Using Serilog;
```

기본적으로 Serilog와 원하는 싱크 패키지를 설치합니다.
각 싱크 패키지의 자세한 정보는 [Sink 구성](#sink-구성) 항목에서 확인 하세요.

## 구성

Serilog의 로거는 `LoggerConfiguration` 클래스를 사용하여 구성한뒤

`CreateLogger()` 실행을 통해 로거를 생성합니다.

다음과 같이 최소 로그 레벨, 출력 형식, 싱크 등을 설정할 수 있습니다.

### 1. 기본적인 설정과 콘솔 로깅:

```
$ dotnet add package Serilog.Sinks.Console
```

콘솔에 `Debug` 수준의 로그를 출력하기 위에서 위 Sink를 설치합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Some log message");

Log.CloseAndFlush();
```

Logger는 애플리케이션 초기화 시 한 번만 생성하며,

`CloseAndFlush()`는 애플리케이션 종료 시 호출합니다.

#### Sink 별 로그 레벨 설정

특정 Sink에 대해서만 더 높은 수준의 로그만 출력하도록 제한하고 싶은 경우,

`restrictedToMinimumLevel` 파라미터를 사용하여 개별 Sink마다 최소 로그 레벨을 별도로 설정할 수 있습니다.

예를 들어 아래와 같이 구성할 수 있습니다:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("log.txt")
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();
```

이 구성에서는:

- `Debug`, `Information`, `Warning` 등 모든 로그가 `log.txt` 파일에 기록됩니다.
- `Information` 이상(`Warning`, `Error`, `Fatal`)만 콘솔에 출력됩니다.

> 💡 **Logger vs. Sink 레벨의 차이점**  
> 전체 Logger의 `MinimumLevel`은 로그 이벤트가 생성될지 여부를 결정하며,
> Sink의 `restrictedToMinimumLevel`은 이미 생성된 이벤트 중 어떤 것을 해당 Sink에 출력할지를 결정합니다.  
> 따라서 Logger 수준보다 낮은 레벨을 Sink에 지정해도 출력되지 않습니다.

#### 로거 등록하기

```
$ dotnet add package Serilog.AspNetCore
```

.NET 6+부터는 다음과 같이 `Serilog.AspNetCore`패키지 또는 `Serilog.Extensions.Hosting`패키지를 활용하여

Program.cs에서 Serilog를 직접 Host에 연결해 사용하는 방식이 일반적입니다.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
	loggerConfiguration
		.ReadFrom.Configuration(hostingContext.Configuration)
		.Enrich.FromLogContext()
		.WriteTo.Console();
});
```

(Serilog를 애플리케이션 전체의 기본 로깅 시스템으로 완전히 대체합니다)

또는

```csharp
builder.Logging.AddSerilog(
	new LoggerConfiguration()
		.ReadFrom.Configuration(configuration)
		.CreateLogger()
	);
```

(Serilog를 로그 제공자로 등록합니다.)

### 2. `appsettings.json` 파일 기반 구성

```
$ dotnet add package Serilog.Settings.Configuration
```

위는 `appsettings.json` 파일 설정을 지원하는 패키지 입니다.

구성을 코드에 하드코딩하지 않고 `JSON` 설정으로 관리할 수 있습니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(_config)
    .CreateLogger();
```

.NET DI 기반의 애플리케이션에 가장 적합한 형태로,

다양한 환경에 따라 다른 로깅 설정을 구별 할 수 있는 장점이 있습니다.

**appsettings.json 구성 예**:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
    "Filter": [
      {
        "Name": "ByExcluding",
        "Args": {
          "expression": "RequestPath like '/ping'"
        }
      }
    ]
  }
}
```

#### 구성 항목

| Configuration Key               | 설명                                                                                                            |
| :------------------------------ | :-------------------------------------------------------------------------------------------------------------- |
| [MinimumLevel](#minimumlevel)   | 전체 로깅 수준과 네임스페이스별 최소 로그 레벨을 설정합니다. `Default`와 `Override`를 지원합니다.               |
| [WriteTo](#writeto)             | 로그를 기록할 하나 이상의 싱크(sink)를 정의합니다. `Name`과 `Args`를 통해 파라미터 설정이 가능합니다.           |
| [Enrich](#enrich)               | 로그 이벤트에 추가적인 정보를 붙이기 위한 enricher를 지정합니다. 예: `FromLogContext`, `WithMachineName` 등     |
| [Destructure](#destructure)     | 복잡한 객체를 로그로 출력하기 위해 사용자 정의 구조 해석 규칙(destructuring policy)을 지정합니다.               |
| [Filter](#filter)               | 특정 조건의 로그를 포함하거나 제외하는 필터를 지정합니다. `ByIncludingOnly`, `ByExcluding` 등을 사용합니다.     |
| Using                           | 설정에서 사용되는 sink, enricher, 기타 구성 요소가 포함된 어셈블리를 지정합니다.                                |
| [AuditTo](#auditto)             | 중요한 감사(audit) 로그를 기록할 sink를 정의합니다. `WriteTo`와 유사하나 로그 손실이 없어야 할 경우 사용합니다. |
| [Properties](#properties)       | 모든 로그 이벤트에 자동으로 포함될 글로벌 속성(key-value 쌍)을 설정합니다.                                      |
| [LevelSwitches](#levelswitches) | 런타임에 동적으로 제어 가능한 로그 수준을 선언합니다. 다른 설정에서 참조할 수 있습니다.                         |
| [Theme](#theme)                 | 콘솔 출력 시 사용할 테마를 지정합니다 (예: `"Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code"`).      |
| [Extensions](#extensions)       | 사용자 정의 확장 기능을 구성할 수 있는 키입니다 (드물게 사용됨).                                                |

##### :MinimumLevel

제공되는 로그 레벨

| Log Level     | 설명                                                                                                                    |
| :------------ | :---------------------------------------------------------------------------------------------------------------------- |
| `Verbose`     | 가장 많은 정보를 담는 수준으로, 운영 환경에서는 거의 (또는 전혀) 활성화되지 않습니다.                                   |
| `Debug`       | 외부에서는 반드시 관찰되지 않지만, 어떤 일이 발생했는지 파악하는 데 유용한 내부 시스템 이벤트에 사용됩니다.             |
| `Information` | 시스템의 책임과 기능에 해당하는 동작을 설명하는 이벤트입니다. 일반적으로 시스템이 수행할 수 있는 관찰 가능한 동작입니다 |
| `Warning`     | 서비스가 저하되었거나 예상된 결과를 벗어난 경우에 사용됩니다.                                                           |
| `Error`       | 기능을 사용할 수 없거나 기대한 동작이 깨진 경우에 사용됩니다.                                                           |
| `Fatal`       | 시스템 전체에 영향을 줄 수 있는 치명적인 오류를 나타냅니다. 즉각적인 대응이 요구됩니다.                                 |

##### :WriteTo

`WriteTo` 키에는 로그를 출력할 Sink들을 배열로 정의합니다.

```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "File",
      "Args": {
        "path": "Logs/log-.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

각 Sink의 세부 설정은 [Sink 구성](#sink-구성)에서 확인할 수 있습니다.

##### :Enrich

`Enrich`는 로그 이벤트에 머신 이름, 스레드 ID 등 추가 정보를 포함시키기 위한 설정입니다.

```json
"Serilog": {
  "Enrich": [
    "FromLogContext",
    "WithMachineName",
    "WithThreadId",
    {
      "Name": "WithProperty",
      "Args": {
        "name": "Application",
        "value": "MyApp"
      }
    }
  ]
}
```

지원하는 enricher 목록은 [추가 정보 설정](#2-enrich를-활용한-추가-정보-설정)에서 확인할 수 있습니다.

##### :Destructure

`Destructure`는 복잡한 객체를 로깅할 때 커스터마이징할 수 있는 정책을 설정합니다.

사용자 정의 구조 해석(destructuring) 정책을 통해 로그 표현을 제어합니다.

```json
"Serilog": {
  "Destructure": [
    {
      "Name": "With",
      "Args": {
        "policy": "MyNamespace.CustomPolicy, MyAssembly"
      }
    }
  ]
}
```

##### :Filter

`Filter`는 로그를 조건에 따라 포함 또는 제외할 수 있게 합니다.

```json
"Serilog": {
  "Filter": [
    {
      "Name": "ByExcluding",
      "Args": {
        "expression": "RequestPath like '/health%'"
      }
    }
  ]
}
```

위 설정은 `/health` 경로와 일치하는 로그를 필터링합니다.

Filter 의 추가적인 구성은 [조건부 로깅 설정](#3-filter를-이용한-조건부-로깅),

Expression 문법에 대한 정보는 [공식 저장소](https://github.com/serilog/serilog-expressions)에서 확인할 수 있습니다.

##### :AuditTo

`AuditTo`는 일반 로그(`WriteTo`)와는 별도로 항상 기록되어야 할 이벤트에 사용됩니다.

```json
"Serilog": {
  "AuditTo": [
    {
      "Name": "File",
      "Args": {
        "path": "Logs/audit-.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

##### :Properties

전역 속성을 설정하여 모든 로그 이벤트에 자동으로 포함시킬 수 있습니다.

```json
"Serilog": {
  "Properties": {
    "Application": "MyApp",
    "Environment": "Production"
  }
}
```

##### :LevelSwitches

`LevelSwitches`는 런타임에 로그 수준을 동적으로 조절할 수 있게 해주는 스위치를 선언합니다.

다른 항목에서 참조(`$switch`)할 수 있습니다.

```json
"Serilog": {
  "LevelSwitches": {
    "$controlSwitch": "Information"
  },
  "MinimumLevel": {
    "ControlledBy": "$controlSwitch"
  }
}
```

##### :Theme

콘솔 출력에 사용할 테마를 지정합니다. 정적 속성 형식으로 입력되어야 하며, `Serilog.Sinks.Console`에서 제공됩니다.

```json
"Serilog": {
  "WriteTo": [
    {
      "Name": "Console",
      "Args": {
        "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code"
      }
    }
  ]
}
```

##### :Extensions

사용자 정의 확장을 구성할 수 있습니다.

```json
"Serilog": {
  "Extensions": [
    {
      "Name": "UseMyCustomLogger",
      "Args": {
        "setting": "value"
      }
    }
  ]
}
```

### 3. 로그 동시 출력 및 Sink 추가

```
$ dotnet add package Serilog.Sinks.File
```

출력된 로그를 파일 형태로 저장하기 위해서 `Serilog.Sinks.File` 패키지를 설치합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

설치 후, `WriteTo` 항목을 반복하여 여러 Sink를 자유롭게 추가할 수 있습니다.

각 Sink에 대한 자세한 설정 방법은 [Sink 구성](#sink-구성) 문서를 참고하세요.

## 출력 구성

### 1. `Output Template`을 통한 포맷팅 설정

텍스트 기반 sink (콘솔, 파일 등)는 `outputTemplate` 파라미터로 로그 포맷을 제어할 수 있습니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("log.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();
```

| 템플릿 코드      | 설명                                            |
| :--------------- | :---------------------------------------------- |
| `{Timestamp}`    | 로그 시간                                       |
| `{Level:u3}`     | 로그 레벨 (세 글자 대문자: INF, ERR 등)         |
| `{Message:lj}`   | 메시지 (내장 객체는 JSON, 문자열은 그대로 출력) |
| `{Properties:j}` | 컨텍스트 정보 (Enricher로 추가된 값들 포함)     |
| `{Exception}`    | 예외 스택 출력 (있는 경우)                      |

### 2. `JSON`형식으로 출력하기

텍스트 기반 sink 는 기본적으로 고정된 텍스트 형식으로 로그를 기록합니다.

로그를 JSON 형식으로 기록하려면 `outputTemplate` 대신 ITextFormatter를 첫 번째 인자로 전달해야 합니다.

```csharp
// Serilog.Formatting.Compact 설치 필요
.WriteTo.File(new CompactJsonFormatter(), "log.txt")
```

#### Serilog.Formatting.Json.JsonFormatter

Serilog 기본 패키지에서 제공하는 기본 JSON 포매터입니다.

**appsettings.json 설정 예시:**

```json
{
  "Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.json",
          "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog"
        }
      }
    ]
  }
}
```

로그 이벤트에 Timestamp, Level, MessageTemplate, Properties, Exception 등의 전체 메타데이터가 포함됩니다.

**출력 예시:**

```json
{
  "Timestamp": "2025-04-02T12:34:56.789Z",
  "Level": "Information",
  "MessageTemplate": "Hello {Name}",
  "RenderedMessage": "Hello Alice",
  "Properties": {
    "Name": "Alice"
  }
}
```

#### Serilog.Formatting.Compact.CompactJsonFormatter

`Serilog.Formatting.Compact` 패키지에서 제공하는 포매터로,

로그 파일 크기를 줄이고 로그 수집 도구와의 연동을 최적화하기 위해 설계되었습니다.

**appsettings.json 설정 예시:**

```json
{
  "Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.json",
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  }
}
```

줄 바꿈으로 구분된 JSON (`NDJSON`) 형식이며 매우 압축되어 있습니다.

Seq, Elasticsearch, Datadog 같은 로그 분석 도구와의 연동에 적합합니다.

짧은 속성명을 사용하고 불필요한 필드는 생략합니다.

**출력 예시:**

```json
{
  "@t": "2025-04-02T12:34:56.789Z",
  "@mt": "Hello {Name}",
  "Name": "Alice",
  "@l": "Information"
}
```

### 3. `Enrich`를 활용한 추가 정보 설정

`Enrich` 기능은 로그 메시지에 추가적인 컨텍스트 정보(예: 머신 이름, 스레드 ID, 사용자 정보 등)를 자동으로 포함시켜,

데이터 분석에 적합한 내용으로 가공할 수 있도록 도와줍니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("AppName", "SampleLoggerApp")
    .WriteTo.Console()
    .CreateLogger();
```

위 예제에서는 로그에 다음과 같은 정보가 자동으로 추가됩니다:

- MachineName: 로그가 생성된 머신의 이름
- ThreadId: 로그를 생성한 스레드 ID
- AppName: 사용자 정의 속성

아래는 자주 사용하는 Enricher 목록입니다.

| Enricher 이름                | 설명                                                        |
| :--------------------------- | :---------------------------------------------------------- |
| `.WithMachineName()`         | 현재 머신의 이름을 포함                                     |
| `.WithThreadId()`            | 현재 스레드 ID 포함                                         |
| `.WithProcessId()`           | 프로세스 ID 포함                                            |
| `.WithEnvironmentUserName()` | 실행 중인 OS 계정명 포함                                    |
| `.WithProperty(key, value)`  | 임의의 커스텀 속성 추가                                     |
| `.WithCorrelationId()`       | 분산 트레이싱을 위한 Correlation ID 포함 (추가 패키지 필요) |
| `.FromLogContext()`          | LogContext.PushProperty()에서 설정된 정보 포함              |

> 일부 Enricher는 별도의 NuGet 패키지를 통해 제공됩니다:
>
> - Serilog.Enrichers.Thread
> - Serilog.Enrichers.Process
> - Serilog.Enrichers.Environment

### 4. `Filter`를 이용한 조건부 로깅

`Filter` 기능은 특정 조건에 따라 로그 메시지를 필터링하는 역할을 합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .Filter.ByExcluding(logEvent =>
        logEvent.Level == LogEventLevel.Debug)
    .WriteTo.Console()
    .CreateLogger();

Log.Debug("이 메시지는 필터에 의해 기록되지 않습니다.");
Log.Information("이 메시지는 출력됩니다.");
```

| Filter 이름                 | 설명                                |
| :-------------------------- | :---------------------------------- |
| `.Filter.ByIncludingOnly()` | 조건을 만족하는 로그만 포함         |
| `.Filter.ByExcluding()`     | 조건을 만족하는 로그는 제외         |
| `.Filter.With()`            | 커스텀 필터 구현체를 사용할 수 있음 |

## Sink 구성

Serilog는 다양한 `Sink`를 통해 로그를 여러 출력 대상으로 전송할 수 있습니다.

### Serilog.Sinks.File

```
$ dotnet add package Serilog.Sinks.File
```

로그 이벤트를 로컬 파일에 `JSON` 또는 `TEXT`형식으로 기록합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

#### 날짜/크기 기준 롤링

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Logs/log-.txt",                // 파일 이름에 날짜 형식 포함
        rollingInterval: RollingInterval.Day, // 일 단위로 로그 분리
        retainedFileCountLimit: 7,            // 최근 7일치만 보관
        rollOnFileSizeLimit: true,           // 크기로 분할 활성화
        fileSizeLimitBytes: 10_000_000,      // 10MB
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();
```

**구성 요소 설명:**
| 옵션| 설명 |
| :-------------------------- | :---------------------------------- |
| `path` | 로그 파일 경로이며 - 기호 뒤에 날짜 포맷이 붙음 (log-20250401.txt) |
| `rollingInterval` | `Day`, `Hour`, `Minute`, `Month`, `Infinite` 중 하나로 날짜 단위 분할 |
| `retainedFileCountLimit` | 보관할 최대 파일 수. 초과 시 오래된 파일 자동 삭제 (null은 무제한) |
| `fileSizeLimitBytes` |파일 크기 기준 분할 (바이트 단위). 기본값: 1GB |
| `rollOnFileSizeLimit` | true일 경우 `fileSizeLimitBytes` 초과 시 새 파일 생성 |
| `outputTemplate` | 로그 출력 형식. 로깅 포맷 일관성 유지 가능|

**appsettings.json 구성 예제:**

```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    {
        "Name": "File",
        "Args": {
            "path": "Logs/log-.json",
            "restrictedToMinimumLevel": "Information",
            "rollingInterval": "Day",
            "retainedFileCountLimit": 7,
            "rollOnFileSizeLimit": true,
            "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog",
            "fileSizeLimitBytes": 10000000
        }
    }
  ]
}
```

#### 공유 로그 파일 설정

여러 프로세스에서 동일한 로그 파일에 접근하도록 허용하려면 `shared` 옵션을 `true`로 설정합니다:

**Program.cs 설정 예시:**

```csharp
var logger = new LoggerConfiguration()
	.WriteTo.File("Logs/log-.txt", shared: true)
	.CreateLogger();
```

**appsettings.json 설정 예시:**

```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    {
      "Name": "File",
      "Args": {
        "path": "Logs/log-.txt",
        "shared": true
      }
    }
  ]
}
```

#### FileLifecycleHooks

Serilog.Sinks.File은 `FileLifecycleHooks` 클래스를 통해 로그 파일의 생명주기 이벤트에 대한 훅을 제공합니다.

이를 통해 로그 파일이 열리거나 삭제되기 전에 사용자 정의 로직을 삽입할 수 있습니다.

- `OnFileOpened`: 로그 파일이 열릴 때 호출되며, 스트림에 헤더를 추가하거나 스트림을 래핑하여 버퍼링, 압축, 암호화 등을 적용할 수 있습니다.​
- `OnFileDeleting`: 오래된 롤링 로그 파일이 삭제되기 전에 호출되며, 해당 파일을 다른 위치에 아카이브하는 등의 작업을 수행할 수 있습니다.

**사용 예시:**

> 로그 파일의 시작 부분에 헤더를 추가하는 커스텀 훅 구현

```csharp
public class CustomFileLifecycleHooks : FileLifecycleHooks
{
	public override Stream OnFileOpened(string path, Stream underlyingStream, Encoding encoding)
	{
		// 스트림에 헤더를 작성
		var writer = new StreamWriter(underlyingStream, encoding);
		writer.WriteLine("Hello This is Custom File Message!");
		writer.Flush();

		// 원본 스트림 반환
		return underlyingStream;
	}
}
```

> 설정 적용, Program.cs:

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        hooks: new HeaderWriterHooks() // 커스텀 훅 적용
    )
    .CreateLogger();
```

**출력된 로그 파일:**

```
Hello This is Custom File Message!
2025-04-02 15:54:20.272 +09:00 [INF] Now listening on: http://[::]:8000
2025-04-02 15:54:20.274 +09:00 [DBG] Loaded hosting startup assembly APIServer
```

### Serilog.Sinks.Console

```
$ dotnet add package Serilog.Sinks.Console
```

로그 메시지를 콘솔에 출력합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
```

> 참고: 콘솔 싱크는 개발 환경에서 주로 사용되며, 프로덕션 환경에서는 성능 이슈로 인해 다른 싱크를 사용하는 것이 권장됩니다.

### Serilog.Sinks.Debug

```
$ dotnet add package Serilog.Sinks.Debug
```

로그 이벤트를 디버그 출력 창(예: Visual Studio의 출력 창)에 전송합니다. 디버깅 시 유용합니다

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Debug()
    .CreateLogger();
```

### Serilog.Sinks.Async

비동기용 래퍼(Wrapper)로, 다른 Serilog 싱크(Sink)를 감쌉니다.

이 싱크를 사용하면 로깅 호출의 오버헤드를 줄이고, 작업을 백그라운드 스레드에 위임함으로써 성능을 향상시킬 수 있습니다.

특히 I/O 병목 현상의 영향을 받을 수 있는 File 및 RollingFile과 같은 비배치(Non-batching) 싱크에 적합합니다.

> 참고: CouchDB, Elasticsearch, MongoDB, Seq, Splunk 등의 네트워크 기반 싱크들은 이미 자체적으로 비동기 배치 처리를 지원하므로, 이 Sink를 사용해도 추가적인 이점이 없습니다.

### Serilog.Sinks.Http

```
$ dotnet add package Serilog.Sinks.Http
```

로그 이벤트를 HTTP 프로토콜을 통해 원격 서버로 전송할 수 있도록 하는 싱크입니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Http("http://your-log-server.com")
    .CreateLogger();
```

### Serilog.Sinks.Elasticsearch

```
$ dotnet add package Serilog.Sinks.Elasticsearch
```

로그 이벤트를 Elasticsearch 클러스터에 전송합니다.

```csharp
Log.Logger =  new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
    })
    .CreateLogger();
```

### Serilog.Sinks.MSSqlServer

```
$ dotnet add package Serilog.Sinks.MSSqlServer
```

로그 이벤트를 SQL Server 데이터베이스에 저장합니다.

### Serilog.Sinks.SQLite

```
$ dotnet add package Serilog.Sinks.SQLite
```

이 싱크는 내부적으로 로그를 버퍼링한 후, 전용 스레드를 통해 배치로 SQLite 데이터베이스에 플러시합니다.
이를 통해 성능을 향상시키고 I/O 병목 현상을 최소화합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.SQLite(@"Logs\log.db")
    .CreateLogger();
```

**appsettings.json 설정 예시:**

```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "SQLite",
        "Args": {
          "sqliteDbPath": "Logs/logs.db",
          "tableName": "Logs"
        }
      }
    ]
  }
}
```

위 설정으로 로그 출력시 `Logs/` 경로에 다음과 같이 저장됩니다

**저장된 .db 파일:**

![](Images/serilog-sqlite.png)

### Serilog.Sinks.MongoDB

```
$ dotnet add package Serilog.Sinks.MongoDB
```

로그 이벤트를 MongoDB에 문서 형태로 저장하는 싱크입니다.

MongoDB의 컬렉션에 개별 문서로 삽입됩니다.

```csharp
var logger = new LoggerConfiguration()
    .WriteTo.MongoDB("mongodb://localhost/logs")
    .CreateLogger();
```

TLS 및 인증등의 고급 설정은 다음과 같이 가능합니다:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.MongoDBBson(cfg =>
    {
        var mongoDbSettings = new MongoClientSettings
        {
            UseTls = true,
            AllowInsecureTls = true,
            Credential = MongoCredential.CreateCredential("databaseName", "username", "password"),
            Server = new MongoServerAddress("127.0.0.1")
        };

        var mongoDbInstance = new MongoClient(mongoDbSettings).GetDatabase("serilog");

        cfg.SetMongoDatabase(mongoDbInstance);
        cfg.SetRollingInternal(RollingInterval.Month);
    })
    .CreateLogger();
```

**appsettings.json 설정 예시:**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Error",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "MongoDBBson",
        "Args": {
          "databaseUrl": "mongodb://username:password@ip:port/dbName?authSource=admin",
          "collectionName": "logs",
          "cappedMaxSizeMb": "1024",
          "cappedMaxDocuments": "50000",
          "rollingInterval": "Month"
        }
      }
    ]
  }
}
```

- `databaseUrl`: MongoDB 접속 URL

- `collectionName`: 로그를 저장할 컬렉션 이름

- `cappedMaxSizeMb`: 컬렉션의 최대 크기 (MB) 설정

- `cappedMaxDocuments`: 컬렉션 내 최대 문서 수

(JSON 설정의 키는 대소문자를 구분하지 않습니다.)

**저장된 로그 예시:**

```mongodb
{
  _id: ObjectId('67eba632615965a169662f6d'),
  Level: 'Information',
  UtcTimeStamp: ISODate('2025-04-01T08:39:14.009Z'),
  MessageTemplate: {
    Text: 'chat completion succeeded',
    Tokens: [ { _t: 'TextToken', Text: 'chat completion succeeded' } ]
  },
  RenderedMessage: 'chat completion succeeded',
  Properties: {},
  Exception: { _csharpnull: true },
  TraceId: 'ea82f36d6af746a03bbe67d8386c5a82',
  SpanId: 'b8cee52faa67327c'
}
```

# 로그 구조화

### 기본 동작

Serilog는 로그에 속성이 전달되면 적절한 표현 방식(문자열, 구조화 등)을 자동으로 선택하려고 시도합니다.

```csharp
var count = 456;
Log.Information("Retrieved {Count} records", count);
```

위의 로그는 JSON 형태로 출력시 다음과 같습니다.

```json
{ "Count": 456 }
```

| 기본 스칼라 | 인식되는 데이터 목록                                                |
| :---------- | :------------------------------------------------------------------ |
| `Boolean`   | bool                                                                |
| `Numerics`  | byte, short, ushort, int, uint, long, ulong, float, double, decimal |
| `Strings`   | string, byte[]                                                      |
| `Temporals` | DateTime, DateTimeOffset, TimeSpan                                  |
| `Others`    | Guid, Uri                                                           |
| `Nullables` | 위 데이터 타입 모두                                                 |

### 콜렉션 처리

객체가 `IEnumerable` 형태로 전달될 경우 콜렉션으로 간주합니다.

```csharp
var fruit = new[] { "Apple", "Pear", "Orange" };
Log.Information("In my bowl I have {Fruit}", fruit);
```

이경우 출력되는 JSON 형태는 다음과 같습니다

```json
{ "Fruit": ["Apple", "Pear", "Orange"] }
```

또한, `Dictionary<TKey,TValue>`의 형태 에서 Key의 데이터 타입이 앞서 언급된 데이터 목록 중 하나 일경우,

자동으로 직렬화가 가능합니다.

(단, `IDictionary<TKey,TValue>`등의 인터페이스를 구현한 객체의 경우 모호성 문제때문에 직렬화 되지 않습니다.)

### 객체 처리

#### 복잡한 객체

이외 Serilog가 인식하지 못하는 사용자 정의 타입을 전달하면 기본적으로 ToString()이 호출됩니다.

```csharp
SqlConnection conn = ...;
Log.Information("Connected to {Connection}", conn);
```

위 와같은 경우, 아래와 같이 문자열로 출력됩니다.

```
"System.Data.SqlClient.SqlConnection"
```

#### 객체 구조 보존

객체 내부 속성을 구조화된 형태로 기록하고 싶을 경우, `@` 연산자를 사용합니다:

```csharp
var sensorInput = new { Latitude = 25, Longitude = 134 };
Log.Information("Processing {@SensorInput}", sensorInput);
```

**JSON:**

```json
{ "SensorInput": { "Latitude": 25, "Longitude": 134 } }
```

#### 구조 분해 사용자 지정

특정 속성만 로깅하고 싶은 경우 `Destructure.ByTransforming<T>()`을 사용합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .Destructure.ByTransforming<HttpRequest>(r => new {
        RawUrl = r.RawUrl,
        Method = r.Method
    })
    .WriteTo...
```

\*변환 함수는 반드시 다른 타입을 반환해야 합니다. 그렇지 않으면 재귀 호출되어 예외가 발생할 수 있습니다.

**Destructure 관련 확장 기능들:**

| 설정정                             | 설명                                                       |
| :--------------------------------- | :--------------------------------------------------------- |
| `.Destructure.ByTransforming<T>()` | 특정 타입의 객체를 변형하여 구조화                         |
| `.Destructure.With<Policy>()`      | 커스텀 구조화 정책 적용                                    |
| `.Destructure.JsonNetTypes()`      | Newtonsoft.Json 특성에 따른 구조화 지원 (별도 패키지 필요) |
| `.Destructure.ToMaximumDepth()`    | 깊은 중첩 객체 구조화 시 최대 깊이 제한                    |
| `.Destructure.AsScalar<T>()`       | 특정 타입을 단일 값으로 처리하도록 지정                    |

#### JSON.NET 연동

복잡한 JSON 직렬화 로직이 필요한 경우, Serilog는 JSON.NET과의 연동도 지원합니다.

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithExceptionDetails()
    .Destructure.JsonNetTypes()
    .WriteTo.Console()
    .CreateLogger();
```

이 설정을 통해 [JsonIgnore], [JsonProperty] 등의 속성을 활용한 구조화 로깅이 가능해집니다.

#### 문자열화

객체 타입이 불확실하거나 ToString 결과만 기록하고 싶을 경우 $ 연산자를 사용합니다:

```csharp
var unknown = new[] { 1, 2, 3 };
Log.Information("Received {$Data}", unknown);
```

**출력 결과:**

```
"System.Int32[]"
```

### Message Template

메시지 템플릿(`Message Template`)은 .NET의 string.Format()에서 사용하는 형식 문자열을 포함하는 상위 개념으로,

`string.Format()`에서 유효한 모든 포맷 문자열은 Serilog에서도 정상적으로 처리됩니다.

메시지 템플릿을 사용하여 다음과 같이 로그 메시지에 변수를 포함할 수 있습니다.

```csharp
var userName = "shana";
var items = 3;
var totalPrice = 99.99;

Log.Information("{UserName}님이 {Items}개의 아이템을 총 {TotalPrice}원에 구매했습니다.", userName, items, totalPrice);
```

위 로그는 아래와 같이 출력됩니다

**Result:**

```
"shana"님이 3개의 아이템을 총 99.99원에 구매했습니다.
```

Serilog는 데이터 타입을 명확하게 구분하기 위해 로그 메시지에서 문자열(`string`) 값을 큰따옴표(`""`)로 감싸서 출력합니다.

#### 속성 구조화 활용

위처럼 메시지 템플릿에 포함된 각 속성은 별도의 필드로 분리됩니다.

**사용 예시:**

```csharp
public static void Log(string message, [CallerMemberName] string? caller = null)
{
	Serilog.Log.Information("[{Caller}]: {Message}", caller, message);
}
```

이 방식은 caller와 message를 Serilog의 메시지 템플릿 안에서 `{속성명}`으로 명시적으로 지정하여,

`JSON` 형태로 출력 시 로그 내부의 `Properties` 섹션에 다음과 같이 출력합니다.

**출력 결과:**

```json
{
  "Timestamp": "2025-04-02T16:22:21.8118034+09:00",
  "Level": "Information",
  "MessageTemplate": "[{caller}] {message}",
  "TraceId": "02e30ce35de9c3bc6b9daca7160325f8",
  "SpanId": "286b6a8552d56885",
  "Properties": {
    "Caller": "Chat",
    "Message": "chat completion succeeded"
  }
}
```

> ❌ 참고: 문자열 보간(string interpolation)을 사용할 경우 구조화된 속성으로 인식되지 않습니다

```csharp
public static void Log(string message, [CallerMemberName] string caller = "")
{
	Serilog.Log.Information($"[{caller}] {message}");
}
```

**출력 결과:**

```json
{
  "Timestamp": "2025-04-02T16:19:35.9413610+09:00",
  "Level": "Information",
  "MessageTemplate": "[Chat] chat completion succeeded",
  "TraceId": "a69199d37a6f811eec4c265ca79bd173",
  "SpanId": "be51b4ebb5c23cf8"
}
```

#### 문법 규칙

- 속성 이름은 중괄호(`{}`) 안에 작성합니다

```csharp
Log.Information("User {UserId} logged in", userId);
```

- 속성 이름은 유효한 C# 식별자여야 합니다.

```
  - (`O`) FooBar는 유효
  - (`X`) Foo.Bar 또는 Foo-Bar는 유효하지 않음
```

- 중괄호를 이스케이프(`escape`)하려면 두 번 중복해서 작성합니다. (`{{`는 `{`로 렌더링됩니다.)
- 숫자 인덱스를 사용하는 포맷 (`{0}`, `{1}` 등)은 string.Format()과 동일하게 파라미터 순서에 따라 바인딩됩니다.

```csharp
Log.Information("Item {0} at index {1}", item, index); // {0}, {1} → item, index에 대응
```

- 속성 이름 중 하나라도 숫자가 아닌 이름이라면, 모든 속성 이름은 왼쪽에서 오른쪽 순서대로 파라미터에 매칭됩니다.

```csharp
Log.Information("User {Name} (ID: {Id})", name, id); // 이름 기준으로 순서 매칭
```

- 속성 이름 앞에 @ 또는 $를 붙이면 직렬화 방식을 제어할 수 있습니다.

  - **@Property**: 객체 전체를 구조화된 형태로 로깅
  - **$Property**: 객체의 ToString() 값을 사용하여 문자열로 로깅

- 속성 이름 뒤에 :000 등 포맷 문자열을 붙이면 렌더링 형식을 제어할 수 있습니다.

  - 이는 `string.Format()`에서 사용하는 포맷 문자열과 동일하게 동작합니다.

  ```csharp
  Log.Information("Order total: {Total:0.00}", total); // 소수점 두 자리로 출력
  ```

# 활용 예제

## ASP .NET Core 9 Web API Server

웹 API 서버에서 Serilog를 활용한 예시 입니다.

[📁 프로젝트 바로가기](APIServer/)

#### 목차

- [Serilog 설정 및 구성](#로그-시스템을-serilog로-설정하기)
- [Logger 래퍼 클래스 구현](#supersocket에-serilog-구성하기)
- [OpenTelemetry Sink 추가](#opentelemetry-sink-추가)

### Serilog 설정 및 구성

아래는 appsettings.json에서 Serilog 다중 출력 구성 예시입니다.

아래 예제에서는 콘솔, JSON 파일, SQLite, MongoDB에 로그를 동시에 기록합니다.

```json
"Serilog": {
  "MinimumLevel": "Debug",
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "File",
      "Args": {
        "path": "Logs/log-.json",
        "restrictedToMinimumLevel": "Information",
        "rollingInterval": "Day",
        "retainedFileCountLimit": 7,
        "rollOnFileSizeLimit": true,
        "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog",
        "fileSizeLimitBytes": 10000000
      }
    },
    {
      "Name": "SQLite",
      "Args": {
        "restrictedToMinimumLevel": "Information",
        "sqliteDbPath": "Logs/logs.db",
        "tableName": "Logs"
      }
    },
    {
      "Name": "MongoDBBson",
      "Args": {
        "restrictedToMinimumLevel": "Error",
        "databaseUrl": "mongodb://shanabunny:comsooyoung!1@localhost:27017/serilog?authSource=admin",
        "collectionName": "logs",
        "cappedMaxSizeMb": "100"
      }
    }
  ]
}
```

### Logger 래퍼 클래스 구현

메서드 이름 자동 추적 기능을 포함하는 유틸리티 클래스를 작성합니다.

> [Logger.cs](APIServer/Logger.cs)

```csharp
public static class Logger
{
	public static void Log(string message, [CallerMemberName] string? caller = null)
	{
		Serilog.Log.Information("{Caller} {Message}", caller, message);
	}

	public static void LogError(string message)
	{
		Serilog.Log.Error(message);
	}

	public static void LogError(ResultCode resultCode, string message, [CallerMemberName] string? caller = null)
	{
		Serilog.Log.Error("{Caller} {ResultCode} {Message}", caller, resultCode, message);
	}

	public static void LogError(Exception e, string message)
	{
		Serilog.Log.Error(e, message);
	}
}
```

### Controller에서 Serilog 활용

클라이언트 요청 처리 흐름에서 다음과 같이 로그를 남깁니다.

> [AIController.cs](APIServer/Controllers/AIController.cs)

```csharp
[HttpPost("chat")]
public async Task<ChatResponse> Chat([FromBody] ChatRequest request)
{
	var response = new ChatResponse();
	(response.Result, response.Completion) = await _aiService.CompleteChatAsync(request);
	if (response.Result != ResultCode.Success)
	{
		Logger.LogError(response.Result, "chat completion failed");
	}
	else
	{
		Logger.Log("chat completion succeeded");
	}
	return response;
}
```

기능 단위로 성공/실패 여부를 명확히 구분하여 로그 출력합니다.

요청에 성공할 경우(`Logger.Log()`실행) 저장된 Serilog 로그는 다음과 같습니다:

```json
{
  "Timestamp": "2025-04-02T17:07:38.1636638+09:00",
  "Level": "Information",
  "MessageTemplate": "{Caller} {Message}",
  "TraceId": "1edad387457dd601435b2dc323c353e4",
  "SpanId": "f8aad533cc823c42",
  "Properties": { "Caller": "Chat", "Message": "chat completion succeeded" }
}
```

## .NET 9 Socket Server using SuperSocket

소켓서버에서 Serilog를 활용한 예시 입니다.

[📁 프로젝트 바로가기](SocketServer/)

#### 목차

- [앱 로그 시스템을 Serilog로 설정하기](#로그-시스템을-serilog로-설정하기)
- [SuperSocket에 Serilog 구성하기](#supersocket에-serilog-구성하기)+
- [SuperSocket에 Serilog 구성하기](#supersocket에-serilog-구성하기)

### 로그 시스템을 Serilog로 설정하기

```
$ dotnet add package Serilog.Extensions.Hosting
```

위 패키지를 사용하여 .NET Host 환경에서 Serilog를 다음과 같이 메인 애플리케이션 로거로 설정합니다.

```csharp
var host = new HostBuilder()
	.ConfigureAppConfiguration((context, config) =>
	{
		var env = context.HostingEnvironment;
		config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
	})
	.UseSerilog((hostingContext, loggerConfiguration) =>
	{
		loggerConfiguration
			.ReadFrom.Configuration(hostingContext.Configuration);
	})
	// ...
	.Build();
```

이 설정만으로는 SuperSocket 내부의 로그 (base.Logger, ILog)에는 영향을 주지 않으며,

따로 Serilog를 SuperSocket에 연동해야 합니다.

### SuperSocket에 Serilog 구성하기

SuperSocket은 자체 로깅 인터페이스인 ILog와 ILogFactory를 사용하므로,

Serilog를 연결하기 위해서는 커스텀 어댑터 및 팩토리 클래스를 구현해야 합니다.

#### 1. SerilogAdaptor 클래스 생성

> [SerilogAdaptor](SocketServer/SerilogAdaptor.cs): SuperSocket의 ILog를 Serilog에 연결하는 어댑터

```csharp
public class SerilogAdaptor : ILog
{
	private readonly ILogger _logger;

	public SerilogAdaptor(ILogger logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public bool IsDebugEnabled => _logger.IsEnabled(LogEventLevel.Debug);
	public bool IsErrorEnabled => _logger.IsEnabled(LogEventLevel.Error);
	public bool IsFatalEnabled => _logger.IsEnabled(LogEventLevel.Fatal);
	public bool IsInfoEnabled => _logger.IsEnabled(LogEventLevel.Information);
	public bool IsWarnEnabled => _logger.IsEnabled(LogEventLevel.Warning);

	public void Debug(string message) => _logger.Debug(message);
	public void Error(string message) => _logger.Error(message);
	public void Error(string message, Exception exception) => _logger.Error(exception, message);
	public void Fatal(string message) => _logger.Fatal(message);
	public void Fatal(string message, Exception exception) => _logger.Fatal(exception, message);
	public void Info(string message) => _logger.Information(message);
	public void Warn(string message) => _logger.Warning(message);
}
```

#### 2. SerilogFactory 구현

SuperSocket은 ILogFactory 팩토리 패턴을 채택하여 각 구성 요소에 이름 기반(Contextual) 로거를 제공합니다.

Serilog의 `ForContext()`를 사용해 name 값을 로그 출처로 지정하여,

SuperSocket 내부의 각 컴포넌트가 고유한 `SourceContext`를 가진 Serilog 로거를 사용하도록 합니다.

> [SerilogFactory](SocketServer/SerilogFactory.cs): SuperSocket의 LogFactoryBase 구현체

```csharp
public class SerilogFactory : LogFactoryBase
{
	public SerilogFactory(string configPath = "appsettings.json", bool isSharedConfig = false)
		: base(configPath)
	{
		// 메인 애플리케이션에서 Program.cs에서 UseSerilog()를 통해
		// Serilog의 전역 로거(Log.Logger)를 이미 설정한 경우
		if (isSharedConfig)
		{
		}
		// Supersocket만 별도로 구성
		else
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile(configPath).Build())
				.CreateLogger();
		}
	}

	public override ILog GetLog(string name)
	{
		var logger = Log.Logger.ForContext("SourceContext", name);
		return new SerilogAdaptor(logger);
	}
}
```

본 예제와 같이 해당 Factory 외부에서 Serilog를 이미 초기화한 경우,

Serilog의 `Log.Logger`를 다시 설정하면 기존 전역 로거 구성이 덮어쓰기되어

로그가 이중 설정되거나, 일부 로그가 유실될 수 있습니다.

이 경우 `isShared`를 true로 설정하여 사전에 등록된 전역 로거를 참조만 하도록 구성합니다.

##### 💡 Serilog를 SuperSocket에서만 단독 실행하는 경우

만약 Serilog를 SuperSocket에서만 단독 실행하는 경우이고,

외부에서 `Serilog.Log.Logger`가 초기화되지 않은 상태라면,

Serilog를 `SerilogFactory` 생성자에서 직접 초기화하도록 `isShared`를 false 로 전달합니다.

구현한 SerilogFactory는 SuperSocket의 Setup 메서드 호출 시 아래와 같이 적용합니다:

```csharp
bool bResult = Setup(new RootConfig(), _networkConfig, logFactory: new SerilogFactory(isSharedConfig: true));
```

서버가 정상적으로 기동되면, Serilog를 통해 다음과 같은 구조화된 로그가 출력됩니다:

```json
{
  "Timestamp": "2025-04-02T19:46:39.9345125+09:00",
  "Level": "Debug",
  "MessageTemplate": "Listener (0.0.0.0:9000) was started",
  "Properties": {
    "SourceContext": "SocketServer",
    "MachineName": "\"SHANABUNNY\"",
    "ThreadId": 1
  }
}
```

### OpenTelemetry Sink 추가

.NET 애플리케이션에서 OpenTelemetry Collector로 로그를 전송하려면
Serilog에 `Serilog.Sinks.OpenTelemetry` 패키지를 사용합니다.

appsettings.json 구성에 다음 항목을 추가합니다:

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "OpenTelemetry",
        "Args": {
          "EndPoint": "http://127.0.0.1:4317",
          "ResourceAttributes": {
            "service.name": "SocketServer"
          }
        }
      }
    ]
  }
}
```

OpenTelemetry용 Serilog 설정의

- 기본 EndPoint는 `http://localhost:4317`이며,
- 기본 Protocol은 `OtlpProtocol.Grpc`입니다.

Protocol 설정은 필요에 따라 OtlpProtocol.HttpProtobuf로 변경할 수 있으며,

이 경우 OpenTelemetry 로그는 HTTP + Protobuf 형식으로 전송됩니다.

Protocol을 명시적으로 설정하고 싶을 경우 protocol 옵션에 원하는 값을 지정하면 됩니다.

추가적으로, OpenTelemetry 로그에는 로그가 속한 서비스나 환경 정보를 포함하는 `ResourceAttributes`를 설정할 수 있습니다.

아래는 Collector가 gRPC로 수신한 로그를 debug exporter를 통해 출력한 예시입니다:

```
otel-collector-1  | Trace ID:
otel-collector-1  | Span ID:
otel-collector-1  | Flags: 0
otel-collector-1  | LogRecord #1
otel-collector-1  | ObservedTimestamp: 2025-04-03 00:47:58.3004766 +0000 UTC
otel-collector-1  | Timestamp: 2025-04-03 00:47:58.3004766 +0000 UTC
otel-collector-1  | SeverityText: Information
otel-collector-1  | SeverityNumber: Info(9)
otel-collector-1  | Body: Str(서버 생성 성공)
otel-collector-1  | Attributes:
otel-collector-1  |      -> MachineName: Str("SHANABUNNY")
otel-collector-1  |      -> ThreadId: Int(1)
otel-collector-1  |      -> message_template.text: Str(서버 생성 성공)
```
