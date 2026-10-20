Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class LoginActivityDAL
    Public Shared Sub Insert(user As User, eventType As String, deviceName As String, ipAddress As String)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand(
                "INSERT INTO tbl_login_activity (user_id, username_snapshot, event_type, device_name, ip_address) " &
                "VALUES (@user_id, @username, @event_type, @device_name, @ip_address)", conn)
                cmd.Parameters.AddWithValue("@user_id", user.UserId)
                cmd.Parameters.AddWithValue("@username", user.Username)
                cmd.Parameters.AddWithValue("@event_type", eventType)
                cmd.Parameters.AddWithValue("@device_name", deviceName)
                cmd.Parameters.AddWithValue("@ip_address", ipAddress)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetForUser(userId As Integer, eventType As String, fromDate As DateTime?, toDate As DateTime?, searchText As String,
                                      pageIndex As Integer, pageSize As Integer, ByRef totalCount As Integer) As List(Of LoginActivityEntry)
        Dim entries As New List(Of LoginActivityEntry)
        Dim filters As String = " WHERE user_id = @user_id"
        If Not String.IsNullOrWhiteSpace(eventType) Then filters &= " AND event_type = @event_type"
        If fromDate.HasValue Then filters &= " AND created_at >= @from_date"
        If toDate.HasValue Then filters &= " AND created_at < @to_date"
        If Not String.IsNullOrWhiteSpace(searchText) Then
            filters &= " AND (event_type LIKE @search_text OR device_name LIKE @search_text OR ip_address LIKE @search_text)"
        End If
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using countCmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_login_activity" & filters, conn)
                countCmd.Parameters.AddWithValue("@user_id", userId)
                AddFilterParameters(countCmd, eventType, fromDate, toDate, searchText)
                totalCount = Convert.ToInt32(countCmd.ExecuteScalar())
            End Using
            Using cmd As New MySqlCommand(
                "SELECT activity_id, event_type, created_at, device_name, ip_address FROM tbl_login_activity " &
                filters & " ORDER BY created_at DESC, activity_id DESC LIMIT @offset, @page_size", conn)
                cmd.Parameters.AddWithValue("@user_id", userId)
                AddFilterParameters(cmd, eventType, fromDate, toDate, searchText)
                cmd.Parameters.AddWithValue("@offset", Math.Max(0, pageIndex) * pageSize)
                cmd.Parameters.AddWithValue("@page_size", pageSize)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        entries.Add(New LoginActivityEntry With {
                            .ActivityId = reader.GetInt32("activity_id"),
                            .EventType = reader.GetString("event_type"),
                            .CreatedAt = reader.GetDateTime("created_at"),
                            .DeviceName = reader.GetString("device_name"),
                            .IpAddress = reader.GetString("ip_address")
                        })
                    End While
                End Using
            End Using
        End Using
        Return entries
    End Function

    Private Shared Sub AddFilterParameters(command As MySqlCommand, eventType As String, fromDate As DateTime?, toDate As DateTime?, searchText As String)
        If Not String.IsNullOrWhiteSpace(eventType) Then command.Parameters.AddWithValue("@event_type", eventType)
        If fromDate.HasValue Then command.Parameters.AddWithValue("@from_date", fromDate.Value)
        If toDate.HasValue Then command.Parameters.AddWithValue("@to_date", toDate.Value)
        If Not String.IsNullOrWhiteSpace(searchText) Then command.Parameters.AddWithValue("@search_text", "%" & searchText.Trim() & "%")
    End Sub

    Public Shared Function DeleteForUser(userId As Integer, activityIds As List(Of Integer)) As Integer
        If activityIds Is Nothing OrElse activityIds.Count = 0 Then Return 0
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Dim placeholders As New List(Of String)
                Using cmd As New MySqlCommand()
                    cmd.Connection = conn
                    cmd.Transaction = transaction
                    cmd.Parameters.AddWithValue("@user_id", userId)
                    For index As Integer = 0 To activityIds.Count - 1
                        Dim parameterName As String = "@activity_id" & index.ToString()
                        placeholders.Add(parameterName)
                        cmd.Parameters.AddWithValue(parameterName, activityIds(index))
                    Next
                    cmd.CommandText = "DELETE FROM tbl_login_activity WHERE user_id = @user_id AND activity_id IN (" &
                                      String.Join(", ", placeholders) & ")"
                    Dim deletedCount As Integer = cmd.ExecuteNonQuery()
                    transaction.Commit()
                    Return deletedCount
                End Using
            End Using
        End Using
    End Function
End Class
