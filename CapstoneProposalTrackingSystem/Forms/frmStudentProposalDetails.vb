Imports System.Collections.Generic

Public Class frmStudentProposalDetails
    Inherits Form

    Private ReadOnly _proposal As Proposal

    Public Sub New(proposal As Proposal)
        If proposal Is Nothing Then Throw New ArgumentNullException(NameOf(proposal))
        _proposal = proposal
        BuildLayout()
    End Sub

    Private Sub BuildLayout()
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = SystemColors.Control
        ClientSize = New Size(650, 720)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        StartPosition = FormStartPosition.CenterParent
        Text = "View Proposal"

        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 12}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 52.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 62.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 26.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 28.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 26.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 20.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 26.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 22.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 26.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        Dim title As New Label With {.Text = "Proposal Details", .Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 18.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(122, 31, 46), .TextAlign = ContentAlignment.MiddleLeft, .Padding = New Padding(0, 4, 0, 0)}
        layout.Controls.Add(title, 0, 0)
        layout.Controls.Add(CreateReadOnlyField(_proposal.Title), 0, 1)
        layout.Controls.Add(CreateReadOnlyField("Semester: " & _proposal.Semester & "    |    Year Level: " & _proposal.YearLevel), 0, 2)
        layout.Controls.Add(CreateReadOnlyField("Group: " & _proposal.GroupName & "    |    Status: " & FormatStatus(_proposal.Status)), 0, 3)
        layout.Controls.Add(CreateSectionLabel("Description"), 0, 4)
        layout.Controls.Add(CreateReadOnlyField(_proposal.Description, True), 0, 5)
        layout.Controls.Add(CreateSectionLabel("Group Members"), 0, 6)
        layout.Controls.Add(CreateReadOnlyField(GetMembersText(), True), 0, 7)
        layout.Controls.Add(CreateSectionLabel("Administrator Remarks"), 0, 8)
        layout.Controls.Add(CreateReadOnlyField(GetRemarks(), True), 0, 9)
        layout.Controls.Add(CreateSectionLabel("Submitted"), 0, 10)
        Dim footer As New Panel With {.Dock = DockStyle.Fill}
        Dim submitted As New Label With {.Text = _proposal.SubmittedAt.ToString("MMM d, yyyy h:mm:ss tt"), .Dock = DockStyle.Left, .Width = 280, .TextAlign = ContentAlignment.MiddleLeft, .ForeColor = Color.DimGray}
        Dim closeButton As New Button With {.Text = "Close", .Size = New Size(90, 30), .Dock = DockStyle.Right}
        AddHandler closeButton.Click, Sub(sender, e) Close()
        footer.Controls.Add(closeButton)
        footer.Controls.Add(submitted)
        layout.Controls.Add(footer, 0, 11)
        Controls.Add(layout)
    End Sub

    Private Shared Function CreateSectionLabel(text As String) As Label
        Return New Label With {.Text = text, .Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 9.5F, FontStyle.Bold), .ForeColor = Color.FromArgb(122, 31, 46), .TextAlign = ContentAlignment.MiddleLeft}
    End Function

    Private Shared Function CreateReadOnlyField(text As String, Optional multiline As Boolean = False) As TextBox
        Return New TextBox With {.Text = text, .Dock = DockStyle.Fill, .ReadOnly = True, .Multiline = multiline, .ScrollBars = If(multiline, ScrollBars.Vertical, ScrollBars.None), .BackColor = SystemColors.Window, .BorderStyle = BorderStyle.FixedSingle}
    End Function

    Private Function GetMembersText() As String
        Dim names As New List(Of String)()
        For Each member As ProposalMember In _proposal.Members
            names.Add(member.MemberName)
        Next
        Return String.Join(Environment.NewLine, names)
    End Function

    Private Function GetRemarks() As String
        Dim log As StatusLogEntry = ProposalService.GetLatestLog(_proposal.ProposalId)
        If log Is Nothing OrElse String.IsNullOrWhiteSpace(log.Remarks) Then Return "No administrator remarks yet."
        Return log.Remarks
    End Function

    Private Shared Function FormatStatus(status As ProposalStatus) As String
        Return If(status = ProposalStatus.ForRevision, "For Revision", status.ToString())
    End Function
End Class
