# 패킷 시퀸스 다이얼그램
  
## API Server      
  
### 계정 생성  
```
@startuml
Client <-> APIServer: Req/Res CreateAccountController
@enduml
```
  
### 로그인  
```
@startuml
Client <-> APIServer: Req/Res LoginController
@enduml
```
  
### 유저 정보 받기
```
@startuml
Client <-> APIServer: Req/Res UserGameInfoController
@enduml
```


### 로봇몬 필드 정보 받기
```
@startuml
Client <-> APIServer: Req/Res FieldMonsterController
@enduml
```
 
### 로봇몬 잡기
```
@startuml
Client <-> APIServer: Req/Res CatchController
@enduml
```

### 로봇몬 연구소 보내기
```
@startuml
Client <-> APIServer: Req/Res RemoveCatchController
@enduml
```

### 로봇몬 잡은 목룍 보기
```
@startuml
Client <-> APIServer: Req/Res CatchListController
@enduml
```

### 출석체크
```
@startuml
Client <-> APIServer: Req/Res DailyCheckController 
@enduml
```

### 랭킹 확인
```
@startuml
Client <-> APIServer: Req/Res RankingListController
@enduml
``` 

### 선물 보내기
```
@startuml
Client <-> APIServer: Req/Res SendPresentController
@enduml
``` 

### 선물 목록 보기
```
@startuml
Client <-> APIServer: Req/Res MailListController
@enduml
``` 

### 선물 주기
```
@startuml
Client <-> APIServer: Req/Res SendMailController
@enduml
``` 

### 선물 받기
```
@startuml
Client <-> APIServer: Req/Re sRecvMailController
@enduml
``` 

### 강화
```
@startuml
Client <-> APIServer: Req/Res UpgradeController
@enduml
``` 

### 진화
```
@startuml
Client <-> APIServer: Req/Res EvolveController
@enduml
``` 
