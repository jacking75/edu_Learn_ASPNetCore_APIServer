# SequenceDiagram

## [Register-Login](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/Register-Login.md)
* 계정 생성 (Register)
* 로그인 (Login)

## [Match](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/Match.md)
* 매칭 시작 요청 (Request Matching)
* 매칭 완료 여부 체크 요청 (Check Matching)


## [GamePlay](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/GamePlay.md)
* 돌두기 (자기 차례 플레이어) (Put Omok)
* 돌두기 포기 요청 (자기 차례 플레이어) (Giveup Put Omok)
* 현재 턴 상태 요청 (차례 대기 플레이어) (Turn Checking)

* 현재 게임 데이터 가져오는 요청 (보드정보 + 플레이어 등등) (OmokGameData)


## [PlayerInfo](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/PlayerInfo.md)
* 플레이어 기본 데이터 가져오는 요청  (닉네임, 게임 재화, 레벨, 경험치, 승, 패, 무) (Basic Player Data)
* 닉네임 변경 요청 (Update NickName)

## [Item](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/Item.md)
* 플레이어의 아이템 리스트를 가져오는 요청 (Get Player Items)

## [MailBox](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/MailBox.md)
* 플레이어의 우편함 리스트 받아오는 요청 (Get Player MailBox)
* 우편함에서 우편을 열어 내용을 보는 요청 (Read Mail)
* 우편에 있는 아이템 수령하는 요청 (Receive item)
* 우편을 삭제하는 요청 (Delete Mail)

## [Attendance](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/Attendance.md)
* 출석 체크 요청 (Attendance Check)
* 출석 정보 가져오는 요청 (Attendance get info)

## [Friend](https://github.com/yujinS0/Omok-Game/blob/main/SequenceDiagram/Friend.md)
* 친구 목록 가져오기 (Get Friend List)
* 친구 신청 목록 가져오기 (Get Friend Request List)
* 친구 신청 (Friend Request)
* 친구 신청 수락하기 (Friend Request Accept)
