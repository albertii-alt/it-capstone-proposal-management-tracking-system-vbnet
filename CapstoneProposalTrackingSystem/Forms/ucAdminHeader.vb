Option Strict On
Option Explicit On

Imports System.Drawing.Drawing2D
Imports System.IO

Public Class ucAdminHeader
    Inherits UserControl

    Public Event ProfileRequested As EventHandler
    Public Event NotificationsRequested As EventHandler

    Private ReadOnly picAdminProfile As New PictureBox()
    Private ReadOnly lblNotificationBell As New Label()
    Private ReadOnly lblNotificationBadge As New Label()
    Private ReadOnly lblDate As New Label()
    Private ReadOnly lblTime As New Label()
    Private ReadOnly lblPageTitle As New Label()
    Private ReadOnly lblPageSubtitle As New Label()
    Private ReadOnly lblAdminName As New Label()
    Private ReadOnly lblAdminRole As New Label()
    Private ReadOnly _clockTimer As New Timer With {.Interval = 1000}

    Public Sub New()
        InitializeComponent()
        AddHandler _clockTimer.Tick, AddressOf ClockTimer_Tick
        AddHandler MyBase.Load, AddressOf Header_Load
        AddHandler MyBase.Disposed, AddressOf Header_Disposed
    End Sub

    Private Sub InitializeComponent()
        Dim headerLayout As New TableLayoutPanel()
        Dim pageHeadingLayout As New TableLayoutPanel()
        Dim identityLayout As New TableLayoutPanel()
        Dim notificationTip As New ToolTip()

        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Dock = DockStyle.Top
        Me.Height = 96
        Me.MinimumSize = New Size(0, 96)
        Me.Name = "ucAdminHeader"

        headerLayout.Dock = DockStyle.Fill
        headerLayout.ColumnCount = 2
        headerLayout.RowCount = 1
        headerLayout.Padding = New Padding(20, 4, 20, 0)
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 496.0F))
        headerLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        pageHeadingLayout.Dock = DockStyle.Fill
        pageHeadingLayout.ColumnCount = 1
        pageHeadingLayout.RowCount = 2
        pageHeadingLayout.Padding = New Padding(0, 2, 8, 4)
        pageHeadingLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        pageHeadingLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        pageHeadingLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        lblPageTitle.Name = "lblAdminPageTitle"
        lblPageTitle.Dock = DockStyle.Fill
        lblPageTitle.Font = New Font("Segoe UI", 22.0F, FontStyle.Bold)
        lblPageTitle.ForeColor = Color.FromArgb(45, 45, 45)
        lblPageTitle.TextAlign = ContentAlignment.MiddleLeft

        lblPageSubtitle.Name = "lblAdminPageSubtitle"
        lblPageSubtitle.Dock = DockStyle.Fill
        lblPageSubtitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblPageSubtitle.ForeColor = Color.DimGray
        lblPageSubtitle.TextAlign = ContentAlignment.MiddleLeft
        pageHeadingLayout.Controls.Add(lblPageTitle, 0, 0)
        pageHeadingLayout.Controls.Add(lblPageSubtitle, 0, 1)

        lblDate.Dock = DockStyle.Fill
        lblDate.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
        lblDate.ForeColor = Color.DimGray
        lblDate.TextAlign = ContentAlignment.MiddleRight

        lblTime.Dock = DockStyle.Fill
        lblTime.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
        lblTime.ForeColor = Color.DimGray
        lblTime.TextAlign = ContentAlignment.MiddleLeft
        lblTime.Padding = New Padding(6, 0, 0, 0)

        lblNotificationBell.Dock = DockStyle.Fill
        lblNotificationBell.ForeColor = Color.FromArgb(122, 31, 46)
        lblNotificationBell.BackColor = Color.Transparent
        lblNotificationBell.Cursor = Cursors.Hand
        lblNotificationBell.AccessibleName = "Notifications"
        lblNotificationBell.AccessibleDescription = "Open notifications."
        notificationTip.SetToolTip(lblNotificationBell, "Notifications")
        AddHandler lblNotificationBell.Click, Sub(sender, e) RaiseEvent NotificationsRequested(Me, EventArgs.Empty)
        AddHandler lblNotificationBell.Paint, AddressOf NotificationBell_Paint

        lblNotificationBadge.AutoSize = False
        lblNotificationBadge.Size = New Size(18, 18)
        lblNotificationBadge.BackColor = Color.FromArgb(166, 58, 58)
        lblNotificationBadge.ForeColor = Color.White
        lblNotificationBadge.Font = New Font("Segoe UI", 7.0F, FontStyle.Bold)
        lblNotificationBadge.TextAlign = ContentAlignment.MiddleCenter
        lblNotificationBadge.Location = New Point(30, 6)
        Using badgePath As New GraphicsPath()
            badgePath.AddEllipse(0, 0, lblNotificationBadge.Width - 1, lblNotificationBadge.Height - 1)
            lblNotificationBadge.Region = New Region(badgePath)
        End Using
        lblNotificationBadge.Visible = False
        lblNotificationBell.Controls.Add(lblNotificationBadge)

        picAdminProfile.Size = New Size(40, 40)
        picAdminProfile.Margin = New Padding(4, 2, 10, 0)
        picAdminProfile.Anchor = AnchorStyles.None
        picAdminProfile.SizeMode = PictureBoxSizeMode.Zoom
        picAdminProfile.Cursor = Cursors.Hand
        picAdminProfile.BackColor = Color.FromArgb(122, 31, 46)
        picAdminProfile.ForeColor = Color.White
        picAdminProfile.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        Using avatarPath As New GraphicsPath()
            avatarPath.AddEllipse(0, 0, picAdminProfile.Width, picAdminProfile.Height)
            picAdminProfile.Region = New Region(avatarPath)
        End Using
        AddHandler picAdminProfile.Paint, AddressOf AdminProfile_Paint
        AddHandler picAdminProfile.Click, Sub(sender, e) RaiseEvent ProfileRequested(Me, EventArgs.Empty)

        lblAdminName.Dock = DockStyle.Fill
        lblAdminName.AutoEllipsis = True
        lblAdminName.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblAdminName.ForeColor = Color.FromArgb(45, 45, 45)
        lblAdminName.TextAlign = ContentAlignment.BottomLeft

        lblAdminRole.Dock = DockStyle.Fill
        lblAdminRole.AutoEllipsis = True
        lblAdminRole.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
        lblAdminRole.ForeColor = Color.DimGray
        lblAdminRole.TextAlign = ContentAlignment.TopLeft

        identityLayout.Dock = DockStyle.Fill
        identityLayout.ColumnCount = 1
        identityLayout.RowCount = 2
        identityLayout.Padding = New Padding(8, 18, 0, 18)
        identityLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        identityLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 26.0F))
        identityLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        identityLayout.Controls.Add(lblAdminName, 0, 0)
        identityLayout.Controls.Add(lblAdminRole, 0, 1)

        Dim headerMeta As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 5,
            .RowCount = 1,
            .Padding = New Padding(0, 4, 0, 0)
        }
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 100.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 110.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 48.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 58.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 180.0F))
        headerMeta.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        headerMeta.Controls.Add(lblDate, 0, 0)
        headerMeta.Controls.Add(lblTime, 1, 0)
        headerMeta.Controls.Add(lblNotificationBell, 2, 0)
        headerMeta.Controls.Add(picAdminProfile, 3, 0)
        headerMeta.Controls.Add(identityLayout, 4, 0)
        headerLayout.Controls.Add(pageHeadingLayout, 0, 0)
        headerLayout.Controls.Add(headerMeta, 1, 0)
        Controls.Add(headerLayout)
    End Sub

    Public Sub SetPageTitle(title As String, subtitle As String)
        lblPageTitle.Text = title
        lblPageSubtitle.Text = subtitle
    End Sub

    Public Sub RefreshIdentity()
        If Session.CurrentUser Is Nothing Then Return

        lblAdminName.Text = Session.CurrentUser.FullName
        lblAdminRole.Text = GetRoleDisplayName(Session.CurrentUser.Role)

        Dim replacementImage As Image = CreateProfileImage(Session.CurrentUser.ProfilePhoto)
        Dim previousImage As Image = picAdminProfile.Image
        picAdminProfile.Image = replacementImage
        If previousImage IsNot Nothing Then previousImage.Dispose()
        picAdminProfile.Invalidate()

        RefreshNotificationBadge()
    End Sub

    Public Sub RefreshNotificationBadge()
        If Session.CurrentUser Is Nothing Then Return

        Dim unread As Integer = NotificationService.CountUnread(Session.CurrentUser.UserId)
        lblNotificationBadge.Text = If(unread > 99, "99+", unread.ToString())
        lblNotificationBadge.Visible = unread > 0
        lblNotificationBadge.BringToFront()
    End Sub

    Private Sub Header_Load(sender As Object, e As EventArgs)
        UpdateClock()
        _clockTimer.Start()
    End Sub

    Private Sub Header_Disposed(sender As Object, e As EventArgs)
        _clockTimer.Stop()
        _clockTimer.Dispose()
        If picAdminProfile.Image IsNot Nothing Then picAdminProfile.Image.Dispose()
    End Sub

    Private Sub ClockTimer_Tick(sender As Object, e As EventArgs)
        UpdateClock()
    End Sub

    Private Sub UpdateClock()
        Dim currentTime As DateTime = DateTime.Now
        lblDate.Text = currentTime.ToString("MMM d, yyyy")
        lblTime.Text = currentTime.ToString("hh:mm:ss tt")
    End Sub

    Private Shared Function CreateProfileImage(photoBytes As Byte()) As Image
        If photoBytes Is Nothing OrElse photoBytes.Length = 0 Then Return Nothing

        Try
            Using stream As New MemoryStream(photoBytes)
                Using image As Image = Image.FromStream(stream)
                    Return New Bitmap(image)
                End Using
            End Using
        Catch ex As ArgumentException
            Return Nothing
        Catch ex As OutOfMemoryException
            Return Nothing
        End Try
    End Function

    Private Sub AdminProfile_Paint(sender As Object, e As PaintEventArgs)
        If picAdminProfile.Image IsNot Nothing Then Return

        Dim displayName As String = If(Session.CurrentUser Is Nothing, "Admin", Session.CurrentUser.FullName)
        TextRenderer.DrawText(
            e.Graphics,
            GetInitials(displayName),
            picAdminProfile.Font,
            picAdminProfile.ClientRectangle,
            picAdminProfile.ForeColor,
            TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
    End Sub

    Private Sub NotificationBell_Paint(sender As Object, e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim bellOffsetX As Integer = Math.Max(0, (lblNotificationBell.ClientSize.Width - 40) \ 2)
        Dim bellOffsetY As Integer = Math.Max(0, (lblNotificationBell.ClientSize.Height - 32) \ 2)
        e.Graphics.TranslateTransform(bellOffsetX, bellOffsetY)

        Using bellBrush As New SolidBrush(Color.FromArgb(122, 31, 46))
            Dim bellBody As Point() = {
                New Point(14, 25), New Point(17, 21), New Point(18, 12),
                New Point(21, 7), New Point(25, 5), New Point(29, 7),
                New Point(32, 12), New Point(33, 21), New Point(36, 25)}
            e.Graphics.FillPolygon(bellBrush, bellBody)
            e.Graphics.FillEllipse(bellBrush, 22, 25, 6, 4)
            e.Graphics.FillRectangle(bellBrush, 17, 22, 16, 4)
        End Using

        Using clapperBrush As New SolidBrush(Color.FromArgb(246, 201, 69))
            e.Graphics.FillEllipse(clapperBrush, 24, 26, 3, 2)
        End Using
        e.Graphics.ResetTransform()
    End Sub

    Private Shared Function GetRoleDisplayName(role As UserRole) As String
        If role = UserRole.SuperAdmin Then Return "Super Admin"
        Return role.ToString()
    End Function

    Private Shared Function GetInitials(displayName As String) As String
        If String.IsNullOrWhiteSpace(displayName) Then Return "A"

        Dim nameParts As String() = displayName.Trim().Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
        If nameParts.Length = 1 Then Return nameParts(0).Substring(0, 1).ToUpperInvariant()
        Return nameParts(0).Substring(0, 1).ToUpperInvariant() & nameParts(nameParts.Length - 1).Substring(0, 1).ToUpperInvariant()
    End Function
End Class
