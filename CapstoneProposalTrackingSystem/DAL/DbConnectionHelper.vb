Imports System.Configuration
Imports MySql.Data.MySqlClient

Public Class DbConnectionHelper

    Private Shared ReadOnly ConnStr As String =
        ConfigurationManager.ConnectionStrings("CapstoneDb").ConnectionString

    Public Shared Function GetConnection() As MySqlConnection
        Return New MySqlConnection(ConnStr)
    End Function

End Class