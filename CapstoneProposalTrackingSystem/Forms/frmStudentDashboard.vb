Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Linq

Public Class frmStudentDashboard
    Private ReadOnly sidebarColor As Color = Color.FromArgb(122, 31, 46)
    Private ReadOnly navTextColor As Color = Color.FromArgb(240, 217, 160)
    Private ReadOnly activeColor As Color = Color.FromArgb(246, 201, 69)
    Private ReadOnly pnlSidebar As New Panel()
    Private ReadOnly pnlContent As New Panel()
    Private ReadOnly pnlViewHost As New Panel()
    Private ReadOnly headerMeta As New TableLayoutPanel()
    Private ReadOnly lblDate As New Label()
    Private ReadOnly lblTime As New Label()
    Private ReadOnly lblPageTitle As New Label()
    Private ReadOnly lblPageSubtitle As New Label()
    Private ReadOnly lblIdentity As New Label()
    Private ReadOnly picAvatar As New PictureBox()
    Private ReadOnly bell As New Label()
    Private ReadOnly notificationBadge As New Label()
    Private ReadOnly clockTimer As New Timer With {.Interval = 1000}
    Private ReadOnly sessionTimer As New Timer With {.Interval = 30000}
    Private ReadOnly navButtons As New List(Of Button)()

    Public Sub New()
        InitializeComponent()
        AddHandler clockTimer.Tick, AddressOf ClockTick
        AddHandler sessionTimer.Tick, AddressOf SessionTick
    End Sub

    Private Sub frmStudentDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Session.CurrentUser Is Nothing OrElse Session.CurrentUser.Role <> UserRole.Student Then
            MessageBox.Show("Access denied.", "Student Portal", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Close()
            Return
        End If

        FormDisplayHelper.MaximizeFullScreen(Me)
        BuildStudentShell()
        clockTimer.Start()
        sessionTimer.Start()
        ShowDashboard()
        UpdateNotificationBell()
    End Sub

    Private Sub BuildStudentShell()
        Controls.Clear()
        Text = "Capstone System - Student"
        BackColor = SystemColors.Control

        pnlSidebar.Dock = DockStyle.Left
        pnlSidebar.Width = 250
        pnlSidebar.BackColor = sidebarColor
        pnlContent.Dock = DockStyle.Fill
        pnlContent.BackColor = SystemColors.Control
        pnlViewHost.Dock = DockStyle.Fill
        pnlViewHost.BackColor = SystemColors.Control
        Controls.Add(pnlContent)
        Controls.Add(pnlSidebar)

        Dim spacer As New Panel With {.Dock = DockStyle.Top, .Height = 24, .BackColor = sidebarColor}
        Dim logo As New PictureBox With {.Dock = DockStyle.Top, .Height = 108, .Padding = New Padding(0, 12, 0, 0), .SizeMode = PictureBoxSizeMode.Zoom, .BackColor = sidebarColor}
        LoadLogo(logo)
        Dim appName As New Label With {
            .Text = "IT Capstone Proposal Management and Tracking System",
            .Dock = DockStyle.Top, .Height = 64, .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter, .Font = New Font("Segoe UI", 8.5F),
            .ForeColor = activeColor, .BackColor = sidebarColor, .Padding = New Padding(8, 2, 8, 2)}
        pnlSidebar.Controls.Add(CreateLogoutButton())
        pnlSidebar.Controls.Add(New Panel With {.Dock = DockStyle.Bottom, .Height = 12, .BackColor = sidebarColor})
        AddNavButton("Profile", ChrW(&HE77B), AddressOf ShowProfile)
        AddNavButton("My Login Activity", ChrW(&HE81C), AddressOf ShowLoginActivity)
        AddNavButton("History", ChrW(&HE9D9), AddressOf ShowHistory)
        AddNavButton("My Proposals", ChrW(&HE8A5), AddressOf ShowProposals)
        AddNavButton("Dashboard", ChrW(&HE80F), AddressOf ShowDashboard)
        pnlSidebar.Controls.Add(New Panel With {.Dock = DockStyle.Top, .Height = 18, .BackColor = sidebarColor})
        pnlSidebar.Controls.Add(appName)
        pnlSidebar.Controls.Add(logo)
        pnlSidebar.Controls.Add(spacer)

        headerMeta.Dock = DockStyle.Top
        headerMeta.Height = 96
        headerMeta.ColumnCount = 6
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 104.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 116.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 52.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 60.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 210.0F))
        headerMeta.Padding = New Padding(0, 12, 12, 0)
        headerMeta.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Dim pageHeading As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2,
            .Padding = New Padding(20, 4, 8, 4)
        }
        pageHeading.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        pageHeading.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        pageHeading.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        lblPageTitle.Dock = DockStyle.Fill
        lblPageTitle.Font = New Font("Segoe UI", 22.0F, FontStyle.Bold)
        lblPageTitle.ForeColor = Color.FromArgb(45, 45, 45)
        lblPageTitle.TextAlign = ContentAlignment.MiddleLeft
        lblPageSubtitle.Dock = DockStyle.Fill
        lblPageSubtitle.Font = New Font("Segoe UI", 9.0F)
        lblPageSubtitle.ForeColor = Color.DimGray
        lblPageSubtitle.TextAlign = ContentAlignment.MiddleLeft
        pageHeading.Controls.Add(lblPageTitle, 0, 0)
        pageHeading.Controls.Add(lblPageSubtitle, 0, 1)
        lblDate.Dock = DockStyle.Fill
        lblDate.TextAlign = ContentAlignment.MiddleCenter
        lblDate.ForeColor = Color.FromArgb(70, 70, 70)
        lblDate.Margin = New Padding(0)
        lblTime.Dock = DockStyle.Fill
        lblTime.TextAlign = ContentAlignment.MiddleCenter
        lblTime.ForeColor = Color.FromArgb(70, 70, 70)
        lblTime.Margin = New Padding(0)
        bell.Dock = DockStyle.Fill
        bell.ForeColor = sidebarColor
        bell.Text = String.Empty
        bell.Font = New Font("Segoe MDL2 Assets", 17.0F)
        bell.TextAlign = ContentAlignment.MiddleCenter
        bell.Cursor = Cursors.Hand
        AddHandler bell.Click, AddressOf ShowNotifications
        AddHandler bell.Paint, AddressOf BellPaint
        notificationBadge.AutoSize = False
        notificationBadge.Size = New Size(18, 18)
        notificationBadge.BackColor = Color.FromArgb(166, 58, 58)
        notificationBadge.ForeColor = Color.White
        notificationBadge.Font = New Font("Segoe UI", 7.0F, FontStyle.Bold)
        notificationBadge.TextAlign = ContentAlignment.MiddleCenter
        notificationBadge.Location = New Point(30, 6)
        Using badgePath As New GraphicsPath()
            badgePath.AddEllipse(0, 0, notificationBadge.Width - 1, notificationBadge.Height - 1)
            notificationBadge.Region = New Region(badgePath)
        End Using
        notificationBadge.Visible = False
        bell.Controls.Add(notificationBadge)
        picAvatar.Size = New Size(40, 40)
        picAvatar.Anchor = AnchorStyles.None
        picAvatar.Margin = New Padding(0)
        picAvatar.SizeMode = PictureBoxSizeMode.Zoom
        picAvatar.BackColor = sidebarColor
        Using path As New GraphicsPath()
            path.AddEllipse(0, 0, 40, 40)
            picAvatar.Region = New Region(path)
        End Using
        AddHandler picAvatar.Paint, AddressOf AvatarPaint
        lblIdentity.Dock = DockStyle.Fill
        lblIdentity.Margin = New Padding(0)
        lblIdentity.Padding = New Padding(4, 0, 4, 0)
        lblIdentity.AutoEllipsis = True
        lblIdentity.Font = New Font("Segoe UI", 9.0F)
        lblIdentity.TextAlign = ContentAlignment.MiddleLeft
        headerMeta.Controls.Add(pageHeading, 0, 0)
        headerMeta.Controls.Add(lblDate, 1, 0)
        headerMeta.Controls.Add(lblTime, 2, 0)
        headerMeta.Controls.Add(bell, 3, 0)
        headerMeta.Controls.Add(picAvatar, 4, 0)
        headerMeta.Controls.Add(lblIdentity, 5, 0)
        pnlContent.Controls.Add(pnlViewHost)
        pnlContent.Controls.Add(headerMeta)
        UpdateHeader()
    End Sub

    Private Sub AddNavButton(text As String, glyph As String, handler As EventHandler)
        Dim button As New Button()
        button.Text = "   " & text
        button.Dock = DockStyle.Top
        button.Height = 46
        button.FlatStyle = FlatStyle.Flat
        button.FlatAppearance.BorderSize = 0
        button.TextAlign = ContentAlignment.MiddleLeft
        button.Padding = New Padding(12, 0, 0, 0)
        button.ImageAlign = ContentAlignment.MiddleLeft
        button.TextImageRelation = TextImageRelation.ImageBeforeText
        button.BackColor = sidebarColor
        button.ForeColor = navTextColor
        button.Tag = glyph
        button.Image = CreateIcon(glyph, navTextColor)
        AddHandler button.Click, handler
        navButtons.Add(button)
        pnlSidebar.Controls.Add(button)
    End Sub

    Private Function CreateLogoutButton() As Button
        Dim button As New Button With {.Text = "   Logout", .Dock = DockStyle.Bottom, .Height = 48,
                                       .FlatStyle = FlatStyle.Flat, .BackColor = sidebarColor,
                                       .ForeColor = Color.FromArgb(242, 166, 166), .TextAlign = ContentAlignment.MiddleLeft,
                                       .Padding = New Padding(12, 0, 0, 0), .ImageAlign = ContentAlignment.MiddleLeft,
                                       .TextImageRelation = TextImageRelation.ImageBeforeText}
        button.FlatAppearance.BorderSize = 0
        button.Image = CreateIcon(ChrW(&HE7E8), button.ForeColor)
        AddHandler button.Click, AddressOf Logout
        Return button
    End Function

    Private Function CreateIcon(glyph As String, iconColor As Color) As Bitmap
        Dim image As New Bitmap(28, 28)
        Using graphics As Graphics = Graphics.FromImage(image)
            graphics.Clear(Color.Transparent)
            Using font As New Font("Segoe MDL2 Assets", 16.0F)
                TextRenderer.DrawText(graphics, glyph, font, New Rectangle(0, 0, 28, 28), iconColor, Color.Transparent,
                                       TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            End Using
        End Using
        Return image
    End Function

    Private Sub LoadLogo(target As PictureBox)
        Dim assembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
        Using stream As Stream = assembly.GetManifestResourceStream("CapstoneProposalTrackingSystem.CCS_Logo.png")
            If stream Is Nothing Then Return
            Using source As Image = Image.FromStream(stream)
                target.Image = New Bitmap(source)
            End Using
        End Using
    End Sub

    Private Sub UpdateHeader()
        If Session.CurrentUser Is Nothing Then Return
        lblDate.Text = DateTime.Now.ToString("MMM d, yyyy")
        lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt")
        lblIdentity.Text = Session.CurrentUser.FullName & Environment.NewLine & "Student"
        If Session.CurrentUser.ProfilePhoto IsNot Nothing Then
            Using stream As New MemoryStream(Session.CurrentUser.ProfilePhoto)
                picAvatar.Image = New Bitmap(Image.FromStream(stream))
            End Using
        End If
        picAvatar.Invalidate()
    End Sub

    Private Sub AvatarPaint(sender As Object, e As PaintEventArgs)
        If picAvatar.Image IsNot Nothing Then Return
        Using brush As New SolidBrush(Color.White)
            e.Graphics.DrawString(If(Session.CurrentUser Is Nothing OrElse String.IsNullOrEmpty(Session.CurrentUser.FullName), "S", Session.CurrentUser.FullName.Substring(0, 1).ToUpperInvariant()),
                                  New Font("Segoe UI", 12.0F, FontStyle.Bold), brush, 12, 9)
        End Using
    End Sub

    Private Sub ShowView(control As UserControl, activeText As String)
        SetPageHeader(activeText)
        For Each button As Button In navButtons
            Dim selected As Boolean = button.Text.Trim() = activeText
            button.BackColor = If(selected, activeColor, sidebarColor)
            button.ForeColor = If(selected, sidebarColor, navTextColor)
            Dim glyph As String = TryCast(button.Tag, String)
            If glyph IsNot Nothing Then button.Image = CreateIcon(glyph, button.ForeColor)
        Next
        For Each controlToDispose As Control In pnlViewHost.Controls.OfType(Of Control)().ToList()
            pnlViewHost.Controls.Remove(controlToDispose)
            controlToDispose.Dispose()
        Next
        control.Dock = DockStyle.Fill
        pnlViewHost.Controls.Add(control)
    End Sub

    Private Sub SetPageHeader(activeText As String)
        lblPageTitle.Text = activeText
        Select Case activeText
            Case "Dashboard"
                lblPageSubtitle.Text = "Welcome back, " & Session.CurrentUser.FullName & ". Here's an overview of your capstone proposals."
            Case "My Proposals"
                lblPageSubtitle.Text = "Submit proposals and track their review status."
            Case "History"
                lblPageSubtitle.Text = "Review the activity and review history for your proposals."
            Case "My Login Activity"
                lblPageSubtitle.Text = "Review your successful sign-in and sign-out history."
            Case "Profile"
                lblPageSubtitle.Text = "Manage your account details and sign-in password."
            Case Else
                lblPageSubtitle.Text = String.Empty
        End Select
    End Sub

    Private Sub ShowDashboard(sender As Object, e As EventArgs)
        ShowDashboard()
    End Sub

    Private Sub ShowDashboard()
        Dim dashboard As New UserControl With {.BackColor = SystemColors.Control}
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 2}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 104.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim counts As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 5}
        For index As Integer = 0 To 4
            counts.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 20.0F))
        Next
        Dim proposals As List(Of Proposal) = ProposalService.GetMyProposals(Session.CurrentUser.UserId)
        AddMetric(counts, "Total Proposals", proposals.Count, 0)
        AddMetric(counts, "Pending", proposals.Where(Function(item) item.Status = ProposalStatus.Pending).Count(), 1)
        AddMetric(counts, "Approved", proposals.Where(Function(item) item.Status = ProposalStatus.Approved).Count(), 2)
        AddMetric(counts, "For Revision", proposals.Where(Function(item) item.Status = ProposalStatus.ForRevision).Count(), 3)
        AddMetric(counts, "Rejected", proposals.Where(Function(item) item.Status = ProposalStatus.Rejected).Count(), 4)
        layout.Controls.Add(counts, 0, 0)

        Dim content As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1}
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 58.0F))
        content.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 42.0F))
        Dim leftColumn As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 2, .Margin = New Padding(0, 10, 8, 0)}
        leftColumn.RowStyles.Add(New RowStyle(SizeType.Percent, 58.0F))
        leftColumn.RowStyles.Add(New RowStyle(SizeType.Percent, 42.0F))
        leftColumn.Controls.Add(CreateRecentActivityCard(proposals), 0, 0)
        leftColumn.Controls.Add(CreateLatestProposalCard(proposals), 0, 1)
        Dim rightColumn As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 3, .Margin = New Padding(8, 10, 0, 0)}
        rightColumn.RowStyles.Add(New RowStyle(SizeType.Percent, 43.0F))
        rightColumn.RowStyles.Add(New RowStyle(SizeType.Percent, 32.0F))
        rightColumn.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        rightColumn.Controls.Add(CreateAttentionCard(proposals), 0, 0)
        rightColumn.Controls.Add(CreateQuickActionsCard(), 0, 1)
        rightColumn.Controls.Add(CreateProfileCompletionCard(), 0, 2)
        content.Controls.Add(leftColumn, 0, 0)
        content.Controls.Add(rightColumn, 1, 0)
        layout.Controls.Add(content, 0, 1)
        dashboard.Controls.Add(layout)
        ShowView(dashboard, "Dashboard")
    End Sub

    Private Sub AddMetric(parent As TableLayoutPanel, caption As String, value As Integer, column As Integer)
        Dim panel As New Panel With {.Dock = DockStyle.Fill, .BackColor = SystemColors.Window, .Margin = New Padding(0, 0, 10, 0)}
        panel.Controls.Add(New Label With {.Text = value.ToString(), .Dock = DockStyle.Top, .Height = 58, .Font = New Font("Segoe UI", 22.0F, FontStyle.Bold), .ForeColor = sidebarColor, .TextAlign = ContentAlignment.MiddleCenter})
        panel.Controls.Add(New Label With {.Text = caption, .Dock = DockStyle.Bottom, .Height = 36, .Font = New Font("Segoe UI", 9.0F), .ForeColor = Color.DimGray, .TextAlign = ContentAlignment.MiddleCenter})
        parent.Controls.Add(panel, column, 0)
    End Sub

    Private Function CreateCard(title As String) As TableLayoutPanel
        Dim card As New TableLayoutPanel With {.Dock = DockStyle.Fill, .BackColor = SystemColors.Window, .ColumnCount = 1, .RowCount = 2, .Padding = New Padding(14), .Margin = New Padding(0, 0, 0, 8)}
        card.RowStyles.Add(New RowStyle(SizeType.Absolute, 30.0F))
        card.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        card.Controls.Add(New Label With {.Text = title, .Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 11.0F, FontStyle.Bold), .ForeColor = sidebarColor, .TextAlign = ContentAlignment.MiddleLeft}, 0, 0)
        Return card
    End Function

    Private Function CreateRecentActivityCard(proposals As List(Of Proposal)) As Control
        Dim card As TableLayoutPanel = CreateCard("Recent Activity")
        Dim body As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}
        Dim activities As New List(Of String)()
        For Each proposal As Proposal In proposals
            activities.Add(proposal.SubmittedAt.ToString("MMM d, h:mm tt") & "  •  Submitted """ & proposal.Title & """")
            For Each log As StatusLogEntry In ProposalService.GetStatusLogs(proposal.ProposalId).Take(3)
                activities.Add(log.ChangedAt.ToString("MMM d, h:mm tt") & "  •  " & FormatStatus(log.NewStatus) & " """ & proposal.Title & """")
            Next
        Next
        If activities.Count = 0 Then activities.Add("No proposal activity yet. Submit your first proposal to get started.")
        For Each activity As String In activities.Take(6)
            body.Controls.Add(New Label With {.Text = activity, .AutoSize = False, .Width = 500, .Height = 28, .ForeColor = Color.FromArgb(75, 75, 75), .Padding = New Padding(0, 4, 0, 0)})
        Next
        card.Controls.Add(body, 0, 1)
        Return card
    End Function

    Private Function CreateLatestProposalCard(proposals As List(Of Proposal)) As Control
        Dim card As TableLayoutPanel = CreateCard("Latest Proposal")
        Dim body As New Panel With {.Dock = DockStyle.Fill}
        Dim latest As Proposal = proposals.OrderByDescending(Function(item) item.SubmittedAt).FirstOrDefault()
        If latest Is Nothing Then
            body.Controls.Add(New Label With {.Text = "No proposal submitted yet.", .Dock = DockStyle.Fill, .ForeColor = Color.DimGray, .TextAlign = ContentAlignment.MiddleLeft})
        Else
            body.Controls.Add(New Label With {.Text = latest.Title, .Dock = DockStyle.Top, .Height = 28, .Font = New Font("Segoe UI", 10.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(55, 55, 55)})
            body.Controls.Add(New Label With {.Text = "Submitted " & latest.SubmittedAt.ToString("MMMM d, yyyy h:mm tt") & "   •   Status: " & FormatStatus(latest.Status), .Dock = DockStyle.Top, .Height = 25, .ForeColor = Color.DimGray})
            Dim latestLog As StatusLogEntry = ProposalService.GetLatestLog(latest.ProposalId)
            body.Controls.Add(New Label With {.Text = If(latestLog Is Nothing OrElse String.IsNullOrWhiteSpace(latestLog.Remarks), "No administrator remarks.", "Administrator remark: " & latestLog.Remarks), .Dock = DockStyle.Fill, .ForeColor = Color.DimGray, .AutoEllipsis = True})
        End If
        card.Controls.Add(body, 0, 1)
        Return card
    End Function

    Private Function CreateAttentionCard(proposals As List(Of Proposal)) As Control
        Dim card As TableLayoutPanel = CreateCard("Needs Your Attention")
        Dim body As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}
        Dim revisions As List(Of Proposal) = proposals.Where(Function(item) item.Status = ProposalStatus.ForRevision).ToList()
        If revisions.Count = 0 Then
            body.Controls.Add(New Label With {.Text = "You're all caught up. No proposals require action.", .AutoSize = False, .Width = 350, .Height = 30, .ForeColor = Color.DarkGreen})
        Else
            For Each proposal As Proposal In revisions.Take(3)
                body.Controls.Add(New Label With {.Text = "Revision requested: " & proposal.Title, .AutoSize = False, .Width = 350, .Height = 34, .ForeColor = Color.FromArgb(150, 90, 20), .Padding = New Padding(0, 4, 0, 0)})
            Next
        End If
        card.Controls.Add(body, 0, 1)
        Return card
    End Function

    Private Function CreateQuickActionsCard() As Control
        Dim card As TableLayoutPanel = CreateCard("Quick Actions")
        Dim actions As New FlowLayoutPanel With {.Dock = DockStyle.Fill, .WrapContents = True, .AutoScroll = True}
        AddDashboardAction(actions, "Submit New Proposal", AddressOf SubmitDashboardProposal)
        AddDashboardAction(actions, "View My Proposals", AddressOf ShowProposals)
        AddDashboardAction(actions, "View History", AddressOf ShowHistory)
        AddDashboardAction(actions, "Edit Profile", AddressOf ShowProfile)
        card.Controls.Add(actions, 0, 1)
        Return card
    End Function

    Private Function CreateProfileCompletionCard() As Control
        Dim card As TableLayoutPanel = CreateCard("Profile Status")
        Dim fields As Integer = 0
        If Not String.IsNullOrWhiteSpace(Session.CurrentUser.FullName) Then fields += 1
        If Not String.IsNullOrWhiteSpace(Session.CurrentUser.Email) Then fields += 1
        If Not String.IsNullOrWhiteSpace(Session.CurrentUser.SchoolId) Then fields += 1
        If Session.CurrentUser.ProfilePhoto IsNot Nothing AndAlso Session.CurrentUser.ProfilePhoto.Length > 0 Then fields += 1
        Dim body As New Panel With {.Dock = DockStyle.Fill}
        body.Controls.Add(New Label With {.Text = fields.ToString() & " of 4 profile details completed", .Dock = DockStyle.Top, .Height = 24, .ForeColor = Color.DimGray})
        body.Controls.Add(New ProgressBar With {.Value = fields * 25, .Maximum = 100, .Dock = DockStyle.Top, .Height = 16, .Style = ProgressBarStyle.Continuous})
        card.Controls.Add(body, 0, 1)
        Return card
    End Function

    Private Sub AddDashboardAction(parent As FlowLayoutPanel, text As String, handler As EventHandler)
        Dim button As New Button With {.Text = text, .AutoSize = True, .Height = 28, .FlatStyle = FlatStyle.Flat, .BackColor = Color.FromArgb(122, 31, 46), .ForeColor = Color.White, .Margin = New Padding(0, 0, 8, 6)}
        button.FlatAppearance.BorderSize = 0
        AddHandler button.Click, handler
        parent.Controls.Add(button)
    End Sub

    Private Sub SubmitDashboardProposal(sender As Object, e As EventArgs)
        Using dialog As New frmSubmitProposal()
            dialog.ShowDialog(Me)
        End Using
        ShowDashboard()
    End Sub

    Private Shared Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then Return "For Revision"
        Return status.ToString()
    End Function

    Private Sub ShowProposals(sender As Object, e As EventArgs)
        ShowProposals()
    End Sub

    Private Sub ShowProposals()
        ShowView(New ucStudentProposals(), "My Proposals")
    End Sub

    Private Sub ShowHistory(sender As Object, e As EventArgs)
        ShowView(New ucStudentHistory(), "History")
    End Sub

    Private Sub ShowLoginActivity(sender As Object, e As EventArgs)
        ShowView(New ucMyLoginActivity(), "My Login Activity")
    End Sub

    Private Sub ShowProfile(sender As Object, e As EventArgs)
        ShowView(New ucProfile(), "Profile")
    End Sub

    Private Sub ShowNotifications(sender As Object, e As EventArgs)
        Using dialog As New frmNotifications(Session.CurrentUser)
            AddHandler dialog.NotificationSelected, AddressOf NotificationSelected
            dialog.ShowDialog(Me)
        End Using
        UpdateNotificationBell()
    End Sub

    Private Sub NotificationSelected(sender As Object, e As frmNotifications.NotificationSelectedEventArgs)
        If e.Entry Is Nothing Then Return
        Select Case e.Entry.TargetModule
            Case "My Proposals"
                ShowProposals()
            Case "Profile"
                ShowView(New ucProfile(), "Profile")
            Case Else
                ShowDashboard()
        End Select
    End Sub

    Private Sub UpdateNotificationBell()
        If Session.CurrentUser Is Nothing Then Return
        Dim unread As Integer = NotificationService.CountUnread(Session.CurrentUser.UserId)
        notificationBadge.Text = If(unread > 99, "99+", unread.ToString())
        notificationBadge.Visible = unread > 0
        notificationBadge.BringToFront()
    End Sub

    Private Sub BellPaint(sender As Object, e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim bellOffsetX As Integer = Math.Max(0, (bell.ClientSize.Width - 40) \ 2)
        Dim bellOffsetY As Integer = Math.Max(0, (bell.ClientSize.Height - 32) \ 2)
        e.Graphics.TranslateTransform(bellOffsetX, bellOffsetY)
        Using bellBrush As New SolidBrush(sidebarColor)
            Dim bellBody As Point() = {
                New Point(14, 25), New Point(17, 21), New Point(18, 12),
                New Point(21, 7), New Point(25, 5), New Point(29, 7),
                New Point(32, 12), New Point(33, 21), New Point(36, 25)}
            e.Graphics.FillPolygon(bellBrush, bellBody)
            e.Graphics.FillEllipse(bellBrush, 22, 25, 6, 4)
            e.Graphics.FillRectangle(bellBrush, 17, 22, 16, 4)
        End Using
        Using clapperBrush As New SolidBrush(activeColor)
            e.Graphics.FillEllipse(clapperBrush, 24, 26, 3, 2)
        End Using
        e.Graphics.ResetTransform()
    End Sub

    Private Sub Logout(sender As Object, e As EventArgs)
        If MessageBox.Show("Are you sure you want to log out?", "Confirm logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then Return
        Try
            LoginActivityService.RecordLogout(Session.CurrentUser)
        Catch ex As Exception
            MessageBox.Show("Your logout could not be recorded, but the session will still be closed." & Environment.NewLine & ex.Message, "Logout Activity", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
        clockTimer.Stop()
        sessionTimer.Stop()
        Session.CurrentUser = Nothing
        Session.SessionId = Nothing
        Dim loginForm As New frmLogin()
        loginForm.Show()
        Close()
    End Sub

    Private Sub ClockTick(sender As Object, e As EventArgs)
        UpdateHeader()
    End Sub

    Private Sub SessionTick(sender As Object, e As EventArgs)
        Try
            If Not ActiveSessionService.IsActive(Session.SessionId) Then
                sessionTimer.Stop()
                MessageBox.Show("This session was signed out from another device.", "Session ended", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Session.CurrentUser = Nothing
                Session.SessionId = Nothing
                Dim loginForm As New frmLogin()
                loginForm.Show()
                Close()
            End If
        Catch ex As Exception
            sessionTimer.Stop()
        End Try
    End Sub
End Class