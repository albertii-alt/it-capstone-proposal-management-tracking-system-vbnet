CREATE TABLE IF NOT EXISTS tbl_active_session (
  session_id CHAR(36) PRIMARY KEY,
  user_id INT NOT NULL,
  username_snapshot VARCHAR(50) NOT NULL,
  device_name VARCHAR(255) NOT NULL,
  ip_address VARCHAR(45) NOT NULL,
  started_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  last_seen_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  revoked_at DATETIME NULL,
  CONSTRAINT fk_active_session_user
    FOREIGN KEY (user_id) REFERENCES tbl_users(user_id) ON DELETE CASCADE,
  INDEX ix_active_session_user (user_id, revoked_at)
) ENGINE=InnoDB;
