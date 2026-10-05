Imports System.Collections.Generic

Public Class ucStudentProposals
    Inherits UserControl

    Private ReadOnly grid As New DataGridView()
    Private ReadOnly remarks As New TextBox()

    Public Sub New()
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = SystemColors.Control
        BuildLayout()
        LoadProposals()
    End Sub

    Private Sub BuildLayout()
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 3}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 112.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 46.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim header As New Panel With {.Dock = DockStyle.Fill}
        header.Controls.Add(New Label With {.Text = "My Proposals", .Dock = DockStyle.Top, .Height = 66, .Font = New Font("Segoe UI", 23.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45), .Padding = New Padding(0, 5, 0, 0)})
        header.Controls.Add(New Label With {.Text = "Submit proposals and track their review status.", .Dock = DockStyle.Bottom, .Height = 26, .ForeColor = Color.DimGray})
        layout.Controls.Add(header, 0, 0)

        Dim toolbar As New Panel With {.Dock = DockStyle.Fill}
        Dim submit As New Button With {.Text = "Submit New Proposal", .Size = New Size(160, 30), .Dock = DockStyle.Right}
        AddHandler submit.Click, AddressOf SubmitProposal
        Dim refresh As New Button With {.Text = "Refresh", .Size = New Size(90, 30), .Dock = DockStyle.Right, .Margin = New Padding(0, 0, 8, 0)}
        AddHandler refresh.Click, Sub() LoadProposals()
        toolbar.Controls.Add(submit)
        toolbar.Controls.Add(refresh)
        layout.Controls.Add(toolbar, 0, 1)

        Dim content As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 2}
        content.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        content.RowStyles.Add(New RowStyle(SizeType.Absolute, 86.0F))
        ConfigureGrid()
        content.Controls.Add(grid, 0, 0)
        remarks.Multiline = True
        remarks.ReadOnly = True
        remarks.BackColor = SystemColors.Window
        remarks.Dock = DockStyle.Fill
        remarks.ScrollBars = ScrollBars.Vertical
        content.Controls.Add(remarks, 0, 1)
        layout.Controls.Add(content, 0, 2)
        Controls.Add(layout)
        AddHandler grid.SelectionChanged, AddressOf GridSelectionChanged
    End Sub

    Private Sub ConfigureGrid()
        grid.Dock = DockStyle.Fill
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.ReadOnly = True
        grid.RowHeadersVisible = False
        grid.AutoGenerateColumns = False
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        grid.ColumnHeadersHeight = 40
        grid.RowTemplate.Height = 46
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        grid.GridColor = Color.FromArgb(235, 235, 235)
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        grid.Columns.Add("title", "TITLE")
        grid.Columns.Add("semester", "SEMESTER")
        grid.Columns.Add("status", "STATUS")
        grid.Columns.Add("submitted", "SUBMITTED")
    End Sub

    Private Sub LoadProposals()
        Try
            grid.Rows.Clear()
            remarks.Clear()
            Dim proposals As List(Of Proposal) = ProposalService.GetMyProposals(Session.CurrentUser.UserId)
            For Each proposal As Proposal In proposals
                Dim rowIndex As Integer = grid.Rows.Add(proposal.Title, proposal.Semester, FormatStatus(proposal.Status), proposal.SubmittedAt.ToString("MMM d, yyyy"))
                grid.Rows(rowIndex).Tag = proposal.ProposalId
            Next
            If grid.Rows.Count = 0 Then remarks.Text = "You have not submitted any proposals yet."
        Catch ex As Exception
            MessageBox.Show("Could not load your proposals: " & ex.Message, "My Proposals", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub GridSelectionChanged(sender As Object, e As EventArgs)
        If grid.SelectedRows.Count = 0 Then Return
        Try
            Dim log As StatusLogEntry = ProposalService.GetLatestLog(CInt(grid.SelectedRows(0).Tag))
            remarks.Text = If(log Is Nothing OrElse String.IsNullOrWhiteSpace(log.Remarks), "No administrator remarks yet.", log.Remarks)
        Catch ex As Exception
            remarks.Text = "Remarks are currently unavailable."
        End Try
    End Sub

    Private Sub SubmitProposal(sender As Object, e As EventArgs)
        Using dialog As New frmSubmitProposal()
            dialog.ShowDialog(FindForm())
        End Using
        LoadProposals()
    End Sub

    Private Shared Function FormatStatus(status As ProposalStatus) As String
        Return If(status = ProposalStatus.ForRevision, "For Revision", status.ToString())
    End Function
End Class
