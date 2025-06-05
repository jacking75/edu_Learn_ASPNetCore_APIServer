# 게임 데이터베이스 스키마

## 데이터베이스 구조 개요

| 데이터베이스 | 설명 | 주요 기능 |
|-----------|-----|---------|
| hive_db | 인증 데이터베이스 | 계정 관리, 로그인 인증 |
| game_db | 게임 데이터베이스 | 사용자 게임 데이터 저장 |
| master_db | 마스터 데이터베이스 | 게임 설정, 아이템 정보 |

---

## hive_db

### account 테이블
**설명**: 사용자 계정 정보 저장

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK, AUTO_INCREMENT | 유니크 유저 번호 |
| email_id | VARCHAR(50) | NOT NULL, UNIQUE (idx_email_id) | 유저아이디. 내용은 이메일 |
| nickname | VARCHAR(50) | NOT NULL | 유저 닉네임 |
| pw | VARCHAR(100) | NOT NULL | 해시된 비밀번호 |
| salt_value | VARCHAR(100) | NOT NULL | 암호화 값 |
| create_dt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP | 생성 일시 |

---

## game_db

### user 테이블
**설명**: 사용자 기본 정보를 저장하는 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK | 계정 고유 번호 |
| main_deck_id | INT | NOT NULL | 메인 덱 정보 |
| last_login_dt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP ON UPDATE | 마지막 로그아웃 시간 |

### user_deck 테이블
**설명**: 사용자 기본 정보를 저장하는 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK | 계정 고유 번호 |
| deck_id | INT | PK | 덱 ID |
| deck_list | VARCHAR(1024) | NOT NULL | 덱 카드 리스트 |
| create_dt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP | 생성 일시 |

### user_asset 테이블
**설명**: 사용자 재화 정보를 저장하는 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK | 계정 고유 번호 |
| asset_name | VARCHAR(100) | PK | 재화 이름 |
| asset_amount | BIGINT | NOT NULL, DEFAULT 0 | 재화 수량 |

### user_attendance 테이블
**설명**: 사용자 출석 정보를 저장하는 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK | 계정 고유 번호 |
| event_id | INT | PK, DEFAULT 0 | 출석 키 |
| attendance_no | INT | NOT NULL, DEFAULT 0 | 출석 번호 |
| attendance_dt | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP ON UPDATE | 최근 업데이트 시간 |

### user_item 테이블
**설명**: 사용자 카드 아이템 정보

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK, INDEX (idx_user_user_id) | 계정 고유 번호 |
| item_id | INT | PK | 아이템 ID |
| item_cnt | INT | NOT NULL | 아이템 수량 |

### user_mail 테이블
**설명**: 사용자 이메일 정보

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| account_uid | BIGINT | PK | 계정 고유 번호 |
| mail_id | BIGINT | PK | 메일 ID |
| status | INT | NOT NULL, DEFAULT 0 | 메일 상태 |
| mail_info | VARCHAR(1024) | NOT NULL | 메일 정보 (JSON) |
| mail_desc | VARCHAR(1024) | NOT NULL | 메일 설명 |
| received_dt | DATETIME | NOT NULL | 수신 일시 |
| expire_dt | DATETIME | NOT NULL | 만료 일시 |

---

## master_db

### version 테이블
**설명**: 데이터 버전 정보를 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| id | INT | PK, AUTO_INCREMENT | 버전 ID |
| app_version | VARCHAR(20) | NOT NULL | 앱 버전 |
| master_data_version | VARCHAR(20) | NOT NULL | 마스터 데이터 버전 |
| create_dt | DATETIME | DEFAULT CURRENT_TIMESTAMP | 생성 일시 |

### item 테이블
**설명**: 아이템 데이터 정보를 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| item_id | BIGINT | NOT NULL | 아이템 ID |
| quality | TINYINT | NOT NULL | 품질 (전설, 영웅, 희귀, 일반, 무료) |
| item_type | TINYINT | NOT NULL | 아이템 타입 |
| ability_key | INT | NOT NULL | 능력치 키 |

### ability 테이블
**설명**: 어빌리티 데이터 정보를 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| ability_key | INT | NOT NULL | 능력치 키 |
| ability_type | VARCHAR(8) | NOT NULL | 능력치 타입 |
| ability_value | BIGINT | NOT NULL | 능력치 값 |

### gacha_info 테이블
**설명**: 아이템 랜덤 뽑기 확률을 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| gacha_key | INT | NOT NULL | 가챠 키 |
| count | INT | NOT NULL | 뽑기 횟수 |

### gacha_rate 테이블
**설명**: 아이템 랜덤 뽑기 정보를 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| gacha_key | INT | NOT NULL | 가챠 키 |
| item_id | BIGINT | NOT NULL | 아이템 ID |
| rate | BIGINT | NOT NULL | 확률 |

### attendance_info 테이블
**설명**: 출석 정보를 저장한 테이블 - 유료, 무료

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| event_id | INT | NOT NULL | 출석 키 |
| free_yn | CHAR(1) | NOT NULL | 무료 여부 (Y/N) |

### attendance_reward 테이블
**설명**: 출석 보상 정보를 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| day_seq | INT | NOT NULL | 일차 |
| event_id | INT | NOT NULL | 출석 키 |
| reward_key | INT | NOT NULL | 보상 키 |

### reward_info 테이블
**설명**: 보상 정보를 저장한 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| reward_key | INT | NOT NULL | 보상 키 |
| reward_class | VARCHAR(20) | NOT NULL | 보상 분류 |
| reward_type | VARCHAR(20) | NOT NULL | 보상 타입 |
| reward_value | BIGINT | NOT NULL | 보상 값 |

### initial_free_items 테이블
**설명**: 회원가입 시 무료로 제공되는 초기 카드 정보 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| item_id | BIGINT | NOT NULL | 아이템 ID |
| item_cnt | INT | NOT NULL, DEFAULT 1 | 아이템 수량 |

### initial_asset 테이블
**설명**: 회원가입 시 기본으로 제공되는 재화 정보 테이블

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| asset_name | VARCHAR(100) | NOT NULL | 재화 이름 |
| asset_amount | BIGINT | NOT NULL, DEFAULT 0 | 재화 수량 |

### initial_mail 테이블
**설명**: 신규 회원 초기 메일

| 컬럼명 | 데이터 타입 | 제약조건 | 설명 |
|-------|-----------|---------|-----|
| mail_id | BIGINT | NOT NULL | 메일 ID |
| status | INT | NOT NULL, DEFAULT 0 | 메일 상태 |
| mail_info | VARCHAR(1024) | NOT NULL | 메일 정보 (JSON) |
| mail_desc | VARCHAR(1024) | NOT NULL | 메일 설명 |
| received_dt | DATETIME | NOT NULL | 수신 일시 |
| expire_dt | DATETIME | NOT NULL | 만료 일시 |
