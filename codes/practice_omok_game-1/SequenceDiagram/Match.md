# 시퀀스 다이어그램 (Match)
------------------------------
## 매칭 시작 요청
### : 매칭 시작 요청 /requestMatching

```mermaid
sequenceDiagram
	actor P as Player
	participant G as Game Server
	participant M as Match Server
	participant R as Redis

	P->>G: 매칭 시작 요청 (/requestMatching)
	G->>M: 매칭 시작 요청
	M->>M: 매칭 큐에 추가

	M-->>G: 결과 응답
	alt 성공
		G-->>P: 매칭 시작 요청 성공
	else 실패
		G-->>P: 매칭 시작 요청 실패
	end

```

------------------------------

## 매칭 완료 여부 체크 (매칭 될 때까지 1초마다 요청)
### : 매칭 완료 여부 체크 (매칭 될 때까지 1초마다 요청) /checkMatching
```mermaid
sequenceDiagram
	actor P as Player
	participant G as Game Server
	participant M as Match Server
	participant R as Redis

	loop MatchWorker 0.1초 마다
		M->>M: 매칭 큐.Count > 2 확인
			opt 2이상이면
				M->>M: Dequeue
				M->>R: 매칭결과 저장
				R-->>M: 
				M->>R: OmokGameData 생성 후 저장
				R-->>M: 
			end
	end

	P->>G: 매칭 완료 여부 체크 요청 (/checkMatching)
	G->>R: 매칭결과 존재하는지 확인
	R-->>G: 
	alt 매칭 결과 존재 O
		G->>R: PlayingUserData 생성 후 저장
		R-->>G:  

		G-->>P: 매칭 완료 응답
	
	else 매칭 결과 존재 X
		G-->>P: 매칭 미완료 응답
	end

```

------------------------------


