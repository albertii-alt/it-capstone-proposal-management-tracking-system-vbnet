Imports MySql.Data.MySqlClient
Imports System.Collections.Generic
Imports System.Linq

Public Class SecurityLogDAL
    Public Shared Sub Insert(userId As Integer?, username As String, eventType As String, deviceName As String, ipAddress As String, details As String)
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("INSERT INTO tbl_security_log (user_id,username_snapshot,event_type,device_name,ip_address,details) VALUES (@uid,@username,@event,@device,@ip,@details)", conn)
                cmd.Parameters.AddWithValue("@uid", If(userId.HasValue, CType(userId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@username", username)
                cmd.Parameters.AddWithValue("@event", eventType)
                cmd.Parameters.AddWithValue("@device", deviceName)
                cmd.Parameters.AddWithValue("@ip", ipAddress)
                cmd.Parameters.AddWithValue("@details", details)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetEntries(eventType As String, userRole As String, searchText As String,
                                      fromDate As DateTime?, toDate As DateTime?, pageIndex As Integer,
                                      pageSize As Integer, ByRef totalCount As Integer) As List(Of SecurityLogEntry)
        Dim result As New List(Of SecurityLogEntry)
        Dim filters As String = " WHERE 1=1"
        If Not String.IsNullOrWhiteSpace(eventType) Then filters &= " AND sl.event_type=@event"
        If Not String.IsNullOrWhiteSpace(userRole) Then filters &= " AND u.role=@user_role"
        If Not String.IsNullOrWhiteSpace(searchText) Then filters &= " AND (sl.username_snapshot LIKE @search OR sl.details LIKE @search OR sl.ip_address LIKE @search)"
        If fromDate.HasValue Then filters &= " AND sl.created_at>=@from_date"
        If toDate.HasValue Then filters &= " AND sl.created_at<@to_date"
        Using conn = DbConnectionHelper.GetConnection()
            conn.Open()
            Dim fromClause As String = " FROM tbl_security_log sl LEFT JOIN tbl_users u ON sl.user_id=u.user_id"
            Using countCmd As New MySqlCommand("SELECT COUNT(*)" & fromClause & filters, conn)
                AddParameters(countCmd, eventType, userRole, searchText, fromDate, toDate)
                totalCount = Convert.ToInt32(countCmd.ExecuteScalar())
            End Using
            Using cmd As New MySqlCommand("SELECT sl.log_id,sl.username_snapshot,sl.event_type,sl.device_name,sl.ip_address,sl.details,sl.created_at" &
                                          fromClause & filters & " ORDER BY sl.created_at DESC,sl.log_id DESC LIMIT @offset,@size", conn)
                AddParameters(cmd, eventType, userRole, searchText, fromDate, toDate)
                cmd.Parameters.AddWithValue("@offset", Math.Max(0, pageIndex) * pageSize)
                cmd.Parameters.AddWithValue("@size", pageSize)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        result.Add(New SecurityLogEntry With {
                            .LogId = Convert.ToInt32(reader("log_id")),
                            .Username = Convert.ToString(reader("username_snapshot")),
                            .EventType = Convert.ToString(reader("event_type")),
                            .DeviceName = Convert.ToString(reader("device_name")),
                            .IpAddress = Convert.ToString(reader("ip_address")),
                            .Details = Convert.ToString(reader("details")),
                            .CreatedAt = Convert.ToDateTime(reader("created_at"))
                        })
                    End While
                End Using
            End Using
        End Using
        Return result
    End Function

    Public Shared Function DeleteEntries(logIds As List(Of Integer)) As Integer
        If logIds Is Nothing OrElse logIds.Count = 0 Then Return 0
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Using command As New MySqlCommand()
                    command.Connection = conn
                    command.Transaction = transaction
                    Dim placeholders As New List(Of String)
                    For index As Integer = 0 To logIds.Count - 1
                        Dim parameterName As String = "@log_id" & index.ToString()
                        placeholders.Add(parameterName)
                        command.Parameters.AddWithValue(parameterName, logIds(index))
                    Next
                    command.CommandText = "DELETE FROM tbl_security_log WHERE log_id IN (" &
                                          String.Join(", ", placeholders) & ")"
                    Dim deletedCount As Integer = command.ExecuteNonQuery()
                    transaction.Commit()
                    Return deletedCount
                End Using
            End Using
        End Using
    End Function

    Private Shared Sub AddParameters(command As MySqlCommand, eventType As String, userRole As String,
                                     searchText As String, fromDate As DateTime?, toDate As DateTime?)
        If Not String.IsNullOrWhiteSpace(eventType) Then command.Parameters.AddWithValue("@event", eventType)
        If Not String.IsNullOrWhiteSpace(userRole) Then command.Parameters.AddWithValue("@user_role", userRole)
        If Not String.IsNullOrWhiteSpace(searchText) Then command.Parameters.AddWithValue("@search", "%" & searchText.Trim() & "%")
        If fromDate.HasValue Then command.Parameters.AddWithValue("@from_date", fromDate.Value)
        If toDate.HasValue Then command.Parameters.AddWithValue("@to_date", toDate.Value)
    End Sub
End Class
