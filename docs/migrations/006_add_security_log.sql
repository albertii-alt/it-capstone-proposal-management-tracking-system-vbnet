CREATE TABLE IF NOT EXISTS tbl_security_log (
  log_id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NULL,
  username_snapshot VARCHAR(50) NOT NULL,
  event_type VARCHAR(50) NOT NULL,
  device_name VARCHAR(255) NOT NULL,
  ip_address VARCHAR(45) NOT NULL,
  details VARCHAR(500) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_security_log_user
    FOREIGN KEY (user_id) REFERENCES tbl_users(user_id) ON DELETE SET NULL,
  INDEX ix_security_log_created (created_at),
  INDEX ix_security_log_event (event_type)
) ENGINE=InnoDB;
