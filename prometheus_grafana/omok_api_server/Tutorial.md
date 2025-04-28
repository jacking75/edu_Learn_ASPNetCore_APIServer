# Prometheus & Grafana 모니터링 실습 튜토리얼

## 목차

### [머신 상태 모니터링](#머신-상태-모니터링)
- [가용 메모리 사용률](#가용-메모리-사용률)
- [평균 cpu 사용률](#평균-cpu-사용률)
- [누적 cpu 사용률](#누적-cpu-사용률)
- [디스크 용량 사용률](#디스크-용량-사용률)
- [네트워크 사용률](#네트워크-사용률)
- [CPU 이용률](#CPU-이용률)

- 
### [API 서버의 초당 요청 수 표시](#api-서버의-초당-요청-수-표시-1)
- [최근 1분간 1초마다 들어온 HTTP 요청의 평균 개수](#최근-1분간-1초마다-들어온-HTTP-요청의-평균-개수)
- [최근 5분간 엔드포인트 별 HTTP 요청 평균 처리 시간(초)](#최근-5분간-엔드포인트-별-HTTP-요청-평균-처리-시간(초))
- [특정 API 누적 요청 수](#특정-API-누적-요청-수)
- [특정 API 초당 요청 수](#특정-API-초당-요청-수)
- [ASP.NET core 서버 내 Gauge metric을 통해 특정 API 모니터링하기](#aspnet-core-내-gauge-metric-활용해서-특정-api-모니터링하기)


### [C#에서의 GC 정보](#GC-정보)
- [애플리케이션 내 Garbage Collection 총 수행 횟수](#애플리케이션-내-Garbage-Collection-총-수행-횟수)
- [세대별 GC 수행 횟수](#세대별-GC-수행-횟수)
- [세대별 GC 평균 수집 횟수](#세대별-GC-평균-수집-횟수)


### [Prometheus 총정리 with C# asp.net core](#prometheus-총정리-with-c-aspnet-core-1)
- [Prometheus Metrics type](#prometheus-metrics-type)
- [ASP.NET Core에서 Prometheus 사용하기](#aspnet-core에서-prometheus-사용하기)

 
---


## 머신 상태 모니터링

프로메테우스에서 머신 상태 모니터링 시 node_exporter 사용하여 실행 중인 서버의 cpu, 메모리, 디스크 사용률과 같은 지표를 수집할 수 있다.

아래는 머신 상태를 모니터링 하고 기본적으로 http 요청을 처리하는 사용 사례이다.


### 가용 메모리 사용률

#### 가용 가능한 메모리 확인
```bash
free -m
```

#### promql을 통해 Grafana에서 데이터 시각화

1. 가용하고있는 메모리 확인
```promql
node_memory_Active_bytes / node_memory_MemTotal_bytes
```

2. 가용한 것을 제외한, 사용 가능한 메모리 확인
```promql
(1 - (node_memory_Active_bytes / node_memory_MemTotal_bytes)) * 100
```


### 평균 cpu 사용률

```promql
100 - (avg by (cpu) (irate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)
```

cpu 평균 사용률을 나타낸다.
- avg by (cpu) : cpu에 대한 평균
- node_cpu_seconds_total{mode="idle"}[5m] : 5m 간 mode가 idle 상태인 cpu 누적 사용
- 전체 - 쉬고있는 cpu 사용률 : 현재 이용률

나의 경우 local 머신에서 돌리고 있어서, cpu가 인텔 코어 i7 12700F 라서 물리적인 코어 수는 12개, 총 스레드 수는 20개 이다.

그래프를 보면 0번부터 19번까지의 코어를 확인할 수 있다. (스레드 수)

<details>

<summary>물리적 코어 vs 논리적 코어</summary>

`물리적 코어`와 `논리적 코어(스레드)` 의 개념이 다르기 때문에 

총 20개의 CPU 항목이 보여지는 것이다.

1. **물리적 코어 vs 논리적 코어 (스레드)**
   - **물리적 코어**: 실제 CPU 내의 독립적인 실행 유닛으로, `Intel Core i7 12700F`의 경우 **12개의 물리적 코어**
   - **논리적 코어(스레드)**: 각 물리적 코어가 동시에 실행할 수 있는 논리적 프로세스 유닛. `Intel Core i7 12700F`의 경우 **Hyper-Threading** 기능을 지원하여 각 코어가 2개의 스레드를 실행 가능
	 + 그래서 실제로는 **12개의 물리적 코어가 20개의 논리적 코어**로 나타남

2. **Prometheus와 Node Exporter가 수집하는 CPU 데이터**
   - Prometheus의 `node_cpu_seconds_total` 메트릭은 **논리적 코어 단위**로 데이터를 수집
	- 논리적 코어 수가 총 20개이기 때문에 `cpu=0`부터 `cpu=19`까지 20개의 코어가 각각 표시
   - **각 논리적 코어는 독립적으로 관리**
	- Node Exporter는 이를 기반으로 각 논리적 코어의 `idle`, `user`, `system` 모드에서의 CPU 사용 시간을 수집

3. **실제로 보이는 20개의 CPU 그래프**
   - 시스템에서 **논리적 코어를 기준으로 데이터**를 수집하기 때문에 **각 스레드마다 CPU 사용량**을 따로 표시
	- 논리적 코어의 사용량을 모두 평균 내어 그래프에 표시하는 경우 CPU 사용률이 전체 시스템 수준의 평균으로 나타난다.
   - 따라서 물리적 코어는 12개지만 Hyper-Threading을 통해 20개의 논리적 코어가 표시되며, **각각 독립적인 사용률**을 보일 수 있다.

이로 인해 물리적 코어보다 많은 논리적 코어가 보여지는 것은 정상적인 현상입니다.

</details>


### 누적 cpu 사용률

```promql
node_cpu_seconds_total
```

누적 cpu 사용 시간을 나타낸다.
- Grafana에서는 단위가 K로 확인되어 4K = 4000초로 확인하면 된다.

### 디스크 용량 사용률

윈도우에서의 디스크 용량 사용률

```promql
100 - (windows_logical_disk_free_bytes / windows_logical_disk_size_bytes) * 100
```

리눅스에서의 디스크 용량 사용률

```promql
100 - ((node_filesystem_free_bytes{mountpoint="/"} / node_filesystem_size_bytes{mountpoint="/"}) * 100)
```

```promql
100 - ((node_filesystem_free_bytes{mountpoint="/mnt/c"} / node_filesystem_size_bytes{mountpoint="/mnt/c"}) * 100)
```

디스크 용량 사용률의 의미 : 사용 중인 용량이 전체 디스크 용량 대비 몇퍼센트인지


### 네트워크 사용률

- rate 함수로 5분 동안 초당 바이트 수의 평균을 계산 후
- bps(bit per second)로 변환하기 위해 * 8

#### 네트워크 전송 속도

```promql
rate(node_network_transmit_bytes_total{device="eth0"}[5m]) * 8
```

#### 네트워크 수신 속도

```promql
rate(node_network_receive_bytes_total{device="eth0"}[5m]) * 8
```
 
Prometheus의 웹 UI에서 네트워크 장치 이름을 조회 후 promql 문을 작성하자
1. http://localhost:9090 으로 이동 후 Graph 탭 클릭
2. Expression에 `node_network_transmit_bytes_total` 입력 후 Execute 클릭 후 장치 명 확인

나의 경우 `eth0` 로 명령어를 작성했다.


### CPU 이용률(%)

```promql
100 - (avg by (instance) (irate(windows_cpu_time_total{mode="idle"}[5m])) * 100)
```

```promql
100 - (avg by (instance) (irate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)
```

- `windows_cpu_time_total{mode="idle"}` : 코어 별 CPU 유휴 시간 (컴퓨터 부팅 후 CPU가 일을 하지 않은 누적 시간)
- `irate(windows_cpu_time_total{mode="idle"}[5m])` : 코어 별 CPU 유휴 시간 비율
	
	+ rate() : 주어진 시간에 대해 (현재 값 - 주어진 시간만큼 전의 값) / (현재 시간 - 주어진 시간만큼 전의 시간)으로 계산
	    - 만약 식에서 5분으로 주어졌다면 최근 5분 간 양 끝 점 사이의 차이를 1초 단위로 평균 낸 값이다.

	+ irate() : (현재 값 - 바로 이전의 값) / (현재 시간 - 바로 이전의 시간)으로 계산
	    - 주어진 시간에 관계 없이 현재 데이터와 가장 최근의 데이터 사이의 차이를 1초 단위로 평균 낸 값이다. 
	    - 주어진 시간 값은 irate를 사용할 때 데이터 시간 간격의 최댓값으로써 두 데이터 사이의 시간 간격보다 크다면 어떤 값이든 같은 결과를 반환한다. 
		- 반대로 주어진 시간 값이 두 데이터 사이의 시간 간격보다 작다면 데이터가 없는 것으로 판단한다.

	+ rate()는 그래프의 기울기가 부드럽게 바뀌므로 전체적인 값의 변화를 볼 때 적합하고, 
	  
	+ irate()는 그래프의 기울기가 보다 급격하게 바뀔 수 있어 CPU 또는 메모리 상태와 같이 짧은 시간 내에 값이 변하는 걸 감지해야 할 때 사용된다.
 

- `avg by (instance) (irate(windows_cpu_time_total{mode="idle"}[5m]))` : 인스턴스 별 CPU 유휴 시간 비율 평균



## 참고

프로메테우스에는 target 데이터를 수집하는 주기인 scrap time이 존재한다.

default 값은 1minute이며, `prometheus.yml` 파일에서 scrap_interval 를 수정하면 되는데, 

너무 짧게 (ex. 1ms) 설정하면 문제가 발생할 수 있다.

<br><br>

---

## API 서버의 초당 요청 수 표시

이어서 API 서버의 초당 요청 수를 표시하는 더 자세한 사용 사례이다.

endpoint 이름에 따라 조회하는 예시부터, Gauge를 사용하는 등의 내용을 다루고 있다.


### 최근 1분간 1초마다 들어온 HTTP 요청의 평균 개수

```promql
sum by (endpoint) (rate(http_requests_received_total[1m]))
```

- `http_requests_received_total` : HTTP 요청의 누적 개수
- `rate(http_requests_received_total[1m])` : 최근 1분 간 HTTP 요청 개수 / 60초

endpoint 별로 HTTP 요청 그래프를 확인할 수 있다.

### 최근 5분간 엔드포인트 별 HTTP 요청 평균 처리 시간(초)

```promql
avg by (endpoint) (rate(http_request_duration_seconds_sum[5m]) / rate(http_request_duration_seconds_count[5m]))
```

- `http_request_duration_seconds_sum` : HTTP 요청 처리 누적 시간
- `rate(http_request_duration_seconds_sum[5m])` : 최근 5분 간 1초마다 HTTP 요청을 처리하는 데 소모한 시간
- `(rate(http_request_duration_seconds_sum[5m]) / rate(http_request_duration_seconds_count[5m]))` : 최근 5분 간 HTTP 요청당 평균 처리 시간


### 특정 API 누적 요청 수

Login API의 누적 요청 수를 조회하는 예시이다.

```promql
http_requests_received_total{endpoint="Login"}
```

### 특정 API 초당 요청 수

Login API의 초당 요청 수를 조회하는 예시이다.

```promql
rate(http_requests_received_total{endpoint="Login"}[1m])
```

- endpoint가 Login이면서 시간의 범위는 1m
- 1분간의 누적의 요청 수의 평균을 알기 위해 rate 함수
- [] 값, 즉 간격이 너무 짧다면 metric을 수집할 수 없다.


### ASP.NET core 내 Gauge Metric 활용해서 특정 API 모니터링하기

ASP.net core에서 Prometheus를 사용하여 특정 API의 요청 수를 모니터링하는 방법이다. 
nuget에서 패키지 설치 이후 `using Prometheus;` 을 통해 사용할 수 있다.

아래는 FakeLoginController 내에서 Gauge Metric을 생성하고, FakeLogin API 요청이 들어올 때마다 증가시키는 예시이다.

```csharp

    private static readonly Gauge FakeLoginGauge = Metrics.CreateGauge("game_server_fake_login", "Fake login Metric");

    ...

    [HttpPost]
    public async Task<GameLoginResponse> FakeLogin([FromBody] GameLoginRequest request)
    {
        FakeLoginGauge.Inc();
		...
    }
```

위와 같이 코드를 구성하면 FakeLogin API 요청이 들어올 때마다 FakeLoginGauge가 증가하게 된다.

해당 Endpoint의 metrics를 조회해보면 아래와 같이 쌓인 것을 확인할 수 있다.


Grafana에서도 해당 gauge Metric을 조회할 수 있다.

```promql
game_server_fake_login
sum(game_server_fake_login)
```


<br><br>

---


## GC 정보

### 애플리케이션 내 Garbage Collection 총 수행 횟수

```promql
dotnet_collection_count_total
```
- 0세대, 1세대, 2세대 수집 횟수 확인 가능


### 세대별 GC 수행 횟수

```promql
dotnet_collection_count_total{generation="0"}
```
- generation="0" : 0세대 수집 횟수


### 세대별 GC 평균 수집 횟수

```promql
rate(dotnet_collection_count_total[1m])
```

- dotnet_collection_count_total : 전체 gc 세대 별 누적 횟수
- rate(dotnet_collection_count_total[1m]) : rate 함수를 통해 1분 간의 평균 수집 횟수 확인 가능


<br><br>

---

# Prometheus 총정리 (with C# asp.net core)



## Prometheus Metrics type

1. Counter : 증가하는 값

2. Gauge : 증가, 감소하는 값

3. Histogram : 값의 범위를 나누어 저장

4. Summary : Histogram과 유사하지만 quantile을 사용하여 값의 분포를 저장

<br><br>

---

## ASP.NET Core에서 Prometheus 사용하기

### ASP.NET core 에서의 설정

- nuget에서 "Install-Package prometheus-net.AspNetCore"을 통해 패키지를 설치한다.
- http request를 모니터링하기 위해 "app.UseRoutin()"이후 "app.UseHttpMetrics()"와 "app.UseMetricsServer()"를 추가한다.

### 특정 http request를 Count하기

- 기본적으로 PromQL "http_request_duration_seconds_count"을 통해 모든 http request를 모니터링 할 수 있지만 
- 특정 http request만을 모니터링 할 수 있도록 Counter 메트릭을 생성할 수 있다.

- `API_Server_LoginCounter` 쿼리로 모니터링 할 수 있다.

### 런타임 모니터링
- build, jit, garbage collection(GC), excption, contetion 등의 정보들을 모니터링 한다. 
- nuget에서 prometheus-net.DotNetRuntime 패키지를 설치
- Program.cs 에서 collector를 선언하여 사용한다.

	```csharp
	// default
	IDisposable collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
 
	// 원하는 정보만 모니터링
	IDisposable collector = DotNetRuntimeStatsBuilder
		.Customize()
		.WithContentionStats()
		.WithJitStats()
		.WithThreadPoolStats()
		.WithGcStats()
		.WithExceptionStats()
		.StartCollecting();
	```

### 소켓

- nuget에서 "Install-Package prometheus-net"를 통해 패키지를 설치한다.
- program.cs

	```csharp
	// http://localhost:5002/metrics 프로메테우스 서버
	var prometheusServer = new MetricServer(hostname:"127.0.0.1",port: 5002);
	prometheusServer.Start();
	```


- 런타임 모니터링 : build, jit, garbage collection(GC), excption, contetion 등의 정보

	```csharp
	// default
	IDisposable collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
 
	// 원하는 정보만 모니터링
	IDisposable collector = DotNetRuntimeStatsBuilder
		.Customize()
		.WithGcStats()
		.WithContentionStats()
		.WithThreadPoolStats()
		.WithJitStats()
		.WithExceptionStats()
		.WithSocketStats()
	   .StartCollecting();

   ```

### 서버 리소스

- CPU, Memory, Network 등 하드웨어 정보를 모니터링

- Node exporter (linux)

#### CPU Load

```promql
sum by (mode) (rate(windows_cpu_time_total[5m]))
```

#### Memory Usage

```promql
windows_cs_physical_memory_bytes - windows_os_physical_memory_free_bytes
```
#### Network

```promql
rate(windows_net_bytes_sent_total[5m])
```
#### Disk

```promql
rate(windows_logical_disk_split_ios_total{volume !~"HarddiskVolume.+"}[5m])
```
#### GC (DotNet)

```promql
increase(dotnet_collection_count_total[5m])
```
#### CPU usage (DotNet)

```promql
avg by (instance) (irate(process_cpu_seconds_total[5m]))
```
5분 동안 각 인스턴스의 초당 CPU 사용 시간을 측정하여, 인스턴스별 평균 CPU 사용률을 계산

#### Network (DotNet)

```promql
rate(dotnet_sockets_bytes_sent_total[5m])

rate(dotnet_sockets_bytes_received_total[5m])
```

#### API Server 모니터링 (ASP.NET Core)

```promql
rate(http_request_duration_seconds_count[5m])
```

#### 소켓 Server 모니터링 (.Net Framework)

현재 연결된 소켓 수

```promql
dotnet_sockets_connections_established_incoming_total
```

#### 받고/보내기 트래픽

```promql
rate(dotnet_sockets_bytes_sent_total[5m])

rate(dotnet_sockets_bytes_received_total[5m])
```