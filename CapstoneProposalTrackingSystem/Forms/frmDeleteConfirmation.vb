Option Strict On
Option Explicit On

Public Class frmDeleteConfirmation
    Inherits Form

    Private ReadOnly _itemType As String
    Private ReadOnly _itemName As String

    Private WithEvents lblHeader As New Label()
    Private WithEvents lblMessage As New Label()
    Private WithEvents txtItemName As New TextBox()
    Private WithEvents btnDelete As New Button()
    Private WithEvents btnCancel As New Button()

    Public Sub New(itemType As String, itemName As String)
        _itemType = If(String.IsNullOrWhiteSpace(itemType), "item", itemType.Trim())
        _itemName = If(itemName, String.Empty)
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(480, 250)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmDeleteConfirmation"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Confirm Delete"

        layout.Name = "tlpDeleteConfirmation"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 4
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 32.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Text = "Delete " & _itemType & "?"
        lblHeader.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.FromArgb(122, 31, 46)
        lblHeader.TextAlign = ContentAlignment.MiddleLeft

        lblMessage.Name = "lblMessage"
        lblMessage.Dock = DockStyle.Fill
        lblMessage.Text = "This action permanently deletes the following " & _itemType & "."
        lblMessage.ForeColor = Color.FromArgb(60, 60, 60)
        lblMessage.TextAlign = ContentAlignment.MiddleLeft

        txtItemName.Name = "txtItemName"
        txtItemName.Dock = DockStyle.Fill
        txtItemName.Multiline = True
        txtItemName.ReadOnly = True
        txtItemName.WordWrap = True
        txtItemName.ScrollBars = ScrollBars.Vertical
        txtItemName.Text = _itemName
        txtItemName.BackColor = SystemColors.Window

        footer.Name = "flpDeleteConfirmationActions"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 6, 0, 0)

        btnDelete.Name = "btnDelete"
        btnDelete.Text = "Delete"
        btnDelete.Size = New Size(100, 32)
        btnDelete.FlatStyle = FlatStyle.Flat
        btnDelete.FlatAppearance.BorderSize = 0
        btnDelete.BackColor = Color.FromArgb(166, 58, 58)
        btnDelete.ForeColor = Color.White
        btnDelete.DialogResult = DialogResult.OK
        btnDelete.UseVisualStyleBackColor = False

        btnCancel.Name = "btnCancel"
        btnCancel.Text = "Cancel"
        btnCancel.Size = New Size(100, 32)
        btnCancel.Margin = New Padding(0, 0, 8, 0)
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.UseVisualStyleBackColor = True

        footer.Controls.Add(btnDelete)
        footer.Controls.Add(btnCancel)
        layout.Controls.Add(lblHeader, 0, 0)
        layout.Controls.Add(lblMessage, 0, 1)
        layout.Controls.Add(txtItemName, 0, 2)
        layout.Controls.Add(footer, 0, 3)

        Me.AcceptButton = btnDelete
        Me.CancelButton = btnCancel
        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub
End Class