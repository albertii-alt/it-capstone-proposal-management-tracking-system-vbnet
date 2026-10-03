Public Class AuthService

    Public Shared Function Login(username As String, password As String) As User
        Dim user As User = UserDAL.GetUserByUsername(username)

        If user Is Nothing Then
            Return Nothing
        End If

        If Not PasswordHasher.VerifyPassword(password, user.PasswordHash) Then
            Return Nothing
        End If

        Return user
    End Function

End Class