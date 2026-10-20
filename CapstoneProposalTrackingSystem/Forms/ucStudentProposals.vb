Imports System.Collections.Generic
Imports System.Linq

Public Class ucStudentProposals
    Inherits UserControl

    Private ReadOnly _proposals As New List(Of Proposal)()
    Private ReadOnly _filteredProposals As New List(Of Proposal)()
    Private ReadOnly grid As New DataGridView()
    Private ReadOnly pills As New FlowLayoutPanel()
    Private ReadOnly pageSummary As New Label()
    Private ReadOnly pageIndicator As New Label()
    Private ReadOnly pageSizeSelector As New ComboBox()
    Private ReadOnly previousPage As New Button()
    Private ReadOnly nextPage As New Button()
    Private ReadOnly actionColumn As New DataGridViewButtonColumn()
    Private ReadOnly btnRefresh As New Button()
    Private _activeFilter As Nullable(Of ProposalStatus)
    Private _pageIndex As Integer
    Private _pageSize As Integer = 25

    Public Sub New()
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = SystemColors.Control
        BuildLayout()
        LoadProposals()
    End Sub

    Private Sub BuildLayout()
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 3}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        AddFilterPill("All", Nothing)
        AddFilterPill("Pending", ProposalStatus.Pending)
        AddFilterPill("Approved", ProposalStatus.Approved)
        AddFilterPill("For Revision", ProposalStatus.ForRevision)
        AddFilterPill("Rejected", ProposalStatus.Rejected)
        Dim actions As New FlowLayoutPanel()
        Dim submit As New Button With {
            .Text = "Submit New Proposal",
            .Size = New Size(150, 28),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(246, 201, 69),
            .ForeColor = Color.FromArgb(122, 31, 46),
            .UseVisualStyleBackColor = False
        }
        submit.FlatAppearance.BorderSize = 0
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(90, 28)
        btnRefresh.UseVisualStyleBackColor = True
        AddHandler submit.Click, AddressOf SubmitProposal
        AddHandler btnRefresh.Click, Sub(sender, e) LoadProposals()
        layout.Controls.Add(AdminFilterToolbar.Create(actions, pills, btnRefresh, 64, submit), 0, 0)

        ConfigureGrid()
        layout.Controls.Add(grid, 0, 1)
        ConfigurePagination(layout)
        Controls.Add(layout)
    End Sub

    Private Sub AddFilterPill(text As String, status As Nullable(Of ProposalStatus))
        pills.FlowDirection = FlowDirection.LeftToRight
        Dim button As New Button With {.Text = text, .Width = If(text = "For Revision", 120, 100), .Height = 30, .FlatStyle = FlatStyle.Flat, .Margin = New Padding(0, 0, 8, 0), .Tag = status}
        button.FlatAppearance.BorderSize = 0
        AddHandler button.Click, Sub(sender, e)
                                     _activeFilter = DirectCast(DirectCast(sender, Button).Tag, Nullable(Of ProposalStatus))
                                     UpdatePillStyles()
                                     ApplyFilter()
                                 End Sub
        pills.Controls.Add(button)
        UpdatePillStyles()
    End Sub

    Private Sub UpdatePillStyles()
        For Each button As Button In pills.Controls.OfType(Of Button)()
            Dim status As Nullable(Of ProposalStatus) = DirectCast(button.Tag, Nullable(Of ProposalStatus))
            Dim active As Boolean = status.Equals(_activeFilter)
            button.BackColor = If(active, Color.FromArgb(122, 31, 46), Color.FromArgb(235, 235, 235))
            button.ForeColor = If(active, Color.White, Color.FromArgb(60, 60, 60))
        Next
    End Sub

    Private Sub ConfigureGrid()
        grid.Dock = DockStyle.Fill
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.ReadOnly = True
        grid.RowHeadersVisible = False
        grid.AutoGenerateColumns = False
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        grid.ColumnHeadersHeight = 40
        grid.RowTemplate.Height = 48
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        grid.GridColor = Color.FromArgb(235, 235, 235)
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        grid.Columns.Add("title", "TITLE")
        grid.Columns.Add("semester", "SEMESTER")
        grid.Columns.Add("status", "STATUS")
        grid.Columns.Add("submitted", "SUBMITTED")
        actionColumn.Name = "actions"
        actionColumn.HeaderText = ""
        actionColumn.Text = "⋮"
        actionColumn.UseColumnTextForButtonValue = True
        actionColumn.Width = 50
        actionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        actionColumn.FlatStyle = FlatStyle.Flat
        actionColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        grid.Columns.Add(actionColumn)
        AddHandler grid.CellContentClick, AddressOf GridCellContentClick
        AddHandler grid.CellDoubleClick, AddressOf GridCellDoubleClick
    End Sub

    Private Sub ConfigurePagination(layout As TableLayoutPanel)
        Dim footer As New Panel With {.Dock = DockStyle.Fill}
        pageSummary.Dock = DockStyle.Left
        pageSummary.Width = 240
        pageSummary.TextAlign = ContentAlignment.MiddleLeft
        pageSummary.ForeColor = Color.DimGray
        Dim controls As New FlowLayoutPanel With {.Dock = DockStyle.Right, .Width = 330, .WrapContents = False, .Padding = New Padding(0, 7, 0, 0)}
        pageSizeSelector.DropDownStyle = ComboBoxStyle.DropDownList
        pageSizeSelector.Width = 82
        pageSizeSelector.Items.AddRange({10, 25, 50, 100})
        pageSizeSelector.SelectedItem = 25
        previousPage.Text = "Previous"
        previousPage.Size = New Size(78, 28)
        nextPage.Text = "Next"
        nextPage.Size = New Size(62, 28)
        pageIndicator.Size = New Size(76, 28)
        pageIndicator.TextAlign = ContentAlignment.MiddleCenter
        controls.Controls.AddRange({pageSizeSelector, previousPage, pageIndicator, nextPage})
        footer.Controls.Add(controls)
        footer.Controls.Add(pageSummary)
        layout.Controls.Add(footer, 0, 2)
        AddHandler pageSizeSelector.SelectedIndexChanged, AddressOf PageSizeChanged
        AddHandler previousPage.Click, Sub(sender, e) ChangePage(-1)
        AddHandler nextPage.Click, Sub(sender, e) ChangePage(1)
    End Sub

    Private Sub LoadProposals()
        Try
            _proposals.Clear()
            _proposals.AddRange(ProposalService.GetMyProposals(Session.CurrentUser.UserId))
            ApplyFilter()
        Catch ex As Exception
            MessageBox.Show("Could not load your proposals: " & ex.Message, "My Proposals", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ApplyFilter()
        Dim query As IEnumerable(Of Proposal) = _proposals
        If _activeFilter.HasValue Then query = query.Where(Function(item) item.Status = _activeFilter.Value)
        _filteredProposals.Clear()
        _filteredProposals.AddRange(query.OrderByDescending(Function(item) item.SubmittedAt))
        _pageIndex = Math.Max(0, Math.Min(_pageIndex, Math.Max(0, TotalPages() - 1)))
        RenderPage()
    End Sub

    Private Function TotalPages() As Integer
        Return CInt(Math.Ceiling(_filteredProposals.Count / CDbl(_pageSize)))
    End Function

    Private Sub RenderPage()
        grid.Rows.Clear()
        Dim pageItems As List(Of Proposal) = _filteredProposals.Skip(_pageIndex * _pageSize).Take(_pageSize).ToList()
        For Each proposal As Proposal In pageItems
            Dim rowIndex As Integer = grid.Rows.Add(proposal.Title, proposal.Semester, FormatStatus(proposal.Status), proposal.SubmittedAt.ToString("MMM d, yyyy h:mm:ss tt"), "⋮")
            grid.Rows(rowIndex).Tag = proposal.ProposalId
        Next
        If _filteredProposals.Count = 0 Then
            pageSummary.Text = "Showing 0 proposals"
            pageIndicator.Text = "Page 0 of 0"
        Else
            pageSummary.Text = "Showing " & (_pageIndex * _pageSize + 1).ToString() & "-" & Math.Min((_pageIndex + 1) * _pageSize, _filteredProposals.Count).ToString() & " of " & _filteredProposals.Count.ToString() & " proposals"
            pageIndicator.Text = "Page " & (_pageIndex + 1).ToString() & " of " & TotalPages().ToString()
        End If
        previousPage.Enabled = _pageIndex > 0
        nextPage.Enabled = _pageIndex < TotalPages() - 1
    End Sub

    Private Sub PageSizeChanged(sender As Object, e As EventArgs)
        If pageSizeSelector.SelectedItem Is Nothing Then Return
        _pageSize = CInt(pageSizeSelector.SelectedItem)
        _pageIndex = 0
        RenderPage()
    End Sub

    Private Sub ChangePage(offset As Integer)
        Dim target As Integer = _pageIndex + offset
        If target < 0 OrElse target >= TotalPages() Then Return
        _pageIndex = target
        RenderPage()
    End Sub

    Private Function FindProposal(row As DataGridViewRow) As Proposal
        If row Is Nothing OrElse row.Tag Is Nothing Then Return Nothing
        Dim proposalId As Integer = CInt(row.Tag)
        Return _proposals.FirstOrDefault(Function(item) item.ProposalId = proposalId)
    End Function

    Private Sub GridCellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex <> actionColumn.Index Then Return
        ShowActions(FindProposal(grid.Rows(e.RowIndex)), grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False))
    End Sub

    Private Sub GridCellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex = actionColumn.Index Then Return
        ViewProposal(FindProposal(grid.Rows(e.RowIndex)))
    End Sub

    Private Sub ShowActions(proposal As Proposal, cellBounds As Rectangle)
        If proposal Is Nothing Then Return
        Dim menu As New ContextMenuStrip()
        Dim edit As New ToolStripMenuItem("Edit")
        Dim view As New ToolStripMenuItem("View")
        Dim delete As New ToolStripMenuItem("Delete")
        AddHandler edit.Click, Sub(sender, e) EditProposal(proposal)
        AddHandler view.Click, Sub(sender, e) ViewProposal(proposal)
        AddHandler delete.Click, Sub(sender, e) DeleteProposal(proposal)
        menu.Items.AddRange({edit, view, delete})
        menu.Show(grid.PointToScreen(New Point(cellBounds.Left, cellBounds.Bottom)))
    End Sub

    Private Sub EditProposal(proposal As Proposal)
        Using dialog As New frmSubmitProposal(proposal)
            If dialog.ShowDialog(FindForm()) = DialogResult.OK Then LoadProposals()
        End Using
    End Sub

    Private Sub ViewProposal(proposal As Proposal)
        If proposal Is Nothing Then Return
        Using dialog As New frmStudentProposalDetails(proposal)
            dialog.ShowDialog(FindForm())
        End Using
    End Sub

    Private Sub DeleteProposal(proposal As Proposal)
        If proposal Is Nothing Then Return
        Using confirmation As New frmDeleteConfirmation("proposal", proposal.Title)
            If confirmation.ShowDialog(FindForm()) <> DialogResult.OK Then Return
        End Using
        Try
            If Not ProposalService.DeleteStudentProposal(proposal.ProposalId, Session.CurrentUser) Then
                MessageBox.Show("The proposal could not be found or no longer belongs to your account.", "My Proposals", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            LoadProposals()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "My Proposals", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
