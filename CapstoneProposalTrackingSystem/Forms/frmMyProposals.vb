Imports System.Collections.Generic

Public Class frmMyProposals

    Private Sub frmMyProposals_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGridColumns()
        LoadProposals()
    End Sub

    Private Sub SetupGridColumns()
        dgvMyProposals.AutoGenerateColumns = False
        dgvMyProposals.Columns.Clear()
        dgvMyProposals.Columns.Add("colTitle", "Title")
        dgvMyProposals.Columns.Add("colSemester", "Semester")
        dgvMyProposals.Columns.Add("colStatus", "Status")
        dgvMyProposals.Columns.Add("colSubmitted", "Submitted")
    End Sub

    Private Sub LoadProposals()
        Try
            dgvMyProposals.Rows.Clear()
            txtRemarks.Clear()

            Dim proposals As List(Of Proposal) = ProposalService.GetMyProposals(Session.CurrentUser.UserId)

            For Each proposal As Proposal In proposals
                Dim rowIndex As Integer = dgvMyProposals.Rows.Add(
                    proposal.Title,
                    proposal.Semester,
                    FormatStatus(proposal.Status),
                    proposal.SubmittedAt.ToString("MM/dd/yyyy"))

                dgvMyProposals.Rows(rowIndex).Tag = proposal.ProposalId
            Next

            If dgvMyProposals.Rows.Count > 0 Then
                dgvMyProposals.Rows(0).Selected = True
                ShowRemarksForSelectedRow()
            Else
                txtRemarks.Text = "You have not submitted any proposals yet."
            End If

        Catch ex As Exception
            MessageBox.Show("Could not load your proposals: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvMyProposals_SelectionChanged(sender As Object, e As EventArgs) Handles dgvMyProposals.SelectionChanged
        ShowRemarksForSelectedRow()
    End Sub

    Private Sub ShowRemarksForSelectedRow()
        If dgvMyProposals.SelectedRows.Count = 0 Then
            Return
        End If

        Dim proposalId As Integer = CInt(dgvMyProposals.SelectedRows(0).Tag)

        Try
            Dim log As StatusLogEntry = ProposalService.GetLatestLog(proposalId)

            If log Is Nothing Then
                txtRemarks.Text = "No remarks yet."
            ElseIf String.IsNullOrWhiteSpace(log.Remarks) Then
                txtRemarks.Text = "The admin did not leave any remarks."
            Else
                txtRemarks.Text = log.Remarks
            End If

        Catch ex As Exception
            MessageBox.Show("Could not load remarks: " & ex.Message)
        End Try
    End Sub

    Private Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then
            Return "For Revision"
        End If

        Return status.ToString()
    End Function

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadProposals()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

End Class