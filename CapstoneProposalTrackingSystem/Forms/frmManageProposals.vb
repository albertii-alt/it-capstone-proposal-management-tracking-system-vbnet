Imports System.Collections.Generic

Public Class frmManageProposals

    Private _proposals As New List(Of Proposal)
    Private _isLoading As Boolean = False

    Private Sub frmManageProposals_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Session.CurrentUser Is Nothing OrElse Session.CurrentUser.Role <> UserRole.Admin Then
            MessageBox.Show("Access denied.")
            Me.Close()
            Return
        End If

        cboStatus.Items.AddRange({"Pending", "Approved", "For Revision", "Rejected"})

        SetupGridColumns()
        LoadProposals(0)
    End Sub

    Private Sub SetupGridColumns()
        dgvProposals.AutoGenerateColumns = False
        dgvProposals.Columns.Clear()
        dgvProposals.Columns.Add("colTitle", "Title")
        dgvProposals.Columns.Add("colYearLevel", "Year Level")
        dgvProposals.Columns.Add("colSemester", "Semester")
        dgvProposals.Columns.Add("colStatus", "Status")
        dgvProposals.Columns.Add("colSubmitted", "Submitted")
    End Sub

    Private Sub LoadProposals(selectProposalId As Integer)
        _isLoading = True

        Try
            dgvProposals.Rows.Clear()
            _proposals = ProposalService.GetAllProposals()

            For Each proposal As Proposal In _proposals
                Dim rowIndex As Integer = dgvProposals.Rows.Add(
                    proposal.Title,
                    proposal.YearLevel,
                    proposal.Semester,
                    FormatStatus(proposal.Status),
                    proposal.SubmittedAt.ToString("MM/dd/yyyy"))

                dgvProposals.Rows(rowIndex).Tag = proposal.ProposalId
            Next

        Catch ex As Exception
            MessageBox.Show("Could not load the proposals: " & ex.Message)
        Finally
            _isLoading = False
        End Try

        If dgvProposals.Rows.Count = 0 Then
            txtDetails.Text = "There are no proposals yet."
            txtRemarks.Clear()
            cboStatus.SelectedIndex = -1
            Return
        End If

        Dim rowToSelect As Integer = 0

        For i As Integer = 0 To dgvProposals.Rows.Count - 1
            If CInt(dgvProposals.Rows(i).Tag) = selectProposalId Then
                rowToSelect = i
                Exit For
            End If
        Next

        dgvProposals.ClearSelection()
        dgvProposals.Rows(rowToSelect).Selected = True
        ShowSelectedProposal()
    End Sub

    Private Sub dgvProposals_SelectionChanged(sender As Object, e As EventArgs) Handles dgvProposals.SelectionChanged
        If _isLoading Then
            Return
        End If

        ShowSelectedProposal()
    End Sub

    Private Function GetSelectedProposal() As Proposal
        If dgvProposals.SelectedRows.Count = 0 Then
            Return Nothing
        End If

        Dim proposalId As Integer = CInt(dgvProposals.SelectedRows(0).Tag)

        For Each proposal As Proposal In _proposals
            If proposal.ProposalId = proposalId Then
                Return proposal
            End If
        Next

        Return Nothing
    End Function

    Private Sub ShowSelectedProposal()
        Dim proposal As Proposal = GetSelectedProposal()

        If proposal Is Nothing Then
            Return
        End If

        Dim memberNames As New List(Of String)
        For Each member As ProposalMember In proposal.Members
            memberNames.Add(member.MemberName)
        Next

        Dim detailsText As String = proposal.Description & vbCrLf & vbCrLf &
                                    "Group members: " & String.Join(", ", memberNames)

        Try
            Dim similarities As List(Of TitleSimilarity) = ProposalService.GetSimilarityBreakdown(proposal.ProposalId, proposal.Title)
            Dim matches As New List(Of TitleSimilarity)

            For Each item As TitleSimilarity In similarities
                If item.Score > 0 Then
                    matches.Add(item)
                End If

                If matches.Count = 5 Then
                    Exit For
                End If
            Next

            If matches.Count = 0 Then
                detailsText &= vbCrLf & vbCrLf & "Duplicate check: no similar titles found."
            Else
                detailsText &= vbCrLf & vbCrLf & "DUPLICATE CHECK (closest matches):"

                For Each item As TitleSimilarity In matches
                    Dim percent As String = (item.Score * 100).ToString("0") & "%"
                    detailsText &= vbCrLf & " - " & percent & " similar to """ & item.Title & """"
                Next
            End If

        Catch ex As Exception
            detailsText &= vbCrLf & vbCrLf & "Duplicate check unavailable."
        End Try

        txtDetails.Text = detailsText

        cboStatus.Text = FormatStatus(proposal.Status)
        txtRemarks.Clear()
    End Sub

    Private Sub btnSaveDecision_Click(sender As Object, e As EventArgs) Handles btnSaveDecision.Click
        Dim proposal As Proposal = GetSelectedProposal()

        If proposal Is Nothing Then
            MessageBox.Show("Select a proposal first.")
            Return
        End If

        If cboStatus.SelectedIndex = -1 Then
            MessageBox.Show("Choose a status first.")
            Return
        End If

        Dim newStatus As ProposalStatus = ToStatus(cboStatus.Text)

        Dim confirm As DialogResult = MessageBox.Show(
            "Set """ & proposal.Title & """ to " & cboStatus.Text & "?",
            "Confirm Decision",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If confirm = DialogResult.No Then
            Return
        End If

        Try
            ProposalService.ReviewProposal(proposal.ProposalId, newStatus, Session.CurrentUser.UserId, txtRemarks.Text)
            MessageBox.Show("Decision saved.")
            LoadProposals(proposal.ProposalId)

        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
        Catch ex As Exception
            MessageBox.Show("Something went wrong while saving. Please try again.")
        End Try
    End Sub

    Private Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then
            Return "For Revision"
        End If

        Return status.ToString()
    End Function

    Private Function ToStatus(text As String) As ProposalStatus
        If text = "For Revision" Then
            Return ProposalStatus.ForRevision
        End If

        Return CType([Enum].Parse(GetType(ProposalStatus), text), ProposalStatus)
    End Function

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Dim current As Proposal = GetSelectedProposal()
        LoadProposals(If(current IsNot Nothing, current.ProposalId, 0))
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

End Class