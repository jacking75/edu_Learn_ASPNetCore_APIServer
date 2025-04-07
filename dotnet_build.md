출처:  https://zenn.dev/takumi_machino/articles/how-dotnet-work  
  
# .NET 애플리케이션은 어떻게 작동하는가?

## 시작하며
C# 또는 .NET 애플리케이션을 Visual Studio 등에서 "▶ 실행"했을 때, 내부적으로 어떤 처리가 이루어져 애플리케이션이 실행되는지 생각해 본 적이 있는가?  
이 글에서는 .NET의 빌드부터 실행까지의 흐름을 초보자도 이해할 수 있도록 설명한다.  
  
  
## 빌드란 무엇인가
빌드란 C# 등의 **소스코드를 컴퓨터가 실행할 수 있는 형태로 변환하는 작업**이다.  
단, .NET의 경우 한 번 **중간 언어(CIL)** 형식으로 변환한 후, 실행 시에 필요한 부분만 진짜 **기계어(네이티브 코드)**로 변환한다.  
  
## 빌드 결과물의 위치
프로젝트를 빌드하면 다음과 같은 디렉토리 구조가 된다.  
  
```
MyApp/
└── bin/
    └── Debug/
        └── net8.0/
            ├── MyApp.dll
            ├── {추가한패키지명}.Json.dll
            |　               ︙
            ├── MyApp.pdb
            ├── MyApp.runtimeconfig.json
            └── MyApp.deps.json
```
  
**MyApp.dll**: 실행 대상 어셈블리(CIL 형식)    
**MyApp.pdb**: 디버그 정보    
**{추가한패키지명}.Json.dll**: 추가한 패키지의 어셈블리  
**runtimeconfig.json**: 실행 시 필요한 런타임 지정  
**deps.json**: 의존 관계 정보  
  
이 **MyApp.dll**을 **dotnet** 명령으로 실행함으로써 애플리케이션이 시작된다.  
"▶ 실행"했을 때는 내부적으로 **dotnet** 명령을 실행하고 있다.  
  
```
dotnet bin/Debug/net8.0/MyApp.dll
```
  
애플리케이션 실행 시에는 CIL 형식의 **.dll**이 .NET 런타임(CLR)에 의해 로드된다.  
필요한 부분은 JIT(Just-In-Time) 컴파일로 네이티브 코드로 변환된 후 실행된다.  
  
```
[MyApp.dll] --로드--> [CLR (.NET 런타임)]
           --JIT변환--> [네이티브 코드로 실행]
```
  
  
## .exe는 어디서 만들어지는가
일반적인 .NET 6/7/8 프로젝트에서는 **.dll**이 생성되며, **.exe**는 출력되지 않는다.  
단, 다음 경우에는 **.exe**가 생성된다.  
  
- **.NET Framework** 프로젝트(Windows 전용) 
- 다음과 같이 자체 완결형으로 빌드한 경우(Windows 전용)
  ```
  dotnet publish -r win-x64 --self-contained -c Release
  ```
  
이를 통해 다음과 같은 출력을 얻을 수 있다.
  
```
bin/Release/net6.0/win-x64/publish/
├── MyApp.exe
├── MyApp.dll
├── 기타 DLL 및 설정 파일
```
  
**.exe**는 단독으로 실행 가능한 애플리케이션이 된다.
  
  
## 참고
https://dotnet.microsoft.com/ko-kr/learn/dotnet/what-is-dotnet-framework  