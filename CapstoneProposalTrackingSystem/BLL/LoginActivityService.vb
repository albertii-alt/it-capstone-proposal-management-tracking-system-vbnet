Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Sockets

Public Class LoginActivityService
    Public Shared Sub RecordLogin(user As User)
        If user Is Nothing Then Throw New ArgumentNullException(NameOf(user))
        Record(user, "Login")
    End Sub

    Public Shared Sub RecordLogout(user As User)
        If user Is Nothing Then Return
        Record(user, "Logout")
    End Sub

    Private Shared Sub Record(user As User, eventType As String)
        Dim deviceName As String = Environment.MachineName
        Dim ipAddress As String = "Unavailable"
        Try
            For Each address As IPAddress In Dns.GetHostEntry(Dns.GetHostName()).AddressList
                If address.AddressFamily = AddressFamily.InterNetwork AndAlso Not System.Net.IPAddress.IsLoopback(address) Then
                    ipAddress = address.ToString()
                    Exit For
                End If
            Next
        Catch ex As SocketException
            ipAddress = "Unavailable (" & ex.SocketErrorCode.ToString() & ")"
        End Try
        LoginActivityDAL.Insert(user, eventType, deviceName, ipAddress)
    End Sub

    Public Shared Function GetForUser(userId As Integer, eventType As String, fromDate As DateTime?, toDate As DateTime?,
                                      pageIndex As Integer, pageSize As Integer, ByRef totalCount As Integer) As List(Of LoginActivityEntry)
        Return LoginActivityDAL.GetForUser(userId, eventType, fromDate, toDate, pageIndex, pageSize, totalCount)
    End Function

    Public Shared Function DeleteForUser(userId As Integer, entries As List(Of LoginActivityEntry)) As Integer
        If entries Is Nothing Then Return 0
        Return LoginActivityDAL.DeleteForUser(userId, entries.Select(Function(entry) entry.ActivityId).ToList())
    End Function
End Class
