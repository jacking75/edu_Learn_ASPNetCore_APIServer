# account DB
  
## user 테이블
게임에서 생성 된 계정 정보들을 가지고 있는 테이블    
  
```sql
USE account_db;

CREATE TABLE account_db.`user`
(
    uid BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '계정번호',
    email VARCHAR(50) NOT NULL UNIQUE COMMENT '이메일',
    salt_value VARCHAR(100) NOT NULL COMMENT  '암호화 값',
    password VARCHAR(100) NOT NULL COMMENT '해싱된 비밀번호',
    createdAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '생성 날짜'
) COMMENT '계정 정보 테이블';
```   
   
<br>  
<br>  
   
   
# game DB
  
## player 테이블
월드에 존재하는 캐릭터에 대한 정보     
```sql
USE game_db;
CREATE TABLE game_db.`player`
(
    player_uid BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '고유 번호',
    user_uid BIGINT NOT NULL COMMENT '계정 번호',
    exp INT NOT NULL COMMENT  '경험치',
    level INT NOT NULL COMMENT  '레벨',
    hp INT NOT NULL COMMENT '현재 체력',
    mp INT NOT NULL COMMENT '현재 마력',
    money BIGINT NOT NULL COMMENT '소지한 머니',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '생성 날짜'
) COMMENT '월드의 캐릭터 정보';
```
    
   
## player_item 테이블
캐릭터가 보유한 아이템에 대한 정보  
  
```sql
USE game_db;

CREATE TABLE game_db.`player_item`
(
    player_item_id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '고유 번호',
    player_id BIGINT NOT NULL COMMENT '플레이어 고유 번호',
    item_code INT NOT NULL COMMENT '마스터 아이템 데이터의 Code 값',  
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '생성 날짜'
) COMMENT '캐릭터 보유 아이템'; 
```
    

 