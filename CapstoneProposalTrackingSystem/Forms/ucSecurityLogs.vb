Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Linq

Public Class ucSecurityLogs
    Inherits UserControl

    Private ReadOnly _pageSizeOptions As Integer() = {10, 25, 50, 100}
    Private ReadOnly _selectedEntries As New Dictionary(Of Integer, SecurityLogEntry)
    Private _currentPageIndex As Integer
    Private _pageSize As Integer = 25
    Private _totalCount As Integer
    Private _selectionMode As Boolean
    Private _updatingSelection As Boolean
    Private _initializing As Boolean = True

    Private ReadOnly lblHeader As New Label()
    Private ReadOnly lblSubtitle As New Label()
    Private ReadOnly cboEvent As New ComboBox()
    Private ReadOnly txtSearch As New TextBox()
    Private ReadOnly dtpFrom As New DateTimePicker()
    Private ReadOnly dtpTo As New DateTimePicker()
    Private ReadOnly btnRefresh As New Button()
    Private ReadOnly btnToggleSelectionMode As New Button()
    Private ReadOnly btnDeleteSelected As New Button()
    Private ReadOnly chkSelectAll As New CheckBox()
    Private ReadOnly dgvLogs As New DataGridView()
    Private ReadOnly lblSummary As New Label()
    Private ReadOnly lblPage As New Label()
    Private ReadOnly cboPageSize As New ComboBox()
    Private ReadOnly btnPrevious As New Button()
    Private ReadOnly btnNext As New Button()
    Private ReadOnly colSelected As New DataGridViewCheckBoxColumn()

    Public Sub New()
        InitializeComponent()
        _initializing = False
        LoadEntries()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim headerPanel As New Panel()
        Dim headerText As New TableLayoutPanel()
        Dim headerActions As New FlowLayoutPanel()
        Dim filters As New FlowLayoutPanel()
        Dim pagination As New FlowLayoutPanel()

        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.Name = "ucSecurityLogs"

        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 4
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 72.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 80.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        headerText.Dock = DockStyle.Fill
        headerText.ColumnCount = 1
        headerText.RowCount = 2
        headerText.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        headerText.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        lblHeader.Text = "Security Logs"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)
        lblSubtitle.Text = "Monitor authentication and account security events."
        lblSubtitle.Dock = DockStyle.Fill
        lblSubtitle.TextAlign = ContentAlignment.MiddleLeft
        lblSubtitle.Font = New Font("Segoe UI", 9.0F)
        lblSubtitle.ForeColor = Color.DimGray
        headerText.Controls.Add(lblHeader, 0, 0)
        headerText.Controls.Add(lblSubtitle, 0, 1)
        headerPanel.Dock = DockStyle.Fill
        headerPanel.Controls.Add(headerText)

        headerActions.Dock = DockStyle.Right
        headerActions.Width = 390
        headerActions.FlowDirection = FlowDirection.RightToLeft
        headerActions.WrapContents = False
        headerActions.Padding = New Padding(0, 2, 0, 0)
        ConfigureButton(btnRefresh, "Refresh", 90)
        ConfigureButton(btnToggleSelectionMode, "Select", 100)
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
        headerPanel.Controls.Add(headerActions)

        filters.Dock = DockStyle.Fill
        filters.FlowDirection = FlowDirection.LeftToRight
        filters.WrapContents = True
        filters.Padding = New Padding(0, 3, 0, 0)
        ConfigureFilterCombo(cboEvent, {"All events", "Successful Login", "Failed Login", "Blocked Login", "Logout",
                                         "Session Created", "Session Revoked", "Session Expired", "Password Changed",
                                         "Password Reset", "Account Created", "Account Activated", "Account Deactivated",
                                         "Account Deleted", "Role Changed", "Profile Security Change",
                                         "Account Security Change", "Sensitive Export"}, 145)
        cboEvent.SelectedIndex = 0
        txtSearch.Width = 190
        txtSearch.Margin = New Padding(0, 0, 12, 0)
        txtSearch.PlaceholderText = "Search security logs"
        dtpFrom.Format = DateTimePickerFormat.Short
        dtpFrom.Value = DateTime.Today.AddDays(-30)
        dtpFrom.Margin = New Padding(0, 0, 8, 0)
        dtpTo.Format = DateTimePickerFormat.Short
        dtpTo.Value = DateTime.Today
        dtpTo.Margin = New Padding(0, 0, 12, 0)
        filters.Controls.AddRange({
            New Label With {.Text = "Event", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            cboEvent,
            New Label With {.Text = "Search", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            txtSearch,
            New Label With {.Text = "From", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            dtpFrom,
            New Label With {.Text = "To", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)},
            dtpTo
        })

        SetupGrid()

        lblSummary.Dock = DockStyle.Left
        lblSummary.Width = 220
        lblSummary.ForeColor = Color.DimGray
        lblSummary.TextAlign = ContentAlignment.MiddleLeft
        pagination.Dock = DockStyle.Right
        pagination.Width = 370
        pagination.FlowDirection = FlowDirection.LeftToRight
        pagination.WrapContents = False
        pagination.Padding = New Padding(0, 7, 0, 0)
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList
        cboPageSize.Width = 82
        cboPageSize.Items.AddRange(_pageSizeOptions.Cast(Of Object)().ToArray())
        cboPageSize.SelectedItem = _pageSize
        cboPageSize.Margin = New Padding(0, 0, 8, 0)
        btnPrevious.Text = "Previous"
        btnPrevious.Size = New Size(78, 28)
        btnPrevious.Margin = New Padding(0, 0, 8, 0)
        btnNext.Text = "Next"
        btnNext.Size = New Size(62, 28)
        lblPage.AutoSize = False
        lblPage.Width = 90
        lblPage.TextAlign = ContentAlignment.MiddleCenter
        pagination.Controls.Add(cboPageSize)
        pagination.Controls.Add(btnPrevious)
        pagination.Controls.Add(lblPage)
        pagination.Controls.Add(btnNext)

        Dim footer As New Panel With {.Dock = DockStyle.Fill}
        footer.Controls.Add(pagination)
        footer.Controls.Add(lblSummary)
        layout.Controls.Add(headerPanel, 0, 0)
        layout.Controls.Add(filters, 0, 1)
        layout.Controls.Add(dgvLogs, 0, 2)
        layout.Controls.Add(footer, 0, 3)
        Controls.Add(layout)

        AddHandler btnRefresh.Click, Sub() Reload()
        AddHandler btnToggleSelectionMode.Click, AddressOf ToggleSelectionMode
        AddHandler btnDeleteSelected.Click, AddressOf DeleteSelected
        AddHandler cboEvent.SelectedIndexChanged, Sub() Reload()
        AddHandler dtpFrom.ValueChanged, Sub() Reload()
        AddHandler dtpTo.ValueChanged, Sub() Reload()
        AddHandler txtSearch.KeyDown, AddressOf SearchKeyDown
        AddHandler cboPageSize.SelectedIndexChanged, AddressOf PageSizeChanged
        AddHandler btnPrevious.Click, Sub() ChangePage(-1)
        AddHandler btnNext.Click, Sub() ChangePage(1)
        AddHandler dgvLogs.CurrentCellDirtyStateChanged, AddressOf GridDirty
        AddHandler dgvLogs.CellValueChanged, AddressOf GridValueChanged
        AddHandler chkSelectAll.CheckedChanged, AddressOf SelectAllChanged
        AddHandler dgvLogs.Resize, Sub() PositionSelectAllCheckbox()
        AddHandler dgvLogs.Scroll, Sub() PositionSelectAllCheckbox()
        AddHandler dgvLogs.ColumnWidthChanged, Sub() PositionSelectAllCheckbox()
    End Sub

    Private Sub ConfigureButton(button As Button, text As String, width As Integer)
        button.Text = text
        button.Size = New Size(width, 28)
        button.Margin = New Padding(8, 0, 0, 0)
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
        dgvLogs.ReadOnly = False
        dgvLogs.RowHeadersVisible = False
        dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        colSelected.Name = "colSelected"
        colSelected.HeaderText = ""
        colSelected.Width = 38
        colSelected.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colSelected.Visible = False
        dgvLogs.Columns.Add(colSelected)
        dgvLogs.Columns.Add("colEvent", "Event")
        dgvLogs.Columns.Add("colUsername", "User")
        dgvLogs.Columns.Add("colCreatedAt", "Date and time")
        dgvLogs.Columns.Add("colDevice", "Device")
        dgvLogs.Columns.Add("colIpAddress", "IP address")
        dgvLogs.Columns.Add("colDetails", "Details")
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
        dgvLogs.Controls.Add(chkSelectAll)
        chkSelectAll.Size = New Size(18, 18)
        chkSelectAll.Visible = False
    End Sub

    Private Sub LoadEntries()
        Dim entries As List(Of SecurityLogEntry) = SecurityLogService.GetEntries(
            If(cboEvent.SelectedIndex <= 0, Nothing, cboEvent.Text),
            txtSearch.Text, dtpFrom.Value.Date, dtpTo.Value.Date.AddDays(1),
            _currentPageIndex, _pageSize, _totalCount)
        dgvLogs.Rows.Clear()
        For Each entry As SecurityLogEntry In entries
            Dim rowIndex As Integer = dgvLogs.Rows.Add(False, entry.EventType, entry.Username,
                entry.CreatedAt.ToString("MMMM d, yyyy h:mm:ss tt"), entry.DeviceName, entry.IpAddress, entry.Details)
            dgvLogs.Rows(rowIndex).Tag = entry
        Next
        RenderSelectionState()
        Dim pages As Integer = Math.Max(1, CInt(Math.Ceiling(_totalCount / CDbl(_pageSize))))
        lblSummary.Text = _totalCount.ToString() & " matching record(s)"
        lblPage.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & pages.ToString()
        btnPrevious.Enabled = _currentPageIndex > 0
        btnNext.Enabled = _currentPageIndex < pages - 1
    End Sub

    Private Sub SearchKeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            Reload()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub Reload()
        If _initializing Then Return
        _currentPageIndex = 0
        LoadEntries()
    End Sub

    Private Sub PageSizeChanged(sender As Object, e As EventArgs)
        If Not _initializing Then
            _pageSize = CInt(cboPageSize.SelectedItem)
            Reload()
        End If
    End Sub

    Private Sub ChangePage(offset As Integer)
        If _currentPageIndex + offset < 0 Then Return
        _currentPageIndex += offset
        LoadEntries()
    End Sub

    Private Sub ToggleSelectionMode(sender As Object, e As EventArgs)
        _selectionMode = Not _selectionMode
        If Not _selectionMode Then _selectedEntries.Clear()
        RenderSelectionState()
    End Sub

    Private Sub GridDirty(sender As Object, e As EventArgs)
        If dgvLogs.IsCurrentCellDirty AndAlso TypeOf dgvLogs.CurrentCell Is DataGridViewCheckBoxCell Then
            dgvLogs.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub GridValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If Not _selectionMode OrElse _updatingSelection OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> colSelected.Index Then Return
        Dim entry As SecurityLogEntry = TryCast(dgvLogs.Rows(e.RowIndex).Tag, SecurityLogEntry)
        If entry Is Nothing Then Return
        If Convert.ToBoolean(dgvLogs.Rows(e.RowIndex).Cells(colSelected.Index).Value) Then
            _selectedEntries(entry.LogId) = entry
        Else
            _selectedEntries.Remove(entry.LogId)
        End If
        UpdateSelectionControls()
    End Sub

    Private Sub SelectAllChanged(sender As Object, e As EventArgs)
        If Not _selectionMode OrElse _updatingSelection Then Return
        _updatingSelection = True
        For Each row As DataGridViewRow In dgvLogs.Rows
            Dim entry As SecurityLogEntry = TryCast(row.Tag, SecurityLogEntry)
            If entry Is Nothing Then Continue For
            If chkSelectAll.Checked Then
                _selectedEntries(entry.LogId) = entry
            Else
                _selectedEntries.Remove(entry.LogId)
            End If
            row.Cells(colSelected.Index).Value = chkSelectAll.Checked
        Next
        dgvLogs.EndEdit()
        _updatingSelection = False
        dgvLogs.Invalidate()
        UpdateSelectionControls()
    End Sub

    Private Sub RenderSelectionState()
        _updatingSelection = True
        For Each row As DataGridViewRow In dgvLogs.Rows
            Dim entry As SecurityLogEntry = TryCast(row.Tag, SecurityLogEntry)
            row.Cells(colSelected.Index).Value = entry IsNot Nothing AndAlso _selectedEntries.ContainsKey(entry.LogId)
            row.Cells(colSelected.Index).ReadOnly = False
        Next
        _updatingSelection = False
        UpdateSelectionControls()
    End Sub

    Private Sub UpdateSelectionControls()
        _updatingSelection = True
        Dim rowsWithEntries As Integer = dgvLogs.Rows.Cast(Of DataGridViewRow)().Count(Function(row) row.Tag IsNot Nothing)
        chkSelectAll.Checked = _selectionMode AndAlso rowsWithEntries > 0 AndAlso
            dgvLogs.Rows.Cast(Of DataGridViewRow)().Where(Function(row) row.Tag IsNot Nothing).
            All(Function(row) _selectedEntries.ContainsKey(DirectCast(row.Tag, SecurityLogEntry).LogId))
        colSelected.Visible = _selectionMode
        chkSelectAll.Visible = _selectionMode
        btnDeleteSelected.Visible = _selectionMode AndAlso _selectedEntries.Count > 0
        btnDeleteSelected.Enabled = btnDeleteSelected.Visible
        btnDeleteSelected.Text = If(_selectedEntries.Count = 0, "Delete Selected",
                                    "Delete Selected (" & _selectedEntries.Count.ToString() & ")")
        btnToggleSelectionMode.Text = If(_selectionMode, "Cancel Selection", "Select")
        _updatingSelection = False
        PositionSelectAllCheckbox()
    End Sub

    Private Sub PositionSelectAllCheckbox()
        If Not _selectionMode Then
            chkSelectAll.Visible = False
            Return
        End If
        Dim bounds As Rectangle = dgvLogs.GetCellDisplayRectangle(colSelected.Index, -1, True)
        chkSelectAll.Location = New Point(bounds.X + Math.Max(0, (bounds.Width - chkSelectAll.Width) \ 2), bounds.Y + 11)
        chkSelectAll.BringToFront()
    End Sub

    Private Sub DeleteSelected(sender As Object, e As EventArgs)
        If _selectedEntries.Count = 0 Then Return
        If MessageBox.Show("Delete " & _selectedEntries.Count.ToString() &
                           " selected security log record(s)? This cannot be undone.",
                           "Delete Security Logs", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return
        Try
            Dim deletedCount As Integer = SecurityLogService.DeleteEntries(_selectedEntries.Values.ToList())
            _selectedEntries.Clear()
            _selectionMode = False
            MessageBox.Show(deletedCount.ToString() & " security log record(s) deleted.", "Security Logs",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadEntries()
        Catch ex As Exception
            MessageBox.Show("Could not delete security logs: " & ex.Message, "Security Logs",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
