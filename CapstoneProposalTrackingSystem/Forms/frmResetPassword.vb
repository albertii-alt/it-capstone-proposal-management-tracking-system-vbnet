Option Strict On
Option Explicit On

Public Class frmResetPassword
    Inherits Form

    Private ReadOnly _userId As Integer

    Private ReadOnly pnlHeader As New Panel()
    Private WithEvents lblHeader As New Label()
    Private WithEvents txtNewPassword As New TextBox()
    Private WithEvents txtConfirmPassword As New TextBox()
    Private WithEvents btnSave As New Button()
    Private WithEvents btnCancel As New Button()

    Public Sub New(userId As Integer, userFullName As String)
        _userId = userId
        InitializeComponent(userFullName)
    End Sub

    Private Sub InitializeComponent(userFullName As String)
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(500, 280)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmResetPassword"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Reset Password"

        pnlHeader.Name = "pnlHeader"
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 56
        pnlHeader.BackColor = Color.FromArgb(122, 31, 46)

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Reset Password for " & userFullName
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Padding = New Padding(20, 0, 12, 0)
        lblHeader.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.AutoEllipsis = True
        pnlHeader.Controls.Add(lblHeader)

        layout.Name = "tlpResetPassword"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20, 14, 20, 12)
        layout.ColumnCount = 1
        layout.RowCount = 3
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        ConfigurePasswordTextBox(txtNewPassword, "txtNewPassword")
        ConfigurePasswordTextBox(txtConfirmPassword, "txtConfirmPassword")

        footer.Name = "flpResetPasswordFooter"
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

        layout.Controls.Add(CreateValueField("New Password", txtNewPassword), 0, 0)
        layout.Controls.Add(CreateValueField("Confirm Password", txtConfirmPassword), 0, 1)
        layout.Controls.Add(footer, 0, 2)

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel
        Me.Controls.Add(layout)
        Me.Controls.Add(pnlHeader)
        Me.ResumeLayout(False)
    End Sub

    Private Function CreateValueField(captionText As String, valueControl As Control) As Panel
        Dim fieldPanel As New Panel()
        Dim caption As New Label()

        fieldPanel.Dock = DockStyle.Fill
        fieldPanel.Padding = New Padding(0, 0, 8, 0)

        caption.AutoSize = False
        caption.Dock = DockStyle.Top
        caption.Height = 18
        caption.Text = captionText
        caption.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
        caption.ForeColor = Color.DimGray
        caption.TextAlign = ContentAlignment.MiddleLeft

        valueControl.Dock = DockStyle.Fill
        fieldPanel.Controls.Add(valueControl)
        fieldPanel.Controls.Add(caption)
        Return fieldPanel
    End Function

    Private Sub ConfigurePasswordTextBox(textBox As TextBox, controlName As String)
        textBox.Name = controlName
        textBox.Dock = DockStyle.Fill
        textBox.BorderStyle = BorderStyle.FixedSingle
        textBox.PasswordChar = "*"c
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If txtNewPassword.Text <> txtConfirmPassword.Text Then
            MessageBox.Show("Passwords do not match.")
            Return
        End If

        Try
            UserService.ResetPassword(_userId, txtNewPassword.Text, Session.CurrentUser.UserId)
            MessageBox.Show("Password reset successfully.")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class