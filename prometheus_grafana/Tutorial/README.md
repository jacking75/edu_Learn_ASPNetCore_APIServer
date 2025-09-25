# Prometheus 실습

## 1. 프로메테우스 설치

## 2. Prometheus.yml
  
```yaml
# my global config
global:
  scrape_interval: 15s # Set the scrape interval to every 15 seconds. Default is every 1 minute.
  evaluation_interval: 15s # Evaluate rules every 15 seconds. The default is every 1 minute.
  # scrape_timeout is set to the global default (10s).
 
# Alertmanager configuration
alerting:
  alertmanagers:
    - static_configs:
        - targets:
          # - alertmanager:9093
 
# Load rules once and periodically evaluate them according to the global 'evaluation_interval'.
rule_files:
  # - "first_rules.yml"
  # - "second_rules.yml"
 
# A scrape configuration containing exactly one endpoint to scrape:
# Here it's Prometheus itself.
scrape_configs:
  - job_name: "prometheus"
    # metrics_path defaults to '/metrics'
    # scheme defaults to 'http'.
    static_configs:
      - targets: ["localhost:9090"]
 
# 이 설정으로 스크랩한 모든 시계열엔 여기 있는 job 이름이 `job=<job_name>` 레이블로 추가된다.
  - job_name: "apiserver"
# 글로벌로 설정해둔 기본값을 재정의하며, 이 job에선 타겟을 5초 간격으로 스크랩한다.
    scrape_interval: 5s
    static_configs:
    # 프로메테우스가 읽어올 주소를 입력한다.
      - targets: ['localhost:5000']
    # labels를 통해 읽어온 데이터에 라벨링을 하여 관리할 수 있다.
        labels:
          groups: "server"
   
  - job_name: "gameserver"
    scrape_interval: 5s
    static_configs:
      - targets: ['localhost:5002']
        labels:
          type: "server"
 
  - job_name: "server_info"
    scrape_interval: 5s
    static_configs:
      - targets: ['localhost:9182']
        labels:
          type: "info"

```


## API 서버에 적용하기
Prometheus는 “Pull 방식” → Web API에서 `/metrics` 같은 엔드포인트를 열어주면 Prometheus 서버가 주기적으로 가져간다.  
  
ASP.NET Core에서는 보통 `prometheus-net` 라이브러리를 많이 사용한다.  
👉 NuGet 패키지: prometheus-net.AspNetCore  

아래와 같은 NuGet도 있다.      
Nuget package for general use and metrics export via HttpListener or to Pushgateway: prometheus-net  
`Install-Package prometheus-net`    
    
Nuget package for ASP.NET Core middleware and stand-alone Kestrel metrics server: prometheus-net.AspNetCore  
`Install-Package prometheus-net.AspNetCore`    
  
Nuget package for ASP.NET Core Health Check integration: prometheus-net.AspNetCore.HealthChecks  
`Install-Package prometheus-net.AspNetCore.HealthChecks`  
  
Nuget package for ASP.NET Core gRPC integration: prometheus-net.AspNetCore.Grpc  
`Install-Package prometheus-net.AspNetCore.Grpc`  
    
  
### 패키지 설치
터미널(또는 Visual Studio 패키지 매니저 콘솔)에서:  
```
dotnet add package prometheus-net.AspNetCore
```  
  

### Program.cs 수정 
  
```  
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

// 1) 기본 HTTP 요청 미들웨어
app.UseHttpMetrics();   // 요청 관련 기본 메트릭 수집

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics(); // 2) /metrics 엔드포인트 등록
});

app.Run();
```

이제 http://localhost:5000/metrics (혹은 Kestrel/설정된 포트)에서 Prometheus 포맷 메트릭이 노출된다.  
위 주소의 port 번호는 api 서버에서 설정된 port 번호를 따르면 된다.  


### 커스텀 메트릭 추가하기

```
using Prometheus;

public class WeatherController : ControllerBase
{
    // Counter 메트릭 예시
    private static readonly Counter WeatherRequests = 
        Metrics.CreateCounter("weather_requests_total", "Total number of weather requests.");

    [HttpGet("weather")]
    public IActionResult GetWeather()
    {
        WeatherRequests.Inc(); // 호출할 때마다 카운터 증가
        return Ok(new { Temp = "24C", Status = "Sunny" });
    }
}
```  
  
- Counter: 단순 증가 값 (요청 수, 에러 수)
- Gauge: 현재 상태값 (메모리 사용량, 큐 길이)
-  Histogram/Summary: 분포/지연 시간 같은 값
  

### Prometheus 서버 설정
Prometheus의 prometheus.yml에 Web API 주소를 등록합니다:

```yaml
scrape_configs:
  - job_name: 'aspnetcore-api'
    scrape_interval: 15s
    static_configs:
      - targets: ['localhost:5000']   # API 주소와 포트
```  
  
- Web API가 /metrics를 열고 있으므로 metrics_path는 기본값(/metrics) 그대로 둔다.
- Prometheus 재시작 후, http://localhost:9090/targets 에서 aspnetcore-api job이 UP 상태로 떠야 한다.


### 참고할 수 있는 메트릭 예시
- 요청 수: http_requests_received_total
- 처리 시간 히스토그램: http_request_duration_seconds_bucket
- 현재 실행 중 요청 수: http_requests_in_progress
  

### GC와 스레드풀 등의 정보도 수집하고 싶을 때
ASP.NET Core API 서버에서 Prometheus로 **GC, 스레드 풀 등 특정 런타임 메트릭만 수집**하고 싶다면, 두 가지 방법이 있다.
 
#### 1) 기본 prometheus-net.AspNetCore 미들웨어만 사용할 때
`app.UseHttpMetrics()` + `endpoints.MapMetrics()` 조합은 **HTTP 요청/응답 관련 메트릭**만 기본 제공한다.  
GC, Thread Pool, Working Set 같은 .NET 런타임 메트릭은 자동으로는 나오지 않는다.  


#### 2) .NET 런타임 메트릭 전용 라이브러리 사용
**패키지**: [`prometheus-net.DotNetRuntime`](https://github.com/dotnet/runtime)  
  
### 설치

```bash
dotnet add package prometheus-net.DotNetRuntime
```

##### Program.cs에서 활성화

```csharp
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

// .NET 런타임 메트릭 등록 (GC, 스레드 풀 등)
DotNetRuntimeStatsBuilder.Default().StartCollecting();
/*
// 원하는 정보만 모니터링
IDisposable collector = DotNetRuntimeStatsBuilder
    .Customize()
    .WithContentionStats()
    .WithJitStats()
    .WithThreadPoolStats()
    .WithGcStats()
    .WithExceptionStats()
    .StartCollecting();
*/


app.UseRouting();

// HTTP 요청 메트릭
app.UseHttpMetrics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.Run();
```
  
#### 3) 수집되는 주요 런타임 메트릭 예시

* **GC 관련**
  * `dotnet_gc_collections_total` (세대별 GC 횟수)
  * `dotnet_gc_heap_size_bytes` (힙 크기)
  * `dotnet_gc_gen0_collections_total`, `gen1`, `gen2`
* **ThreadPool 관련**
  * `dotnet_threadpool_threads_total`
  * `dotnet_threadpool_queue_length`
* **JIT, Lock 등**
  * `dotnet_jit_methods_total`
  * `dotnet_contention_total`
  

#### 4) 특정 메트릭만 수집하고 싶을 때
Prometheus는 “pull + filter” 방식이라, 서버 측에서 노출을 막기보다는 **Prometheus 쪽에서 선택적으로 가져오는 방식**을 쓴다.  

##### (1) Prometheus 설정에서 특정 메트릭만 스크랩
  
```yaml
scrape_configs:
  - job_name: 'aspnetcore-api'
    static_configs:
      - targets: ['localhost:5000']
    metric_relabel_configs:
      - source_labels: [__name__]
        regex: "dotnet_gc_.*"
        action: keep
```
  
→ 이렇게 하면 `dotnet_gc_`로 시작하는 메트릭만 저장된다.  
  
##### (2) 코드에서 직접 필터링
`prometheus-net` 자체는 개별 메트릭 노출을 끄는 옵션이 없으므로, 특정 수치를 원치 않으면:

* 커스텀 미들웨어로 제한된 `/metrics` 엔드포인트 구현
* 또는 `metric_relabel_configs` 쪽에서 필터링하는 게 일반적이다.

  
#### 5) Grafana에서 활용

* `dotnet_gc_collections_total` → GC 빈도 추적
* `dotnet_threadpool_queue_length` → 대기 작업 적체 확인
* `dotnet_threadpool_threads_total` → 동시 처리 리소스 확인



### HTTP 요청 관련 메트릭 쿼리
ASP.NET Core Web API + `prometheus-net.AspNetCore` 

#### 1) 기본적으로 수집되는 HTTP 메트릭
`app.UseHttpMetrics()`를 쓰면 아래와 같은 메트릭이 자동 노출된다:  

* **총 요청 수**
  `http_requests_received_total{method="GET",code="200"}`
* **진행 중 요청 수**
  `http_requests_in_progress`
* **요청 처리 시간** (히스토그램)
  `http_request_duration_seconds_bucket`
  `http_request_duration_seconds_sum`
  `http_request_duration_seconds_count`


#### 2) PromQL 쿼리 예시

##### (1) 초당 요청 수 (QPS)

```promql
rate(http_requests_received_total[5m])
```

* 5분 구간 이동 평균 요청 속도
  
##### (2) 상태 코드별 요청 비율

```promql
sum(rate(http_requests_received_total[5m])) by (code)
```

* 200/400/500 코드별 요청 비율 확인 가능

##### (3) 평균 응답 시간

```promql
rate(http_request_duration_seconds_sum[5m])
/
rate(http_request_duration_seconds_count[5m])
```

* 요청당 평균 처리 시간 (초 단위)

##### (4) P90 응답 시간 (느린 요청 감지)

```promql
histogram_quantile(0.9, rate(http_request_duration_seconds_bucket[5m]))
```

* 응답 시간 분포에서 90% 구간 추정치

##### (5) 현재 처리 중인 요청 수

```promql
http_requests_in_progress
```
  

#### 3) Prometheus 웹 UI에서 실행 방법
1. 브라우저에서 `http://localhost:9090` 접속
2. 상단 탭에서 **Graph/Explore** 선택
3. 위 PromQL 쿼리 입력 후 **Execute**
4. Graph 버튼으로 그래프 확인 가능

  
#### 4) 응용 팁

* 특정 메서드만 보고 싶으면:

  ```promql
  rate(http_requests_received_total{method="POST"}[5m])
  ```
* 특정 경로만 보고 싶으면 (endpoint 라벨이 있을 때):

  ```promql
  rate(http_requests_received_total{route="/api/orders"}[5m])
  ```
   

### 지정한  **job_name**에만 쿼리 할 때
**라벨 필터(label filter)** 를 쓴다.  

#### 1) Prometheus 라벨 구조 복습
Prometheus는 수집할 때 자동으로 몇 가지 라벨을 붙여준다:

* `job` : `prometheus.yml`에서 정의한 `job_name`
* `instance` : `target` (예: `localhost:9182`)
* 그 외 exporter가 노출한 라벨들 (method, code, path 등)
  
즉, 쿼리할 때 `job="windows"` 처럼 필터링할 수 있다.  

#### 2) 쿼리 예시

##### (1) 특정 job의 모든 메트릭 가져오기

```promql
http_requests_received_total{job="aspnetcore-api"}
```

##### (2) job 단위로 요청 수 비교

```promql
sum(rate(http_requests_received_total[5m])) by (job)
```

→ 여러 job이 있을 때 job별 요청률을 한눈에 비교

##### (3) 특정 job + 특정 instance

```promql
rate(http_requests_received_total{job="aspnetcore-api",instance="localhost:5000"}[5m])
```

##### (4) 특정 job에서만 응답 시간 P90 구하기

```promql
histogram_quantile(
  0.9,
  sum(rate(http_request_duration_seconds_bucket{job="aspnetcore-api"}[5m])) by (le)
)
```
  
  
#### 3) 실전 팁

* **job 단위 대시보드**: Grafana 패널에서 변수(`$job`)를 만들어 두면 job 선택 드롭다운으로 전환 가능
* **비교용**: `by(job)` 집계를 쓰면 여러 job을 동시에 비교할 수 있습니다
* **필터 조합**: `{job="aspnetcore-api", method="POST"}` 같이 조합 가능



### 특정 http request를 Count하기
- 기본적으로 PromQL `http_request_duration_seconds_count`을 통해 모든 http request를 모니터링 할 수 있지만 특정 http request만을 모니터링 할 수 있도록 Counter 메트릭을 생성할 수 있다.  
  
```
namespace ApiServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAccountDb _accountDb;
        private readonly ILogger<LoginController> _logger;
        private readonly IRedisDb _redisDb;
         
        // 프로메테우스 Counter 측정 항목 설명
        private static readonly Counter _LoginCounter = Metrics.CreateCounter("API_Server_LoginCounter", "API_Server_LoginCounter");
         
        public LoginController(ILogger<LoginController> logger, IAccountDb accountDb, IRedisDb redisDb)
        {
            _accountDb = accountDb;
            _logger = logger;
            _redisDb = redisDb;
        }
 
        [HttpPost]
        public async Task<LoginResponse> LoginPost(LoginRequest request)
        {
            //...
 
            // 프로메테우스 카운터 증가
            _LoginCounter.Inc();
             
            return response;
        }
    }
}
```
  
프로메테우스에서 "API_Server_LoginCounter" 쿼리를 통해 모니터링 할 수 있다.


## TCP 소켓 서버
핵심은 **“서버 로직은 TCP 소켓으로 처리하면서, 별도로 Prometheus가 가져갈 수 있는 `/metrics` HTTP 엔드포인트를 열어주는 것”** 이다.

### 1) NuGet 패키지 설치
Prometheus용 라이브러리 **prometheus-net**을 씁니다.

```bash
dotnet add package prometheus-net
dotnet add package prometheus-net.AspNetCore
```  
 
`prometheus-net.AspNetCore` 은 `KestrelMetricServer`을 위해 설치한다.  


### TCP 서버 코드 예시

아주 단순한 TCP 서버 (비동기 echo 서버) 예제이다.

```csharp
using System.Net;
using System.Net.Sockets;
using System.Text;
using Prometheus;

class Program
{
    // Prometheus Counter 메트릭
    private static readonly Counter TcpRequestsTotal =
        Metrics.CreateCounter("tcp_requests_total", "Total number of TCP requests handled.");

    static async Task Main(string[] args)
    {
        // 1) Prometheus 전용 HTTP 서버 시작 (/metrics 노출)
        // 포트 1234에서 metrics 엔드포인트 제공
        var metricServer = new KestrelMetricServer(port: 1234);
        metricServer.Start();

        // 2) TCP 서버 시작
        var listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();
        Console.WriteLine("TCP Server listening on port 9000. Prometheus on :1234/metrics");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        var buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Received: {received}");

        // 3) 요청 카운터 증가
        TcpRequestsTotal.Inc();

        // 에코 응답
        byte[] response = Encoding.UTF8.GetBytes($"Echo: {received}");
        await stream.WriteAsync(response, 0, response.Length);
    }
}
```

### 확인
1. TCP 서버: `nc localhost 9000` → 메시지 보내면 echo 응답  
2. Prometheus 메트릭: `http://localhost:1234/metrics` 접속 →  
  
   ```
   # HELP tcp_requests_total Total number of TCP requests handled.
   # TYPE tcp_requests_total counter
   tcp_requests_total 5
   ```
  
### Prometheus 설정 (prometheus.yml)

```yaml
scrape_configs:
  - job_name: 'tcp-server'
    static_configs:
      - targets: ['localhost:1234']
```

### 확장 아이디어

* 연결 수 추적 (Gauge):

  ```csharp
  private static readonly Gauge ActiveConnections =
      Metrics.CreateGauge("tcp_active_connections", "Number of active TCP connections.");
  ```

  → 클라이언트 연결 시 `.Inc()`, 끊을 때 `.Dec()`.

* 처리 시간 측정 (Histogram):

  ```csharp
  private static readonly Histogram RequestDuration =
      Metrics.CreateHistogram("tcp_request_duration_seconds", "TCP request handling time.");

  using (RequestDuration.NewTimer())
  {
      // 요청 처리 코드
  }
  ```

### ✅ 정리

* **prometheus-net** 라이브러리의 `KestrelMetricServer`로 `/metrics` HTTP 엔드포인트를 열고,
* TCP 서버 로직에서 Counter/Gauge/Histogram 같은 메트릭을 업데이트하면 됩니다.
* Prometheus가 해당 포트를 스크랩해서 모니터링할 수 있습니다.


### TCP 서버 자체 상태(연결/요청) + 애플리케이션 로직(메시지 처리량, 큐 길이 등)
  
#### 1) 기본 구성
1. **TCP 서버 로직**은 그대로 유지 (예: `TcpListener`)
2. **KestrelMetricServer**를 별도로 띄워 `/metrics` HTTP 엔드포인트 제공
3. TCP 서버에서 이벤트가 발생할 때마다 Prometheus 메트릭을 업데이트

즉,  
* TCP 통신 = 비즈니스 로직
* KestrelMetricServer = 모니터링 HTTP 엔드포인트

#### 2) 추적할 메트릭 종류

🔹 TCP 서버 상태  
  
* **활성 연결 수 (Gauge)**

  ```csharp
  private static readonly Gauge ActiveConnections =
      Metrics.CreateGauge("tcp_active_connections", "Number of active TCP connections.");
  ```

* **총 처리 요청 수 (Counter)**

  ```csharp
  private static readonly Counter RequestsTotal =
      Metrics.CreateCounter("tcp_requests_total", "Total number of TCP requests handled.");
  ```

* **요청 처리 시간 (Histogram)**

  ```csharp
  private static readonly Histogram RequestDuration =
      Metrics.CreateHistogram("tcp_request_duration_seconds", "Request processing time in seconds.");
  ```
  
🔹 애플리케이션 로직 상태

* **메시지 처리량 (Counter)**
  → 메시지를 받을 때마다 `MessagesTotal.Inc()`

* **큐 길이 (Gauge)**
  → 메시지 큐에 push → `.Inc()`, 처리 시 → `.Dec()`

* **에러 발생 수 (Counter)**

  ```csharp
  private static readonly Counter ErrorsTotal =
      Metrics.CreateCounter("tcp_errors_total", "Number of errors during request processing.");
  ```

#### 3) 코드 예시 (간단 버전)

```csharp
using System.Net;
using System.Net.Sockets;
using System.Text;
using Prometheus;

class Program
{
    private static readonly Gauge ActiveConnections =
        Metrics.CreateGauge("tcp_active_connections", "Number of active TCP connections.");

    private static readonly Counter RequestsTotal =
        Metrics.CreateCounter("tcp_requests_total", "Total number of TCP requests handled.");

    private static readonly Histogram RequestDuration =
        Metrics.CreateHistogram("tcp_request_duration_seconds", "TCP request handling time.");

    private static readonly Gauge QueueLength =
        Metrics.CreateGauge("tcp_message_queue_length", "Messages waiting in queue.");

    static async Task Main(string[] args)
    {
        // Prometheus metrics endpoint (http://localhost:1234/metrics)
        var metricServer = new KestrelMetricServer(port: 1234);
        metricServer.Start();

        var listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();
        Console.WriteLine("TCP Server listening on port 9000");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        ActiveConnections.Inc();
        try
        {
            using (client)
            {
                var buffer = new byte[1024];
                using (var timer = RequestDuration.NewTimer())
                {
                    var stream = client.GetStream();
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    RequestsTotal.Inc();
                    QueueLength.Inc();   // 메시지 큐에 들어감

                    // 처리 로직 (예: echo)
                    byte[] response = Encoding.UTF8.GetBytes($"Echo: {msg}");
                    await stream.WriteAsync(response, 0, response.Length);

                    QueueLength.Dec();   // 처리 완료
                }
            }
        }
        catch
        {
            // 에러 카운터 올리기 가능
        }
        finally
        {
            ActiveConnections.Dec();
        }
    }
}
```
  
#### 4) Prometheus 설정

```yaml
scrape_configs:
  - job_name: 'tcp-server'
    static_configs:
      - targets: ['localhost:1234']
```
  
#### 5) Grafana에서 활용

* **tcp\_active\_connections** → 현재 연결 수 모니터링
* **rate(tcp\_requests\_total\[5m])** → 초당 요청 처리량(QPS)
* **histogram\_quantile(0.9, rate(tcp\_request\_duration\_seconds\_bucket\[5m]))** → 90% 응답 시간
* **tcp\_message\_queue\_length** → 메시지 적체 여부 확인


#### ✅ 요약

* **TCP 레벨 상태**: 연결 수(Gauge), 요청 수(Counter), 처리 시간(Histogram)
* **애플리케이션 로직**: 메시지 처리량(Counter), 큐 길이(Gauge), 에러 수(Counter)
* Prometheus는 `/metrics` 엔드포인트에서 수집 → Grafana로 시각화
  
 
### 전체 단위 메트릭과 클라이언트별 라벨 메트릭
  
#### 🔹 1. 서버 전체 단위 메트릭
**정의**: 서버 전체 상태를 하나의 수치로 집계 → “서버가 얼마나 바쁘냐”를 보는 용도.

* 예시 메트릭:

  * 활성 연결 수

    ```csharp
    private static readonly Gauge ActiveConnections =
        Metrics.CreateGauge("tcp_active_connections_total", "Active TCP connections.");
    ```

    * 클라이언트가 접속하면 `ActiveConnections.Inc();`
    * 끊어지면 `ActiveConnections.Dec();`
  * 총 요청 수

    ```csharp
    private static readonly Counter RequestsTotal =
        Metrics.CreateCounter("tcp_requests_total", "Total handled TCP requests.");
    ```

    * 요청 처리할 때마다 `RequestsTotal.Inc();`
  * 평균 처리 시간 (히스토그램)

    ```csharp
    private static readonly Histogram RequestDuration =
        Metrics.CreateHistogram("tcp_request_duration_seconds", "Request processing time.");
    ```

👉 장점:

* 데이터 개수가 적어 **가볍고 빠름**
* 전체 부하 추세 파악에 적합 (Grafana 대시보드에서 한눈에 보기 좋음)

👉 단점:

* 어떤 클라이언트(IP)가 문제를 일으키는지 파악하기 어려움
  
#### 🔹 2. 클라이언트별 라벨 메트릭
**정의**: 메트릭에 `client_ip` 같은 라벨을 붙여서, 특정 클라이언트 단위로 상세 모니터링.

* 예시 코드:

  ```csharp
  private static readonly Counter RequestsByClient =
      Metrics.CreateCounter("tcp_requests_by_client_total",
          "Total requests per client.",
          new CounterConfiguration
          {
              LabelNames = new[] { "client_ip" }
          });

  private static readonly Gauge ActiveConnectionsByClient =
      Metrics.CreateGauge("tcp_active_connections_by_client",
          "Active connections per client.",
          new GaugeConfiguration
          {
              LabelNames = new[] { "client_ip" }
          });
  ```

* 사용 예시:

  ```csharp
  string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
  RequestsByClient.WithLabels(clientIp).Inc();
  ActiveConnectionsByClient.WithLabels(clientIp).Inc();
  ```

👉 장점:
* **문제 클라이언트 추적 가능** (예: 특정 IP가 요청을 과도하게 보낼 때)
* 트래픽 분포, Top N 클라이언트 분석 가능

👉 단점:
* 클라이언트가 많아질수록 **라벨 조합 폭발(label cardinality)** 문제가 생김 → Prometheus 성능 저하
* 수천 개 이상의 클라이언트 IP를 모두 저장하면 메모리/스토리지 부담
  
#### 🔹 3. 운영 시 고려
* **전체 단위 메트릭**은 항상 필수 → 서버 전체 부하/상태 감지용
* **클라이언트별 라벨 메트릭**은 선택적 →

  * 내부 서비스처럼 클라이언트 수가 제한적일 때 유용
  * 외부 불특정 다수 클라이언트가 접속하는 서버라면 위험 (메트릭 폭발)

👉 그래서 보통은:

1. 전체 단위 메트릭 = Prometheus 기본 수집
2. 클라이언트별 메트릭 =

   * 샘플링해서 저장
   * Top-N 클라이언트만 추적
   * 또는 로그 기반 분석 툴(ELK, Loki 등)과 병행
  
#### 🔹 4. PromQL 예시

* 전체 요청률:

  ```promql
  rate(tcp_requests_total[5m])
  ```
* 클라이언트별 요청률:

  ```promql
  rate(tcp_requests_by_client_total[5m]) by (client_ip)
  ```
* 특정 클라이언트(IP=192.168.0.10)의 연결 수:

  ```promql
  tcp_active_connections_by_client{client_ip="192.168.0.10"}
  ```

#### ✅ **정리**

* **전체 단위 메트릭** → 항상 안정적, 서버 상태를 빠르게 알 수 있음
* **클라이언트별 라벨 메트릭** → 상세 분석에 유용하지만 라벨 폭발 주의
* 따라서 운영에서는 두 가지를 **병행**하되, 클라이언트별 메트릭은 **제한적/샘플링**해서 쓰는 게 안전합니다.
  
   

### JIT, GC, 예외(Exception) 메트릭을 수집
  
#### 🔹 1. NuGet 패키지 추가
  
```bash
dotnet add package prometheus-net.DotNetRuntime
```
  
* [`prometheus-net.DotNetRuntime`](https://github.com/djluck/prometheus-net.DotNetRuntime) 은 CLR 이벤트를 구독해서 **GC, JIT, ThreadPool, Exception** 같은 런타임 메트릭을 자동으로 노출한다.
* 기존 `prometheus-net`과 호환되며 `/metrics` 엔드포인트에 추가된다.

  
#### 🔹 2. Program.cs 예시 (소켓 서버 + 런타임 메트릭)
 
```csharp
using Prometheus;

class Program
{
    static async Task Main(string[] args)
    {
        // 1) .NET 런타임 메트릭 수집 시작 (GC, JIT, Exception 등)
        DotNetRuntimeStatsBuilder.Default().StartCollecting();

        // 2) Prometheus metrics endpoint 열기 (예: http://localhost:1234/metrics)
        var metricServer = new KestrelMetricServer(port: 1234);
        metricServer.Start();

        Console.WriteLine("Socket server with Prometheus metrics running...");

        // 3) TCP 서버 실행 로직
        var listener = new TcpListener(System.Net.IPAddress.Any, 9000);
        listener.Start();
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string received = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine($"Received: {received}");

            var response = System.Text.Encoding.UTF8.GetBytes($"Echo: {received}");
            await stream.WriteAsync(response, 0, response.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            // 예외 발생 수는 DotNetRuntimeStatsBuilder가 자동 수집
        }
    }
}
```

  
#### 🔹 3. 수집되는 주요 메트릭

##### GC

* `dotnet_gc_collections_total{generation="0"}`
* `dotnet_gc_heap_size_bytes`
* `dotnet_gc_time_seconds_total`

##### JIT

* `dotnet_jit_methods_total`
* `dotnet_jit_time_seconds_total`

##### Exception

* `dotnet_exceptions_total{type="System.InvalidOperationException"}`

##### ThreadPool

* `dotnet_threadpool_threads_total`
* `dotnet_threadpool_queue_length`

 
#### 🔹 4. Prometheus 설정
  
```yaml
scrape_configs:
  - job_name: 'socket-server'
    static_configs:
      - targets: ['localhost:1234']
```
  
#### 🔹 5. 주의할 점
* 런타임 메트릭은 **애플리케이션 성능 특성**을 볼 때 유용하지만, 너무 잦은 스크랩은 Prometheus 부담 → 보통 `scrape_interval: 15s` 정도가 적당합니다.
* Exception 메트릭은 **발생한 예외 타입별로 라벨**이 붙는데,

  * 예외 타입이 너무 다양하면 라벨 카디널리티 문제 가능성 → 운영에서는 주요 예외만 잡히도록 조율 필요
  
  
#### ✅ 정리

* **prometheus-net.DotNetRuntime**을 쓰면 소켓 서버에서도 **GC, JIT, Exception, ThreadPool** 메트릭을 자동 수집 가능
* `/metrics` 엔드포인트에 자동 추가 → Prometheus에서 그대로 스크랩
* Exception 라벨 폭발 문제만 주의

  

## 매트릭 지표 

### 🔹 1. GC 관련 메트릭

| 메트릭                                               | 의미            | 활용                                                             |
| ------------------------------------------------- | ------------- | -------------------------------------------------------------- |
| `dotnet_gc_collections_total{generation="0/1/2"}` | 세대별 GC 발생 횟수  | - **Gen0**가 많으면 일시적 객체 생성이 많음<br>- **Gen2**가 자주 발생하면 메모리 압박 크다 |
| `dotnet_gc_heap_size_bytes`                       | 힙 크기          | - 전체 메모리 사용량 모니터링<br>- GC 직후에도 줄지 않으면 **대형 객체/메모리 누수** 가능성     |
| `dotnet_gc_time_seconds_total`                    | GC에 소비된 누적 시간 | - CPU를 GC에 쓰고 있는 시간 비율 확인<br>- 서비스 성능 저하 원인 진단에 도움             |
| `dotnet_gc_committed_memory_bytes`                | GC 커밋된 메모리    | - 메모리 압박 추세 확인 (실제 OS 메모리 사용량 반영)                              |

👉 **튜닝 포인트**

* Gen2/LOH 컬렉션이 잦으면 → **객체 생명주기 관리/메모리 풀링** 고려
* `gcServer` 모드(`runtimeconfig.json`) 켜서 서버용 GC로 바꾸면 멀티코어 환경에서 효율↑


### 🔹 2. JIT 메트릭

| 메트릭                             | 의미             | 활용                                                             |
| ------------------------------- | -------------- | -------------------------------------------------------------- |
| `dotnet_jit_methods_total`      | JIT 컴파일된 메서드 수 | 서버가 오래 켜져 있는데 값이 계속 증가 → **Dynamic Code Generation** 과다 사용 가능성 |
| `dotnet_jit_time_seconds_total` | JIT에 소요된 시간    | 기동 직후 높다가 안정되는 게 정상                                            |

👉 **튜닝 포인트**

* 성능 민감한 경우 **ReadyToRun(R2R) 빌드** 또는 **Tiered Compilation 최적화** 사용


### 🔹 3. 예외 메트릭 (성능 측면)

* `dotnet_exceptions_total{type="..."}`
* 예외는 발생 시마다 스택 트레이스 수집으로 **비용이 크다**
* 특정 타입 예외가 빈번하면 try-catch 로직 개선 or validation 사전 체크 필요

### 🔹 4. ThreadPool / 대기열 메트릭

| 메트릭                               | 의미                 | 활용                                       |
| --------------------------------- | ------------------ | ---------------------------------------- |
| `dotnet_threadpool_threads_total` | ThreadPool 총 쓰레드 수 | 증가 추세 → 요청량 급증 or 작업이 블로킹됨               |
| `dotnet_threadpool_queue_length`  | 대기 중인 작업 개수        | 값이 계속 높음 → ThreadPool이 backlog 처리 못하고 있음 |

👉 **튜닝 포인트**

* I/O 작업은 async/await로 처리해 ThreadPool 점유 최소화
* CPU bound 작업은 `Task.Run` 대신 별도 `System.Threading.Channels` / 전용 워커 스레드 고려

### 🔹 5. PromQL 예시 (Grafana 대시보드에서 활용)

* **GC 비율 (전체 CPU 대비 GC 시간)**

  ```promql
  rate(dotnet_gc_time_seconds_total[5m]) 
  / rate(process_cpu_seconds_total[5m]) * 100
  ```

* **세대별 GC 발생률**

  ```promql
  rate(dotnet_gc_collections_total{generation="2"}[5m])
  ```

* **메모리 사용량 추세**

  ```promql
  dotnet_gc_heap_size_bytes
  ```

* **스레드풀 대기열 모니터링**

  ```promql
  dotnet_threadpool_queue_length
  ```

  
### ✅ 정리
성능 최적화(메모리/GC 튜닝)에서는 아래 메트릭을 중점적으로 봐야 한다:

* GC: **세대별 컬렉션 빈도, 힙 크기, GC 시간 비율**
* JIT: **JIT 소요 시간**, 장기적으로 **동적 코드 증가 여부**
* Exception: **빈번한 예외 발생** → 성능 손실 원인
* ThreadPool: **대기열 길이, 스레드 수 변화**

👉 운영에서는 **Grafana 대시보드**를 만들어 “GC 동작 패턴 + 메모리 사용 + ThreadPool 상태”를 한눈에 볼 수 있게 하는 게 베스트 프랙티스이다.

  

## 새로운 서버가 증가할 때
Prometheus를 직접 써보면 제일 불편한 게 **“새 서버가 늘어날 때마다 prometheus.yml을 수정 → 서버 재시작”** 부분이다.
이를 해결하기 위해 Prometheus는 **Service Discovery(서비스 디스커버리)** 기능을 지원한다.  


### 🔹 1. Service Discovery (정석 방법)
Prometheus는 여러 환경에 맞는 디스커버리를 내장하고 있다:

* **Kubernetes**: `kubernetes_sd_configs`
  → 새 Pod/Service가 생기면 자동으로 타깃 추가
* **Consul**: `consul_sd_configs`
  → Consul에 등록된 서비스 목록 자동 감지
* **EC2, GCP, Azure, OpenStack**: 클라우드 VM 자동 등록
* **Docker Swarm / ECS**: 컨테이너 기반 디스커버리

👉 운영 환경이 위 중 하나라면 “새 서버 추가 → Prometheus 자동 인지”가 가능하다.

### 🔹 2. File-based Service Discovery (가장 쉬운 방법)
직접 yaml을 수정해서 Prometheus를 재시작하는 대신, **외부 파일 하나만 갱신**하면 Prometheus가 자동 반영하게 만들 수 있다.

#### `prometheus.yml`

```yaml
scrape_configs:
  - job_name: 'my-servers'
    file_sd_configs:
      - files:
        - targets.json
```

#### `targets.json`

```json
[
  {
    "targets": ["server1:9182", "server2:9182"],
    "labels": {
      "env": "prod"
    }
  }
]
```

➡️ Prometheus는 `targets.json` 파일을 주기적으로 다시 읽는다.
즉, 새 서버가 늘어나면 JSON 파일만 수정하면 되고 **Prometheus 재시작이 필요 없다**.

### 🔹 3. Pushgateway (임시/배치 잡에 적합)
만약 서버 수가 들쭉날쭉하거나, 짧게 돌았다가 사라지는 잡(Job)이라면 →
Prometheus가 일일이 discovery 하기 어려우므로 **Pushgateway**를 써서 서버 쪽에서 직접 메트릭을 push하도록 만들 수 있다.

하지만 Pushgateway는 “항상 떠 있는 서버” 모니터링에는 권장되지 않고, **배치 작업이나 임시 잡**에 적합하다.
  
### 🔹 4. Service Discovery + Service Registry 조합
규모가 커지면 보통:

* **Consul / Etcd / ZooKeeper** 같은 서비스 레지스트리
* 또는 **Kubernetes / ECS / Docker Swarm** 같은 오케스트레이터

→ Prometheus가 여기랑 연동해서 자동으로 타깃 추가/삭제
  
### ✅ 요약
* 지금처럼 **직접 yaml 수정 + 재시작** → 불편하고 확장성 없음
* **가장 쉬운 개선책** → `file_sd_configs` + JSON 파일 관리 (재시작 필요 없음)
* **운영 환경에 따라 최적**

  * Kubernetes → `kubernetes_sd_configs`
  * Consul → `consul_sd_configs`
  * 클라우드 환경 → EC2/GCP/Azure 디스커버리
  
  
## File-based Service Discovery 
온프레미스 환경에서는 보통 **서버 IP나 호스트 이름이 고정**되어 있고, Kubernetes 같은 자동 디스커버리가 없으니 **File-based Service Discovery** 방식이 가장 적합하다.  
  
### 🔹 1. 기본 prometheus.yml

```yaml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'onprem-servers'
    file_sd_configs:
      - files:
          - targets.json   # 별도의 파일에서 서버 목록을 불러옴
```

여기서 `targets.json`만 관리하면 Prometheus를 재시작하지 않고도 서버를 추가/삭제할 수 있다.

### 🔹 2. targets.json 예시

```json
[
  {
    "targets": ["192.168.10.11:9182", "192.168.10.12:9182"],
    "labels": {
      "env": "prod",
      "role": "app"
    }
  },
  {
    "targets": ["192.168.20.21:9182"],
    "labels": {
      "env": "staging",
      "role": "db"
    }
  }
]
```

* `"targets"`: Prometheus가 스크랩할 서버 목록 (IP:포트)
* `"labels"`: 라벨 추가 (Grafana 대시보드나 쿼리에서 환경/역할 구분 가능)

### 🔹 3. 동작 방식
* Prometheus는 `targets.json`을 **몇 초마다 자동 재로드**합니다.
* 새 서버를 추가하려면 JSON에 IP만 넣고 저장하면 됩니다.
* Prometheus 자체를 재시작할 필요가 없습니다.

### 🔹 4. 확장 아이디어

* **자동 생성 스크립트**:
  새 서버가 추가될 때 Ansible, Chef, Puppet 같은 배포 툴이 `targets.json`을 업데이트하도록 자동화할 수 있습니다.
* **DNS 서비스 디스커버리**:
  온프레미스라도 `A 레코드`나 `SRV 레코드`를 잘 관리하면 `dns_sd_configs`를 써서 자동 발견도 가능합니다.

예:

```yaml
scrape_configs:
  - job_name: 'onprem-dns'
    dns_sd_configs:
      - names: ['exporters.mycompany.local']
        type: 'A'
        port: 9182
```

### ✅ 정리

* **온프레미스 서버**에서는 `file_sd_configs` + `targets.json` 방식이 가장 현실적
* 서버가 늘어나면 JSON만 수정 → Prometheus는 자동 반영
* 더 고급 환경이면 **DNS 기반 디스커버리**나 **배포 자동화 툴**과 연계

  

