Option Strict On
Option Explicit On

Public Class frmProfileSuccess
    Inherits Form

    Private ReadOnly _title As String
    Private ReadOnly _message As String

    Public Sub New(title As String, message As String)
        _title = If(String.IsNullOrWhiteSpace(title), "Success", title.Trim())
        _message = If(String.IsNullOrWhiteSpace(message), "Your changes were saved.", message.Trim())
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim header As New Panel()
        Dim lblHeader As New Label()
        Dim lblBadge As New Label()
        Dim lblMessage As New Label()
        Dim footer As New FlowLayoutPanel()
        Dim btnDone As New Button()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(460, 250)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmProfileSuccess"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = _title

        header.Name = "pnlSuccessHeader"
        header.Dock = DockStyle.Fill
        header.BackColor = Color.FromArgb(52, 112, 78)

        lblHeader.Name = "lblSuccessHeader"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Padding = New Padding(22, 0, 16, 0)
        lblHeader.Text = _title
        lblHeader.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        header.Controls.Add(lblHeader)

        layout.Name = "tlpProfileSuccess"
        layout.Dock = DockStyle.Fill
        layout.ColumnCount = 1
        layout.RowCount = 4
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 54.0F))

        lblBadge.Name = "lblSuccessBadge"
        lblBadge.AutoSize = False
        lblBadge.Size = New Size(92, 28)
        lblBadge.Margin = New Padding(22, 10, 0, 0)
        lblBadge.Text = "SUCCESS"
        lblBadge.Font = New Font("Segoe UI", 8.0F, FontStyle.Bold)
        lblBadge.ForeColor = Color.FromArgb(41, 99, 66)
        lblBadge.BackColor = Color.FromArgb(223, 240, 229)
        lblBadge.TextAlign = ContentAlignment.MiddleCenter

        lblMessage.Name = "lblSuccessMessage"
        lblMessage.Dock = DockStyle.Fill
        lblMessage.Padding = New Padding(22, 0, 22, 8)
        lblMessage.Text = _message
        lblMessage.Font = New Font("Segoe UI", 10.0F, FontStyle.Regular)
        lblMessage.ForeColor = Color.FromArgb(55, 55, 55)
        lblMessage.TextAlign = ContentAlignment.MiddleLeft

        footer.Name = "flpSuccessActions"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 20, 0)

        btnDone.Name = "btnDone"
        btnDone.Text = "Done"
        btnDone.Size = New Size(100, 34)
        btnDone.FlatStyle = FlatStyle.Flat
        btnDone.FlatAppearance.BorderSize = 0
        btnDone.BackColor = Color.FromArgb(122, 31, 46)
        btnDone.ForeColor = Color.White
        btnDone.DialogResult = DialogResult.OK
        btnDone.UseVisualStyleBackColor = False
        footer.Controls.Add(btnDone)

        layout.Controls.Add(header, 0, 0)
        layout.Controls.Add(lblBadge, 0, 1)
        layout.Controls.Add(lblMessage, 0, 2)
        layout.Controls.Add(footer, 0, 3)

        Me.AcceptButton = btnDone
        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub
End Class