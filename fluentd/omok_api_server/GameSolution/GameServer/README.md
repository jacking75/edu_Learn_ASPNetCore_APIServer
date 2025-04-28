# Server Logging

게임서버에서 로그가 출력되는 과정을 설명합니다.

# ZLogger Configuration

`ZLogger` 사용을 위해서 아래와 같이 구성합니다.
```csharp
// Program.cs
SetLogger();
ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
```

Zlogger를 통해 제공받을 수 있는 내용은 다음과 같습니다.

## 로그 기본 구성

| Key Name     | Description                                                                                                                                                                                      |
| :----------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Category `  | 생성된 Logger의 할당된 카테고리 이름. <br/>**ex:** ILogger<HomeController\> 를 통해 쓰인 로그의 Category는 `App.Controllers.HomeController`가 됩니다                                             |
| `Timestamp ` | 로그가 기록된 시간을 나타냅니다.                                                                                                                                                                 |
| `LogLevel`   | `Microsoft.Extensions.Logging`에서 제공되는 로그의 심각도를 나타냅니다.                                                                                                                          |
| `EventId `   | `Microsoft.Extensions.Logging`에서 제공되는 이벤트 ID입니다. 각 로그 항목에 대한 고유 식별자 역할을 합니다.                                                                                      |
| `ScopeState` | `ILogger.BeginScope(...)`를 통해 설정된 추가 속성들을 포함합니다. 이는 로그 항목과 관련된 추가 정보를 저장합니다. (`ZLoggerOptions.IncludeScopes = true`일 경우).                                |
| `Context`    | 로그 기록 시 전달된 추가 객체입니다.                                                                                                                                                             |
| `MemberName` | 로그 호출 시의 멤버 이름을 나타냅니다. `CallerMemberName`을 통해 자동으로 설정되며, 호출된 메서드의 이름이 기록됩니다. <br/> **ex:** GameServer의 경우 `ActionLog`, `MetricLog`로 나뉘게 됩니다. |

## ZLogger 콘솔 출력 설정하기

ZLogger는 내장된 Providers를 통해 위 정보를 다양한 형태로 제공합니다. 

이중 `AddZLoggerConsole`를 사용하여 로그가 콘솔에 출력되도록 합니다.

```csharp
void SetLogger()
{
    ILoggingBuilder logging = builder.Logging;
    logging.ClearProviders();

    _ = logging.AddZLoggerConsole(options =>
    {
        option
        options.UseJsonFormatter(formatter =>
        {

            formatter.JsonPropertyNames = JsonPropertyNames.Default with
            {
                Timestamp = JsonEncodedText.Encode("timestamp"),
                MemberName = JsonEncodedText.Encode("membername"),
                Exception = JsonEncodedText.Encode("exception"),
            };

            formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
            formatter.IncludeProperties = IncludeProperties.Timestamp | IncludeProperties.ParameterKeyValues | IncludeProperties.MemberName | IncludeProperties.Exception;
        });
    });
}
```

| Parameter              | Description                                                                                                                                                                           |
| :--------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `OutputEncodingToUtf8` | `Console.OutputEncoding = new UTF8Encoding(false)` 로 Provider 를 생성합니다. (기본값: `true`) 해당 설정은 UTF-8 인코딩을 BOM(Byte Order Mark)이 없는 형식으로 출력되도록 설정합니다. |
| `UseJsonFormatter `    | 로그를 `JSON` 형식으로 출력하도록 합니다.                                                                                                                                             |

</br>

제공되는 [`options`](https://github.com/Cysharp/ZLogger?tab=readme-ov-file#zloggeroptions) 파라미터를 통해 `UseJsonFormatter`를 호출하여 출력 포맷을 `JSON`으로 바꾸고,

이후 [`formatter`](https://github.com/Cysharp/ZLogger#formatter-configurations) 파라미터를 통해 상세 출력 내용을 구성할 수 있습니다.

## Formatter (`SystemTextJsonZLoggerFormatter`) 구성

| Parameter               | Description                                                                                                                                                   |
| :---------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `JsonPropertyNames`     | 출력되는 JSON 로그의 property name을 변경할 수 있습니다.                                                                                                      |
| `IncludeProperties`     | 수집할 property를 추가하거나 제한할 수 있습니다.                                                                                                              |
| `KeyNameMutator`        | ParameterKeyValues의 key name 포맷팅 컨벤션을 지정합니다                                                                                                      |
| `JsonSerializerOptions` | [System.Text.Json](https://learn.microsoft.com/ko-kr/dotnet/api/system.text.json.jsonserializeroptions?view=net-8.0) 에서 제공하는 옵션을 활용할 수 있습니다. |

## JsonPropertyNames 사용하기

`JsonPropertyNames`를 이용하여 기본 로그 구성의 property name을 변경할 수 있습니다.

(기본 구성 외 추가적인 property들의 이름은 `AdditionalFormatter`를 활용하여 변경 가능합니다)

```csharp
formatter.JsonPropertyNames = JsonPropertyNames.Default with
{
    Timestamp = JsonEncodedText.Encode("timestamp"),
    MemberName = JsonEncodedText.Encode("membername"),
};
```

RESULT:

```json
{
  "timestamp": "2024-10-02T08:29:50.7544882+00:00",
  "membername": "ActionLog"
}
```

전송될 로그의 property name이 저장될 데이터베이스의 column 이름과 일치하도록 변경하여 fluentd에서 로그 데이터 가공을 줄이는게 목적입니다.

## IncludeProperties 사용하기

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

`IncludeProperties`는 [로그의 기본 구성](#로그-기본-구성)의 key name으로 이루어진 enum 이며, 원하는 구성을 비트 연산하여 적용합니다.

본 프로젝트에서는 해당 옵션을 활용하여 `Timestamp` , `ParameterKeyValues`, `MemberName`, `Exception`만 출력되게 구성되었습니다.

기본 값은 `Timestamp | LogLevel | CategoryName | Message | Exception | ScopeKeyValues | ParameterKeyValues` 입니다.

## KeyNameMutator 사용하기

```
formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter
```

`KeyNameMutator` 설정을 통해 `ParameterKeyValues` 삽입 시 생성되는 keyname의 컨벤션을 지정합니다.

기본적으로 `ParameterKeyValues`의 JSON 키 이름은 입력값 그대로 출력되기 때문에, 객체의 property를 호출하여 값 입력 시 아래처럼 해당 객체까지 키 값에 같이 출력됩니다.

```csharp
var user = new User(1, "Alice");
logger.ZLogInformation($"Name: {user.Name}");
```

```json
{ "user.Name": "Alice" }
```

`KeyNameMutator`을 `LastMemberName`으로 설정하면 해당 값의 키로 마지막 멤버 이름만을 가져오게 합니다.
따라서 `KeyNameMutator` 값을 `LastMemberName`으로 설정하였을 경우, 아래와 같이 출력되게 됩니다.

```json
{ "Name": "Alice" }
```

GameServer는 lowercase 컨벤션을 적용합니다.

```csharp
formatter.KeyNameMutator = KeyNameMutator.LastMemberNameLowerFirstCharacter;
```

해당 옵션을 적용할경우 동일한 로그가 아래와 같이 출력됩니다.

```json
{ "name": "Alice" }
```

<br/>

# Key Name 로그에서 지정

`ParameterKeyValues`의 key name을 로깅 메서드를 실행할때 `:` 또는 `:@` syntax를 활용하여 지정할 수 있습니다.

```csharp
_logger.ZLogInformation($"[{tag:json}] {context:json}");
```

## 사용자 정의 형식 `:`

`:` 구문은 변수에 사용자 정의 형식을 적용하여 로그 메시지에 표시할 때 사용됩니다.

예를 들어 BaseController에서 `ActionLog` 메서드에서 사용되는 context는 object이지만, 아래처럼 `:json`을 활용해 객체를 JSON으로 직렬화합니다.

```csharp
protected void ActionLog(object context, [CallerMemberName] string? tag = null)
{
    _logger.ZLogInformation($"[{tag:json}] {context:json}");
}
```

## 명시적 이름 변경 `:@`

`:@` 구문은 구조화된 데이터로 로깅할 때 변수의 이름을 명시적으로 변경할 수 있습니다.

기본적으로 ZLogger는 변수 이름을 구조화된 로그의 속성 키로 사용하지만, `:@newname`을 사용하면 해당 변수의 key name을 로그 출력에서 `newname`으로 지정할 수 있습니다.

### 사용자 정의 형식과 명시적 이름 변경 조합

`:@`(명시적 이름 변경)과 `:`(사용자 정의 형식)을 함께 사용하여 변수의 이름을 지정하고, 동시에 형식을 적용할 수도 있습니다.

```
logger.ZLogDebug($"Today is {DateTime.Now:@date:yyyy-MM-dd}.");
```

위 예시에서는 property 이름을 `date`로 변경하고, 날짜 형식을 `yyyy-MM-dd`로 지정합니다

# Logging Methods

통일화된 로그 형태를 위해 ZLogger를 사용하는 메서드들을 추상화 합니다.

## ActionLog on Controllers

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

## Metric Log in Services

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

## 출력용 메서드

디버깅 목적으로 출력되는 로그 메서드 입니다.

공통 항목

| Parameter    | Description                                                                                             |
| :----------- | :------------------------------------------------------------------------------------------------------ |
| `caller`     | 로깅 메서드를 호출한 함수 이름입니다. 자동으로 메서드명이 기록됩니다.                                   |
| `context`    | 로그와 관련된 추가적인 객체나 데이터를 포함할 수 있습니다.                                              |
| `membername` | ZLogger 메서드를 호출한 함수 이름입니다. 로깅 메서드 종류를 구분합니다. 자동으로 메서드명이 기록됩니다. |

### InformationLog

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

### ExceptionLog

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

### ErrorLog

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
