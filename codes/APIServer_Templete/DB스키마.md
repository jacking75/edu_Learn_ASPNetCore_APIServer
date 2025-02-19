아래 사용 예를 참고하여 만드는 게임에 맞게 DB 스키마 정보를 만들도록 한다.  
사용하지 않는 것은 삭제한다.  
  
  
  
# account DB

## account_info 테이블
하이브 계정 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - account
CREATE TABLE account
(
    `player_id`         BIGINT          NOT NULL    AUTO_INCREMENT COMMENT '플레이어 아이디',
    `user_id`             VARCHAR(50)     NOT NULL    COMMENT '유저아이디. 내용은 이메일',
    `salt_value`        VARCHAR(100)    NOT NULL    COMMENT '암호화 값',
    `pw`                VARCHAR(100)    NOT NULL    COMMENT '해싱된 비밀번호',
    `create_dt`         DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP COMMENT '생성 일시',
    `recent_login_dt`   DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP COMMENT '최근 로그인 일시',
     PRIMARY KEY (player_id),
     UNIQUE KEY (user_id)
)
```

# game DB
  
## user_info 테이블
게임에서 생성 된 계정 정보들을 가지고 있는 테이블    
  
```sql
-- 테이블 생성 SQL - user
CREATE TABLE user
(
    `uid`                           INT            NOT NULL    AUTO_INCREMENT COMMENT '유저아이디', 
    `player_id`                     BIGINT         NOT NULL    COMMENT '플레이어 아이디', 
    `nickname`                      VARCHAR(50)    NOT NULL    COMMENT '닉네임', 
    `create_dt`                     DATETIME       NOT NULL    DEFAULT CURRENT_TIMESTAMP COMMENT '생성 일시', 
    `recent_login_dt`               DATETIME       NOT NULL    DEFAULT CURRENT_TIMESTAMP COMMENT '최근 로그인 일시', 
    `total_bestscore`               INT            NOT NULL    DEFAULT 0 COMMENT '최고점수 역대', 
    `total_bestscore_cur_season`    INT            NOT NULL    DEFAULT 0 COMMENT '최고점수 현재 시즌', 
    `total_bestscore_prev_season`   INT            NOT NULL    DEFAULT 0 COMMENT '최고점수 이전 시즌', 
    `star_point`                    INT            NOT NULL    DEFAULT 0 COMMENT '스타 포인트', 
    `main_char_key`                 INT            NOT NULL    COMMENT '메인 캐릭터 키',
     PRIMARY KEY (uid),
     UNIQUE KEY (nickname)
);

```

## user_money 테이블
유저의 재화 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_money
CREATE TABLE user_money
(
    `uid`         INT    NOT NULL    COMMENT '유저아이디', 
    `jewelry`     INT    NOT NULL    DEFAULT 0 COMMENT '보석', 
    `gold_medal`  INT    NOT NULL    DEFAULT 0 COMMENT '금 메달', 
    `cash`        INT    NOT NULL    DEFAULT 0 COMMENT '현금', 
    `sunchip`     INT    NOT NULL    DEFAULT 0 COMMENT '썬칩', 
     PRIMARY KEY (uid)
);
-- Foreign Key 설정 SQL - user_money(uid) -> user(uid)
ALTER TABLE user_money
    ADD CONSTRAINT FK_user_money_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## friend 테이블
친구 정보를 가지고 있는 테이블  
```sql
-- 테이블 생성 SQL - friend
CREATE TABLE friend
(
    `uid`         INT         NOT NULL    COMMENT '유저아이디', 
    `friend_uid`  INT         NOT NULL    COMMENT '친구 유저아이디', 
    `friend_yn`   TINYINT     NOT NULL    DEFAULT 0  COMMENT '친구 여부', 
    `create_dt`   DATETIME    NOT NULL    DEFAULT CURRENT_TIMESTAMP COMMENT '생성 일시', 
     PRIMARY KEY (uid, friend_uid)
);
-- Foreign Key 설정 SQL - friend(uid) -> user(uid)
ALTER TABLE friend
    ADD CONSTRAINT FK_friend_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;

-- Foreign Key 설정 SQL - friend(friend_uid) -> user(uid)
ALTER TABLE friend
    ADD CONSTRAINT FK_friend_friend_uid_user_uid FOREIGN KEY (friend_uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## mailbox 테이블
우편함 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - mailbox
CREATE TABLE mailbox
(
    `mail_seq`    INT             NOT NULL    AUTO_INCREMENT COMMENT '우편 일련번호', 
    `uid`         INT             NOT NULL    COMMENT '유저아이디', 
    `mail_title`  VARCHAR(100)    NOT NULL    COMMENT '우편 제목', 
    `create_dt`   DATETIME        NOT NULL    COMMENT '생성 일시', 
    `expire_dt`   DATETIME        NOT NULL    COMMENT '만료 일시', 
    `receive_dt`  DATETIME        NOT NULL    DEFAULT CURRENT_TIMESTAMP COMMENT '수령 일시', 
    `receive_yn`  TINYINT         NOT NULL    DEFAULT 0 COMMENT '수령 유무',
     PRIMARY KEY (mail_seq)
);
-- Foreign Key 설정 SQL - mailbox(uid) -> user(uid)
ALTER TABLE mailbox
    ADD CONSTRAINT FK_mailbox_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## mailbox_reward 테이블
우편함의 보상정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - mailbox_reward
CREATE TABLE mailbox_reward
(
    `mail_seq`     INT            NOT NULL    COMMENT '우편 일련번호', 
    `reward_key`   INT            NOT NULL    COMMENT '보상 키', 
    `reward_qty`   INT            NOT NULL    COMMENT '보상 수', 
    `reward_type`  VARCHAR(20)    NOT NULL    COMMENT '보상 타입', 
     PRIMARY KEY (mail_seq, reward_key)
);

-- Foreign Key 설정 SQL - mailbox_reward(mail_seq) -> mailbox(mail_seq)
ALTER TABLE mailbox_reward
    ADD CONSTRAINT FK_mailbox_reward_mail_seq_mailbox_mail_seq FOREIGN KEY (mail_seq)
        REFERENCES mailbox (mail_seq) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_attendance 테이블
유저의 출석 현황을 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_attendance
CREATE TABLE user_attendance
(
    `uid`                   INT         NOT NULL    COMMENT '유저아이디', 
    `attendance_cnt`        INT         NOT NULL    COMMENT '출석 횟수', 
    `recent_attendance_dt`  DATETIME    NOT NULL    COMMENT '최근 출석 일시', 
     PRIMARY KEY (uid)
);
-- Foreign Key 설정 SQL - user_attendance(uid) -> user(uid)
ALTER TABLE user_attendance
    ADD CONSTRAINT FK_user_attendance_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_minigame 테이블
유저의 게임 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_minigame
CREATE TABLE user_minigame
(
    `uid`                    INT         NOT NULL    COMMENT '유저아이디', 
    `game_key`               INT         NOT NULL    COMMENT '게임 키', 
    `bestscore`              INT         NOT NULL    COMMENT '최고점수', 
    `bestscore_cur_season`   INT         NOT NULL    COMMENT '최고점수 현재 시즌', 
    `bestscore_prev_season`  INT         NOT NULL    COMMENT '최고점수 이전 시즌', 
    `new_record_dt`          DATETIME    NOT NULL    DEFUALT CURRENT_TIMESTAMP COMMENT '신 기록 일시', 
    `recent_play_dt`         DATETIME    NOT NULL    DEFUALT CURRENT_TIMESTAMP COMMENT '최근 플레이 일시', 
    `play_char_key`          INT         NOT NULL    COMMENT '플레이 캐릭터 키', 
    `create_dt`              DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, game_key)
);

-- Foreign Key 설정 SQL - user_minigame(uid) -> user(uid)
ALTER TABLE user_minigame
    ADD CONSTRAINT FK_user_minigame_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_char 테이블
유저의 캐릭터 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_char
CREATE TABLE user_char
(
    `uid`           INT         NOT NULL    COMMENT '유저아이디', 
    `char_key`      INT         NOT NULL    COMMENT '캐릭터 키', 
    `char_level`    INT         NOT NULL    DEFAULT 1 COMMENT '캐릭터 레벨', 
    `char_cnt`      INT         NOT NULL    DEFAULT 1 COMMENT '캐릭터 개수',
    `skin_key`      INT         NOT NULL    DEFAULT 0 COMMENT '스킨 키', 
    `create_dt`     DATETIME    NOT NULL    COMMENT '생성 일시', 
    `costume_json`  JSON        NOT NULL    COMMENT '코스튬 JSON', 
     PRIMARY KEY (uid, char_key)
);
-- Foreign Key 설정 SQL - user_char(uid) -> user(uid)
ALTER TABLE user_char
    ADD CONSTRAINT FK_user_char_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_char_random_skill 테이블
유저의 캐릭터의 랜덤 스킬 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_char_random_skill
CREATE TABLE user_char_random_skill
(
    `uid`        INT         NOT NULL    COMMENT '유저아이디', 
    `char_key`   INT         NOT NULL    COMMENT '캐릭터 키', 
    `index_num`  INT         NOT NULL    COMMENT '순서 숫자', 
    `skill_key`  INT         NOT NULL    COMMENT '스킬 키', 
    `create_dt`  DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, char_key, index_num)
);
-- Foreign Key 설정 SQL - user_char_random_skill(uid) -> user(uid)
ALTER TABLE user_char_random_skill
    ADD CONSTRAINT FK_user_char_random_skill_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_costume 테이블
유저의 코스튬 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_costume
CREATE TABLE user_costume
(
    `uid`            INT         NOT NULL    COMMENT '유저아이디', 
    `costume_key`    INT         NOT NULL    COMMENT '코스튬 키', 
    `costume_level`  INT         NOT NULL    DEFAULT 1 COMMENT '코스튬 레벨',
    `costume_cnt`    INT         NOT NULL    DEFAULT 1 COMMENT '코스튬 개수',
    `create_dt`      DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, costume_key)
);

-- Foreign Key 설정 SQL - user_costume(uid) -> user(uid)
ALTER TABLE user_costume
    ADD CONSTRAINT FK_user_costume_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_skin 테이블
유저의 스킨 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_skin
CREATE TABLE user_skin
(
    `uid`        INT         NOT NULL    COMMENT '유저아이디', 
    `skin_key`   INT         NOT NULL    COMMENT '스킨 키', 
    `create_dt`  DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, skin_key)
);
-- Foreign Key 설정 SQL - user_skin(uid) -> user(uid)
ALTER TABLE user_skin
    ADD CONSTRAINT FK_user_skin_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## user_food 테이블
유저의 푸드 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - user_food
CREATE TABLE user_food
(
    `uid`            INT         NOT NULL    COMMENT '유저아이디', 
    `food_key`       INT         NOT NULL    COMMENT '푸드 키', 
    `food_qty`       INT         NOT NULL    COMMENT '푸드 수', 
    `food_gear_qty`  INT         NOT NULL    COMMENT '푸드 기어 수', 
    `food_level`     INT         NOT NULL    COMMENT '푸드 레벨', 
    `create_dt`      DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, food_key)
);

-- Foreign Key 설정 SQL - user_food(uid) -> user(uid)
ALTER TABLE user_food
    ADD CONSTRAINT FK_user_food_uid_user_uid FOREIGN KEY (uid)
        REFERENCES user (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```


<br>
<br>

# master DB
## version 테이블
앱버전과 데이터 버전을 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - version
CREATE TABLE version
(
    `app_version`            INT         NOT NULL    COMMENT '앱 버전', 
    `master_data_version`    INT         NOT NULL    COMMENT '마스터 데이터 버전', 
);
```

## master_game 테이블
게임 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_game
CREATE TABLE master_game
(
    `game_key`   INT            NOT NULL    COMMENT '게임 키', 
    `game_name`  VARCHAR(50)    NOT NULL    COMMENT '게임 이름', 
     PRIMARY KEY (game_key)
);
```

## master_char 테이블
캐릭터 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_char
CREATE TABLE master_char
(
    `char_key`    INT            NOT NULL    COMMENT '캐릭터 키', 
    `char_name`   VARCHAR(50)    NOT NULL    COMMENT '캐릭터 이름', 
    `char_grade`  VARCHAR(20)    NOT NULL    COMMENT '캐릭터 등급', 
    `game_key`    INT            NOT NULL    COMMENT '게임 키', 
    `stat_run`    INT            NOT NULL    COMMENT '스탯 달리기', 
    `stat_power`  INT            NOT NULL    COMMENT '스탯 힘', 
    `stat_jump`   INT            NOT NULL    COMMENT '스탯 점프', 
    `create_dt`   DATETIME       NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (char_key)
);

-- Foreign Key 설정 SQL - master_char(game_key) -> master_game(game_key)
ALTER TABLE master_char
    ADD CONSTRAINT FK_master_char_game_key_master_game_game_key FOREIGN KEY (game_key)
        REFERENCES master_game (game_key) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## master_costume 테이블
코스튬 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_costume
CREATE TABLE master_costume
(
    `costume_key`   INT            NOT NULL    COMMENT '코스튬 키', 
    `costume_name`  VARCHAR(50)    NOT NULL    COMMENT '코스튬 이름', 
    `costume_type`  INT            NOT NULL    COMMENT '코스튬 종류', 
    `create_dt`     DATETIME       NOT NULL    COMMENT '생성 일시', 
    `set_key`       INT            NOT NULL    DEFAULT 0 COMMENT '세트 키', 
     PRIMARY KEY (costume_key)
);
```

## master_costume_set 테이블
코스튬 세트 정보를 가지고 있는 테이블
```sql
CREATE TABLE master_costume_set
(
    `set_key`             INT            NOT NULL    COMMENT '세트 키', 
    `set_name`            VARCHAR(50)    NOT NULL    COMMENT '세트 이름', 
    `char_key`            INT            NOT NULL    COMMENT '캐릭터 키', 
    `set_bonus_percent`   INT            NOT NULL    COMMENT '세트 보너스 퍼센트', 
    `char_bonus_percent`  INT            NOT NULL    COMMENT '캐릭터 보너스 퍼센트', 
    `create_dt`           DATETIME       NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (set_key)
);
-- Foreign Key 설정 SQL - master_costume_set(char_key) -> master_char(char_key)
ALTER TABLE master_costume_set
    ADD CONSTRAINT FK_master_costume_set_char_key_master_char_char_key FOREIGN KEY (char_key)
        REFERENCES master_char (char_key) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## master_skin 테이블
스킨 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_skin
CREATE TABLE master_skin
(
    `skin_key`            INT            NOT NULL    COMMENT '스킨 키', 
    `skin_name`           VARCHAR(50)    NOT NULL    COMMENT '스킨 이름', 
    `char_key`            INT            NOT NULL    COMMENT '캐릭터 키', 
    `skin_bonus_percent`  INT            NOT NULL    COMMENT '스킨 보너스 퍼센트', 
    `create_dt`           DATETIME       NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (skin_key)
);
-- Foreign Key 설정 SQL - master_skin(char_key) -> master_char(char_key)
ALTER TABLE master_skin
    ADD CONSTRAINT FK_master_skin_char_key_master_char_char_key FOREIGN KEY (char_key)
        REFERENCES master_char (char_key) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## master_skill 테이블
스킬 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_skill
CREATE TABLE master_skill
(
    `skill_key`         INT         NOT NULL    COMMENT '스킬 키', 
    `act_prob_percent`  INT         NOT NULL    COMMENT '발동 확률 퍼센트', 
    `create_dt`         DATETIME    NOT NULL    COMMENT '생성 일시', 
    `char_key`          INT         NOT NULL    DEFAULT 0 COMMENT '캐릭터 키', 
     PRIMARY KEY (skill_key)
);
-- Foreign Key 설정 SQL - master_skill(char_key) -> master_char(char_key)
ALTER TABLE master_skill
    ADD CONSTRAINT FK_master_skill_char_key_master_char_char_key FOREIGN KEY (char_key)
        REFERENCES master_char (char_key) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## master_food 테이블
푸드 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_food
CREATE TABLE master_food
(
    `food_key`   INT            NOT NULL    COMMENT '푸드 키', 
    `food_name`  VARCHAR(50)    NOT NULL    COMMENT '푸드 이름', 
    `create_dt`  DATETIME       NOT NULL    COMMENT '생성 일시', 
    `game_key`   INT            NOT NULL    COMMENT '게임 키', 
     PRIMARY KEY (food_key)
);
```

## master_gacha_reward 테이블
가챠 보상의 확률 정보와 뽑는 수량을 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_gacha_reward
CREATE TABLE master_gacha_reward
(
    `gacha_reward_key`        INT            NOT NULL    COMMENT '가챠 보상 키', 
    `gacha_reward_name`       VARCHAR(50)    NOT NULL    COMMENT '가챠 보상 이름', 
    `char_prob_percent`       INT            NOT NULL    COMMENT '캐릭터 확률 퍼센트', 
    `skin_prob_percent`       INT            NOT NULL    COMMENT '스킨 확률 퍼센트', 
    `costume_prob_percent`    INT            NOT NULL    COMMENT '코스튬 확률 퍼센트', 
    `food_prob_percent`       INT            NOT NULL    COMMENT '푸드 확률 퍼센트', 
    `food_gear_prob_percent`  INT            NOT NULL    COMMENT '푸드 기어 확률 퍼센트', 
    `gacha_count`             INT            NOT NULL    COMMENT '가챠 개수', 
    `create_dt`               DATETIME       NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (gacha_reward_key)
);
```

## master_gacha_reward_list 테이블
가챠 보상에 포함되는 보상들의 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_gacha_reward_list
CREATE TABLE master_gacha_reward_list
(
    `gacha_reward_key`  INT            NOT NULL    COMMENT '가챠 보상 키', 
    `reward_key`        INT            NOT NULL    COMMENT '보상 키', 
    `reward_type`       VARCHAR(20)    NOT NULL    COMMENT '보상 종류',
    `reward_qty`        INT             NOT NULL   DEFAULT 1 COMMENT '보상 수',
    `create_dt`         DATETIME       NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (gacha_reward_key, reward_key)
);
-- Foreign Key 설정 SQL - master_gacha_reward_list(gacha_reward_key) -> master_gacha_reward(gacha_reward_key)
ALTER TABLE master_gacha_reward_list
    ADD CONSTRAINT FK_master_gacha_reward_list_gacha_reward_key_master_gacha_reward FOREIGN KEY (gacha_reward_key)
        REFERENCES master_gacha_reward (gacha_reward_key) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## master_attendance_reward 테이블
출석 보상 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_attendance_reward
CREATE TABLE master_attendance_reward
(
    `day_seq`     INT         NOT NULL    COMMENT '날짜 번호', 
    `reward_key`  INT         NOT NULL    COMMENT '보상 키', 
    `reward_qty`  INT         NOT NULL    DEFAULT 0 COMMENT '보상 수',
    `reward_type` VARCHAR(20) NOT NULL    COMMENT '보상 종류',
    `create_dt`   DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (day_seq)
);
```

## master_item_level 테이블
아이템(캐릭터, 코스튬, 푸드) 레벨업을 위한 개수의 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_item_level
CREATE TABLE master_item_level
(
    `level`     INT            NOT NULL    COMMENT '레벨', 
    `item_cnt`  VARCHAR(50)    NOT NULL    DEFAULT 1 COMMENT '아이템 개수', 
     PRIMARY KEY (level)
);
```