# ASP.NET Core Web API 게임 서버 학습 자료 분석

본 문서는 저장소의 다양한 학습 자료를 분석하고, 카테고리별로 정리한 내용입니다. 각 자료의 유용성(★~★★★★)을 표기하여 학습자가 효율적으로 자료를 선택할 수 있도록 돕습니다.


## 참고사항

* ★★★★: 필수 자료
* ★★★: 매우 유용한 자료
* ★★: 유용한 자료
* ★: 선택적 학습 자료

본 분석은 각 학습 자료의 내용, 접근성, 최신성, 그리고 게임 API 서버 개발이라는 목적에 얼마나 부합하는지를 기준으로 평가되었습니다. 학습자의 배경지식과 경험에 따라 자료의 유용성은 달라질 수 있습니다.


## 사용 기술 스택
- **언어**: C#
- **프레임워크**: ASP.NET Core
- **데이터베이스**: MySQL, Redis
- **DB 연결**: SQLKata, Kata Execution (EF Core 사용하지 않음) , Cloud Structures (Redis 라이브러리)
- **로거**: ZLogger 
- **게임 서버 OS**: Linux
- **기타 기술**: Docker, nGrinder, 프로메테우스 & 그라파나
- **클라이언트 테스트**: C# Blazor (후순위)

## 목차
1. [초급 학습자 추천 경로](#초급-학습자-추천-경로)
2. [C# 프로그래밍](#c-프로그래밍 )
3. [ASP.NET Core 기초](#aspnet-core-기초)
4. [ASP.NET Core 고급](#aspnet-core-고급)
5. [데이터베이스 연동 (MySQL)](#데이터베이스-연동)
6. [Redis 활용](#redis-활용)
7. [로깅 및 모니터링](#로깅-및-모니터링)
8. [환경 설정 및 배포](#환경-설정-및-배포)
9. [참고 프로젝트](#참고-프로젝트)
10. [기타 학습 자료](#기타-학습-자료)
11. [LinqPad 관련자료](#linqpad-관련-자료)

## 초급 학습자 추천 경로

ASP.NET Core로 게임 API 서버를 개발하기 위한 학습 경로를 다음과 같이 추천합니다.   
직접 실습해야 할 부분은, 실습을 해보면서 프로젝트를 진행해 나가야 합니다.

0. **게임 서버 기초**
   - [모바일 게임 개발 개요](https://drive.google.com/file/d/1WImt5yn7cpIBm2opZFWMapWW2GX6XB_R/view?usp=sharing ) (★★★) - 모바일 게임 개발 개요
   - [Web서비스의 서버구성과목적](https://docs.google.com/presentation/d/105NPfv7CPfgk0Iw_6vSB_oOavQZpes7-Wit5HuCm7oM/edit?usp=sharing) (★★★★) - 전반적인 서버 구조 이해
   - [전체적인 서버 개발 기초 자료(기존 서버 캠퍼스 인턴 자료)](https://sueshin.tistory.com/category/%EA%B0%9C%EC%9D%B8%EA%B3%B5%EB%B6%80/Web%20API%20%EA%B2%8C%EC%9E%84%20%EC%84%9C%EB%B2%84%20%EA%B3%B5%EB%B6%80) (★★★★) - 개인공부/Web API 게임 서버 공부 부분. 전체적인 서버 개발 기초 자료.   
   한 번 흩어보고 앞으로 배울 내용들에 대해 감을 잡는 것도 좋고, 정독하는 것도 좋다. 

1. **C# 프로그래밍 기초**
   - [(인프런 무료 영상) C# 처음부터 배우기](https://inf.run/bfkW) (★★★★) - C# 기초 학습 
   - [C# 자료구조](http://www.csharpstudy.com/DS/array.aspx ) (★★★)
   - [coding_rule.md](./coding_rule.md) (★★★) - 코딩 규칙 및 가이드라인


2. **ASP.NET Core 기본**
   - [(YOUTUBE) 1~6단계로 ASP.NET Core 기본 실습](https://youtu.be/YTDWXJG1SD8?si=PHz6XvNGy4yU-Sjj) (★★★★) - ASP.NET Core의 기본 개념과 실습. 예제 코드는 `codes` 디렉토리의 `basic2` 디렉토리 안에 있다. 
   - [VSCode 게임 웹서버 환경설정](UsingVSCode.md) (★★★★) - VSCode에서 ASP.NET Core의 환경 설정 및 디버깅 방법
   - [ASP.NET Core Web 시작](https://blog.naver.com/sssang97/223088811266) (★★★★) - 기본 시작 방법 (Visual Studio 기반)
   - [컨트롤러와 route 규칙](https://blog.naver.com/sssang97/223088978577 )  [Request 처리](https://blog.naver.com/sssang97/223089025576 )  [Response 처리](https://blog.naver.com/sssang97/223089045407 )
   - [DAO, DTO, VO 란? 간단한 개념 정리](https://melonicedlatte.com/2021/07/24/231500.html) (★★★) - 데이터 전송 방식 이해
   - [ASP.NET Core 입문 – 미들웨어와 파이프라인 해설](https://docs.google.com/document/d/e/2PACX-1vRsqcyeBi--VYCPwQlhW9LsAyYUKSuuh80_BiSgnNnrfULsZFgz3i_Bj8nGG6dl-Q6NEiKBjhGx2bJ6/pub) (★★★★) - 미들웨어 개념 이해
   - [의존성 주입 방법의 라이프 사이클 - AddTransient, AddScoped, AddSingleton](https://docs.google.com/document/d/e/2PACX-1vRFi_2Z6yMOWNwWfILDXGsbqYS3aJfiO6aO2u22Awy-pQ5XEEz0GpIOjehif47noYsR06jT6z_pD6Mr/pub) (★★★★) - DI 생명주기
   - [API 서버의 디렉토리 구성](APIServer_Directory.md) (★★★★) - ASP.NET Core API 서버 디렉토리 구성 예
   - [API 서버 간 통신 때 HttpClientFactory 사용하기](HttpClientFactory.md) (★★★★) - HttpClientFactory 사용법
   - [MS Docs: ASP.NET Core에서 IHttpClientFactory를 사용하여 HTTP 요청 만들기](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0 ) (★★★★) - HttpClientFactory 사용법

3. **데이터베이스**
   - [MySqlConnector 간단 정리](https://gist.github.com/jacking75/51a1c96f4efa1b7a27030a7410f39bc6) (★★★) - MySQL 연결 방법
   - [DB 프로그래밍 라이브러리 SqlKata 소개](https://docs.google.com/document/d/e/2PACX-1vTnRYJOXyOagNhTdhpkI_xOQX4DlMu0TRcC9Ehew6wraufgEtBuQiSdGpKzaEmRb-jfsLv43i0nBQsp/pub) (★★★★) - 쿼리 빌더 라이브러리
   - [예제 프로그램: github_sqlkata_demo.zip](https://drive.google.com/file/d/1FBpB1zQ84LqGOA9WAJ6vk5S3453ekqDc/view?usp=sharing) (★★★) - SQLKata 예제

4. **Redis 학습**
   - [(영상) Redis 야무지게 사용하기](https://www.youtube.com/watch?v=92NizoBL4uA) (★★★★) - NHN Redis 강의
   - [Redis 기능 학습하기](http://redisgate.kr/redis/introduction/redis_intro.php) (★★★) - 잘 구조화된 Redis 학습 자료. 해당 링크의 글은 읽고, 해당 사이트 다른 글은 필요할 때 찾아보는 것을 추천 
   - [Redis의 기본 데이터 유형 및 명령](https://docs.google.com/document/d/10mHFq-kTpGBk1-id5Z-zoseiLnTKr_T8N3byBZP5mEg/edit?usp=sharing) (★★★★) - Redis 자료형. 익숙한 자료구조들이 있어서, 필요한 데이터 유형을 찾아보는 것을 추천한다.

5. **Redis 라이브러리 CloudStructure 학습**
   - [CloudStructures를 이용한 C# Redis 프로그래밍](https://gist.github.com/jacking75/5f91f8cf975e0bf778508acdf79499c0 ) (★★★) - CloudStructures를 이용한 redis 다루기 입문
   - [CloudStructures 라이브러리 사용 설명서](./redis_CloudStructures_Docs) (★★★★) - C#에서 Redis 사용 

6. **로깅 (ZLogger, SeriLog 중 택 1)**
   - [ZLogger 사용하기](./ZLogger/) (★★★★) - 로그 라이브러리 ZLogger 사용 방법
   - [SeriLog 사용하기](./Serilog.md) (★★★★) - ZLogger 대체 로깅 라이브러리


## C# 프로그래밍 

### 참고 자료
1. [C# 비동기 프로그래밍 정리](https://docs.google.com/document/d/e/2PACX-1vRHRbQjeoJH9lXalTClFBuB-D41v9TaBTPc_TeUS-yKhPZTJa2dWjpv_Rib863b_disjspqymOjgKwq/pub) (★★★) - 비동기 프로그래밍 필수 자료
2. [(인프런 무료 영상) C# 초보 강좌 예제로 배우는 C#](https://inf.run/PVsq) (★★) - 예제 중심 C# 학습
3. [문법](https://www.csharpstudy.com/CSharp/CSharp-intro.aspx) (★★) - C# 기본 문법 학습
4. [(YOUTUBE) VisualAcademy](https://www.youtube.com/@VisualAcademy/videos) (★★) - 다양한 C# 강의 모음
5. [6.0 버전 이후의 새 기능](http://www.csharpstudy.com/Latest/CS-new-features.aspx ) (★)
6. [C# 멀티스레딩](http://www.csharpstudy.com/Threads/thread.aspx ) (★)
7. [(인프런 무료 영상) C# 프로그래밍 - 커리큘럼 총 63 개](https://inf.run/PueZ) (★★) - Advanced한 C# 강의
8. [YOUTUBE 닷넷데브](https://www.youtube.com/c/%EB%8B%B7%EB%84%B7%EB%8D%B0%EB%B8%8C/videos ) (★) - C#개발 관련으로 정말 다양한 자료


## ASP.NET Core 기초

### 필수 자료
1. [ASP.Net Core - DI 시스템 사용하기](https://docs.google.com/document/d/e/2PACX-1vR0NxfIPIQe_CAxf2-yo9B9QB4O0NIlJz3U-oe4iF_sHIpMnJiu_4o3ZlWoPD0kcW9ve9ka49_sLa8u/pub) (★★★) - 의존성 주입
2. [C#(.NET) 이미 빌드 된 API 서버의 엔드 포인트 url을 변경하는 방법](https://docs.google.com/document/d/e/2PACX-1vRhHebXWTa1OOY60NT3T0ZRkr8NMNRIuFADhVZIknSs_YsnlYTz7FObkexo9x1iNfoQID31-hVxiwTB/pub ) (★★★) - appsettings 설정 자료
3. [.NET 6에서 Kestrel 포트를 설정하는 몇 가지(너무 많은?) 방법](https://forum.dotnetdev.kr/t/net-6-kestrel-bryan-hogan/2681 ) (★★★) - appsettings 설정 자료
4. [Visual Studio - launchSettings.json을 이용한 HTTP/HTTPS 포트 바인딩](https://www.sysnet.pe.kr/2/0/13539 ) (★★★) - appsettings 설정 자료
5. [ASP.NET Core 앱에서 URL을 설정하는 5가지 방법](https://docs.google.com/document/d/1x3ZJQtGt2uNW5_xRT6QHrOmZkfbNU2KQ23N5RCLi4cE/edit?usp=sharing ) (★★★) - appsettings 설정 자료


### 참고 자료
1. [닷넷 빌드와 실행](./dotnet_build.md) (★★) - 닷넷 빌드와 실행 시 일어나는 일에 대한 상세 설명
2. [ASP.NET Core](https://learn.microsoft.com/ko-kr/aspnet/core) (★★) - 공식 사이트로 방대한 정보 포함 
3. [ASP.NET Core 애플리케이션 최소 구현 치트 시트](https://jacking75.github.io/csharp_netcore_cheatsheet/) (★★) - 최소 구현을 위한 환경 설정
4. [Microsoft Docs MCP Server](https://github.com/MicrosoftDocs/mcp) (★★)  MS Docs에 있는 ASP.NET Core 관련 기술 문서를 LLM으로 물어볼 때 도움이 된다.  
5. [ASP.Net Core - DI에서 Dbcontext 사용하기](https://docs.google.com/document/d/e/2PACX-1vTw549tqwoIE6WOWv5a8lFUFNakk988zXvpu2NHzhWrf8dKnbyYBza281hp3Gk9kRqO6r22hTQ63hGI/pub ) (★★) - Dbcontext를 사용하는 자료
6. [ASP.NET Core - DI로 추가한 클래스의 초기화 방법](https://www.sysnet.pe.kr/2/0/13152 ) (★★) - 간단한 예제
7. [ASP.NET Core 서버 시작까지 흐름 추적](https://jacking75.github.io/NET_lAspNetCoreTrace/) (★) - 조금 예전 버전 자료

## ASP.NET Core 고급

### 필수 자료
2. [ASP.NET Core에서 호스팅되는 서비스를 사용하는 백그라운드 작업](https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/host/hosted-services?tabs=visual-studio&utm_source=pocket_mylist&view=aspnetcore-9.0) (★★★) - 백그라운드 작업
3. [ASP.NET Core 출력 캐싱 미들웨어](https://learn.microsoft.com/ko-kr/aspnet/core/performance/caching/output?view=aspnetcore-9.0) (★★★) - 성능 최적화

### 참고 자료
1. [ASP.NET Core의 속도 제한 미들웨어](https://learn.microsoft.com/ko-kr/aspnet/core/performance/rate-limit?preserve-view=true&view=aspnetcore-9.0) (★★) - DDoS 방지
2. [요청 본문을 Stream 또는 PipeReader로 바인딩 (해당 링크 중간 부분)](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-9.0#rbs) (★★) - 대용량 파일 처리
3. [.NET Core9.0에서 CORS 설정하기](https://docs.microsoft.com/ko-kr/aspnet/core/security/cors?view=aspnetcore-9.0) (★★) - CORS 설정
4. [ASP.NET Core에서 API 별 버전 관리](https://docs.google.com/document/d/1XMqTausAUspWr0GLXMkdLpX-vE4uDnQj0JZ3Fmi61-Y/edit?usp=sharing) (★★) - API 버전 관리
5. [IHostedService 및 BackgroundService 클래스를 사용하여 마이크로 서비스에서 백그라운드 작업 구현](https://docs.microsoft.com/ko-kr/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice ) (★) - ASP.Net Core 백그라운드 작업 고급
6. [Client IP safelist for ASP.NET Core](https://learn.microsoft.com/ko-kr/aspnet/core/security/ip-safelist?view=aspnetcore-9.0)  (★★) - 화이트리스트 기반 접근제어 미들웨어
7. [Custom ASP.NET Core Middleware Example (영어 자료)](https://blogs.msdn.microsoft.com/dotnet/2016/09/19/custom-asp-net-core-middleware-example/ )(★) - 미들웨어 example
8. [Exploring a minimal WebAPI with ASP.NET Core](http://www.hanselman.com/blog/ExploringAMinimalWebAPIWithASPNETCore.aspx )  (★★) - minimal WebAPI 영어자료
9. [ASP.NET Core WebAPI에서 Custom Formatter를 이용하여 CSV 및 JSON 출력을 전환하는 샘플](https://docs.google.com/document/d/12sLRZPFBh3uJ5UGTfd59bvuTZhjIHkwi9tT2ZyDYjJs/edit?usp=sharing )  (★★)
10. [ASP.NET Core - 우선순위에 따른 HTTP/HTTPS 호스트:포트 바인딩 방법](https://www.sysnet.pe.kr/2/0/13545 )  (★★)

#### gRPC
11. [ASP.NET Core gRPC 앱에서 gRPC JSON 코드 변환](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/json-transcoding?view=aspnetcore-9.0 ) (★★) 
12. [gRPC JSON 코드 변환에 대한 HTTP 및 JSON 구성](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/json-transcoding-binding?view=aspnetcore-9.0 ) (★★) 
13. [.NET 앱에 대한 Protobuf 메시지 만들기](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/protobuf?view=aspnetcore-9.0) (★★) 
14. [.NET용 gRPC 구성](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/configuration?view=aspnetcore-9.0 ) (★★) 
15. [.NET의 gRPC에서 로깅 및 진단](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/diagnostics?view=aspnetcore-9.0 ) (★★) 
16. [gRPC와 프로세스 간 통신](https://learn.microsoft.com/ko-kr/aspnet/core/grpc/interprocess?view=aspnetcore-9.0 ) (★) 
17. [샘플코드 Transcoder](https://github.com/grpc/grpc-dotnet/tree/master/examples/Transcoder) (★) 
18. [ASP.NET Core gRPC 앱에서 gRPC JSON 코드 변환 프로젝트 만들기](https://docs.google.com/document/d/e/2PACX-1vTQCG9IMA32WOgCFO417LqwS0KxrPK_AiijaR9tuz3a0lboL9C4TuHiOw-y9WZ0LtfIq1Wn2qxHvkZE/pub) (★) 
  
## 데이터베이스 연동

### 필수 자료
1. [코드에서 DB 트랜잭션 하기](./how_to_db_transaction.md) (★★★★) - 트랜잭션 처리
2. [데이터 설계](https://drive.google.com/file/d/14NcczPD6XS1rLH6TpORCcLhF9ftsVSgu/view?usp=sharing ) (★★★) - 게임 DB 설계자료


### 참고 자료

## Redis 활용

### 참고 자료
1. [CloudStructures - Redis 라이브러리 소개](https://jacking75.github.io/NET_lib_CloudStructures/) (★★) - C# Redis 라이브러리
2. [Cache-Aside_pattern.md](./Cache-Aside_pattern.md) (★★★) - 캐싱 패턴 이해

## 로깅 및 모니터링

### 필수 자료
3. [ASP.NET Core 프레임워크 내부의 로그 수준 설정하기](https://docs.google.com/document/d/e/2PACX-1vRN7e0qnQE9gC780ddPfIojCnVUhd7mf-uYk6oRIibo_nEbs3HWJe8-61jDAiel37AQxe8BxKjb58-l/pub) (★★★) - 로그 레벨 설정

### 참고 자료
1. [fluentd 디렉토리](./fluentd/) (★★) - 로그 수집 시스템
2. [prometheus_grafana 디렉토리](./prometheus_grafana/) (★★) - 모니터링 시스템

## 환경 설정 및 배포

### 필수 자료
1. [Microsoft.Extensions.Configuration 및 Microsoft.Extensions.Options 사용 시작](https://docs.google.com/document/d/12OwjWyzMwYWMWi9LEZEvgE2gjx-aGWP17f2Pw_z1S4o/pub) (★★★★) - 설정 읽는 방법
2. [ASP.NET Core에서 여러 환경 사용](https://docs.microsoft.com/ko-kr/aspnet/core/fundamentals/environments) (★★★) - 환경별 설정
3. [Development, Production 용 appsettings.json 사용하기](./appsettings.{Environment}.json.md )

### 참고 자료
1. [Microsoft.Extensions.Configuration.Json에서 읽은 설정 값에 Prefix 부여](https://docs.google.com/document/d/e/2PACX-1vQbK6RbrkoyhQDL1u1_8_ZQ02Dsqhkdv3WHj2UHY8SyuM5fgOy3RtIZ2B2f0iFtMqySU-dCZKjAsd4Y/pub) (★★) - 설정 관련 팁
2. [C#(.NET) 이미 빌드 된 API 서버의 엔드 포인트 url을 변경하는 방법](https://docs.google.com/document/d/e/2PACX-1vRhHebXWTa1OOY60NT3T0ZRkr8NMNRIuFADhVZIknSs_YsnlYTz7FObkexo9x1iNfoQID31-hVxiwTB/pub) (★★) - 배포 후 설정
3. [.NET 6에서 Kestrel 포트를 설정하는 몇 가지 방법](https://forum.dotnetdev.kr/t/net-6-kestrel-bryan-hogan/2681) (★★) - 포트 설정
4. [ASP.NET Core 앱에서 URL을 설정하는 5가지 방법](https://docs.google.com/document/d/1x3ZJQtGt2uNW5_xRT6QHrOmZkfbNU2KQ23N5RCLi4cE/edit?usp=sharing) (★★★) - URL 설정
5. ["Docker Desktop for Windows" - ASP.NET Core 응용 프로그램의 소켓 주소 바인딩](https://www.sysnet.pe.kr/2/0/13548) (★) - Docker 설정
6. [docker 디렉토리](./docker/) (★) - Docker 관련 예제 및 설명
7. [Visual Studio - .NET 8 프로젝트부터 dockerfile에 추가된 "USER app" 설정](https://www.sysnet.pe.kr/2/0/13547 ) (★) - 사용자 모드 같은 것에 대한 설정


## 참고 프로젝트

1. [미니 게임 천국](https://github.com/jacking75/edu_Learn_ASPNetCore_APIServer/tree/main/codes/practice_MiniGameHeavenAPIServer) (★★★★) - 가장 추천되는 참고 프로젝트
2. [api_server_training_dungeon_farming](./api_server_training_dungeon_farming/) (★★★★) - 완성된 API 서버 프로젝트
3. [지니 팜](https://github.com/jacking75/com2us_edu_GenieFarm) (★★★) - 인턴 프로젝트
4. [수집형 RPG API 서버](https://github.com/beatheat/Com2usEduProject ) (★★) 
5. 2024년 API 서버로 만든 오목 게임 서버  (★★) 
    - 두 개의 프로젝트가 있다. 둘다 만드는 게임은 같지만 각각 개발자는 달라서 구현도 조금씩 다르다
    - API 서버로 실시간 게임 콘텐츠를 구현하는 방법을 볼 수 있다. 롱 폴링 방식 사용 
    - `codes/practice_omok_game-1` , 'codes/practice_omok_game-2`  
6. 2025년 신입 사원 교육 프로젝트  (★★) 
    - 하스스톤 모작
    - `codes/HearthStone` 디렉토리에 있다. 

## 기타 학습 자료

### API Server 개발하기 
- `code` 디렉토리의 `GameAPIServer_Template` 디렉토리에 있는 코드를 참고해서 만들기 바란다.
    - `GameAPIServer_Template_Doc` 에 코드에 대한 설명 문서가 있으니 꼭 보기 바란다
- 프로젝트는 새로 만들고 구조나 코드 등을 참고한다.    
- `GameAPIServer_Template` 에 있는 코드 보다 더 좋은 코드를 만드는 것을 목표로 한다.  

<br>    
  
### example_filter_APIServer
- 해당 프로그램은 `codes` 디렉토리 안에 있다.  
- 기획데이터(마스터 데이터)가 json으로 된 것을 로딩
- 미들웨어로 라우팅을 커스텀마이징
- 미들웨어로 클라이언트 보낸 요청 데이터의 암호를 풀어서 핸들러에 전달하도록 한다
- 필터로 클라이언트에 보내는 응답의 포맷을 변환하거나 암호화 하도록 한다  
  
<br>   

### Deep Wiki
이 저장소의 내용을 DeepWiki로 만들었습니다. [2025년 5월 9일 제작한 위키](https://deepwiki.com/jacking75/edu_Learn_ASPNetCore_APIServer/1-overview)는 영어로 작성되어 있습니다. 

### 인턴 자료
1. [학습자료_2022년_인턴 폴더](./학습자료_2022년_인턴/) (★★★) - 인턴 학습 자료
2. [코드 디렉토리](./codes/) (★★★) - 다양한 예제 프로젝트 모음
3. [인턴 서버 개발 기초](https://velog.io/@oak_cassia/%EC%84%9C%EB%B2%84%EC%BA%A0%ED%8D%BC%EC%8A%A4-1%EA%B8%B0-ASP.NET-Core) (★★★) - 게임 서버 개발자 기초
4. [인턴 서버 개발 기초2](https://gist.github.com/jacking75/344fd3c8c16fc27fe51d4c983a3a6306) (★★★) - 게임 서버 개발자 기초 자료
5. [인턴 서버 개발 기초3](https://paper-tub-6ae.notion.site/0be4168f7f224f82a89110423e3943cb) (★★★) -  컴팩트한 서버 개발 기초자료
6. [인턴 .NET 개발 기초](https://babebab.tistory.com/53 ) (★★★) - .NET 서버 개발 기초
7. [워리할수있다 (tistory.com)](https://wallyyoucandoit.tistory.com/?page=1)(★★) 
8. [Intro - WebAPI Server (gitbook.io)](https://dong-d.gitbook.io/webapi-server/) (★★) 
9. [ASP (notion.site)](https://easy-cell-518.notion.site/ASP-0f310b9157de4cb683a5250c5eb9cc19 ) (★★) 
10. https://velog.io/@oak_cassia/%EC%84%9C%EB%B2%84%EC%BA%A0%ED%8D%BC%EC%8A%A4-1%EA%B8%B0-ASP.NET-Core (★★) 
11. https://beathe.at/2023-04/ASP.NET-Core-GameServer (링크 끊김)
12. [학습자료_2024_서버캠퍼스2기](학습자료_2024_서버캠퍼스2기) (★★★) - 2024 서버캠퍼스 수료생 자료 


### .Net 예제
1. [ASP.NET Core 다양한 샘플 코드](https://github.com/dodyg/practical-aspnetcore) (★★★) - 다양한 예제 코드 모음)  

### 게임 엔진 아키텍처
3. [게임 엔진 아웃라인](https://drive.google.com/file/d/1HeBu2NXeLXU8VY5l7Gph4l6SRZkhZQae/view?usp=sharing ) (☆)

### 네트워크 및 통신
1. [네트워크 기초](https://drive.google.com/file/d/1VmxaTRA1qJnoIGWKK6SoYDH-pHBvnGbz/view?usp=sharing ) (★★) - 네트워크 기초


---

## LinqPad 관련 자료

1. [LinqPad 소개](https://docs.google.com/presentation/d/1THcgeub4cNRJdFCxHatpxkPR0AhYJay55P5xxZ1sgtE/edit?usp=sharing) (★★★) - LinqPad 기본 사용법
2. [LinqPad 무료 버전에서 nuget 사용하기](https://docs.google.com/document/d/1Hn8WDZxkX5os86DZANeHS4ggeqq6NWD67QbfigawJuM/edit?usp=sharing) (★★★) - NuGet 패키지 관리
