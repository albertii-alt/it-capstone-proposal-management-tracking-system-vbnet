Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Linq

Public Class ucManageAccounts
    Inherits UserControl

    Private _users As New List(Of User)
    Private _filteredUsers As New List(Of User)
    Private _initializing As Boolean = False
    Private _paginationControlsReady As Boolean = False
    Private _currentPageIndex As Integer = 0
    Private _pageSize As Integer = 25

    Private WithEvents lblHeader As New Label()
    Private WithEvents lblHeaderSubtitle As New Label()
    Private WithEvents cboRoleFilter As New ComboBox()
    Private WithEvents cboStatusFilter As New ComboBox()
    Private WithEvents lblSearch As New Label()
    Private WithEvents txtSearch As New TextBox()
    Private WithEvents dgvUsers As New DataGridView()
    Private ReadOnly pnlPagination As New Panel()
    Private ReadOnly flpPaginationControls As New FlowLayoutPanel()
    Private WithEvents lblPaginationSummary As New Label()
    Private WithEvents lblPageIndicator As New Label()
    Private WithEvents cboPageSize As New ComboBox()
    Private WithEvents btnPreviousPage As New Button()
    Private WithEvents btnNextPage As New Button()
    Private WithEvents btnAddUser As New Button()
    Private WithEvents btnRefresh As New Button()
    Private ReadOnly colActions As New DataGridViewButtonColumn()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim headerPanel As New Panel()
        Dim headerTextLayout As New TableLayoutPanel()
        Dim headerActions As New FlowLayoutPanel()
        Dim filterPanel As New FlowLayoutPanel()
        Dim paginationControls As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Name = "ucManageAccounts"

        layout.Name = "tlpManageAccounts"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 4
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 96.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Manage Accounts"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)

        lblHeaderSubtitle.Name = "lblHeaderSubtitle"
        lblHeaderSubtitle.Text = "Create accounts, update roles, and manage access."
        lblHeaderSubtitle.Dock = DockStyle.Fill
        lblHeaderSubtitle.TextAlign = ContentAlignment.MiddleLeft
        lblHeaderSubtitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblHeaderSubtitle.ForeColor = Color.DimGray

        headerTextLayout.Name = "tlpManageAccountsHeaderText"
        headerTextLayout.Dock = DockStyle.Fill
        headerTextLayout.ColumnCount = 1
        headerTextLayout.RowCount = 2
        headerTextLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 60.0F))
        headerTextLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        headerTextLayout.Controls.Add(lblHeader, 0, 0)
        headerTextLayout.Controls.Add(lblHeaderSubtitle, 0, 1)

        headerPanel.Name = "pnlManageAccountsHeader"
        headerPanel.Dock = DockStyle.Fill

        headerActions.Name = "flpManageAccountsHeaderActions"
        headerActions.Dock = DockStyle.Right
        headerActions.Width = 280
        headerActions.FlowDirection = FlowDirection.RightToLeft
        headerActions.WrapContents = False
        headerActions.Padding = New Padding(0, 2, 0, 0)

        btnRefresh.Name = "btnRefresh"
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(100, 28)
        btnRefresh.Margin = New Padding(8, 0, 0, 0)
        btnRefresh.UseVisualStyleBackColor = True

        btnAddUser.Name = "btnAddUser"
        btnAddUser.Text = "+ Add Account"
        btnAddUser.Size = New Size(150, 28)
        btnAddUser.FlatStyle = FlatStyle.Flat
        btnAddUser.FlatAppearance.BorderSize = 0
        btnAddUser.BackColor = Color.FromArgb(246, 201, 69)
        btnAddUser.ForeColor = Color.FromArgb(122, 31, 46)
        btnAddUser.UseVisualStyleBackColor = False

        headerActions.Controls.Add(btnRefresh)
        headerActions.Controls.Add(btnAddUser)
        headerPanel.Controls.Add(headerTextLayout)
        headerPanel.Controls.Add(headerActions)

        filterPanel.Name = "flpAccountFilters"
        filterPanel.Dock = DockStyle.Fill
        filterPanel.FlowDirection = FlowDirection.LeftToRight
        filterPanel.WrapContents = True
        filterPanel.Padding = New Padding(0, 8, 0, 0)

        cboRoleFilter.Name = "cboRoleFilter"
        cboRoleFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboRoleFilter.Width = 150
        cboRoleFilter.Margin = New Padding(0, 0, 12, 0)

        cboStatusFilter.Name = "cboStatusFilter"
        cboStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cboStatusFilter.Width = 150
        cboStatusFilter.Margin = New Padding(0, 0, 12, 0)

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

        filterPanel.Controls.Add(cboRoleFilter)
        filterPanel.Controls.Add(cboStatusFilter)
        filterPanel.Controls.Add(lblSearch)
        filterPanel.Controls.Add(txtSearch)

        dgvUsers.Name = "dgvUsers"
        dgvUsers.Dock = DockStyle.Fill
        dgvUsers.AllowUserToAddRows = False
        dgvUsers.AllowUserToDeleteRows = False
        dgvUsers.AllowUserToResizeRows = False
        dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvUsers.BackgroundColor = SystemColors.Window
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvUsers.MultiSelect = False
        dgvUsers.ReadOnly = True
        dgvUsers.RowHeadersVisible = False
        dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        pnlPagination.Name = "pnlAccountPagination"
        pnlPagination.Dock = DockStyle.Fill
        pnlPagination.Height = 44
        pnlPagination.BackColor = SystemColors.Control

        lblPaginationSummary.Name = "lblPaginationSummary"
        lblPaginationSummary.Dock = DockStyle.Left
        lblPaginationSummary.Width = 220
        lblPaginationSummary.Text = "Showing 0 accounts"
        lblPaginationSummary.ForeColor = Color.DimGray
        lblPaginationSummary.TextAlign = ContentAlignment.MiddleLeft

        paginationControls.Name = "flpAccountPaginationControls"
        paginationControls.Dock = DockStyle.Right
        paginationControls.Width = 330
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
        layout.Controls.Add(dgvUsers, 0, 2)
        layout.Controls.Add(pnlPagination, 0, 3)

        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
        _paginationControlsReady = True
    End Sub

    Private Sub ucManageAccounts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _initializing = True
        cboRoleFilter.Items.AddRange({"All Roles", "Student", "Admin", "Super Admin"})
        cboStatusFilter.Items.AddRange({"All Statuses", "Active", "Inactive"})
        cboRoleFilter.SelectedIndex = 0
        cboStatusFilter.SelectedIndex = 0
        SetupGridColumns()
        _initializing = False

        If FetchUsers() Then
            ApplyFilters()
        End If
    End Sub

    Private Sub SetupGridColumns()
        dgvUsers.AutoGenerateColumns = False
        dgvUsers.Columns.Clear()
        dgvUsers.Columns.Add("colFullName", "Full Name")
        dgvUsers.Columns.Add("colUsername", "Username")
        dgvUsers.Columns.Add("colEmail", "Email")
        dgvUsers.Columns.Add("colRole", "Role")
        dgvUsers.Columns.Add("colStatus", "Status")

        dgvUsers.Columns("colFullName").FillWeight = 125.0F
        dgvUsers.Columns("colUsername").FillWeight = 100.0F
        dgvUsers.Columns("colEmail").FillWeight = 145.0F
        dgvUsers.Columns("colRole").FillWeight = 65.0F
        dgvUsers.Columns("colStatus").FillWeight = 75.0F

        dgvUsers.EnableHeadersVisualStyles = False
        dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        dgvUsers.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        dgvUsers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        dgvUsers.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgvUsers.ColumnHeadersHeight = 40
        dgvUsers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgvUsers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgvUsers.GridColor = Color.FromArgb(235, 235, 235)
        dgvUsers.BorderStyle = BorderStyle.None
        dgvUsers.RowTemplate.Height = 48
        dgvUsers.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        dgvUsers.DefaultCellStyle.Font = New Font("Segoe UI", 9.5F, FontStyle.Regular)
        dgvUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        dgvUsers.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)

        colActions.Name = "colActions"
        colActions.HeaderText = String.Empty
        colActions.Text = "⋮"
        colActions.UseColumnTextForButtonValue = True
        colActions.Width = 50
        colActions.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        colActions.FlatStyle = FlatStyle.Flat
        colActions.SortMode = DataGridViewColumnSortMode.NotSortable
        colActions.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        dgvUsers.Columns.Add(colActions)
    End Sub

    Private Sub dgvUsers_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgvUsers.CellPainting
        If e.ColumnIndex <> dgvUsers.Columns("colStatus").Index OrElse e.RowIndex < 0 OrElse e.Value Is Nothing Then
            Return
        End If

        e.PaintBackground(e.ClipBounds, True)

        Dim statusText As String = e.Value.ToString()
        Dim badgeColor As Color
        Select Case statusText
            Case "Active"
                badgeColor = Color.FromArgb(76, 154, 106)
            Case "Inactive"
                badgeColor = Color.FromArgb(166, 58, 58)
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
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit

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

        Using textBrush As New SolidBrush(Color.White)
            Using textFormat As New StringFormat()
                textFormat.Alignment = StringAlignment.Center
                textFormat.LineAlignment = StringAlignment.Center
                e.Graphics.DrawString(statusText, e.CellStyle.Font, textBrush, badgeBounds, textFormat)
            End Using
        End Using

        e.Handled = True
    End Sub

    Private Function FetchUsers() As Boolean
        Try
            _users = UserService.GetAllUsers()
            Return True
        Catch ex As Exception
            MessageBox.Show("Could not load accounts: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub ApplyFilters()
        If _initializing Then
            Return
        End If

        Dim usersToShow As IEnumerable(Of User) = _users

        If cboRoleFilter.SelectedIndex = 1 Then
            usersToShow = usersToShow.Where(Function(user) user.Role = UserRole.Student)
        ElseIf cboRoleFilter.SelectedIndex = 2 Then
            usersToShow = usersToShow.Where(Function(user) user.Role = UserRole.Admin)
        ElseIf cboRoleFilter.SelectedIndex = 3 Then
            usersToShow = usersToShow.Where(Function(user) user.Role = UserRole.SuperAdmin)
        End If

        Dim searchText As String = txtSearch.Text.Trim()
        If searchText.Length > 0 Then
            usersToShow = usersToShow.Where(
                Function(user) If(user.FullName, String.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                If(user.Username, String.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                If(user.Email, String.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
        End If

        If cboStatusFilter.SelectedIndex = 1 Then
            usersToShow = usersToShow.Where(Function(user) user.IsActive)
        ElseIf cboStatusFilter.SelectedIndex = 2 Then
            usersToShow = usersToShow.Where(Function(user) Not user.IsActive)
        End If

        _filteredUsers = usersToShow.ToList()
        _currentPageIndex = 0
        RenderCurrentPage()
    End Sub

    Private Sub RenderCurrentPage()
        Dim totalCount As Integer = _filteredUsers.Count
        Dim totalPages As Integer = CInt(Math.Ceiling(totalCount / CDbl(_pageSize)))
        _currentPageIndex = Math.Max(0, Math.Min(_currentPageIndex, Math.Max(0, totalPages - 1)))

        Dim firstIndex As Integer = _currentPageIndex * _pageSize
        Dim pageUsers As List(Of User) = _filteredUsers.Skip(firstIndex).Take(_pageSize).ToList()

        dgvUsers.Rows.Clear()
        For Each user As User In pageUsers
            Dim rowIndex As Integer = dgvUsers.Rows.Add(
                user.FullName,
                user.Username,
                user.Email,
                If(user.Role = UserRole.SuperAdmin, "Super Admin", user.Role.ToString()),
                If(user.IsActive, "Active", "Inactive"))
            dgvUsers.Rows(rowIndex).Tag = user.UserId
        Next

        If totalCount = 0 Then
            lblPaginationSummary.Text = "Showing 0 accounts"
            lblPageIndicator.Text = "Page 0 of 0"
        Else
            Dim firstDisplayed As Integer = firstIndex + 1
            Dim lastDisplayed As Integer = Math.Min(firstIndex + _pageSize, totalCount)
            lblPaginationSummary.Text = "Showing " & firstDisplayed.ToString() & "-" & lastDisplayed.ToString() &
                                        " of " & totalCount.ToString() & " accounts"
            lblPageIndicator.Text = "Page " & (_currentPageIndex + 1).ToString() & " of " & totalPages.ToString()
        End If

        btnPreviousPage.Enabled = _currentPageIndex > 0
        btnNextPage.Enabled = _currentPageIndex < totalPages - 1
    End Sub

    Private Sub dgvUsers_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvUsers.CellContentClick
        If e.ColumnIndex <> colActions.Index OrElse e.RowIndex < 0 Then
            Return
        End If

        Dim userId As Integer = CInt(dgvUsers.Rows(e.RowIndex).Tag)
        Dim theUser As User = _users.FirstOrDefault(Function(user) user.UserId = userId)
        If theUser Is Nothing Then
            Return
        End If

        Dim contextMenu As New ContextMenuStrip()
        Dim editItem As New ToolStripMenuItem("Edit")
        Dim resetPasswordItem As New ToolStripMenuItem("Reset Password")
        Dim activeItem As New ToolStripMenuItem(If(theUser.IsActive, "Deactivate", "Activate"))
        Dim deleteItem As New ToolStripMenuItem("Delete")
        Dim isSuperAdminTarget As Boolean = theUser.Role = UserRole.SuperAdmin
        Dim isSuperAdminActor As Boolean = Session.CurrentUser IsNot Nothing AndAlso Session.CurrentUser.Role = UserRole.SuperAdmin

        editItem.Enabled = Not isSuperAdminTarget OrElse isSuperAdminActor
        resetPasswordItem.Enabled = Not isSuperAdminTarget OrElse isSuperAdminActor
        activeItem.Enabled = Not (isSuperAdminTarget AndAlso theUser.IsActive)
        deleteItem.Enabled = Not isSuperAdminTarget

        AddHandler editItem.Click, Sub(menuSender As Object, menuEventArgs As EventArgs)
            Using accountForm As New frmAccountForm(existingUser:=theUser)
                If accountForm.ShowDialog(Me.FindForm()) = DialogResult.OK AndAlso FetchUsers() Then
                    ApplyFilters()
                End If
            End Using
        End Sub

        AddHandler resetPasswordItem.Click, Sub(menuSender As Object, menuEventArgs As EventArgs)
            Using resetPasswordForm As New frmResetPassword(theUser.UserId, theUser.FullName)
                resetPasswordForm.ShowDialog(Me.FindForm())
            End Using
        End Sub

        AddHandler activeItem.Click, Sub(menuSender As Object, menuEventArgs As EventArgs)
            Dim newActiveValue As Boolean = Not theUser.IsActive
            If theUser.Role = UserRole.SuperAdmin AndAlso Not newActiveValue Then
                MessageBox.Show("The Super Admin account cannot be deactivated.")
                Return
            End If

            If Not newActiveValue AndAlso
               (Session.CurrentUser Is Nothing OrElse theUser.UserId = Session.CurrentUser.UserId) Then
                MessageBox.Show("You cannot deactivate your own account.")
                Return
            End If

            Dim actionText As String = If(newActiveValue, "Activate", "Deactivate")
            Dim confirmation As DialogResult = MessageBox.Show(
                actionText & " " & theUser.FullName & "?",
                "Confirm Account Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

            If confirmation <> DialogResult.Yes Then
                Return
            End If

            Try
                UserService.SetUserActive(theUser.UserId, newActiveValue, Session.CurrentUser.UserId)
                If FetchUsers() Then
                    ApplyFilters()
                End If
            Catch ex As ApplicationException
                MessageBox.Show(ex.Message)
            End Try
        End Sub

        AddHandler deleteItem.Click, Sub(menuSender As Object, menuEventArgs As EventArgs)
            If theUser.Role = UserRole.SuperAdmin Then
                MessageBox.Show("The Super Admin account cannot be deleted.")
                Return
            End If

            If Session.CurrentUser Is Nothing OrElse theUser.UserId = Session.CurrentUser.UserId Then
                MessageBox.Show("You cannot delete your own account.")
                Return
            End If

            Dim confirmation As DialogResult = MessageBox.Show(
                "Permanently delete the account for " & theUser.FullName & " (" & theUser.Username & ")? This cannot be undone.",
                "Confirm Permanent Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning)

            If confirmation <> DialogResult.Yes Then
                Return
            End If

            Try
                UserService.DeleteUser(theUser.UserId, Session.CurrentUser.UserId)
                MessageBox.Show("Account deleted successfully.")
                If FetchUsers() Then
                    ApplyFilters()
                End If
            Catch ex As ApplicationException
                MessageBox.Show(ex.Message)
            Catch ex As Exception
                MessageBox.Show("Could not delete the account: " & ex.Message)
            End Try
        End Sub

        contextMenu.Items.Add(editItem)
        contextMenu.Items.Add(resetPasswordItem)
        contextMenu.Items.Add(activeItem)
        contextMenu.Items.Add(New ToolStripSeparator())
        contextMenu.Items.Add(deleteItem)

        Dim cellBounds As Rectangle = dgvUsers.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, False)
        Dim menuLocation As Point = dgvUsers.PointToScreen(New Point(cellBounds.Left, cellBounds.Bottom))
        contextMenu.Show(menuLocation)
    End Sub

    Private Sub cboRoleFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboRoleFilter.SelectedIndexChanged
        ApplyFilters()
    End Sub

    Private Sub cboStatusFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStatusFilter.SelectedIndexChanged
        ApplyFilters()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
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
        Dim totalPages As Integer = CInt(Math.Ceiling(_filteredUsers.Count / CDbl(_pageSize)))
        If _currentPageIndex >= totalPages - 1 Then
            Return
        End If

        _currentPageIndex += 1
        RenderCurrentPage()
    End Sub

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs) Handles btnAddUser.Click
        Using accountForm As New frmAccountForm(existingUser:=Nothing)
            If accountForm.ShowDialog(Me.FindForm()) = DialogResult.OK AndAlso FetchUsers() Then
                ApplyFilters()
            End If
        End Using
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        If FetchUsers() Then
            ApplyFilters()
        End If
    End Sub

    Public Sub RefreshData()
        btnRefresh_Click(btnRefresh, EventArgs.Empty)
    End Sub
End Class