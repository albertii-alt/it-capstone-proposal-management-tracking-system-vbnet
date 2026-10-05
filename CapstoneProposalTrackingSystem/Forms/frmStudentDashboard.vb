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
    Private ReadOnly lblIdentity As New Label()
    Private ReadOnly picAvatar As New PictureBox()
    Private ReadOnly bell As New Label()
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
        headerMeta.Height = 84
        headerMeta.ColumnCount = 6
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 104.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 116.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 40.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 56.0F))
        headerMeta.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 180.0F))
        headerMeta.Padding = New Padding(0, 12, 12, 0)
        headerMeta.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
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
        bell.Text = ChrW(&HE7ED)
        bell.Font = New Font("Segoe MDL2 Assets", 17.0F)
        bell.TextAlign = ContentAlignment.MiddleCenter
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
        lblIdentity.Font = New Font("Segoe UI", 9.0F)
        lblIdentity.TextAlign = ContentAlignment.MiddleLeft
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

    Private Sub ShowDashboard(sender As Object, e As EventArgs)
        ShowDashboard()
    End Sub

    Private Sub ShowDashboard()
        Dim dashboard As New UserControl With {.BackColor = SystemColors.Control}
        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .Padding = New Padding(20), .ColumnCount = 1, .RowCount = 3}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 112.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 130.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        Dim title As New Label With {.Text = "Dashboard", .Dock = DockStyle.Fill, .Font = New Font("Segoe UI", 23.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45), .TextAlign = ContentAlignment.MiddleLeft, .Padding = New Padding(0, 5, 0, 0)}
        layout.Controls.Add(title, 0, 0)
        Dim counts As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 4}
        For index As Integer = 0 To 3
            counts.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        Next
        Dim proposals As List(Of Proposal) = ProposalService.GetMyProposals(Session.CurrentUser.UserId)
        AddMetric(counts, "Total Proposals", proposals.Count, 0)
        AddMetric(counts, "Pending", proposals.Where(Function(item) item.Status = ProposalStatus.Pending).Count(), 1)
        AddMetric(counts, "Approved", proposals.Where(Function(item) item.Status = ProposalStatus.Approved).Count(), 2)
        AddMetric(counts, "For Revision", proposals.Where(Function(item) item.Status = ProposalStatus.ForRevision).Count(), 3)
        layout.Controls.Add(counts, 0, 1)
        Dim welcome As New Label With {.Text = "Welcome, " & Session.CurrentUser.FullName & "." & Environment.NewLine & "Use the sidebar to submit proposals, review feedback, and manage your account.", .Dock = DockStyle.Top, .Height = 70, .Font = New Font("Segoe UI", 11.0F), .ForeColor = Color.DimGray}
        layout.Controls.Add(welcome, 0, 2)
        dashboard.Controls.Add(layout)
        ShowView(dashboard, "Dashboard")
    End Sub

    Private Sub AddMetric(parent As TableLayoutPanel, caption As String, value As Integer, column As Integer)
        Dim panel As New Panel With {.Dock = DockStyle.Fill, .BackColor = SystemColors.Window, .Margin = New Padding(0, 0, 12, 0)}
        panel.Controls.Add(New Label With {.Text = value.ToString(), .Dock = DockStyle.Top, .Height = 58, .Font = New Font("Segoe UI", 22.0F, FontStyle.Bold), .ForeColor = sidebarColor, .TextAlign = ContentAlignment.MiddleCenter})
        panel.Controls.Add(New Label With {.Text = caption, .Dock = DockStyle.Bottom, .Height = 36, .Font = New Font("Segoe UI", 9.0F), .ForeColor = Color.DimGray, .TextAlign = ContentAlignment.MiddleCenter})
        parent.Controls.Add(panel, column, 0)
    End Sub

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