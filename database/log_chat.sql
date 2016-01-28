-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versione server:              10.1.9-MariaDB - mariadb.org binary distribution
-- S.O. server:                  Win32
-- HeidiSQL Versione:            9.3.0.4984
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dump della struttura di tabella ellapp.log_chat
CREATE TABLE IF NOT EXISTS `log_chat` (
  `idLog` int(11) NOT NULL AUTO_INCREMENT,
  `content` longtext,
  `from` int(11) DEFAULT NULL COMMENT 'ID',
  `to_type` varchar(50) DEFAULT NULL COMMENT 'Global Chat, Group, Private',
  `to` int(11) DEFAULT NULL COMMENT 'The ID',
  PRIMARY KEY (`idLog`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- L’esportazione dei dati non era selezionata.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
