# hive DB

## account_info 테이블
하이브 계정 정보를 가지고 있는 테이블

```sql
USE hive;

CREATE TABLE hive.`account_info`
(
    player_id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '플레이어 아이디',
    email VARCHAR(50) NOT NULL UNIQUE COMMENT '이메일',
    salt_value VARCHAR(100) NOT NULL COMMENT  '암호화 값',
    pw VARCHAR(100) NOT NULL COMMENT '해싱된 비밀번호',
    create_dt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '생성 일시',
    recent_login_dt DATETIME COMMENT '최근 로그인 일시',
) COMMENT '계정 정보 테이블';
```   
 