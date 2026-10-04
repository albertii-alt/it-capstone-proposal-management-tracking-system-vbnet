Public Class AdminActivityLogEntry
    Public Property ActivityId As Integer
    Public Property ActorId As Integer?
    Public Property ActorName As String
    Public Property ActionType As String
    Public Property EntityType As String
    Public Property EntityId As Integer?
    Public Property EntityName As String
    Public Property Description As String
    Public Property Metadata As String
    Public Property CreatedAt As DateTime
    Public Property IsStoredAuditEntry As Boolean
    Public Property ActivitySource As String
End Class
