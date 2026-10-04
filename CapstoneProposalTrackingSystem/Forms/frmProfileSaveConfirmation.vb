Option Strict On
Option Explicit On

Imports System.Collections.Generic

Public Class frmProfileSaveConfirmation
    Inherits Form

    Private ReadOnly _changes As List(Of String)

    Public Sub New(changes As IEnumerable(Of String))
        _changes = New List(Of String)(changes)
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()
        Dim lblHeader As New Label()
        Dim lblPrompt As New Label()
        Dim txtChanges As New TextBox()
        Dim btnSave As New Button()
        Dim btnCancel As New Button()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(500, 340)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmProfileSaveConfirmation"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Confirm Profile Changes"

        layout.Name = "tlpProfileSaveConfirmation"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(22)
        layout.ColumnCount = 1
        layout.RowCount = 4
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Text = "Save profile changes?"
        lblHeader.Font = New Font("Segoe UI", 15.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.FromArgb(122, 31, 46)
        lblHeader.TextAlign = ContentAlignment.MiddleLeft

        lblPrompt.Name = "lblPrompt"
        lblPrompt.Dock = DockStyle.Fill
        lblPrompt.Text = "Review the changes before updating your profile."
        lblPrompt.ForeColor = Color.FromArgb(60, 60, 60)
        lblPrompt.TextAlign = ContentAlignment.MiddleLeft

        txtChanges.Name = "txtChanges"
        txtChanges.Dock = DockStyle.Fill
        txtChanges.Multiline = True
        txtChanges.ReadOnly = True
        txtChanges.WordWrap = True
        txtChanges.ScrollBars = ScrollBars.Vertical
        txtChanges.BackColor = SystemColors.Window
        txtChanges.Text = String.Join(Environment.NewLine, _changes)

        footer.Name = "flpProfileSaveActions"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 0, 0)

        btnSave.Name = "btnSaveChanges"
        btnSave.Text = "Save changes"
        btnSave.Size = New Size(120, 32)
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.BackColor = Color.FromArgb(122, 31, 46)
        btnSave.ForeColor = Color.White
        btnSave.DialogResult = DialogResult.OK
        btnSave.UseVisualStyleBackColor = False

        btnCancel.Name = "btnCancel"
        btnCancel.Text = "Cancel"
        btnCancel.Size = New Size(100, 32)
        btnCancel.Margin = New Padding(0, 0, 8, 0)
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.UseVisualStyleBackColor = True

        footer.Controls.Add(btnSave)
        footer.Controls.Add(btnCancel)
        layout.Controls.Add(lblHeader, 0, 0)
        layout.Controls.Add(lblPrompt, 0, 1)
        layout.Controls.Add(txtChanges, 0, 2)
        layout.Controls.Add(footer, 0, 3)

        Me.AcceptButton = btnSave
        Me.CancelButton = btnCancel
        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub
End Class