Imports System.Collections.Generic

Public Class frmActiveSessions
    Inherits Form

    Private ReadOnly grid As New DataGridView()
    Private ReadOnly sessions As New List(Of ActiveSession)
    Private ReadOnly currentUserId As Integer
    Private ReadOnly currentSessionId As String

    Public Sub New(userId As Integer, sessionId As String)
        currentUserId = userId
        currentSessionId = sessionId
        Text = "Active Sessions"
        StartPosition = FormStartPosition.CenterParent
        ClientSize = New Size(760, 360)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        BuildLayout()
        LoadSessions()
    End Sub

    Private Sub BuildLayout()
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(16), .ColumnCount = 1, .RowCount = 3}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 45))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42))
        layout.Controls.Add(New Label With {.Text = "Active Sessions", .Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 16, FontStyle.Bold)}, 0, 0)
        grid.Dock = DockStyle.Fill
        grid.ReadOnly = True
        grid.AllowUserToAddRows = False
        grid.RowHeadersVisible = False
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        grid.AutoGenerateColumns = False
        grid.Columns.Add("device", "Device")
        grid.Columns.Add("ip", "IP address")
        grid.Columns.Add("started", "Signed in")
        grid.Columns.Add("lastSeen", "Last seen")
        grid.Columns.Add("current", "Status")
        grid.Columns("device").Width = 190
        grid.Columns("ip").Width = 130
        grid.Columns("started").Width = 170
        grid.Columns("lastSeen").Width = 170
        grid.Columns("current").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        layout.Controls.Add(grid, 0, 1)
        Dim buttons As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.RightToLeft}
        Dim closeButton As New Button With {.Text = "Close", .Width = 90}
        Dim revokeOthers As New Button With {.Text = "Log out all other devices", .Width = 170}
        Dim revokeOne As New Button With {.Text = "Log out selected device", .Width = 170}
        AddHandler closeButton.Click, Sub() Close()
        AddHandler revokeOne.Click, AddressOf RevokeSelected
        AddHandler revokeOthers.Click, AddressOf RevokeOtherSessions
        buttons.Controls.Add(closeButton)
        buttons.Controls.Add(revokeOthers)
        buttons.Controls.Add(revokeOne)
        layout.Controls.Add(buttons, 0, 2)
        Controls.Add(layout)
    End Sub

    Private Sub LoadSessions()
        sessions.Clear()
        sessions.AddRange(ActiveSessionService.GetForUser(currentUserId, currentSessionId))
        grid.Rows.Clear()
        For Each item As ActiveSession In sessions
            Dim index As Integer = grid.Rows.Add(item.DeviceName, item.IpAddress,
                                                 item.StartedAt.ToString("MMM d, yyyy h:mm tt"),
                                                 item.LastSeenAt.ToString("MMM d, yyyy h:mm tt"),
                                                 If(item.IsCurrent, "This device", "Active"))
            grid.Rows(index).Tag = item
        Next
    End Sub

    Private Sub RevokeSelected(sender As Object, e As EventArgs)
        If grid.CurrentRow Is Nothing Then Return
        Dim item As ActiveSession = TryCast(grid.CurrentRow.Tag, ActiveSession)
        If item Is Nothing OrElse item.IsCurrent Then
            MessageBox.Show("Select another active device. The current device cannot be remotely logged out here.")
            Return
        End If
        ActiveSessionService.Revoke(currentUserId, item.SessionId)
        LoadSessions()
    End Sub

    Private Sub RevokeOtherSessions(sender As Object, e As EventArgs)
        If MessageBox.Show("Log out all other active devices?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return
        ActiveSessionService.RevokeOthers(currentUserId, currentSessionId)
        LoadSessions()
    End Sub
End Class
