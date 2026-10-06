Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

Public Class StudentHistoryDAL
    Public Shared Sub Insert(entry As StudentHistoryEntry)
        If entry Is Nothing Then Throw New ArgumentNullException(NameOf(entry))
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Const query As String = "INSERT INTO tbl_student_history " &
                                    "(student_id, proposal_id, proposal_title, action_type, remarks, recorded_at) " &
                                    "VALUES (@student_id, @proposal_id, @proposal_title, @action_type, @remarks, @recorded_at)"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@student_id", entry.StudentId)
                cmd.Parameters.AddWithValue("@proposal_id", If(entry.ProposalId.HasValue, CType(entry.ProposalId.Value, Object), DBNull.Value))
                cmd.Parameters.AddWithValue("@proposal_title", entry.ProposalTitle)
                cmd.Parameters.AddWithValue("@action_type", entry.Action)
                cmd.Parameters.AddWithValue("@remarks", entry.Remarks)
                cmd.Parameters.AddWithValue("@recorded_at", entry.RecordedAt)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetForStudent(studentId As Integer) As List(Of StudentHistoryEntry)
        Dim entries As New List(Of StudentHistoryEntry)()
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Const query As String = "SELECT history_id, student_id, proposal_id, proposal_title, action_type, remarks, recorded_at " &
                                    "FROM tbl_student_history WHERE student_id = @student_id " &
                                    "ORDER BY recorded_at DESC, history_id DESC"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@student_id", studentId)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        entries.Add(New StudentHistoryEntry With {
                            .HistoryId = reader.GetInt32("history_id"),
                            .StudentId = reader.GetInt32("student_id"),
                            .ProposalId = If(reader.IsDBNull(reader.GetOrdinal("proposal_id")), Nothing, CType(reader.GetInt32("proposal_id"), Integer?)),
                            .ProposalTitle = reader.GetString("proposal_title"),
                            .Action = reader.GetString("action_type"),
                            .Remarks = reader.GetString("remarks"),
                            .RecordedAt = reader.GetDateTime("recorded_at")
                        })
                    End While
                End Using
            End Using
        End Using
        Return entries
    End Function

    Public Shared Function DeleteForStudent(studentId As Integer, historyIds As List(Of Integer)) As Integer
        If historyIds Is Nothing Then Throw New ArgumentNullException(NameOf(historyIds))
        If historyIds.Count = 0 Then Return 0
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Dim deletedCount As Integer = 0
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                For Each historyId As Integer In historyIds
                    Using cmd As New MySqlCommand("DELETE FROM tbl_student_history WHERE history_id = @history_id AND student_id = @student_id", conn, transaction)
                        cmd.Parameters.AddWithValue("@history_id", historyId)
                        cmd.Parameters.AddWithValue("@student_id", studentId)
                        deletedCount += cmd.ExecuteNonQuery()
                    End Using
                Next
                transaction.Commit()
            End Using
            Return deletedCount
        End Using
    End Function
End Class
