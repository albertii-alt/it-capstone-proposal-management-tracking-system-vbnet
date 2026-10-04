Option Strict On
Option Explicit On

Imports System.Collections.Generic

Public Class ucAdminActivityLogs
    Inherits UserControl

    Private ReadOnly cboAction As New ComboBox()
    Private ReadOnly cboEntity As New ComboBox()
    Private ReadOnly txtSearch As New TextBox()
    Private ReadOnly dtpFrom As New DateTimePicker()
    Private ReadOnly dtpTo As New DateTimePicker()
    Private ReadOnly btnRefresh As New Button()
    Private ReadOnly dgvLogs As New DataGridView()

    Public Sub New()
        ConfigureLayout()
        LoadEntries()
    End Sub

    Private Sub ConfigureLayout()
        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .ColumnCount = 1,
            .RowCount = 3
        }
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 70.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 78.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim header As New Label With {
            .Text = "Admin Activity Logs" & Environment.NewLine & "Review administrative changes made in the system.",
            .Dock = DockStyle.Fill,
            .Font = New Font("Segoe UI", 20.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(45, 45, 45)
        }

        Dim filters As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .WrapContents = True,
            .Padding = New Padding(0, 8, 0, 0)
        }
        ConfigureCombo(cboAction, {"All actions", "Created", "Updated", "Deleted", "Activated", "Deactivated", "Approved", "Rejected", "Requested Revision", "Reset Password"})
        ConfigureCombo(cboEntity, {"All areas", "Account", "Proposal", "Title Archive", "Profile"})
        txtSearch.Width = 180
        txtSearch.PlaceholderText = "Search actor, target, or details"
        dtpFrom.Format = DateTimePickerFormat.Short
        dtpTo.Format = DateTimePickerFormat.Short
        dtpFrom.Value = DateTime.Today.AddDays(-30)
        dtpTo.Value = DateTime.Today
        btnRefresh.Text = "Refresh"
        btnRefresh.Width = 85
        filters.Controls.AddRange({
            New Label With {.Text = "Action", .AutoSize = True, .Margin = New Padding(0, 6, 4, 0)},
            cboAction,
            New Label With {.Text = "Area", .AutoSize = True, .Margin = New Padding(12, 6, 4, 0)},
            cboEntity,
            txtSearch,
            New Label With {.Text = "From", .AutoSize = True, .Margin = New Padding(12, 6, 4, 0)},
            dtpFrom,
            New Label With {.Text = "To", .AutoSize = True, .Margin = New Padding(8, 6, 4, 0)},
            dtpTo,
            btnRefresh
        })

        dgvLogs.Dock = DockStyle.Fill
        dgvLogs.ReadOnly = True
        dgvLogs.AllowUserToAddRows = False
        dgvLogs.AllowUserToDeleteRows = False
        dgvLogs.RowHeadersVisible = False
        dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvLogs.Columns.Add("CreatedAt", "When")
        dgvLogs.Columns.Add("ActorName", "Administrator")
        dgvLogs.Columns.Add("ActionType", "Action")
        dgvLogs.Columns.Add("EntityType", "Area")
        dgvLogs.Columns.Add("EntityName", "Target")
        dgvLogs.Columns.Add("Description", "Details")
        dgvLogs.Columns("CreatedAt").FillWeight = 18
        dgvLogs.Columns("ActorName").FillWeight = 18
        dgvLogs.Columns("ActionType").FillWeight = 15
        dgvLogs.Columns("EntityType").FillWeight = 14
        dgvLogs.Columns("EntityName").FillWeight = 20
        dgvLogs.Columns("Description").FillWeight = 35

        AddHandler cboAction.SelectedIndexChanged, Sub(sender As Object, e As EventArgs) LoadEntries()
        AddHandler cboEntity.SelectedIndexChanged, Sub(sender As Object, e As EventArgs) LoadEntries()
        AddHandler txtSearch.KeyDown, AddressOf Search_KeyDown
        AddHandler btnRefresh.Click, Sub(sender As Object, e As EventArgs) LoadEntries()

        layout.Controls.Add(header, 0, 0)
        layout.Controls.Add(filters, 0, 1)
        layout.Controls.Add(dgvLogs, 0, 2)
        Controls.Add(layout)
        cboAction.SelectedIndex = 0
        cboEntity.SelectedIndex = 0
    End Sub

    Private Sub ConfigureCombo(combo As ComboBox, values As IEnumerable(Of String))
        combo.DropDownStyle = ComboBoxStyle.DropDownList
        combo.Width = 145
        combo.Items.AddRange(values.ToArray())
    End Sub

    Private Sub Search_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            LoadEntries()
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub LoadEntries()
        If cboAction.SelectedIndex < 0 OrElse cboEntity.SelectedIndex < 0 Then Return

        Try
            Dim entries As List(Of AdminActivityLogEntry) = AdminActivityLogService.GetEntries(
                If(cboAction.SelectedIndex = 0, "", cboAction.Text),
                If(cboEntity.SelectedIndex = 0, "", cboEntity.Text),
                txtSearch.Text,
                dtpFrom.Value.Date,
                dtpTo.Value.Date.AddDays(1),
                0,
                500)

            dgvLogs.Rows.Clear()
            For Each entry As AdminActivityLogEntry In entries
                dgvLogs.Rows.Add(entry.CreatedAt.ToString("yyyy-MM-dd HH:mm"), entry.ActorName,
                                 entry.ActionType, entry.EntityType, entry.EntityName, entry.Description)
            Next
        Catch ex As Exception
            MessageBox.Show("Could not load activity logs: " & ex.Message, "Activity Logs",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
