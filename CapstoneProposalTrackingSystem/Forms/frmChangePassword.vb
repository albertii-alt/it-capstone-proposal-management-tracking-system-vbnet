Option Strict On
Option Explicit On

Public Class frmChangePassword
    Inherits Form

    Private ReadOnly _userId As Integer
    Private ReadOnly txtCurrentPassword As New TextBox()
    Private ReadOnly txtNewPassword As New TextBox()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private ReadOnly lblStatus As New Label()

    Public Sub New(userId As Integer)
        _userId = userId
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim fields As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()
        Dim lblHeader As New Label()
        Dim lblPrompt As New Label()
        Dim btnUpdate As New Button()
        Dim btnCancel As New Button()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(500, 410)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmChangePassword"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Change Password"

        layout.Name = "tlpChangePassword"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(22)
        layout.ColumnCount = 1
        layout.RowCount = 5
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Text = "Change your password"
        lblHeader.Font = New Font("Segoe UI", 15.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.FromArgb(122, 31, 46)
        lblHeader.TextAlign = ContentAlignment.MiddleLeft

        lblPrompt.Name = "lblPrompt"
        lblPrompt.Dock = DockStyle.Fill
        lblPrompt.Text = "Enter your current password and choose a new one."
        lblPrompt.ForeColor = Color.FromArgb(60, 60, 60)
        lblPrompt.TextAlign = ContentAlignment.MiddleLeft

        fields.Name = "tlpPasswordFields"
        fields.Dock = DockStyle.Fill
        fields.ColumnCount = 1
        fields.RowCount = 6
        fields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        fields.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))

        ConfigurePasswordBox(txtCurrentPassword, "txtCurrentPassword")
        ConfigurePasswordBox(txtNewPassword, "txtNewPassword")
        ConfigurePasswordBox(txtConfirmPassword, "txtConfirmPassword")
        fields.Controls.Add(CreateFieldLabel("Current password"), 0, 0)
        fields.Controls.Add(txtCurrentPassword, 0, 1)
        fields.Controls.Add(CreateFieldLabel("New password"), 0, 2)
        fields.Controls.Add(txtNewPassword, 0, 3)
        fields.Controls.Add(CreateFieldLabel("Confirm new password"), 0, 4)
        fields.Controls.Add(txtConfirmPassword, 0, 5)

        lblStatus.Name = "lblStatus"
        lblStatus.Dock = DockStyle.Fill
        lblStatus.TextAlign = ContentAlignment.MiddleLeft
        lblStatus.ForeColor = Color.FromArgb(166, 58, 58)

        footer.Name = "flpChangePasswordActions"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 0, 0)

        btnUpdate.Name = "btnUpdatePassword"
        btnUpdate.Text = "Update password"
        btnUpdate.Size = New Size(140, 32)
        btnUpdate.FlatStyle = FlatStyle.Flat
        btnUpdate.FlatAppearance.BorderSize = 0
        btnUpdate.BackColor = Color.FromArgb(122, 31, 46)
        btnUpdate.ForeColor = Color.White
        btnUpdate.UseVisualStyleBackColor = False
        AddHandler btnUpdate.Click, AddressOf btnUpdate_Click

        btnCancel.Name = "btnCancel"
        btnCancel.Text = "Cancel"
        btnCancel.Size = New Size(100, 32)
        btnCancel.Margin = New Padding(0, 0, 8, 0)
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.UseVisualStyleBackColor = True

        footer.Controls.Add(btnUpdate)
        footer.Controls.Add(btnCancel)
        layout.Controls.Add(lblHeader, 0, 0)
        layout.Controls.Add(lblPrompt, 0, 1)
        layout.Controls.Add(fields, 0, 2)
        layout.Controls.Add(lblStatus, 0, 3)
        layout.Controls.Add(footer, 0, 4)

        Me.AcceptButton = btnUpdate
        Me.CancelButton = btnCancel
        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub

    Private Shared Function CreateFieldLabel(text As String) As Label
        Return New Label With {
            .Dock = DockStyle.Fill,
            .Text = text,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(75, 75, 75)
        }
    End Function

    Private Shared Sub ConfigurePasswordBox(textBox As TextBox, controlName As String)
        textBox.Name = controlName
        textBox.Dock = DockStyle.Fill
        textBox.BorderStyle = BorderStyle.FixedSingle
        textBox.Margin = New Padding(0, 2, 0, 5)
        textBox.UseSystemPasswordChar = True
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs)
        If txtNewPassword.Text <> txtConfirmPassword.Text Then
            lblStatus.Text = "The new password and confirmation do not match."
            Return
        End If

        Try
            UserService.ChangeOwnPassword(_userId, txtCurrentPassword.Text, txtNewPassword.Text)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As ApplicationException
            lblStatus.Text = ex.Message
            txtCurrentPassword.Clear()
            txtCurrentPassword.Focus()
        Catch ex As Exception
            lblStatus.Text = "Could not update your password. Please try again."
            txtCurrentPassword.Clear()
            txtCurrentPassword.Focus()
        End Try
    End Sub
End Class