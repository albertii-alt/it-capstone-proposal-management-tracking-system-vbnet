CREATE TABLE IF NOT EXISTS tbl_notifications (
  notification_id INT AUTO_INCREMENT PRIMARY KEY,
  recipient_id INT NOT NULL,
  notification_type VARCHAR(50) NOT NULL,
  title VARCHAR(150) NOT NULL,
  message VARCHAR(500) NOT NULL,
  entity_id INT NULL,
  target_module VARCHAR(80) NOT NULL,
  is_read BOOLEAN NOT NULL DEFAULT FALSE,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_notifications_recipient
    FOREIGN KEY (recipient_id) REFERENCES tbl_users(user_id) ON DELETE CASCADE,
  INDEX ix_notifications_recipient_read (recipient_id, is_read, created_at)
) ENGINE=InnoDB;
