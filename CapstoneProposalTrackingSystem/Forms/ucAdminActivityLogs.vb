Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Linq

Public Class ucAdminActivityLogs
    Inherits UserControl

    Private ReadOnly _pageSizeOptions As Integer() = {10, 25, 50, 100}
    Private _currentPageIndex As Integer
    Private _pageSize As Integer = 25
    Private _totalCount As Integer
    Private _initializing As Boolean = True
    Private ReadOnly _selectedActivityKeys As New HashSet(Of String)
    Private ReadOnly _selectedEntries As New Dictionary(Of String, AdminActivityLogEntry)
    Private _selectionMode As Boolean
    Private _updatingSelection As Boolean

    Private WithEvents lblHeader As New Label()
    Private WithEvents lblHeaderSubtitle As New Label()
    Private WithEvents cboAction As New ComboBox()
    Private WithEvents cboEntity As New ComboBox()
    Private WithEvents txtSearch As New TextBox()
    Private WithEvents dtpFrom As New DateTimePicker()
    Private WithEvents dtpTo As New DateTimePicker()
    Private WithEvents btnRefresh As New Button()
    Private WithEvents btnToggleSelectionMode As New Button()
    Private WithEvents btnDeleteSelected As New Button()
    Private WithEvents chkSelectAll As New CheckBox()
    Private WithEvents dgvLogs As New DataGridView()
    Private ReadOnly pnlPagination As New Panel()
    Private WithEvents lblPaginationSummary As New Label()
    Private WithEvents lblPageIndicator As New Label()
    Private WithEvents cboPageSize As New ComboBox()
    Private WithEvents btnPreviousPage As New Button()
    Private WithEvents btnNextPage As New Button()
    Private ReadOnly colSelected As New DataGridViewCheckBoxColumn()

    Public Sub New()
        InitializeComponent()
        _initializing = False
        LoadEntries()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim headerPanel As New Panel()
        Dim headerTextLayout As New TableLayoutPanel()
        Dim filterPanel As New FlowLayoutPanel()
        Dim toolbar As New TableLayoutPanel()
        Dim paginationControls As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Name = "ucAdminActivityLogs"

        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 3
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 110.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        lblHeader.Text = "Admin Activity Logs"
        lblHeader.Visible = False
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)

        lblHeaderSubtitle.Text = "Review administrative changes made in the system."
        lblHeaderSubtitle.Dock = DockStyle.Fill
        lblHeaderSubtitle.TextAlign = ContentAlignment.MiddleLeft
        lblHeaderSubtitle.Font = New Font("Segoe UI", 9.0F)
        lblHeaderSubtitle.ForeColor = Color.DimGray

        headerTextLayout.Dock = DockStyle.Fill
        headerTextLayout.ColumnCount = 1
        headerTextLayout.RowCount = 2
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 0.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        headerTextLayout.Controls.Add(lblHeader, 0, 0)
        headerTextLayout.Controls.Add(lblHeaderSubtitle, 0, 1)
        headerPanel.Dock = DockStyle.Fill
        headerPanel.Controls.Add(headerTextLayout)

        Dim headerActions As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(0, 6, 0, 0),
            .Margin = New Padding(0)
        }
        btnToggleSelectionMode.Text = "Select"
        btnToggleSelectionMode.Size = New Size(100, 28)
        btnToggleSelectionMode.Margin = New Padding(8, 0, 0, 0)
        btnDeleteSelected.Text = "Delete Selected"
        btnDeleteSelected.Size = New Size(180, 28)
        btnDeleteSelected.Margin = New Padding(8, 0, 0, 0)
        btnDeleteSelected.FlatStyle = FlatStyle.Flat
        btnDeleteSelected.FlatAppearance.BorderSize = 0
        btnDeleteSelected.BackColor = Color.FromArgb(166, 58, 58)
        btnDeleteSelected.ForeColor = Color.White
        btnDeleteSelected.UseVisualStyleBackColor = False
        btnDeleteSelected.Visible = False
        btnDeleteSelected.Enabled = False
        headerActions.Controls.Add(btnRefresh)
        headerActions.Controls.Add(btnToggleSelectionMode)
        headerActions.Controls.Add(btnDeleteSelected)

        filterPanel.Dock = DockStyle.Fill
        filterPanel.FlowDirection = FlowDirection.LeftToRight
        filterPanel.WrapContents = True
        filterPanel.Padding = New Padding(0, 3, 0, 0)
        ConfigureFilterCombo(cboAction, {"All actions", "Created", "Updated", "Deleted", "Activated", "Deactivated", "Approved", "Rejected", "Requested Revision", "Reset Password"}, 145)
        ConfigureFilterCombo(cboEntity, {"All areas", "Account", "Proposal", "Student History", "Title Archive", "Profile"}, 145)
        cboAction.SelectedIndex = 0
        cboEntity.SelectedIndex = 0
        txtSearch.Width = 190
        txtSearch.Margin = New Padding(0, 0, 12, 0)
        txtSearch.PlaceholderText = "Search activity"
        dtpFrom.Format = DateTimePickerFormat.Short
        dtpFrom.Value = DateTime.Today.AddDays(-30)
        dtpFrom.Margin = New Padding(0, 0, 8, 0)
        dtpTo.Format = DateTimePickerFormat.Short
        dtpTo.Value = DateTime.Today
        dtpTo.Margin = New Padding(0, 0, 12, 0)
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(90, 28)
        btnRefresh.Margin = New Padding(8, 0, 0, 0)
        filterPanel.Controls.AddRange({
            New Label With {.Text = "Action", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            cboAction,
            New Label With {.Text = "Area", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            cboEntity,
            New Label With {.Text = "Search", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            txtSearch,
            New Label With {.Text = "From", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            dtpFrom,
            New Label With {.Text = "To", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            dtpTo
        })
        SetupGrid()

        pnlPagination.Dock = DockStyle.Fill
        pnlPagination.Height = 44
        lblPaginationSummary.Dock = DockStyle.Left
        lblPaginationSummary.Width = 220
        lblPaginationSummary.ForeColor = Color.DimGray
        lblPaginationSummary.TextAlign = ContentAlignment.MiddleLeft
        lblPageIndicator.AutoSize = False
        lblPageIndicator.Width = 90
        lblPageIndicator.TextAlign = ContentAlignment.MiddleCenter
        paginationControls.Dock = DockStyle.Right
        paginationControls.Width = 370
        paginationControls.FlowDirection = FlowDirection.LeftToRight
        paginationControls.WrapContents = False
        paginationControls.Padding = New Padding(0, 7, 0, 0)
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList
        cboPageSize.Width = 82
        cboPageSize.Items.AddRange(_pageSizeOptions.Cast(Of Object)().ToArray())
        cboPageSize.SelectedItem = _pageSize
        cboPageSize.Margin = New Padding(0, 0, 8, 0)
        btnPreviousPage.Text = "Previous"
        btnPreviousPage.Size = New Size(78, 28)
        btnPreviousPage.Enabled = False
        btnPreviousPage.Margin = New Padding(0, 0, 8, 0)
        btnNextPage.Text = "Next"
        btnNextPage.Size = New Size(62, 28)
        btnNextPage.Enabled = False
        paginationControls.Controls.Add(cboPageSize)
        paginationControls.Controls.Add(btnPreviousPage)
        paginationControls.Controls.Add(lblPageIndicator)
        paginationControls.Controls.Add(btnNextPage)
        pnlPagination.Controls.Add(paginationControls)
        pnlPagination.Controls.Add(lblPaginationSummary)

        filterPanel.Padding = New Padding(0, 6, 4, 0)
        toolbar.Name = "tlpActivityLogsToolbar"
        toolbar.Dock = DockStyle.Fill
        toolbar.ColumnCount = 2
        toolbar.RowCount = 1
        toolbar.Margin = New Padding(0)
        toolbar.Padding = New Padding(0)
        toolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        toolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 410.0F))
        toolbar.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        toolbar.Controls.Add(filterPanel, 0, 0)
        toolbar.Controls.Add(headerActions, 1, 0)

        layout.Controls.Add(toolbar, 0, 0)
        layout.Controls.Add(dgvLogs, 0, 1)
        layout.Controls.Add(pnlPagination, 0, 2)
        Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub

    Private Sub ConfigureFilterCombo(combo As ComboBox, values As IEnumerable(Of String), width As Integer)
        combo.DropDownStyle = ComboBoxStyle.DropDownList
        combo.Width = width
        combo.Margin = New Padding(0, 0, 12, 0)
        combo.Items.AddRange(values.ToArray())
    End Sub

    Private Sub SetupGrid()
        dgvLogs.Dock = DockStyle.Fill
        dgvLogs.AutoGenerateColumns = False
        dgvLogs.AllowUserToAddRows = False
        dgvLogs.AllowUserToDeleteRows = False
        dgvLogs.AllowUserToResizeRows = False
        dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvLogs.BackgroundColor = SystemColors.Window
        dgvLogs.MultiSelect = False
        ' Keep the grid editable only for selection checkboxes. Activity values remain read-only.
        dgvLogs.ReadOnly = False
        dgvLogs.RowHeadersVisible = False
        dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvLogs.Controls.Add(chkSelectAll)
        chkSelectAll.Size = New Size(18, 18)
        chkSelectAll.Visible = False
        colSelected.Name = "colSelected"
        colSelected.HeaderText = ""
        colSelected.Width = 38
        colSelected.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colSelected.Visible = False
        colSelected.ReadOnly = False
        dgvLogs.Columns.Insert(0, colSelected)
        dgvLogs.Columns.Add("colCreatedAt", "When")
        dgvLogs.Columns.Add("colActorName", "Administrator")
        dgvLogs.Columns.Add("colActionType", "Action")
        dgvLogs.Columns.Add("colEntityType", "Area")
        dgvLogs.Columns.Add("colEntityName", "Target")
        dgvLogs.Columns.Add("colDescription", "Details")
        For Each column As DataGridViewColumn In dgvLogs.Columns
            column.ReadOnly = column IsNot colSelected
        Next
        dgvLogs.EnableHeadersVisualStyles = False
        dgvLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvLogs.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvLogs.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvLogs.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvLogs.ColumnHeadersHeight = 40
        dgvLogs.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvLogs.GridColor = Color.FromArgb(235, 235, 235)
        dgvLogs.BorderStyle = BorderStyle.None
        dgvLogs.RowTemplate.Height = 48
        dgvLogs.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvLogs.DefaultCellStyle.Font = New Font("Segoe UI", 9.5F)
        dgvLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvLogs.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)
    End Sub

    Private Sub dgvLogs_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvLogs.CurrentCellDirtyStateChanged
        If dgvLogs.IsCurrentCellDirty AndAlso TypeOf dgvLogs.CurrentCell Is DataGridViewCheckBoxCell Then
            dgvLogs.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub dgvLogs_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLogs.CellValueChanged
        If Not _selectionMode OrElse _updatingSelection OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> colSelected.Index Then Return
        Dim entry As AdminActivityLogEntry = TryCast(dgvLogs.Rows(e.RowIndex).Tag, AdminActivityLogEntry)
        If entry Is Nothing Then Return
        Dim activityKey As String = GetActivityKey(entry)
        If Convert.ToBoolean(dgvLogs.Rows(e.RowIndex).Cells(colSelected.Index).Value) Then
            _selectedActivityKeys.Add(activityKey)
            _selectedEntries(activityKey) = entry
        Else
            _selectedActivityKeys.Remove(activityKey)
            _selectedEntries.Remove(activityKey)
        End If
        UpdateSelectionControls()
    End Sub

    Private Sub chkSelectAll_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelectAll.CheckedChanged
        If Not _selectionMode OrElse _updatingSelection Then Return
        _updatingSelection = True
        For Each row As DataGridViewRow In dgvLogs.Rows
            Dim entry As AdminActivityLogEntry = TryCast(row.Tag, AdminActivityLogEntry)
            If entry Is Nothing Then Continue For
            Dim activityKey As String = GetActivityKey(entry)
            If chkSelectAll.Checked Then
                _selectedActivityKeys.Add(activityKey)
                _selectedEntries(activityKey) = entry
            Else
                _selectedActivityKeys.Remove(activityKey)
                _selectedEntries.Remove(activityKey)
            End If
            row.Cells(colSelected.Index).Value = chkSelectAll.Checked
        Next
        dgvLogs.EndEdit()
        RenderSelectionState()
        _updatingSelection = False
        dgvLogs.Invalidate()
        UpdateSelectionControls()
    End Sub

    Private Sub UpdateSelectionControls()
        _updatingSelection = True
        Dim storedOnPage As Integer = dgvLogs.Rows.Cast(Of DataGridViewRow)().
            Count(Function(row) TryCast(row.Tag, AdminActivityLogEntry) IsNot Nothing AndAlso
                               DirectCast(row.Tag, AdminActivityLogEntry) IsNot Nothing)
        chkSelectAll.Checked = _selectionMode AndAlso storedOnPage > 0 AndAlso
                               dgvLogs.Rows.Cast(Of DataGridViewRow)().All(
                                   Function(row)
                                       Dim entry As AdminActivityLogEntry = TryCast(row.Tag, AdminActivityLogEntry)
                                       Return entry Is Nothing OrElse _selectedActivityKeys.Contains(GetActivityKey(entry))
                                   End Function)
        chkSelectAll.Visible = _selectionMode
        btnDeleteSelected.Visible = _selectionMode AndAlso _selectedActivityKeys.Count > 0
        btnDeleteSelected.Enabled = btnDeleteSelected.Visible
        btnDeleteSelected.Text = If(_selectedActivityKeys.Count = 0, "Delete Selected",
                                    "Delete Selected (" & _selectedActivityKeys.Count.ToString() & ")")
        btnToggleSelectionMode.Text = If(_selectionMode, "Cancel Selection", "Select")
        _updatingSelection = False
        PositionSelectAllCheckbox()
    End Sub

    Private Sub RenderSelectionState()
        _updatingSelection = True
        For Each row As DataGridViewRow In dgvLogs.Rows
            Dim entry As AdminActivityLogEntry = TryCast(row.Tag, AdminActivityLogEntry)
            Dim isSelected As Boolean = entry IsNot Nothing AndAlso
                                        _selectedActivityKeys.Contains(GetActivityKey(entry))
            row.Cells(colSelected.Index).Value = isSelected
            row.Cells(colSelected.Index).ReadOnly = False
        Next
        dgvLogs.Refresh()
        _updatingSelection = False
        UpdateSelectionControls()
    End Sub

    Private Sub PositionSelectAllCheckbox()
        If Not _selectionMode OrElse colSelected.Index < 0 Then
            chkSelectAll.Visible = False
            Return
        End If
        Dim bounds As Rectangle = dgvLogs.GetCellDisplayRectangle(colSelected.Index, -1, True)
        chkSelectAll.Location = New Point(bounds.Left + (bounds.Width - chkSelectAll.Width) \ 2,
                                          bounds.Top + (bounds.Height - chkSelectAll.Height) \ 2)
        chkSelectAll.Visible = True
        chkSelectAll.BringToFront()
    End Sub

    Private Sub dgvLogs_LayoutChanged(sender As Object, e As EventArgs) Handles dgvLogs.Resize, dgvLogs.Scroll, dgvLogs.ColumnWidthChanged, dgvLogs.ColumnHeadersHeightChanged
        PositionSelectAllCheckbox()
    End Sub

    Private Sub btnToggleSelectionMode_Click(sender As Object, e As EventArgs) Handles btnToggleSelectionMode.Click
        _selectionMode = Not _selectionMode
        If Not _selectionMode Then
            _selectedActivityKeys.Clear()
            _selectedEntries.Clear()
        End If
        colSelected.Visible = _selectionMode
        RenderSelectionState()
    End Sub

    Private Sub btnDeleteSelected_Click(sender As Object, e As EventArgs) Handles btnDeleteSelected.Click
        If _selectedActivityKeys.Count = 0 Then Return
        Dim confirmation As DialogResult = MessageBox.Show(
            "Delete " & _selectedActivityKeys.Count.ToString() & " selected activity log record(s)? This cannot be undone.",
            "Confirm Activity Log Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirmation <> DialogResult.Yes Then Return
        Try
            Dim deletedCount As Integer = AdminActivityLogService.DeleteEntries(_selectedEntries.Values.ToList())
            _selectedActivityKeys.Clear()
            _selectedEntries.Clear()
            MessageBox.Show(deletedCount.ToString() & " activity log record(s) deleted successfully.")
            LoadEntries()
        Catch ex As Exception
            MessageBox.Show("Could not delete activity logs: " & ex.Message, "Activity Logs",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetActivityKey(entry As AdminActivityLogEntry) As String
        Return entry.ActivitySource & ":" & entry.ActivityId.ToString()
    End Function

    Private Sub dgvLogs_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvLogs.CellDoubleClick
        If e.RowIndex < 0 Then
            Return
        End If

        Dim entry As AdminActivityLogEntry = TryCast(dgvLogs.Rows(e.RowIndex).Tag, AdminActivityLogEntry)
        If entry Is Nothing Then
            Return
        End If

        Using detailsDialog As New frmAdminActivityLogDetails(entry)
            detailsDialog.ShowDialog(Me.FindForm())
        End Using
    End Sub

    Private Sub ReloadFromFirstPage(sender As Object, e As EventArgs) Handles cboAction.SelectedIndexChanged, cboEntity.SelectedIndexChanged, btnRefresh.Click
        If Not _initializing Then
            _currentPageIndex = 0
            LoadEntries()
        End If
    End Sub

    Private Sub Search_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            _currentPageIndex = 0
            LoadEntries()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub cboPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPageSize.SelectedIndexChanged
        If Not _initializing AndAlso cboPageSize.SelectedItem IsNot Nothing Then
            _pageSize = CInt(cboPageSize.SelectedItem)
            _currentPageIndex = 0
            LoadEntries()
        End If
    End Sub

    Private Sub btnPreviousPage_Click(sender As Object, e As EventArgs) Handles btnPreviousPage.Click
        If _currentPageIndex > 0 Then
            _currentPageIndex -= 1
            LoadEntries()
        End If
    End Sub

    Private Sub btnNextPage_Click(sender As Object, e As EventArgs) Handles btnNextPage.Click
        If _currentPageIndex < GetTotalPages() - 1 Then
            _currentPageIndex += 1
            LoadEntries()
        End If
    End Sub

    Private Function GetTotalPages() As Integer
        Return CInt(Math.Ceiling(_totalCount / CDbl(_pageSize)))
    End Function

    Private Sub LoadEntries()
        Try
            Dim entries As List(Of AdminActivityLogEntry) = AdminActivityLogService.GetEntries(
                If(cboAction.SelectedIndex <= 0, "", cboAction.Text),
                If(cboEntity.SelectedIndex <= 0, "", cboEntity.Text),
                txtSearch.Text,
                dtpFrom.Value.Date,
                dtpTo.Value.Date.AddDays(1),
                _currentPageIndex,
                _pageSize,
                _totalCount)

            dgvLogs.Rows.Clear()
            For Each entry As AdminActivityLogEntry In entries
                Dim rowIndex As Integer = dgvLogs.Rows.Add(
                    False,
                    entry.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    entry.ActorName,
                    entry.ActionType,
                    entry.EntityType,
                    entry.EntityName,
                    entry.Description)
                dgvLogs.Rows(rowIndex).Tag = entry
                dgvLogs.Rows(rowIndex).Cells(colSelected.Index).Value = False
                dgvLogs.Rows(rowIndex).Cells(colSelected.Index).ReadOnly = False
            Next
            RenderSelectionState()

            Dim totalPages As Integer = GetTotalPages()
            If _totalCount = 0 Then
                lblPaginationSummary.Text = "Showing 0 activities"
                lblPageIndicator.Text = "Page 0 of 0"
            Else
                Dim firstDisplayed As Integer = (_currentPageIndex * _pageSize) + 1
                Dim lastDisplayed As Integer = Math.Min(firstDisplayed + entries.Count - 1, _totalCount)
                lblPaginationSummary.Text = "Showing " & firstDisplayed.ToString() & "-" &
                                            lastDisplayed.ToString() & " of " & _totalCount.ToString() & " activities"
                lblPageIndicator.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & totalPages.ToString()
            End If
            btnPreviousPage.Enabled = _currentPageIndex > 0
            btnNextPage.Enabled = _currentPageIndex < totalPages - 1
        Catch ex As Exception
            MessageBox.Show("Could not load activity logs: " & ex.Message, "Activity Logs",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
