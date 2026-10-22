Public Class frmLogin
    Private ReadOnly btnRegister As New Button()

    Public Sub New()
        InitializeComponent()
        btnRegister.Text = "Register as Student"
        btnRegister.FlatStyle = FlatStyle.Flat
        btnRegister.FlatAppearance.BorderSize = 0
        btnRegister.BackColor = Color.Transparent
        btnRegister.ForeColor = Color.FromArgb(122, 31, 46)
        btnRegister.Font = New Font("Segoe UI", 8.5F, FontStyle.Underline)
        btnRegister.Size = New Size(150, 28)
        btnRegister.Cursor = Cursors.Hand
        btnRegister.TabStop = False
        AddHandler btnRegister.Click, AddressOf btnRegister_Click
        pnlLoginCard.Controls.Add(btnRegister)

        ConfigureFullScreenLayout()
        AddHandler pnlFormSide.Resize, AddressOf PositionLoginForm
        AddHandler pnlLogo.Resize, AddressOf PositionBrandContent
        AddHandler pnlLogoContent.Resize, AddressOf PositionBrandContent
        PositionLoginForm(Me, EventArgs.Empty)
        PositionBrandContent(Me, EventArgs.Empty)
    End Sub

    Private Sub frmLogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FormDisplayHelper.MaximizeFullScreen(Me)
        PositionLoginForm(Me, EventArgs.Empty)
        PositionBrandContent(Me, EventArgs.Empty)
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

    Private Sub ConfigureFullScreenLayout()
        pnlLoginBox.Dock = DockStyle.Fill
        pnlLoginBox.Location = Point.Empty
        pnlLoginBox.Size = Me.ClientSize
        pnlLoginBox.BorderStyle = BorderStyle.None
        pnlLoginBox.Padding = New Padding(0)

        pnlLogo.Dock = DockStyle.Left
        pnlLogo.Width = 420
        pnlLogo.BorderStyle = BorderStyle.None
        pnlFormSide.Dock = DockStyle.Fill
        pnlFormSide.Padding = New Padding(32)
        pnlLoginCard.BorderStyle = BorderStyle.None
        pnlLoginCard.BackColor = SystemColors.Window
        pnlLoginCard.Size = New Size(520, 520)

        lblSystemTitle.Text = "IT Capstone Proposal"
        lblSystemTitle.Font = New Font("Segoe UI", 20.0F, FontStyle.Bold)
        lblSystemTitle.ForeColor = Color.FromArgb(246, 201, 69)
        lblSystemTitle.TextAlign = ContentAlignment.MiddleCenter
        lblSystemSubtitle.Text = "Management and Tracking System"
        lblSystemSubtitle.Font = New Font("Segoe UI", 11.0F)
        lblSystemSubtitle.ForeColor = Color.FromArgb(240, 217, 160)
        lblSystemSubtitle.TextAlign = ContentAlignment.MiddleCenter
        pnlLogo.Controls.Add(lblSystemTitle)
        pnlLogo.Controls.Add(lblSystemSubtitle)

        pnlLogoContent.Dock = DockStyle.None
        lblTitle.Font = New Font("Segoe UI", 28.0F, FontStyle.Bold)
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        lblSubtext.Font = New Font("Segoe UI", 11.0F)
        lblUsername.Font = New Font("Segoe UI", 10.5F, FontStyle.Bold)
        lblPassword.Font = New Font("Segoe UI", 10.5F, FontStyle.Bold)
        txtUsername.Font = New Font("Segoe UI", 13.0F)
        txtPassword.Font = New Font("Segoe UI", 13.0F)
        chkShowPassword.Font = New Font("Segoe UI", 10.0F)
        btnLogin.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        btnLogin.Cursor = Cursors.Hand
        btnRegister.Font = New Font("Segoe UI", 10.0F, FontStyle.Underline)
        pnlLoginCard.Controls.SetChildIndex(btnRegister, 0)
    End Sub

    Private Sub PositionLoginForm(sender As Object, e As EventArgs)
        Dim availableWidth As Integer = Math.Max(0, pnlFormSide.ClientSize.Width - pnlFormSide.Padding.Horizontal)
        Dim cardWidth As Integer = Math.Min(560, availableWidth)
        Dim cardHeight As Integer = 520
        Dim topBoundary As Integer = pnlFormSide.Padding.Top
        Dim availableHeight As Integer = Math.Max(0, pnlFormSide.ClientSize.Height - pnlFormSide.Padding.Bottom - topBoundary)
        Dim cardTop As Integer = topBoundary + Math.Max(0, (availableHeight - cardHeight) \ 2)
        Dim maxCardTop As Integer = Math.Max(pnlFormSide.Padding.Top, pnlFormSide.ClientSize.Height - pnlFormSide.Padding.Bottom - cardHeight)
        cardTop = Math.Min(cardTop, maxCardTop)
        pnlLoginCard.Size = New Size(cardWidth, cardHeight)
        pnlLoginCard.Location = New Point(
            pnlFormSide.Padding.Left + Math.Max(0, (availableWidth - cardWidth) \ 2),
            cardTop)

        Dim contentWidth As Integer = Math.Max(0, cardWidth - 24)
        Dim horizontalInset As Integer = 12
        lblTitle.SetBounds(horizontalInset, 8, contentWidth, 76)
        lblSubtext.SetBounds(horizontalInset, 88, contentWidth, 48)
        lblUsername.SetBounds(horizontalInset, 150, contentWidth, 24)
        txtUsername.SetBounds(horizontalInset, 178, contentWidth, 44)
        lblPassword.SetBounds(horizontalInset, 242, contentWidth, 24)
        txtPassword.SetBounds(horizontalInset, 270, contentWidth, 44)
        chkShowPassword.SetBounds(horizontalInset, 324, contentWidth, 28)
        btnLogin.SetBounds(horizontalInset, 374, contentWidth, 52)
        btnRegister.SetBounds(horizontalInset, 436, contentWidth, 32)
        lblStatus.SetBounds(horizontalInset, 474, contentWidth, 32)
    End Sub

    Private Sub PositionBrandContent(sender As Object, e As EventArgs)
        Dim brandWidth As Integer = pnlLogo.ClientSize.Width
        Dim brandHeight As Integer = pnlLogo.ClientSize.Height
        lblSystemTitle.SetBounds(24, Math.Max(24, brandHeight \ 10), Math.Max(0, brandWidth - 48), 58)
        lblSystemSubtitle.SetBounds(24, lblSystemTitle.Bottom, Math.Max(0, brandWidth - 48), 34)
        lblSystemTitle.BringToFront()
        lblSystemSubtitle.BringToFront()

        Dim contentHeight As Integer = 330
        pnlLogoContent.Size = New Size(brandWidth, contentHeight)
        pnlLogoContent.Location = New Point(0, Math.Max(0, (brandHeight - contentHeight) \ 2) + 24)
        picLogo.Size = New Size(190, 190)
        picLogo.Location = New Point(Math.Max(0, (brandWidth - picLogo.Width) \ 2), 0)
        lblWelcomeTitle.SetBounds(12, 202, Math.Max(0, brandWidth - 24), 44)
        lblWelcomeSubtitle.SetBounds(24, 250, Math.Max(0, brandWidth - 48), 68)
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

    Private Sub btnRegister_Click(sender As Object, e As EventArgs)
        Using registrationForm As New frmStudentRegistration()
            registrationForm.ShowDialog(Me)
        End Using
    End Sub
End Class