CREATE DATABASE IF NOT EXISTS `logdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `logdb`;

-- Table structure for table `log_info`
DROP TABLE IF EXISTS `log_info`;
CREATE TABLE `log_info` (
 `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
  `timestamp` DATETIME NOT NULL,
  `loglevel` VARCHAR(50) NOT NULL,
  `category` VARCHAR(255) NOT NULL,
  `message` TEXT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Table structure for table `log_error`
DROP TABLE IF EXISTS `log_error`;
CREATE TABLE `log_error` (
  `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
  `timestamp` DATETIME NOT NULL,
  `loglevel` VARCHAR(50) NOT NULL,
  `category` VARCHAR(255) NOT NULL,
  `message` TEXT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE DATABASE  IF NOT EXISTS `gamedb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `gamedb`;
-- MySQL dump 10.13  Distrib 8.4.2, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: gamedb
-- ------------------------------------------------------
-- Server version	8.4.2

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

--
-- Table structure for table `user_info`
--

DROP TABLE IF EXISTS `user_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user_info` (
  `user_uid` bigint NOT NULL AUTO_INCREMENT,
  `hive_player_id` bigint NOT NULL,
  `user_name` varchar(20) DEFAULT NULL,
  `create_dt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `attendance_update_dt` timestamp NULL DEFAULT NULL,
  `recent_login_dt` timestamp NOT NULL,
  `play_total` int DEFAULT '0',
  `win_total` int DEFAULT '0',
  PRIMARY KEY (`user_uid`),
  UNIQUE KEY `hive_player_id` (`hive_player_id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user_info`
--

LOCK TABLES `user_info` WRITE;
/*!40000 ALTER TABLE `user_info` DISABLE KEYS */;
INSERT INTO `user_info` VALUES (1,1,'test1','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(2,2,'test2','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(3,3,'test3','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(4,4,'test4','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(5,5,'test5','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(6,6,'test6','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(7,7,'test7','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(8,8,'test8','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0),(9,9,'test9','2024-09-27 02:23:27',NULL,'2024-09-27 02:23:27',0,0);
/*!40000 ALTER TABLE `user_info` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-09-27 11:33:24
