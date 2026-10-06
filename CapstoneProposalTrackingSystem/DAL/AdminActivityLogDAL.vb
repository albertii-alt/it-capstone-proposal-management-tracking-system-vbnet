Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class AdminActivityLogDAL

    Public Shared Sub Insert(entry As AdminActivityLogEntry)
        If entry Is Nothing Then
            Throw New ArgumentNullException(NameOf(entry))
        End If

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Dim query As String =
                "INSERT INTO tbl_admin_activity_log " &
                "(actor_id, actor_name, action_type, entity_type, entity_id, entity_name, description, metadata, created_at) " &
                "VALUES (@actor_id, @actor_name, @action_type, @entity_type, @entity_id, @entity_name, @description, @metadata, " &
                "IFNULL(@created_at, CURRENT_TIMESTAMP))"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@actor_id", If(entry.ActorId.HasValue, CType(entry.ActorId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@actor_name", entry.ActorName)
                cmd.Parameters.AddWithValue("@action_type", entry.ActionType)
                cmd.Parameters.AddWithValue("@entity_type", entry.EntityType)
                cmd.Parameters.AddWithValue("@entity_id", If(entry.EntityId.HasValue, CType(entry.EntityId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@entity_name", If(entry.EntityName, CType(DBNull.Value, Object)))
                cmd.Parameters.AddWithValue("@description", entry.Description)
                cmd.Parameters.AddWithValue("@metadata", If(entry.Metadata, CType(DBNull.Value, Object)))
                cmd.Parameters.AddWithValue("@created_at", If(entry.CreatedAt > DateTime.MinValue, CType(entry.CreatedAt, Object), DBNull.Value))
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetEntries(actionType As String, entityType As String, searchText As String,
                                      fromDate As DateTime?, toDate As DateTime?,
                                      pageIndex As Integer, pageSize As Integer,
                                      ByRef totalCount As Integer) As List(Of AdminActivityLogEntry)
        If pageIndex < 0 Then Throw New ArgumentOutOfRangeException(NameOf(pageIndex))
        If pageSize <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(pageSize))

        Dim entries As New List(Of AdminActivityLogEntry)()
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim sourceQuery As String =
                "SELECT activity_id, actor_id, actor_name, action_type, entity_type, entity_id, entity_name, " &
                "is_stored_audit_entry, activity_source, " &
                "description, metadata, created_at FROM (" &
                "SELECT a.activity_id, a.actor_id, a.actor_name, a.action_type, a.entity_type, a.entity_id, " &
                "a.entity_name, a.description, a.metadata, a.created_at, TRUE AS is_stored_audit_entry, 'AdminActivity' AS activity_source " &
                "FROM tbl_admin_activity_log a " &
                "UNION ALL " &
                "SELECT l.log_id AS activity_id, l.admin_id AS actor_id, u.full_name AS actor_name, " &
                "CASE l.new_status WHEN 'Approved' THEN 'Approved' WHEN 'Rejected' THEN 'Rejected' " &
                "ELSE 'Requested Revision' END AS action_type, 'Proposal' AS entity_type, " &
                "l.proposal_id AS entity_id, p.title AS entity_name, " &
                "CONCAT('Changed the proposal ""', p.title, '"" to ', l.new_status, '.') AS description, " &
                "l.remarks AS metadata, l.changed_at AS created_at, FALSE AS is_stored_audit_entry, 'StatusLog' AS activity_source " &
                "FROM tbl_status_log l INNER JOIN tbl_users u ON u.user_id = l.admin_id " &
                "INNER JOIN tbl_proposals p ON p.proposal_id = l.proposal_id " &
                "WHERE NOT EXISTS (" &
                "SELECT 1 FROM tbl_admin_activity_log a2 " &
                "WHERE a2.entity_type = 'Proposal' AND a2.entity_id = l.proposal_id " &
                "AND a2.actor_id = l.admin_id AND a2.action_type = " &
                "CASE l.new_status WHEN 'Approved' THEN 'Approved' WHEN 'Rejected' THEN 'Rejected' " &
                "ELSE 'Requested Revision' END AND ABS(TIMESTAMPDIFF(SECOND, a2.created_at, l.changed_at)) <= 5) " &
                "UNION ALL " &
                "SELECT pa.activity_id, NULL AS actor_id, pa.actor_name, " &
                "CASE pa.action_type WHEN 'Submitted' THEN 'Created' ELSE 'Deleted' END AS action_type, " &
                "'Proposal' AS entity_type, NULL AS entity_id, pa.proposal_title AS entity_name, " &
                "CONCAT('""', pa.proposal_title, '"" was ', LOWER(pa.action_type), ' by ', pa.actor_name, '.') AS description, " &
                "NULL AS metadata, pa.created_at, FALSE AS is_stored_audit_entry, 'ProposalActivity' AS activity_source " &
                "FROM tbl_proposal_activity pa" &
                ") activity_entries "
            Dim filterQuery As String =
                "WHERE (@action_type = '' OR action_type = @action_type) " &
                "AND (@entity_type = '' OR entity_type = @entity_type) " &
                "AND (@search_text = '' OR actor_name LIKE @search_pattern OR entity_name LIKE @search_pattern OR description LIKE @search_pattern) " &
                "AND (@from_date IS NULL OR created_at >= @from_date) " &
                "AND (@to_date IS NULL OR created_at < @to_date) "
            Dim query As String =
                sourceQuery & filterQuery & " ORDER BY created_at DESC, activity_id DESC LIMIT @page_size OFFSET @offset"

            Using cmd As New MySqlCommand(query, conn)
                Dim cleanSearch As String = If(searchText, "").Trim()
                AddFilterParameters(cmd, actionType, entityType, cleanSearch, fromDate, toDate)
                cmd.Parameters.AddWithValue("@page_size", pageSize)
                cmd.Parameters.AddWithValue("@offset", pageIndex * pageSize)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        entries.Add(New AdminActivityLogEntry With {
                            .ActivityId = reader.GetInt32("activity_id"),
                            .ActorId = If(reader.IsDBNull(reader.GetOrdinal("actor_id")), Nothing, CType(reader.GetInt32("actor_id"), Integer?)),
                            .ActorName = reader.GetString("actor_name"),
                            .ActionType = reader.GetString("action_type"),
                            .EntityType = reader.GetString("entity_type"),
                            .EntityId = If(reader.IsDBNull(reader.GetOrdinal("entity_id")), Nothing, CType(reader.GetInt32("entity_id"), Integer?)),
                            .EntityName = If(reader.IsDBNull(reader.GetOrdinal("entity_name")), "", reader.GetString("entity_name")),
                            .Description = reader.GetString("description"),
                            .Metadata = If(reader.IsDBNull(reader.GetOrdinal("metadata")), "", reader.GetString("metadata")),
                            .CreatedAt = reader.GetDateTime("created_at"),
                            .IsStoredAuditEntry = reader.GetBoolean("is_stored_audit_entry"),
                            .ActivitySource = reader.GetString("activity_source")
                        })
                    End While
                End Using
            End Using

            Using countCommand As New MySqlCommand("SELECT COUNT(*) FROM (" & sourceQuery & filterQuery & ") filtered_entries", conn)
                Dim cleanSearch As String = If(searchText, "").Trim()
                AddFilterParameters(countCommand, actionType, entityType, cleanSearch, fromDate, toDate)
                totalCount = Convert.ToInt32(countCommand.ExecuteScalar())
            End Using
        End Using

        Return entries
    End Function

    Public Shared Function DeleteEntries(entries As List(Of AdminActivityLogEntry)) As Integer
        If entries Is Nothing Then Throw New ArgumentNullException(NameOf(entries))
        If entries.Count = 0 Then Return 0
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Dim deletedCount As Integer = 0
            Using transaction = conn.BeginTransaction()
                For Each entry As AdminActivityLogEntry In entries
                    If entry Is Nothing OrElse entry.ActivityId <= 0 Then
                        Throw New ArgumentException("Each activity record must have a valid source identifier.", NameOf(entries))
                    End If

                    Dim tableName As String
                    Dim idColumn As String
                    Select Case entry.ActivitySource
                        Case "AdminActivity"
                            tableName = "tbl_admin_activity_log"
                            idColumn = "activity_id"
                        Case "StatusLog"
                            tableName = "tbl_status_log"
                            idColumn = "log_id"
                        Case "ProposalActivity"
                            tableName = "tbl_proposal_activity"
                            idColumn = "activity_id"
                        Case Else
                            Throw New ArgumentException("Unsupported activity record source.", NameOf(entries))
                    End Select

                    Dim commandText As String = "DELETE FROM " & tableName & " WHERE " & idColumn & " = @record_id"
                    Using command As New MySqlCommand(commandText, conn, transaction)
                        command.Parameters.AddWithValue("@record_id", entry.ActivityId)
                        deletedCount += command.ExecuteNonQuery()
                    End Using
                Next
                transaction.Commit()
            End Using
            Return deletedCount
        End Using
    End Function

    Private Shared Sub AddFilterParameters(command As MySqlCommand, actionType As String, entityType As String,
                                            searchText As String, fromDate As DateTime?, toDate As DateTime?)
        command.Parameters.AddWithValue("@action_type", If(actionType, ""))
        command.Parameters.AddWithValue("@entity_type", If(entityType, ""))
        command.Parameters.AddWithValue("@search_text", searchText)
        command.Parameters.AddWithValue("@search_pattern", "%" & searchText & "%")
        command.Parameters.AddWithValue("@from_date", If(fromDate.HasValue, CType(fromDate.Value, Object), DBNull.Value))
        command.Parameters.AddWithValue("@to_date", If(toDate.HasValue, CType(toDate.Value, Object), DBNull.Value))
    End Sub
End Class
