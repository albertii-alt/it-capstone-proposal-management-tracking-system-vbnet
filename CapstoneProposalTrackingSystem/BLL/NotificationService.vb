Imports System.Collections.Generic

Public Class NotificationService
    Public Shared Sub NotifyUser(userId As Integer, notificationType As String, title As String,
                                 message As String, entityId As Integer?, targetModule As String)
        NotificationDAL.InsertForUser(userId, notificationType, title, message, entityId, targetModule)
    End Sub

    Public Shared Sub NotifyAdministrators(notificationType As String, title As String,
                                           message As String, entityId As Integer?, targetModule As String)
        For Each user As User In UserService.GetAllUsers()
            If UserRoleHelper.IsAdministrator(user.Role) AndAlso user.IsActive Then
                NotifyUser(user.UserId, notificationType, title, message, entityId, targetModule)
            End If
        Next
    End Sub

    Public Shared Function GetForUser(userId As Integer) As List(Of NotificationEntry)
        Return NotificationDAL.GetForUser(userId)
    End Function

    Public Shared Function CountUnread(userId As Integer) As Integer
        Return NotificationDAL.CountUnread(userId)
    End Function

    Public Shared Sub MarkRead(notificationId As Integer, userId As Integer)
        NotificationDAL.MarkRead(notificationId, userId)
    End Sub

    Public Shared Sub Delete(notificationId As Integer, userId As Integer)
        NotificationDAL.Delete(notificationId, userId)
    End Sub

    Public Shared Sub DeleteAll(userId As Integer)
        NotificationDAL.DeleteAll(userId)
    End Sub
End Class
