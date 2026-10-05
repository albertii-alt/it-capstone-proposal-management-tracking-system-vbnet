Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class UserDAL

    Public Shared Function GetUserByUsername(username As String) As User
        Dim user As User = Nothing

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT user_id, username, password_hash, role, is_active, full_name, email, profile_photo, school_id, account_status, registered_at, reviewed_at, reviewed_by, review_remarks " &
                                   "FROM tbl_users WHERE username = @username"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@username", username)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        user = New User With {
                            .UserId = reader.GetInt32("user_id"),
                            .Username = reader.GetString("username"),
                            .PasswordHash = reader.GetString("password_hash"),
                            .Role = CType([Enum].Parse(GetType(UserRole), reader.GetString("role")), UserRole),
                            .IsActive = reader.GetBoolean("is_active"),
                            .FullName = reader.GetString("full_name"),
                            .Email = reader.GetString("email"),
                            .ProfilePhoto = If(reader.IsDBNull(reader.GetOrdinal("profile_photo")), Nothing, CType(reader("profile_photo"), Byte())),
                            .SchoolId = If(reader.IsDBNull(reader.GetOrdinal("school_id")), Nothing, reader.GetString("school_id")),
                            .AccountStatus = reader.GetString("account_status"),
                            .RegisteredAt = If(reader.IsDBNull(reader.GetOrdinal("registered_at")), Nothing, reader.GetDateTime("registered_at")),
                            .ReviewedAt = If(reader.IsDBNull(reader.GetOrdinal("reviewed_at")), Nothing, reader.GetDateTime("reviewed_at")),
                            .ReviewedBy = If(reader.IsDBNull(reader.GetOrdinal("reviewed_by")), Nothing, reader.GetInt32("reviewed_by")),
                            .ReviewRemarks = If(reader.IsDBNull(reader.GetOrdinal("review_remarks")), Nothing, reader.GetString("review_remarks"))
                        }
                    End If
                End Using
            End Using
        End Using

        Return user
    End Function

    Public Shared Function GetUserById(userId As Integer) As User
        Dim user As User = Nothing

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT user_id, username, password_hash, role, is_active, full_name, email, profile_photo, school_id, account_status, registered_at, reviewed_at, reviewed_by, review_remarks " &
                                  "FROM tbl_users WHERE user_id = @user_id"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@user_id", userId)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        user = New User With {
                            .UserId = reader.GetInt32("user_id"),
                            .Username = reader.GetString("username"),
                            .PasswordHash = reader.GetString("password_hash"),
                            .Role = CType([Enum].Parse(GetType(UserRole), reader.GetString("role")), UserRole),
                            .IsActive = reader.GetBoolean("is_active"),
                            .FullName = reader.GetString("full_name"),
                            .Email = reader.GetString("email"),
                            .ProfilePhoto = If(reader.IsDBNull(reader.GetOrdinal("profile_photo")), Nothing, CType(reader("profile_photo"), Byte())),
                            .SchoolId = If(reader.IsDBNull(reader.GetOrdinal("school_id")), Nothing, reader.GetString("school_id")),
                            .AccountStatus = reader.GetString("account_status"),
                            .RegisteredAt = If(reader.IsDBNull(reader.GetOrdinal("registered_at")), Nothing, reader.GetDateTime("registered_at")),
                            .ReviewedAt = If(reader.IsDBNull(reader.GetOrdinal("reviewed_at")), Nothing, reader.GetDateTime("reviewed_at")),
                            .ReviewedBy = If(reader.IsDBNull(reader.GetOrdinal("reviewed_by")), Nothing, reader.GetInt32("reviewed_by")),
                            .ReviewRemarks = If(reader.IsDBNull(reader.GetOrdinal("review_remarks")), Nothing, reader.GetString("review_remarks"))
                        }
                    End If
                End Using
            End Using
        End Using

        Return user
    End Function

    Public Shared Function GetAllUsers() As List(Of User)
        Dim users As New List(Of User)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT user_id, username, password_hash, role, is_active, full_name, email, school_id, account_status, registered_at, reviewed_at, reviewed_by, review_remarks " &
                                  "FROM tbl_users ORDER BY full_name"

            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        users.Add(New User With {
                            .UserId = reader.GetInt32("user_id"),
                            .Username = reader.GetString("username"),
                            .PasswordHash = reader.GetString("password_hash"),
                            .Role = CType([Enum].Parse(GetType(UserRole), reader.GetString("role")), UserRole),
                            .IsActive = reader.GetBoolean("is_active"),
                            .FullName = reader.GetString("full_name"),
                            .Email = reader.GetString("email"),
                            .SchoolId = If(reader.IsDBNull(reader.GetOrdinal("school_id")), Nothing, reader.GetString("school_id")),
                            .AccountStatus = reader.GetString("account_status"),
                            .RegisteredAt = If(reader.IsDBNull(reader.GetOrdinal("registered_at")), Nothing, reader.GetDateTime("registered_at")),
                            .ReviewedAt = If(reader.IsDBNull(reader.GetOrdinal("reviewed_at")), Nothing, reader.GetDateTime("reviewed_at")),
                            .ReviewedBy = If(reader.IsDBNull(reader.GetOrdinal("reviewed_by")), Nothing, reader.GetInt32("reviewed_by")),
                            .ReviewRemarks = If(reader.IsDBNull(reader.GetOrdinal("review_remarks")), Nothing, reader.GetString("review_remarks"))
                        })
                    End While
                End Using
            End Using
        End Using

        Return users
    End Function

    Public Shared Function CreateUser(user As User) As Integer
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "INSERT INTO tbl_users (username, password_hash, role, is_active, full_name, email, school_id, account_status, registered_at, reviewed_at, reviewed_by, review_remarks) " &
                                  "VALUES (@username, @password_hash, @role, @is_active, @full_name, @email, @school_id, @account_status, COALESCE(@registered_at, CURRENT_TIMESTAMP), @reviewed_at, @reviewed_by, @review_remarks)"

            Try
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", user.Username)
                    cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash)
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString())
                    cmd.Parameters.AddWithValue("@is_active", user.IsActive)
                    cmd.Parameters.AddWithValue("@full_name", user.FullName)
                    cmd.Parameters.AddWithValue("@email", user.Email)
                    cmd.Parameters.AddWithValue("@school_id", If(String.IsNullOrWhiteSpace(user.SchoolId), CType(DBNull.Value, Object), user.SchoolId))
                    cmd.Parameters.AddWithValue("@account_status", If(String.IsNullOrWhiteSpace(user.AccountStatus), "Approved", user.AccountStatus))
                    cmd.Parameters.AddWithValue("@registered_at", If(user.RegisteredAt.HasValue, CType(user.RegisteredAt.Value, Object), DBNull.Value))
                    cmd.Parameters.AddWithValue("@reviewed_at", If(user.ReviewedAt.HasValue, CType(user.ReviewedAt.Value, Object), DBNull.Value))
                    cmd.Parameters.AddWithValue("@reviewed_by", If(user.ReviewedBy.HasValue, CType(user.ReviewedBy.Value, Object), DBNull.Value))
                    cmd.Parameters.AddWithValue("@review_remarks", If(String.IsNullOrWhiteSpace(user.ReviewRemarks), CType(DBNull.Value, Object), user.ReviewRemarks))
                    cmd.ExecuteNonQuery()
                    Return Convert.ToInt32(cmd.LastInsertedId)
                End Using
            Catch ex As MySqlException When ex.Number = 1062
                Throw New ApplicationException("That username, email, or school ID is already in use.", ex)
            End Try
        End Using
    End Function

    Public Shared Function GetPendingStudentUsers() As List(Of User)
        Dim users As New List(Of User)()
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Const query As String = "SELECT user_id, username, full_name, email, school_id, role, is_active, account_status, registered_at, review_remarks " &
                                    "FROM tbl_users WHERE role = 'Student' AND account_status = 'Pending' ORDER BY registered_at"
            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        users.Add(New User With {
                            .UserId = reader.GetInt32("user_id"),
                            .Username = reader.GetString("username"),
                            .FullName = reader.GetString("full_name"),
                            .Email = reader.GetString("email"),
                            .SchoolId = reader.GetString("school_id"),
                            .Role = UserRole.Student,
                            .IsActive = reader.GetBoolean("is_active"),
                            .AccountStatus = reader.GetString("account_status"),
                            .RegisteredAt = reader.GetDateTime("registered_at"),
                            .ReviewRemarks = If(reader.IsDBNull(reader.GetOrdinal("review_remarks")), Nothing, reader.GetString("review_remarks"))
                        })
                    End While
                End Using
            End Using
        End Using
        Return users
    End Function

    Public Shared Sub ReviewStudentAccount(userId As Integer, status As String, isActive As Boolean, reviewerId As Integer, remarks As String)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Const query As String = "UPDATE tbl_users SET account_status = @account_status, is_active = @is_active, reviewed_at = CURRENT_TIMESTAMP, reviewed_by = @reviewed_by, review_remarks = @review_remarks " &
                                    "WHERE user_id = @user_id AND role = 'Student'"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@account_status", status)
                cmd.Parameters.AddWithValue("@is_active", isActive)
                cmd.Parameters.AddWithValue("@reviewed_by", reviewerId)
                cmd.Parameters.AddWithValue("@review_remarks", If(String.IsNullOrWhiteSpace(remarks), CType(DBNull.Value, Object), remarks.Trim()))
                cmd.Parameters.AddWithValue("@user_id", userId)
                If cmd.ExecuteNonQuery() = 0 Then Throw New ApplicationException("That student registration is no longer available.")
            End Using
        End Using
    End Sub

    Public Shared Sub UpdateUser(user As User)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "UPDATE tbl_users SET username = @username, full_name = @full_name, email = @email, role = @role " &
                                  "WHERE user_id = @user_id"

            Try
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", user.Username)
                    cmd.Parameters.AddWithValue("@full_name", user.FullName)
                    cmd.Parameters.AddWithValue("@email", user.Email)
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString())
                    cmd.Parameters.AddWithValue("@user_id", user.UserId)
                    cmd.ExecuteNonQuery()
                End Using
            Catch ex As MySqlException When ex.Number = 1062
                Throw New ApplicationException("That username or email is already in use.", ex)
            End Try
        End Using
    End Sub

    Public Shared Sub UpdateOwnProfile(user As User)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "UPDATE tbl_users SET username = @username, full_name = @full_name, email = @email, profile_photo = @profile_photo " &
                                  "WHERE user_id = @user_id"

            Try
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", user.Username)
                    cmd.Parameters.AddWithValue("@full_name", user.FullName)
                    cmd.Parameters.AddWithValue("@email", user.Email)
                    Dim photoParameter = cmd.Parameters.Add("@profile_photo", MySqlDbType.MediumBlob)
                    photoParameter.Value = If(user.ProfilePhoto Is Nothing, CType(DBNull.Value, Object), user.ProfilePhoto)
                    cmd.Parameters.AddWithValue("@user_id", user.UserId)
                    If cmd.ExecuteNonQuery() = 0 Then
                        Throw New ApplicationException("That account no longer exists.")
                    End If
                End Using
            Catch ex As MySqlException When ex.Number = 1062
                Throw New ApplicationException("That username or email is already in use.", ex)
            End Try
        End Using
    End Sub

    Public Shared Sub SetUserActive(userId As Integer, isActive As Boolean)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Using cmd As New MySqlCommand("UPDATE tbl_users SET is_active = @is_active WHERE user_id = @user_id", conn)
                cmd.Parameters.AddWithValue("@is_active", isActive)
                cmd.Parameters.AddWithValue("@user_id", userId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub UpdatePassword(userId As Integer, newPasswordHash As String)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Using cmd As New MySqlCommand("UPDATE tbl_users SET password_hash = @password_hash WHERE user_id = @user_id", conn)
                cmd.Parameters.AddWithValue("@password_hash", newPasswordHash)
                cmd.Parameters.AddWithValue("@user_id", userId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub DeleteUser(userId As Integer)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Try
                Using cmd As New MySqlCommand("DELETE FROM tbl_users WHERE user_id = @user_id", conn)
                    cmd.Parameters.AddWithValue("@user_id", userId)
                    If cmd.ExecuteNonQuery() = 0 Then
                        Throw New ApplicationException("That account no longer exists.")
                    End If
                End Using
            Catch ex As MySqlException When ex.Number = 1451
                Throw New ApplicationException("This account is linked to proposal submissions or review history and cannot be deleted. Deactivate it instead.", ex)
            End Try
        End Using
    End Sub

End Class