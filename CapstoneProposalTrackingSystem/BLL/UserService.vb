Option Strict On
Option Explicit On

Imports System.Collections.Generic

Public Class UserService

    Public Shared Function GetAllUsers() As List(Of User)
        Return UserDAL.GetAllUsers()
    End Function

    Public Shared Function CreateUser(username As String, fullName As String, email As String, role As UserRole, plainPassword As String) As Integer
        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(fullName) OrElse String.IsNullOrWhiteSpace(email) Then
            Throw New ApplicationException("Username, full name, and email are all required.")
        End If

        If role = UserRole.SuperAdmin Then
            Throw New ApplicationException("Super Admin accounts can only be provisioned through the protected setup process.")
        End If

        If String.IsNullOrWhiteSpace(plainPassword) OrElse plainPassword.Length < 6 Then
            Throw New ApplicationException("Password must be at least 6 characters.")
        End If

        Dim newUser As New User With {
            .Username = username.Trim(),
            .FullName = fullName.Trim(),
            .Email = email.Trim(),
            .Role = role,
            .IsActive = True,
            .PasswordHash = PasswordHasher.HashPassword(plainPassword)
        }

        Return UserDAL.CreateUser(newUser)
    End Function

    Public Shared Sub UpdateUser(userId As Integer, username As String, fullName As String, email As String, role As UserRole, currentAdminUserId As Integer)
        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(fullName) OrElse String.IsNullOrWhiteSpace(email) Then
            Throw New ApplicationException("Username, full name, and email are required.")
        End If

        Dim targetUser As User = UserDAL.GetUserById(userId)
        If targetUser Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        If targetUser.Role = UserRole.SuperAdmin Then
            Dim actingUser As User = UserDAL.GetUserById(currentAdminUserId)
            If actingUser Is Nothing OrElse actingUser.Role <> UserRole.SuperAdmin Then
                Throw New ApplicationException("Only a Super Admin can edit the protected account.")
            End If

            If role <> UserRole.SuperAdmin Then
                Throw New ApplicationException("The Super Admin role cannot be removed.")
            End If
        ElseIf role = UserRole.SuperAdmin Then
            Throw New ApplicationException("The Super Admin role can only be assigned through the protected setup process.")
        End If

        UserDAL.UpdateUser(New User With {
            .UserId = userId,
            .Username = username.Trim(),
            .FullName = fullName.Trim(),
            .Email = email.Trim(),
            .Role = role
        })
    End Sub

    Public Shared Sub ResetPassword(userId As Integer, newPlainPassword As String, currentAdminUserId As Integer)
        If String.IsNullOrWhiteSpace(newPlainPassword) OrElse newPlainPassword.Length < 6 Then
            Throw New ApplicationException("Password must be at least 6 characters.")
        End If

        Dim targetUser As User = UserDAL.GetUserById(userId)
        If targetUser Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        If targetUser.Role = UserRole.SuperAdmin Then
            Dim actingUser As User = UserDAL.GetUserById(currentAdminUserId)
            If actingUser Is Nothing OrElse actingUser.Role <> UserRole.SuperAdmin Then
                Throw New ApplicationException("Only a Super Admin can reset the protected account's password.")
            End If
        End If

        UserDAL.UpdatePassword(userId, PasswordHasher.HashPassword(newPlainPassword))
    End Sub

    Public Shared Sub SetUserActive(userId As Integer, isActive As Boolean, currentAdminUserId As Integer)
        If userId = currentAdminUserId AndAlso Not isActive Then
            Throw New ApplicationException("You cannot deactivate your own account.")
        End If

        Dim targetUser As User = UserDAL.GetUserById(userId)
        If targetUser Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        If targetUser.Role = UserRole.SuperAdmin AndAlso Not isActive Then
            Throw New ApplicationException("The Super Admin account cannot be deactivated.")
        End If

        UserDAL.SetUserActive(userId, isActive)
    End Sub

    Public Shared Sub DeleteUser(userId As Integer, currentAdminUserId As Integer)
        If userId <= 0 Then
            Throw New ApplicationException("A valid account is required.")
        End If

        If userId = currentAdminUserId Then
            Throw New ApplicationException("You cannot delete your own account.")
        End If

        Dim targetUser As User = UserDAL.GetUserById(userId)
        If targetUser Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        If targetUser.Role = UserRole.SuperAdmin Then
            Throw New ApplicationException("The Super Admin account cannot be deleted.")
        End If

        UserDAL.DeleteUser(userId)
    End Sub

End Class