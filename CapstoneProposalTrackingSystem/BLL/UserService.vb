Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Net.Mail
Imports MySql.Data.MySqlClient

Public Class UserService

    Public Shared Function GetAllUsers() As List(Of User)
        Return UserDAL.GetAllUsers()
    End Function

    Public Shared Function GetOwnProfile(userId As Integer) As User
        If userId <= 0 Then
            Throw New ApplicationException("A valid account is required.")
        End If

        Dim user As User
        Try
            user = UserDAL.GetUserById(userId)
        Catch ex As MySqlException
            Throw New ApplicationException("Could not load your profile. Please try again.", ex)
        End Try
        If user Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        Return user
    End Function

    Public Shared Sub UpdateOwnProfile(userId As Integer, username As String, fullName As String, email As String, profilePhoto As Byte())
        If userId <= 0 Then
            Throw New ApplicationException("A valid account is required.")
        End If

        If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(fullName) OrElse String.IsNullOrWhiteSpace(email) Then
            Throw New ApplicationException("Username, full name, and email are all required.")
        End If

        If username.Trim().Length > 50 OrElse fullName.Trim().Length > 100 OrElse email.Trim().Length > 100 Then
            Throw New ApplicationException("Username, full name, or email exceeds the allowed length.")
        End If

        Try
            Dim parsedEmail As New MailAddress(email.Trim())
            If parsedEmail.Address <> email.Trim() Then
                Throw New ApplicationException("Enter a valid email address.")
            End If
        Catch ex As FormatException
            Throw New ApplicationException("Enter a valid email address.", ex)
        End Try

        Dim existingUser As User
        Try
            existingUser = UserDAL.GetUserById(userId)
        Catch ex As MySqlException
            Throw New ApplicationException("Could not verify your account. Please try again.", ex)
        End Try
        If existingUser Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        Try
            UserDAL.UpdateOwnProfile(New User With {
                .UserId = userId,
                .Username = username.Trim(),
                .FullName = fullName.Trim(),
                .Email = email.Trim(),
                .ProfilePhoto = profilePhoto
            })
        Catch ex As MySqlException
            Throw New ApplicationException("Could not save your profile. Please try again.", ex)
        End Try
    End Sub

    Public Shared Sub ChangeOwnPassword(userId As Integer, currentPassword As String, newPassword As String)
        If userId <= 0 Then
            Throw New ApplicationException("A valid account is required.")
        End If

        If String.IsNullOrWhiteSpace(currentPassword) Then
            Throw New ApplicationException("Enter your current password.")
        End If

        If String.IsNullOrWhiteSpace(newPassword) OrElse newPassword.Length < 6 Then
            Throw New ApplicationException("Password must be at least 6 characters.")
        End If

        Dim user As User
        Try
            user = UserDAL.GetUserById(userId)
        Catch ex As MySqlException
            Throw New ApplicationException("Could not verify your current password. Please try again.", ex)
        End Try
        If user Is Nothing Then
            Throw New ApplicationException("That account no longer exists.")
        End If

        If Not PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash) Then
            Throw New ApplicationException("Your current password is incorrect.")
        End If

        If currentPassword = newPassword Then
            Throw New ApplicationException("Choose a new password different from your current password.")
        End If

        Try
            UserDAL.UpdatePassword(userId, PasswordHasher.HashPassword(newPassword))
        Catch ex As MySqlException
            Throw New ApplicationException("Could not update your password. Please try again.", ex)
        End Try
    End Sub

    Public Shared Function CreateUser(username As String, fullName As String, email As String, role As UserRole, plainPassword As String, Optional currentAdminUserId As Integer = 0) As Integer
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

        Dim newUserId As Integer = UserDAL.CreateUser(newUser)
        If currentAdminUserId > 0 Then
            Dim actor As User = UserDAL.GetUserById(currentAdminUserId)
            AdminActivityLogService.Record(actor, "Created", "Account", newUserId, newUser.FullName,
                                           "Created the " & newUser.Role.ToString() & " account for " & newUser.FullName & ".")
        End If
        Return newUserId
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

        Dim actorForLog As User = UserDAL.GetUserById(currentAdminUserId)
        AdminActivityLogService.Record(actorForLog, "Updated", "Account", targetUser.UserId, targetUser.FullName,
                                       "Updated the account details for " & targetUser.FullName & ".",
                                       "Role: " & targetUser.Role.ToString() & " -> " & role.ToString())
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
        Dim actorForLog As User = UserDAL.GetUserById(currentAdminUserId)
        AdminActivityLogService.Record(actorForLog, "Reset Password", "Account", targetUser.UserId, targetUser.FullName,
                                       "Reset the password for " & targetUser.FullName & ".")
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
        Dim actorForLog As User = UserDAL.GetUserById(currentAdminUserId)
        AdminActivityLogService.Record(actorForLog, If(isActive, "Activated", "Deactivated"), "Account",
                                       targetUser.UserId, targetUser.FullName,
                                       If(isActive, "Activated ", "Deactivated ") & targetUser.FullName & "'s account.")
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
        Dim actorForLog As User = UserDAL.GetUserById(currentAdminUserId)
        AdminActivityLogService.Record(actorForLog, "Deleted", "Account", targetUser.UserId, targetUser.FullName,
                                       "Deleted the account for " & targetUser.FullName & ".")
    End Sub

End Class