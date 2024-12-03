# 시퀀스 다이어그램 (Friend)

* 친구 목록 가져오기 (Get Friend List)
* 친구 신청 목록 가져오기 (Get Friend Request List)
* 친구 신청 (Friend Request)
* 친구 신청 수락하기 (Friend Request Accept)

------------------------------

## 친구 목록 가져오기
### 플레이어의 친구 목록 가져오는 요청 (Get Friend List)
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 친구 목록 가져오는 요청 (/friend/get-list)
	Game Server ->> GameDB : 친구 목록 가져오기
	GameDB -->> Game Server : 
	Game Server -->> Player : 친구 목록 응답

```


------------------------------

## 친구 신청 목록 가져오기
### 플레이어의 친구 신청 목록 가져오는 요청(받은 신청) (Get Friend Request List)
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 친구 신청 목록 가져오는 요청 (/friend/get-request-list)
	Game Server ->> GameDB : 친구 신청 목록 가져오기
	GameDB -->> Game Server : 
	Game Server -->> Player : 친구 신청 목록 응답

```


------------------------------


## 친구 신청
### 플레이어가 친구 신청을 보내는 요청
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 친구 신청 요청 (/friend/request)
	Game Server ->> GameDB : 친구 신청 테이블 조회
	GameDB -->> Game Server : 

	alt 요청 존재 O
		alt 요청 상태 == 0 (대기)
			Game Server -->> Player : 친구 신청 대기중 응답
	
		else 요청 상태 == 1 (수락)
			Game Server -->> Player : 이미 친구 응답
		end
	
	else 요청 존재 X
		alt 상대가 보낸(순서가 바뀐) 요청도 존재 X
			Game Server ->> GameDB : 요청 추가
			GameDB -->> Game Server :  
			Game Server -->> Player : 친구 신청 성공 응답
		else 상대가 보낸(순서가 바뀐) 요청 존재 O
			Game Server -->> Player : 상대가 보낸 친구 신청 대기중 오류 응답
		end
	
	end

```




------------------------------


## 친구 신청 수락하기
### 플레이어가 받은 친구 신청을 수락하는 요청
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 친구 신청 수락 요청 (/friend/accept)
	Game Server ->> GameDB : 친구 신청 테이블 조회
	GameDB -->> Game Server : 
	
	alt 테이블에 신청 없을 때
		Game Server -->> Player : 오류(존재하지 않는 요청) 응답
	else
		alt 신청 상태 == 0 (대기)
			Game Server ->> GameDB : 요청 수락(1) 업데이트
			GameDB -->> Game Server :  

			Game Server ->> GameDB : friend 테이블에 추가 (2번)
			GameDB -->> Game Server :  
			
			Game Server -->> Player : 친구 신청 수락 완료 응답

		else 신청 상태 == 1 (수락)
			Game Server -->> Player : 이미 수락한 신청 응답
		end
	end

```







