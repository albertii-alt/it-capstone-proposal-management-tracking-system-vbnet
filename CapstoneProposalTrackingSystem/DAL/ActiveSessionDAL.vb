Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class ActiveSessionDAL
    Public Shared Sub Create(sessionId As String, user As User, deviceName As String, ipAddress As String)
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("INSERT INTO tbl_active_session (session_id,user_id,username_snapshot,device_name,ip_address) VALUES (@id,@uid,@username,@device,@ip)", conn)
                cmd.Parameters.AddWithValue("@id", sessionId)
                cmd.Parameters.AddWithValue("@uid", user.UserId)
                cmd.Parameters.AddWithValue("@username", user.Username)
                cmd.Parameters.AddWithValue("@device", deviceName)
                cmd.Parameters.AddWithValue("@ip", ipAddress)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub Touch(sessionId As String)
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("UPDATE tbl_active_session SET last_seen_at = CURRENT_TIMESTAMP WHERE session_id=@id AND revoked_at IS NULL", conn)
                cmd.Parameters.AddWithValue("@id", sessionId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function IsActive(sessionId As String) As Boolean
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_active_session WHERE session_id=@id AND revoked_at IS NULL", conn)
                cmd.Parameters.AddWithValue("@id", sessionId)
                Return Convert.ToInt32(cmd.ExecuteScalar()) = 1
            End Using
        End Using
    End Function

    Public Shared Function GetForUser(userId As Integer, currentSessionId As String) As List(Of ActiveSession)
        Dim result As New List(Of ActiveSession)
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("SELECT session_id,device_name,ip_address,started_at,last_seen_at FROM tbl_active_session WHERE user_id=@uid AND revoked_at IS NULL ORDER BY last_seen_at DESC", conn)
                cmd.Parameters.AddWithValue("@uid", userId)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New ActiveSession With {
                            .SessionId = Convert.ToString(reader("session_id")),
                            .DeviceName = Convert.ToString(reader("device_name")),
                            .IpAddress = Convert.ToString(reader("ip_address")),
                            .StartedAt = reader.GetDateTime("started_at"),
                            .LastSeenAt = reader.GetDateTime("last_seen_at"),
                            .IsCurrent = String.Equals(Convert.ToString(reader("session_id")), currentSessionId, StringComparison.OrdinalIgnoreCase)
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    Public Shared Sub Revoke(userId As Integer, sessionId As String)
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("UPDATE tbl_active_session SET revoked_at=CURRENT_TIMESTAMP WHERE user_id=@uid AND session_id=@id", conn)
                cmd.Parameters.AddWithValue("@uid", userId)
                cmd.Parameters.AddWithValue("@id", sessionId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub RevokeOthers(userId As Integer, currentSessionId As String)
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("UPDATE tbl_active_session SET revoked_at=CURRENT_TIMESTAMP WHERE user_id=@uid AND session_id<>@id AND revoked_at IS NULL", conn)
                cmd.Parameters.AddWithValue("@uid", userId)
                cmd.Parameters.AddWithValue("@id", currentSessionId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
