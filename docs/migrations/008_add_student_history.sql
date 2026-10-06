CREATE TABLE IF NOT EXISTS tbl_student_history (
  history_id    INT AUTO_INCREMENT PRIMARY KEY,
  student_id    INT NOT NULL,
  proposal_id   INT NULL,
  proposal_title VARCHAR(255) NOT NULL,
  action_type   VARCHAR(100) NOT NULL,
  remarks       TEXT NOT NULL,
  recorded_at   DATETIME NOT NULL,
  CONSTRAINT fk_student_history_student
    FOREIGN KEY (student_id) REFERENCES tbl_users(user_id) ON DELETE CASCADE,
  CONSTRAINT fk_student_history_proposal
    FOREIGN KEY (proposal_id) REFERENCES tbl_proposals(proposal_id) ON DELETE SET NULL,
  INDEX ix_student_history_student_recorded (student_id, recorded_at)
) ENGINE=InnoDB;

INSERT INTO tbl_student_history
  (student_id, proposal_id, proposal_title, action_type, remarks, recorded_at)
SELECT p.submitted_by, p.proposal_id, p.title, 'Submitted',
       'Proposal submitted for review.', p.submitted_at
FROM tbl_proposals p
INNER JOIN tbl_users u ON u.user_id = p.submitted_by
WHERE u.role = 'Student'
  AND NOT EXISTS (
    SELECT 1 FROM tbl_student_history h
    WHERE h.student_id = p.submitted_by
      AND h.proposal_id = p.proposal_id
      AND h.action_type = 'Submitted'
  );

INSERT INTO tbl_student_history
  (student_id, proposal_id, proposal_title, action_type, remarks, recorded_at)
SELECT p.submitted_by, p.proposal_id, p.title,
       CASE l.new_status
         WHEN 'ForRevision' THEN 'Requested Revision'
         ELSE CONCAT('Status changed to ', l.new_status)
       END,
       COALESCE(NULLIF(l.remarks, ''), 'No remarks provided.'),
       l.changed_at
FROM tbl_status_log l
INNER JOIN tbl_proposals p ON p.proposal_id = l.proposal_id
INNER JOIN tbl_users u ON u.user_id = p.submitted_by
WHERE u.role = 'Student'
  AND NOT EXISTS (
    SELECT 1 FROM tbl_student_history h
    WHERE h.student_id = p.submitted_by
      AND h.proposal_id = l.proposal_id
      AND h.action_type = CASE l.new_status
        WHEN 'ForRevision' THEN 'Requested Revision'
        ELSE CONCAT('Status changed to ', l.new_status)
      END
      AND h.recorded_at = l.changed_at
  );

INSERT INTO tbl_admin_activity_log
  (actor_id, actor_name, action_type, entity_type, entity_id, entity_name, description, metadata, created_at)
SELECT h.student_id, u.full_name, h.action_type, 'Student History', h.proposal_id,
       h.proposal_title,
       CONCAT('Student history: ', h.action_type, ' for "', h.proposal_title, '".'),
       NULLIF(h.remarks, ''), h.recorded_at
FROM tbl_student_history h
INNER JOIN tbl_users u ON u.user_id = h.student_id
WHERE NOT EXISTS (
  SELECT 1 FROM tbl_admin_activity_log a
  WHERE a.actor_id = h.student_id
    AND a.entity_type = 'Student History'
    AND a.entity_id <=> h.proposal_id
    AND a.action_type = h.action_type
    AND a.created_at = h.recorded_at
);
