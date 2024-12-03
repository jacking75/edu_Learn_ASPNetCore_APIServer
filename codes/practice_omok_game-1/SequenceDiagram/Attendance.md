# 시퀀스 다이어그램 (Attendance)
* 출석 정보 가져오는 요청 (Attendance get info)
* 출석 체크 요청 (Attendance Check)
------------------------------

## 출석 정보 가져오는 요청
### 플레이어의 출석 정보 가져오는 요청 
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 출석 정보 가져오기 요청(/attendance/get-info) 
	Game Server ->> GameDB : 출석 정보 가져오기
	GameDB -->> Game Server : 
	alt 존재 X
		Game Server -->> Player : 오류 응답
	else 존재 O
		Game Server -->> Player : 출석 정보 응답
	end

```


------------------------------

## 출석 체크 요청
### : 플레이어가 출석 체크를 요청
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 출석 체크 요청(/attendance/check)
  Game Server ->> GameDB : 최근 출석 일시 가져오기
	GameDB -->> Game Server : 
  alt 최근 출석 일시 == 오늘
    Game Server -->> Player : AttendanceCheckFailAlreadyChecked 응답
  else 최근 출석 일시 != 오늘
	  Game Server ->> GameDB : 출석 정보 업데이트
	  GameDB -->> Game Server : 
	  Game Server ->> GameDB : 출석 횟수 가져오기
	  GameDB -->> Game Server : 
	  Game Server ->> GameDB : 보상 아이템 테이블에 추가
	  GameDB -->> Game Server :  
    Game Server -->> Player : Result 결과 정보
  end

```



------------------------------
