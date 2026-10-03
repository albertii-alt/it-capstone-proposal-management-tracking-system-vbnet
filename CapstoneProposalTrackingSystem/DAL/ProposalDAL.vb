Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

Public Class ProposalDAL

    Public Shared Function InsertProposal(proposal As Proposal) As Integer
        Dim newProposalId As Integer = 0

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim query As String = "INSERT INTO tbl_proposals (title, group_name, description, course, year_level, semester, status, submitted_by, submitted_at) " &
                                      "VALUES (@title, @group_name, @description, @course, @year_level, @semester, @status, @submitted_by, @submitted_at)"

                Using cmd As New MySqlCommand(query, conn, transaction)
                    cmd.Parameters.AddWithValue("@title", proposal.Title)
                    cmd.Parameters.AddWithValue("@group_name", proposal.GroupName)
                    cmd.Parameters.AddWithValue("@description", proposal.Description)
                    cmd.Parameters.AddWithValue("@course", proposal.Course)
                    cmd.Parameters.AddWithValue("@year_level", proposal.YearLevel)
                    cmd.Parameters.AddWithValue("@semester", proposal.Semester)
                    cmd.Parameters.AddWithValue("@status", proposal.Status.ToString())
                    cmd.Parameters.AddWithValue("@submitted_by", proposal.SubmittedBy)
                    cmd.Parameters.AddWithValue("@submitted_at", proposal.SubmittedAt)

                    cmd.ExecuteNonQuery()
                    newProposalId = Convert.ToInt32(cmd.LastInsertedId)
                End Using

                If proposal.Members IsNot Nothing Then
                    For Each member In proposal.Members
                        Dim memberQuery As String = "INSERT INTO tbl_proposal_members (proposal_id, member_name) " &
                                                    "VALUES (@proposal_id, @member_name)"

                        Using memberCmd As New MySqlCommand(memberQuery, conn, transaction)
                            memberCmd.Parameters.AddWithValue("@proposal_id", newProposalId)
                            memberCmd.Parameters.AddWithValue("@member_name", member.MemberName)
                            memberCmd.ExecuteNonQuery()
                        End Using
                    Next
                End If

                transaction.Commit()

            Catch ex As MySqlException
                transaction.Rollback()
                Throw New ApplicationException("Failed to save the proposal. Please try again.", ex)
            End Try
        End Using

        Return newProposalId
    End Function

    Public Shared Function GetAllProposals() As List(Of Proposal)
        Dim proposals As New List(Of Proposal)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT proposal_id, title, group_name, description, course, year_level, semester, status, submitted_by, submitted_at " &
                                  "FROM tbl_proposals ORDER BY submitted_at DESC"

            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim proposal As New Proposal With {
                            .ProposalId = reader.GetInt32("proposal_id"),
                            .Title = reader.GetString("title"),
                            .GroupName = reader.GetString("group_name"),
                            .Description = reader.GetString("description"),
                            .Course = reader.GetString("course"),
                            .YearLevel = reader.GetString("year_level"),
                            .Semester = reader.GetString("semester"),
                            .Status = CType([Enum].Parse(GetType(ProposalStatus), reader.GetString("status")), ProposalStatus),
                            .SubmittedBy = reader.GetInt32("submitted_by"),
                            .SubmittedAt = reader.GetDateTime("submitted_at")
                        }

                        proposal.Members = GetMembersForProposal(proposal.ProposalId)
                        proposals.Add(proposal)
                    End While
                End Using
            End Using
        End Using

        Return proposals
    End Function
    Public Shared Function GetProposalsBySubmitter(userId As Integer) As List(Of Proposal)
        Dim proposals As New List(Of Proposal)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT proposal_id, title, group_name, description, course, year_level, semester, status, submitted_by, submitted_at " &
                                  "FROM tbl_proposals WHERE submitted_by = @submitted_by ORDER BY submitted_at DESC"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@submitted_by", userId)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim proposal As New Proposal With {
                            .ProposalId = reader.GetInt32("proposal_id"),
                            .Title = reader.GetString("title"),
                            .GroupName = reader.GetString("group_name"),
                            .Description = reader.GetString("description"),
                            .Course = reader.GetString("course"),
                            .YearLevel = reader.GetString("year_level"),
                            .Semester = reader.GetString("semester"),
                            .Status = CType([Enum].Parse(GetType(ProposalStatus), reader.GetString("status")), ProposalStatus),
                            .SubmittedBy = reader.GetInt32("submitted_by"),
                            .SubmittedAt = reader.GetDateTime("submitted_at")
                        }

                        proposal.Members = GetMembersForProposal(proposal.ProposalId)
                        proposals.Add(proposal)
                    End While
                End Using
            End Using
        End Using

        Return proposals
    End Function

    Public Shared Function GetAllTitlesForDuplicateCheck() As List(Of String)
        Dim titles As New List(Of String)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT title FROM tbl_proposals"

            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        titles.Add(reader.GetString("title"))
                    End While
                End Using
            End Using
        End Using

        Return titles
    End Function

    Private Shared Function GetMembersForProposal(proposalId As Integer) As List(Of ProposalMember)
        Dim members As New List(Of ProposalMember)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT member_id, proposal_id, member_name " &
                                  "FROM tbl_proposal_members WHERE proposal_id = @proposal_id ORDER BY member_id"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@proposal_id", proposalId)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        members.Add(New ProposalMember With {
                            .MemberId = reader.GetInt32("member_id"),
                            .ProposalId = reader.GetInt32("proposal_id"),
                            .MemberName = reader.GetString("member_name")
                        })
                    End While
                End Using
            End Using
        End Using

        Return members
    End Function

    Public Shared Sub UpdateStatusWithLog(proposalId As Integer, newStatus As ProposalStatus, adminId As Integer, remarks As String)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Dim transaction As MySqlTransaction = conn.BeginTransaction()

            Try
                Dim oldStatus As String

                Using selectCmd As New MySqlCommand("SELECT status FROM tbl_proposals WHERE proposal_id = @proposal_id", conn, transaction)
                    selectCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                    Dim result As Object = selectCmd.ExecuteScalar()

                    If result Is Nothing Then
                        Throw New ApplicationException("That proposal no longer exists.")
                    End If

                    oldStatus = result.ToString()
                End Using

                If oldStatus = newStatus.ToString() Then
                    Throw New ApplicationException("The proposal already has that status.")
                End If

                Using updateCmd As New MySqlCommand("UPDATE tbl_proposals SET status = @status WHERE proposal_id = @proposal_id", conn, transaction)
                    updateCmd.Parameters.AddWithValue("@status", newStatus.ToString())
                    updateCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                    updateCmd.ExecuteNonQuery()
                End Using

                Dim logQuery As String = "INSERT INTO tbl_status_log (proposal_id, admin_id, old_status, new_status, remarks, changed_at) " &
                                         "VALUES (@proposal_id, @admin_id, @old_status, @new_status, @remarks, @changed_at)"

                Using logCmd As New MySqlCommand(logQuery, conn, transaction)
                    logCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                    logCmd.Parameters.AddWithValue("@admin_id", adminId)
                    logCmd.Parameters.AddWithValue("@old_status", oldStatus)
                    logCmd.Parameters.AddWithValue("@new_status", newStatus.ToString())
                    logCmd.Parameters.AddWithValue("@remarks", remarks)
                    logCmd.Parameters.AddWithValue("@changed_at", DateTime.Now)
                    logCmd.ExecuteNonQuery()
                End Using

                transaction.Commit()

            Catch ex As ApplicationException
                transaction.Rollback()
                Throw

            Catch ex As MySqlException
                transaction.Rollback()
                Throw New ApplicationException("Failed to save the decision. Please try again.", ex)
            End Try
        End Using
    End Sub

    Public Shared Function GetAllTitlesExcludingProposal(proposalId As Integer) As List(Of String)
        Dim titles As New List(Of String)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT title FROM tbl_proposals WHERE proposal_id <> @proposal_id"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@proposal_id", proposalId)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        titles.Add(reader.GetString("title"))
                    End While
                End Using
            End Using
        End Using

        Return titles
    End Function

    Public Shared Function GetAllTitleInfoExcludingProposal(proposalId As Integer) As List(Of ProposalTitleInfo)
        Dim results As New List(Of ProposalTitleInfo)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT title, group_name, submitted_at FROM tbl_proposals WHERE proposal_id <> @proposal_id"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@proposal_id", proposalId)

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        results.Add(New ProposalTitleInfo With {
                            .Title = reader.GetString("title"),
                            .GroupName = reader.GetString("group_name"),
                            .SubmittedAt = reader.GetDateTime("submitted_at")
                        })
                    End While
                End Using
            End Using
        End Using

        Return results
    End Function

    Public Shared Function DeleteProposal(proposalId As Integer) As Boolean
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Using logCmd As New MySqlCommand(
                        "DELETE FROM tbl_status_log WHERE proposal_id = @proposal_id",
                        conn,
                        transaction)
                        logCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                        logCmd.ExecuteNonQuery()
                    End Using

                    Using memberCmd As New MySqlCommand(
                        "DELETE FROM tbl_proposal_members WHERE proposal_id = @proposal_id",
                        conn,
                        transaction)
                        memberCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                        memberCmd.ExecuteNonQuery()
                    End Using

                    Dim deletedRows As Integer
                    Using proposalCmd As New MySqlCommand(
                        "DELETE FROM tbl_proposals WHERE proposal_id = @proposal_id",
                        conn,
                        transaction)
                        proposalCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                        deletedRows = proposalCmd.ExecuteNonQuery()
                    End Using

                    transaction.Commit()
                    Return deletedRows > 0
                Catch ex As MySqlException
                    transaction.Rollback()
                    Throw New ApplicationException("Failed to delete the proposal. Please try again.", ex)
                Catch
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function
End Class
