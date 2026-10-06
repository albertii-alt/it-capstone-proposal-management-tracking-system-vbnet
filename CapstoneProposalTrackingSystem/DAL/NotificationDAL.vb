Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

Public Class NotificationDAL
    Public Shared Sub InsertForUser(recipientId As Integer, notificationType As String, title As String,
                                     message As String, entityId As Integer?, targetModule As String)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Const query As String = "INSERT INTO tbl_notifications " &
                "(recipient_id, notification_type, title, message, entity_id, target_module) " &
                "VALUES (@recipient_id, @notification_type, @title, @message, @entity_id, @target_module)"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@recipient_id", recipientId)
                cmd.Parameters.AddWithValue("@notification_type", notificationType)
                cmd.Parameters.AddWithValue("@title", title)
                cmd.Parameters.AddWithValue("@message", message)
                cmd.Parameters.AddWithValue("@entity_id", If(entityId.HasValue, CType(entityId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@target_module", targetModule)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetForUser(recipientId As Integer) As List(Of NotificationEntry)
        Dim result As New List(Of NotificationEntry)()
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Const query As String = "SELECT notification_id, recipient_id, notification_type, title, message, entity_id, target_module, is_read, created_at " &
                "FROM tbl_notifications WHERE recipient_id = @recipient_id ORDER BY is_read ASC, created_at DESC"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@recipient_id", recipientId)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New NotificationEntry With {
                            .NotificationId = reader.GetInt32("notification_id"),
                            .RecipientId = reader.GetInt32("recipient_id"),
                            .NotificationType = reader.GetString("notification_type"),
                            .Title = reader.GetString("title"),
                            .Message = reader.GetString("message"),
                            .EntityId = If(reader.IsDBNull(reader.GetOrdinal("entity_id")), Nothing, CType(reader.GetInt32("entity_id"), Integer?)),
                            .TargetModule = reader.GetString("target_module"),
                            .IsRead = reader.GetBoolean("is_read"),
                            .CreatedAt = reader.GetDateTime("created_at")
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    Public Shared Sub MarkRead(notificationId As Integer, recipientId As Integer)
        ExecuteForUser("UPDATE tbl_notifications SET is_read = TRUE WHERE notification_id = @notification_id AND recipient_id = @recipient_id", notificationId, recipientId)
    End Sub

    Public Shared Sub Delete(notificationId As Integer, recipientId As Integer)
        ExecuteForUser("DELETE FROM tbl_notifications WHERE notification_id = @notification_id AND recipient_id = @recipient_id", notificationId, recipientId)
    End Sub

    Public Shared Sub DeleteAll(recipientId As Integer)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("DELETE FROM tbl_notifications WHERE recipient_id = @recipient_id", conn)
                cmd.Parameters.AddWithValue("@recipient_id", recipientId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function CountUnread(recipientId As Integer) As Integer
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_notifications WHERE recipient_id = @recipient_id AND is_read = FALSE", conn)
                cmd.Parameters.AddWithValue("@recipient_id", recipientId)
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Using
    End Function

    Private Shared Sub ExecuteForUser(query As String, notificationId As Integer, recipientId As Integer)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@notification_id", notificationId)
                cmd.Parameters.AddWithValue("@recipient_id", recipientId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
