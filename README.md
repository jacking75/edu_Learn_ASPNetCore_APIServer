# ASP.Net Core Web API Game Server 학습
- 닷넷에서 API Server용 게임서버 개발을 위해 학습을 하기 위한 것이다.
- [닷넷 빌드와 실행](./dotnet_build.md )
- [ASP.NET Core](https://learn.microsoft.com/ko-kr/aspnet/core  )  
- [C# 비동기 프로그래밍 정리](https://docs.google.com/document/d/e/2PACX-1vRHRbQjeoJH9lXalTClFBuB-D41v9TaBTPc_TeUS-yKhPZTJa2dWjpv_Rib863b_disjspqymOjgKwq/pub )
- [(YOUTUBE) VisualAcademy](https://www.youtube.com/@VisualAcademy/videos )
- [practical-aspnetcore](https://github.com/dodyg/practical-aspnetcore) ASP.NET Core의 다양한 샘플 코드를 볼 수 있다.  
  
**이 저장소에 있는 코드는 약간의 오류나 좋지 않은 코드는 일부러 놓아두고 있습니다. 학습하는 사람이 학습하면서 수정하는 것을 유도하고 있습니다. PR은 받지 않습니다.**        

이 저장소의 내용을 DeepWiki로 만들었다. [2025년5월9일 제작한 위키](https://deepwiki.com/jacking75/edu_Learn_ASPNetCore_APIServer/1-overview)  영어로 되어 있다.  
    
<br>     

## 1. 학습하기 
**직접 예제 코드를 만들어 보면서 학습을 잘하자**    
  
### Redis 프로그래밍
- [Redis의 기본 데이터 유형 및 명령](https://docs.google.com/document/d/10mHFq-kTpGBk1-id5Z-zoseiLnTKr_T8N3byBZP5mEg/edit?usp=sharing )
- [(영상) Redis 야무지게 사용하기](https://forward.nhn.com/2021/sessions/16 )
- [Redis 기능 학습하기](http://redisgate.kr/redis/introduction/redis_intro.php )
- C# Redis 프로그래밍 학습
    - [CloudStructures - Redis 라이브러리  소개- jacking75](https://jacking75.github.io/NET_lib_CloudStructures/ )
    - [CloudStructures를 이용한 C# Redis 프로그래밍](https://gist.github.com/jacking75/5f91f8cf975e0bf778508acdf79499c0 ) 
	- [CloudStructures 라이브러리 사용 설명서](./redis_CloudStructures_Docs)
- `redis_training` 디렉토리에 있는 것으로 redis 실습을 해보기 바란다
   
    
### MySQL 프로그래밍
- [MySqlConnector 간단 정리](https://gist.github.com/jacking75/51a1c96f4efa1b7a27030a7410f39bc6 ) 
- [DB 프로그래밍 라이브러리 SqlKata 소개](https://docs.google.com/document/d/e/2PACX-1vTnRYJOXyOagNhTdhpkI_xOQX4DlMu0TRcC9Ehew6wraufgEtBuQiSdGpKzaEmRb-jfsLv43i0nBQsp/pub )
    - [예제 프로그램: github_sqlkata_demo.zip](https://drive.google.com/file/d/1FBpB1zQ84LqGOA9WAJ6vk5S3453ekqDc/view?usp=sharing )
- [코드에서 DB 트랜잭션 하기](./how_to_db_transaction.md )
    
    
### 로그  
  
#### 로그 라이브러리 ZLogger
.NET 에서 로그 라이브러리로 ZLogger를 추천한다.  
사용 방법은 `ZLogger` 디렉토리를 보기 바란다.    
   
#### 로그 라이브러리 SeriLog  
[SeriLog 사용하기](./Serilog.md )     
        
         
### 문서 
- [Web서비스의 서버구성과목적](https://docs.google.com/presentation/d/105NPfv7CPfgk0Iw_6vSB_oOavQZpes7-Wit5HuCm7oM/edit?usp=sharing )
- [DAO, DTO, VO 란? 간단한 개념 정리](https://melonicedlatte.com/2021/07/24/231500.html )
- ASP.NET Core Web [시작](https://blog.naver.com/sssang97/223088811266 )  [컨트롤러와 route 규칙](https://blog.naver.com/sssang97/223088978577 )  [Request 처리](https://blog.naver.com/sssang97/223089025576 )  [Response 처리](https://blog.naver.com/sssang97/223089045407 )
- [ASP.NET Core 애플리케이션 최소 구현 치트 시트](https://jacking75.github.io/csharp_netcore_cheatsheet/)
- 설정 파일  app.config
    - [Microsoft.Extensions.Configuration.Json에서 읽은 설정 값에 Prefix 부여](https://docs.google.com/document/d/e/2PACX-1vQbK6RbrkoyhQDL1u1_8_ZQ02Dsqhkdv3WHj2UHY8SyuM5fgOy3RtIZ2B2f0iFtMqySU-dCZKjAsd4Y/pub )
    - [MS Docs ASP.NET Core에서 여러 환경 사용](https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/environments?view=aspnetcore-6.0 )
    - [ASP.NET Core에서 여러 환경 사용](https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/environments )
    - [Microsoft.Extensions.Configuration 및 Microsoft.Extensions.Options 사용 시작](https://docs.google.com/document/d/12OwjWyzMwYWMWi9LEZEvgE2gjx-aGWP17f2Pw_z1S4o/pub)
- DI
    - [ASP.Net Core - DI 시스템 사용하기](https://docs.google.com/document/d/e/2PACX-1vR0NxfIPIQe_CAxf2-yo9B9QB4O0NIlJz3U-oe4iF_sHIpMnJiu_4o3ZlWoPD0kcW9ve9ka49_sLa8u/pub )
    - [ASP.Net Core - DI에서 Dbcontext 사용하기](https://docs.google.com/document/d/e/2PACX-1vTw549tqwoIE6WOWv5a8lFUFNakk988zXvpu2NHzhWrf8dKnbyYBza281hp3Gk9kRqO6r22hTQ63hGI/pub )
    - [ASP.NET Core - DI로 추가한 클래스의 초기화 방법](https://www.sysnet.pe.kr/2/0/13152 )
	- [의존성 주입 방법의 라이프 사이클 - AddTransient, AddScoped, AddSingleton](https://docs.google.com/document/d/e/2PACX-1vRFi_2Z6yMOWNwWfILDXGsbqYS3aJfiO6aO2u22Awy-pQ5XEEz0GpIOjehif47noYsR06jT6z_pD6Mr/pub )
- [ASP.NET Core 입문 – 미들웨어와 파이프라인 해설](https://docs.google.com/document/d/e/2PACX-1vRsqcyeBi--VYCPwQlhW9LsAyYUKSuuh80_BiSgnNnrfULsZFgz3i_Bj8nGG6dl-Q6NEiKBjhGx2bJ6/pub )	
- [C#(.NET) 이미 빌드 된 API 서버의 엔드 포인트 url을 변경하는 방법](https://docs.google.com/document/d/e/2PACX-1vRhHebXWTa1OOY60NT3T0ZRkr8NMNRIuFADhVZIknSs_YsnlYTz7FObkexo9x1iNfoQID31-hVxiwTB/pub )
- [.NET 6에서 Kestrel 포트를 설정하는 몇가지(너무 많은?) 방법](https://forum.dotnetdev.kr/t/net-6-kestrel-bryan-hogan/2681 )
- [Visual Studio - launchSettings.json을 이용한 HTTP/HTTPS 포트 바인딩](https://www.sysnet.pe.kr/2/0/13539 )
- [ASP.NET Core 앱에서 URL을 설정하는 5가지 방법](https://docs.google.com/document/d/1x3ZJQtGt2uNW5_xRT6QHrOmZkfbNU2KQ23N5RCLi4cE/edit?usp=sharing )
- [ASP.NET Core의 속도 제한 미들웨어](https://learn.microsoft.com/ko-kr/aspnet/core/performance/rate-limit?preserve-view=true&view=aspnetcore-7.0  )
- [요청 본문을 Stream 또는 PipeReader로 바인딩](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-7.0#rbs ) 
- [STREAM](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/minimal-apis?preserve-view=true&view=aspnetcore-7.0#stream7  )
- [ASP.NET Core 출력 캐싱 미들웨어](https://learn.microsoft.com/ko-kr/aspnet/core/performance/caching/output?view=aspnetcore-7.0  ) 
- [ASP.NET Core 프레임워크 내부의 로그 수준 설정하기](https://docs.google.com/document/d/e/2PACX-1vRN7e0qnQE9gC780ddPfIojCnVUhd7mf-uYk6oRIibo_nEbs3HWJe8-61jDAiel37AQxe8BxKjb58-l/pub )
- [ASP.NET Core에서 호스팅되는 서비스를 사용하는 백그라운드 작업](https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/host/hosted-services?tabs=visual-studio&utm_source=pocket_mylist&view=aspnetcore-6.0 )
- [IHostedService 및 BackgroundService 클래스를 사용하여 마이크로 서비스에서 백그라운드 작업 구현](https://docs.microsoft.com/ko-kr/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice )
- [practical-aspnetcore ASP.NET Core 예제 프로젝트 모음](https://github.com/dodyg/practical-aspnetcore )
- [ASP.NET Core 서버 시작까지 흐름 추적](https://jacking75.github.io/NET_lAspNetCoreTrace/)
- [ASP.NET Caching](https://docs.google.com/presentation/d/14x3Byprw28n-bwzwT0a5IZ_IBty9ZCaZmUM3pNUmrww/edit?usp=sharing)
- [Client IP safelist for ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/ip-safelist?view=aspnetcore-2.1 )
- [Custom ASP.NET Core Middleware Example](https://blogs.msdn.microsoft.com/dotnet/2016/09/19/custom-asp-net-core-middleware-example/ )
- [Exploring a minimal WebAPI with ASP.NET Core](http://www.hanselman.com/blog/ExploringAMinimalWebAPIWithASPNETCore.aspx )
- [ASP.NET Core에서 API 별 버전 관리](https://docs.google.com/document/d/1XMqTausAUspWr0GLXMkdLpX-vE4uDnQj0JZ3Fmi61-Y/edit?usp=sharing )
- [ASP.NET Core WebAPI에서 Custom Formatter를 이용하여 CSV 및 JSON 출력을 전환하는 샘플](https://docs.google.com/document/d/12sLRZPFBh3uJ5UGTfd59bvuTZhjIHkwi9tT2ZyDYjJs/edit?usp=sharing )
- 실행, 배포
    - ["Docker Desktop for Windows" - ASP.NET Core 응용 프로그램의 소켓 주소 바인딩(IPv4/IPv6 loopback, Any)](https://www.sysnet.pe.kr/2/0/13548 )
    - [Visual Studio - .NET 8 프로젝트부터 dockerfile에 추가된 "USER app" 설정](https://www.sysnet.pe.kr/2/0/13547 )
    - [ASP.NET Core - 우선순위에 따른 HTTP/HTTPS 호스트:포트 바인딩 방법](https://www.sysnet.pe.kr/2/0/13545 )
- CORS
    - [.NET Core3.1에서 CORS 설정하기](https://jacking75.github.io/csharp_cors/ )
    - [(MS Docs) ASP.NET Core에서 CORS(원본 간 요청) 사용](https://learn.microsoft.com/ko-kr/aspnet/core/security/cors?view=aspnetcore-8.0 )
- 외부 자료: 
    - [모바일 게임 개발 개요](https://drive.google.com/file/d/1WImt5yn7cpIBm2opZFWMapWW2GX6XB_R/view?usp=sharing ) 
	- [게임 엔진 아웃라인](https://drive.google.com/file/d/1HeBu2NXeLXU8VY5l7Gph4l6SRZkhZQae/view?usp=sharing ) 
	- [데이터 설계](https://drive.google.com/file/d/14NcczPD6XS1rLH6TpORCcLhF9ftsVSgu/view?usp=sharing ) 
	- [통신](https://drive.google.com/file/d/1VmxaTRA1qJnoIGWKK6SoYDH-pHBvnGbz/view?usp=sharing )

<br>   
<br>      
    
	
## 단계 별로 따라하면서 API 서버 만들기
아래 영상과 예제 코드를 참고하면서 단계 별로 만들면서 배운다.  
- [(YOUTUBE)1~6단계로 ASP.NET Core 기본 실습](https://youtu.be/YTDWXJG1SD8?si=PHz6XvNGy4yU-Sjj ) 
- [설명 문서](https://docs.google.com/presentation/d/e/2PACX-1vQMWp7xa2ihTYvrytkMPSGSLOaKZq2qQgL4yCa7RXiBKkBfpqc4Y_LBDpWLaeJXoctfSn0ASPbQQfVz/pub?start=false&loop=false&delayms=3000&slide=id.p )  
- 예제 코드는 `codes` 디렉토리의 `basic2` 디렉토리 안에 있다.    
      
<br>      
  
  
## API Server 개발하기 
- `code` 디렉토리의 `GameAPIServer_Template` 디렉토리에 있는 코드를 참고해서 만들기 바란다.
    - `GameAPIServer_Template_Doc` 에 코드에 대한 설명 문서가 있으니 꼭 보기 바란다
- 프로젝트는 새로 만들고 구조나 코드 등을 참고한다.    
- `GameAPIServer_Template` 에 있는 코드 보다 더 좋은 코드를 만드는 것을 목표로 한다.  
      
    	
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
  
          
    
<br>    
  
## example_filter_APIServer
- 아래 프로그램은 `codes` 디렉토리 안에 있다.  
- 기획데이터(마스터 데이터)가 json으로 된 것을 로딩
- 미들웨어로 라우팅을 커스텀마이징
- 미들웨어로 클라이언트 보낸 요청 데이터의 암호를 풀어서 핸들러에 전달하도록 한다
- 필터로 클라이언트에 보내는 응답의 포맷을 변환하거나 암호화 하도록 한다  
  
<br>   
  
## gRPC Server    
- 아래 프로그램은 `codes` 디렉토리 안에 있다.  
- 아래 기능을 사용하여 JSON을 주고 받을 수 있도록 한다. 아직 클라이언트에서 gRPC 통신을 쉽게 할 수 없는 경우가 많다.  
- MS Docs 
    - [ASP.NET Core gRPC 앱에서 gRPC JSON 코드 변환](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/json-transcoding?view=aspnetcore-7.0 )
    - [gRPC JSON 코드 변환에 대한 HTTP 및 JSON 구성](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/json-transcoding-binding?view=aspnetcore-7.0 )
	  - [.NET 앱에 대한 Protobuf 메시지 만들기](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/protobuf?view=aspnetcore-7.0)
	  - [.NET용 gRPC 구성](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/configuration?view=aspnetcore-7.0 )
	  - [.NET의 gRPC에서 로깅 및 진단](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/diagnostics?view=aspnetcore-7.0 )
	  - [gRPC와 프로세스 간 통신](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/interprocess?view=aspnetcore-7.0 )
- [샘플코드 Transcoder](https://github.com/grpc/grpc-dotnet/tree/master/examples/Transcoder)  
- [ASP.NET Core gRPC 앱에서 gRPC JSON 코드 변환 프로젝트 만들기](https://docs.google.com/document/d/e/2PACX-1vTQCG9IMA32WOgCFO417LqwS0KxrPK_AiijaR9tuz3a0lboL9C4TuHiOw-y9WZ0LtfIq1Wn2qxHvkZE/pub)    
  
   
<br>   
     
## 참고할 실습 프로젝트 
- (2022년) 신입 사원 교육 프로젝트
    - `codes/practice_robotmon-go` 디렉토리에 있다.  
	- `.NET 6` 버전 사용  
- 2023년 지니어스 인턴 프로젝트 
    - https://github.com/jacking75/com2us_edu_GenieFarm
- (2024년 1월) 신입 사원 교육 프로젝트
    - 미니게임천국 모작
    - `codes/practice_MiniGameHeavenAPIServer` 디렉토리에 있다.  
	- `.NET 8` 버전 사용      
- 2024년 API 서버로 만든 오목 게임 서버 
    - 두 개의 프로젝트가 있다. 둘다 만드는 게임은 같지만 각각 개발자는 달라서 구현도 조금씩 다르다
    - API 서버로 실시간 게임 콘텐츠를 구현하는 방법을 볼 수 있다. 롱 폴링 방식 사용 
    - `codes/practice_omok_game-1` , 'codes/practice_omok_game-2`  
- 2025년 신입 사원 교육 프로젝트 
    - 하스스톤 모작
    - `codes/HearthStone` 디렉토리에 있다.  
  
    
<br>   
<br>   
  
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
  
### Models 안에 DAO. DTO 정의  
DAO, DTO를 Models 안에 정의하는 방식도 있다.  
  
ASP.NET Core의 Model 네임스페이스에서는 주로 다음과 같은 요소들을 정의한다  
1. 엔티티 클래스 - 데이터베이스 테이블에 매핑되는 클래스들
2. 뷰 모델(ViewModel) - 뷰에 데이터를 전달하기 위한 클래스
3. 데이터 전송 객체(DTO) - 계층 간 데이터 전달을 위한 객체
4. 사용자 입력 검증을 위한 어노테이션/속성
5. 비즈니스 로직을 포함하는 도메인 모델  
  
예를 들어, 간단한 사용자 모델은 다음과 같이 정의할 수 있다:  

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace YourApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
```

ASP.NET Core MVC 패턴에서 Model은 애플리케이션의 데이터와 비즈니스 로직을 담당하는 중요한 부분이다. 데이터베이스와 상호작용하고 데이터의 유효성을 검증하며, 비즈니스 규칙을 구현하는 역할을 한다.        
  
1. **Models 폴더 구조화 방법**:
   ```
   Models/
   ├── Entities/        (데이터베이스 엔티티 클래스)
   ├── DTOs/            (데이터 전송 객체)
   ├── ViewModels/      (뷰에 사용되는 모델)
   └── Repository/      (Spring의 DAO와 유사한 역할)
   ```
  
2. **엔티티 클래스 예시**:
   ```csharp
   namespace YourApplication.Models.Entities
   {
       public class User
       {
           public int Id { get; set; }
           public string Name { get; set; }
           public string Email { get; set; }
           // 데이터베이스 테이블에 해당하는 속성들
       }
   }
   ```

3. **DTO 예시**:
   ```csharp
   namespace YourApplication.Models.DTOs
   {
       public class UserDTO
       {
           public int Id { get; set; }
           public string Name { get; set; }
           public string Email { get; set; }
           // 클라이언트에 전송할 데이터만 포함
       }
   }
   ```

4. **ViewModel 예시**:
   ```csharp
   namespace YourApplication.Models.ViewModels
   {
       public class UserViewModel
       {
           public string Name { get; set; }
           public string Email { get; set; }
           public List<string> Roles { get; set; }
           // 화면 표시에 필요한 추가 정보 포함
       }
   }
   ```

5. **Repository 인터페이스 예시**:
   ```csharp
   namespace YourApplication.Models.Repository
   {
       public interface IUserRepository
       {
           User GetById(int id);
           IEnumerable<User> GetAll();
           void Add(User user);
           void Update(User user);
           void Delete(int id);
       }
   }
   ```
  
ASP.NET Core에서는 Spring과 같은 엄격한 DAO/DTO 패턴을 강제하지는 않지만, 대규모 프로젝트에서는 위와 같이 명확히 분리하는 것이 유지보수에 도움이 된다.   
  
Repository 패턴은 ASP.NET Core에서 Spring의 DAO와 유사한 역할을 하며, 데이터 액세스 로직을 캡슐화한다. 실제 구현은 보통 `Services` 또는 `Data` 폴더에 두는 경우가 많다.    
      
	  
<br>   
    
## 참고 코드 
  
- 팜 게임 만들기: `api_server_training_tany_farm` 디렉토리
- 수집형 RPG 게임 만들기: `api_server_training_dungeon_farming` 디렉토리   
     
### 2022년 인턴 사원이 만든 학습 자료
`학습자료_2022년_인턴` 이라는 디렉토리에 2022년 인턴 사원으로 근무한 분이 만든 학습 자료가 있다.    
이 자료들을 순서대로 한번 보기 바란다.  
이것들을 본 후 실습을 하나씩 한다. 
`03_Dapper`, `06연습 프로젝트 구현`은 따라서 실습을 할 필요는 없다.  그냥 보기만 한다.    
   
학습에서는 데이터 베이스 프로그래밍으로 `Dapper`를 사용하고 있는데 실제 실습에서는 `SqlKata`를 사용한다  
[SqlKata 소개](https://docs.google.com/document/d/e/2PACX-1vTnRYJOXyOagNhTdhpkI_xOQX4DlMu0TRcC9Ehew6wraufgEtBuQiSdGpKzaEmRb-jfsLv43i0nBQsp/pub  )    
[demo 프로그램](./codes/github_sqlkata_demo.zip)  `codes` 디렉토리에 있는 `github_sqlkata_demo.zip` 파일이다.     
  
      
### 서버 캠퍼스 1기 수료생 정리 자료
- https://sueshin.tistory.com/category/%EA%B0%9C%EC%9D%B8%EA%B3%B5%EB%B6%80/Web%20API%20%EA%B2%8C%EC%9E%84%20%EC%84%9C%EB%B2%84%20%EA%B3%B5%EB%B6%80
- https://gist.github.com/jacking75/344fd3c8c16fc27fe51d4c983a3a6306
- https://paper-tub-6ae.notion.site/0be4168f7f224f82a89110423e3943cb
- [ASP.NET Core로 Web API 만들기](https://babebab.tistory.com/53 )
- [워리할수있다 (tistory.com)](https://wallyyoucandoit.tistory.com/?page=1)
- [Intro - WebAPI Server (gitbook.io)](https://dong-d.gitbook.io/webapi-server/)
- [ASP (notion.site)](https://easy-cell-518.notion.site/ASP-0f310b9157de4cb683a5250c5eb9cc19 )
- https://velog.io/@oak_cassia/%EC%84%9C%EB%B2%84%EC%BA%A0%ED%8D%BC%EC%8A%A4-1%EA%B8%B0-ASP.NET-Core
- https://beathe.at/2023-04/ASP.NET-Core-GameServer
    - [GitHub Com2usEduProject](https://github.com/beatheat/Com2usEduProject )
   
  
### 서버 캠퍼스 2기 수료생 정리 자료
`학습자료_2024_서버캠퍼스2기`  디렉토리를 보기 바란다  
  
    
<br>   

## C# 학습 자료
- 학습에는 LinqPad라는 툴을 사용하면 편리하다
    - [2019-03_LinqPad](https://docs.google.com/presentation/d/1THcgeub4cNRJdFCxHatpxkPR0AhYJay55P5xxZ1sgtE/edit?usp=sharing )
    - [LinqPad 무료 버전에서 nuget을 사용하고 싶을 때](https://docs.google.com/document/d/1Hn8WDZxkX5os86DZANeHS4ggeqq6NWD67QbfigawJuM/edit?usp=sharing ) 
- 빠르게 핵심 위주로 배우고 싶다면 예제로 배우는 C# 프로그래밍 사이트의 글을 본다. 
    - [문법](http://www.csharpstudy.com/CSharp/CSharp-intro.aspx )
    - [6.0 버전 이후의 새 기능](http://www.csharpstudy.com/Latest/CS-new-features.aspx )
    - [자료구조](http://www.csharpstudy.com/DS/array.aspx )
    - [멀티쓰레딩](http://www.csharpstudy.com/Threads/thread.aspx )
- [(인프런 무료 영상) C# 초보 강좌 예제로 배우는 C# - 11 강 ∙ 8시간 17분](https://inf.run/PVsq)
- [(인프런 무료 영상) C# 처음부터 배우기 - 커리큘럼 총 11 개 ˙ 3시간 53분의 수업](https://inf.run/bfkW)
- [(인프런 무료 영상) C# 프로그래밍 - 커리큘럼 총 63 개 ˙ 29시간 56분의 수업](https://inf.run/PueZ)
- [YOUTUBE 닷넷데브](https://www.youtube.com/c/%EB%8B%B7%EB%84%B7%EB%8D%B0%EB%B8%8C/videos )
