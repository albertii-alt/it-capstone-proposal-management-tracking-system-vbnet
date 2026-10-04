Imports MySql.Data.MySqlClient

Public Class AuthService

    Public Shared Function Login(username As String, password As String) As User
        Dim user As User = UserDAL.GetUserByUsername(username)

        If user Is Nothing Then
            SecurityLogService.Record(Nothing, username, "Failed Login", "No matching account was found.")
            Return Nothing
        End If

        If Not PasswordHasher.VerifyPassword(password, user.PasswordHash) Then
            SecurityLogService.Record(user.UserId, user.Username, "Failed Login", "The supplied password was incorrect.")
            Return Nothing
        End If

        If Not user.IsActive Then
            SecurityLogService.Record(user.UserId, user.Username, "Blocked Login", "The account is deactivated.")
            Throw New ApplicationException("This account has been deactivated. Contact the administrator.")
        End If

        Try
            LoginActivityService.RecordLogin(user)
        Catch ex As MySqlException
            Throw New ApplicationException(
                "Login activity storage is not ready. Apply docs/migrations/004_add_login_activity.sql, then try again.", ex)
        End Try
        SecurityLogService.Record(user.UserId, user.Username, "Successful Login", "The account signed in successfully.")
        Session.SessionId = ActiveSessionService.Start(user)
        Return user
    End Function

End Class