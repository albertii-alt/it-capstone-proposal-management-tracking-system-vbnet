Public Class frmStudentDashboard

    Private Sub frmStudentDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FormDisplayHelper.MaximizeFullScreen(Me)
        FormDisplayHelper.CenterCardInContainer(Me, pnlDashboardCard)
        lblWelcome.Text = "Welcome, " & Session.CurrentUser.FullName
    End Sub

    Private Sub btnSubmitProposal_Click(sender As Object, e As EventArgs) Handles btnSubmitProposal.Click
        Dim submitForm As New frmSubmitProposal()
        submitForm.ShowDialog()
    End Sub

    Private Sub btnMyProposals_Click(sender As Object, e As EventArgs) Handles btnMyProposals.Click
        Dim myProposalsForm As New frmMyProposals()
        myProposalsForm.ShowDialog()
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Session.CurrentUser = Nothing
        Dim loginForm As New frmLogin()
        loginForm.Show()
        Me.Close()
    End Sub

End Class