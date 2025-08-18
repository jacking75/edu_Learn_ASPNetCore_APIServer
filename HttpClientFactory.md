##  API 서버간 통신 때 HttpClientFactory 사용하기
[MS Docs: ASP.NET Core에서 IHttpClientFactory를 사용하여 HTTP 요청  만들기](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0 )  
    
```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("Google", httpClient =>{
    httpClient.BaseAddress = new Uri("https://www.google.com/");
});
builder.Services.AddHttpClient("BaiDu", httpClient =>{
    httpClient.BaseAddress = new Uri("https://www.baidu.com/");
});
```
  
```
interface IGoogleService{
    Task<string> GetContentAsync();
}
class GoogleService : IGoogleService{
    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
	
    public async Task<string> GetContentAsync()
    {
        var googleClient = _httpClientFactory.CreateClient("Google");
		var response = await googleClient.GetAsync("some-endpoint");
		...
    }
}
```        
    
IHttpClientFactory는 ASP.NET Core에서 HTTP 클라이언트를 관리하는 데 많은 장점을 제공한다.  
- 클라이언트 인스턴스 풀링: IHttpClientFactory는 HttpClient 인스턴스를 풀링하여 재사용합니다. 이로 인해 매 요청마다 새로운 HttpClient를 생성하는 비용을 줄이고 성능을 향상시킵니다.
- 수명 관리: IHttpClientFactory는 HttpClient 인스턴스의 수명을 관리합니다. 요청이 완료되면 HttpClient를 반환하고 재사용할 수 있도록 합니다. 이로 인해 메모리 누수를 방지하고 리소스를 효율적으로 사용할 수 있습니다.
- 구성 가능한 클라이언트: IHttpClientFactory를 사용하면 여러 클라이언트를 구성할 수 있습니다. 위의 코드에서 “Google” 및 “BaiDu” 클라이언트를 등록했습니다. 각 클라이언트는 서로 다른 BaseAddress를 가지며 다른 서비스에 대한 요청을 보낼 수 있습니다.
- DI (의존성 주입) 지원: IHttpClientFactory는 DI 컨테이너와 통합되어 의존성 주입을 통해 HttpClient를 쉽게 주입할 수 있습니다.
- 테스트 용이성: IHttpClientFactory를 사용하면 HttpClient를 목(Mock)으로 대체하여 단위 테스트를 수행할 수 있습니다. 이는 외부 서비스에 의존하는 코드를 테스트할 때 유용합니다.
    
요약하자면, IHttpClientFactory는 성능, 메모리 관리, 구성 가능성 및 테스트 용이성 측면에서 HTTP 클라이언트를 효율적으로 관리하는 데 도움이 됩니다.  
  
### 동시 요청을 많이 보내고 싶을 때   
ASP.NET Core Web API 서버에서 `HttpClientFactory`를 사용하여 다른 서버에 많은 요청을 동시에 보내야 하는 경우, 다음과 같은 사항들을 고려해야 합니다.    
  
---    

#### 1. 기본 연결 제한 (MaxConnectionsPerServer) 조정  
.NET의 기본 `SocketsHttpHandler`에는 서버당 최대 동시 연결 수가 제한되어 있습니다. 기본값은 일반적으로 2로 설정되어 있어, 많은 요청을 동시에 보낼 경우 병목현상이 발생할 수 있습니다. 이를 해결하기 위해 **MaxConnectionsPerServer** 값을 늘려야 합니다.  
  
예를 들어, 다음과 같이 HttpClientFactory를 통해 생성되는 HttpClient에 대해 최대 연결 수를 늘릴 수 있습니다:  
```csharp
services.AddHttpClient("MyClient")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 100 // 필요에 맞게 설정 (예: 100 또는 그 이상)
        });
```  
  
이렇게 설정하면, 동일한 서버로의 동시 연결 수가 증가하여 많은 요청을 동시에 처리할 수 있습니다.  
  
---  
  
#### 2. HttpMessageHandler 재사용 및 관리  
`HttpClient`는 재사용이 가능한 인스턴스를 만드는 것이 중요합니다. `IHttpClientFactory`를 사용하면 HttpMessageHandler를 효율적으로 관리할 수 있으며, 사용 후에도 Dispose 호출로 인한 소켓 포트 소진(Socket exhaustion)을 예방할 수 있습니다.  
  
- **핵심 포인트:**  
  - HttpClient 인스턴스를 재사용하거나, `HttpClientFactory`를 통해 관리하도록 하여 연결 재사용을 극대화합니다.
  - 필요하다면 **PooledConnectionLifetime** 또는 **PooledConnectionIdleTimeout**을 설정하여 오래된 연결을 주기적으로 닫을 수 있습니다.  
  
예시:  
```csharp
services.AddHttpClient("MyClient")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 100,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5), // 최대 연결 수명
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2) // 유휴 연결 유지 시간
        });
```  
  
---
  
#### 3. 폴리시 및 타임아웃 적용   
많은 요청을 동시에 보낼 때는 잠재적인 네트워크 지연이나 실패를 고려해 **타임아웃**과 **재시도(retry) 정책**등을 적용하는 것이 좋습니다.  
  
- **타임아웃 설정:**  
  HttpClient의 Timeout 속성을 조정하여 매 요청의 최대 대기 시간을 설정합니다.
  
- **폴리시 설정:**  
  [Polly 라이브러리](https://github.com/App-vNext/Polly)를 사용하여 재시도, 회로 차단기(Circuit Breaker) 등의 폴리시를 추가할 수 있습니다.
  
예시 (Polly를 사용한 재시도 정책):  
```csharp
services.AddHttpClient("MyClient")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 100,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2)
        })
    .AddTransientHttpErrorPolicy(builder =>
        builder.WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        )
    );
```
  
---
  
#### 4. 적절한 부하 테스트 및 모니터링   
- `HttpClientFactory` 사용 시, 설정한 옵션들이 실제 환경에서 기대한 대로 동작하는지 부하 테스트를 수행합니다.
- ASP.NET Core의 로깅 및 모니터링 기능을 통해 연결 상태, 응답 시간, 오류 등을 체크하여 필요한 경우 추가적인 조정을 수행합니다.
  
---

#### 결론  
많은 요청을 동시에 처리하기 위해서는 기본 연결 제한 값을 늘리고, HttpClient 및 HttpMessageHandler 재사용, 적절한 타임아웃과 폴리시 적용, 그리고 이후 모니터링을 통한 지속적인 관리가 필수적입니다. 이러한 설정을 통해 ASP.NET Core 웹 API에서 `HttpClientFactory`를 효율적으로 사용하여 높은 동시성의 요청을 안정적으로 처리할 수 있습니다.
  