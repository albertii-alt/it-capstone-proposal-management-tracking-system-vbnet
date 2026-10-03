Imports System.Security.Cryptography
Imports System.Text

Public Class PasswordHasher

    Public Shared Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim hashBytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
            Return Convert.ToBase64String(hashBytes)
        End Using
    End Function

    Public Shared Function VerifyPassword(password As String, storedHash As String) As Boolean
        Dim computedHash As String = HashPassword(password)
        Return computedHash = storedHash
    End Function

End Class