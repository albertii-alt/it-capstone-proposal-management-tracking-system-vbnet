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
    Private ReadOnly brandPanel As New Panel()
    Private ReadOnly brandLogo As New PictureBox()
    Private ReadOnly brandTitle As New Label()
    Private ReadOnly brandSubtitle As New Label()
    Private ReadOnly registrationLayout As New TableLayoutPanel()
    Private ReadOnly registrationFields As New TableLayoutPanel()
    Private ReadOnly registrationFieldsHost As New Panel()
    Private ReadOnly registrationContent As New Panel()
    Private ReadOnly registrationFooter As New Panel()
    Private ReadOnly _registerButton As New Button()
    Private ReadOnly _cancelButton As New Button()
    Private ReadOnly registrationTitle As New Label()
    Private ReadOnly registrationSubtitle As New Label()

    Public Sub New()
        InitializeRegistrationLayout()
        AddHandler Me.Load, AddressOf RegistrationForm_Load
        AddHandler Me.Resize, AddressOf RegistrationForm_Resize
        AddHandler brandPanel.Resize, AddressOf PositionRegistrationBrand
        PositionRegistrationBrand(Me, EventArgs.Empty)
    End Sub

    Private Sub InitializeRegistrationLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = Color.FromArgb(250, 249, 246)
        Me.ClientSize = New Size(1280, 800)
        Me.MinimumSize = New Size(900, 650)
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.MinimizeBox = True
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Student Registration"

        registrationLayout.Dock = DockStyle.Fill
        registrationLayout.ColumnCount = 2
        registrationLayout.RowCount = 1
        registrationLayout.Margin = New Padding(0)
        registrationLayout.Padding = New Padding(0)
        registrationLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 420.0F))
        registrationLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        registrationLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        brandPanel.Name = "pnlRegistrationBrand"
        brandPanel.BackColor = Color.FromArgb(122, 31, 46)
        brandPanel.Dock = DockStyle.Fill
        brandLogo.Name = "picRegistrationLogo"
        brandLogo.Size = New Size(190, 190)
        brandLogo.SizeMode = PictureBoxSizeMode.Zoom
        brandTitle.Text = "Create Student Account"
        brandTitle.ForeColor = Color.FromArgb(246, 201, 69)
        brandTitle.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)
        brandTitle.TextAlign = ContentAlignment.MiddleCenter
        brandSubtitle.Text = "Submit your details for administrator verification."
        brandSubtitle.ForeColor = Color.FromArgb(240, 217, 160)
        brandSubtitle.Font = New Font("Segoe UI", 12.0F)
        brandSubtitle.TextAlign = ContentAlignment.MiddleCenter
        LoadLogo(brandLogo)
        brandPanel.Controls.AddRange({brandSubtitle, brandTitle, brandLogo})

        Dim formPanel As New Panel With {
            .Name = "pnlRegistrationForm",
            .Dock = DockStyle.Fill,
            .Padding = New Padding(48, 32, 48, 24),
            .AutoScroll = True,
            .BackColor = SystemColors.Window
        }
        registrationContent.Name = "pnlRegistrationContent"
        registrationContent.Size = New Size(900, 578)
        registrationContent.Anchor = AnchorStyles.None

        registrationTitle.Text = "Student Registration"
        registrationTitle.Font = New Font("Segoe UI", 28.0F, FontStyle.Bold)
        registrationTitle.ForeColor = Color.FromArgb(40, 40, 40)
        registrationTitle.Height = 64
        registrationTitle.TextAlign = ContentAlignment.MiddleLeft
        registrationTitle.Padding = New Padding(0, 2, 0, 2)

        registrationSubtitle.Text = "Use the same credentials you will use to sign in."
        registrationSubtitle.Font = New Font("Segoe UI", 11.0F)
        registrationSubtitle.ForeColor = Color.DimGray
        registrationSubtitle.TextAlign = ContentAlignment.MiddleLeft
        registrationSubtitle.Height = 36

        registrationTitle.Name = "lblRegistrationTitle"
        registrationSubtitle.Name = "lblRegistrationSubtitle"
        registrationFieldsHost.Name = "pnlRegistrationFieldsHost"
        registrationFieldsHost.Dock = DockStyle.None
        registrationFieldsHost.Height = 292
        registrationFieldsHost.Margin = New Padding(0)

        Dim fields As TableLayoutPanel = registrationFields
        fields.Name = "tlpRegistrationFields"
        fields.Dock = DockStyle.None
        fields.Size = New Size(820, 282)
        fields.ColumnCount = 2
        fields.RowCount = 3
        fields.Margin = New Padding(0)
        fields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        fields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        For index As Integer = 0 To 2
            fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 94.0F))
        Next

        ConfigureTextBox(txtSchoolId, "School ID")
        ConfigureTextBox(txtFullName, "Full Name")
        ConfigureTextBox(txtEmail, "Email")
        ConfigureTextBox(txtUsername, "Username")
        ConfigureTextBox(txtPassword, "Password")
        ConfigureTextBox(txtConfirmPassword, "Confirm Password")
        txtPassword.PasswordChar = "*"c
        txtConfirmPassword.PasswordChar = "*"c
        fields.Controls.Add(CreateField("School ID", txtSchoolId), 0, 0)
        fields.Controls.Add(CreateField("Full Name", txtFullName), 1, 0)
        fields.Controls.Add(CreateField("Email", txtEmail), 0, 1)
        fields.Controls.Add(CreateField("Username", txtUsername), 1, 1)
        fields.Controls.Add(CreateField("Password", txtPassword), 0, 2)
        fields.Controls.Add(CreateField("Confirm Password", txtConfirmPassword), 1, 2)
        registrationFieldsHost.Controls.Add(fields)
        AddHandler registrationFieldsHost.Resize, AddressOf CenterRegistrationFields
        CenterRegistrationFields(registrationFieldsHost, EventArgs.Empty)

        chkShowPassword.Name = "chkShowPassword"
        chkShowPassword.Text = "Show password"
        chkShowPassword.AutoSize = True
        chkShowPassword.Height = 34
        chkShowPassword.Font = New Font("Segoe UI", 10.0F)
        chkShowPassword.ForeColor = Color.DimGray
        AddHandler chkShowPassword.CheckedChanged, AddressOf chkShowPassword_CheckedChanged

        lblStatus.Name = "lblRegistrationStatus"
        lblStatus.Height = 42
        lblStatus.Font = New Font("Segoe UI", 10.0F)
        lblStatus.ForeColor = Color.FromArgb(166, 58, 58)
        lblStatus.TextAlign = ContentAlignment.MiddleCenter

        registrationFooter.Name = "pnlRegistrationActions"
        registrationFooter.Dock = DockStyle.None
        registrationFooter.Height = 60
        registrationFooter.BackColor = SystemColors.Window

        _registerButton.Text = "Submit Registration"
        _registerButton.Size = New Size(210, 48)
        _registerButton.BackColor = Color.FromArgb(246, 201, 69)
        _registerButton.ForeColor = Color.FromArgb(122, 31, 46)
        _registerButton.FlatStyle = FlatStyle.Flat
        _registerButton.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        _registerButton.Cursor = Cursors.Hand
        _registerButton.FlatAppearance.BorderSize = 0
        AddHandler _registerButton.Click, AddressOf RegisterButton_Click

        _cancelButton.Text = "Cancel"
        _cancelButton.Size = New Size(110, 48)
        _cancelButton.DialogResult = DialogResult.Cancel
        _cancelButton.Font = New Font("Segoe UI", 10.0F)
        AddHandler registrationFooter.Resize, AddressOf CenterRegistrationActions
        registrationFooter.Controls.Add(_registerButton)
        registrationFooter.Controls.Add(_cancelButton)

        registrationContent.Controls.Add(registrationTitle)
        registrationContent.Controls.Add(registrationSubtitle)
        registrationContent.Controls.Add(registrationFieldsHost)
        registrationContent.Controls.Add(chkShowPassword)
        registrationContent.Controls.Add(lblStatus)
        registrationContent.Controls.Add(registrationFooter)
        formPanel.Controls.Add(registrationContent)

        AddHandler formPanel.Resize, AddressOf CenterRegistrationContent
        AddHandler registrationContent.Resize, AddressOf LayoutRegistrationContent
        CenterRegistrationContent(formPanel, EventArgs.Empty)
        LayoutRegistrationContent(registrationContent, EventArgs.Empty)
        CenterRegistrationToggle(registrationContent, EventArgs.Empty)

        registrationLayout.Controls.Add(brandPanel, 0, 0)
        registrationLayout.Controls.Add(formPanel, 1, 0)
        Me.Controls.Add(registrationLayout)
        Me.AcceptButton = _registerButton
        Me.CancelButton = _cancelButton
    End Sub

    Private Sub RegistrationForm_Load(sender As Object, e As EventArgs)
        FormDisplayHelper.MaximizeFullScreen(Me)
        UpdateRegistrationBrandWidth()
    End Sub

    Private Sub RegistrationForm_Resize(sender As Object, e As EventArgs)
        UpdateRegistrationBrandWidth()
    End Sub

    Private Sub UpdateRegistrationBrandWidth()
        If registrationLayout Is Nothing OrElse brandPanel Is Nothing Then Return
        Dim brandWidth As Integer = Math.Max(300, Math.Min(500, CInt(registrationLayout.ClientSize.Width * 0.36R)))
        registrationLayout.ColumnStyles(0).Width = brandWidth
        PositionRegistrationBrand(Me, EventArgs.Empty)
    End Sub

    Private Sub PositionRegistrationBrand(sender As Object, e As EventArgs)
        Dim panelWidth As Integer = brandPanel.ClientSize.Width
        Dim panelHeight As Integer = brandPanel.ClientSize.Height
        brandLogo.Location = New Point(Math.Max(0, (panelWidth - brandLogo.Width) \ 2),
                                       Math.Max(24, CInt(panelHeight * 0.28R) - brandLogo.Height \ 2))
        brandTitle.SetBounds(24, brandLogo.Bottom + 24, Math.Max(0, panelWidth - 48), 64)
        brandSubtitle.SetBounds(36, brandTitle.Bottom + 12, Math.Max(0, panelWidth - 72), 72)
    End Sub

    Private Sub CenterRegistrationFields(sender As Object, e As EventArgs)
        Dim fieldsWidth As Integer = Math.Min(900, registrationFieldsHost.ClientSize.Width)
        registrationFields.Size = New Size(fieldsWidth, registrationFields.Height)
        registrationFields.Location = New Point(
            Math.Max(0, (registrationFieldsHost.ClientSize.Width - fieldsWidth) \ 2),
            0)
        AlignRegistrationContentToFields()
        CenterRegistrationActions(registrationFooter, EventArgs.Empty)
    End Sub

    Private Sub CenterRegistrationContent(sender As Object, e As EventArgs)
        Dim formPanel As Panel = DirectCast(sender, Panel)
        Dim availableWidth As Integer = Math.Max(0, formPanel.ClientSize.Width - formPanel.Padding.Horizontal)
        Dim availableHeight As Integer = Math.Max(0, formPanel.ClientSize.Height - formPanel.Padding.Vertical)
        Dim contentWidth As Integer = Math.Min(900, availableWidth)
        registrationContent.Width = contentWidth
        registrationContent.Location = New Point(
            formPanel.Padding.Left + Math.Max(0, (availableWidth - contentWidth) \ 2),
            formPanel.Padding.Top + Math.Max(0, (availableHeight - registrationContent.Height) \ 2))
    End Sub

    Private Sub LayoutRegistrationContent(sender As Object, e As EventArgs)
        Dim contentWidth As Integer = registrationContent.ClientSize.Width
        registrationTitle.SetBounds(0, 0, contentWidth, 64)
        registrationSubtitle.SetBounds(0, 64, contentWidth, 36)
        registrationFieldsHost.SetBounds(0, 120, contentWidth, 292)
        lblStatus.SetBounds(0, 456, contentWidth, 42)
        registrationFooter.SetBounds(0, 498, contentWidth, 60)
        CenterRegistrationFields(registrationFieldsHost, EventArgs.Empty)
        CenterRegistrationToggle(registrationContent, EventArgs.Empty)
    End Sub

    Private Sub AlignRegistrationContentToFields()
        Dim leftEdge As Integer = registrationFields.Left + 15
        Dim rightEdge As Integer = registrationFields.Right - 25
        Dim alignedWidth As Integer = Math.Max(0, rightEdge - leftEdge)

        registrationTitle.SetBounds(leftEdge, 0, alignedWidth, 64)
        registrationSubtitle.SetBounds(leftEdge, 64, alignedWidth, 36)
        lblStatus.SetBounds(leftEdge, 456, alignedWidth, 42)
    End Sub

    Private Sub CenterRegistrationToggle(sender As Object, e As EventArgs)
        chkShowPassword.Location = New Point(
            registrationFields.Left + 15,
            422)
    End Sub

    Private Sub CenterRegistrationActions(sender As Object, e As EventArgs)
        Dim gap As Integer = 12
        Dim groupWidth As Integer = _registerButton.Width + _cancelButton.Width + gap
        Dim rightEdge As Integer = Math.Min(registrationFooter.ClientSize.Width, registrationFields.Right - 25)
        Dim left As Integer = Math.Max(0, rightEdge - groupWidth)
        _cancelButton.Location = New Point(left, 6)
        _registerButton.Location = New Point(_cancelButton.Right + gap, 6)
    End Sub

    Private Shared Sub ConfigureTextBox(textBox As TextBox, caption As String)
        textBox.Name = caption.Replace(" ", String.Empty)
        textBox.Dock = DockStyle.Fill
        textBox.Margin = New Padding(4, 18, 8, 2)
        textBox.BorderStyle = BorderStyle.FixedSingle
        textBox.Font = New Font("Segoe UI", 12.0F)
        textBox.MinimumSize = New Size(0, 42)
    End Sub

    Private Shared Function CreateField(caption As String, valueControl As TextBox) As Panel
        Dim panel As New Panel With {.Dock = DockStyle.Fill, .Padding = New Padding(8, 4, 14, 4)}
        Dim label As New Label With {
            .Text = caption,
            .Dock = DockStyle.Top,
            .Height = 26,
            .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold),
            .ForeColor = Color.DimGray
        }
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
