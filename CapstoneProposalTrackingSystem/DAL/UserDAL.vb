Imports MySql.Data.MySqlClient

Public Class UserDAL

    Public Shared Function GetUserByUsername(username As String) As User
        Dim user As User = Nothing

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT user_id, username, password_hash, role, full_name, email " &
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
                            .FullName = reader.GetString("full_name"),
                            .Email = reader.GetString("email")
                        }
                    End If
                End Using
            End Using
        End Using

        Return user
    End Function

End Class