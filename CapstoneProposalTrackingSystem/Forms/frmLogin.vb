Public Class frmLogin
    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FormDisplayHelper.MaximizeFullScreen(Me)
        FormDisplayHelper.CenterCardInContainer(Me, pnlLoginBox)
        FormDisplayHelper.CenterCardInContainer(pnlFormSide, pnlLoginCard)
        FormDisplayHelper.CenterCardInContainer(pnlLogo, pnlLogoContent)
        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
        Using stream As System.IO.Stream = asm.GetManifestResourceStream("CapstoneProposalTrackingSystem.CCS_Logo.png")
            If stream IsNot Nothing Then
                picLogo.Image = Image.FromStream(stream)
            Else
                Dim availableNames As String() = asm.GetManifestResourceNames()
                MessageBox.Show("Logo resource not found. Available embedded resources: " & String.Join(vbCrLf, availableNames))
            End If
        End Using
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        lblStatus.Text = ""

        Dim username As String = txtUsername.Text.Trim()
        Dim password As String = txtPassword.Text

        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            lblStatus.Text = "Please enter both username and password."
            Return
        End If

        Dim user As User
        Try
            user = AuthService.Login(username, password)
        Catch ex As ApplicationException
            lblStatus.Text = ex.Message
            Return
        End Try

        If user Is Nothing Then
            lblStatus.Text = "Invalid username or password."
            Return
        End If

        Session.CurrentUser = user

        Select Case user.Role
            Case UserRole.Student
                Dim studentForm As New frmStudentDashboard()
                studentForm.Show()
            Case UserRole.Admin, UserRole.SuperAdmin
                Dim adminForm As New frmAdminShell()
                adminForm.Show()
        End Select

        Me.Hide()
    End Sub

    Private Sub pnlLoginCard_Paint(sender As Object, e As PaintEventArgs)

    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
        txtPassword.PasswordChar = If(chkShowPassword.Checked, ChrW(0), "*"c)
    End Sub
End Class