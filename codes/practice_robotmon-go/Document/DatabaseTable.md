# MySQL8.0 -> 5.6.36
MySQL5.6.36 버전에서도 호환 가능하게 생성하는 방법    
"utf8mb4" -> "utf8"  
"utf8_0900_ai_ci" (또는 비슷한 것) -> "utf8_general_ci"  
  
# schema, database
CREATE SCHEMA `account` ;  
CREATE SCHEMA `game` ;  
CREATE SCHEMA `log`;

## account DB  
CREATE TABLE `users` (  
  `UID` bigint NOT NULL AUTO_INCREMENT, 
  `ID` varchar(45) NOT NULL, 
  `PW` varchar(200) NOT NULL,  
  `Salt` varchar(200) NOT NULL,  
  PRIMARY KEY (`UID`),  
  UNIQUE KEY `ID_UNIQUE` (`ID`)  
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

## game DB
CREATE TABLE `catch` (   
  `CatchID` bigint NOT NULL AUTO_INCREMENT,  
  `UserID` varchar(45) NOT NULL,  
  `MonsterID` bigint NOT NULL,  
  `CatchTime` date NOT NULL,  
  `CombatPoint` int NOT NULL,  
  PRIMARY KEY (`CatchID`)  
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `dailycheck` (  
  `ID` varchar(45) NOT NULL,  
  `RewardCount` int NOT NULL,  
  `RewardDate` date NOT NULL,  
  PRIMARY KEY (`ID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  
  
CREATE TABLE `dailyinfo` (  
  `DayCount` int NOT NULL,  
  `StarCount` int NOT NULL,  
  PRIMARY KEY (`DayCount`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `gamedata` (  
  `ID` varchar(45) NOT NULL,  
  `StarPoint` int NOT NULL,  
  `UserLevel` int NOT NULL,  
  `UserExp` int NOT NULL,  
  `UpgradeCandy` int NOT NULL,  
  PRIMARY KEY (`ID`),  
  UNIQUE KEY `ID_UNIQUE` (`ID`),  
  CONSTRAINT `constraint_StarPoint` CHECK ((`StarPoint` >= 0)),  
  CONSTRAINT `constraint_UpgradeCandy` CHECK ((`UpgradeCandy` >= 0))  
) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `mail` (  
  `postID` bigint NOT NULL AUTO_INCREMENT,  
  `ID` varchar(45) NOT NULL,  
  `StarCount` int NOT NULL,  
  `Date` date NOT NULL,  
  PRIMARY KEY (`postID`)  
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `monsterevolve` (  
  `MID` bigint NOT NULL,  
  `StarCount` int NOT NULL,  
  `EvolveMID` bigint NOT NULL,  
  PRIMARY KEY (`MID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `monsterinfo` (  
  `MID` bigint NOT NULL,  
  `MonsterName` varchar(100) NOT NULL,  
  `Type` varchar(100) NOT NULL,  
  `Level` int NOT NULL,  
  `HP` int NOT NULL,  
  `Att` int NOT NULL,  
  `Def` int NOT NULL,  
  `StarCount` int NOT NULL,  
  `UpgradeCount` int NOT NULL,  
  PRIMARY KEY (`MID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `monsterupgrade` (  
  `MID` bigint NOT NULL,  
  `UpgradeCost` int NOT NULL,  
  `StarCount` int NOT NULL,  
  `Exp` int NOT NULL,  
  PRIMARY KEY (`MID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  

CREATE TABLE `userlevelinfo` (  
  `Level` int NOT NULL,  
  `LevelUpExp` int NOT NULL,  
  PRIMARY KEY (`Level`)   
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  
  
## log DB  
CREATE TABLE `catch_log` (  
  `UID` bigint NOT NULL AUTO_INCREMENT,  
  `CatchID` bigint NOT NULL,  
  `UserID` varchar(45) NOT NULL,  
  `MonsterID` bigint NOT NULL,  
  `time` datetime NOT NULL,  
  `CombatPoint` int NOT NULL,  
  PRIMARY KEY (`UID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  
  
CREATE TABLE `evolve_log` (  
  `UID` bigint NOT NULL AUTO_INCREMENT,  
  `CatchID` bigint NOT NULL,  
  `BeforeMID` bigint NOT NULL,  
  `EvolveMID` bigint NOT NULL,  
  `CandyCount` int NOT NULL,  
  `time` datetime NOT NULL,  
  PRIMARY KEY (`UID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  
  
CREATE TABLE `login_log` (  
  `UID` bigint NOT NULL AUTO_INCREMENT,  
  `ID` varchar(45) NOT NULL,  
  `time` datetime NOT NULL,  
  PRIMARY KEY (`UID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  
  
CREATE TABLE `error_log` (  
  `UID` bigint NOT NULL AUTO_INCREMENT,  
  `errorStr` varchar(1024) NOT NULL,  
  `time` datetime NOT NULL,  
  PRIMARY KEY (`UID`)  
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;  
