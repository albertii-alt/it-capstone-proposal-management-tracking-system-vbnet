Public Class User
    Public Property UserId As Integer
    Public Property Username As String
    Public Property PasswordHash As String
    Public Property Role As UserRole
    Public Property IsActive As Boolean
    Public Property FullName As String
    Public Property Email As String
    Public Property ProfilePhoto As Byte()
    Public Property SchoolId As String
    Public Property AccountStatus As String
    Public Property RegisteredAt As DateTime?
    Public Property ReviewedAt As DateTime?
    Public Property ReviewedBy As Integer?
    Public Property ReviewRemarks As String
End Class