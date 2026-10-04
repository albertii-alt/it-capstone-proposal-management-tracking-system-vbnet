CREATE TABLE IF NOT EXISTS tbl_login_activity (
  activity_id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  username_snapshot VARCHAR(50) NOT NULL,
  event_type ENUM('Login', 'Logout') NOT NULL,
  device_name VARCHAR(255) NOT NULL,
  ip_address VARCHAR(45) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_login_activity_user
    FOREIGN KEY (user_id) REFERENCES tbl_users(user_id) ON DELETE CASCADE,
  INDEX ix_login_activity_user_created (user_id, created_at)
) ENGINE=InnoDB;

SET @has_device_name = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'tbl_login_activity'
    AND column_name = 'device_name'
);
SET @add_device_name = IF(
  @has_device_name = 0,
  'ALTER TABLE tbl_login_activity ADD COLUMN device_name VARCHAR(255) NOT NULL DEFAULT ''Unavailable''',
  'SELECT 1'
);
PREPARE add_device_name_statement FROM @add_device_name;
EXECUTE add_device_name_statement;
DEALLOCATE PREPARE add_device_name_statement;

SET @has_ip_address = (
  SELECT COUNT(*)
  FROM information_schema.columns
  WHERE table_schema = DATABASE()
    AND table_name = 'tbl_login_activity'
    AND column_name = 'ip_address'
);
SET @add_ip_address = IF(
  @has_ip_address = 0,
  'ALTER TABLE tbl_login_activity ADD COLUMN ip_address VARCHAR(45) NOT NULL DEFAULT ''Unavailable''',
  'SELECT 1'
);
PREPARE add_ip_address_statement FROM @add_ip_address;
EXECUTE add_ip_address_statement;
DEALLOCATE PREPARE add_ip_address_statement;
