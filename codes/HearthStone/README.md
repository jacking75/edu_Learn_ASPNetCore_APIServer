# Hearthstone Card Game Project

## 1. 하스스톤 카드 게임 개요
하스스톤은 블리자드 엔터테인먼트에서 개발한 디지털 카드 게임으로, 플레이어는 덱을 구성하여 턴제 방식으로 상대와 대결합니다. 이 프로젝트는 하스스톤의 핵심 게임 메커니즘을 구현하며, Blazor WebAssembly를 사용하여 클라이언트와 서버 간 상호작용을 처리합니다.

---

## 2. 게임 흐름
1. **게임 시작**:
   - 플레이어는 덱을 선택하고 상대와 매칭됩니다.
   - 각 플레이어는 초기 핸드 카드를 받습니다.

2. **턴 진행**:
   - 플레이어는 자신의 턴에 카드를 사용하거나 영웅 능력을 발동합니다.
   - 마나를 소모하여 카드를 소환하거나 주문을 시전합니다.

3. **전투 단계**:
   - 소환된 하수인으로 상대 하수인 또는 영웅을 공격합니다.

4. **게임 종료**:
   - 상대 영웅의 체력을 0으로 만들면 승리합니다.
   - 덱이 소진되거나 특정 조건이 충족되면 게임이 종료됩니다.

---

## 3. 기술 요소
- ** AI **
	- Claude
	- GitHub Copilot

- ** C# **
	- .NET 9
	- ASP.NET Core Web API
	- Blazor WebAssembly

- ** DB **
	- mysql
	- redis

- ** 시스템 **
	- Docker
	- Prometheus
	- Grafana
	- Fluentd
---

## 4. 주요 기능
- **사용자 인증**:
  - 회원가입 및 로그인 기능.

- **게임 매칭**:
  - 플레이어 간 매칭 시스템 구현.

- **카드 관리**:
  - 카드 생성, 수정, 삭제 기능.
  - 카드의 능력 및 효과 처리.

- **게임 로직**:
  - 턴제 게임 진행 로직.
  - 하수인 소환, 주문 시전, 전투 처리.

- **보상 시스템**:
  - 출석 보상 및 게임 보상 제공.

---

## 5. TODO-LIST

- [x] 회원가입 및 로그인 기능 구현
- [x] 플레이어 매칭 시스템 구현
- [x] 카드 생성/수정/삭제 기능 구현
- [x] 카드 능력 및 효과 처리
- [x] 턴제 게임 진행 로직 구현
- [x] 하수인 소환 및 주문 시전 기능 구현
- [x] 전투 처리 로직 구현
- [x] 출석 보상 및 게임 보상 시스템 구현
- [x] Blazor WebAssembly 클라이언트 구현
- [x] ASP.NET Core Web API 서버 구현
- [x] MySQL/Redis 연동
- [x] Docker 환경 구성
- [] Prometheus, Grafana, Fluentd 연동

---

## 실행 방법
1. **Docker 컨테이너 실행**:
   docker-compose up -d

2. **MySQL 초기화 데이터 확인**:

3. **Blazor WebAssembly 실행**:
   - Visual Studio에서 `ClientWeb` 프로젝트를 실행합니다.

4. **API 서버 실행**:
   - Visual Studio에서 `GameServer` 프로젝트를 실행합니다.

---

## 6.게임 플로우
https://drive.google.com/file/d/1TlIh6YD82Gt9a78hLngnT1miNp3q9t2w/view?usp=sharing


<img src="00.Document/ScreenShot/gamestart.png" alt="게임 시작 화면" width="400"/>
3장의 덱 카드 랜덤 셋팅, 5초 후 게임이 시작

---

<img src="00.Document/ScreenShot/1.png" alt="게임 시작 화면" width="400"/>
상대턴이면 인터렉션 불가능


---

<img src="00.Document/ScreenShot/2.png" alt="게임 시작 화면" width="400"/>
상대가 "카드 사용"을 할때 유저가 확인 가능

---

<img src="00.Document/ScreenShot/3.png" alt="게임 시작 화면" width="400"/>
내 턴, 카드 드로우, 카드사용, 공격, 턴 종료 가능

---

<img src="00.Document/ScreenShot/4.png" alt="게임 시작 화면" width="400"/>
내가 가진 마나코스트가 카드의 마나코스트보다 크면 사용 가능

---

<img src="00.Document/ScreenShot/5.png" alt="게임 시작 화면" width="400"/>
카드 드로우시 새로운 카드 등장, 매 턴마다 한번의 카드드로우만 가능

---

<img src="00.Document/ScreenShot/6.png" alt="게임 시작 화면" width="400"/>
내가 가진 마나코스트보다 카드의 마나 코스트가 작으면 사용 불가능

---

<img src="00.Document/ScreenShot/7.png" alt="게임 시작 화면" width="400"/>
카드 사용시 마나코스트 차감

---

<img src="00.Document/ScreenShot/8.png" alt="게임 시작 화면" width="400"/>
공격시 마나코스트 차감 단, 마나코스트가 없다면 공격 불가능

---

<img src="00.Document/ScreenShot/9.png" alt="게임 시작 화면" width="400"/>
내 카드 선택 후 공격시 상대방 hp 차감


<img src="00.Document/ScreenShot/10.png" alt="게임 시작 화면" width="400"/>
내 카드 선택후 공격시 내 마나코스트 차감

---

<img src="00.Document/ScreenShot/11.png" alt="게임 시작 화면" width="400"/>
내 카드 선택 후 상대방 카드 선택하면 카드의 hp 차감

<img src="00.Document/ScreenShot/12.png" alt="게임 시작 화면" width="400"/>
상대방 카드의 hp가 0이 되면 카드 삭제

---

<img src="00.Document/ScreenShot/13.png" alt="게임 시작 화면" width="400"/>
<img src="00.Document/ScreenShot/14.png" alt="게임 종료 화면" width="400"/>
상대방의 hp 가 0이 되면 게임 승리


---
## 7. 문서 정리
00.Documen/Contents - 컨텐츠 시퀀스 다이어그램

00.Documen/DB - DB 스키마

