CREATE DATABASE  IF NOT EXISTS `log` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `log`;

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

DROP TABLE IF EXISTS `catch_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `catch_log` (
	`CatchID` bigint NOT NULL AUTO_INCREMENT,
	`UserID` varchar(45) NOT NULL,
	`MonsterID` bigint NOT NULL,
	`CatchTime` datetime NOT NULL,
	`CombatPoint` int NOT NULL,
	PRIMARY KEY(`CatchID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `catch_log` WRITE;
/*!40000 ALTER TABLE `catch_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `catch_log` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `evolve_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `evolve_log` (
	`UID` bigint NOT NULL,
	`MID` bigint NOT NULL,
	`EvolveMID` bigint NOT NULL,
	`StarCount` int NOT NULL,
	`time` datetime NOT NULL,
	PRIMARY KEY(`UID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `evolve_log` WRITE;
/*!40000 ALTER TABLE `evolve_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `evolve_log` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `login_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `login_log` (
	`UID` bigint NOT NULL AUTO_INCREMENT,
	`ID` varchar(45) NOT NULL,
	`time` datetime NOT NULL,
	PRIMARY KEY(`UID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `login_log` WRITE;
/*!40000 ALTER TABLE `login_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `login_log` ENABLE KEYS */;
UNLOCK TABLES;

DROP TABLE IF EXISTS `error_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8 */;
CREATE TABLE `error_log` (
	`UID` bigint NOT NULL AUTO_INCREMENT,
	`errorStr` varchar(1024) NOT NULL,
	`time` datetime NOT NULL,
	PRIMARY KEY(`UID`)
	) ENGINE = InnoDB DEFAULT CHARSET = utf8 COLLATE = utf8_general_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `error_log` WRITE;
/*!40000 ALTER TABLE `error_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `error_log` ENABLE KEYS */;
UNLOCK TABLES;