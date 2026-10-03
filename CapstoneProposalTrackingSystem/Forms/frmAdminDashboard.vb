Imports System.Collections.Generic

Public Class frmAdminDashboard

    Private Sub frmAdminDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Session.CurrentUser Is Nothing OrElse Session.CurrentUser.Role <> UserRole.Admin Then
            MessageBox.Show("Access denied.")
            Me.Close()
            Return
        End If

        lblWelcome.Text = "Welcome, " & Session.CurrentUser.FullName
        LoadCounts()
    End Sub

    Private Sub LoadCounts()
        Try
            Dim counts As Dictionary(Of ProposalStatus, Integer) = ProposalService.GetStatusCounts()

            lblPendingCount.Text = counts(ProposalStatus.Pending).ToString()
            lblApprovedCount.Text = counts(ProposalStatus.Approved).ToString()
            lblRevisionCount.Text = counts(ProposalStatus.ForRevision).ToString()
            lblRejectedCount.Text = counts(ProposalStatus.Rejected).ToString()
        Catch ex As Exception
            MessageBox.Show("Could not load the summary counts: " & ex.Message)
        End Try
    End Sub

    Private Sub btnManageProposals_Click(sender As Object, e As EventArgs) Handles btnManageProposals.Click
        Dim manageForm As New frmManageProposals()
        manageForm.ShowDialog()
        LoadCounts()
    End Sub

    Private Sub btnTitleArchive_Click(sender As Object, e As EventArgs) Handles btnTitleArchive.Click
        MessageBox.Show("Title Archive is coming soon.")
    End Sub

    Private Sub btnReports_Click(sender As Object, e As EventArgs) Handles btnReports.Click
        MessageBox.Show("Reports is coming soon.")
    End Sub

    Private Sub btnManageAccounts_Click(sender As Object, e As EventArgs) Handles btnManageAccounts.Click
        MessageBox.Show("Manage Accounts is coming soon.")
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Session.CurrentUser = Nothing
        Dim loginForm As New frmLogin()
        loginForm.Show()
        Me.Close()
    End Sub

End Class