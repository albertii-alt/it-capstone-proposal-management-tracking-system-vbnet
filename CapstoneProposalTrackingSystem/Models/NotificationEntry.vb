Public Class NotificationEntry
    Public Property NotificationId As Integer
    Public Property RecipientId As Integer
    Public Property NotificationType As String
    Public Property Title As String
    Public Property Message As String
    Public Property EntityId As Integer?
    Public Property TargetModule As String
    Public Property IsRead As Boolean
    Public Property CreatedAt As DateTime
End Class
