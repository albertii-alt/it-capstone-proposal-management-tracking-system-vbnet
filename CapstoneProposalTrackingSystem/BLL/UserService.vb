Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text.RegularExpressions
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

        Dim changedFields As New List(Of String)
        If Not String.Equals(existingUser.Username, username.Trim(), StringComparison.Ordinal) Then
            changedFields.Add("username")
        End If
        If Not String.Equals(existingUser.FullName, fullName.Trim(), StringComparison.Ordinal) Then
            changedFields.Add("full name")
        End If
        If Not String.Equals(existingUser.Email, email.Trim(), StringComparison.OrdinalIgnoreCase) Then
            changedFields.Add("email")
        End If
        If Not ProfilePhotosMatch(existingUser.ProfilePhoto, profilePhoto) Then
            changedFields.Add("profile photo")
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

        Dim actor As User = UserDAL.GetUserById(userId)
        If actor IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(actor.Role) Then
            Dim changedDescription As String = "Updated the administrator profile."
            If changedFields.Count > 0 Then
                changedDescription &= " Changed: " & String.Join(", ", changedFields) & "."
            End If
            AdminActivityLogService.Record(actor, "Updated", "Profile", actor.UserId, actor.FullName,
                                           changedDescription,
                                           "Changed fields: " & String.Join(", ", changedFields))
            If changedFields.Count > 0 Then
                SecurityLogService.Record(actor.UserId, actor.Username, "Profile Security Change",
                                          "Changed administrator profile fields: " & String.Join(", ", changedFields) & ".")
            End If
        End If
    End Sub

    Private Shared Function ProfilePhotosMatch(leftPhoto As Byte(), rightPhoto As Byte()) As Boolean
        If leftPhoto Is Nothing OrElse leftPhoto.Length = 0 Then
            Return rightPhoto Is Nothing OrElse rightPhoto.Length = 0
        End If
        If rightPhoto Is Nothing OrElse leftPhoto.Length <> rightPhoto.Length Then Return False
        For index As Integer = 0 To leftPhoto.Length - 1
            If leftPhoto(index) <> rightPhoto(index) Then Return False
        Next
        Return True
    End Function

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

        Dim actor As User = UserDAL.GetUserById(userId)
        If actor IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(actor.Role) Then
            AdminActivityLogService.Record(actor, "Changed Password", "Profile", actor.UserId, actor.FullName,
                                           "Changed the administrator account password.")
            SecurityLogService.Record(actor.UserId, actor.Username, "Password Changed",
                                      "The administrator changed their account password.")
        End If
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
            .AccountStatus = "Approved",
            .PasswordHash = PasswordHasher.HashPassword(plainPassword)
        }

        Dim newUserId As Integer = UserDAL.CreateUser(newUser)
        If currentAdminUserId > 0 Then
            Dim actor As User = UserDAL.GetUserById(currentAdminUserId)
            AdminActivityLogService.Record(actor, "Created", "Account", newUserId, newUser.FullName,
                                           "Created the " & newUser.Role.ToString() & " account for " & newUser.FullName & ".")
            SecurityLogService.Record(actor.UserId, actor.Username, "Account Created",
                                       "Created the " & newUser.Role.ToString() & " account for " & newUser.FullName & ".")
        End If
        Return newUserId
    End Function

    Public Shared Function RegisterStudent(schoolId As String, username As String, fullName As String, email As String, plainPassword As String) As Integer
        If String.IsNullOrWhiteSpace(schoolId) OrElse
           String.IsNullOrWhiteSpace(username) OrElse
           String.IsNullOrWhiteSpace(fullName) OrElse
           String.IsNullOrWhiteSpace(email) Then
            Throw New ApplicationException("School ID, username, full name, and email are all required.")
        End If

        Dim normalizedSchoolId As String = schoolId.Trim()
        If Not Regex.IsMatch(normalizedSchoolId, "^[0-9]{2}-[0-9]{6}$") Then
            Throw New ApplicationException("Enter a valid school ID using the format 24-018701.")
        End If
        If username.Trim().Length > 50 OrElse fullName.Trim().Length > 100 OrElse email.Trim().Length > 100 Then
            Throw New ApplicationException("School ID, username, full name, or email exceeds the allowed length.")
        End If
        If String.IsNullOrWhiteSpace(plainPassword) OrElse plainPassword.Length < 6 Then
            Throw New ApplicationException("Password must be at least 6 characters.")
        End If

        Try
            Dim parsedEmail As New MailAddress(email.Trim())
            If parsedEmail.Address <> email.Trim() Then Throw New ApplicationException("Enter a valid email address.")
        Catch ex As FormatException
            Throw New ApplicationException("Enter a valid email address.", ex)
        End Try

        Dim newUser As New User With {
            .SchoolId = normalizedSchoolId,
            .Username = username.Trim(),
            .FullName = fullName.Trim(),
            .Email = email.Trim(),
            .Role = UserRole.Student,
            .IsActive = False,
            .AccountStatus = "Pending",
            .PasswordHash = PasswordHasher.HashPassword(plainPassword),
            .RegisteredAt = DateTime.Now
        }

        Try
            Return UserDAL.CreateUser(newUser)
        Catch ex As MySqlException
            Throw New ApplicationException("Could not submit the registration. Please try again.", ex)
        End Try
    End Function

    Public Shared Function GetPendingStudentRegistrations() As List(Of User)
        Try
            Return UserDAL.GetPendingStudentUsers()
        Catch ex As MySqlException
            Throw New ApplicationException("Could not load student registrations. Please try again.", ex)
        End Try
    End Function

    Public Shared Sub ReviewStudentRegistration(userId As Integer, approve As Boolean, reviewerId As Integer, remarks As String)
        If reviewerId <= 0 Then Throw New ApplicationException("A valid administrator is required.")
        Dim reviewer As User = UserDAL.GetUserById(reviewerId)
        If reviewer Is Nothing OrElse Not UserRoleHelper.IsAdministrator(reviewer.Role) Then
            Throw New ApplicationException("Only an administrator can review student registrations.")
        End If

        Dim target As User = UserDAL.GetUserById(userId)
        If target Is Nothing OrElse target.Role <> UserRole.Student Then
            Throw New ApplicationException("That student registration no longer exists.")
        End If

        Dim newStatus As String = If(approve, "Approved", "Rejected")
        UserDAL.ReviewStudentAccount(userId, newStatus, approve, reviewerId, remarks)
        AdminActivityLogService.Record(reviewer,
                                       If(approve, "Approved", "Rejected"),
                                       "Student Registration",
                                       target.UserId,
                                       target.FullName,
                                       If(approve,
                                          "Approved the student registration for " & target.FullName & " (" & target.SchoolId & ").",
                                          "Rejected the student registration for " & target.FullName & " (" & target.SchoolId & ")."),
                                       If(String.IsNullOrWhiteSpace(remarks), Nothing, "Remarks: " & remarks.Trim()))
        SecurityLogService.Record(reviewer.UserId, reviewer.Username,
                                  If(approve, "Student Registration Approved", "Student Registration Rejected"),
                                  If(approve, "Approved ", "Rejected ") & target.FullName & "'s registration (" & target.SchoolId & ").")
    End Sub

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

        Dim changedUsername As Boolean = Not String.Equals(targetUser.Username, username.Trim(), StringComparison.Ordinal)
        Dim changedEmail As Boolean = Not String.Equals(targetUser.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)
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
        If targetUser.Role <> role Then
            SecurityLogService.Record(actorForLog.UserId, actorForLog.Username, "Role Changed",
                                       "Changed " & targetUser.FullName & "'s role from " &
                                       targetUser.Role.ToString() & " to " & role.ToString() & ".")
        End If
        If changedUsername OrElse changedEmail Then
            Dim changedFields As New List(Of String)
            If changedUsername Then changedFields.Add("username")
            If changedEmail Then changedFields.Add("email")
            SecurityLogService.Record(actorForLog.UserId, actorForLog.Username, "Account Security Change",
                                       "Changed " & targetUser.FullName & "'s " & String.Join(" and ", changedFields) & ".")
        End If
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
        SecurityLogService.Record(actorForLog.UserId, actorForLog.Username, "Password Reset",
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
        SecurityLogService.Record(actorForLog.UserId, actorForLog.Username,
                                  If(isActive, "Account Activated", "Account Deactivated"),
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
        SecurityLogService.Record(actorForLog.UserId, actorForLog.Username, "Account Deleted",
                                  "Deleted the account for " & targetUser.FullName & ".")
    End Sub

End Class