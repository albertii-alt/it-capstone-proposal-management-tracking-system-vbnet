Imports System.Reflection

Public Class frmStudentRegistration
    Inherits Form

    Private ReadOnly txtSchoolId As New TextBox()
    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtPassword As New TextBox()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private ReadOnly chkShowPassword As New CheckBox()
    Private ReadOnly lblStatus As New Label()

    Public Sub New()
        InitializeRegistrationLayout()
    End Sub

    Private Sub InitializeRegistrationLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = Color.FromArgb(250, 249, 246)
        Me.ClientSize = New Size(760, 480)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Student Registration"

        Dim card As New Panel With {
            .BackColor = SystemColors.Window,
            .BorderStyle = BorderStyle.FixedSingle,
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0)}
        Dim brand As New Panel With {.BackColor = Color.FromArgb(122, 31, 46), .Dock = DockStyle.Left, .Width = 280}
        Dim logo As New PictureBox With {.Size = New Size(150, 150), .Location = New Point(65, 28), .SizeMode = PictureBoxSizeMode.Zoom}
        Dim brandTitle As New Label With {
            .Text = "Create Student Account",
            .ForeColor = Color.FromArgb(246, 201, 69),
            .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(16, 194),
            .Size = New Size(248, 44)}
        Dim brandSubtitle As New Label With {
            .Text = "Submit your details for administrator verification.",
            .ForeColor = Color.FromArgb(240, 217, 160),
            .Font = New Font("Segoe UI", 9.0F),
            .TextAlign = ContentAlignment.MiddleCenter,
            .Location = New Point(20, 242),
            .Size = New Size(240, 64)}
        LoadLogo(logo)
        brand.Controls.AddRange({brandSubtitle, brandTitle, logo})

        Dim formPanel As New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(20, 14, 20, 10)}
        Dim title As New Label With {.Text = "Student Registration", .Font = New Font("Segoe UI", 20.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(40, 40, 40), .Dock = DockStyle.Top, .Height = 48, .TextAlign = ContentAlignment.MiddleLeft, .Padding = New Padding(0, 2, 0, 2)}
        Dim subtitle As New Label With {.Text = "Use the same credentials you will use to sign in.", .Font = New Font("Segoe UI", 8.5F), .ForeColor = Color.DimGray, .Dock = DockStyle.Top, .Height = 24}
        Dim fields As New TableLayoutPanel With {.Dock = DockStyle.Top, .Height = 180, .ColumnCount = 2, .RowCount = 3}
        fields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        fields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        For index As Integer = 0 To 2
            fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 60.0F))
        Next

        ConfigureTextBox(txtSchoolId, "School ID", "24-018701")
        ConfigureTextBox(txtFullName, "Full Name", Nothing)
        ConfigureTextBox(txtEmail, "Email", Nothing)
        ConfigureTextBox(txtUsername, "Username", Nothing)
        ConfigureTextBox(txtPassword, "Password", Nothing)
        ConfigureTextBox(txtConfirmPassword, "Confirm Password", Nothing)
        txtPassword.PasswordChar = "*"c
        txtConfirmPassword.PasswordChar = "*"c
        fields.Controls.Add(CreateField("School ID", txtSchoolId), 0, 0)
        fields.Controls.Add(CreateField("Full Name", txtFullName), 1, 0)
        fields.Controls.Add(CreateField("Email", txtEmail), 0, 1)
        fields.Controls.Add(CreateField("Username", txtUsername), 1, 1)
        fields.Controls.Add(CreateField("Password", txtPassword), 0, 2)
        fields.Controls.Add(CreateField("Confirm Password", txtConfirmPassword), 1, 2)

        chkShowPassword.Name = "chkShowPassword"
        chkShowPassword.Text = "Show password"
        chkShowPassword.Dock = DockStyle.Top
        chkShowPassword.Height = 24
        chkShowPassword.ForeColor = Color.DimGray
        AddHandler chkShowPassword.CheckedChanged, AddressOf chkShowPassword_CheckedChanged

        lblStatus.Dock = DockStyle.Top
        lblStatus.Height = 24
        lblStatus.ForeColor = Color.FromArgb(166, 58, 58)
        lblStatus.TextAlign = ContentAlignment.MiddleCenter

        Dim footer As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 42, .FlowDirection = FlowDirection.RightToLeft, .WrapContents = False}
        Dim registerButton As New Button With {.Text = "Submit Registration", .Size = New Size(150, 32), .BackColor = Color.FromArgb(246, 201, 69), .ForeColor = Color.FromArgb(122, 31, 46), .FlatStyle = FlatStyle.Flat}
        registerButton.FlatAppearance.BorderSize = 0
        AddHandler registerButton.Click, AddressOf RegisterButton_Click
        Dim cancelButton As New Button With {.Text = "Cancel", .Size = New Size(90, 32), .DialogResult = DialogResult.Cancel}
        footer.Controls.Add(registerButton)
        footer.Controls.Add(cancelButton)

        formPanel.Controls.Add(footer)
        formPanel.Controls.Add(lblStatus)
        formPanel.Controls.Add(chkShowPassword)
        formPanel.Controls.Add(fields)
        formPanel.Controls.Add(subtitle)
        formPanel.Controls.Add(title)
        card.Controls.Add(formPanel)
        card.Controls.Add(brand)
        Me.Controls.Add(card)
        Me.AcceptButton = registerButton
        Me.CancelButton = cancelButton
    End Sub

    Private Shared Sub ConfigureTextBox(textBox As TextBox, caption As String, placeholder As String)
        textBox.Name = caption.Replace(" ", String.Empty)
        textBox.Dock = DockStyle.Fill
        textBox.Margin = New Padding(4, 18, 8, 2)
        textBox.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Shared Function CreateField(caption As String, valueControl As TextBox) As Panel
        Dim panel As New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(4, 0, 8, 0)}
        Dim label As New Label With {.Text = caption, .Dock = DockStyle.Top, .Height = 18, .Font = New Font("Segoe UI", 8.0F), .ForeColor = Color.DimGray}
        valueControl.Dock = DockStyle.Fill
        valueControl.Margin = New Padding(0)
        panel.Controls.Add(valueControl)
        panel.Controls.Add(label)
        Return panel
    End Function

    Private Shared Sub LoadLogo(picture As PictureBox)
        Dim resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CapstoneProposalTrackingSystem.CCS_Logo.png")
        If resourceStream IsNot Nothing Then picture.Image = Image.FromStream(resourceStream)
    End Sub

    Private Sub RegisterButton_Click(sender As Object, e As EventArgs)
        lblStatus.Text = String.Empty
        If txtPassword.Text <> txtConfirmPassword.Text Then
            lblStatus.Text = "Passwords do not match."
            Return
        End If
        Try
            UserService.RegisterStudent(txtSchoolId.Text, txtUsername.Text, txtFullName.Text, txtEmail.Text, txtPassword.Text)
            MessageBox.Show("Registration submitted. Please wait for administrator approval.", "Registration Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As ApplicationException
            lblStatus.Text = ex.Message
        End Try
    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs)
        Dim passwordChar As Char = If(chkShowPassword.Checked, ChrW(0), "*"c)
        txtPassword.PasswordChar = passwordChar
        txtConfirmPassword.PasswordChar = passwordChar
    End Sub
End Class
