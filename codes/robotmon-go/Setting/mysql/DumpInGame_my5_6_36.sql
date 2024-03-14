CREATE DATABASE /*!32312 IF NOT EXISTS*/ `account` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `account`;

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `users` (
	`UID` bigint NOT NULL AUTO_INCREMENT,
	`ID` varchar(45) NOT NULL,
	`PW` varchar(200) NOT NULL,
	`Salt` varchar(200) NOT NULL,
	PRIMARY KEY(`UID`),
		UNIQUE KEY `ID_UNIQUE` (`ID`)
			) ENGINE = InnoDB AUTO_INCREMENT = 13 DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;


CREATE DATABASE /*!32312 IF NOT EXISTS*/ `game` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `game`;

DROP TABLE IF EXISTS `catch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `catch` (
	`CatchID` bigint NOT NULL AUTO_INCREMENT,
	`UserID` varchar(45) NOT NULL,
	`MonsterID` bigint NOT NULL,
	`CatchTime` date NOT NULL,
	`CombatPoint` int NOT NULL,
	PRIMARY KEY(`CatchID`)
	) ENGINE = InnoDB AUTO_INCREMENT = 14 DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `catch` WRITE;
/*!40000 ALTER TABLE `catch` DISABLE KEYS */;
/*!40000 ALTER TABLE `catch` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `dailycheck`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `dailycheck` (
	`ID` varchar(45) NOT NULL,
	`RewardCount` int NOT NULL,
	`RewardDate` date NOT NULL,
	PRIMARY KEY(`ID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `dailycheck` WRITE;
/*!40000 ALTER TABLE `dailycheck` DISABLE KEYS */;
/*!40000 ALTER TABLE `dailycheck` ENABLE KEYS */;
UNLOCK TABLES;


DROP TABLE IF EXISTS `dailyinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `dailyinfo` (
	`DayCount` int NOT NULL,
	`StarCount` int NOT NULL,
	PRIMARY KEY(`DayCount`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `dailyinfo` WRITE;
/*!40000 ALTER TABLE `dailyinfo` DISABLE KEYS */;
INSERT INTO `dailyinfo` VALUES(1, 2), (2, 4), (3, 8), (4, 16), (5, 32), (6, 64), (7, 128);
/*!40000 ALTER TABLE `dailyinfo` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `gamedata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `gamedata` (
	`ID` varchar(45) NOT NULL,
	`StarPoint` int NOT NULL,
	`UserLevel` int NOT NULL,
	`UserExp` int NOT NULL,
	`UpgradeCandy` int NOT NULL,
	PRIMARY KEY(`ID`),
		UNIQUE KEY `ID_UNIQUE` (`ID`),
			CONSTRAINT `constraint_StarPoint` CHECK((`StarPoint` >= 0)),
			CONSTRAINT `constraint_UpgradeCandy` CHECK((`UpgradeCandy` >= 0))
			) ENGINE = MyISAM DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `gamedata` WRITE;
/*!40000 ALTER TABLE `gamedata` DISABLE KEYS */;
/*!40000 ALTER TABLE `gamedata` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `mail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `mail` (
	`postID` bigint NOT NULL AUTO_INCREMENT,
	`ID` varchar(45) NOT NULL,
	`StarCount` int NOT NULL,
	`Date` date NOT NULL,
	PRIMARY KEY(`postID`)
	) ENGINE = InnoDB AUTO_INCREMENT = 9 DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `mail` WRITE;
/*!40000 ALTER TABLE `mail` DISABLE KEYS */;
/*!40000 ALTER TABLE `mail` ENABLE KEYS */;
UNLOCK TABLES;


DROP TABLE IF EXISTS `monsterevolve`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `monsterevolve` (
	`MID` bigint NOT NULL,
	`StarCount` int NOT NULL,
	`EvolveMID` bigint NOT NULL,
	PRIMARY KEY(`MID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `monsterevolve` WRITE;
/*!40000 ALTER TABLE `monsterevolve` DISABLE KEYS */;
INSERT INTO `monsterevolve` VALUES(1, 10, 2), (3, 15, 4), (5, 20, 6);
/*!40000 ALTER TABLE `monsterevolve` ENABLE KEYS */;
UNLOCK TABLES;


DROP TABLE IF EXISTS `monsterinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
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
	PRIMARY KEY(`MID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


LOCK TABLES `monsterinfo` WRITE;
/*!40000 ALTER TABLE `monsterinfo` DISABLE KEYS */;
INSERT INTO `monsterinfo` VALUES(1, '기동전사 감담', '격투', 3, 100, 20, 20, 1, 3), (2, '기동전사 감담 X', '격투', 11, 200, 50, 40, 3, 5), (3, '마짐가 A', '미사일', 3, 150, 15, 30, 1, 3), (4, '마짐가 AA', '미사일', 11, 300, 40, 50, 2, 5), (5, '담바이', '이세계', 5, 200, 30, 30, 4, 3), (6, '초 담바이', '이세계', 12, 400, 60, 50, 8, 5);
/*!40000 ALTER TABLE `monsterinfo` ENABLE KEYS */;
UNLOCK TABLES;


DROP TABLE IF EXISTS `monsterupgrade`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `monsterupgrade` (
	`MID` bigint NOT NULL,
	`UpgradeCost` int NOT NULL,
	`StarCount` int NOT NULL,
	`Exp` int NOT NULL,
	PRIMARY KEY(`MID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


LOCK TABLES `monsterupgrade` WRITE;
/*!40000 ALTER TABLE `monsterupgrade` DISABLE KEYS */;
INSERT INTO `monsterupgrade` VALUES(1, 10, 100, 10), (2, 15, 150, 15), (3, 20, 200, 20), (4, 25, 250, 25), (5, 20, 200, 20), (6, 30, 300, 30);
/*!40000 ALTER TABLE `monsterupgrade` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `userlevelinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `userlevelinfo` (
	`Level` int NOT NULL,
	`LevelUpExp` int NOT NULL,
	PRIMARY KEY(`Level`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;


LOCK TABLES `userlevelinfo` WRITE;
/*!40000 ALTER TABLE `userlevelinfo` DISABLE KEYS */;
INSERT INTO `userlevelinfo` VALUES(1, 1000), (2, 2000), (3, 3000), (4, 4000), (5, 5000), (6, 6000), (7, 7000), (8, 8000), (9, 9000), (10, 10000);
/*!40000 ALTER TABLE `userlevelinfo` ENABLE KEYS */;
UNLOCK TABLES;
