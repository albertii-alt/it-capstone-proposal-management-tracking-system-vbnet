Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class UserDAL

    Public Shared Function GetUserByUsername(username As String) As User
        Dim user As User = Nothing

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT user_id, username, password_hash, role, is_active, full_name, email, profile_photo " &
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
                            .ProfilePhoto = If(reader.IsDBNull(reader.GetOrdinal("profile_photo")), Nothing, CType(reader("profile_photo"), Byte()))
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

            Dim query As String = "SELECT user_id, username, password_hash, role, is_active, full_name, email, profile_photo " &
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
                            .ProfilePhoto = If(reader.IsDBNull(reader.GetOrdinal("profile_photo")), Nothing, CType(reader("profile_photo"), Byte()))
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

            Dim query As String = "SELECT user_id, username, password_hash, role, is_active, full_name, email " &
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
                            .Email = reader.GetString("email")
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

            Dim query As String = "INSERT INTO tbl_users (username, password_hash, role, is_active, full_name, email) " &
                                  "VALUES (@username, @password_hash, @role, @is_active, @full_name, @email)"

            Try
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", user.Username)
                    cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash)
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString())
                    cmd.Parameters.AddWithValue("@is_active", user.IsActive)
                    cmd.Parameters.AddWithValue("@full_name", user.FullName)
                    cmd.Parameters.AddWithValue("@email", user.Email)
                    cmd.ExecuteNonQuery()
                    Return Convert.ToInt32(cmd.LastInsertedId)
                End Using
            Catch ex As MySqlException When ex.Number = 1062
                Throw New ApplicationException("That username or email is already in use.", ex)
            End Try
        End Using
    End Function

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