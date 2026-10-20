Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports ScottPlot.WinForms

Public Class ucReports
    Inherits UserControl

    Private _allProposals As New List(Of Proposal)
    Private _filteredProposals As New List(Of Proposal)
    Private _currentPageIndex As Integer = 0
    Private _pageSize As Integer = 25
    Private _pageControlsReady As Boolean = False

    Private WithEvents btnExportCsv As New Button()
    Private WithEvents btnRefresh As New Button()
    Private WithEvents btnPreviousPage As New Button()
    Private WithEvents btnNextPage As New Button()
    Private WithEvents cboYearLevelFilter As New ComboBox()
    Private WithEvents cboSemesterFilter As New ComboBox()
    Private WithEvents cboStatusFilter As New ComboBox()
    Private WithEvents txtSearchFilter As New TextBox()
    Private WithEvents chkSubmittedFrom As New CheckBox()
    Private WithEvents dtpSubmittedFrom As New DateTimePicker()
    Private WithEvents chkSubmittedThrough As New CheckBox()
    Private WithEvents dtpSubmittedThrough As New DateTimePicker()
    Private WithEvents cboPageSize As New ComboBox()
    Private WithEvents dgvReportData As New DataGridView()
    Private ReadOnly pnlPagination As New Panel()
    Private ReadOnly flpPaginationControls As New FlowLayoutPanel()
    Private WithEvents lblPaginationSummary As New Label()
    Private WithEvents lblPageIndicator As New Label()
    Private ReadOnly lblTotalProposalsCount As New Label()
    Private ReadOnly lblAverageSimilarityCount As New Label()
    Private ReadOnly lblApprovalRateCount As New Label()
    Private ReadOnly plotByStatus As New FormsPlot()
    Private ReadOnly plotByYearLevel As New FormsPlot()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim headerPanel As New Panel()
        Dim headerTextLayout As New TableLayoutPanel()
        Dim headerActions As New FlowLayoutPanel()
        Dim lblHeader As New Label()
        Dim lblHeaderSubtitle As New Label()
        Dim summaryPanel As New TableLayoutPanel()
        Dim chartsPanel As New TableLayoutPanel()
        Dim filterPanel As New FlowLayoutPanel()
        Dim reportToolbar As New TableLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Name = "ucReports"
        Me.Padding = New Padding(20)

        dgvReportData.Name = "dgvReportData"
        dgvReportData.Dock = DockStyle.Fill
        dgvReportData.AllowUserToAddRows = False
        dgvReportData.AllowUserToDeleteRows = False
        dgvReportData.AllowUserToResizeRows = False
        dgvReportData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        dgvReportData.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        dgvReportData.BackgroundColor = SystemColors.Window
        dgvReportData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvReportData.MultiSelect = False
        dgvReportData.ReadOnly = True
        dgvReportData.RowHeadersVisible = False
        dgvReportData.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        filterPanel.Name = "flpReportFilters"
        filterPanel.Dock = DockStyle.Top
        filterPanel.Height = 44
        filterPanel.FlowDirection = FlowDirection.LeftToRight
        filterPanel.WrapContents = True
        filterPanel.Padding = New Padding(0, 6, 4, 0)

        ConfigureFilterComboBox(cboYearLevelFilter, "cboYearLevelFilter", 120)
        ConfigureFilterComboBox(cboSemesterFilter, "cboSemesterFilter", 120)
        ConfigureFilterComboBox(cboStatusFilter, "cboStatusFilter", 120)
        txtSearchFilter.Name = "txtSearchFilter"
        txtSearchFilter.Width = 210
        txtSearchFilter.Height = 28
        txtSearchFilter.Margin = New Padding(0, 0, 12, 0)
        txtSearchFilter.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        txtSearchFilter.PlaceholderText = "Search title or group"

        ConfigureDateFilterCheckBox(chkSubmittedFrom, "chkSubmittedFrom", "From")

        ConfigureDateFilter(dtpSubmittedFrom, "dtpSubmittedFrom")
        dtpSubmittedFrom.Margin = New Padding(0, 0, 10, 0)

        ConfigureDateFilterCheckBox(chkSubmittedThrough, "chkSubmittedThrough", "Through")

        ConfigureDateFilter(dtpSubmittedThrough, "dtpSubmittedThrough")

        filterPanel.Controls.Add(cboYearLevelFilter)
        filterPanel.Controls.Add(cboSemesterFilter)
        filterPanel.Controls.Add(cboStatusFilter)
        filterPanel.Controls.Add(txtSearchFilter)
        filterPanel.Controls.Add(chkSubmittedFrom)
        filterPanel.Controls.Add(dtpSubmittedFrom)
        filterPanel.Controls.Add(chkSubmittedThrough)
        filterPanel.Controls.Add(dtpSubmittedThrough)

        reportToolbar.Name = "tlpReportToolbar"
        reportToolbar.Dock = DockStyle.Top
        reportToolbar.Height = 82
        reportToolbar.ColumnCount = 2
        reportToolbar.RowCount = 1
        reportToolbar.Margin = New Padding(0)
        reportToolbar.Padding = New Padding(0)
        reportToolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        reportToolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 240.0F))
        reportToolbar.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        pnlPagination.Name = "pnlReportPagination"
        pnlPagination.Dock = DockStyle.Bottom
        pnlPagination.Height = 44
        pnlPagination.BackColor = SystemColors.Control

        lblPaginationSummary.Name = "lblPaginationSummary"
        lblPaginationSummary.Dock = DockStyle.Left
        lblPaginationSummary.Width = 250
        lblPaginationSummary.Text = "Showing 0 proposals"
        lblPaginationSummary.ForeColor = Color.DimGray
        lblPaginationSummary.TextAlign = ContentAlignment.MiddleLeft

        flpPaginationControls.Name = "flpReportPaginationControls"
        flpPaginationControls.Dock = DockStyle.Right
        flpPaginationControls.Width = 330
        flpPaginationControls.FlowDirection = FlowDirection.LeftToRight
        flpPaginationControls.WrapContents = False
        flpPaginationControls.Padding = New Padding(0, 7, 0, 0)

        cboPageSize.Name = "cboPageSize"
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList
        cboPageSize.Width = 82
        cboPageSize.Items.AddRange({10, 25, 50, 100})
        cboPageSize.SelectedItem = 25
        cboPageSize.Margin = New Padding(0, 0, 8, 0)

        btnPreviousPage.Name = "btnPreviousPage"
        btnPreviousPage.Text = "Previous"
        btnPreviousPage.Size = New Size(78, 28)
        btnPreviousPage.Enabled = False
        btnPreviousPage.UseVisualStyleBackColor = True
        btnPreviousPage.Margin = New Padding(0, 0, 8, 0)

        lblPageIndicator.Name = "lblPageIndicator"
        lblPageIndicator.Size = New Size(76, 28)
        lblPageIndicator.Text = "Page 0 of 0"
        lblPageIndicator.TextAlign = ContentAlignment.MiddleCenter
        lblPageIndicator.ForeColor = Color.DimGray
        lblPageIndicator.Margin = New Padding(0, 0, 8, 0)

        btnNextPage.Name = "btnNextPage"
        btnNextPage.Text = "Next"
        btnNextPage.Size = New Size(62, 28)
        btnNextPage.Enabled = False
        btnNextPage.UseVisualStyleBackColor = True

        flpPaginationControls.Controls.Add(cboPageSize)
        flpPaginationControls.Controls.Add(btnPreviousPage)
        flpPaginationControls.Controls.Add(lblPageIndicator)
        flpPaginationControls.Controls.Add(btnNextPage)
        pnlPagination.Controls.Add(flpPaginationControls)
        pnlPagination.Controls.Add(lblPaginationSummary)

        chartsPanel.Name = "tlpReportCharts"
        chartsPanel.Dock = DockStyle.Top
        chartsPanel.Height = 250
        chartsPanel.ColumnCount = 2
        chartsPanel.RowCount = 1
        chartsPanel.Padding = New Padding(0, 4, 0, 8)
        chartsPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        chartsPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        chartsPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        plotByStatus.Name = "plotByStatus"
        plotByStatus.Dock = DockStyle.Fill
        plotByStatus.MinimumSize = New Size(0, 220)
        plotByStatus.Margin = New Padding(0, 0, 8, 0)
        ConfigureReportPlot(plotByStatus)

        plotByYearLevel.Name = "plotByYearLevel"
        plotByYearLevel.Dock = DockStyle.Fill
        plotByYearLevel.MinimumSize = New Size(0, 220)
        plotByYearLevel.Margin = New Padding(8, 0, 0, 0)
        ConfigureReportPlot(plotByYearLevel)
        chartsPanel.Controls.Add(plotByStatus, 0, 0)
        chartsPanel.Controls.Add(plotByYearLevel, 1, 0)

        summaryPanel.Name = "tlpReportSummaryCards"
        summaryPanel.Dock = DockStyle.Top
        summaryPanel.Height = 100
        summaryPanel.ColumnCount = 3
        summaryPanel.RowCount = 1
        summaryPanel.Padding = New Padding(0, 0, 0, 4)
        summaryPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.3333F))
        summaryPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.3333F))
        summaryPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.3334F))
        summaryPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        summaryPanel.Controls.Add(CreateSummaryCard("Total Proposals", lblTotalProposalsCount, Color.FromArgb(122, 31, 46)), 0, 0)
        summaryPanel.Controls.Add(CreateSummaryCard("Avg. Duplicate Similarity", lblAverageSimilarityCount, Color.FromArgb(201, 138, 43)), 1, 0)
        summaryPanel.Controls.Add(CreateSummaryCard("Approval Rate", lblApprovalRateCount, Color.FromArgb(246, 201, 69)), 2, 0)

        headerPanel.Name = "pnlReportsHeader"
        headerPanel.Dock = DockStyle.Top
        headerPanel.Height = 40

        headerTextLayout.Name = "tlpReportsHeaderText"
        headerTextLayout.Dock = DockStyle.Fill
        headerTextLayout.ColumnCount = 1
        headerTextLayout.RowCount = 2
        headerTextLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 0.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Reports"
        lblHeader.Visible = False
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)

        lblHeaderSubtitle.Name = "lblHeaderSubtitle"
        lblHeaderSubtitle.Text = "View proposal statistics and export filtered report data."
        lblHeaderSubtitle.Dock = DockStyle.Fill
        lblHeaderSubtitle.TextAlign = ContentAlignment.MiddleLeft
        lblHeaderSubtitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblHeaderSubtitle.ForeColor = Color.DimGray
        headerTextLayout.Controls.Add(lblHeader, 0, 0)
        headerTextLayout.Controls.Add(lblHeaderSubtitle, 0, 1)

        headerActions.Name = "flpReportHeaderActions"
        headerActions.Dock = DockStyle.Right
        headerActions.Width = 240
        headerActions.FlowDirection = FlowDirection.RightToLeft
        headerActions.WrapContents = False
        headerActions.Padding = New Padding(0, 6, 0, 0)

        btnRefresh.Name = "btnRefresh"
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(92, 28)
        btnRefresh.Margin = New Padding(8, 0, 0, 0)
        btnRefresh.UseVisualStyleBackColor = True

        btnExportCsv.Name = "btnExportCsv"
        btnExportCsv.Text = "Export to CSV"
        btnExportCsv.Size = New Size(124, 28)
        btnExportCsv.FlatStyle = FlatStyle.Flat
        btnExportCsv.FlatAppearance.BorderSize = 0
        btnExportCsv.BackColor = Color.FromArgb(122, 31, 46)
        btnExportCsv.ForeColor = Color.White
        btnExportCsv.UseVisualStyleBackColor = False

        headerActions.Controls.Add(btnRefresh)
        headerActions.Controls.Add(btnExportCsv)
        reportToolbar.Controls.Add(filterPanel, 0, 0)
        reportToolbar.Controls.Add(headerActions, 1, 0)
        headerPanel.Controls.Add(headerTextLayout)

        Me.Controls.Add(dgvReportData)
        Me.Controls.Add(pnlPagination)
        Me.Controls.Add(reportToolbar)
        Me.Controls.Add(chartsPanel)
        Me.Controls.Add(summaryPanel)
        Me.ResumeLayout(False)
        _pageControlsReady = True
    End Sub

    Private Function CreateSummaryCard(title As String, countLabel As Label, accentColor As Color) As Panel
        Dim card As New Panel()
        Dim accentStrip As New Panel()
        Dim titleLabel As New Label()

        card.BackColor = SystemColors.Window
        card.BorderStyle = BorderStyle.FixedSingle
        card.Dock = DockStyle.Fill
        card.Margin = New Padding(8, 6, 8, 6)

        accentStrip.Dock = DockStyle.Left
        accentStrip.Width = 4
        accentStrip.BackColor = accentColor

        titleLabel.AutoSize = True
        titleLabel.Location = New Point(16, 12)
        titleLabel.Text = title
        titleLabel.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
        titleLabel.ForeColor = Color.DimGray

        countLabel.AutoSize = True
        countLabel.Location = New Point(16, 39)
        countLabel.Font = New Font("Segoe UI", 22.0F, FontStyle.Bold)
        countLabel.ForeColor = Color.FromArgb(45, 45, 45)
        countLabel.Text = "0"

        card.Controls.Add(accentStrip)
        card.Controls.Add(titleLabel)
        card.Controls.Add(countLabel)
        Return card
    End Function

    Private Sub ConfigureReportPlot(reportPlot As FormsPlot)
        reportPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromColor(SystemColors.Window)
        reportPlot.Plot.DataBackground.Color = ScottPlot.Color.FromColor(SystemColors.Window)
        reportPlot.Plot.Axes.Left.Min = 0
        reportPlot.Plot.Axes.Left.MajorTickStyle.Length = 0
        reportPlot.Plot.Axes.Bottom.MajorTickStyle.Length = 0
        reportPlot.Plot.Axes.Margins(0.04, 0.12)
    End Sub

    Private Sub ConfigureFilterComboBox(filterComboBox As ComboBox, controlName As String, controlWidth As Integer)
        filterComboBox.Name = controlName
        filterComboBox.DropDownStyle = ComboBoxStyle.DropDownList
        filterComboBox.Width = controlWidth
        filterComboBox.Margin = New Padding(0, 0, 12, 0)
    End Sub

    Private Sub ConfigureDateFilter(dateFilter As DateTimePicker, controlName As String)
        dateFilter.Name = controlName
        dateFilter.Format = DateTimePickerFormat.Custom
        dateFilter.CustomFormat = "MMM d, yyyy"
        dateFilter.Enabled = False
        dateFilter.Width = 128
        dateFilter.Height = 28
        dateFilter.Margin = New Padding(0, 0, 10, 0)
    End Sub

    Private Sub ConfigureDateFilterCheckBox(dateFilterCheckBox As CheckBox, controlName As String, label As String)
        dateFilterCheckBox.Name = controlName
        dateFilterCheckBox.Text = label
        dateFilterCheckBox.AutoSize = True
        dateFilterCheckBox.Height = 28
        dateFilterCheckBox.Margin = New Padding(0, 0, 4, 0)
        dateFilterCheckBox.TextAlign = ContentAlignment.MiddleLeft
        dateFilterCheckBox.UseVisualStyleBackColor = True
    End Sub

    Private Sub SetupGridColumns()
        dgvReportData.AutoGenerateColumns = False
        dgvReportData.Columns.Clear()
        dgvReportData.Columns.Add("colTitle", "Title")
        dgvReportData.Columns.Add("colGroupName", "Group")
        dgvReportData.Columns.Add("colYearLevel", "Year Level")
        dgvReportData.Columns.Add("colSemester", "Semester")
        dgvReportData.Columns.Add("colStatus", "Status")
        dgvReportData.Columns.Add("colSubmitted", "Submitted")

        dgvReportData.Columns("colTitle").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvReportData.Columns("colTitle").MinimumWidth = 260
        dgvReportData.Columns("colTitle").DefaultCellStyle.WrapMode = DataGridViewTriState.True
        dgvReportData.Columns("colGroupName").Width = 115
        dgvReportData.Columns("colYearLevel").Width = 95
        dgvReportData.Columns("colSemester").Width = 105
        dgvReportData.Columns("colStatus").Width = 105
        dgvReportData.Columns("colSubmitted").Width = 145

        dgvReportData.EnableHeadersVisualStyles = False
        dgvReportData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvReportData.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvReportData.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvReportData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvReportData.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvReportData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvReportData.ColumnHeadersHeight = 40
        dgvReportData.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvReportData.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvReportData.GridColor = Color.FromArgb(235, 235, 235)
        dgvReportData.BorderStyle = BorderStyle.None
        dgvReportData.RowTemplate.Height = 48
        dgvReportData.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvReportData.DefaultCellStyle.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
        dgvReportData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvReportData.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)
    End Sub

    Private Sub ucReports_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGridColumns()
        cboYearLevelFilter.Items.AddRange({"All Year Levels", "1st Year", "2nd Year", "3rd Year", "4th Year"})
        cboSemesterFilter.Items.AddRange({"All Semesters", "1st Semester", "2nd Semester"})
        cboStatusFilter.Items.AddRange({"All Statuses", "Pending", "Approved", "For Revision", "Rejected"})
        cboYearLevelFilter.SelectedIndex = 0
        cboSemesterFilter.SelectedIndex = 0
        cboStatusFilter.SelectedIndex = 0
        LoadReportData()
    End Sub

    Private Sub LoadReportData()
        Try
            _allProposals = ProposalService.GetAllProposals()
        Catch ex As Exception
            MessageBox.Show("Could not load the proposals: " & ex.Message)
            Return
        End Try

        ApplyFiltersToGrid()
    End Sub

    Private Sub UpdateSummaryCards(proposals As List(Of Proposal))
        Dim totalCount As Integer = proposals.Count
        lblTotalProposalsCount.Text = totalCount.ToString()

        If totalCount < 2 Then
            lblAverageSimilarityCount.Text = "N/A"
        Else
            Dim strongestSimilarityTotal As Double = 0.0R

            For Each proposal As Proposal In proposals
                Dim otherProposals As List(Of ProposalTitleInfo) = proposals _
                    .Where(Function(candidate) Not Object.ReferenceEquals(candidate, proposal)) _
                    .Select(Function(candidate) New ProposalTitleInfo With {
                        .Title = If(candidate.Title, String.Empty),
                        .GroupName = candidate.GroupName,
                        .SubmittedAt = candidate.SubmittedAt
                    }) _
                    .ToList()
                Dim similarities As List(Of TitleSimilarity) = DuplicateChecker.GetAllSimilarities(
                    If(proposal.Title, String.Empty),
                    otherProposals)
                strongestSimilarityTotal += similarities(0).Score
            Next

            Dim averageSimilarity As Double = strongestSimilarityTotal / totalCount
            lblAverageSimilarityCount.Text = CInt(Math.Round(averageSimilarity * 100.0R)).ToString() & "%"
        End If

        If totalCount = 0 Then
            lblApprovalRateCount.Text = "N/A"
        Else
            Dim approvedCount As Integer = proposals.Where(Function(proposal) proposal.Status = ProposalStatus.Approved).Count()
            lblApprovalRateCount.Text = CInt(Math.Round(approvedCount / CDbl(totalCount) * 100.0R)).ToString() & "%"
        End If
    End Sub

    Private Sub UpdateCharts(proposals As List(Of Proposal))
        Dim statuses As ProposalStatus() = {
            ProposalStatus.Pending,
            ProposalStatus.Approved,
            ProposalStatus.ForRevision,
            ProposalStatus.Rejected
        }
        Dim statusLabels As String() = {"Pending", "Approved", "For Revision", "Rejected"}
        Dim statusColors As Color() = {
            Color.FromArgb(246, 201, 69),
            Color.FromArgb(76, 154, 106),
            Color.FromArgb(201, 138, 43),
            Color.FromArgb(166, 58, 58)
        }
        Dim statusCounts As Integer() = statuses _
            .Select(Function(status) proposals.Where(Function(proposal) proposal.Status = status).Count()) _
            .ToArray()

        plotByStatus.Plot.Clear()
        For index As Integer = 0 To statuses.Length - 1
            Dim bar As New ScottPlot.Bar With {
                .Position = index,
                .Value = statusCounts(index),
                .Size = 0.7,
                .FillColor = ScottPlot.Color.FromColor(statusColors(index))
            }
            plotByStatus.Plot.Add.Bar(bar)
        Next
        plotByStatus.Plot.Title("Proposals by Status")
        plotByStatus.Plot.YLabel("Count")
        plotByStatus.Plot.Axes.Bottom.SetTicks({0.0R, 1.0R, 2.0R, 3.0R}, statusLabels)
        SetReportPlotScale(plotByStatus, statusCounts.Max())
        plotByStatus.Refresh()

        Dim yearLevels As String() = {"1st Year", "2nd Year", "3rd Year", "4th Year"}
        Dim yearLevelCounts As Integer() = yearLevels _
            .Select(Function(yearLevel) proposals.Where(
                Function(proposal) String.Equals(proposal.YearLevel, yearLevel, StringComparison.Ordinal)).Count()) _
            .ToArray()

        plotByYearLevel.Plot.Clear()
        For index As Integer = 0 To yearLevels.Length - 1
            Dim bar As New ScottPlot.Bar With {
                .Position = index,
                .Value = yearLevelCounts(index),
                .Size = 0.7,
                .FillColor = ScottPlot.Color.FromColor(Color.FromArgb(122, 31, 46))
            }
            plotByYearLevel.Plot.Add.Bar(bar)
        Next
        plotByYearLevel.Plot.Title("Proposals by Year Level")
        plotByYearLevel.Plot.YLabel("Count")
        Dim yearLevelTickPositions As Double() = Enumerable.Range(0, yearLevels.Length) _
            .Select(Function(index) CDbl(index)).ToArray()
        plotByYearLevel.Plot.Axes.Bottom.SetTicks(yearLevelTickPositions, yearLevels)
        SetReportPlotScale(plotByYearLevel, yearLevelCounts.Max())
        plotByYearLevel.Refresh()
    End Sub

    Private Sub SetReportPlotScale(reportPlot As FormsPlot, highestBarValue As Integer)
        reportPlot.Plot.Axes.AutoScale()
        reportPlot.Plot.Axes.Bottom.Min = -0.5
        reportPlot.Plot.Axes.Bottom.Max = 3.5
        reportPlot.Plot.Axes.Left.Min = 0
        reportPlot.Plot.Axes.Left.Max = Math.Max(1, highestBarValue) * 1.2
    End Sub

    Private Sub ApplyFiltersToGrid()
        Dim proposalsToShow As IEnumerable(Of Proposal) = _allProposals

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

        If cboStatusFilter.SelectedIndex > 0 Then
            Dim selectedStatus As String = CStr(cboStatusFilter.SelectedItem)
            proposalsToShow = proposalsToShow.Where(Function(proposal) FormatStatus(proposal.Status) = selectedStatus)
        End If

        Dim searchText As String = txtSearchFilter.Text.Trim()
        If searchText.Length > 0 Then
            proposalsToShow = proposalsToShow.Where(
                Function(proposal) ContainsIgnoreCase(proposal.Title, searchText) OrElse
                                   ContainsIgnoreCase(proposal.GroupName, searchText))
        End If

        If chkSubmittedFrom.Checked Then
            Dim submittedFrom As DateTime = dtpSubmittedFrom.Value.Date
            proposalsToShow = proposalsToShow.Where(Function(proposal) proposal.SubmittedAt >= submittedFrom)
        End If

        If chkSubmittedThrough.Checked Then
            Dim submittedBefore As DateTime = dtpSubmittedThrough.Value.Date.AddDays(1)
            proposalsToShow = proposalsToShow.Where(Function(proposal) proposal.SubmittedAt < submittedBefore)
        End If

        _filteredProposals = proposalsToShow.ToList()
        _currentPageIndex = 0
        UpdateSummaryCards(_filteredProposals)
        UpdateCharts(_filteredProposals)
        RenderCurrentPage()
    End Sub

    Private Function ContainsIgnoreCase(value As String, searchText As String) As Boolean
        Return If(value, String.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Sub RenderCurrentPage()
        Dim totalCount As Integer = _filteredProposals.Count
        Dim totalPages As Integer = CInt(Math.Ceiling(totalCount / CDbl(_pageSize)))
        _currentPageIndex = Math.Max(0, Math.Min(_currentPageIndex, Math.Max(0, totalPages - 1)))

        Dim firstIndex As Integer = _currentPageIndex * _pageSize
        Dim pageProposals As List(Of Proposal) = _filteredProposals.Skip(firstIndex).Take(_pageSize).ToList()

        dgvReportData.Rows.Clear()
        For Each proposal As Proposal In pageProposals
            dgvReportData.Rows.Add(
                If(proposal.Title, String.Empty),
                If(proposal.GroupName, String.Empty),
                If(proposal.YearLevel, String.Empty),
                If(proposal.Semester, String.Empty),
                FormatStatus(proposal.Status),
                proposal.SubmittedAt.ToString("MM/dd/yyyy h:mm:ss tt"))
        Next

        If totalCount = 0 Then
            lblPaginationSummary.Text = "Showing 0 proposals"
            lblPageIndicator.Text = "Page 0 of 0"
        Else
            Dim firstDisplayed As Integer = firstIndex + 1
            Dim lastDisplayed As Integer = Math.Min(firstIndex + _pageSize, totalCount)
            lblPaginationSummary.Text = "Showing " & firstDisplayed.ToString() & "-" & lastDisplayed.ToString() &
                                        " of " & totalCount.ToString() & " proposals"
            lblPageIndicator.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & totalPages.ToString()
        End If

        btnPreviousPage.Enabled = _currentPageIndex > 0
        btnNextPage.Enabled = _currentPageIndex < totalPages - 1
    End Sub

    Private Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then
            Return "For Revision"
        End If

        Return status.ToString()
    End Function

    Private Sub cboYearLevelFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboYearLevelFilter.SelectedIndexChanged
        ApplyFiltersToGrid()
    End Sub

    Private Sub cboSemesterFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSemesterFilter.SelectedIndexChanged
        ApplyFiltersToGrid()
    End Sub

    Private Sub cboStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStatusFilter.SelectedIndexChanged
        ApplyFiltersToGrid()
    End Sub

    Private Sub txtSearchFilter_TextChanged(sender As Object, e As EventArgs) Handles txtSearchFilter.TextChanged
        ApplyFiltersToGrid()
    End Sub

    Private Sub dtpSubmittedFrom_ValueChanged(sender As Object, e As EventArgs) Handles dtpSubmittedFrom.ValueChanged
        ApplyFiltersToGrid()
    End Sub

    Private Sub chkSubmittedFrom_CheckedChanged(sender As Object, e As EventArgs) Handles chkSubmittedFrom.CheckedChanged
        dtpSubmittedFrom.Enabled = chkSubmittedFrom.Checked
        ApplyFiltersToGrid()
    End Sub

    Private Sub dtpSubmittedThrough_ValueChanged(sender As Object, e As EventArgs) Handles dtpSubmittedThrough.ValueChanged
        ApplyFiltersToGrid()
    End Sub

    Private Sub chkSubmittedThrough_CheckedChanged(sender As Object, e As EventArgs) Handles chkSubmittedThrough.CheckedChanged
        dtpSubmittedThrough.Enabled = chkSubmittedThrough.Checked
        ApplyFiltersToGrid()
    End Sub

    Private Sub cboPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPageSize.SelectedIndexChanged
        If Not _pageControlsReady OrElse cboPageSize.SelectedItem Is Nothing Then
            Return
        End If

        _pageSize = CInt(cboPageSize.SelectedItem)
        _currentPageIndex = 0
        RenderCurrentPage()
    End Sub

    Private Sub btnPreviousPage_Click(sender As Object, e As EventArgs) Handles btnPreviousPage.Click
        If _currentPageIndex <= 0 Then
            Return
        End If

        _currentPageIndex -= 1
        RenderCurrentPage()
    End Sub

    Private Sub btnNextPage_Click(sender As Object, e As EventArgs) Handles btnNextPage.Click
        Dim totalPages As Integer = CInt(Math.Ceiling(_filteredProposals.Count / CDbl(_pageSize)))
        If _currentPageIndex >= totalPages - 1 Then
            Return
        End If

        _currentPageIndex += 1
        RenderCurrentPage()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadReportData()
    End Sub

    Private Function CsvEscape(value As String) As String
        Dim safeValue As String = If(value, String.Empty)
        If safeValue.Contains(",") OrElse safeValue.Contains(ChrW(34)) OrElse safeValue.Contains(vbCr) OrElse safeValue.Contains(vbLf) Then
            Return ChrW(34) & safeValue.Replace(ChrW(34), New String(ChrW(34), 2)) & ChrW(34)
        End If

        Return safeValue
    End Function

    Private Sub btnExportCsv_Click(sender As Object, e As EventArgs) Handles btnExportCsv.Click
        Using saveDialog As New SaveFileDialog()
            saveDialog.FileName = "ProposalReport_" & DateTime.Now.ToString("yyyyMMdd") & ".csv"
            saveDialog.Filter = "CSV files (*.csv)|*.csv"
            saveDialog.DefaultExt = "csv"
            saveDialog.AddExtension = True

            If saveDialog.ShowDialog(Me.FindForm()) <> DialogResult.OK Then
                Return
            End If

            Try
                Using writer As New StreamWriter(saveDialog.FileName, False, New UTF8Encoding(True))
                    writer.WriteLine("Title,Group,Year Level,Semester,Status,Submitted")
                    For Each proposal As Proposal In _filteredProposals
                        Dim fields As String() = {
                            CsvEscape(If(proposal.Title, String.Empty)),
                            CsvEscape(If(proposal.GroupName, String.Empty)),
                            CsvEscape(If(proposal.YearLevel, String.Empty)),
                            CsvEscape(If(proposal.Semester, String.Empty)),
                            CsvEscape(FormatStatus(proposal.Status)),
                            CsvEscape(proposal.SubmittedAt.ToString("MM/dd/yyyy h:mm:ss tt"))
                        }
                        writer.WriteLine(String.Join(",", fields))
                    Next
                End Using

                If Session.CurrentUser IsNot Nothing AndAlso UserRoleHelper.IsAdministrator(Session.CurrentUser.Role) Then
                    AdminActivityLogService.Record(Session.CurrentUser, "Exported", "Report", Nothing,
                                                   "Proposal report",
                                                   "Exported the proposal report to CSV.",
                                                   "File: " & saveDialog.FileName)
                    SecurityLogService.Record(Session.CurrentUser.UserId, Session.CurrentUser.Username,
                                              "Sensitive Export",
                                              "Exported the proposal report to CSV. File: " & saveDialog.FileName)
                End If
                MessageBox.Show("Report exported successfully to:" & Environment.NewLine & saveDialog.FileName)
            Catch ex As Exception
                MessageBox.Show("Could not export the report: " & ex.Message)
            End Try
        End Using
    End Sub

    Public Sub RefreshData()
        LoadReportData()
    End Sub
End Class