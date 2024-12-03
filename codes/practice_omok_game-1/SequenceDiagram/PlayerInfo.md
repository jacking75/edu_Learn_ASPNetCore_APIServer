# 시퀀스 다이어그램 (PlayerInfo)

------------------------------

## 플레이어 기본 데이터 가져오는 요청
### : 플레이어 기본 데이터 가져오는 요청 (닉네임, 레벨, 경험치, 승, 패, 무)
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  	participant GameDB

	Player ->> Game Server : Player 기본 데이터 정보 요청 (/playerInfo/basic-player-data)
	Game Server ->> GameDB : BasicPlayerData 가져오기
	GameDB -->> Game Server : 
	alt 존재 X
		Game Server -->> Player : 오류 응답
	else 존재 O
		Game Server -->> Player : BasicPlayerData 응답
	end
```



------------------------------


## 닉네임 변경 요청 
### : 닉네임 변경 요청 
```mermaid
sequenceDiagram
	actor Player
	participant Game Server
  	participant GameDB

	Player ->> Game Server : 닉네임 업데이트 요청 (/playerInfo/update-nickname)
	Game Server ->> GameDB : 닉네임 업데이트
	GameDB -->> Game Server :  
	Game Server -->> Player : Result 결과 정보

```


------------------------------

