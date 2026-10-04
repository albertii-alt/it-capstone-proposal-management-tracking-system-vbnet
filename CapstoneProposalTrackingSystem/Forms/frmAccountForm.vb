Option Strict On
Option Explicit On

Public Class frmAccountForm
    Inherits Form

    Private ReadOnly _existingUser As User

    Private ReadOnly pnlHeader As New Panel()
    Private WithEvents lblHeader As New Label()
    Private WithEvents txtFullName As New TextBox()
    Private WithEvents txtEmail As New TextBox()
    Private WithEvents txtUsername As New TextBox()
    Private WithEvents cboRole As New ComboBox()
    Private WithEvents txtPassword As New TextBox()
    Private WithEvents btnSave As New Button()
    Private WithEvents btnCancel As New Button()

    Public Sub New(Optional existingUser As User = Nothing)
        _existingUser = existingUser
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()
        Dim isCreateMode As Boolean = _existingUser Is Nothing

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.ClientSize = If(isCreateMode, New Size(520, 470), New Size(520, 396))
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAccountForm"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = If(isCreateMode, "Add Account", "Edit Account")

        pnlHeader.Name = "pnlHeader"
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 56
        pnlHeader.BackColor = Color.FromArgb(122, 31, 46)

        lblHeader.Name = "lblHeader"
        lblHeader.Text = If(isCreateMode, "Add Account", "Edit Account")
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Padding = New Padding(20, 0, 0, 0)
        lblHeader.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblHeader)

        layout.Name = "tlpAccountForm"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20, 14, 20, 12)
        layout.ColumnCount = 1
        layout.RowCount = If(isCreateMode, 6, 5)
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 72.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        If isCreateMode Then
            layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 74.0F))
        End If
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        ConfigureTextBox(txtFullName, "txtFullName")
        ConfigureTextBox(txtEmail, "txtEmail")
        ConfigureTextBox(txtUsername, "txtUsername")
        ConfigureTextBox(txtPassword, "txtPassword")
        txtPassword.PasswordChar = "*"c

        cboRole.Name = "cboRole"
        cboRole.Dock = DockStyle.Fill
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList
        cboRole.Items.AddRange({"Student", "Admin"})
        cboRole.SelectedIndex = 0
        If _existingUser IsNot Nothing AndAlso _existingUser.Role = UserRole.SuperAdmin Then
            cboRole.Items.Add("SuperAdmin")
            cboRole.SelectedItem = "SuperAdmin"
            cboRole.Enabled = False
        End If

        footer.Name = "flpAccountFormFooter"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 0, 0)

        btnSave.Name = "btnSave"
        btnSave.Text = "Save"
        btnSave.Size = New Size(120, 32)
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.BackColor = Color.FromArgb(246, 201, 69)
        btnSave.ForeColor = Color.FromArgb(122, 31, 46)
        btnSave.UseVisualStyleBackColor = False

        btnCancel.Name = "btnCancel"
        btnCancel.Text = "Cancel"
        btnCancel.Size = New Size(100, 32)
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.UseVisualStyleBackColor = True

        footer.Controls.Add(btnSave)
        footer.Controls.Add(btnCancel)

        layout.Controls.Add(CreateValueField("Full Name", txtFullName), 0, 0)
        layout.Controls.Add(CreateValueField("Email", txtEmail), 0, 1)
        layout.Controls.Add(
            CreateValueField("Username", txtUsername, If(isCreateMode, Nothing, "Used at the next sign-in")),
            0,
            2)
        layout.Controls.Add(CreateValueField("Role", cboRole), 0, 3)
        If isCreateMode Then
            layout.Controls.Add(CreateValueField("Password", txtPassword, "Minimum 6 characters"), 0, 4)
        End If
        layout.Controls.Add(footer, 0, layout.RowCount - 1)

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel
        Me.Controls.Add(layout)
        Me.Controls.Add(pnlHeader)
        Me.ResumeLayout(False)
    End Sub

    Private Function CreateValueField(captionText As String, valueControl As Control, Optional hintText As String = Nothing) As Panel
        Dim fieldPanel As New Panel()
        Dim fieldLayout As New TableLayoutPanel()
        Dim caption As New Label()

        fieldPanel.Dock = DockStyle.Fill
        fieldLayout.Dock = DockStyle.Fill
        fieldLayout.ColumnCount = 1
        fieldLayout.RowCount = If(String.IsNullOrEmpty(hintText), 2, 3)
        fieldLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        fieldLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 18.0F))
        If String.IsNullOrEmpty(hintText) Then
            fieldLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Else
            fieldLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
            fieldLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 16.0F))
        End If

        caption.Dock = DockStyle.Fill
        caption.Text = captionText
        caption.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
        caption.ForeColor = Color.DimGray
        caption.TextAlign = ContentAlignment.MiddleLeft

        valueControl.Dock = DockStyle.Fill
        fieldLayout.Controls.Add(caption, 0, 0)
        fieldLayout.Controls.Add(valueControl, 0, 1)

        If Not String.IsNullOrEmpty(hintText) Then
            Dim hint As New Label()
            hint.Dock = DockStyle.Fill
            hint.Text = hintText
            hint.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
            hint.ForeColor = Color.DimGray
            hint.TextAlign = ContentAlignment.MiddleLeft
            fieldLayout.Controls.Add(hint, 0, 2)
        End If

        fieldPanel.Controls.Add(fieldLayout)
        Return fieldPanel
    End Function

    Private Sub ConfigureTextBox(textBox As TextBox, controlName As String)
        textBox.Name = controlName
        textBox.Dock = DockStyle.Fill
        textBox.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Sub frmAccountForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _existingUser Is Nothing Then
            Return
        End If

        txtFullName.Text = _existingUser.FullName
        txtEmail.Text = _existingUser.Email
        txtUsername.Text = _existingUser.Username
        cboRole.Text = _existingUser.Role.ToString()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim role As UserRole = CType([Enum].Parse(GetType(UserRole), cboRole.Text), UserRole)

        If _existingUser Is Nothing Then
            If String.IsNullOrWhiteSpace(txtUsername.Text) OrElse
               String.IsNullOrWhiteSpace(txtFullName.Text) OrElse
               String.IsNullOrWhiteSpace(txtEmail.Text) OrElse
               String.IsNullOrWhiteSpace(cboRole.Text) OrElse
               String.IsNullOrWhiteSpace(txtPassword.Text) Then
                MessageBox.Show("Please fill in all fields.")
                Return
            End If

            Try
                UserService.CreateUser(
                    txtUsername.Text.Trim(),
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    role,
                    txtPassword.Text,
                    Session.CurrentUser.UserId)
                MessageBox.Show("Account created.")
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Catch ex As ApplicationException
                MessageBox.Show(ex.Message)
            End Try
        Else
            Dim updatedUsername As String = txtUsername.Text.Trim()
            If Not String.Equals(updatedUsername, _existingUser.Username, StringComparison.Ordinal) Then
                Dim confirmation As DialogResult = MessageBox.Show(
                    "Change the username to """ & updatedUsername & """? This account will need the new username at its next login.",
                    "Confirm Username Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)

                If confirmation <> DialogResult.Yes Then
                    Return
                End If
            End If

            Try
                UserService.UpdateUser(
                    _existingUser.UserId,
                    updatedUsername,
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    role,
                    Session.CurrentUser.UserId)

                If Session.CurrentUser IsNot Nothing AndAlso Session.CurrentUser.UserId = _existingUser.UserId Then
                    Session.CurrentUser.Username = updatedUsername
                End If

                MessageBox.Show("Account updated.")
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Catch ex As ApplicationException
                MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class