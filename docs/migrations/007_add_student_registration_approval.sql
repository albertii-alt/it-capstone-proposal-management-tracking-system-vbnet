ALTER TABLE tbl_users
    ADD COLUMN school_id VARCHAR(9) NULL AFTER email,
    ADD COLUMN account_status VARCHAR(20) NOT NULL DEFAULT 'Approved' AFTER is_active,
    ADD COLUMN registered_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP AFTER account_status,
    ADD COLUMN reviewed_at DATETIME NULL AFTER registered_at,
    ADD COLUMN reviewed_by INT NULL AFTER reviewed_at,
    ADD COLUMN review_remarks VARCHAR(500) NULL AFTER reviewed_by;

ALTER TABLE tbl_users
    ADD UNIQUE KEY uq_tbl_users_school_id (school_id),
    ADD CONSTRAINT fk_tbl_users_reviewed_by
        FOREIGN KEY (reviewed_by) REFERENCES tbl_users(user_id) ON DELETE SET NULL;
