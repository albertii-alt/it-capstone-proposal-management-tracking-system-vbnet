Imports System.Collections.Generic
Imports System.Linq

Public Class ucStudentHistory
    Inherits UserControl

    Private ReadOnly grid As New DataGridView()
    Private ReadOnly historyItems As New List(Of HistoryItem)()
    Private ReadOnly selectedItems As New Dictionary(Of String, HistoryItem)()
    Private ReadOnly pageSummary As New Label()
    Private ReadOnly pageIndicator As New Label()
    Private ReadOnly pageSizeSelector As New ComboBox()
    Private ReadOnly previousPage As New Button()
    Private ReadOnly nextPage As New Button()
    Private ReadOnly selectButton As New Button()
    Private ReadOnly deleteSelectedButton As New Button()
    Private ReadOnly selectAllCheckBox As New CheckBox()
    Private ReadOnly selectedColumn As New DataGridViewCheckBoxColumn()
    Private selectionMode As Boolean
    Private updatingSelection As Boolean
    Private pageIndex As Integer
    Private pageSize As Integer = 25

    Private Class HistoryItem
        Public Property HistoryId As Integer
        Public Property Timestamp As DateTime
        Public Property Proposal As String
        Public Property Action As String
        Public Property Remarks As String

        Public ReadOnly Property Key As String
            Get
                Return HistoryId.ToString()
            End Get
        End Property
    End Class

    Public Sub New()
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = SystemColors.Control
        BuildLayout()
        LoadHistory()
    End Sub

    Private Sub BuildLayout()
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 3}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 112.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))
        Dim header As New Panel With {.Dock = DockStyle.Fill}
        header.Controls.Add(New Label With {.Text = "History", .Dock = DockStyle.Top, .Height = 66, .Font = New Font("Segoe UI", 23.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45), .Padding = New Padding(0, 5, 0, 0)})
        header.Controls.Add(New Label With {.Text = "Review the activity and review history for your proposals.", .Dock = DockStyle.Bottom, .Height = 26, .ForeColor = Color.DimGray})
        ConfigureButton(selectButton, "Select", 100)
        ConfigureButton(deleteSelectedButton, "Delete Selected", 160)
        deleteSelectedButton.BackColor = Color.FromArgb(166, 58, 58)
        deleteSelectedButton.ForeColor = Color.White
        deleteSelectedButton.UseVisualStyleBackColor = False
        deleteSelectedButton.Visible = False
        deleteSelectedButton.Enabled = False
        Dim actions As New FlowLayoutPanel With {
            .Dock = DockStyle.Right,
            .Width = 280,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(0, 10, 0, 0)
        }
        AddHandler selectButton.Click, AddressOf ToggleSelection
        AddHandler deleteSelectedButton.Click, AddressOf DeleteSelected
        actions.Controls.Add(selectButton)
        actions.Controls.Add(deleteSelectedButton)
        header.Controls.Add(actions)
        layout.Controls.Add(header, 0, 0)
        ConfigureGrid()
        layout.Controls.Add(grid, 0, 1)
        ConfigurePagination(layout)
        Controls.Add(layout)
    End Sub

    Private Sub ConfigureButton(button As Button, text As String, width As Integer)
        button.Text = text
        button.Size = New Size(width, 28)
        button.FlatStyle = FlatStyle.Flat
        button.FlatAppearance.BorderSize = 0
        button.UseVisualStyleBackColor = True
    End Sub

    Private Sub ConfigureGrid()
        grid.Dock = DockStyle.Fill
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.ReadOnly = False
        grid.RowHeadersVisible = False
        grid.AutoGenerateColumns = False
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        grid.ColumnHeadersHeight = 40
        grid.RowTemplate.Height = 46
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(246, 246, 246)
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 100, 100)
        grid.GridColor = Color.FromArgb(235, 235, 235)
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.DefaultCellStyle.Padding = New Padding(8, 0, 0, 0)
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(250, 240, 220)
        grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(40, 40, 40)
        selectedColumn.Name = "selected"
        selectedColumn.HeaderText = ""
        selectedColumn.Width = 38
        selectedColumn.MinimumWidth = 38
        selectedColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        selectedColumn.Visible = False
        grid.Columns.Add(selectedColumn)
        grid.Columns.Add("date", "DATE")
        grid.Columns.Add("proposal", "PROPOSAL")
        grid.Columns.Add("action", "ACTIVITY")
        grid.Columns.Add("remarks", "REMARKS")
        For Each column As DataGridViewColumn In grid.Columns
            column.ReadOnly = column IsNot selectedColumn
        Next
        grid.Controls.Add(selectAllCheckBox)
        selectAllCheckBox.Text = ""
        selectAllCheckBox.Size = New Size(18, 18)
        selectAllCheckBox.BackColor = Color.Transparent
        selectAllCheckBox.Visible = False
        AddHandler grid.CurrentCellDirtyStateChanged, AddressOf GridDirty
        AddHandler grid.CellValueChanged, AddressOf GridValueChanged
        AddHandler selectAllCheckBox.CheckedChanged, AddressOf SelectAllChanged
        AddHandler grid.Resize, Sub(sender, e) PositionSelectAll()
        AddHandler grid.Scroll, Sub(sender, e) PositionSelectAll()
        AddHandler grid.ColumnWidthChanged, Sub(sender, e) PositionSelectAll()
    End Sub

    Private Sub ConfigurePagination(layout As TableLayoutPanel)
        Dim footer As New Panel With {.Dock = DockStyle.Fill}
        pageSummary.Dock = DockStyle.Left
        pageSummary.Width = 260
        pageSummary.TextAlign = ContentAlignment.MiddleLeft
        pageSummary.ForeColor = Color.DimGray
        Dim controls As New FlowLayoutPanel With {.Dock = DockStyle.Right, .Width = 330, .WrapContents = False, .Padding = New Padding(0, 7, 0, 0)}
        pageSizeSelector.DropDownStyle = ComboBoxStyle.DropDownList
        pageSizeSelector.Width = 82
        pageSizeSelector.Items.AddRange({10, 25, 50, 100})
        pageSizeSelector.SelectedItem = pageSize
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

    Private Sub LoadHistory()
        Try
            historyItems.Clear()
            grid.Rows.Clear()
            For Each entry As StudentHistoryEntry In StudentHistoryService.GetForStudent(Session.CurrentUser.UserId)
                AddHistory(entry)
            Next
            pageIndex = 0
            RenderPage()
        Catch ex As Exception
            MessageBox.Show("Could not load your history: " & ex.Message, "History", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AddHistory(entry As StudentHistoryEntry)
        historyItems.Add(New HistoryItem With {
            .HistoryId = entry.HistoryId,
            .Timestamp = entry.RecordedAt,
            .Proposal = entry.ProposalTitle,
            .Action = entry.Action,
            .Remarks = entry.Remarks
        })
    End Sub

    Private Sub RenderPage()
        grid.Rows.Clear()
        Dim orderedItems As List(Of HistoryItem) = historyItems.OrderByDescending(Function(item) item.Timestamp).ToList()
        Dim pageItems As List(Of HistoryItem) = orderedItems.Skip(pageIndex * pageSize).Take(pageSize).ToList()
        For Each item As HistoryItem In pageItems
            Dim rowIndex As Integer = grid.Rows.Add(False, item.Timestamp.ToString("MMM d, yyyy h:mm tt"), item.Proposal, item.Action, item.Remarks)
            grid.Rows(rowIndex).Tag = item
        Next
        If orderedItems.Count = 0 Then
            grid.Rows.Add(False, "", "No records", "No activity", "Your proposal activity will appear here.")
        End If
        If orderedItems.Count = 0 Then
            pageSummary.Text = "Showing 0 history records"
            pageIndicator.Text = "Page 0 of 0"
        Else
            pageSummary.Text = "Showing " & (pageIndex * pageSize + 1).ToString() & "-" &
                               Math.Min((pageIndex + 1) * pageSize, orderedItems.Count).ToString() &
                               " of " & orderedItems.Count.ToString() & " history records"
            pageIndicator.Text = "Page " & (pageIndex + 1).ToString() & " of " &
                                 Math.Max(1, CInt(Math.Ceiling(orderedItems.Count / CDbl(pageSize)))).ToString()
        End If
        Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(orderedItems.Count / CDbl(pageSize))))
        previousPage.Enabled = pageIndex > 0
        nextPage.Enabled = pageIndex < pageCount - 1
        RenderSelectionState()
    End Sub

    Private Sub PageSizeChanged(sender As Object, e As EventArgs)
        If pageSizeSelector.SelectedItem Is Nothing Then Return
        pageSize = CInt(pageSizeSelector.SelectedItem)
        pageIndex = 0
        RenderPage()
    End Sub

    Private Sub ChangePage(offset As Integer)
        Dim target As Integer = pageIndex + offset
        Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(historyItems.Count / CDbl(pageSize))))
        If target < 0 OrElse target >= pageCount Then Return
        pageIndex = target
        RenderPage()
    End Sub

    Private Sub ToggleSelection(sender As Object, e As EventArgs)
        selectionMode = Not selectionMode
        If Not selectionMode Then selectedItems.Clear()
        selectedColumn.Visible = selectionMode
        RenderSelectionState()
    End Sub

    Private Sub GridDirty(sender As Object, e As EventArgs)
        If grid.IsCurrentCellDirty AndAlso TypeOf grid.CurrentCell Is DataGridViewCheckBoxCell Then
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub GridValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If updatingSelection OrElse Not selectionMode OrElse e.RowIndex < 0 OrElse e.ColumnIndex <> selectedColumn.Index Then Return
        Dim item As HistoryItem = TryCast(grid.Rows(e.RowIndex).Tag, HistoryItem)
        If item Is Nothing Then Return
        If Convert.ToBoolean(grid.Rows(e.RowIndex).Cells(selectedColumn.Index).Value) Then
            selectedItems(item.Key) = item
        Else
            selectedItems.Remove(item.Key)
        End If
        UpdateSelectionControls()
    End Sub

    Private Sub SelectAllChanged(sender As Object, e As EventArgs)
        If updatingSelection OrElse Not selectionMode Then Return
        updatingSelection = True
        For Each row As DataGridViewRow In grid.Rows
            Dim item As HistoryItem = TryCast(row.Tag, HistoryItem)
            If item Is Nothing Then Continue For
            If selectAllCheckBox.Checked Then
                selectedItems(item.Key) = item
            Else
                selectedItems.Remove(item.Key)
            End If
            row.Cells(selectedColumn.Index).Value = selectAllCheckBox.Checked
        Next
        updatingSelection = False
        RenderSelectionState()
    End Sub

    Private Sub RenderSelectionState()
        updatingSelection = True
        For Each row As DataGridViewRow In grid.Rows
            Dim item As HistoryItem = TryCast(row.Tag, HistoryItem)
            row.Cells(selectedColumn.Index).Value = item IsNot Nothing AndAlso selectedItems.ContainsKey(item.Key)
            row.Cells(selectedColumn.Index).ReadOnly = False
        Next
        updatingSelection = False
        UpdateSelectionControls()
        PositionSelectAll()
    End Sub

    Private Sub UpdateSelectionControls()
        updatingSelection = True
        Dim hasRows As Boolean = grid.Rows.Cast(Of DataGridViewRow)().Any(Function(row) row.Tag IsNot Nothing)
        selectAllCheckBox.Checked = selectionMode AndAlso hasRows AndAlso grid.Rows.Cast(Of DataGridViewRow)().All(
            Function(row) row.Tag Is Nothing OrElse selectedItems.ContainsKey(DirectCast(row.Tag, HistoryItem).Key))
        selectAllCheckBox.Visible = selectionMode
        selectButton.Text = If(selectionMode, "Cancel Selection", "Select")
        deleteSelectedButton.Visible = selectionMode AndAlso selectedItems.Count > 0
        deleteSelectedButton.Enabled = deleteSelectedButton.Visible
        deleteSelectedButton.Text = If(selectedItems.Count = 0, "Delete Selected", "Delete Selected (" & selectedItems.Count.ToString() & ")")
        updatingSelection = False
    End Sub

    Private Sub PositionSelectAll()
        If Not selectionMode Then
            selectAllCheckBox.Visible = False
            Return
        End If
        Dim bounds As Rectangle = grid.GetCellDisplayRectangle(selectedColumn.Index, -1, True)
        selectAllCheckBox.Location = New Point(bounds.Left + 10, bounds.Top + 11)
        selectAllCheckBox.Visible = True
        selectAllCheckBox.BringToFront()
    End Sub

    Private Sub DeleteSelected(sender As Object, e As EventArgs)
        If selectedItems.Count = 0 OrElse Session.CurrentUser Is Nothing Then Return
        If MessageBox.Show("Delete " & selectedItems.Count.ToString() & " selected history record(s)? This will not delete the related proposal.",
                           "Confirm History Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return
        Try
            Dim deletedCount As Integer = StudentHistoryService.DeleteForStudent(
                Session.CurrentUser, selectedItems.Values.Select(Function(item) item.HistoryId).ToList())
            AdminActivityLogService.RecordStudentEvent(Session.CurrentUser, Nothing, "Student History",
                                                        "Deleted History Records",
                                                        deletedCount.ToString() & " history record(s) deleted by the student.",
                                                        DateTime.Now)
            selectedItems.Clear()
            MessageBox.Show(deletedCount.ToString() & " history record(s) deleted successfully.", "History",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadHistory()
        Catch ex As Exception
            MessageBox.Show("Could not delete history: " & ex.Message, "History",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
