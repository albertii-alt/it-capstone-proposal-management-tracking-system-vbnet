Imports MySql.Data.MySqlClient
Imports System.Collections.Generic

Public Class RecentActivityEntry
    Public Property ProposalTitle As String
    Public Property OldStatus As String
    Public Property NewStatus As String
    Public Property ChangedAt As DateTime
End Class

Public Class StatusLogDAL

    Public Shared Function GetLatestLogForProposal(proposalId As Integer) As StatusLogEntry
        Dim log As StatusLogEntry = Nothing

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT log_id, proposal_id, admin_id, old_status, new_status, remarks, changed_at " &
                                  "FROM tbl_status_log WHERE proposal_id = @proposal_id " &
                                  "ORDER BY changed_at DESC LIMIT 1"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@proposal_id", proposalId)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        log = New StatusLogEntry With {
                            .LogId = reader.GetInt32("log_id"),
                            .ProposalId = reader.GetInt32("proposal_id"),
                            .AdminId = reader.GetInt32("admin_id"),
                            .NewStatus = CType([Enum].Parse(GetType(ProposalStatus), reader.GetString("new_status")), ProposalStatus),
                            .Remarks = If(reader.IsDBNull(reader.GetOrdinal("remarks")), "", reader.GetString("remarks")),
                            .ChangedAt = reader.GetDateTime("changed_at")
                        }

                        If Not reader.IsDBNull(reader.GetOrdinal("old_status")) Then
                            log.OldStatus = CType([Enum].Parse(GetType(ProposalStatus), reader.GetString("old_status")), ProposalStatus)
                        End If
                    End If
                End Using
            End Using
        End Using

        Return log
    End Function

    Public Shared Sub InsertLog(log As StatusLogEntry)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "INSERT INTO tbl_status_log (proposal_id, admin_id, old_status, new_status, remarks, changed_at) " &
                                  "VALUES (@proposal_id, @admin_id, @old_status, @new_status, @remarks, @changed_at)"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@proposal_id", log.ProposalId)
                cmd.Parameters.AddWithValue("@admin_id", log.AdminId)
                cmd.Parameters.AddWithValue("@old_status", If(log.OldStatus.HasValue, log.OldStatus.Value.ToString(), CObj(DBNull.Value)))
                cmd.Parameters.AddWithValue("@new_status", log.NewStatus.ToString())
                cmd.Parameters.AddWithValue("@remarks", log.Remarks)
                cmd.Parameters.AddWithValue("@changed_at", log.ChangedAt)

                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Function GetRecentActivity(maxRows As Integer) As List(Of RecentActivityEntry)
        Dim entries As New List(Of RecentActivityEntry)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT p.title, l.old_status, l.new_status, l.changed_at " &
                                  "FROM tbl_status_log l " &
                                  "INNER JOIN tbl_proposals p ON p.proposal_id = l.proposal_id " &
                                  "ORDER BY l.changed_at DESC " &
                                  "LIMIT @max_rows"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@max_rows", maxRows)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        entries.Add(New RecentActivityEntry With {
                            .ProposalTitle = reader.GetString("title"),
                            .OldStatus = If(reader.IsDBNull(reader.GetOrdinal("old_status")), "", reader.GetString("old_status")),
                            .NewStatus = reader.GetString("new_status"),
                            .ChangedAt = reader.GetDateTime("changed_at")
                        })
                    End While
                End Using
            End Using
        End Using

        Return entries
    End Function

End Class