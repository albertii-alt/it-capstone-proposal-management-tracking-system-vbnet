Imports System.Collections.Generic

Public Class frmNotifications
    Inherits Form

    Private ReadOnly ownerUser As User
    Private ReadOnly listPanel As New FlowLayoutPanel()
    Public Event NotificationSelected As EventHandler(Of NotificationSelectedEventArgs)

    Public Class NotificationSelectedEventArgs
        Inherits EventArgs
        Public ReadOnly Property Entry As NotificationEntry
        Public Sub New(entry As NotificationEntry)
            Me.Entry = entry
        End Sub
    End Class

    Public Sub New(user As User)
        ownerUser = user
        AutoScaleMode = AutoScaleMode.Dpi
        StartPosition = FormStartPosition.CenterParent
        Text = "Notifications"
        Size = New Size(560, 620)
        BuildLayout()
        LoadNotifications()
    End Sub

    Private Sub BuildLayout()
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(16), .ColumnCount = 1, .RowCount = 2}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Dim header As New Panel With {.Dock = DockStyle.Fill}
        header.Controls.Add(New Label With {.Text = "Notifications", .Dock = DockStyle.Left, .Width = 300, .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45)})
        Dim clearAll As New Button With {.Text = "Clear All", .Dock = DockStyle.Right, .Width = 90, .Height = 28}
        AddHandler clearAll.Click, AddressOf ClearAllNotifications
        header.Controls.Add(clearAll)
        listPanel.Dock = DockStyle.Fill
        listPanel.FlowDirection = FlowDirection.TopDown
        listPanel.WrapContents = False
        listPanel.AutoScroll = True
        layout.Controls.Add(header, 0, 0)
        layout.Controls.Add(listPanel, 0, 1)
        Controls.Add(layout)
    End Sub

    Private Sub LoadNotifications()
        listPanel.Controls.Clear()
        Dim entries As List(Of NotificationEntry) = NotificationService.GetForUser(ownerUser.UserId)
        If entries.Count = 0 Then
            listPanel.Controls.Add(New Label With {.Text = "You're all caught up.", .AutoSize = False, .Width = 500, .Height = 50, .TextAlign = ContentAlignment.MiddleCenter, .ForeColor = Color.DimGray})
            Return
        End If
        For Each entry As NotificationEntry In entries
            Dim row As New Panel With {.Width = 500, .Height = 82, .Margin = New Padding(0, 0, 0, 8), .BackColor = If(entry.IsRead, Color.White, Color.FromArgb(255, 248, 225))}
            Dim open As New Button With {.Text = entry.Title & Environment.NewLine & entry.Message & Environment.NewLine & entry.CreatedAt.ToString("MMM d, yyyy h:mm tt"), .Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleLeft, .FlatStyle = FlatStyle.Flat, .Padding = New Padding(10, 4, 70, 4), .BackColor = row.BackColor}
            open.FlatAppearance.BorderSize = 0
            AddHandler open.Click, Sub(sender, e)
                                       NotificationService.MarkRead(entry.NotificationId, ownerUser.UserId)
                                       RaiseEvent NotificationSelected(Me, New NotificationSelectedEventArgs(entry))
                                       Close()
                                   End Sub
            Dim clear As New Button With {.Text = "×", .Width = 34, .Height = 28, .Anchor = AnchorStyles.Top Or AnchorStyles.Right, .Location = New Point(row.Width - 42, 8), .FlatStyle = FlatStyle.Flat}
            clear.FlatAppearance.BorderSize = 0
            AddHandler clear.Click, Sub(sender, e)
                                        NotificationService.Delete(entry.NotificationId, ownerUser.UserId)
                                        LoadNotifications()
                                    End Sub
            row.Controls.Add(open)
            row.Controls.Add(clear)
            listPanel.Controls.Add(row)
        Next
    End Sub

    Private Sub ClearAllNotifications(sender As Object, e As EventArgs)
        If MessageBox.Show("Clear all notifications?", "Notifications", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
        NotificationService.DeleteAll(ownerUser.UserId)
        LoadNotifications()
    End Sub
End Class
