## Master Database

Master Database는 각 게임 컨텐츠 세팅을 위한 게임 설정 데이터가 저장 됩니다.

### Money Template

```sql
CREATE TABLE money (
    money_code INT PRIMARY KEY,
    money_name VARCHAR(50) NOT NULL,
);
```

### Item Template

```sql
CREATE TABLE item (
    item_id INT PRIMARY KEY,
    item_name VARCHAR(50) NOT NULL,
    item_image VARCHAR(100) NOT NULL,
);
```

### Reward Template

```sql
CREATE TABLE reward (
    reward_uid BIGINT AUTO_INCREMENT PRIMARY KEY,
    reward_code INT NOT NULL,
    item_amount INT NOT NULL,
    item_id INT NOT NULL,
    INDEX idx_reward_code (reward_code)
);
```

### Attendance Template

```sql
CREATE TABLE attendance (
    attendance_code INT AUTO_INCREMENT PRIMARY KEY,
    attendance_name VARCHAR(50) NOT NULL,
    enabled_yn TINYINT(1) NOT NULL DEFAULT 0,
    repeatable_yn TINYINT(1) NOT NULL DEFAULT 0,
    INDEX idx_enabled_yn (enabled_yn)
);
```

### Attendance Detail Template

```sql
CREATE TABLE attendance_detail (
    attendance_detail_uid BIGINT AUTO_INCREMENT PRIMARY KEY,
    attendance_code INT NOT NULL,
    attendance_seq INT NOT NULL,
    reward_code INT NOT NULL,
    UNIQUE INDEX idx_attendance_code_seq (attendance_code, attendance_seq)
);
```
