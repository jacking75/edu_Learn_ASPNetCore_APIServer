# 시퀀스 다이어그램 (MailBox)


## 플레이어의 우편함 목록을 가져오는 요청
### : 플레이어의 우편함 리스트를 가져오는 요청 /mail/get-mailbox
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  participant GameDB

	Player ->> Game Server : 우편함 리스트 요청(/mail/get-mailbox)
	Game Server ->> GameDB : MailBox list 가져오기
	GameDB -->> Game Server : 
	Game Server -->> Player : 결과 응답
```



------------------------------

## 플레이어가 우편함에서 우편을 열어보는 요청 (우편 내용 보기)
### : 플레이어가 자신의 우편함에서 우편을 읽는 요청 /mail/read
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  	participant GameDB

	Player ->> Game Server : 우편 확인 요청(/mail/read)
	Game Server ->> GameDB : 우편 읽어오기(ReadMailDetail)
	GameDB -->> Game Server : 
	alt 우편 존재 X
		Game Server -->> Player : 오류 응답
	else 우편 존재 O
		Game Server -->> Player : 우편 내용 응답
	end
```







------------------------------

## 플레이어가 우편 속 아이템을 수령하는 요청
### : 플레이어가 우편 속 아이템을 수령하는 요청 /mail/receive-item
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  	participant GameDB

	Player ->> Game Server : 우편 속 아이템 수령 요청(/mail/receive-item)
	Game Server ->> GameDB : 우편 아이템 상태 조회 (GetMailItemInfo)
	GameDB -->> Game Server : 
	alt 수령 불가능 상태
		Game Server -->> Player : 오류 응답
	else 수령 가능 상태
		Game Server ->> GameDB : 수령 상태로 변경
		GameDB ->> GameDB : 아이템 테이블에 추가
		GameDB -->> Game Server : 
		
		Game Server -->> Player : 성공여부 응답
	end
```




------------------------------

## 플레이어가 우편을 삭제하는 요청
### : 플레이어가 우편을 삭제하는 요청 /mail/delete
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  	participant GameDB

	Player ->> Game Server : 우편 삭제 요청(/mail/delete)
	Game Server ->> GameDB : 우편 아이템 상태 조회 (GetMailItemInfo)
	GameDB -->> Game Server :  
	alt 보상 미수령 상태 
		Game Server -->> Player : 삭제 실패 에러코드 응답

	else 보상 수령 상태
		Game Server ->> GameDB : 삭제 요청
		GameDB -->> Game Server:  
		Game Server -->> Player : 삭제 완료 응답
	end
```

--------------------------------
