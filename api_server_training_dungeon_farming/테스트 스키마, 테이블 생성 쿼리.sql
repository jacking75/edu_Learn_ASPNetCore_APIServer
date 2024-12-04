#============================================================================================
# 로컬에서 외부 데이터 Import를 위해 옵션 설정
#============================================================================================
SET GLOBAL LOCAL_INFILE = 1;


#============================================================================================
# 마스터 스키마 관련 쿼리
#============================================================================================
DROP DATABASE IF EXISTS `MasterDB`;
CREATE DATABASE IF NOT EXISTS `MasterDB`;
USE `MasterDB`;


#============================================================================================
# 아이템 정보 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `MasterDB`.`item_info`;
CREATE TABLE IF NOT EXISTS `MasterDB`.`item_info`
(
    `item_code` 			INT 			PRIMARY KEY 				COMMENT '아이템 식별자',
    `item_name` 			VARCHAR(50) 	NOT NULL UNIQUE 			COMMENT '아이템 이름',
    `item_type_code` 		INT 			NOT NULL 					COMMENT '아이템 타입 기획데이터 식별자',
    `item_sale_price`		INT 			NOT NULL 					COMMENT '아이템 판매 가격',
    `item_purchase_price` 	INT 			NOT NULL 					COMMENT '아이템 구매 가격',
    `item_useful_level` 	SMALLINT 									COMMENT '아이템 사용 가능 레벨',
    `item_attack_power`		BIGINT			DEFAULT 0					COMMENT '아이템 공격력',
    `item_defensive_power`	BIGINT			DEFAULT 0					COMMENT '아이템 방어력',
    `item_magic`			BIGINT			DEFAULT 0					COMMENT '아이템 마법',
    `max_enhance_count`		INT				DEFAULT 0					COMMENT '최대 강화 횟수'
) COMMENT '아이템 정보';

# 데이터 Impoort
LOAD DATA LOCAL INFILE "E:/DEV/Education/edu_TrainingGameServer/DungeonFarming/masterData/ItemInfo.csv"
INTO TABLE `MasterDB`.`item_info`
FIELDS TERMINATED BY ","
LINES TERMINATED BY "\n"
IGNORE 1 ROWS;	


#============================================================================================
# 아이템 타입 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `MasterDB`.`item_type`;
CREATE TABLE IF NOT EXISTS `MasterDB`.`item_type`
(
    `item_type_code` 	INT 			PRIMARY KEY 			COMMENT '아이템 타입 식별자',
    `item_type_name` 	VARCHAR(50) 	NOT NULL UNIQUE 		COMMENT '아이템 타입 이름'
) COMMENT '아이템 타입';

# 데이터 Impoort
LOAD DATA LOCAL INFILE "E:/DEV/Education/edu_TrainingGameServer/DungeonFarming/masterData/ItemType.csv"
INTO TABLE `MasterDB`.`item_type`
FIELDS TERMINATED BY ","
LINES TERMINATED BY "\n"
IGNORE 1 ROWS;


#============================================================================================
# 출석 보상 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `MasterDB`.`attendance_event_reward`;
CREATE TABLE  `MasterDB`.`attendance_event_reward`
(
    `days`  		SMALLINT 	NOT NULL    COMMENT '일차', 
    `item_code`   	INT      	NOT NULL    COMMENT '아이템 기획데이터 식별자', 
    `item_count`  	INT      	NOT NULL    COMMENT '아이템 수량'
);

# 데이터 Impoort
LOAD DATA LOCAL INFILE "E:/DEV/Education/edu_TrainingGameServer/DungeonFarming/masterData/AttendanceReward.csv"
INTO TABLE `MasterDB`.`attendance_event_reward`
FIELDS TERMINATED BY ","
LINES TERMINATED BY "\n"
IGNORE 1 ROWS;


#============================================================================================
# 인앱 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `MasterDB`.`inapp_product`;
CREATE TABLE  `MasterDB`.`inapp_product`
(
    `pid`			INT 			NOT NULL	COMMENT '인앱 상품 식별자', 
    `item_code`		INT 			NOT NULL  	COMMENT '아이템 기획데이터 식별자',
    `item_name` 	VARCHAR(50) 	NOT NULL  	COMMENT '아이템 이름',
    `item_count`	INT     		NOT NULL    COMMENT '아이템 수량'
);

# 데이터 Impoort
LOAD DATA LOCAL INFILE "E:/DEV/Education/edu_TrainingGameServer/DungeonFarming/masterData/InAppProduct.csv"
INTO TABLE `MasterDB`.`inapp_product`
FIELDS TERMINATED BY ","
LINES TERMINATED BY "\n"
IGNORE 1 ROWS;


#============================================================================================
# 스테이지 별 적군 정보
#============================================================================================
DROP TABLE IF EXISTS `MasterDB`.`stage_enemy`;
CREATE TABLE  `MasterDB`.`stage_enemy`
(
    `stage_code`	INT 			NOT NULL	COMMENT '스테이지 식별자', 
    `enemy_code`	INT 			NOT NULL  	COMMENT '적 식별자',
    `enemy_count`	INT     		NOT NULL    COMMENT '적 등장 개수?',
    `exp`			INT     		NOT NULL    COMMENT '획득 경험치'
);

# 데이터 Impoort
LOAD DATA LOCAL INFILE "E:/DEV/Education/edu_TrainingGameServer/DungeonFarming/masterData/StageAttackNPC.csv"
INTO TABLE `MasterDB`.`stage_enemy`
FIELDS TERMINATED BY ","
LINES TERMINATED BY "\n"
IGNORE 1 ROWS;


#============================================================================================
# 스테이지 별 획득 가능 아이템
#============================================================================================
DROP TABLE IF EXISTS `MasterDB`.`stage_farming_item`;
CREATE TABLE  `MasterDB`.`stage_farming_item`
(
    `stage_code`	INT 			NOT NULL	COMMENT '스테이지 식별자', 
    `item_code`		INT 			NOT NULL  	COMMENT '아이템 코드'
);

# 데이터 Impoort
LOAD DATA LOCAL INFILE "E:/DEV/Education/edu_TrainingGameServer/DungeonFarming/masterData/StageFarmingItem.csv"
INTO TABLE `MasterDB`.`stage_farming_item`
FIELDS TERMINATED BY ","
LINES TERMINATED BY "\n"
IGNORE 1 ROWS;


#============================================================================================
# 계정 스키마 관련 쿼리
#============================================================================================
DROP DATABASE IF EXISTS `AccountDB`;
CREATE DATABASE IF NOT EXISTS `AccountDB`;
USE `AccountDB`;


#============================================================================================
# 계정 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `AccountDB`.`account`;
CREATE TABLE IF NOT EXISTS `AccountDB`.`account`
(
    `account_id`		BIGINT 			PRIMARY KEY AUTO_INCREMENT  	COMMENT '계정 UID',
    `user_id` 			BIGINT 			NOT NULL 	UNIQUE 				COMMENT '유저 UID',
    `email`				VARCHAR(50) 	NOT NULL 	UNIQUE 				COMMENT '이메일',
    `password` 			VARCHAR(100) 	NOT NULL 						COMMENT '비밀번호',
    `salt_value` 		VARCHAR(100) 	NOT NULL 						COMMENT '암호화 값',
    `join_date` 		DATETIME 		DEFAULT CURRENT_TIMESTAMP()		COMMENT '카입 일시'
) COMMENT '계정 테이블';


#============================================================================================
# 게임 스키마 관련 쿼리
#============================================================================================
DROP DATABASE IF EXISTS `GameDB`;
CREATE DATABASE IF NOT EXISTS `GameDB`;
USE `GameDB`;


#============================================================================================
# 게임데이터 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `GameDB`.`user_play_data`;
CREATE TABLE IF NOT EXISTS `GameDB`.`user_play_data`
(
    `user_id` 			BIGINT 			PRIMARY KEY AUTO_INCREMENT  			COMMENT '유저 식별자',
    `level` 			SMALLINT 		NOT NULL 								COMMENT '레벨',
    `exp` 				INT 			NOT NULL 								COMMENT '경험치',
    `create_date` 		DATETIME 		NOT NULL DEFAULT CURRENT_TIMESTAMP()	COMMENT '생성 일시'
) COMMENT '유저 플레이 데이터';


#============================================================================================
# 인벤토리 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `GameDB`.`user_inventory_item`;
CREATE TABLE IF NOT EXISTS `GameDB`.`user_inventory_item`
(
    `inventory_item_id` 		BIGINT 		PRIMARY KEY AUTO_INCREMENT  			COMMENT '인벤토리 아이템 식별자',
    `user_id` 					BIGINT 		NOT NULL 								COMMENT '유저 식별자',
    `item_code`					INT 		NOT NULL 								COMMENT '아이템 기획데이터 식별자',
    `item_attack_power`			BIGINT		DEFAULT 0 								COMMENT '아이템 공격력',
    `item_defensive_power`		BIGINT		DEFAULT 0 								COMMENT '아이템 방어력',
    `item_magic`				BIGINT		DEFAULT 0 								COMMENT '아이템 마법',
    `enhance_stage`				SMALLINT	DEFAULT 0								COMMENT '강화 단계',
    `item_count` 				INT 		NOT NULL DEFAULT 0						COMMENT '아이템 수량'
) COMMENT '유저 인벤토리'; 


#============================================================================================
# 메일함 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `GameDB`.`user_mailbox`;
CREATE TABLE IF NOT EXISTS `GameDB`.`user_mailbox`
(
	`mail_id`			BIGINT 			PRIMARY KEY AUTO_INCREMENT 					COMMENT '우편 순번',
    `user_id`			BIGINT 			NOT NULL									COMMENT '유저 식별자',
    `mail_type` 		SMALLINT 		NOT NULL 									COMMENT '우편 타입',
    `mail_title`		VARCHAR(300)	NOT NULL									COMMENT '우편 제목',
    `item_code`			INT 			NOT NULL									COMMENT '아이템 기획데이터 식별자',
    `item_count`		INT 			NOT NULL									COMMENT '아이템 개수',
    `is_receive`		BOOL			NOT NULL DEFAULT FALSE						COMMENT '수령 여부',
    `send_date`			DATETIME		NOT NULL DEFAULT CURRENT_TIMESTAMP()		COMMENT '발송 일시',
    `receive_date`		DATETIME													COMMENT '수령 일시',
    `expire_date`		DATETIME		NOT NULL 									COMMENT '만료 일시'
) COMMENT '유저 메일함';


#============================================================================================
# 출석부 테이블 관련 쿼리
#============================================================================================
DROP TABLE IF EXISTS `GameDB`.`user_attendance_book`;
CREATE TABLE `GameDB`.`user_attendance_book`
(
	`attendance_book_id`	BIGINT 		PRIMARY KEY AUTO_INCREMENT 		COMMENT '유저 출석부 식별자',
    `user_id`				BIGINT 		NOT NULL						COMMENT '유저 식별자',
    `last_attendance_day`   SMALLINT         	 						COMMENT '마지막 출석일',
    `start_update_date`   	DATETIME       								COMMENT '출석 시작 일자',
    `last_update_date` 		DATETIME      				     			COMMENT '마지막 갱신 일자'
) COMMENT '유저 출석부';


#============================================================================================
# 인앱 상품 지급 이력
#============================================================================================
DROP TABLE IF EXISTS `GameDB`.`user_inapp_purchase_history`;
CREATE TABLE `GameDB`.`user_inapp_purchase_history`
(
	`history_id`	BIGINT			PRIMARY KEY AUTO_INCREMENT	COMMENT '인앱 구매 히스토리 식별자',
    `user_id`		BIGINT			NOT NULL					COMMENT '유저 식별자',
    `pid`			INT				NOT NULL					COMMENT '인앱 기획데이터 식별자',
    `receipt`   	VARCHAR(100)	NOT NULL UNIQUE  	 		COMMENT '영수증',
    `receive_date`	DATETIME       	DEFAULT CURRENT_TIMESTAMP()	COMMENT '구매 일자'
) COMMENT '유저 인앱 상품 지급 이력';


#============================================================================================
# 유저가 완료한 스테이지 정보
#============================================================================================
DROP TABLE IF EXISTS `GameDB`.`user_cleared_dungeon_stage`;
CREATE TABLE `GameDB`.`user_cleared_dungeon_stage`
(
    `user_id`			BIGINT			NOT NULL								COMMENT '유저 식별자',
    `stage_code`		INT				NOT NULL								COMMENT '완료된 스테이지',
    `cleared_date`		DATETIME       	NOT NULL	DEFAULT CURRENT_TIMESTAMP()	COMMENT '완료 일자',
     PRIMARY KEY (`user_id`, `stage_code`)
) COMMENT '유저가 완료한 스테이지 정보';