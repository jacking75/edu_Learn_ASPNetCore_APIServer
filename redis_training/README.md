# Redis 프로그래밍 실습
레디스 프로그래밍을 학습하기 위해 제시한 과제에 맞게 구현해야 한다.  
    
    
## 각 언어별 사용 라이브러리
- C#: CloudStructures
- Golang: go-redis
- C++: [레디스 공식 사이트](https://redis.io/resources/clients/#cpp ), [Acl 라이브러리의 Redis](https://github.com/jacking75/edu_cpp_server_programming/tree/main/acl-beginner/RedisServerAcl )
  

## 구현 조건
- 사례 별 요청과 응답을 편리하게 조작하기 위해 API 서버(`csharp_APIServer` 디렉토리)에서 구현한다.  
- DB(MySQL 같은)는 사용하지 않고, 오직 Redis만 사용하거나 더미 데이터를 사용한다.  
- Redis는 1대만 사용한다고 가정한다.  
- Redis의 영구 저장 기능은 사용하지 않는다.
- Redis는 서비스 중 절대 죽지 않는다고 가정한다.
- 웹소켓은 사용하지 않는다. 오직 HTTP만 사용한다.  
- Redis 버전은 6.0 이상을 사용한다.
  
<br>  
    
# 과제 
  
## 로그인
- 저장할 것: 인증키, 앱버전, 데이터버전
- api url: RequestLogin
```
public class LoginReq
{
    public string ID { get; set; }
    public string Password { get; set; }

    public int AppVersion { get; set; }
    public int DataVersion { get; set; }
}

public class LoginRes
{
    public int Result { get; set; } = 0; // 0 성공, 0 이외는 모두 실패
    public string AuthKey { get; set; }
}
```
  
  
<br>  
  
## 로그인 때 저장한 유저 정보 가져오기 
- 사전에 유저의 게임 데이터를 Redis에 넣어 놓는다.
    - 로그인 때 넣으면 좋을 듯
- api url: RequestLoadUserGameData
```
public class LoadUserGameDatsReq
{
    public string ID { get; set; }
}

public class LoadUserGameDatsReq
{
    public int Result { get; set; } = 0; // 0 성공, 0 이외는 모두 실패
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Money { get; set; }
}
```  
  

<br>    

## 동일 유저 요청는 순차적으로 처리되도록 하기(Lock 걸기)
- 유저가 동시에 요청하더라도 한번에 하나만 처리해야 한다.
    - 게임서버가 요청을 받아서 처리하는 중에 또 요청이 오면 이 요청은 에러 처리가 되어야 한다.
    - 테스트를 위해 서버는 이 요청을 받으면 내부에서 Sleep을 10초 건다.
- 요청을 처리 중 서버가 크래시 되더라도 게임서버가 재 시작되면 유저는 요청을 할 수 있어야 한다.    
- api url: RequestGetItm
```
public class GetItmReq
{
    public string ID { get; set; }
}

public class GetItmRes
{
    public int Result { get; set; } = 0;
}
```
  

<br>    

## 방문자 수 기록하기
- 로그인을 기준으로 한다. 1일 1회의 로그인만 방문자 수로 집계한다.  
  
  
<br>  

## 요청을 2분 단위로 3회만 가능하도록 제한걸기 
- 게임 서버에 2분 동안 3회만 요청 가능하다
- 2분이 지나면 요청 가능 횟수는 다시 3회가 된다.  
- api url: RequestChangeNickName
```
요청과 응답 데이터는 자유롭게 만든다  
```
  

<br>    

## 하루에 한 번만 참여 가능한 이벤트 
- 경험치 업 이벤트는 1일 1회만 참여가능
- 이력 측면에서 특정 유저가 특정 날짜에 이 이벤트를 참여 했는지 여부를 알 수 있어야 한다.
   

<br>     
  
## 인증 문자를 받고나서 다음 인증 문자를 받으려면 1분의 대기 시간을 주기
  

<br>    

## 링크드 리스트 구현하기
- Redis만을 사용하여 유저ID를 데이터로 하는 링크드 리스트 구현하기
- 유저 추가, 삭제
- 순회: 유저 앞에 있는 유저, A 유저 뒤에 있는 유저
  
  
<br>    

## 좋아요
  
### 유저를 상대로 좋아요 하기/취소하기
  

<br>    

### 누가 좋아요를 했는지 확인하기

  
<br>  

### 좋아요를 한 개수 조회하기

  
<br>  

## 최근에 본 인앱 상품 목록 보여주기(최근 순서대로 3개)     
  
  
<br>    

## 랭킹 구현
- 1~10등
- 자신의 등수
- 자신의 등수를 기준으로 상하 각각 2명씩
- 선택: 동접자 처리 
  

<br>    

## 배틀
- 클라이언트는 배틀 시작을 요청하고, 응답을 서버로부터 받아야 한다
    - NPC 정보를 보낸다. NPC 코드, NPC 이름, NPC 보상 경험치
    - 배틀 클리어 후 받는 보상 정보를 보낸다.
- 클라이언트는 배틀 중 처치한 NPC 정보를 알려준다.
- 클라이언트는 배틀가 완료되면 서버에 알린다.
    - 배틀 클리어 조건은 배틀 시작에서 받은 모든 NPC를 다 처치해야 한다.
    - 서버는 배틀가 완료되면 이 때 처치한 모든 NPC로 부터 얻을 수 있는 경험치와 보상을 계산해서 알려준다.
    - 배틀 정보는 삭제 되어야 한다.  
  

<br>    
  
## 배틀 - 보스 레이드
- 특정 보스를 여러 유저가 동시에 같이 공격한다
    - 유저는 최대 5명까지만 참여 가능하다
- 보스를 처리하면 보상을 받는다
    - 보상은 공격에 가장 크게 기여를 한 유저 순서로 차등 지급한다
    - 보상과 각 유저 별 공격 히스토리(어떤 공격, 데미지 수치)도 보여줘야 한다
  
  
<br>    

## 채팅
- 웹서버만으로 채팅을 구현한다
- 채팅은 로비 단위로 구분된다. 로비 번호는 1~ 100개. 
    - 채팅을 위해 로비로 나누어진 것으로 로비 입장 같은 것을 필요 없다.
    - 각 로비당 최대 인원은 100명까지
- 유저는 로그인 시에 자동으로 로비에 입장해야 한다. 즉 서버에서 자동으로 넣는다.
- 유저는 로그인 이후 로비 번호를 선택해서 로비 입장을 요청할 수도 있다.
- 채팅 창을 통해서 이전 채팅 히스토리를 볼 수 있어야 한다. 단 현재 채팅에서 50개까지만 가능하다.

  
<br>  

## Job Queue
- 다른 프로세스로 실행 중인 worker에 작업 요청하기
  
  
<br>  
    
## 선착순 쿠폰 발행하기
- 선착순 3명에게만 쿠폰을 발행한다



<br>  
<br>  
  

# 참고
- [Redis 관련 글 모음](https://gist.github.com/jacking75/5d9927851b22a539774301017e0cefd7 )
- [회사 위키의 Redis](https://jira.com2us.com/wiki/pages/viewpage.action?pageId=151550552 )
- [Redis를 활용한 다양한 시스템 설계](https://devs0n.tistory.com/92)
- [(일어) Redis 활용술 초급](https://www.slideshare.net/jaeseopjeong/redis-35862286)


























