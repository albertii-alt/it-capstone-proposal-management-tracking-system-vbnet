Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Drawing.Drawing2D
Imports System.Linq

Public Class ucManageProposals
    Inherits UserControl

    Private _proposals As New List(Of Proposal)
    Private _isLoading As Boolean = False
    Private _activeStatusFilter As Nullable(Of ProposalStatus) = Nothing

    Private WithEvents lblHeader As New Label()
    Private ReadOnly pnlFilterToolbar As New Panel()
    Private ReadOnly flpStatusPills As New FlowLayoutPanel()
    Private ReadOnly flpFilterDropdowns As New FlowLayoutPanel()
    Private WithEvents btnFilterAll As New Button()
    Private WithEvents btnFilterPending As New Button()
    Private WithEvents btnFilterApproved As New Button()
    Private WithEvents btnFilterRevision As New Button()
    Private WithEvents btnFilterRejected As New Button()
    Private WithEvents cboYearLevelFilter As New ComboBox()
    Private WithEvents cboSemesterFilter As New ComboBox()
    Private WithEvents cboSortBy As New ComboBox()
    Private WithEvents dgvProposals As New DataGridView()
    Private ReadOnly colActions As New DataGridViewButtonColumn()
    Private WithEvents lblDetailsField As New Label()
    Private WithEvents txtDetails As New TextBox()
    Private WithEvents lblStatusField As New Label()
    Private WithEvents cboStatus As New ComboBox()
    Private WithEvents lblRemarksField As New Label()
    Private WithEvents txtRemarks As New TextBox()
    Private WithEvents btnSaveDecision As New Button()
    Private WithEvents btnRefresh As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim headerPanel As New Panel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.Name = "ucManageProposals"

        layout.Name = "tlpManageProposals"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 3
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 32.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 80.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Manage Proposals"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)

        headerPanel.Name = "pnlManageProposalsHeader"
        headerPanel.Dock = DockStyle.Fill

        btnRefresh.Name = "btnRefresh"
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(110, 28)
        btnRefresh.Dock = DockStyle.Right
        btnRefresh.UseVisualStyleBackColor = True
        headerPanel.Controls.Add(lblHeader)
        headerPanel.Controls.Add(btnRefresh)

        pnlFilterToolbar.Name = "pnlFilterToolbar"
        pnlFilterToolbar.Dock = DockStyle.Top
        pnlFilterToolbar.Height = 80
        pnlFilterToolbar.BackColor = SystemColors.Control

        flpStatusPills.Name = "flpStatusPills"
        flpStatusPills.Dock = DockStyle.Top
        flpStatusPills.Height = 36
        flpStatusPills.FlowDirection = FlowDirection.LeftToRight
        flpStatusPills.WrapContents = False
        flpStatusPills.Padding = New Padding(0, 3, 0, 0)

        ConfigureFilterButton(btnFilterAll, "btnFilterAll", "All", 100, True)
        ConfigureFilterButton(btnFilterPending, "btnFilterPending", "Pending", 100, False)
        ConfigureFilterButton(btnFilterApproved, "btnFilterApproved", "Approved", 100, False)
        ConfigureFilterButton(btnFilterRevision, "btnFilterRevision", "For Revision", 120, False)
        ConfigureFilterButton(btnFilterRejected, "btnFilterRejected", "Rejected", 100, False)

        flpStatusPills.Controls.Add(btnFilterAll)
        flpStatusPills.Controls.Add(btnFilterPending)
        flpStatusPills.Controls.Add(btnFilterApproved)
        flpStatusPills.Controls.Add(btnFilterRevision)
        flpStatusPills.Controls.Add(btnFilterRejected)

        flpFilterDropdowns.Name = "flpFilterDropdowns"
        flpFilterDropdowns.Dock = DockStyle.Top
        flpFilterDropdowns.Height = 36
        flpFilterDropdowns.FlowDirection = FlowDirection.LeftToRight
        flpFilterDropdowns.WrapContents = False
        flpFilterDropdowns.Padding = New Padding(0, 3, 0, 0)

        ConfigureFilterComboBox(cboYearLevelFilter, "cboYearLevelFilter", 150)
        cboYearLevelFilter.Items.AddRange({"All Year Levels", "1st Year", "2nd Year", "3rd Year", "4th Year"})
        cboYearLevelFilter.SelectedIndex = 0

        ConfigureFilterComboBox(cboSemesterFilter, "cboSemesterFilter", 150)
        cboSemesterFilter.Items.AddRange({"All Semesters", "1st Semester", "2nd Semester"})
        cboSemesterFilter.SelectedIndex = 0

        ConfigureFilterComboBox(cboSortBy, "cboSortBy", 160)
        cboSortBy.Items.AddRange({"Newest First", "Oldest First", "Title (A-Z)", "Title (Z-A)"})
        cboSortBy.SelectedIndex = 0

        flpFilterDropdowns.Controls.Add(cboYearLevelFilter)
        flpFilterDropdowns.Controls.Add(cboSemesterFilter)
        flpFilterDropdowns.Controls.Add(cboSortBy)
        pnlFilterToolbar.Controls.Add(flpFilterDropdowns)
        pnlFilterToolbar.Controls.Add(flpStatusPills)

        dgvProposals.Name = "dgvProposals"
        dgvProposals.Dock = DockStyle.Fill
        dgvProposals.AllowUserToAddRows = False
        dgvProposals.AllowUserToDeleteRows = False
        dgvProposals.AllowUserToResizeRows = False
        dgvProposals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvProposals.BackgroundColor = SystemColors.Window
        dgvProposals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvProposals.MultiSelect = False
        dgvProposals.ReadOnly = True
        dgvProposals.RowHeadersVisible = False
        dgvProposals.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        layout.Controls.Add(headerPanel, 0, 0)
        layout.Controls.Add(pnlFilterToolbar, 0, 1)
        layout.Controls.Add(dgvProposals, 0, 2)

        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub

    Private Sub ConfigureFilterButton(filterButton As Button, controlName As String, buttonText As String, buttonWidth As Integer, isActive As Boolean)
        filterButton.Name = controlName
        filterButton.Text = buttonText
        filterButton.FlatStyle = FlatStyle.Flat
        filterButton.FlatAppearance.BorderSize = 0
        filterButton.Size = New Size(buttonWidth, 30)
        filterButton.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        filterButton.Margin = New Padding(0, 0, 8, 0)
        filterButton.BackColor = If(isActive, Color.FromArgb(122, 31, 46), Color.FromArgb(235, 235, 235))
        filterButton.ForeColor = If(isActive, Color.White, Color.FromArgb(60, 60, 60))
    End Sub

    Private Sub ConfigureFilterComboBox(filterComboBox As ComboBox, controlName As String, controlWidth As Integer)
        filterComboBox.Name = controlName
        filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList
        filterComboBox.Width = controlWidth
        filterComboBox.Margin = New Padding(0, 0, 12, 0)
    End Sub

    Private Sub SetActiveFilterButton(activeButton As Button, statusFilter As Nullable(Of ProposalStatus))
        Dim filterButtons As Button() = {
            btnFilterAll,
            btnFilterPending,
            btnFilterApproved,
            btnFilterRevision,
            btnFilterRejected
        }

        For Each filterButton As Button In filterButtons
            Dim isActive As Boolean = filterButton Is activeButton
            filterButton.BackColor = If(isActive, Color.FromArgb(122, 31, 46), Color.FromArgb(235, 235, 235))
            filterButton.ForeColor = If(isActive, Color.White, Color.FromArgb(60, 60, 60))
        Next

        _activeStatusFilter = statusFilter
    End Sub

    Private Sub btnFilterAll_Click(sender As Object, e As EventArgs) Handles btnFilterAll.Click
        SetActiveFilterButton(btnFilterAll, Nothing)
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub btnFilterPending_Click(sender As Object, e As EventArgs) Handles btnFilterPending.Click
        SetActiveFilterButton(btnFilterPending, New Nullable(Of ProposalStatus)(ProposalStatus.Pending))
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub btnFilterApproved_Click(sender As Object, e As EventArgs) Handles btnFilterApproved.Click
        SetActiveFilterButton(btnFilterApproved, New Nullable(Of ProposalStatus)(ProposalStatus.Approved))
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub btnFilterRevision_Click(sender As Object, e As EventArgs) Handles btnFilterRevision.Click
        SetActiveFilterButton(btnFilterRevision, New Nullable(Of ProposalStatus)(ProposalStatus.ForRevision))
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub btnFilterRejected_Click(sender As Object, e As EventArgs) Handles btnFilterRejected.Click
        SetActiveFilterButton(btnFilterRejected, New Nullable(Of ProposalStatus)(ProposalStatus.Rejected))
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub cboYearLevelFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboYearLevelFilter.SelectedIndexChanged
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub cboSemesterFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSemesterFilter.SelectedIndexChanged
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub cboSortBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSortBy.SelectedIndexChanged
        ApplyFiltersAndSort(0)
    End Sub

    Private Sub ucManageProposals_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cboStatus.Items.AddRange({"Pending", "Approved", "For Revision", "Rejected"})

        SetupGridColumns()
        If FetchProposals() Then
            ApplyFiltersAndSort(0)
        End If
    End Sub

    Private Sub SetupGridColumns()
        dgvProposals.AutoGenerateColumns = False
        dgvProposals.Columns.Clear()
        dgvProposals.Columns.Add("colTitle", "Title")
        dgvProposals.Columns.Add("colGroupName", "Group")
        dgvProposals.Columns.Add("colYearLevel", "Year Level")
        dgvProposals.Columns.Add("colSemester", "Semester")
        dgvProposals.Columns.Add("colStatus", "Status")
        dgvProposals.Columns.Add("colSubmitted", "Submitted")

        dgvProposals.EnableHeadersVisualStyles = False
        dgvProposals.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvProposals.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvProposals.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvProposals.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvProposals.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvProposals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvProposals.ColumnHeadersHeight = 40
        dgvProposals.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvProposals.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvProposals.GridColor = Color.FromArgb(235, 235, 235)
        dgvProposals.BorderStyle = BorderStyle.None
        dgvProposals.RowTemplate.Height = 48
        dgvProposals.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvProposals.DefaultCellStyle.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
        dgvProposals.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvProposals.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)

        colActions.Name = "colActions"
        colActions.HeaderText = ""
        colActions.Text = "⋮"
        colActions.UseColumnTextForButtonValue = True
        colActions.Width = 50
        colActions.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colActions.FlatStyle = FlatStyle.Flat
        colActions.SortMode = DataGridViewColumnSortMode.NotSortable
        colActions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgvProposals.Columns.Add(colActions)
    End Sub

    Private Sub dgvProposals_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvProposals.CellPainting
        If e.ColumnIndex <> dgvProposals.Columns("colStatus").Index OrElse e.RowIndex < 0 OrElse e.Value Is Nothing Then
            Return
        End If

        e.PaintBackground(e.ClipBounds, True)

        Dim statusText As String = e.Value.ToString()
        Dim badgeColor As Color
        Dim textColor As Color

        Select Case statusText
            Case "Pending"
                badgeColor = Color.FromArgb(246, 201, 69)
                textColor = Color.FromArgb(122, 31, 46)
            Case "Approved"
                badgeColor = Color.FromArgb(76, 154, 106)
                textColor = Color.White
            Case "For Revision"
                badgeColor = Color.FromArgb(201, 138, 43)
                textColor = Color.White
            Case "Rejected"
                badgeColor = Color.FromArgb(166, 58, 58)
                textColor = Color.White
            Case Else
                e.PaintContent(e.ClipBounds)
                e.Handled = True
                Return
        End Select

        Dim measuredSize As SizeF = e.Graphics.MeasureString(statusText, e.CellStyle.Font)
        Dim badgeWidth As Integer = CInt(Math.Ceiling(measuredSize.Width)) + 28
        Dim badgeHeight As Integer = CInt(Math.Ceiling(measuredSize.Height)) + 12
        Dim badgeBounds As New Rectangle(
            e.CellBounds.Left + 8,
            e.CellBounds.Top + (e.CellBounds.Height - badgeHeight) \ 2,
            badgeWidth,
            badgeHeight)
        Dim cornerDiameter As Integer = Math.Min(20, Math.Min(badgeBounds.Width, badgeBounds.Height))

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using badgePath As New GraphicsPath()
            badgePath.AddArc(badgeBounds.Left, badgeBounds.Top, cornerDiameter, cornerDiameter, 180.0F, 90.0F)
            badgePath.AddArc(badgeBounds.Right - cornerDiameter, badgeBounds.Top, cornerDiameter, cornerDiameter, 270.0F, 90.0F)
            badgePath.AddArc(badgeBounds.Right - cornerDiameter, badgeBounds.Bottom - cornerDiameter, cornerDiameter, cornerDiameter, 0.0F, 90.0F)
            badgePath.AddArc(badgeBounds.Left, badgeBounds.Bottom - cornerDiameter, cornerDiameter, cornerDiameter, 90.0F, 90.0F)
            badgePath.CloseFigure()

            Using badgeBrush As New SolidBrush(badgeColor)
                e.Graphics.FillPath(badgeBrush, badgePath)
            End Using
        End Using

        Using textBrush As New SolidBrush(textColor)
            Using textFormat As New StringFormat()
                textFormat.Alignment = StringAlignment.Center
                textFormat.LineAlignment = StringAlignment.Center
                e.Graphics.DrawString(statusText, e.CellStyle.Font, textBrush, badgeBounds, textFormat)
            End Using
        End Using

        e.Handled = True
    End Sub

    Private Sub dgvProposals_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvProposals.CellContentClick
        If e.ColumnIndex <> colActions.Index OrElse e.RowIndex < 0 Then
            Return
        End If

        Dim proposalId As Integer = CInt(dgvProposals.Rows(e.RowIndex).Tag)
        Dim proposal As Proposal = Nothing

        For Each item As Proposal In _proposals
            If item.ProposalId = proposalId Then
                proposal = item
                Exit For
            End If
        Next

        If proposal Is Nothing Then
            Return
        End If

        Dim contextMenu As New ContextMenuStrip()
        Dim viewReviewItem As New ToolStripMenuItem("View / Review")
        Dim deleteItem As New ToolStripMenuItem("Delete")

        AddHandler viewReviewItem.Click, Sub(menuSender As Object, menuEventArgs As EventArgs)
            Dim reviewDialog As New frmProposalReview(proposal.ProposalId)
            Dim result As DialogResult = reviewDialog.ShowDialog()

            If result = DialogResult.OK Then
                If FetchProposals() Then
                    ApplyFiltersAndSort(proposal.ProposalId)
                End If
            End If
        End Sub
        AddHandler deleteItem.Click, Sub(menuSender As Object, menuEventArgs As EventArgs) MessageBox.Show("Delete: " & proposal.Title)

        contextMenu.Items.Add(viewReviewItem)
        contextMenu.Items.Add(deleteItem)

        Dim cellBounds As Rectangle = dgvProposals.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
        Dim menuLocation As Point = dgvProposals.PointToScreen(New Point(cellBounds.Left, cellBounds.Bottom))
        contextMenu.Show(menuLocation)
    End Sub

    Private Function FetchProposals() As Boolean
        Try
            _proposals = ProposalService.GetAllProposals()
            Return True
        Catch ex As Exception
            MessageBox.Show("Could not load the proposals: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub RenderGrid(proposalsToShow As List(Of Proposal), selectProposalId As Integer)
        _isLoading = True

        Try
            dgvProposals.Rows.Clear()

            For Each proposal As Proposal In proposalsToShow
                Dim rowIndex As Integer = dgvProposals.Rows.Add(
                    proposal.Title,
                    proposal.GroupName,
                    proposal.YearLevel,
                    proposal.Semester,
                    FormatStatus(proposal.Status),
                    proposal.SubmittedAt.ToString("MM/dd/yyyy"))

                dgvProposals.Rows(rowIndex).Tag = proposal.ProposalId
            Next
        Finally
            _isLoading = False
        End Try

        If dgvProposals.Rows.Count = 0 Then
            txtDetails.Text = "There are no proposals yet."
            txtRemarks.Clear()
            cboStatus.SelectedIndex = -1
            Return
        End If

        Dim rowToSelect As Integer = 0

        For i As Integer = 0 To dgvProposals.Rows.Count - 1
            If CInt(dgvProposals.Rows(i).Tag) = selectProposalId Then
                rowToSelect = i
                Exit For
            End If
        Next

        dgvProposals.ClearSelection()
        dgvProposals.Rows(rowToSelect).Selected = True
        ShowSelectedProposal()
    End Sub

    Private Sub ApplyFiltersAndSort(selectProposalId As Integer)
        Dim proposalsToShow As IEnumerable(Of Proposal) = _proposals

        If _activeStatusFilter.HasValue Then
            proposalsToShow = proposalsToShow.Where(Function(proposal) proposal.Status = _activeStatusFilter.Value)
        End If

        If cboYearLevelFilter.SelectedIndex > 0 Then
            Dim selectedYearLevel As String = CStr(cboYearLevelFilter.SelectedItem)
            proposalsToShow = proposalsToShow.Where(
                Function(proposal) String.Equals(proposal.YearLevel, selectedYearLevel, StringComparison.Ordinal))
        End If

        If cboSemesterFilter.SelectedIndex > 0 Then
            Dim selectedSemester As String = CStr(cboSemesterFilter.SelectedItem)
            proposalsToShow = proposalsToShow.Where(
                Function(proposal) String.Equals(proposal.Semester, selectedSemester, StringComparison.Ordinal))
        End If

        Dim sortedProposals As IEnumerable(Of Proposal)
        Select Case cboSortBy.SelectedIndex
            Case 1
                sortedProposals = proposalsToShow.OrderBy(Function(proposal) proposal.SubmittedAt)
            Case 2
                sortedProposals = proposalsToShow.OrderBy(Function(proposal) proposal.Title, StringComparer.Ordinal)
            Case 3
                sortedProposals = proposalsToShow.OrderByDescending(Function(proposal) proposal.Title, StringComparer.Ordinal)
            Case Else
                sortedProposals = proposalsToShow.OrderByDescending(Function(proposal) proposal.SubmittedAt)
        End Select

        RenderGrid(sortedProposals.ToList(), selectProposalId)
    End Sub

    Private Sub LoadProposals(selectProposalId As Integer)
        If FetchProposals() Then
            ApplyFiltersAndSort(selectProposalId)
        End If
    End Sub

    Private Sub dgvProposals_SelectionChanged(sender As Object, e As EventArgs) Handles dgvProposals.SelectionChanged
        If _isLoading Then
            Return
        End If

        ShowSelectedProposal()
    End Sub

    Private Function GetSelectedProposal() As Proposal
        If dgvProposals.SelectedRows.Count = 0 Then
            Return Nothing
        End If

        Dim proposalId As Integer = CInt(dgvProposals.SelectedRows(0).Tag)

        For Each proposal As Proposal In _proposals
            If proposal.ProposalId = proposalId Then
                Return proposal
            End If
        Next

        Return Nothing
    End Function

    Private Sub ShowSelectedProposal()
        Dim proposal As Proposal = GetSelectedProposal()

        If proposal Is Nothing Then
            Return
        End If

        Dim memberNames As New List(Of String)
        For Each member As ProposalMember In proposal.Members
            memberNames.Add(member.MemberName)
        Next

        Dim detailsText As String = proposal.Description & vbCrLf & vbCrLf &
                                    "Group members: " & String.Join(", ", memberNames)

        Try
            Dim similarities As List(Of TitleSimilarity) = ProposalService.GetSimilarityBreakdown(proposal.ProposalId, proposal.Title)
            Dim matches As New List(Of TitleSimilarity)

            For Each item As TitleSimilarity In similarities
                If item.Score > 0 Then
                    matches.Add(item)
                End If

                If matches.Count = 5 Then
                    Exit For
                End If
            Next

            If matches.Count = 0 Then
                detailsText &= vbCrLf & vbCrLf & "Duplicate check: no similar titles found."
            Else
                detailsText &= vbCrLf & vbCrLf & "DUPLICATE CHECK (closest matches):"

                For Each item As TitleSimilarity In matches
                    Dim percent As String = (item.Score * 100).ToString("0") & "%"
                    detailsText &= vbCrLf & " - " & percent & " similar to """ & item.Title & """"
                Next
            End If

        Catch ex As Exception
            detailsText &= vbCrLf & vbCrLf & "Duplicate check unavailable."
        End Try

        cboStatus.Text = FormatStatus(proposal.Status)
        txtRemarks.Clear()
    End Sub

    Private Sub btnSaveDecision_Click(sender As Object, e As EventArgs) Handles btnSaveDecision.Click
        Dim proposal As Proposal = GetSelectedProposal()

        If proposal Is Nothing Then
            MessageBox.Show("Select a proposal first.")
            Return
        End If

        If cboStatus.SelectedIndex = -1 Then
            MessageBox.Show("Choose a status first.")
            Return
        End If

        Dim newStatus As ProposalStatus = ToStatus(cboStatus.Text)

        Dim confirm As DialogResult = MessageBox.Show(
            "Set """ & proposal.Title & """ to " & cboStatus.Text & "?",
            "Confirm Decision",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If confirm = DialogResult.No Then
            Return
        End If

        Try
            ProposalService.ReviewProposal(proposal.ProposalId, newStatus, Session.CurrentUser.UserId, txtRemarks.Text)
            MessageBox.Show("Decision saved.")
            If FetchProposals() Then
                ApplyFiltersAndSort(proposal.ProposalId)
            End If

        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
        Catch ex As Exception
            MessageBox.Show("Something went wrong while saving. Please try again.")
        End Try
    End Sub

    Private Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then
            Return "For Revision"
        End If

        Return status.ToString()
    End Function

    Private Function ToStatus(text As String) As ProposalStatus
        If text = "For Revision" Then
            Return ProposalStatus.ForRevision
        End If

        Return CType([Enum].Parse(GetType(ProposalStatus), text), ProposalStatus)
    End Function

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Dim current As Proposal = GetSelectedProposal()

        If FetchProposals() Then
            ApplyFiltersAndSort(If(current IsNot Nothing, current.ProposalId, 0))
        End If
    End Sub

    Public Sub RefreshData()
        btnRefresh_Click(btnRefresh, EventArgs.Empty)
    End Sub

End Class