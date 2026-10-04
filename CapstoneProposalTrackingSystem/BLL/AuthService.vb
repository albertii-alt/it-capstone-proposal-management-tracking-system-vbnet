Imports MySql.Data.MySqlClient

Public Class AuthService

    Public Shared Function Login(username As String, password As String) As User
        Dim user As User = UserDAL.GetUserByUsername(username)

        If user Is Nothing Then
            Return Nothing
        End If

        If Not PasswordHasher.VerifyPassword(password, user.PasswordHash) Then
            Return Nothing
        End If

        If Not user.IsActive Then
            Throw New ApplicationException("This account has been deactivated. Contact the administrator.")
        End If

        Try
            LoginActivityService.RecordLogin(user)
        Catch ex As MySqlException
            Throw New ApplicationException(
                "Login activity storage is not ready. Apply docs/migrations/004_add_login_activity.sql, then try again.", ex)
        End Try
        Session.SessionId = ActiveSessionService.Start(user)
        Return user
    End Function

End Class