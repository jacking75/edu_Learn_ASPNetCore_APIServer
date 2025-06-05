-- 첫 번째 데이터베이스는 MYSQL_DATABASE 환경변수로 이미 생성됨 (hive_db)
-- 추가 데이터베이스 생성
CREATE DATABASE IF NOT EXISTS master_db;
CREATE DATABASE IF NOT EXISTS game_db;

-- 각 데이터베이스에 대한 권한 설정
GRANT ALL PRIVILEGES ON hive_db.* TO 'root'@'%';
GRANT ALL PRIVILEGES ON master_db.* TO 'root'@'%';
GRANT ALL PRIVILEGES ON game_db.* TO 'root'@'%';
FLUSH PRIVILEGES;

use hive_db;

-- 사용자 회원가입 및 로그인을 저장하는 테이블

CREATE TABLE `account` (
    `account_uid` BIGINT NOT NULL AUTO_INCREMENT COMMENT '유니크 유저 번호',
    `email_id` VARCHAR(50) NOT NULL COMMENT '유저아이디. 내용은 이메일',
    `nickname` VARCHAR(50) NOT NULL COMMENT '유저 닉네임',
    `pw` VARCHAR(100) NOT NULL COMMENT '해싱된 비밀번호',
    `salt_value` VARCHAR(100) NOT NULL COMMENT '암호화 값',
    `create_dt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '생성 일시',
    PRIMARY KEY (`account_uid`),
    UNIQUE KEY `idx_email_id` (`email_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

use game_db;

-- 사용자 기본 정보를 저장하는 테이블
CREATE TABLE `user` (
    `account_uid` BIGINT NOT NULL,
    `main_deck_id` Int NOT NULL COMMENT '메인 덱 정보',
    `last_login_dt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '마지막 로그아웃 시간',    
    PRIMARY KEY (`account_uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 사용자 기본 정보를 저장하는 테이블
CREATE TABLE `user_deck` (
    `account_uid` BIGINT NOT NULL,
    `deck_id` Int NOT NULL,
    `deck_list` VARCHAR(1024) NOT NULL,
    `create_dt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`account_uid`, `deck_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 사용자 재화 정보를 저장하는 테이블
CREATE TABLE `user_asset` (
    `account_uid` BIGINT NOT NULL,
    `asset_name` VARCHAR(100) NOT NULL,
    `asset_amount` BIGINT NOT NULL DEFAULT 0,
    PRIMARY KEY (`account_uid`, `asset_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 사용자 출석 정보를 저장하는 테이블
CREATE TABLE `user_attendance` (
    `account_uid` BIGINT NOT NULL,
    `event_id` INT NOT NULL DEFAULT 0,
    `attendance_no` INT NOT NULL DEFAULT 0,
    `attendance_dt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '최근 업데이트 시간',    
    PRIMARY KEY (`account_uid`, `event_id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `user_item` (
    `account_uid` BIGINT NOT NULL,
    `item_id` INT NOT NULL,
    `item_cnt` INT NOT NULL,
    PRIMARY KEY (`account_uid`, `item_id`),
    INDEX `idx_user_user_id` (`account_uid`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='사용자 카드 아이템 정보';

CREATE TABLE `user_mail` (
    `account_uid` BIGINT NOT NULL,
    `mail_id` BIGINT NOT NULL,
    `status` INT NOT NULL DEFAULT 0,
    `reward_key` INT NOT NULL,
    `mail_desc` VARCHAR(1024) NOT NULL,
    `received_dt` DATETIME NOT NULL,
    `expire_dt` DATETIME NOT NULL,
    PRIMARY KEY (`account_uid`, `mail_id`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='사용자 이메일 정보';


use master_db;

-- 데이터 버전 정보를 저장한 테이블
CREATE TABLE IF NOT EXISTS `version` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `app_version` VARCHAR(20) NOT NULL,
    `master_data_version` VARCHAR(20) NOT NULL,
    `create_dt` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 아이템 데이터 정보를 저장한 테이블
CREATE TABLE IF NOT EXISTS `item` (
    `item_id` BIGINT NOT NULL,
    `item_grade_code` CHAR(2) NOT NULL, -- 전설, 영웅, 희귀, 일반, 무료
    `item_type` TINYINT NOT NULL, 
    `ability_key` INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 어빌리티 데이터 정보를 저장한 테이블
CREATE TABLE IF NOT EXISTS `ability` (
    `ability_key` INT NOT NULL,
    `ability_type` VARCHAR(8) NOT NULL,
    `ability_value` BIGINT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 아이템 랜덤 뽑기 정보를 저장한 테이블
CREATE TABLE IF NOT EXISTS `gacha_info` (
    `gacha_key` INT NOT NULL,    
    `count` INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 아이템 랜덤 뽑기 확률를 저장한 테이블
CREATE TABLE IF NOT EXISTS `gacha_rate` (
    `gacha_key` INT NOT NULL,    
    `item_id` INT NOT NULL,
    `rate` BIGINT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 출석 정보를 저장한 테이블 - 유료, 무료
CREATE TABLE IF NOT EXISTS `attendance_info` (
    `event_id` INT NOT NULL,
    `free_yn` bit NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 출석 보상 정보를 저장한 테이블
CREATE TABLE IF NOT EXISTS `attendance_reward` (
    `day_seq` INT NOT NULL,
    `event_id` INT NOT NULL,
    `reward_key` INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 보상 정보를 저장한 테이블
CREATE TABLE IF NOT EXISTS `reward_info` (
    `reward_key` INT NOT NULL,
    `reward_class` VARCHAR(20) NOT NULL,
    `reward_type` VARCHAR(20) NOT NULL,
    `reward_value` BIGINT NOT NULL
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 회원가입 시 무료로 제공되는 초기 카드 정보 테이블
CREATE TABLE IF NOT EXISTS `initial_free_items` (
    `item_id` INT  NOT NULL,
    `item_cnt` INT NOT NULL DEFAULT 1
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 회원가입 시 기본으로 제공되는 재화 정보 테이블
CREATE TABLE IF NOT EXISTS `initial_asset` (
     `asset_name` VARCHAR(100) NOT NULL,
     `asset_amount` BIGINT NOT NULL DEFAULT 0
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `initial_mail` (
    `mail_id` BIGINT NOT NULL,
    `status` INT NOT NULL DEFAULT 0,
    `reward_key` INT NOT NULL,
    `mail_desc` VARCHAR(1024) NOT NULL,
    `received_dt` DATETIME NOT NULL,
    `expire_dt` DATETIME NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='신규 회원 초기 메일';


CREATE TABLE IF NOT EXISTS `shop` (
    `shop_id` INT NOT NULL,    
    `gacha_key` INT NOT NULL,    
    `asset_name` VARCHAR(100) NOT NULL,    
    `asset_amount` BIGINT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


USE master_db;

-- 1. VERSION 테이블 (20개)
INSERT INTO `version` (`app_version`, `master_data_version`) VALUES 
('1.0.0', '2025-01-01'),
('1.0.1', '2025-01-15'),
('1.0.2', '2025-02-01'),
('1.1.0', '2025-02-15'),
('1.1.1', '2025-03-01'),
('1.1.2', '2025-03-15'),
('1.2.0', '2025-04-01'),
('1.2.1', '2025-04-15'),
('1.2.2', '2025-05-01'),
('1.3.0', '2025-05-15'),
('1.3.1', '2025-06-01'),
('1.3.2', '2025-06-15'),
('1.4.0', '2025-07-01'),
('1.4.1', '2025-07-15'),
('1.4.2', '2025-08-01'),
('1.5.0', '2025-08-15'),
('1.5.1', '2025-09-01'),
('1.5.2', '2025-09-15'),
('1.6.0', '2025-10-01'),
('1.6.1', '2025-10-15');

INSERT INTO `ability` (`ability_key`, `ability_type`, `ability_value`) VALUES
-- 1턴 하수인 카드 (1 마나)
(1001, 'attack', 1), (1001, 'hp', 1), (1001, 'mana', 1),
(1002, 'attack', 1), (1002, 'hp', 2), (1002, 'mana', 1),
(1003, 'attack', 1), (1003, 'hp', 3), (1003, 'mana', 1),
(1004, 'attack', 2), (1004, 'hp', 1), (1004, 'mana', 1),
(1005, 'attack', 2), (1005, 'hp', 2), (1005, 'mana', 1),

-- 2턴 하수인 카드 (2 마나)
(1006, 'attack', 2), (1006, 'hp', 3), (1006, 'mana', 2),
(1007, 'attack', 3), (1007, 'hp', 2), (1007, 'mana', 2),
(1008, 'attack', 1), (1008, 'hp', 4), (1008, 'mana', 2),
(1009, 'attack', 3), (1009, 'hp', 1), (1009, 'mana', 2),

-- 3턴 하수인 카드 (3 마나)
(1010, 'attack', 3), (1010, 'hp', 3), (1010, 'mana', 3),
(1011, 'attack', 2), (1011, 'hp', 4), (1011, 'mana', 3),
(1012, 'attack', 4), (1012, 'hp', 2), (1012, 'mana', 3),
(1013, 'attack', 1), (1013, 'hp', 6), (1013, 'mana', 3),
(1014, 'attack', 3), (1014, 'hp', 4), (1014, 'mana', 3),

-- 추가 조합 (다양한 밸런스)
(1015, 'attack', 1), (1015, 'hp', 1), (1015, 'mana', 2), -- 약한 2코스트
(1016, 'attack', 2), (1016, 'hp', 1), (1016, 'mana', 3), -- 약한 3코스트
(1017, 'attack', 1), (1017, 'hp', 2), (1017, 'mana', 3), -- 약한 3코스트
(1018, 'attack', 3), (1018, 'hp', 3), (1018, 'mana', 2), -- 강한 2코스트
(1019, 'attack', 2), (1019, 'hp', 3), (1019, 'mana', 1), -- 강한 1코스트

-- 특별 하수인 (균형 잡힌)
(1020, 'attack', 2), (1020, 'hp', 2), (1020, 'mana', 2), -- 2/2/2
(1021, 'attack', 3), (1021, 'hp', 3), (1021, 'mana', 3), -- 3/3/3
(1022, 'attack', 1), (1022, 'hp', 1), (1022, 'mana', 1), -- 1/1/1

-- 희귀한 조합
(1023, 'attack', 3), (1023, 'hp', 2), (1023, 'mana', 1), -- 공격형 1코스트
(1024, 'attack', 1), (1024, 'hp', 3), (1024, 'mana', 2), -- 방어형 2코스트
(1025, 'attack', 3), (1025, 'hp', 1), (1025, 'mana', 3), -- 공격형 3코스트
(1026, 'attack', 2), (1026, 'hp', 5), (1026, 'mana', 3), -- 방어형 3코스트
(1027, 'attack', 4), (1027, 'hp', 4), (1027, 'mana', 3); -- 강한 3코스트


-- 2. ITEM 테이블 (20개)
-- item_grade_code: LE(전설), EP(영웅), RA(희귀), CO(일반), FR(무료)
-- item_type: 0(영웅), 1(미니언), 2(주문), 3(무기), 4(아티팩트), 5(팩)
-- 2. item 테이블에 미니언 20개 추가 (ability_key와 매칭)
INSERT INTO `item` (`item_id`, `item_grade_code`, `item_type`, `ability_key`) VALUES
-- 1코스트(마나) 하수인 카드
(1001, 'CO', 1, 1001), -- 1/1/1 일반 하수인
(1002, 'CO', 1, 1002), -- 1/2/1 일반 하수인
(1003, 'CO', 1, 1003), -- 1/3/1 일반 하수인
(1004, 'RA', 1, 1004), -- 2/1/1 희귀 하수인 (공격형)
(1005, 'RA', 1, 1005), -- 2/2/1 희귀 하수인 (밸런스)
(1019, 'RA', 1, 1019), -- 2/3/1 희귀 하수인 (방어형)
(1023, 'EP', 1, 1023), -- 3/2/1 영웅 하수인 (강한 1코스트)

-- 2코스트(마나) 하수인 카드
(1006, 'CO', 1, 1006), -- 2/3/2 일반 하수인
(1007, 'CO', 1, 1007), -- 3/2/2 일반 하수인
(1008, 'RA', 1, 1008), -- 1/4/2 희귀 하수인 (방어형)
(1009, 'RA', 1, 1009), -- 3/1/2 희귀 하수인 (공격형)
(1015, 'FR', 1, 1015), -- 1/1/2 무료 하수인 (약함)
(1018, 'EP', 1, 1018), -- 3/3/2 영웅 하수인 (강함)
(1020, 'RA', 1, 1020), -- 2/2/2 희귀 하수인 (밸런스)
(1024, 'CO', 1, 1024), -- 1/3/2 일반 하수인 (방어형)

-- 3코스트(마나) 하수인 카드
(1010, 'CO', 1, 1010), -- 3/3/3 일반 하수인
(1011, 'CO', 1, 1011), -- 2/4/3 일반 하수인
(1012, 'RA', 1, 1012), -- 4/2/3 희귀 하수인 (공격형)
(1013, 'RA', 1, 1013), -- 1/6/3 희귀 하수인 (방어형)
(1014, 'EP', 1, 1014), -- 3/4/3 영웅 하수인 (강함)
(1016, 'FR', 1, 1016), -- 2/1/3 무료 하수인 (약함)
(1017, 'FR', 1, 1017), -- 1/2/3 무료 하수인 (약함)
(1021, 'EP', 1, 1021), -- 3/3/3 영웅 하수인 (밸런스)
(1025, 'RA', 1, 1025), -- 3/1/3 희귀 하수인 (공격형)
(1026, 'EP', 1, 1026), -- 2/5/3 영웅 하수인 (방어형)
(1027, 'LE', 1, 1027); -- 4/4/3 전설 하수인 (매우 강함)

-- 4. GACHA_INFO 테이블 (20개)
-- gacha_key: 가챠 고유 키, count: 이 가챠를 통해 나올 아이템 개수
INSERT INTO `gacha_info` (`gacha_key`, `count`) VALUES 
(121, 1);  -- 하수인 카드팩 (5장)


-- gacha_key: 가챠 고유 키, count: 이 가챠를 통해 나올 아이템 개수
INSERT INTO `shop` (`shop_id`, `gacha_key`,`asset_name`, `asset_amount`) VALUES 
(1, 121, 'gold', 5);  -- 하수인 카드팩 (5장)

-- 5. GACHA_RATE 테이블 (20개)
-- 미니언 20종 전용 가챠 레이트 (gacha_key 120)
INSERT INTO `gacha_rate` (`gacha_key`, `item_id`, `rate`) VALUES
-- 1코스트 하수인 카드 (7장)
(121, 1001, 750),  -- CO - 일반 하수인 1/1/1
(121, 1002, 750),  -- CO - 일반 하수인 1/2/1
(121, 1003, 750),  -- CO - 일반 하수인 1/3/1
(121, 1004, 375),  -- RA - 희귀 하수인 2/1/1
(121, 1005, 375),  -- RA - 희귀 하수인 2/2/1
(121, 1019, 375),  -- RA - 희귀 하수인 2/3/1
(121, 1023, 75),   -- EP - 영웅 하수인 3/2/1

-- 2코스트 하수인 카드 (8장)
(121, 1006, 625),  -- CO - 일반 하수인 2/3/2
(121, 1007, 625),  -- CO - 일반 하수인 3/2/2
(121, 1008, 312),  -- RA - 희귀 하수인 1/4/2
(121, 1009, 312),  -- RA - 희귀 하수인 3/1/2
(121, 1015, 475),  -- FR - 무료 하수인 1/1/2
(121, 1018, 62),   -- EP - 영웅 하수인 3/3/2
(121, 1020, 313),  -- RA - 희귀 하수인 2/2/2
(121, 1024, 625),  -- CO - 일반 하수인 1/3/2

-- 3코스트 하수인 카드 (11장)
(121, 1010, 455),  -- CO - 일반 하수인 3/3/3
(121, 1011, 455),  -- CO - 일반 하수인 2/4/3
(121, 1012, 227),  -- RA - 희귀 하수인 4/2/3
(121, 1013, 228),  -- RA - 희귀 하수인 1/6/3
(121, 1014, 45),   -- EP - 영웅 하수인 3/4/3
(121, 1016, 475),  -- FR - 무료 하수인 2/1/3
(121, 1017, 475),  -- FR - 무료 하수인 1/2/3
(121, 1021, 45),   -- EP - 영웅 하수인 3/3/3
(121, 1025, 228),  -- RA - 희귀 하수인 3/1/3
(121, 1026, 45),   -- EP - 영웅 하수인 2/5/3
(121, 1027, 10);   -- LE - 전설 하수인 4/4/3

-- 6. ATTENDANCE_INFO 테이블 (20개)
INSERT INTO `attendance_info` (`event_id`, `free_yn`) VALUES 
(1, 1),  -- 일반 출석 이벤트 (무료)
(2, 1),  -- 신규 유저 출석 이벤트 (무료)
(101, 0), -- 프리미엄 패스 출석 (유료)
(102, 0), -- 시즌 패스 출석 (유료)
(103, 0), -- 배틀 패스 출석 (유료)
(104, 0), -- VIP 출석 혜택 (유료)
(105, 0), -- 골드 패스 출석 (유료)
(106, 0), -- 여름 스페셜 패스 (유료)
(107, 0), -- 겨울 스페셜 패스 (유료)
(108, 0), -- 1주년 스페셜 패스 (유료)
(109, 0), -- 2주년 스페셜 패스 (유료)
(110, 0); -- 리미티드 이벤트 패스 (유료)

-- 7. ATTENDANCE_REWARD 테이블 (20개)
INSERT INTO `attendance_reward` (`day_seq`, `event_id`, `reward_key`) VALUES 
-- 일반 출석 이벤트 보상 (event_id 1)
(1, 1, 101), -- 1일차: 골드 100
(2, 1, 102), -- 2일차: 골드 200
(3, 1, 103), -- 3일차: 골드 300
(4, 1, 104), -- 4일차: 골드 400
(5, 1, 105), -- 5일차: 골드 500
(6, 1, 106), -- 6일차: 골드 600
(7, 1, 201), -- 7일차: 다이아 100

-- 신규 유저 출석 이벤트 보상 (event_id 2)
(1, 2, 101), -- 1일차: 골드 100
(2, 2, 102), -- 2일차: 골드 200
(3, 2, 201), -- 3일차: 다이아 100
(4, 2, 302), -- 4일차: 카드팩 1개
(5, 2, 103), -- 5일차: 골드 300
(6, 2, 401), -- 6일차: 희귀 카드 1장
(7, 2, 501), -- 7일차: 전설 카드 1장

-- 프리미엄 패스 출석 보상 (event_id 101)
(1, 101, 103), -- 1일차: 골드 300
(2, 101, 201), -- 2일차: 다이아 100
(3, 101, 204), -- 3일차: 다이아 400
(4, 101, 302), -- 4일차: 카드팩 1개
(5, 101, 303), -- 5일차: 카드팩 2개
(6, 101, 502); -- 6일차: 전설 카드 2장

-- ATTENDANCE_REWARD 테이블 추가 데이터 (총 40개가 되도록)
INSERT INTO `attendance_reward` (`day_seq`, `event_id`, `reward_key`) VALUES 
-- 월간 출석 이벤트 보상 (event_id 5)
(1, 5, 101),  -- 1일차: 골드 100
(5, 5, 102),  -- 5일차: 골드 200
(10, 5, 103), -- 10일차: 골드 300
(15, 5, 201), -- 15일차: 다이아 100
(20, 5, 302), -- 20일차: 카드팩 1개
(25, 5, 303), -- 25일차: 카드팩 2개
(30, 5, 501), -- 30일차: 전설 카드 1장

-- 시즌 패스 출석 보상 (event_id 102)
(1, 102, 103),  -- 1일차: 골드 300
(3, 102, 201),  -- 3일차: 다이아 100
(5, 102, 302),  -- 5일차: 카드팩 1개
(7, 102, 402),  -- 7일차: 희귀 카드 2장
(10, 102, 204), -- 10일차: 다이아 400
(15, 102, 303), -- 15일차: 카드팩 2개
(20, 102, 501), -- 20일차: 전설 카드 1장

-- 1주년 출석 이벤트 보상 (event_id 9)
(1, 9, 105),  -- 1일차: 골드 500
(2, 9, 202),  -- 2일차: 다이아 200
(3, 9, 302),  -- 3일차: 카드팩 1개
(4, 9, 402),  -- 4일차: 희귀 카드 2장
(5, 9, 105),  -- 5일차: 골드 500
(6, 9, 203),  -- 6일차: 다이아 300
(7, 9, 501);  -- 7일차: 전설 카드 1장

-- 8. REWARD_INFO 테이블 (20개)
INSERT INTO `reward_info` (`reward_key`, `reward_class`, `reward_type`, `reward_value`) VALUES 
-- 골드 보상
(101, 'currency', 'gold', 100),
(102, 'currency', 'gold', 200),
(103, 'currency', 'gold', 300),
(104, 'currency', 'gold', 400),
(105, 'currency', 'gold', 500),
(106, 'currency', 'gold', 600),
(107, 'currency', 'gold', 700),
(108, 'currency', 'gold', 800),
(109, 'currency', 'gold', 900),
(110, 'currency', 'gold', 1000),

-- 다이아 보상
(201, 'currency', 'diamond', 100),
(202, 'currency', 'diamond', 200),
(203, 'currency', 'diamond', 300),
(204, 'currency', 'diamond', 400),
(205, 'currency', 'diamond', 500),

-- 아이템 보상 (카드팩, 카드 등)
(301, 'item', '9001', 1), -- 일반 카드팩 1개
(302, 'item', '9002', 1), -- 희귀 카드팩 1개
(303, 'item', '9002', 2), -- 희귀 카드팩 2개
(401, 'item', '2001', 1), -- 희귀 미니언 카드 1장
(402, 'item', '2001', 2); -- 희귀 미니언 카드 2장

-- REWARD_INFO 테이블 추가 데이터 (총 40개가 되도록)
INSERT INTO `reward_info` (`reward_key`, `reward_class`, `reward_type`, `reward_value`) VALUES 
-- 골드 보상 추가
(111, 'currency', 'gold', 1500),
(112, 'currency', 'gold', 2000),
(113, 'currency', 'gold', 2500),
(114, 'currency', 'gold', 3000),
(115, 'currency', 'gold', 5000),

-- 다이아 보상 추가
(206, 'currency', 'diamond', 600),
(207, 'currency', 'diamond', 800),
(208, 'currency', 'diamond', 1000),
(209, 'currency', 'diamond', 1500),
(210, 'currency', 'diamond', 2000),

-- 아이템 보상 추가 (영웅, 전설 카드 등)
(403, 'item', '2002', 1), -- 희귀 미니언 카드 1장 (다른 종류)
(404, 'item', '2002', 2), -- 희귀 미니언 카드 2장 (다른 종류)
(405, 'item', '1003', 1), -- 영웅급 영웅 카드 1장
(406, 'item', '1004', 1), -- 영웅급 영웅 카드 1장 (다른 종류)
(501, 'item', '1001', 1), -- 전설 영웅 카드 1장
(502, 'item', '1001', 2), -- 전설 영웅 카드 2장
(503, 'item', '1002', 1), -- 전설 영웅 카드 1장 (다른 종류)
(504, 'item', '3001', 1), -- 전설 주문 카드 1장
(505, 'item', '4001', 1), -- 전설 무기 카드 1장
(506, 'item', '5001', 1); -- 전설 아티팩트 카드 1장

-- 9. INITIAL_FREE_ITEMS 테이블 (5개)
INSERT INTO `initial_free_items` (`item_id`, `item_cnt`) VALUES 
(1001, 1), -- 영웅급 영웅 카드 1장
(1002, 1), -- 일반 카드팩 1개
(1003, 1), -- 일반 카드팩 1개
(1004, 1), -- 일반 카드팩 1개
(1005, 1), -- 일반 카드팩 1개
(1006, 1), -- 일반 카드팩 1개
(1007, 1), -- 일반 카드팩 1개
(1008, 1), -- 일반 카드팩 1개
(1009, 1), -- 일반 카드팩 1개
(1015, 1); -- 일반 카드팩 1개

-- 10. INITIAL_MAIL 테이블 (5개)
-- 10. INITIAL_MAIL 테이블 (5개)
INSERT INTO `initial_mail` (`mail_id`, `status`, `reward_key`, `mail_desc`, `received_dt`, `expire_dt`) VALUES 
(1000001, 0, 105, 'Welcome! Here 500 gold as our gift to you.', '2025-05-14 10:00:00', '2026-05-14 10:00:00'),
(1000002, 0, 201, 'Welcome! Enjoy 100 diamonds as our gift.', '2025-05-14 10:00:00', '2026-05-14 10:00:00'),
(1000003, 0, 301, 'Welcome! We sent you a card pack as a gift.', '2025-05-14 10:00:00', '2026-05-14 10:00:00'),
(1000004, 0, 405, 'Welcome! Here an epic hero card for you.', '2025-05-14 10:00:00', '2026-05-14 10:00:00'),
(1000005, 0, 302, 'Pre-registration special reward! Here a rare card pack for you.', '2025-05-14 10:00:00', '2026-05-14 10:00:00');
