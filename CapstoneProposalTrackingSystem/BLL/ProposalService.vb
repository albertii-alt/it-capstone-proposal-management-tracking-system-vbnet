Imports System.Collections.Generic

Public Class PendingDuplicateRisk
    Public Property ProposalId As Integer
    Public Property Title As String
    Public Property GroupName As String
    Public Property Score As Double
    Public Property MatchedTitle As String
End Class

Public Class ProposalService

    Public Shared Function CheckDuplicate(title As String) As DuplicateCheckResult
        Dim existingTitles As List(Of String) = ProposalDAL.GetAllTitlesForDuplicateCheck()
        Return DuplicateChecker.CheckForDuplicates(title, existingTitles)
    End Function

    Public Shared Function SubmitProposal(proposal As Proposal) As Integer
        Return SubmitProposal(proposal, Nothing)
    End Function

    Public Shared Function SubmitProposal(proposal As Proposal, submittingUser As User) As Integer
        proposal.SubmittedAt = DateTime.Now
        proposal.Status = ProposalStatus.Pending

        Dim adminActorName As String = Nothing
        If submittingUser IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(submittingUser.Role) Then
            adminActorName = submittingUser.FullName
        End If

        Dim proposalId As Integer = ProposalDAL.InsertProposal(proposal, adminActorName)
        If submittingUser IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(submittingUser.Role) Then
            AdminActivityLogService.Record(submittingUser, "Created", "Proposal", proposalId, proposal.Title,
                                           "Created the proposal """ & proposal.Title & """.")
        End If
        Return proposalId
    End Function

    Public Shared Sub UpdateProposal(proposal As Proposal)
        UpdateProposal(proposal, Nothing)
    End Sub

    Public Shared Sub UpdateProposal(proposal As Proposal, actor As User)
        If proposal Is Nothing OrElse proposal.ProposalId <= 0 Then
            Throw New ArgumentException("A valid proposal is required.", NameOf(proposal))
        End If

        Dim previousProposal As Proposal = ProposalDAL.GetProposalById(proposal.ProposalId)
        ProposalDAL.UpdateProposal(proposal)

        If actor IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(actor.Role) AndAlso previousProposal IsNot Nothing Then
            Dim changedFields As New List(Of String)
            If Not String.Equals(previousProposal.Title, proposal.Title, StringComparison.Ordinal) Then changedFields.Add("title")
            If Not String.Equals(previousProposal.GroupName, proposal.GroupName, StringComparison.Ordinal) Then changedFields.Add("group")
            If Not String.Equals(previousProposal.Description, proposal.Description, StringComparison.Ordinal) Then changedFields.Add("description")
            If Not String.Equals(previousProposal.YearLevel, proposal.YearLevel, StringComparison.Ordinal) Then changedFields.Add("year level")
            If Not String.Equals(previousProposal.Semester, proposal.Semester, StringComparison.Ordinal) Then changedFields.Add("semester")
            If Not ProposalMembersMatch(previousProposal.Members, proposal.Members) Then changedFields.Add("members")

            Dim description As String = "Updated the proposal """ & proposal.Title & """."
            If changedFields.Count > 0 Then
                description &= " Changed: " & String.Join(", ", changedFields) & "."
            End If
            AdminActivityLogService.Record(actor, "Updated", "Proposal", proposal.ProposalId, proposal.Title,
                                           description,
                                           If(changedFields.Count = 0, Nothing, "Changed fields: " & String.Join(", ", changedFields)))
        End If
    End Sub

    Private Shared Function ProposalMembersMatch(leftMembers As List(Of ProposalMember), rightMembers As List(Of ProposalMember)) As Boolean
        If leftMembers Is Nothing OrElse rightMembers Is Nothing Then
            Return leftMembers Is rightMembers
        End If
        If leftMembers.Count <> rightMembers.Count Then Return False
        For index As Integer = 0 To leftMembers.Count - 1
            If Not String.Equals(leftMembers(index).MemberName, rightMembers(index).MemberName, StringComparison.Ordinal) Then
                Return False
            End If
        Next
        Return True
    End Function

    Public Shared Function GetMyProposals(userId As Integer) As List(Of Proposal)
        Return ProposalDAL.GetProposalsBySubmitter(userId)
    End Function

    Public Shared Function GetAllProposals() As List(Of Proposal)
        Return ProposalDAL.GetAllProposals()
    End Function

    Public Shared Function GetAllArchivedTitles() As List(Of ArchivedTitle)
        Return ProposalDAL.GetAllArchivedTitles()
    End Function

    Public Shared Function DeleteArchivedTitles(archiveIds As List(Of Integer), Optional actor As User = Nothing) As Integer
        If archiveIds Is Nothing Then
            Throw New ArgumentNullException(NameOf(archiveIds))
        End If

        If archiveIds.Exists(Function(archiveId) archiveId <= 0) Then
            Throw New ArgumentOutOfRangeException(NameOf(archiveIds), "Archive IDs must be positive.")
        End If

        Dim deletedCount As Integer = ProposalDAL.DeleteArchivedTitles(archiveIds)
        If deletedCount > 0 AndAlso actor IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(actor.Role) Then
            AdminActivityLogService.Record(actor, "Deleted", "Title Archive", Nothing,
                                           deletedCount.ToString() & " archived title(s)",
                                           "Deleted " & deletedCount.ToString() & " archived title(s).",
                                           "Archive IDs: " & String.Join(", ", archiveIds))
            SecurityLogService.Record(actor.UserId, actor.Username, "Bulk Deletion",
                                      "Deleted " & deletedCount.ToString() & " archived title(s).")
        End If
        Return deletedCount
    End Function

    Public Shared Function DeleteProposal(proposalId As Integer) As Boolean
        Return DeleteProposal(proposalId, "Admin")
    End Function

    Public Shared Function DeleteProposal(proposalId As Integer, actorName As String) As Boolean
        If proposalId <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(proposalId))
        End If

        Return ProposalDAL.DeleteProposal(proposalId, actorName)
    End Function

    Public Shared Function DeleteProposal(proposalId As Integer, actor As User) As Boolean
        If actor Is Nothing Then
            Throw New ArgumentNullException(NameOf(actor))
        End If

        Dim proposal As Proposal = ProposalDAL.GetProposalById(proposalId)
        If proposal Is Nothing Then
            Return False
        End If

        Dim deleted As Boolean = ProposalDAL.DeleteProposal(proposalId, actor.FullName)
        If deleted Then
            AdminActivityLogService.Record(actor, "Deleted", "Proposal", proposalId, proposal.Title,
                                           "Deleted the proposal """ & proposal.Title & """.")
            SecurityLogService.Record(actor.UserId, actor.Username, "Sensitive Deletion",
                                      "Deleted the proposal """ & proposal.Title & """.")
        End If
        Return deleted
    End Function

    Public Shared Function GetLatestLog(proposalId As Integer) As StatusLogEntry
        Return StatusLogDAL.GetLatestLogForProposal(proposalId)
    End Function

    Public Shared Function GetStatusLogs(proposalId As Integer) As List(Of StatusLogEntry)
        Return StatusLogDAL.GetLogsForProposal(proposalId)
    End Function

    Public Shared Function GetStatusCounts() As Dictionary(Of ProposalStatus, Integer)
        Dim counts As New Dictionary(Of ProposalStatus, Integer) From {
            {ProposalStatus.Pending, 0},
            {ProposalStatus.Approved, 0},
            {ProposalStatus.Rejected, 0},
            {ProposalStatus.ForRevision, 0}
        }

        For Each proposal As Proposal In ProposalDAL.GetAllProposals()
            counts(proposal.Status) += 1
        Next

        Return counts
    End Function

    Public Shared Sub ReviewProposal(proposalId As Integer, newStatus As ProposalStatus, adminId As Integer, remarks As String)
        Dim cleanRemarks As String = If(remarks, "").Trim()

        If (newStatus = ProposalStatus.Rejected OrElse newStatus = ProposalStatus.ForRevision) AndAlso cleanRemarks.Length = 0 Then
            Throw New ApplicationException("Remarks are required when rejecting a proposal or requesting a revision.")
        End If

        Dim targetProposal As Proposal = Nothing
        For Each proposal As Proposal In ProposalDAL.GetAllProposals()
            If proposal.ProposalId = proposalId Then
                targetProposal = proposal
                Exit For
            End If
        Next

        If targetProposal Is Nothing Then
            Throw New ApplicationException("That proposal no longer exists.")
        End If

        Dim keywords As String = DuplicateChecker.GetKeywordString(targetProposal.Title)

        ProposalDAL.UpdateStatusWithLog(proposalId, newStatus, adminId, cleanRemarks, targetProposal.Title, keywords, targetProposal.Semester, targetProposal.GroupName, targetProposal.YearLevel, targetProposal.SubmittedAt)
        Dim actor As User = UserDAL.GetUserById(adminId)
        Dim actionType As String = If(newStatus = ProposalStatus.Approved, "Approved",
                                      If(newStatus = ProposalStatus.Rejected, "Rejected", "Requested Revision"))
        AdminActivityLogService.Record(actor, actionType, "Proposal", proposalId, targetProposal.Title,
                                       "Changed the proposal """ & targetProposal.Title & """ to " & newStatus.ToString() & ".",
                                       If(String.IsNullOrWhiteSpace(cleanRemarks), Nothing, "Remarks: " & cleanRemarks))
    End Sub

    Public Shared Function CheckDuplicateForExistingProposal(proposalId As Integer, title As String) As DuplicateCheckResult
        Dim otherTitles As List(Of String) = ProposalDAL.GetAllTitlesExcludingProposal(proposalId)
        Return DuplicateChecker.CheckForDuplicates(title, otherTitles)
    End Function

    Public Shared Function GetSimilarityBreakdown(proposalId As Integer, title As String) As List(Of TitleSimilarity)
        Dim others As List(Of ProposalTitleInfo) = ProposalDAL.GetAllTitleInfoExcludingProposal(proposalId)
        Return DuplicateChecker.GetAllSimilarities(title, others)
    End Function

    Public Shared Function GetPendingDuplicateRisks() As List(Of PendingDuplicateRisk)
        Dim risks As New List(Of PendingDuplicateRisk)
        Dim allProposals As List(Of Proposal) = ProposalDAL.GetAllProposals()

        For Each proposal As Proposal In allProposals
            If proposal.Status <> ProposalStatus.Pending Then
                Continue For
            End If

            Dim otherTitles As List(Of String) = ProposalDAL.GetAllTitlesExcludingProposal(proposal.ProposalId)
            Dim result As DuplicateCheckResult = DuplicateChecker.CheckForDuplicates(proposal.Title, otherTitles)

            If result.IsWarning Then
                risks.Add(New PendingDuplicateRisk With {
                    .ProposalId = proposal.ProposalId,
                    .Title = proposal.Title,
                    .GroupName = proposal.GroupName,
                    .Score = result.HighestSimilarity,
                    .MatchedTitle = result.MatchedTitle
                })
            End If
        Next

        risks.Sort(Function(a, b) b.Score.CompareTo(a.Score))
        Return risks
    End Function

    Public Shared Function GetRecentActivity(maxRows As Integer) As List(Of RecentActivityEntry)
        Return StatusLogDAL.GetRecentActivity(maxRows)
    End Function

    Public Shared Function ResyncArchive(Optional actor As User = Nothing) As Integer
        Dim allProposals As List(Of Proposal) = ProposalDAL.GetAllProposals()
        Dim resyncedCount As Integer = 0

        For Each proposal As Proposal In allProposals
            If proposal.Status = ProposalStatus.Approved Then
                Dim keywords As String = DuplicateChecker.GetKeywordString(proposal.Title)
                ProposalDAL.UpsertArchiveEntry(proposal.ProposalId, proposal.Title, proposal.GroupName, keywords, proposal.Semester, proposal.YearLevel, proposal.SubmittedAt)
                resyncedCount += 1
            End If
        Next

        If resyncedCount > 0 AndAlso actor IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(actor.Role) Then
            AdminActivityLogService.Record(actor, "Updated", "Title Archive", Nothing,
                                           "Title archive",
                                           "Resynchronized " & resyncedCount.ToString() & " approved title(s) in the title archive.")
        End If
        Return resyncedCount
    End Function

    Public Shared Function GetProposalById(proposalId As Integer) As Proposal
        Return ProposalDAL.GetProposalById(proposalId)
    End Function
End Class