<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLogin
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        pnlLoginBox = New Panel()
        lblSystemTitle = New Label()
        lblSystemSubtitle = New Label()
        pnlLogo = New Panel()
        pnlLogoContent = New Panel()
        lblWelcomeSubtitle = New Label()
        lblWelcomeTitle = New Label()
        picLogo = New PictureBox()
        pnlFormSide = New Panel()
        pnlLoginCard = New Panel()
        lblStatus = New Label()
        lblSubtext = New Label()
        btnLogin = New Button()
        txtPassword = New TextBox()
        lblPassword = New Label()
        chkShowPassword = New CheckBox()
        txtUsername = New TextBox()
        lblUsername = New Label()
        lblTitle = New Label()
        pnlLoginBox.SuspendLayout()
        pnlLogo.SuspendLayout()
        pnlLogoContent.SuspendLayout()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        pnlFormSide.SuspendLayout()
        pnlLoginCard.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlLoginBox
        ' 
        pnlLoginBox.BackColor = SystemColors.Window
        pnlLoginBox.BorderStyle = BorderStyle.FixedSingle
        pnlLoginBox.Controls.Add(pnlFormSide)
        pnlLoginBox.Controls.Add(pnlLogo)
        pnlLoginBox.Location = New Point(0, 0)
        pnlLoginBox.Name = "pnlLoginBox"
        pnlLoginBox.Size = New Size(760, 480)
        pnlLoginBox.TabIndex = 0
        ' 
        ' lblSystemTitle
        ' 
        lblSystemTitle.AutoSize = False
        lblSystemTitle.Font = New Font("Segoe UI", 28.0F, FontStyle.Bold)
        lblSystemTitle.ForeColor = Color.FromArgb(CByte(122), CByte(31), CByte(46))
        lblSystemTitle.Location = New Point(40, 40)
        lblSystemTitle.Name = "lblSystemTitle"
        lblSystemTitle.Size = New Size(1100, 68)
        lblSystemTitle.TabIndex = 1
        lblSystemTitle.Text = "IT Capstone Proposal Management and Tracking System"
        lblSystemTitle.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' lblSystemSubtitle
        ' 
        lblSystemSubtitle.AutoSize = False
        lblSystemSubtitle.Font = New Font("Segoe UI", 12.0F, FontStyle.Regular)
        lblSystemSubtitle.ForeColor = Color.DimGray
        lblSystemSubtitle.Location = New Point(42, 118)
        lblSystemSubtitle.Name = "lblSystemSubtitle"
        lblSystemSubtitle.Size = New Size(720, 32)
        lblSystemSubtitle.TabIndex = 2
        lblSystemSubtitle.Text = "Submit, review, and track capstone proposals in one place."
        lblSystemSubtitle.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' pnlLogo
        ' 
        pnlLogo.BackColor = Color.FromArgb(CByte(122), CByte(31), CByte(46))
        pnlLogo.Controls.Add(pnlLogoContent)
        pnlLogo.Dock = DockStyle.Left
        pnlLogo.Location = New Point(0, 0)
        pnlLogo.Name = "pnlLogo"
        pnlLogo.Size = New Size(280, 380)
        pnlLogo.TabIndex = 0
        ' 
        ' pnlLogoContent
        ' 
        pnlLogoContent.BackColor = Color.FromArgb(CByte(122), CByte(31), CByte(46))
        pnlLogoContent.Controls.Add(lblWelcomeSubtitle)
        pnlLogoContent.Controls.Add(lblWelcomeTitle)
        pnlLogoContent.Controls.Add(picLogo)
        pnlLogoContent.Location = New Point(0, 0)
        pnlLogoContent.Name = "pnlLogoContent"
        pnlLogoContent.Size = New Size(280, 320)
        pnlLogoContent.TabIndex = 0
        ' 
        ' lblWelcomeSubtitle
        ' 
        lblWelcomeSubtitle.Font = New Font("Segoe UI", 9.5F)
        lblWelcomeSubtitle.ForeColor = Color.FromArgb(CByte(240), CByte(217), CByte(160))
        lblWelcomeSubtitle.Location = New Point(0, 236)
        lblWelcomeSubtitle.MaximumSize = New Size(280, 60)
        lblWelcomeSubtitle.Name = "lblWelcomeSubtitle"
        lblWelcomeSubtitle.Size = New Size(280, 60)
        lblWelcomeSubtitle.TabIndex = 2
        lblWelcomeSubtitle.Text = "Please sign in to continue to the Capstone Proposal Tracking System"
        lblWelcomeSubtitle.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblWelcomeTitle
        ' 
        lblWelcomeTitle.Font = New Font("Segoe UI", 18.0F, FontStyle.Bold)
        lblWelcomeTitle.ForeColor = Color.FromArgb(CByte(246), CByte(201), CByte(69))
        lblWelcomeTitle.Location = New Point(0, 192)
        lblWelcomeTitle.Name = "lblWelcomeTitle"
        lblWelcomeTitle.Size = New Size(280, 40)
        lblWelcomeTitle.TabIndex = 1
        lblWelcomeTitle.Text = "Welcome Back!"
        lblWelcomeTitle.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' picLogo
        ' 
        picLogo.Location = New Point(60, 20)
        picLogo.Name = "picLogo"
        picLogo.Size = New Size(160, 160)
        picLogo.SizeMode = PictureBoxSizeMode.Zoom
        picLogo.TabIndex = 0
        picLogo.TabStop = False
        ' 
        ' pnlFormSide
        ' 
        pnlFormSide.BackColor = SystemColors.Window
        pnlFormSide.Controls.Add(pnlLoginCard)
        pnlFormSide.Dock = DockStyle.Fill
        pnlFormSide.Location = New Point(280, 0)
        pnlFormSide.Name = "pnlFormSide"
        pnlFormSide.Size = New Size(420, 380)
        pnlFormSide.TabIndex = 1
        ' 
        ' pnlLoginCard
        ' 
        pnlLoginCard.BackColor = SystemColors.Window
        pnlLoginCard.BorderStyle = BorderStyle.None
        pnlLoginCard.Controls.Add(lblStatus)
        pnlLoginCard.Controls.Add(btnLogin)
        pnlLoginCard.Controls.Add(txtPassword)
        pnlLoginCard.Controls.Add(lblPassword)
        pnlLoginCard.Controls.Add(chkShowPassword)
        pnlLoginCard.Controls.Add(txtUsername)
        pnlLoginCard.Controls.Add(lblUsername)
        pnlLoginCard.Controls.Add(lblSubtext)
        pnlLoginCard.Controls.Add(lblTitle)
        pnlLoginCard.Location = New Point(0, 0)
        pnlLoginCard.Name = "pnlLoginCard"
        pnlLoginCard.Size = New Size(420, 420)
        pnlLoginCard.TabIndex = 0
        ' 
        ' lblStatus
        ' 
        lblStatus.AutoSize = False
        lblStatus.ForeColor = Color.Red
        lblStatus.Location = New Point(30, 400)
        lblStatus.Margin = New Padding(4, 0, 4, 0)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New Size(360, 20)
        lblStatus.TabIndex = 6
        lblStatus.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' lblSubtext
        ' 
        lblSubtext.AutoSize = False
        lblSubtext.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
        lblSubtext.ForeColor = Color.DimGray
        lblSubtext.Location = New Point(30, 92)
        lblSubtext.Name = "lblSubtext"
        lblSubtext.Size = New Size(360, 44)
        lblSubtext.TabIndex = 1
        lblSubtext.Text = "Enter your credentials to access the proposal system."
        ' 
        ' btnLogin
        ' 
        btnLogin.BackColor = Color.FromArgb(CByte(246), CByte(201), CByte(69))
        btnLogin.FlatAppearance.BorderSize = 0
        btnLogin.FlatStyle = FlatStyle.Flat
        btnLogin.Font = New Font("Segoe UI", 10.5F, FontStyle.Bold)
        btnLogin.ForeColor = Color.FromArgb(CByte(122), CByte(31), CByte(46))
        btnLogin.Location = New Point(30, 326)
        btnLogin.Margin = New Padding(4, 5, 4, 5)
        btnLogin.Name = "btnLogin"
        btnLogin.Size = New Size(360, 44)
        btnLogin.TabIndex = 6
        btnLogin.Text = "Login  →"
        btnLogin.UseVisualStyleBackColor = False
        ' 
        ' txtPassword
        ' 
        txtPassword.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
        txtPassword.Location = New Point(30, 252)
        txtPassword.Margin = New Padding(4, 5, 4, 5)
        txtPassword.Name = "txtPassword"
        txtPassword.PasswordChar = "*"c
        txtPassword.Size = New Size(360, 32)
        txtPassword.TabIndex = 4
        ' 
        ' lblPassword
        ' 
        lblPassword.AutoSize = True
        lblPassword.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblPassword.ForeColor = Color.FromArgb(CByte(60), CByte(60), CByte(60))
        lblPassword.Location = New Point(30, 222)
        lblPassword.Margin = New Padding(4, 0, 4, 0)
        lblPassword.Name = "lblPassword"
        lblPassword.Size = New Size(70, 20)
        lblPassword.TabIndex = 3
        lblPassword.Text = "Password"
        ' 
        ' chkShowPassword
        ' 
        chkShowPassword.AutoSize = True
        chkShowPassword.ForeColor = Color.DimGray
        chkShowPassword.Location = New Point(30, 292)
        chkShowPassword.Name = "chkShowPassword"
        chkShowPassword.Size = New Size(108, 24)
        chkShowPassword.TabIndex = 5
        chkShowPassword.Text = "Show password"
        chkShowPassword.UseVisualStyleBackColor = True
        ' 
        ' txtUsername
        ' 
        txtUsername.Font = New Font("Segoe UI", 10.5F, FontStyle.Regular)
        txtUsername.Location = New Point(30, 166)
        txtUsername.Margin = New Padding(4, 5, 4, 5)
        txtUsername.Name = "txtUsername"
        txtUsername.Size = New Size(360, 32)
        txtUsername.TabIndex = 2
        ' 
        ' lblUsername
        ' 
        lblUsername.AutoSize = True
        lblUsername.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblUsername.ForeColor = Color.FromArgb(CByte(60), CByte(60), CByte(60))
        lblUsername.Location = New Point(30, 136)
        lblUsername.Margin = New Padding(4, 0, 4, 0)
        lblUsername.Name = "lblUsername"
        lblUsername.Size = New Size(75, 20)
        lblUsername.TabIndex = 1
        lblUsername.Text = "Username"
        ' 
        ' lblTitle
        ' 
        lblTitle.AutoSize = False
        lblTitle.Font = New Font("Segoe UI", 22.0F, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(CByte(40), CByte(40), CByte(40))
        lblTitle.Location = New Point(30, 30)
        lblTitle.Margin = New Padding(4, 0, 4, 0)
        lblTitle.Name = "lblTitle"
        lblTitle.Size = New Size(200, 60)
        lblTitle.TabIndex = 0
        lblTitle.Text = "Sign in"
        lblTitle.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' frmLogin
        ' 
        AutoScaleDimensions = New SizeF(96.0F, 96.0F)
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = Color.FromArgb(CByte(250), CByte(249), CByte(246))
        ClientSize = New Size(700, 380)
        Controls.Add(pnlLoginBox)
        Controls.Add(lblSystemSubtitle)
        Controls.Add(lblSystemTitle)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Margin = New Padding(4, 5, 4, 5)
        MaximizeBox = False
        Name = "frmLogin"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Login"
        pnlLoginBox.ResumeLayout(False)
        pnlLogo.ResumeLayout(False)
        pnlLogoContent.ResumeLayout(False)
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        pnlFormSide.ResumeLayout(False)
        pnlLoginCard.ResumeLayout(False)
        pnlLoginCard.PerformLayout()
        ResumeLayout(False)
        AcceptButton = btnLogin

    End Sub

    Friend WithEvents pnlLoginBox As Panel
    Friend WithEvents lblSystemTitle As Label
    Friend WithEvents lblSystemSubtitle As Label
    Friend WithEvents pnlLogo As Panel
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents pnlLogoContent As Panel
    Friend WithEvents lblWelcomeTitle As Label
    Friend WithEvents lblWelcomeSubtitle As Label
    Friend WithEvents pnlFormSide As Panel
    Friend WithEvents pnlLoginCard As Panel
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubtext As Label
    Friend WithEvents lblUsername As Label
    Friend WithEvents txtUsername As TextBox
    Friend WithEvents lblPassword As Label
    Friend WithEvents chkShowPassword As CheckBox
    Friend WithEvents txtPassword As TextBox
    Friend WithEvents btnLogin As Button
    Friend WithEvents lblStatus As Label
End Class