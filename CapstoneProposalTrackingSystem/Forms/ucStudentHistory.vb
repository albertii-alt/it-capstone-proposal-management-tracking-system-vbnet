Imports System.Collections.Generic

Public Class ucStudentHistory
    Inherits UserControl

    Private ReadOnly grid As New DataGridView()
    Private ReadOnly historyItems As New List(Of HistoryItem)()

    Private Class HistoryItem
        Public Property Timestamp As DateTime
        Public Property Proposal As String
        Public Property Action As String
        Public Property Remarks As String
    End Class

    Public Sub New()
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = SystemColors.Control
        BuildLayout()
        LoadHistory()
    End Sub

    Private Sub BuildLayout()
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 2}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 112.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Dim header As New Panel With {.Dock = DockStyle.Fill}
        header.Controls.Add(New Label With {.Text = "History", .Dock = DockStyle.Top, .Height = 66, .Font = New Font("Segoe UI", 23.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45), .Padding = New Padding(0, 5, 0, 0)})
        header.Controls.Add(New Label With {.Text = "Review the activity and review history for your proposals.", .Dock = DockStyle.Bottom, .Height = 26, .ForeColor = Color.DimGray})
        layout.Controls.Add(header, 0, 0)
        ConfigureGrid()
        layout.Controls.Add(grid, 0, 1)
        Controls.Add(layout)
    End Sub

    Private Sub ConfigureGrid()
        grid.Dock = DockStyle.Fill
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.ReadOnly = True
        grid.RowHeadersVisible = False
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        grid.ColumnHeadersHeight = 40
        grid.RowTemplate.Height = 46
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        grid.GridColor = Color.FromArgb(235, 235, 235)
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.Columns.Add("date", "DATE")
        grid.Columns.Add("proposal", "PROPOSAL")
        grid.Columns.Add("action", "ACTIVITY")
        grid.Columns.Add("remarks", "REMARKS")
    End Sub

    Private Sub LoadHistory()
        Try
            historyItems.Clear()
            grid.Rows.Clear()
            Dim proposals As List(Of Proposal) = ProposalService.GetMyProposals(Session.CurrentUser.UserId)
            For Each proposal As Proposal In proposals
                AddHistory(proposal.SubmittedAt, proposal.Title, "Submitted", "Proposal submitted for review.")
                For Each log As StatusLogEntry In ProposalService.GetStatusLogs(proposal.ProposalId)
                    Dim action As String = GetStatusChangeText(log)
                    AddHistory(log.ChangedAt, proposal.Title, action, If(String.IsNullOrWhiteSpace(log.Remarks), "No remarks provided.", log.Remarks))
                Next
            Next
            If historyItems.Count = 0 Then
                AddHistory(DateTime.Now, "No records", "No activity", "Your proposal activity will appear here.")
            End If
            grid.Rows.Clear()
            For Each item As HistoryItem In historyItems.OrderByDescending(Function(entry) entry.Timestamp)
                grid.Rows.Add(item.Timestamp.ToString("MMM d, yyyy h:mm tt"), item.Proposal, item.Action, item.Remarks)
            Next
        Catch ex As Exception
            MessageBox.Show("Could not load your history: " & ex.Message, "History", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Shared Function GetStatusChangeText(log As StatusLogEntry) As String
        If log.NewStatus = ProposalStatus.ForRevision Then Return "Requested Revision"
        If log.OldStatus.HasValue Then
            Return "Status changed to " & FormatStatus(log.NewStatus)
        End If
        Return "Status set to " & FormatStatus(log.NewStatus)
    End Function

    Private Shared Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then Return "For Revision"
        Return status.ToString()
    End Function

    Private Sub AddHistory(timestamp As DateTime, title As String, action As String, description As String)
        historyItems.Add(New HistoryItem With {.Timestamp = timestamp, .Proposal = title, .Action = action, .Remarks = description})
    End Sub
End Class
