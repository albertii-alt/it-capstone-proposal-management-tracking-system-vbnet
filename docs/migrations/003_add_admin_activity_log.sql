CREATE TABLE IF NOT EXISTS tbl_admin_activity_log (
  activity_id  INT AUTO_INCREMENT PRIMARY KEY,
  actor_id     INT NULL,
  actor_name   VARCHAR(100) NOT NULL,
  action_type  VARCHAR(50) NOT NULL,
  entity_type  VARCHAR(50) NOT NULL,
  entity_id    INT NULL,
  entity_name  VARCHAR(255) NULL,
  description  VARCHAR(500) NOT NULL,
  metadata     TEXT NULL,
  created_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_admin_activity_actor
    FOREIGN KEY (actor_id) REFERENCES tbl_users(user_id) ON DELETE SET NULL,
  INDEX ix_admin_activity_created_at (created_at),
  INDEX ix_admin_activity_action_type (action_type),
  INDEX ix_admin_activity_entity_type (entity_type)
) ENGINE=InnoDB;
