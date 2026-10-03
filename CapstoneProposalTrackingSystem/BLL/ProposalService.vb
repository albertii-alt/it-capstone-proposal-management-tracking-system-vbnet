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
        proposal.SubmittedAt = DateTime.Now
        proposal.Status = ProposalStatus.Pending

        Return ProposalDAL.InsertProposal(proposal)
    End Function

    Public Shared Function GetMyProposals(userId As Integer) As List(Of Proposal)
        Return ProposalDAL.GetProposalsBySubmitter(userId)
    End Function

    Public Shared Function GetAllProposals() As List(Of Proposal)
        Return ProposalDAL.GetAllProposals()
    End Function

    Public Shared Function GetLatestLog(proposalId As Integer) As StatusLogEntry
        Return StatusLogDAL.GetLatestLogForProposal(proposalId)
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

        ProposalDAL.UpdateStatusWithLog(proposalId, newStatus, adminId, cleanRemarks)
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
End Class