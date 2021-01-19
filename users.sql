-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.4.14-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             10.1.0.5464
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for lorisangel
CREATE DATABASE IF NOT EXISTS `lorisangel` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `lorisangel`;

-- Dumping structure for table lorisangel.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` bigint(20) NOT NULL COMMENT 'Discord ID',
  `name` varchar(50) DEFAULT NULL COMMENT 'Discord username',
  `createdon` datetime DEFAULT NULL COMMENT 'Discord Account Creation Time',
  `joinedon` datetime DEFAULT NULL COMMENT 'Lori''s Angel Profile Creating Time',
  `lastseen` datetime DEFAULT NULL COMMENT 'Last Seen Active Time',
  `status` varchar(50) DEFAULT NULL COMMENT 'Discord Status',
  `badges` longtext DEFAULT NULL COMMENT 'List of all Lori''s Angel badges',
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='A table of all the Lori''s Angels users';

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
