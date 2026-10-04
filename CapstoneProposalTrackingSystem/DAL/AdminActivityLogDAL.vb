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
                "(actor_id, actor_name, action_type, entity_type, entity_id, entity_name, description, metadata) " &
                "VALUES (@actor_id, @actor_name, @action_type, @entity_type, @entity_id, @entity_name, @description, @metadata)"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@actor_id", If(entry.ActorId.HasValue, CType(entry.ActorId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@actor_name", entry.ActorName)
                cmd.Parameters.AddWithValue("@action_type", entry.ActionType)
                cmd.Parameters.AddWithValue("@entity_type", entry.EntityType)
                cmd.Parameters.AddWithValue("@entity_id", If(entry.EntityId.HasValue, CType(entry.EntityId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@entity_name", If(entry.EntityName, CType(DBNull.Value, Object)))
                cmd.Parameters.AddWithValue("@description", entry.Description)
                cmd.Parameters.AddWithValue("@metadata", If(entry.Metadata, CType(DBNull.Value, Object)))
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetEntries(actionType As String, entityType As String, searchText As String,
                                      fromDate As DateTime?, toDate As DateTime?,
                                      pageIndex As Integer, pageSize As Integer) As List(Of AdminActivityLogEntry)
        If pageIndex < 0 Then Throw New ArgumentOutOfRangeException(NameOf(pageIndex))
        If pageSize <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(pageSize))

        Dim entries As New List(Of AdminActivityLogEntry)()
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String =
                "SELECT activity_id, actor_id, actor_name, action_type, entity_type, entity_id, entity_name, " &
                "description, metadata, created_at FROM tbl_admin_activity_log " &
                "WHERE (@action_type = '' OR action_type = @action_type) " &
                "AND (@entity_type = '' OR entity_type = @entity_type) " &
                "AND (@search_text = '' OR actor_name LIKE @search_pattern OR entity_name LIKE @search_pattern OR description LIKE @search_pattern) " &
                "AND (@from_date IS NULL OR created_at >= @from_date) " &
                "AND (@to_date IS NULL OR created_at < @to_date) " &
                "ORDER BY created_at DESC, activity_id DESC LIMIT @page_size OFFSET @offset"

            Using cmd As New MySqlCommand(query, conn)
                Dim cleanSearch As String = If(searchText, "").Trim()
                cmd.Parameters.AddWithValue("@action_type", If(actionType, ""))
                cmd.Parameters.AddWithValue("@entity_type", If(entityType, ""))
                cmd.Parameters.AddWithValue("@search_text", cleanSearch)
                cmd.Parameters.AddWithValue("@search_pattern", "%" & cleanSearch & "%")
                cmd.Parameters.AddWithValue("@from_date", If(fromDate.HasValue, CType(fromDate.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@to_date", If(toDate.HasValue, CType(toDate.Value, Object), DBNull.Value))
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
                            .CreatedAt = reader.GetDateTime("created_at")
                        })
                    End While
                End Using
            End Using
        End Using

        Return entries
    End Function
End Class
