Option Strict On
Option Explicit On

Public Class frmAdminActivityLogDetails
    Inherits Form

    Private ReadOnly _entry As AdminActivityLogEntry
    Private ReadOnly pnlHeader As New Panel()
    Private ReadOnly lblHeader As New Label()
    Private ReadOnly txtDetails As New TextBox()
    Private ReadOnly btnClose As New Button()

    Public Sub New(entry As AdminActivityLogEntry)
        If entry Is Nothing Then
            Throw New ArgumentNullException(NameOf(entry))
        End If

        _entry = entry
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(620, 470)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAdminActivityLogDetails"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Activity Log Details"

        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 56
        pnlHeader.BackColor = Color.FromArgb(122, 31, 46)
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Padding = New Padding(20, 0, 0, 0)
        lblHeader.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Text = "Activity Log Details"
        pnlHeader.Controls.Add(lblHeader)

        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20, 14, 20, 12)
        layout.ColumnCount = 1
        layout.RowCount = 2
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        txtDetails.Dock = DockStyle.Fill
        txtDetails.Multiline = True
        txtDetails.ReadOnly = True
        txtDetails.ScrollBars = ScrollBars.Vertical
        txtDetails.BackColor = SystemColors.Window
        txtDetails.Font = New Font("Segoe UI", 9.5F)
        txtDetails.Text = BuildDetailsText()

        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 0, 0)
        btnClose.Text = "Close"
        btnClose.Size = New Size(100, 32)
        btnClose.DialogResult = DialogResult.OK
        footer.Controls.Add(btnClose)

        layout.Controls.Add(txtDetails, 0, 0)
        layout.Controls.Add(footer, 0, 1)
        Me.AcceptButton = btnClose
        Me.CancelButton = btnClose
        Me.Controls.Add(layout)
        Me.Controls.Add(pnlHeader)
        Me.ResumeLayout(False)
    End Sub

    Private Function BuildDetailsText() As String
        Dim entityIdText As String = If(_entry.EntityId.HasValue, _entry.EntityId.Value.ToString(), "Not available")
        Dim entityNameText As String = If(String.IsNullOrWhiteSpace(_entry.EntityName), "Not available", _entry.EntityName)
        Dim metadataText As String = If(String.IsNullOrWhiteSpace(_entry.Metadata), "None", _entry.Metadata)

        Return "Date and time: " & _entry.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") & Environment.NewLine &
               "Administrator: " & _entry.ActorName & Environment.NewLine &
               "Action: " & _entry.ActionType & Environment.NewLine &
               "Area: " & _entry.EntityType & Environment.NewLine &
               "Target: " & entityNameText & Environment.NewLine &
               "Target ID: " & entityIdText & Environment.NewLine &
               Environment.NewLine &
               "Details:" & Environment.NewLine &
               _entry.Description & Environment.NewLine &
               Environment.NewLine &
               "Additional information:" & Environment.NewLine &
               metadataText
    End Function
End Class
