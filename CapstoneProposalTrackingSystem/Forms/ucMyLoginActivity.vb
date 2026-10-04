Imports System.Collections.Generic
Imports System.Linq

Public Class ucMyLoginActivity
    Inherits UserControl

    Private ReadOnly _pageSizeOptions As Integer() = {10, 25, 50, 100}
    Private ReadOnly _selectedEntries As New Dictionary(Of String, LoginActivityEntry)
    Private _currentPageIndex As Integer
    Private _pageSize As Integer = 25
    Private _totalCount As Integer
    Private _selectionMode As Boolean
    Private _updatingSelection As Boolean
    Private _initializing As Boolean = True

    Private ReadOnly lblHeader As New Label()
    Private ReadOnly lblSubtitle As New Label()
    Private ReadOnly cboEvent As New ComboBox()
    Private ReadOnly dtpFrom As New DateTimePicker()
    Private ReadOnly dtpTo As New DateTimePicker()
    Private ReadOnly btnRefresh As New Button()
    Private ReadOnly btnSelect As New Button()
    Private ReadOnly btnDeleteSelected As New Button()
    Private ReadOnly btnActiveSessions As New Button()
    Private ReadOnly chkSelectAll As New CheckBox()
    Private ReadOnly dgvActivity As New DataGridView()
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
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 4}
        Me.AutoScaleMode = AutoScaleMode.Dpi
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 96.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 80.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        Dim headerPanel As New Panel With {.Dock = DockStyle.Fill}
        Dim headerText As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 2}
        headerText.RowStyles.Add(New RowStyle(SizeType.Absolute, 60.0F))
        headerText.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        lblHeader.Text = "My Login Activity"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblSubtitle.Text = "Review your successful sign-in and sign-out history."
        lblSubtitle.Dock = DockStyle.Fill
        lblSubtitle.Font = New Font("Segoe UI", 9.0F)
        lblSubtitle.ForeColor = Color.DimGray
        headerText.Controls.Add(lblHeader, 0, 0)
        headerText.Controls.Add(lblSubtitle, 0, 1)
        headerPanel.Controls.Add(headerText)

        Dim actions As New FlowLayoutPanel With {.Dock = DockStyle.Right, .Width = 390, .FlowDirection = FlowDirection.RightToLeft, .WrapContents = False, .Padding = New Padding(0, 2, 0, 0)}
        ConfigureButton(btnRefresh, "Refresh", 90)
        ConfigureButton(btnSelect, "Select", 100)
        ConfigureButton(btnActiveSessions, "Active Sessions", 125)
        btnDeleteSelected.Text = "Delete Selected"
        btnDeleteSelected.Size = New Size(180, 28)
        btnDeleteSelected.FlatStyle = FlatStyle.Flat
        btnDeleteSelected.FlatAppearance.BorderSize = 0
        btnDeleteSelected.BackColor = Color.FromArgb(166, 58, 58)
        btnDeleteSelected.ForeColor = Color.White
        btnDeleteSelected.UseVisualStyleBackColor = False
        btnDeleteSelected.Visible = False
        btnDeleteSelected.Enabled = False
        actions.Controls.Add(btnRefresh)
        actions.Controls.Add(btnSelect)
        actions.Controls.Add(btnActiveSessions)
        actions.Controls.Add(btnDeleteSelected)
        headerPanel.Controls.Add(actions)

        Dim filters As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = True, .Padding = New Padding(0, 3, 0, 0)}
        cboEvent.DropDownStyle = ComboBoxStyle.DropDownList
        cboEvent.Width = 145
        cboEvent.Items.AddRange({"All events", "Login", "Logout"})
        cboEvent.SelectedIndex = 0
        dtpFrom.Format = DateTimePickerFormat.Short
        dtpFrom.Value = DateTime.Today.AddDays(-30)
        dtpTo.Format = DateTimePickerFormat.Short
        dtpTo.Value = DateTime.Today
        filters.Controls.Add(New Label With {.Text = "Event", .AutoSize = True, .Margin = New Padding(0, 6, 5, 0)})
        filters.Controls.Add(cboEvent)
        filters.Controls.Add(New Label With {.Text = "From", .AutoSize = True, .Margin = New Padding(12, 6, 5, 0)})
        filters.Controls.Add(dtpFrom)
        filters.Controls.Add(New Label With {.Text = "To", .AutoSize = True, .Margin = New Padding(12, 6, 5, 0)})
        filters.Controls.Add(dtpTo)

        SetupGrid()

        Dim footer As New Panel With {.Dock = DockStyle.Fill}
        lblSummary.Dock = DockStyle.Left
        lblSummary.Width = 220
        lblSummary.ForeColor = Color.DimGray
        lblSummary.TextAlign = ContentAlignment.MiddleLeft
        Dim pagination As New FlowLayoutPanel With {.Dock = DockStyle.Right, .Width = 370, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False, .Padding = New Padding(0, 7, 0, 0)}
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList
        cboPageSize.Width = 82
        cboPageSize.Items.AddRange(_pageSizeOptions.Cast(Of Object)().ToArray())
        cboPageSize.SelectedItem = _pageSize
        btnPrevious.Text = "Previous"
        btnPrevious.Size = New Size(78, 28)
        btnNext.Text = "Next"
        btnNext.Size = New Size(62, 28)
        lblPage.AutoSize = False
        lblPage.Width = 90
        lblPage.TextAlign = ContentAlignment.MiddleCenter
        pagination.Controls.Add(cboPageSize)
        pagination.Controls.Add(btnPrevious)
        pagination.Controls.Add(lblPage)
        pagination.Controls.Add(btnNext)
        footer.Controls.Add(lblSummary)
        footer.Controls.Add(pagination)

        chkSelectAll.Text = ""
        chkSelectAll.Size = New Size(18, 18)
        chkSelectAll.BackColor = Color.Transparent
        chkSelectAll.Visible = False
        dgvActivity.Controls.Add(chkSelectAll)
        Controls.Add(layout)
        layout.Controls.Add(headerPanel, 0, 0)
        layout.Controls.Add(filters, 0, 1)
        layout.Controls.Add(dgvActivity, 0, 2)
        layout.Controls.Add(footer, 0, 3)

        AddHandler btnRefresh.Click, Sub() Reload()
        AddHandler btnActiveSessions.Click, AddressOf ShowActiveSessions
        AddHandler btnSelect.Click, AddressOf ToggleSelection
        AddHandler btnDeleteSelected.Click, AddressOf DeleteSelected
        AddHandler cboEvent.SelectedIndexChanged, Sub() Reload()
        AddHandler dtpFrom.ValueChanged, Sub() Reload()
        AddHandler dtpTo.ValueChanged, Sub() Reload()
        AddHandler cboPageSize.SelectedIndexChanged, AddressOf PageSizeChanged
        AddHandler btnPrevious.Click, Sub() ChangePage(-1)
        AddHandler btnNext.Click, Sub() ChangePage(1)
        AddHandler dgvActivity.CurrentCellDirtyStateChanged, AddressOf GridDirty
        AddHandler dgvActivity.CellValueChanged, AddressOf GridValueChanged
        AddHandler chkSelectAll.CheckedChanged, AddressOf SelectAllChanged
        AddHandler dgvActivity.Resize, Sub() PositionSelectAll()
        AddHandler dgvActivity.Scroll, Sub() PositionSelectAll()
        AddHandler dgvActivity.ColumnWidthChanged, Sub() PositionSelectAll()
    End Sub

    Private Sub ShowActiveSessions(sender As Object, e As EventArgs)
        If Session.CurrentUser Is Nothing Then Return
        Try
            Using dialog As New frmActiveSessions(Session.CurrentUser.UserId, Session.SessionId)
                dialog.ShowDialog(FindForm())
            End Using
        Catch ex As Exception
            MessageBox.Show("Could not load active sessions: " & ex.Message,
                            "Active Sessions", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfigureButton(button As Button, text As String, width As Integer)
        button.Text = text
        button.Size = New Size(width, 28)
        button.Margin = New Padding(8, 0, 0, 0)
        button.UseVisualStyleBackColor = True
    End Sub

    Private Sub SetupGrid()
        dgvActivity.Dock = DockStyle.Fill
        dgvActivity.AutoGenerateColumns = False
        dgvActivity.AllowUserToAddRows = False
        dgvActivity.AllowUserToDeleteRows = False
        dgvActivity.RowHeadersVisible = False
        dgvActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvActivity.BackgroundColor = SystemColors.Window
        dgvActivity.BorderStyle = BorderStyle.None
        dgvActivity.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvActivity.GridColor = Color.FromArgb(235, 235, 235)
        dgvActivity.EnableHeadersVisualStyles = False
        dgvActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvActivity.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvActivity.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvActivity.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvActivity.ColumnHeadersHeight = 40
        dgvActivity.RowTemplate.Height = 48
        dgvActivity.DefaultCellStyle.Font = New Font("Segoe UI", 9.5F)
        dgvActivity.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvActivity.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvActivity.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)
        colSelected.Name = "colSelected"
        colSelected.HeaderText = ""
        colSelected.Width = 38
        colSelected.Visible = False
        dgvActivity.Columns.Add(colSelected)
        dgvActivity.Columns.Add("colEvent", "Event")
        dgvActivity.Columns.Add("colDate", "Date and time")
        dgvActivity.Columns.Add("colDevice", "Device")
        dgvActivity.Columns.Add("colIp", "IP address")
        dgvActivity.Columns("colEvent").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvActivity.Columns("colEvent").FillWeight = 16.0F
        dgvActivity.Columns("colEvent").MinimumWidth = 110
        dgvActivity.Columns("colDate").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvActivity.Columns("colDate").FillWeight = 30.0F
        dgvActivity.Columns("colDate").MinimumWidth = 190
        dgvActivity.Columns("colDevice").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvActivity.Columns("colDevice").FillWeight = 27.0F
        dgvActivity.Columns("colDevice").MinimumWidth = 170
        dgvActivity.Columns("colIp").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        dgvActivity.Columns("colIp").FillWeight = 27.0F
        dgvActivity.Columns("colIp").MinimumWidth = 150
    End Sub

    Private Sub LoadEntries()
        If Session.CurrentUser Is Nothing Then Return
        Dim entries As List(Of LoginActivityEntry) =
            LoginActivityService.GetForUser(
                Session.CurrentUser.UserId,
                If(cboEvent.SelectedIndex <= 0, Nothing, cboEvent.Text),
                dtpFrom.Value.Date,
                dtpTo.Value.Date.AddDays(1),
                _currentPageIndex,
                _pageSize,
                _totalCount)
        dgvActivity.Rows.Clear()
        For Each entry As LoginActivityEntry In entries
            Dim rowIndex As Integer = dgvActivity.Rows.Add(False, entry.EventType, entry.CreatedAt.ToString("MMMM d, yyyy h:mm:ss tt"), entry.DeviceName, entry.IpAddress)
            dgvActivity.Rows(rowIndex).Tag = entry
        Next
        Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(_totalCount / CDbl(_pageSize))))
        lblSummary.Text = _totalCount.ToString() & " matching record(s)"
        lblPage.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & pageCount.ToString()
        btnPrevious.Enabled = _currentPageIndex > 0
        btnNext.Enabled = _currentPageIndex < pageCount - 1
        RenderSelectionState()
    End Sub

    Private Sub Reload()
        If _initializing Then Return
        _currentPageIndex = 0
        LoadEntries()
    End Sub

    Private Sub PageSizeChanged(sender As Object, e As EventArgs)
        If _initializing OrElse cboPageSize.SelectedItem Is Nothing Then Return
        _pageSize = CInt(cboPageSize.SelectedItem)
        Reload()
    End Sub

    Private Sub ChangePage(offset As Integer)
        If _currentPageIndex + offset < 0 Then Return
        _currentPageIndex += offset
        LoadEntries()
    End Sub

    Private Function EntryKey(entry As LoginActivityEntry) As String
        Return entry.ActivityId.ToString()
    End Function

    Private Sub ToggleSelection(sender As Object, e As EventArgs)
        _selectionMode = Not _selectionMode
        If Not _selectionMode Then _selectedEntries.Clear()
        colSelected.Visible = _selectionMode
        RenderSelectionState()
    End Sub

    Private Sub GridDirty(sender As Object, e As EventArgs)
        If dgvActivity.IsCurrentCellDirty AndAlso TypeOf dgvActivity.CurrentCell Is DataGridViewCheckBoxCell Then
            dgvActivity.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub GridValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If _updatingSelection OrElse Not _selectionMode OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> colSelected.Index Then Return
        Dim entry As LoginActivityEntry = TryCast(dgvActivity.Rows(e.RowIndex).Tag, LoginActivityEntry)
        If entry Is Nothing Then Return
        If Convert.ToBoolean(dgvActivity.Rows(e.RowIndex).Cells(colSelected.Index).Value) Then
            _selectedEntries(EntryKey(entry)) = entry
        Else
            _selectedEntries.Remove(EntryKey(entry))
        End If
        UpdateSelectionControls()
    End Sub

    Private Sub SelectAllChanged(sender As Object, e As EventArgs)
        If _updatingSelection OrElse Not _selectionMode Then Return
        _updatingSelection = True
        For Each row As DataGridViewRow In dgvActivity.Rows
            Dim entry As LoginActivityEntry = TryCast(row.Tag, LoginActivityEntry)
            If entry Is Nothing Then Continue For
            If chkSelectAll.Checked Then
                _selectedEntries(EntryKey(entry)) = entry
            Else
                _selectedEntries.Remove(EntryKey(entry))
            End If
            row.Cells(colSelected.Index).Value = chkSelectAll.Checked
        Next
        _updatingSelection = False
        RenderSelectionState()
    End Sub

    Private Sub RenderSelectionState()
        _updatingSelection = True
        For Each row As DataGridViewRow In dgvActivity.Rows
            Dim entry As LoginActivityEntry = TryCast(row.Tag, LoginActivityEntry)
            row.Cells(colSelected.Index).Value = entry IsNot Nothing AndAlso _selectedEntries.ContainsKey(EntryKey(entry))
            row.Cells(colSelected.Index).ReadOnly = False
        Next
        _updatingSelection = False
        UpdateSelectionControls()
        PositionSelectAll()
    End Sub

    Private Sub UpdateSelectionControls()
        _updatingSelection = True
        Dim hasRows As Boolean = dgvActivity.Rows.Cast(Of DataGridViewRow)().Any(Function(row) row.Tag IsNot Nothing)
        chkSelectAll.Checked = _selectionMode AndAlso hasRows AndAlso dgvActivity.Rows.Cast(Of DataGridViewRow)().All(
            Function(row) row.Tag Is Nothing OrElse _selectedEntries.ContainsKey(EntryKey(DirectCast(row.Tag, LoginActivityEntry))))
        chkSelectAll.Visible = _selectionMode
        btnDeleteSelected.Visible = _selectionMode AndAlso _selectedEntries.Count > 0
        btnDeleteSelected.Enabled = btnDeleteSelected.Visible
        btnDeleteSelected.Text = If(_selectedEntries.Count = 0, "Delete Selected", "Delete Selected (" & _selectedEntries.Count.ToString() & ")")
        btnSelect.Text = If(_selectionMode, "Cancel Selection", "Select")
        _updatingSelection = False
    End Sub

    Private Sub PositionSelectAll()
        If Not _selectionMode Then
            chkSelectAll.Visible = False
            Return
        End If
        Dim bounds As Rectangle = dgvActivity.GetCellDisplayRectangle(colSelected.Index, -1, True)
        chkSelectAll.Location = New Point(bounds.Left + 10, bounds.Top + 11)
        chkSelectAll.Visible = True
        chkSelectAll.BringToFront()
    End Sub

    Private Sub DeleteSelected(sender As Object, e As EventArgs)
        If _selectedEntries.Count = 0 OrElse Session.CurrentUser Is Nothing Then Return
        If MessageBox.Show("Delete " & _selectedEntries.Count.ToString() & " selected login record(s)? This cannot be undone.",
                           "Confirm Login Activity Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return
        Try
            Dim deletedCount As Integer = LoginActivityService.DeleteForUser(Session.CurrentUser.UserId, _selectedEntries.Values.ToList())
            _selectedEntries.Clear()
            MessageBox.Show(deletedCount.ToString() & " login record(s) deleted successfully.")
            LoadEntries()
        Catch ex As Exception
            MessageBox.Show("Could not delete login activity: " & ex.Message, "My Login Activity", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
