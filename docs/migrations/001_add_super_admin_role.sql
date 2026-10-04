USE it_capstone_proposal_system;

ALTER TABLE tbl_users
  MODIFY COLUMN role ENUM('Student', 'Admin', 'SuperAdmin') NOT NULL;

SET @super_admin_exists = (
  SELECT COUNT(*)
  FROM tbl_users
  WHERE role = 'SuperAdmin'
);

SET @first_admin_id = (
  SELECT user_id
  FROM (
    SELECT user_id
    FROM tbl_users
    WHERE role = 'Admin' AND @super_admin_exists = 0
    ORDER BY user_id
    LIMIT 1
  ) AS admin_candidates
);

UPDATE tbl_users
SET role = 'SuperAdmin'
WHERE user_id = @first_admin_id AND @super_admin_exists = 0;

SELECT user_id, username, role
FROM tbl_users
WHERE role = 'SuperAdmin';