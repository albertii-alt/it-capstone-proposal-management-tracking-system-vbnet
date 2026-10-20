Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Sockets

Public Class SecurityLogService
    Public Shared Sub Record(userId As Integer?, username As String, eventType As String, details As String)
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
            ipAddress = "Unavailable"
        End Try
        SecurityLogDAL.Insert(userId, If(username, ""), eventType, deviceName, ipAddress, details)
    End Sub

    Public Shared Function GetEntries(eventType As String, userRole As String, searchText As String,
                                      fromDate As DateTime?, toDate As DateTime?, pageIndex As Integer,
                                      pageSize As Integer, ByRef totalCount As Integer) As List(Of SecurityLogEntry)
        Return SecurityLogDAL.GetEntries(eventType, userRole, searchText, fromDate, toDate,
                                         pageIndex, pageSize, totalCount)
    End Function

    Public Shared Function DeleteEntries(entries As List(Of SecurityLogEntry)) As Integer
        If entries Is Nothing Then Return 0
        Return SecurityLogDAL.DeleteEntries(entries.Select(Function(entry) entry.LogId).ToList())
    End Function
End Class
