Imports System.Collections.Generic
Imports System.Linq

Public Class ucStudentRegistrations
    Inherits UserControl

    Private ReadOnly dgvRegistrations As New DataGridView()
    Private ReadOnly pnlGridHost As New Panel()
    Private ReadOnly lblEmptyState As New Label()
    Private ReadOnly pnlPagination As New Panel()
    Private ReadOnly lblPaginationSummary As New Label()
    Private ReadOnly lblPageIndicator As New Label()
    Private ReadOnly cboPageSize As New ComboBox()
    Private ReadOnly btnPreviousPage As New Button()
    Private ReadOnly btnNextPage As New Button()
    Private ReadOnly btnToggleSelection As New Button()
    Private ReadOnly btnApproveSelected As New Button()
    Private ReadOnly btnRejectSelected As New Button()
    Private ReadOnly chkSelectAll As New CheckBox()
    Private ReadOnly selectedUserIds As New HashSet(Of Integer)()
    Private _currentPageIndex As Integer
    Private _pageSize As Integer = 25
    Private _selectionMode As Boolean
    Private _updatingSelection As Boolean
    Private _registrations As New List(Of User)()

    Public Sub New()
        InitializeLayout()
        AddHandler Me.Load, AddressOf ucStudentRegistrations_Load
    End Sub

    Private Sub InitializeLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Padding = New Padding(20)

        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 4, .Padding = New Padding(20)}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 96.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        Dim header As New Panel With {.Dock = DockStyle.Fill}
        Dim title As New Label With {.Text = "Student Registrations", .Font = New Font("Segoe UI", 23.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45), .Dock = DockStyle.Top, .Height = 48}
        Dim subtitle As New Label With {.Text = "Review student accounts before granting access to the system.", .Font = New Font("Segoe UI", 9.0F), .ForeColor = Color.DimGray, .Dock = DockStyle.Bottom, .Height = 26}
        header.Controls.Add(subtitle)
        header.Controls.Add(title)

        Dim toolbar As New Panel With {.Dock = DockStyle.Fill}
        Dim refreshButton As New Button With {.Text = "Refresh", .Size = New Size(100, 28), .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        refreshButton.Location = New Point(toolbar.ClientSize.Width - refreshButton.Width, 4)
        AddHandler toolbar.Resize, Sub(sender, e) refreshButton.Left = toolbar.ClientSize.Width - refreshButton.Width
        AddHandler refreshButton.Click, Sub(sender, e) LoadRegistrations()
        toolbar.Controls.Add(refreshButton)
        ConfigureSelectionButton(btnToggleSelection, "Select")
        ConfigureSelectionButton(btnApproveSelected, "Approve Selected")
        ConfigureSelectionButton(btnRejectSelected, "Reject Selected")
        btnToggleSelection.Location = New Point(0, 4)
        btnApproveSelected.Location = New Point(108, 4)
        btnRejectSelected.Location = New Point(238, 4)
        btnApproveSelected.Visible = False
        btnRejectSelected.Visible = False
        AddHandler btnToggleSelection.Click, AddressOf ToggleSelection_Click
        AddHandler btnApproveSelected.Click, Sub(sender, e) ReviewSelected(True)
        AddHandler btnRejectSelected.Click, Sub(sender, e) ReviewSelected(False)
        toolbar.Controls.AddRange({btnRejectSelected, btnApproveSelected, btnToggleSelection})

        ConfigureGrid()
        pnlGridHost.Dock = DockStyle.Fill
        pnlGridHost.BackColor = SystemColors.Window
        pnlGridHost.Controls.Add(dgvRegistrations)
        ConfigureEmptyState()
        pnlGridHost.Controls.Add(lblEmptyState)
        ConfigurePagination()
        layout.Controls.Add(header, 0, 0)
        layout.Controls.Add(toolbar, 0, 1)
        layout.Controls.Add(pnlGridHost, 0, 2)
        layout.Controls.Add(pnlPagination, 0, 3)
        Me.Controls.Add(layout)
    End Sub

    Private Sub ConfigureGrid()
        dgvRegistrations.Dock = DockStyle.Fill
        dgvRegistrations.AllowUserToAddRows = False
        dgvRegistrations.AllowUserToDeleteRows = False
        dgvRegistrations.AllowUserToResizeRows = False
        dgvRegistrations.AutoGenerateColumns = False
        dgvRegistrations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        dgvRegistrations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvRegistrations.BackgroundColor = SystemColors.Window
        dgvRegistrations.BorderStyle = BorderStyle.None
        dgvRegistrations.ColumnHeadersHeight = 40
        dgvRegistrations.EnableHeadersVisualStyles = False
        dgvRegistrations.Font = New Font("Segoe UI", 9.5F)
        dgvRegistrations.ReadOnly = False
        dgvRegistrations.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvRegistrations.GridColor = Color.FromArgb(235, 235, 235)
        dgvRegistrations.BorderStyle = BorderStyle.None
        dgvRegistrations.RowTemplate.Height = 48
        dgvRegistrations.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvRegistrations.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvRegistrations.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)
        dgvRegistrations.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvRegistrations.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvRegistrations.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvRegistrations.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvRegistrations.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvRegistrations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvRegistrations.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvRegistrations.RowHeadersVisible = False
        dgvRegistrations.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvRegistrations.MultiSelect = False
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "schoolId", .HeaderText = "SCHOOL ID", .DataPropertyName = "SchoolId", .Width = 110})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "fullName", .HeaderText = "FULL NAME", .DataPropertyName = "FullName", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "email", .HeaderText = "EMAIL", .DataPropertyName = "Email", .Width = 210})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "username", .HeaderText = "USERNAME", .DataPropertyName = "Username", .Width = 130})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "registeredAt", .HeaderText = "REGISTERED", .DataPropertyName = "RegisteredAt", .Width = 145})
        Dim selectedColumn As New DataGridViewCheckBoxColumn With {.Name = "selected", .HeaderText = "", .Width = 40, .AutoSizeMode = DataGridViewAutoSizeColumnMode.None, .Visible = False}
        dgvRegistrations.Columns.Insert(0, selectedColumn)
        Dim approveColumn As New DataGridViewButtonColumn With {.Name = "approve", .HeaderText = "", .Text = "Approve", .UseColumnTextForButtonValue = True, .Width = 85, .AutoSizeMode = DataGridViewAutoSizeColumnMode.None}
        Dim rejectColumn As New DataGridViewButtonColumn With {.Name = "reject", .HeaderText = "", .Text = "Reject", .UseColumnTextForButtonValue = True, .Width = 75, .AutoSizeMode = DataGridViewAutoSizeColumnMode.None}
        dgvRegistrations.Columns.Add(approveColumn)
        dgvRegistrations.Columns.Add(rejectColumn)
        For Each column As DataGridViewColumn In dgvRegistrations.Columns
            column.ReadOnly = column.Name <> "selected"
        Next
        AddHandler dgvRegistrations.CellContentClick, AddressOf dgvRegistrations_CellContentClick
        AddHandler dgvRegistrations.CellValueChanged, AddressOf dgvRegistrations_CellValueChanged
        AddHandler dgvRegistrations.CurrentCellDirtyStateChanged, Sub(sender, e)
                                                                        If dgvRegistrations.IsCurrentCellDirty Then dgvRegistrations.CommitEdit(DataGridViewDataErrorContexts.Commit)
                                                                    End Sub
        dgvRegistrations.Controls.Add(chkSelectAll)
        chkSelectAll.Size = New Size(16, 16)
        chkSelectAll.Visible = False
        AddHandler chkSelectAll.CheckedChanged, AddressOf SelectAll_CheckedChanged
    End Sub

    Private Shared Sub ConfigureSelectionButton(button As Button, text As String)
        button.Text = text
        button.Size = New Size(120, 28)
        button.UseVisualStyleBackColor = True
    End Sub

    Private Sub ConfigurePagination()
        pnlPagination.Dock = DockStyle.Fill
        lblPaginationSummary.Dock = DockStyle.Left
        lblPaginationSummary.Width = 220
        lblPaginationSummary.ForeColor = Color.DimGray
        lblPaginationSummary.TextAlign = ContentAlignment.MiddleLeft
        Dim controls As New FlowLayoutPanel With {.Dock = DockStyle.Right, .Width = 330, .WrapContents = False, .Padding = New Padding(0, 7, 0, 0)}
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList
        cboPageSize.Width = 82
        cboPageSize.Items.AddRange({10, 25, 50, 100})
        cboPageSize.SelectedItem = 25
        AddHandler cboPageSize.SelectedIndexChanged, AddressOf PageSize_Changed
        btnPreviousPage.Text = "Previous"
        btnPreviousPage.Size = New Size(78, 28)
        btnNextPage.Text = "Next"
        btnNextPage.Size = New Size(62, 28)
        AddHandler btnPreviousPage.Click, Sub(sender, e) ChangePage(-1)
        AddHandler btnNextPage.Click, Sub(sender, e) ChangePage(1)
        lblPageIndicator.Size = New Size(76, 28)
        lblPageIndicator.TextAlign = ContentAlignment.MiddleCenter
        controls.Controls.AddRange({cboPageSize, btnPreviousPage, lblPageIndicator, btnNextPage})
        pnlPagination.Controls.Add(controls)
        pnlPagination.Controls.Add(lblPaginationSummary)
    End Sub

    Private Sub ConfigureEmptyState()
        lblEmptyState.Dock = DockStyle.Fill
        lblEmptyState.BackColor = SystemColors.Window
        lblEmptyState.Font = New Font("Segoe UI", 11.0F, FontStyle.Regular)
        lblEmptyState.ForeColor = Color.DimGray
        lblEmptyState.Text = "No pending student registrations." & Environment.NewLine &
                             "New registration requests will appear here for review."
        lblEmptyState.TextAlign = ContentAlignment.MiddleCenter
        lblEmptyState.Visible = False
    End Sub

    Private Sub ucStudentRegistrations_Load(sender As Object, e As EventArgs)
        LoadRegistrations()
    End Sub

    Private Sub LoadRegistrations()
        Try
            _registrations = UserService.GetPendingStudentRegistrations()
            selectedUserIds.RemoveWhere(Function(id) Not _registrations.Any(Function(registration) registration.UserId = id))
            _currentPageIndex = Math.Min(_currentPageIndex, Math.Max(0, TotalPages() - 1))
            RenderCurrentPage()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Student Registrations", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RenderCurrentPage()
        dgvRegistrations.Rows.Clear()
        Dim pageItems = _registrations.Skip(_currentPageIndex * _pageSize).Take(_pageSize).ToList()
        For Each registration As User In pageItems
                Dim rowIndex As Integer = dgvRegistrations.Rows.Add(
                    selectedUserIds.Contains(registration.UserId),
                    registration.SchoolId,
                    registration.FullName,
                    registration.Email,
                    registration.Username,
                    If(registration.RegisteredAt.HasValue, registration.RegisteredAt.Value.ToString("MMM d, yyyy h:mm tt"), String.Empty),
                    "Approve",
                    "Reject")
                dgvRegistrations.Rows(rowIndex).Tag = registration.UserId
        Next
        lblEmptyState.Visible = _registrations.Count = 0
        dgvRegistrations.Visible = _registrations.Count > 0
        Dim totalPageCount As Integer = TotalPages()
        Dim firstRecord As Integer = 0
        If _registrations.Count > 0 Then firstRecord = (_currentPageIndex * _pageSize) + 1
        Dim lastRecord As Integer = Math.Min((_currentPageIndex + 1) * _pageSize, _registrations.Count)
        lblPaginationSummary.Text = "Showing " & firstRecord.ToString() &
                                    "-" & lastRecord.ToString() &
                                    " of " & _registrations.Count.ToString() & " registrations"
        lblPageIndicator.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & totalPageCount.ToString()
        btnPreviousPage.Enabled = _currentPageIndex > 0
        btnNextPage.Enabled = _currentPageIndex < totalPageCount - 1
        btnApproveSelected.Enabled = selectedUserIds.Count > 0
        btnRejectSelected.Enabled = selectedUserIds.Count > 0
        If _selectionMode Then
            Dim currentPageItems = _registrations.Skip(_currentPageIndex * _pageSize).Take(_pageSize).ToList()
            _updatingSelection = True
            chkSelectAll.Checked = currentPageItems.Count > 0 AndAlso currentPageItems.All(Function(item) selectedUserIds.Contains(item.UserId))
            _updatingSelection = False
            PositionSelectAllCheckbox()
        End If
    End Sub

    Private Function TotalPages() As Integer
        Return Math.Max(1, CInt(Math.Ceiling(_registrations.Count / CDbl(_pageSize))))
    End Function

    Private Sub ChangePage(offset As Integer)
        Dim newIndex As Integer = _currentPageIndex + offset
        If newIndex < 0 OrElse newIndex >= TotalPages() Then Return
        _currentPageIndex = newIndex
        RenderCurrentPage()
    End Sub

    Private Sub PageSize_Changed(sender As Object, e As EventArgs)
        If cboPageSize.SelectedItem Is Nothing Then Return
        _pageSize = CInt(cboPageSize.SelectedItem)
        _currentPageIndex = 0
        RenderCurrentPage()
    End Sub

    Private Sub ToggleSelection_Click(sender As Object, e As EventArgs)
        _selectionMode = Not _selectionMode
        btnToggleSelection.Text = If(_selectionMode, "Done", "Select")
        btnApproveSelected.Visible = _selectionMode
        btnRejectSelected.Visible = _selectionMode
        chkSelectAll.Visible = _selectionMode
        dgvRegistrations.Columns("selected").Visible = _selectionMode
        RenderCurrentPage()
    End Sub

    Private Sub dgvRegistrations_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If Not _selectionMode OrElse _updatingSelection OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> dgvRegistrations.Columns("selected").Index Then Return
        Dim userId As Integer = CInt(dgvRegistrations.Rows(e.RowIndex).Tag)
        If CBool(dgvRegistrations.Rows(e.RowIndex).Cells("selected").Value) Then
            selectedUserIds.Add(userId)
        Else
            selectedUserIds.Remove(userId)
        End If
        RenderCurrentPage()
    End Sub

    Private Sub SelectAll_CheckedChanged(sender As Object, e As EventArgs)
        If _updatingSelection Then Return
        Dim pageItems = _registrations.Skip(_currentPageIndex * _pageSize).Take(_pageSize)
        For Each registration As User In pageItems
            If chkSelectAll.Checked Then selectedUserIds.Add(registration.UserId) Else selectedUserIds.Remove(registration.UserId)
        Next
        RenderCurrentPage()
    End Sub

    Private Sub PositionSelectAllCheckbox()
        If Not _selectionMode Then
            chkSelectAll.Visible = False
            Return
        End If

        Dim headerBounds As Rectangle = dgvRegistrations.GetCellDisplayRectangle(dgvRegistrations.Columns("selected").Index, -1, True)
        chkSelectAll.Location = New Point(
            headerBounds.Left + Math.Max(0, (headerBounds.Width - chkSelectAll.Width) \ 2),
            headerBounds.Top + Math.Max(0, (headerBounds.Height - chkSelectAll.Height) \ 2))
        chkSelectAll.Visible = True
    End Sub

    Private Sub ReviewSelected(approve As Boolean)
        If selectedUserIds.Count = 0 Then Return
        If MessageBox.Show("Apply this action to " & selectedUserIds.Count.ToString() & " selected registration(s)?", "Confirm Registration Review", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
        Try
            For Each userId As Integer In selectedUserIds.ToList()
                UserService.ReviewStudentRegistration(userId, approve, Session.CurrentUser.UserId, String.Empty)
            Next
            selectedUserIds.Clear()
            LoadRegistrations()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Student Registrations", MessageBoxButtons.OK, MessageBoxIcon.Error)
            LoadRegistrations()
        End Try
    End Sub

    Private Sub dgvRegistrations_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse dgvRegistrations.Rows(e.RowIndex).Tag Is Nothing Then Return
        Dim userId As Integer = CInt(dgvRegistrations.Rows(e.RowIndex).Tag)
        Dim registration As User = _registrations.FirstOrDefault(Function(item) item.UserId = userId)
        If registration Is Nothing Then Return
        If dgvRegistrations.Columns(e.ColumnIndex).Name = "selected" Then Return
        If dgvRegistrations.Columns(e.ColumnIndex).Name = "approve" Then
            Review(registration, True)
        ElseIf dgvRegistrations.Columns(e.ColumnIndex).Name = "reject" Then
            Review(registration, False)
        End If
    End Sub

    Private Sub Review(registration As User, approve As Boolean)
        Dim actionText As String = If(approve, "approve", "reject")
        Dim prompt As String = actionText.Substring(0, 1).ToUpperInvariant() & actionText.Substring(1) &
                               " the student account for " & registration.FullName & " (" & registration.SchoolId & ")?"
        If MessageBox.Show(prompt, "Confirm Registration Review", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Dim remarks As String = String.Empty
        If Not approve Then
            remarks = InputBox("Optional rejection remarks:", "Reject Registration")
        End If

        Try
            UserService.ReviewStudentRegistration(registration.UserId, approve, Session.CurrentUser.UserId, remarks)
            LoadRegistrations()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Student Registrations", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
