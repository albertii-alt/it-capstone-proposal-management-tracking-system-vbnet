Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Linq

Public Class ucTitleArchive
    Inherits UserControl

    Private _archivedTitles As New List(Of ArchivedTitle)
    Private _filteredArchivedTitles As New List(Of ArchivedTitle)
    Private ReadOnly _selectedArchiveIds As New HashSet(Of Integer)
    Private _currentPageIndex As Integer = 0
    Private _pageSize As Integer = 25
    Private _paginationControlsReady As Boolean = False
    Private _updatingSelection As Boolean = False
    Private _selectionMode As Boolean = False

    Private WithEvents lblHeader As New Label()
    Private WithEvents lblHeaderSubtitle As New Label()
    Private WithEvents cboYearLevelFilter As New ComboBox()
    Private WithEvents cboSemesterFilter As New ComboBox()
    Private WithEvents cboSortBy As New ComboBox()
    Private WithEvents lblSearch As New Label()
    Private WithEvents txtSearch As New TextBox()
    Private WithEvents dgvArchive As New DataGridView()
    Private ReadOnly pnlPagination As New Panel()
    Private ReadOnly flpPaginationControls As New FlowLayoutPanel()
    Private WithEvents lblPaginationSummary As New Label()
    Private WithEvents lblPageIndicator As New Label()
    Private WithEvents cboPageSize As New ComboBox()
    Private WithEvents btnPreviousPage As New Button()
    Private WithEvents btnNextPage As New Button()
    Private WithEvents btnRefresh As New Button()
    Private WithEvents btnToggleSelectionMode As New Button()
    Private WithEvents btnDeleteSelected As New Button()
    Private WithEvents chkSelectAll As New CheckBox()
    Private ReadOnly colSelected As New DataGridViewCheckBoxColumn()
    Private ReadOnly colViewDetails As New DataGridViewButtonColumn()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim headerPanel As New Panel()
        Dim headerTextLayout As New TableLayoutPanel()
        Dim filterPanel As New FlowLayoutPanel()
        Dim headerActions As New FlowLayoutPanel()
        Dim paginationControls As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.Name = "ucTitleArchive"

        layout.Name = "tlpTitleArchive"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 4
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 72.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 80.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Title Archive"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)

        lblHeaderSubtitle.Name = "lblHeaderSubtitle"
        lblHeaderSubtitle.Text = "Browse approved titles and search their archived keywords."
        lblHeaderSubtitle.Dock = DockStyle.Fill
        lblHeaderSubtitle.TextAlign = ContentAlignment.MiddleLeft
        lblHeaderSubtitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblHeaderSubtitle.ForeColor = Color.DimGray

        headerTextLayout.Name = "tlpTitleArchiveHeaderText"
        headerTextLayout.Dock = DockStyle.Fill
        headerTextLayout.ColumnCount = 1
        headerTextLayout.RowCount = 2
        headerTextLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        headerTextLayout.Controls.Add(lblHeader, 0, 0)
        headerTextLayout.Controls.Add(lblHeaderSubtitle, 0, 1)

        headerPanel.Name = "pnlTitleArchiveHeader"
        headerPanel.Dock = DockStyle.Fill

        headerActions.Name = "flpTitleArchiveHeaderActions"
        headerActions.Dock = DockStyle.Right
        headerActions.Width = 390
        headerActions.FlowDirection = FlowDirection.RightToLeft
        headerActions.WrapContents = False
        headerActions.Padding = New Padding(0, 2, 0, 0)

        btnRefresh.Name = "btnRefresh"
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(100, 28)
        btnRefresh.Margin = New Padding(8, 0, 0, 0)
        btnRefresh.UseVisualStyleBackColor = True
        headerActions.Controls.Add(btnRefresh)

        btnToggleSelectionMode.Name = "btnToggleSelectionMode"
        btnToggleSelectionMode.Text = "Select"
        btnToggleSelectionMode.Size = New Size(100, 28)
        btnToggleSelectionMode.Margin = New Padding(8, 0, 0, 0)
        btnToggleSelectionMode.UseVisualStyleBackColor = True
        headerActions.Controls.Add(btnToggleSelectionMode)

        btnDeleteSelected.Name = "btnDeleteSelected"
        btnDeleteSelected.Text = "Delete Selected"
        btnDeleteSelected.Size = New Size(150, 28)
        btnDeleteSelected.Margin = New Padding(8, 0, 0, 0)
        btnDeleteSelected.FlatStyle = FlatStyle.Flat
        btnDeleteSelected.FlatAppearance.BorderSize = 0
        btnDeleteSelected.BackColor = Color.FromArgb(166, 58, 58)
        btnDeleteSelected.ForeColor = Color.White
        btnDeleteSelected.UseVisualStyleBackColor = False
        btnDeleteSelected.Enabled = False
        btnDeleteSelected.Visible = False
        headerActions.Controls.Add(btnDeleteSelected)

        headerPanel.Controls.Add(headerTextLayout)
        headerPanel.Controls.Add(headerActions)

        filterPanel.Name = "flpArchiveFilters"
        filterPanel.Dock = DockStyle.Fill
        filterPanel.FlowDirection = FlowDirection.LeftToRight
        filterPanel.WrapContents = True
        filterPanel.Padding = New Padding(0, 8, 0, 0)

        cboYearLevelFilter.Name = "cboYearLevelFilter"
        cboYearLevelFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboYearLevelFilter.Width = 150
        cboYearLevelFilter.Items.AddRange({"All Year Levels", "1st Year", "2nd Year", "3rd Year", "4th Year"})
        cboYearLevelFilter.SelectedIndex = 0
        cboYearLevelFilter.Margin = New Padding(0, 0, 12, 0)

        cboSemesterFilter.Name = "cboSemesterFilter"
        cboSemesterFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboSemesterFilter.Width = 150
        cboSemesterFilter.Margin = New Padding(0, 0, 12, 0)

        cboSortBy.Name = "cboSortBy"
        cboSortBy.DropDownStyle = ComboBoxStyle.DropDownList
        cboSortBy.Width = 160
        cboSortBy.Items.AddRange({"Newest First", "Oldest First", "Title (A-Z)", "Title (Z-A)"})
        cboSortBy.SelectedIndex = 0
        cboSortBy.Margin = New Padding(0, 0, 12, 0)

        lblSearch.Name = "lblSearch"
        lblSearch.AutoSize = False
        lblSearch.Size = New Size(50, 24)
        lblSearch.Text = "Search"
        lblSearch.TextAlign = ContentAlignment.MiddleLeft
        lblSearch.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblSearch.ForeColor = Color.DimGray
        lblSearch.Margin = New Padding(0, 0, 6, 0)

        txtSearch.Name = "txtSearch"
        txtSearch.Width = 250
        txtSearch.Margin = New Padding(0, 0, 12, 0)

        filterPanel.Controls.Add(cboYearLevelFilter)
        filterPanel.Controls.Add(cboSemesterFilter)
        filterPanel.Controls.Add(cboSortBy)
        filterPanel.Controls.Add(lblSearch)
        filterPanel.Controls.Add(txtSearch)

        dgvArchive.Name = "dgvArchive"
        dgvArchive.Dock = DockStyle.Fill
        dgvArchive.AllowUserToAddRows = False
        dgvArchive.AllowUserToDeleteRows = False
        dgvArchive.AllowUserToResizeRows = False
        dgvArchive.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvArchive.BackgroundColor = SystemColors.Window
        dgvArchive.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvArchive.MultiSelect = False
        dgvArchive.ReadOnly = False
        dgvArchive.RowHeadersVisible = False
        dgvArchive.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        pnlPagination.Name = "pnlArchivePagination"
        pnlPagination.Dock = DockStyle.Fill
        pnlPagination.Height = 44
        pnlPagination.BackColor = SystemColors.Control

        lblPaginationSummary.Name = "lblPaginationSummary"
        lblPaginationSummary.Dock = DockStyle.Left
        lblPaginationSummary.Width = 220
        lblPaginationSummary.Text = "Showing 0 archived titles"
        lblPaginationSummary.ForeColor = Color.DimGray
        lblPaginationSummary.TextAlign = ContentAlignment.MiddleLeft

        paginationControls.Name = "flpArchivePaginationControls"
        paginationControls.Dock = DockStyle.Right
        paginationControls.Width = 340
        paginationControls.FlowDirection = FlowDirection.LeftToRight
        paginationControls.WrapContents = False
        paginationControls.Padding = New Padding(0, 7, 0, 0)

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
        lblPageIndicator.Text = "Page 1 of 1"
        lblPageIndicator.TextAlign = ContentAlignment.MiddleCenter
        lblPageIndicator.ForeColor = Color.DimGray
        lblPageIndicator.Margin = New Padding(0, 0, 8, 0)

        btnNextPage.Name = "btnNextPage"
        btnNextPage.Text = "Next"
        btnNextPage.Size = New Size(62, 28)
        btnNextPage.Enabled = False
        btnNextPage.UseVisualStyleBackColor = True

        paginationControls.Controls.Add(cboPageSize)
        paginationControls.Controls.Add(btnPreviousPage)
        paginationControls.Controls.Add(lblPageIndicator)
        paginationControls.Controls.Add(btnNextPage)
        pnlPagination.Controls.Add(paginationControls)
        pnlPagination.Controls.Add(lblPaginationSummary)

        layout.Controls.Add(headerPanel, 0, 0)
        layout.Controls.Add(filterPanel, 0, 1)
        layout.Controls.Add(dgvArchive, 0, 2)
        layout.Controls.Add(pnlPagination, 0, 3)

        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
        _paginationControlsReady = True
    End Sub

    Private Sub SetupGridColumns()
        dgvArchive.AutoGenerateColumns = False
        dgvArchive.Columns.Clear()
        colSelected.Name = "colSelected"
        colSelected.HeaderText = ""
        colSelected.Width = 40
        colSelected.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colSelected.SortMode = DataGridViewColumnSortMode.NotSortable
        colSelected.ReadOnly = False
        colSelected.Visible = False
        dgvArchive.Columns.Add(colSelected)
        dgvArchive.Columns.Add("colTitle", "Title")
        dgvArchive.Columns.Add("colGroupName", "Group")
        dgvArchive.Columns.Add("colSubmitted", "Submitted")
        dgvArchive.Columns.Add("colApprovedDate", "Approved Date")

        colViewDetails.Name = "colViewDetails"
        colViewDetails.HeaderText = ""
        colViewDetails.Text = "View Details"
        colViewDetails.UseColumnTextForButtonValue = True
        colViewDetails.Width = 120
        colViewDetails.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colViewDetails.SortMode = DataGridViewColumnSortMode.NotSortable
        dgvArchive.Columns.Add(colViewDetails)

        For Each gridColumn As DataGridViewColumn In dgvArchive.Columns
            gridColumn.ReadOnly = Not (gridColumn Is colSelected)
        Next

        dgvArchive.EnableHeadersVisualStyles = False
        dgvArchive.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvArchive.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvArchive.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvArchive.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvArchive.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvArchive.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvArchive.ColumnHeadersHeight = 40
        dgvArchive.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvArchive.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvArchive.GridColor = Color.FromArgb(235, 235, 235)
        dgvArchive.BorderStyle = BorderStyle.None
        dgvArchive.RowTemplate.Height = 48
        dgvArchive.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvArchive.DefaultCellStyle.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
        dgvArchive.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvArchive.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)

        chkSelectAll.Name = "chkSelectAll"
        chkSelectAll.AutoSize = False
        chkSelectAll.Size = New Size(16, 16)
        chkSelectAll.Text = String.Empty
        chkSelectAll.BackColor = Color.Transparent
        chkSelectAll.TabStop = False
        chkSelectAll.Visible = False
        chkSelectAll.AccessibleName = "Select all archive titles"
        chkSelectAll.AccessibleDescription = "Select or clear all archived titles, including other pages."
        dgvArchive.Controls.Add(chkSelectAll)
        PositionSelectAllCheckbox()
    End Sub

    Private Function FetchArchivedTitles() As Boolean
        Try
            _archivedTitles = ProposalService.GetAllArchivedTitles()
            Dim currentIds As New HashSet(Of Integer)(_archivedTitles.Select(Function(title) title.ArchiveId))
            _selectedArchiveIds.RemoveWhere(Function(archiveId) Not currentIds.Contains(archiveId))

            If cboSemesterFilter.Items.Count = 0 Then
                cboSemesterFilter.Items.Add("All Semesters")

                Dim semesters As List(Of String) = _archivedTitles _
                    .Select(Function(title) title.SemesterApproved) _
                    .Where(Function(semester) Not String.IsNullOrWhiteSpace(semester)) _
                    .Distinct(StringComparer.OrdinalIgnoreCase) _
                    .OrderBy(Function(semester) semester, StringComparer.OrdinalIgnoreCase) _
                    .ToList()

                For Each semester As String In semesters
                    cboSemesterFilter.Items.Add(semester)
                Next

                cboSemesterFilter.SelectedIndex = 0
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show("Could not load archived titles: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub ApplyFilters()
        _currentPageIndex = 0
        Dim titlesToShow As IEnumerable(Of ArchivedTitle) = _archivedTitles

        If cboYearLevelFilter.SelectedIndex > 0 Then
            Dim selectedYearLevel As String = CStr(cboYearLevelFilter.SelectedItem)
            titlesToShow = titlesToShow.Where(
                Function(title) String.Equals(title.YearLevel, selectedYearLevel, StringComparison.Ordinal))
        End If

        If cboSemesterFilter.SelectedIndex > 0 Then
            Dim selectedSemester As String = CStr(cboSemesterFilter.SelectedItem)
            titlesToShow = titlesToShow.Where(
                Function(title) String.Equals(title.SemesterApproved, selectedSemester, StringComparison.OrdinalIgnoreCase))
        End If

        Dim searchText As String = txtSearch.Text.Trim()
        If searchText.Length > 0 Then
            titlesToShow = titlesToShow.Where(
                Function(title) If(title.Title, String.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                If(title.Keywords, String.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
        End If

        Dim sortedTitles As IEnumerable(Of ArchivedTitle)
        Select Case cboSortBy.SelectedIndex
            Case 1
                sortedTitles = titlesToShow.OrderBy(Function(title) title.SubmittedAt)
            Case 2
                sortedTitles = titlesToShow.OrderBy(Function(title) title.Title, StringComparer.Ordinal)
            Case 3
                sortedTitles = titlesToShow.OrderByDescending(Function(title) title.Title, StringComparer.Ordinal)
            Case Else
                sortedTitles = titlesToShow.OrderByDescending(Function(title) title.SubmittedAt)
        End Select

        _filteredArchivedTitles = sortedTitles.ToList()
        RenderCurrentPage()
    End Sub

    Private Sub RenderCurrentPage()
        Dim totalCount As Integer = _filteredArchivedTitles.Count
        Dim totalPages As Integer = CInt(Math.Ceiling(totalCount / CDbl(_pageSize)))
        _currentPageIndex = Math.Max(0, Math.Min(_currentPageIndex, Math.Max(0, totalPages - 1)))

        Dim firstIndex As Integer = _currentPageIndex * _pageSize
        Dim pageTitles As List(Of ArchivedTitle) = _filteredArchivedTitles.Skip(firstIndex).Take(_pageSize).ToList()

        dgvArchive.Rows.Clear()
        For Each archivedTitle As ArchivedTitle In pageTitles
            _updatingSelection = True
            Dim rowIndex As Integer = dgvArchive.Rows.Add(
                _selectedArchiveIds.Contains(archivedTitle.ArchiveId),
                archivedTitle.Title,
                archivedTitle.GroupName,
                If(archivedTitle.SubmittedAt = DateTime.MinValue, "", archivedTitle.SubmittedAt.ToString("MM/dd/yyyy h:mm:ss tt")),
                archivedTitle.ApprovedAt.ToString("MM/dd/yyyy"))
            dgvArchive.Rows(rowIndex).Tag = archivedTitle.ArchiveId
            _updatingSelection = False
        Next

        If totalCount = 0 Then
            lblPaginationSummary.Text = "Showing 0 archived titles"
            lblPageIndicator.Text = "Page 0 of 0"
        Else
            Dim firstDisplayed As Integer = firstIndex + 1
            Dim lastDisplayed As Integer = Math.Min(firstIndex + _pageSize, totalCount)
            lblPaginationSummary.Text = "Showing " & firstDisplayed.ToString() & "-" & lastDisplayed.ToString() &
                                        " of " & totalCount.ToString() & " archived titles"
            lblPageIndicator.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & totalPages.ToString()
        End If

        btnPreviousPage.Enabled = _currentPageIndex > 0
        btnNextPage.Enabled = _currentPageIndex < totalPages - 1
        UpdateSelectionControls()
    End Sub

    Private Sub dgvArchive_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvArchive.CellContentClick
        If e.RowIndex < 0 Then
            Return
        End If

        Dim archiveId As Integer = CInt(dgvArchive.Rows(e.RowIndex).Tag)
        Dim archivedTitle As ArchivedTitle = Nothing

        For Each item As ArchivedTitle In _archivedTitles
            If item.ArchiveId = archiveId Then
                archivedTitle = item
                Exit For
            End If
        Next

        If archivedTitle Is Nothing Then
            Return
        End If

        If e.ColumnIndex = colViewDetails.Index Then
            Using detailsDialog As New frmArchiveDetails(archivedTitle)
                If detailsDialog.ShowDialog(Me.FindForm()) = DialogResult.OK AndAlso FetchArchivedTitles() Then
                    ApplyFilters()
                End If
            End Using
        End If
    End Sub

    Private Sub dgvArchive_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvArchive.CurrentCellDirtyStateChanged
        If dgvArchive.IsCurrentCellDirty AndAlso TypeOf dgvArchive.CurrentCell Is DataGridViewCheckBoxCell Then
            dgvArchive.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub dgvArchive_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvArchive.CellValueChanged
        If Not _selectionMode OrElse _updatingSelection OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> colSelected.Index Then
            Return
        End If

        Dim archiveId As Integer = CInt(dgvArchive.Rows(e.RowIndex).Tag)
        If Convert.ToBoolean(dgvArchive.Rows(e.RowIndex).Cells(colSelected.Index).Value) Then
            _selectedArchiveIds.Add(archiveId)
        Else
            _selectedArchiveIds.Remove(archiveId)
        End If

        UpdateSelectionControls()
    End Sub

    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged
        If Not _selectionMode OrElse _updatingSelection Then
            Return
        End If

        If chkSelectAll.Checked Then
            For Each archivedTitle As ArchivedTitle In _archivedTitles
                _selectedArchiveIds.Add(archivedTitle.ArchiveId)
            Next
        Else
            _selectedArchiveIds.Clear()
        End If

        RenderCurrentPage()
    End Sub

    Private Sub UpdateSelectionControls()
        _updatingSelection = True
        chkSelectAll.Checked = _selectionMode AndAlso _archivedTitles.Count > 0 AndAlso _selectedArchiveIds.Count = _archivedTitles.Count
        chkSelectAll.Visible = _selectionMode
        btnDeleteSelected.Visible = _selectionMode AndAlso _selectedArchiveIds.Count > 0
        btnDeleteSelected.Enabled = btnDeleteSelected.Visible
        btnToggleSelectionMode.Text = If(_selectionMode, "Cancel Selection", "Select")
        btnDeleteSelected.Text = If(_selectedArchiveIds.Count > 0,
                                    "Delete Selected (" & _selectedArchiveIds.Count.ToString() & ")",
                                    "Delete Selected")
        _updatingSelection = False
        PositionSelectAllCheckbox()
    End Sub

    Private Sub PositionSelectAllCheckbox()
        If Not _selectionMode OrElse Not dgvArchive.Controls.Contains(chkSelectAll) OrElse colSelected.Index < 0 Then
            chkSelectAll.Visible = False
            Return
        End If

        Dim headerBounds As Rectangle = dgvArchive.GetCellDisplayRectangle(colSelected.Index, -1, True)
        chkSelectAll.Visible = True
        chkSelectAll.Location = New Point(
            headerBounds.Left + (headerBounds.Width - chkSelectAll.Width) \ 2,
            headerBounds.Top + (headerBounds.Height - chkSelectAll.Height) \ 2)
        chkSelectAll.BringToFront()
    End Sub

    Private Sub btnToggleSelectionMode_Click(sender As Object, e As EventArgs) Handles btnToggleSelectionMode.Click
        _selectionMode = Not _selectionMode
        If Not _selectionMode Then
            _selectedArchiveIds.Clear()
        End If

        colSelected.Visible = _selectionMode
        UpdateSelectionControls()
        RenderCurrentPage()
    End Sub

    Private Sub dgvArchive_LayoutChanged(sender As Object, e As EventArgs) Handles dgvArchive.Resize, dgvArchive.Scroll, dgvArchive.ColumnWidthChanged, dgvArchive.ColumnDisplayIndexChanged, dgvArchive.ColumnHeadersHeightChanged
        PositionSelectAllCheckbox()
    End Sub

    Private Sub DeleteArchiveEntries(archivedTitles As List(Of ArchivedTitle))
        If archivedTitles Is Nothing OrElse archivedTitles.Count = 0 Then
            Return
        End If

        Dim itemType As String = If(archivedTitles.Count = 1, "archived title", "archived titles")
        Dim itemNames As String = String.Join(vbCrLf, archivedTitles.Select(
            Function(title) title.Title & " (Archive #" & title.ArchiveId.ToString() & ")"))

        Using confirmation As New frmDeleteConfirmation(itemType, itemNames)
            If confirmation.ShowDialog(Me.FindForm()) <> DialogResult.OK Then
                Return
            End If
        End Using

        Try
            Dim archiveIds As List(Of Integer) = archivedTitles.Select(Function(title) title.ArchiveId).ToList()
            Dim deletedCount As Integer = ProposalService.DeleteArchivedTitles(archiveIds)
            _selectedArchiveIds.ExceptWith(archiveIds)
            MessageBox.Show(deletedCount.ToString() & " archived title(s) deleted successfully.")

            If FetchArchivedTitles() Then
                ApplyFilters()
            End If
        Catch ex As Exception
            MessageBox.Show("Could not delete archived titles: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDeleteSelected_Click(sender As Object, e As EventArgs) Handles btnDeleteSelected.Click
        Dim selectedTitles As List(Of ArchivedTitle) = _archivedTitles _
            .Where(Function(title) _selectedArchiveIds.Contains(title.ArchiveId)) _
            .ToList()
        DeleteArchiveEntries(selectedTitles)
    End Sub

    Private Sub ucTitleArchive_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGridColumns()

        If FetchArchivedTitles() Then
            ApplyFilters()
        End If
    End Sub

    Private Sub cboSemesterFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSemesterFilter.SelectedIndexChanged
        ApplyFilters()
    End Sub

    Private Sub cboYearLevelFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboYearLevelFilter.SelectedIndexChanged
        ApplyFilters()
    End Sub

    Private Sub cboSortBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSortBy.SelectedIndexChanged
        ApplyFilters()
    End Sub

    Private Sub cboPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPageSize.SelectedIndexChanged
        If Not _paginationControlsReady OrElse cboPageSize.SelectedItem Is Nothing Then
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
        Dim totalPages As Integer = CInt(Math.Ceiling(_filteredArchivedTitles.Count / CDbl(_pageSize)))
        If _currentPageIndex >= totalPages - 1 Then
            Return
        End If

        _currentPageIndex += 1
        RenderCurrentPage()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        ApplyFilters()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        If FetchArchivedTitles() Then
            ApplyFilters()
        End If
    End Sub

    Public Sub RefreshData()
        btnRefresh_Click(btnRefresh, EventArgs.Empty)
    End Sub
End Class