# Development, Production 용 appsettings.json 사용하기

## 실행 시 환경 선택 방법

### 로컬 (CLI)

* 개발:

  ```bash
  ASPNETCORE_ENVIRONMENT=Development dotnet run
  ```
* 프로덕션(리릴리스 빌드 예시):

  ```bash
  ASPNETCORE_ENVIRONMENT=Production dotnet publish -c Release
  ASPNETCORE_ENVIRONMENT=Production dotnet ./bin/Release/net8.0/MyApp.dll
  ```

Windows PowerShell에서는:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

> `DOTNET_ENVIRONMENT`를 써도 동일하게 동작한다.


### launchSettings.json 프로필

개발 시 프로필별로 미리 고정할 수 있다.

```json
{
  "profiles": {
    "MyApp (Dev)": {
      "commandName": "Project",
      "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" }
    },
    "MyApp (Prod)": {
      "commandName": "Project",
      "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Production" }
    }
  }
}
```

Visual Studio나 `dotnet run --launch-profile "MyApp (Prod)"`로 선택해서 실행한다.


### Linux systemd

`/etc/systemd/system/myapp.service`:

```ini
[Service]
Environment=ASPNETCORE_ENVIRONMENT=Production
ExecStart=/usr/bin/dotnet /var/www/myapp/MyApp.dll
```

`sudo systemctl daemon-reload && sudo systemctl restart myapp`로 적용한다.
  
  
### IIS (Windows)
환경 변수는 **시스템 환경 변수**로 설정하거나, 애플리케이션 풀의 “환경 변수”에 `ASPNETCORE_ENVIRONMENT=Production`을 추가한다.
  
  
### Docker / docker-compose

```yaml
services:
  web:
    image: myapp:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```
  
  
### Kubernetes (Deployment)

```yaml
env:
  - name: ASPNETCORE_ENVIRONMENT
    value: "Production"
```
  
  
### Azure App Service

구성(설정) → **애플리케이션 설정**에 `ASPNETCORE_ENVIRONMENT=Production`을 추가한다.

  
  
## 환경별 코드 분기(선택)
환경에 따라 코드 흐름을 바꾸고 싶을 때:

```csharp
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
```

  
## 자주 하는 실수와 체크리스트

* `appsettings.Production.json`을 만들고도 환경 변수를 안 바꿔서 적용이 안 되는 경우가 많다 → **프로세스 시작 시점에 환경 변수가 설정되어 있어야 한다**.
* 민감한 값(비밀번호, 연결 문자열)은 환경 파일 대신 **환경 변수**나 **비밀 관리자/Key Vault**를 권장한다.
* 라이브로 환경을 바꾸는 기능은 없다 → **환경 변경 시 재시작**이 필요하다.
* 환경 이름 오타 주의(`production` 대신 `Production`) → 대소문자 구분은 플랫폼마다 다르지만, **표준 표기**를 쓰는 게 안전하다.


## 요약
* 서버 시작 시 **`ASPNETCORE_ENVIRONMENT`(또는 `DOTNET_ENVIRONMENT`)를 `Development`/`Production`으로 지정**해서 원하는 버전을 선택하면 된다.
* 그에 맞춰 `appsettings.{Environment}.json`이 자동 로드되고, 코드에서도 `app.Environment.IsDevelopment()` 등으로 분기하면 된다.
