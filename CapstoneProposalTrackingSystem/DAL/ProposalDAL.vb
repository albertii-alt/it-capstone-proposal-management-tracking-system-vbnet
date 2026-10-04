Imports System.Collections.Generic
Imports MySql.Data.MySqlClient

Public Class ArchivedTitle
    Public Property ArchiveId As Integer
    Public Property ProposalId As Integer
    Public Property Title As String
    Public Property GroupName As String
    Public Property YearLevel As String
    Public Property Keywords As String
    Public Property SemesterApproved As String
    Public Property ApprovedAt As DateTime
    Public Property SubmittedAt As DateTime
End Class

Public Class ProposalDAL

    Public Shared Function InsertProposal(proposal As Proposal, Optional adminActorName As String = Nothing) As Integer
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

                If Not String.IsNullOrWhiteSpace(adminActorName) Then
                    Dim activityQuery As String = "INSERT INTO tbl_proposal_activity (action_type, proposal_title, actor_name, created_at) " &
                                                  "VALUES (@action_type, @proposal_title, @actor_name, @created_at)"

                    Using activityCmd As New MySqlCommand(activityQuery, conn, transaction)
                        activityCmd.Parameters.AddWithValue("@action_type", "Submitted")
                        activityCmd.Parameters.AddWithValue("@proposal_title", proposal.Title)
                        activityCmd.Parameters.AddWithValue("@actor_name", adminActorName.Trim())
                        activityCmd.Parameters.AddWithValue("@created_at", proposal.SubmittedAt)
                        activityCmd.ExecuteNonQuery()
                    End Using
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

    Public Shared Sub UpdateStatusWithLog(proposalId As Integer, newStatus As ProposalStatus, adminId As Integer, remarks As String, proposalTitle As String, proposalKeywords As String, proposalSemester As String, groupName As String, yearLevel As String, submittedAt As DateTime)
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

                Dim changedAt As DateTime = DateTime.Now

                Dim logQuery As String = "INSERT INTO tbl_status_log (proposal_id, admin_id, old_status, new_status, remarks, changed_at) " &
                                         "VALUES (@proposal_id, @admin_id, @old_status, @new_status, @remarks, @changed_at)"

                Using logCmd As New MySqlCommand(logQuery, conn, transaction)
                    logCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                    logCmd.Parameters.AddWithValue("@admin_id", adminId)
                    logCmd.Parameters.AddWithValue("@old_status", oldStatus)
                    logCmd.Parameters.AddWithValue("@new_status", newStatus.ToString())
                    logCmd.Parameters.AddWithValue("@remarks", remarks)
                    logCmd.Parameters.AddWithValue("@changed_at", changedAt)
                    logCmd.ExecuteNonQuery()
                End Using

                If newStatus = ProposalStatus.Approved Then
                    Dim archiveQuery As String = "INSERT INTO tbl_title_archive (proposal_id, title, group_name, keywords, semester_approved, year_level, submitted_at, approved_at) " &
                                                 "VALUES (@proposal_id, @title, @group_name, @keywords, @semester, @year_level, @submitted_at, @approved_at) " &
                                                 "ON DUPLICATE KEY UPDATE title = @title, group_name = @group_name, keywords = @keywords, semester_approved = @semester, year_level = @year_level, submitted_at = @submitted_at"

                    Using archiveCmd As New MySqlCommand(archiveQuery, conn, transaction)
                        archiveCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                        archiveCmd.Parameters.AddWithValue("@title", proposalTitle)
                        archiveCmd.Parameters.AddWithValue("@group_name", groupName)
                        archiveCmd.Parameters.AddWithValue("@keywords", proposalKeywords)
                        archiveCmd.Parameters.AddWithValue("@semester", proposalSemester)
                        archiveCmd.Parameters.AddWithValue("@year_level", yearLevel)
                        archiveCmd.Parameters.AddWithValue("@submitted_at", submittedAt)
                        archiveCmd.Parameters.AddWithValue("@approved_at", changedAt)
                        archiveCmd.ExecuteNonQuery()
                    End Using
                End If

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

    Public Shared Function DeleteProposal(proposalId As Integer, Optional actorName As String = "Admin") As Boolean
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Dim proposalTitle As String = Nothing
                    Using findCmd As New MySqlCommand(
                        "SELECT title FROM tbl_proposals WHERE proposal_id = @proposal_id FOR UPDATE",
                        conn,
                        transaction)
                        findCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                        Dim titleResult As Object = findCmd.ExecuteScalar()

                        If titleResult IsNot Nothing AndAlso titleResult IsNot DBNull.Value Then
                            proposalTitle = titleResult.ToString()
                        End If
                    End Using

                    If proposalTitle Is Nothing Then
                        transaction.Commit()
                        Return False
                    End If

                    Using archiveCmd As New MySqlCommand(
                        "UPDATE tbl_title_archive SET proposal_id = NULL WHERE proposal_id = @proposal_id",
                        conn,
                        transaction)
                        archiveCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                        archiveCmd.ExecuteNonQuery()
                    End Using

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

                    If deletedRows > 0 Then
                        Dim activityQuery As String = "INSERT INTO tbl_proposal_activity (action_type, proposal_title, actor_name, created_at) " &
                                                      "VALUES (@action_type, @proposal_title, @actor_name, @created_at)"

                        Using activityCmd As New MySqlCommand(activityQuery, conn, transaction)
                            activityCmd.Parameters.AddWithValue("@action_type", "Deleted")
                            activityCmd.Parameters.AddWithValue("@proposal_title", proposalTitle)
                            activityCmd.Parameters.AddWithValue("@actor_name", If(String.IsNullOrWhiteSpace(actorName), "Admin", actorName.Trim()))
                            activityCmd.Parameters.AddWithValue("@created_at", DateTime.Now)
                            activityCmd.ExecuteNonQuery()
                        End Using
                    End If

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

    Public Shared Function GetAllArchivedTitles() As List(Of ArchivedTitle)
        Dim titles As New List(Of ArchivedTitle)()

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim query As String = "SELECT a.archive_id, a.proposal_id, a.title, a.group_name, a.keywords, a.semester_approved, a.approved_at, " &
                                  "COALESCE(a.year_level, p.year_level) AS year_level, COALESCE(a.submitted_at, p.submitted_at) AS submitted_at " &
                                  "FROM tbl_title_archive a " &
                                  "LEFT JOIN tbl_proposals p ON p.proposal_id = a.proposal_id " &
                                  "ORDER BY a.archive_id DESC"

            Using cmd As New MySqlCommand(query, conn)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        titles.Add(New ArchivedTitle With {
                            .ArchiveId = reader.GetInt32("archive_id"),
                            .ProposalId = If(reader.IsDBNull(reader.GetOrdinal("proposal_id")), 0, reader.GetInt32("proposal_id")),
                            .Title = reader.GetString("title"),
                            .GroupName = If(reader.IsDBNull(reader.GetOrdinal("group_name")), "", reader.GetString("group_name")),
                            .YearLevel = If(reader.IsDBNull(reader.GetOrdinal("year_level")), "", reader.GetString("year_level")),
                            .Keywords = reader.GetString("keywords"),
                            .SemesterApproved = reader.GetString("semester_approved"),
                            .ApprovedAt = If(reader.IsDBNull(reader.GetOrdinal("approved_at")), DateTime.MinValue, reader.GetDateTime("approved_at")),
                            .SubmittedAt = If(reader.IsDBNull(reader.GetOrdinal("submitted_at")), DateTime.MinValue, reader.GetDateTime("submitted_at"))
                        })
                    End While
                End Using
            End Using
        End Using

        Return titles
    End Function

    Public Shared Function DeleteArchivedTitles(archiveIds As List(Of Integer)) As Integer
        If archiveIds Is Nothing OrElse archiveIds.Count = 0 Then
            Return 0
        End If

        Dim uniqueIds As New List(Of Integer)
        For Each archiveId As Integer In archiveIds
            If archiveId > 0 AndAlso Not uniqueIds.Contains(archiveId) Then
                uniqueIds.Add(archiveId)
            End If
        Next

        If uniqueIds.Count = 0 Then
            Return 0
        End If

        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Dim parameterNames As New List(Of String)
                    For index As Integer = 0 To uniqueIds.Count - 1
                        parameterNames.Add("@archive_id" & index.ToString())
                    Next

                    Dim query As String = "DELETE FROM tbl_title_archive WHERE archive_id IN (" & String.Join(", ", parameterNames) & ")"
                    Using cmd As New MySqlCommand(query, conn, transaction)
                        For index As Integer = 0 To uniqueIds.Count - 1
                            cmd.Parameters.AddWithValue(parameterNames(index), uniqueIds(index))
                        Next

                        Dim deletedCount As Integer = cmd.ExecuteNonQuery()
                        transaction.Commit()
                        Return deletedCount
                    End Using
                Catch ex As MySqlException
                    transaction.Rollback()
                    Throw New ApplicationException("Failed to delete archived titles. Please try again.", ex)
                Catch
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        End Using
    End Function

    Public Shared Sub UpsertArchiveEntry(proposalId As Integer, title As String, groupName As String, keywords As String, semester As String, yearLevel As String, submittedAt As DateTime)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()

            Dim approvedAt As DateTime = DateTime.Now

            Dim dateQuery As String = "SELECT changed_at FROM tbl_status_log " &
                                      "WHERE proposal_id = @proposal_id AND new_status = 'Approved' " &
                                      "ORDER BY changed_at ASC LIMIT 1"

            Using dateCmd As New MySqlCommand(dateQuery, conn)
                dateCmd.Parameters.AddWithValue("@proposal_id", proposalId)
                Dim result As Object = dateCmd.ExecuteScalar()

                If result IsNot Nothing Then
                    approvedAt = Convert.ToDateTime(result)
                End If
            End Using

            Dim query As String = "INSERT INTO tbl_title_archive (proposal_id, title, group_name, keywords, semester_approved, year_level, submitted_at, approved_at) " &
                                  "VALUES (@proposal_id, @title, @group_name, @keywords, @semester, @year_level, @submitted_at, @approved_at) " &
                                  "ON DUPLICATE KEY UPDATE title = @title, group_name = @group_name, keywords = @keywords, semester_approved = @semester, year_level = @year_level, submitted_at = @submitted_at"

            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@proposal_id", proposalId)
                cmd.Parameters.AddWithValue("@title", title)
                cmd.Parameters.AddWithValue("@group_name", groupName)
                cmd.Parameters.AddWithValue("@keywords", keywords)
                cmd.Parameters.AddWithValue("@semester", semester)
                cmd.Parameters.AddWithValue("@year_level", yearLevel)
                cmd.Parameters.AddWithValue("@submitted_at", submittedAt)
                cmd.Parameters.AddWithValue("@approved_at", approvedAt)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Shared Sub UpdateProposal(proposal As Proposal)
        Using conn As MySqlConnection = DbConnectionHelper.GetConnection()
            conn.Open()
            Using transaction As MySqlTransaction = conn.BeginTransaction()
                Try
                    Using existsCmd As New MySqlCommand("SELECT COUNT(*) FROM tbl_proposals WHERE proposal_id = @proposal_id", conn, transaction)
                        existsCmd.Parameters.AddWithValue("@proposal_id", proposal.ProposalId)
                        If Convert.ToInt32(existsCmd.ExecuteScalar()) = 0 Then
                            Throw New ApplicationException("That proposal no longer exists.")
                        End If
                    End Using

                    Dim updateQuery As String = "UPDATE tbl_proposals SET title = @title, group_name = @group_name, description = @description, course = @course, year_level = @year_level, semester = @semester WHERE proposal_id = @proposal_id"
                    Using updateCmd As New MySqlCommand(updateQuery, conn, transaction)
                        updateCmd.Parameters.AddWithValue("@title", proposal.Title)
                        updateCmd.Parameters.AddWithValue("@group_name", proposal.GroupName)
                        updateCmd.Parameters.AddWithValue("@description", proposal.Description)
                        updateCmd.Parameters.AddWithValue("@course", proposal.Course)
                        updateCmd.Parameters.AddWithValue("@year_level", proposal.YearLevel)
                        updateCmd.Parameters.AddWithValue("@semester", proposal.Semester)
                        updateCmd.Parameters.AddWithValue("@proposal_id", proposal.ProposalId)
                        updateCmd.ExecuteNonQuery()
                    End Using

                    Using deleteMembersCmd As New MySqlCommand("DELETE FROM tbl_proposal_members WHERE proposal_id = @proposal_id", conn, transaction)
                        deleteMembersCmd.Parameters.AddWithValue("@proposal_id", proposal.ProposalId)
                        deleteMembersCmd.ExecuteNonQuery()
                    End Using

                    If proposal.Members IsNot Nothing Then
                        For Each member As ProposalMember In proposal.Members
                            Using memberCmd As New MySqlCommand("INSERT INTO tbl_proposal_members (proposal_id, member_name) VALUES (@proposal_id, @member_name)", conn, transaction)
                                memberCmd.Parameters.AddWithValue("@proposal_id", proposal.ProposalId)
                                memberCmd.Parameters.AddWithValue("@member_name", member.MemberName)
                                memberCmd.ExecuteNonQuery()
                            End Using
                        Next
                    End If

                    If proposal.Status = ProposalStatus.Approved Then
                        Dim keywords As String = DuplicateChecker.GetKeywordString(proposal.Title)
                        Dim archiveQuery As String = "UPDATE tbl_title_archive SET title = @title, group_name = @group_name, keywords = @keywords, semester_approved = @semester, year_level = @year_level, submitted_at = @submitted_at WHERE proposal_id = @proposal_id"
                        Using archiveCmd As New MySqlCommand(archiveQuery, conn, transaction)
                            archiveCmd.Parameters.AddWithValue("@title", proposal.Title)
                            archiveCmd.Parameters.AddWithValue("@group_name", proposal.GroupName)
                            archiveCmd.Parameters.AddWithValue("@keywords", keywords)
                            archiveCmd.Parameters.AddWithValue("@semester", proposal.Semester)
                            archiveCmd.Parameters.AddWithValue("@year_level", proposal.YearLevel)
                            archiveCmd.Parameters.AddWithValue("@submitted_at", proposal.SubmittedAt)
                            archiveCmd.Parameters.AddWithValue("@proposal_id", proposal.ProposalId)
                            archiveCmd.ExecuteNonQuery()
                        End Using
                    End If

                    transaction.Commit()
                Catch ex As ApplicationException
                    transaction.Rollback()
                    Throw
                Catch ex As MySqlException
                    transaction.Rollback()
                    Throw New ApplicationException("Failed to update the proposal. Please try again.", ex)
                End Try
            End Using
        End Using
    End Sub

    Public Shared Function GetProposalById(proposalId As Integer) As Proposal
        For Each proposal As Proposal In GetAllProposals()
            If proposal.ProposalId = proposalId Then
                Return proposal
            End If
        Next

        Return Nothing
    End Function
End Class
