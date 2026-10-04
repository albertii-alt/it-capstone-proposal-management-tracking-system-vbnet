Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Sockets

Public Class ActiveSessionService
    Public Shared Function Start(user As User) As String
        Dim sessionId As String = Guid.NewGuid().ToString()
        ActiveSessionDAL.Create(sessionId, user, Environment.MachineName, GetIpAddress())
        SecurityLogService.Record(user.UserId, user.Username, "Session Created",
                                  "A new session was created on device " & Environment.MachineName & ".")
        Return sessionId
    End Function

    Public Shared Sub Touch(sessionId As String)
        If Not String.IsNullOrWhiteSpace(sessionId) Then ActiveSessionDAL.Touch(sessionId)
    End Sub

    Public Shared Function IsActive(sessionId As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(sessionId) AndAlso ActiveSessionDAL.IsActive(sessionId)
    End Function

    Public Shared Function GetForUser(userId As Integer, currentSessionId As String) As List(Of ActiveSession)
        Return ActiveSessionDAL.GetForUser(userId, currentSessionId)
    End Function

    Public Shared Sub Revoke(userId As Integer, sessionId As String)
        ActiveSessionDAL.Revoke(userId, sessionId)
        Dim user As User = UserDAL.GetUserById(userId)
        If user IsNot Nothing Then
            SecurityLogService.Record(user.UserId, user.Username, "Session Revoked",
                                      "A session was remotely revoked by the account owner.")
        End If
    End Sub

    Public Shared Sub RevokeOthers(userId As Integer, currentSessionId As String)
        ActiveSessionDAL.RevokeOthers(userId, currentSessionId)
        Dim user As User = UserDAL.GetUserById(userId)
        If user IsNot Nothing Then
            SecurityLogService.Record(user.UserId, user.Username, "Session Revoked",
                                      "All other active sessions were remotely revoked by the account owner.")
        End If
    End Sub

    Private Shared Function GetIpAddress() As String
        Try
            For Each address As IPAddress In Dns.GetHostEntry(Dns.GetHostName()).AddressList
                If address.AddressFamily = AddressFamily.InterNetwork AndAlso Not System.Net.IPAddress.IsLoopback(address) Then Return address.ToString()
            Next
        Catch ex As SocketException
        End Try
        Return "Unavailable"
    End Function
End Class
