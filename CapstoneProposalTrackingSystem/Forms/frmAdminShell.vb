Option Strict On
Option Explicit On

Public Class frmAdminShell
    Inherits Form

    Private ReadOnly _sidebarColor As Color = Color.FromArgb(122, 31, 46)
    Private ReadOnly _navTextColor As Color = Color.FromArgb(240, 217, 160)
    Private ReadOnly _activeColor As Color = Color.FromArgb(246, 201, 69)

    Private ReadOnly pnlSidebar As New Panel()
    Private ReadOnly pnlContent As New Panel()
    Private ReadOnly pnlLogoTopSpacer As New Panel()
    Private ReadOnly pnlNavigationTopSpacer As New Panel()
    Private ReadOnly pnlLogoutBottomSpacer As New Panel()
    Private ReadOnly picLogo As New PictureBox()
    Private ReadOnly lblAppName As New Label()
    Private ReadOnly btnNavDashboard As New Button()
    Private ReadOnly btnNavManageProposals As New Button()
    Private ReadOnly btnNavTitleArchive As New Button()
    Private ReadOnly btnNavReports As New Button()
    Private ReadOnly btnNavManageAccounts As New Button()
    Private ReadOnly btnNavStudentRegistrations As New Button()
    Private ReadOnly btnNavActivityLogs As New Button()
    Private ReadOnly btnNavLoginActivity As New Button()
    Private ReadOnly btnNavSecurityLogs As New Button()
    Private ReadOnly btnNavProfile As New Button()
    Private ReadOnly btnNavLogout As New Button()
    Private ReadOnly sessionTimer As New Timer With {.Interval = 30000}

    Public Sub New()
        InitializeComponent()
        ConfigureLayout()
        AddHandler btnNavDashboard.Click, AddressOf btnNavDashboard_Click
        AddHandler btnNavManageProposals.Click, AddressOf btnNavManageProposals_Click
        AddHandler btnNavTitleArchive.Click, AddressOf btnNavTitleArchive_Click
        AddHandler btnNavReports.Click, AddressOf btnNavReports_Click
        AddHandler btnNavManageAccounts.Click, AddressOf btnNavManageAccounts_Click
        AddHandler btnNavStudentRegistrations.Click, AddressOf btnNavStudentRegistrations_Click
        AddHandler btnNavActivityLogs.Click, AddressOf btnNavActivityLogs_Click
        AddHandler btnNavLoginActivity.Click, AddressOf btnNavLoginActivity_Click
        AddHandler btnNavSecurityLogs.Click, AddressOf btnNavSecurityLogs_Click
        AddHandler btnNavProfile.Click, AddressOf btnNavProfile_Click
        AddHandler btnNavLogout.Click, AddressOf btnNavLogout_Click
        AddHandler MyBase.Load, AddressOf frmAdminShell_Load
        AddHandler sessionTimer.Tick, AddressOf sessionTimer_Tick
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.ClientSize = New Size(1000, 650)
        Me.MinimumSize = New Size(800, 500)
        Me.Name = "frmAdminShell"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "Capstone System - Admin"
        Me.ResumeLayout(False)
    End Sub

    Private Sub ConfigureLayout()
        pnlSidebar.Name = "pnlSidebar"
        pnlSidebar.Dock = DockStyle.Left
        pnlSidebar.Width = 250
        pnlSidebar.BackColor = _sidebarColor

        pnlContent.Name = "pnlContent"
        pnlContent.Dock = DockStyle.Fill
        pnlContent.BackColor = SystemColors.Control

        pnlLogoTopSpacer.Name = "pnlLogoTopSpacer"
        pnlLogoTopSpacer.Dock = DockStyle.Top
        pnlLogoTopSpacer.Height = 24
        pnlLogoTopSpacer.BackColor = _sidebarColor

        picLogo.Name = "picLogo"
        picLogo.Dock = DockStyle.Top
        picLogo.Height = 108
        picLogo.Padding = New Padding(0, 12, 0, 0)
        picLogo.SizeMode = PictureBoxSizeMode.Zoom
        picLogo.BackColor = _sidebarColor
        LoadLogoImage()

        lblAppName.Name = "lblAppName"
        lblAppName.Text = "IT Capstone Proposal Management and Tracking System"
        lblAppName.Dock = DockStyle.Top
        lblAppName.Height = 64
        lblAppName.AutoSize = False
        lblAppName.TextAlign = ContentAlignment.MiddleCenter
        lblAppName.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
        lblAppName.ForeColor = _activeColor
        lblAppName.BackColor = _sidebarColor
        lblAppName.Padding = New Padding(8, 2, 8, 2)

        pnlNavigationTopSpacer.Name = "pnlNavigationTopSpacer"
        pnlNavigationTopSpacer.Dock = DockStyle.Top
        pnlNavigationTopSpacer.Height = 18
        pnlNavigationTopSpacer.BackColor = _sidebarColor

        ConfigureNavButton(btnNavDashboard, "btnNavDashboard", "Dashboard", ChrW(&HE80F))
        ConfigureNavButton(btnNavManageProposals, "btnNavManageProposals", "Manage Proposals", ChrW(&HE8A5))
        ConfigureNavButton(btnNavTitleArchive, "btnNavTitleArchive", "Title Archive", ChrW(&HE7B8))
        ConfigureNavButton(btnNavReports, "btnNavReports", "Reports", ChrW(&HE9D2))
        ConfigureNavButton(btnNavManageAccounts, "btnNavManageAccounts", "Manage Accounts", ChrW(&HE716))
        ConfigureNavButton(btnNavStudentRegistrations, "btnNavStudentRegistrations", "Student Registrations", ChrW(&HE77B))
        ConfigureNavButton(btnNavActivityLogs, "btnNavActivityLogs", "Activity Logs", ChrW(&HE9D9))
        ConfigureNavButton(btnNavLoginActivity, "btnNavLoginActivity", "My Login Activity", ChrW(&HE81C))
        ConfigureNavButton(btnNavSecurityLogs, "btnNavSecurityLogs", "Security Logs", ChrW(&HE72E))
        ConfigureNavButton(btnNavProfile, "btnNavProfile", "Profile", ChrW(&HE77B))

        btnNavLogout.Name = "btnNavLogout"
        btnNavLogout.Text = "   Logout"
        btnNavLogout.Dock = DockStyle.Bottom
        btnNavLogout.Height = 48
        btnNavLogout.FlatStyle = FlatStyle.Flat
        btnNavLogout.FlatAppearance.BorderSize = 0
        btnNavLogout.TextAlign = ContentAlignment.MiddleLeft
        btnNavLogout.Padding = New Padding(12, 0, 0, 0)
        btnNavLogout.ImageAlign = ContentAlignment.MiddleLeft
        btnNavLogout.TextImageRelation = TextImageRelation.ImageBeforeText
        btnNavLogout.Image = CreateNavIcon(ChrW(&HE7E8), Color.FromArgb(242, 166, 166))
        btnNavLogout.BackColor = _sidebarColor
        btnNavLogout.ForeColor = Color.FromArgb(242, 166, 166)

        pnlLogoutBottomSpacer.Name = "pnlLogoutBottomSpacer"
        pnlLogoutBottomSpacer.Dock = DockStyle.Bottom
        pnlLogoutBottomSpacer.Height = 12
        pnlLogoutBottomSpacer.BackColor = _sidebarColor

        pnlSidebar.Controls.Add(btnNavLogout)
        pnlSidebar.Controls.Add(pnlLogoutBottomSpacer)
        pnlSidebar.Controls.Add(btnNavProfile)
        pnlSidebar.Controls.Add(btnNavSecurityLogs)
        pnlSidebar.Controls.Add(btnNavLoginActivity)
        pnlSidebar.Controls.Add(btnNavActivityLogs)
        pnlSidebar.Controls.Add(btnNavManageAccounts)
        pnlSidebar.Controls.Add(btnNavStudentRegistrations)
        pnlSidebar.Controls.Add(btnNavReports)
        pnlSidebar.Controls.Add(btnNavTitleArchive)
        pnlSidebar.Controls.Add(btnNavManageProposals)
        pnlSidebar.Controls.Add(btnNavDashboard)
        pnlSidebar.Controls.Add(pnlNavigationTopSpacer)
        pnlSidebar.Controls.Add(lblAppName)
        pnlSidebar.Controls.Add(picLogo)
        pnlSidebar.Controls.Add(pnlLogoTopSpacer)

        Me.Controls.Add(pnlContent)
        Me.Controls.Add(pnlSidebar)
    End Sub

    Private Sub LoadLogoImage()
        Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
        Using stream As System.IO.Stream = asm.GetManifestResourceStream("CapstoneProposalTrackingSystem.CCS_Logo.png")
            If stream Is Nothing Then
                MessageBox.Show("Logo resource not found.", "Unable to load logo", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Using sourceImage As Image = Image.FromStream(stream)
                picLogo.Image = New Bitmap(sourceImage)
            End Using
        End Using
    End Sub

    Private Sub ConfigureNavButton(navButton As Button, controlName As String, buttonText As String, iconGlyph As String)
        navButton.Name = controlName
        navButton.Text = "   " & buttonText
        navButton.Dock = DockStyle.Top
        navButton.Height = 46
        navButton.FlatStyle = FlatStyle.Flat
        navButton.FlatAppearance.BorderSize = 0
        navButton.TextAlign = ContentAlignment.MiddleLeft
        navButton.Padding = New Padding(12, 0, 0, 0)
        navButton.ImageAlign = ContentAlignment.MiddleLeft
        navButton.TextImageRelation = TextImageRelation.ImageBeforeText
        navButton.BackColor = _sidebarColor
        navButton.ForeColor = _navTextColor
        navButton.Tag = iconGlyph
        navButton.Image = CreateNavIcon(iconGlyph, _navTextColor)
    End Sub

    Private Function CreateNavIcon(iconGlyph As String, iconColor As Color) As Bitmap
        Dim icon As New Bitmap(28, 28)

        Using graphics As Graphics = Graphics.FromImage(icon)
            graphics.Clear(Color.Transparent)
            Using iconFont As New Font("Segoe MDL2 Assets", 16.0F, FontStyle.Regular, GraphicsUnit.Point)
                TextRenderer.DrawText(
                    graphics,
                    iconGlyph,
                    iconFont,
                    New Rectangle(0, 0, icon.Width, icon.Height),
                    iconColor,
                    Color.Transparent,
                    TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
            End Using
        End Using

        Return icon
    End Function

    Private Sub SetNavButtonIcon(navButton As Button, iconColor As Color)
        Dim iconGlyph As String = TryCast(navButton.Tag, String)
        If String.IsNullOrEmpty(iconGlyph) Then Return

        Dim previousIcon As Image = navButton.Image
        navButton.Image = CreateNavIcon(iconGlyph, iconColor)
        If previousIcon IsNot Nothing Then previousIcon.Dispose()
    End Sub

    Private Sub SetActiveNavButton(activeButton As Button)
        Dim navButtons As Button() = {
            btnNavDashboard,
            btnNavManageProposals,
            btnNavTitleArchive,
            btnNavReports,
            btnNavManageAccounts,
            btnNavStudentRegistrations,
            btnNavActivityLogs,
            btnNavLoginActivity,
            btnNavSecurityLogs,
            btnNavProfile
        }

        For Each navButton As Button In navButtons
            navButton.BackColor = _sidebarColor
            navButton.ForeColor = _navTextColor
            SetNavButtonIcon(navButton, _navTextColor)
        Next

        activeButton.BackColor = _activeColor
        activeButton.ForeColor = _sidebarColor
        SetNavButtonIcon(activeButton, _sidebarColor)
    End Sub

    Private Sub LoadContent(control As UserControl, activeButton As Button)
        pnlContent.Controls.Clear()
        control.Dock = DockStyle.Fill
        pnlContent.Controls.Add(control)
        SetActiveNavButton(activeButton)
    End Sub

    Private Function CreatePlaceholderContent(pageName As String) As UserControl
        Dim content As New UserControl()
        Dim message As New Label()

        content.BackColor = SystemColors.Control
        message.Text = "Coming soon: " & pageName
        message.Dock = DockStyle.Fill
        message.TextAlign = ContentAlignment.MiddleCenter
        content.Controls.Add(message)

        Return content
    End Function

    Private Sub frmAdminShell_Load(sender As Object, e As EventArgs)
        FormDisplayHelper.MaximizeFullScreen(Me)
        If Session.CurrentUser Is Nothing OrElse Not UserRoleHelper.IsAdministrator(Session.CurrentUser.Role) Then
            MessageBox.Show("Access denied.")
            Me.Close()
            Return
        End If

        btnNavDashboard.PerformClick()
        sessionTimer.Start()
    End Sub

    Private Sub btnNavDashboard_Click(sender As Object, e As EventArgs)
        Dim dashboard As New ucDashboard()
        AddHandler dashboard.ProfileRequested, AddressOf ucDashboard_ProfileRequested
        AddHandler dashboard.DuplicateReviewRequested, AddressOf ucDashboard_DuplicateReviewRequested
        AddHandler dashboard.ActivityRequested, AddressOf ucDashboard_ActivityRequested
        LoadContent(dashboard, btnNavDashboard)
    End Sub

    Private Sub ucDashboard_ProfileRequested(sender As Object, e As EventArgs)
        btnNavProfile.PerformClick()
    End Sub

    Private Sub ucDashboard_DuplicateReviewRequested(sender As Object, e As EventArgs)
        btnNavManageProposals.PerformClick()
    End Sub

    Private Sub ucDashboard_ActivityRequested(sender As Object, e As EventArgs)
        btnNavActivityLogs.PerformClick()
    End Sub

    Private Sub btnNavManageProposals_Click(sender As Object, e As EventArgs)
        LoadContent(New ucManageProposals(), btnNavManageProposals)
    End Sub

    Private Sub btnNavTitleArchive_Click(sender As Object, e As EventArgs)
        LoadContent(New ucTitleArchive(), btnNavTitleArchive)
    End Sub

    Private Sub btnNavReports_Click(sender As Object, e As EventArgs)
        LoadContent(New ucReports(), btnNavReports)
    End Sub

    Private Sub btnNavManageAccounts_Click(sender As Object, e As EventArgs)
        LoadContent(New ucManageAccounts(), btnNavManageAccounts)
    End Sub

    Private Sub btnNavStudentRegistrations_Click(sender As Object, e As EventArgs)
        LoadContent(New ucStudentRegistrations(), btnNavStudentRegistrations)
    End Sub

    Private Sub btnNavActivityLogs_Click(sender As Object, e As EventArgs)
        LoadContent(New ucAdminActivityLogs(), btnNavActivityLogs)
    End Sub

    Private Sub btnNavLoginActivity_Click(sender As Object, e As EventArgs)
        LoadContent(New ucMyLoginActivity(), btnNavLoginActivity)
    End Sub

    Private Sub btnNavSecurityLogs_Click(sender As Object, e As EventArgs)
        LoadContent(New ucSecurityLogs(), btnNavSecurityLogs)
    End Sub

    Private Sub btnNavProfile_Click(sender As Object, e As EventArgs)
        LoadContent(New ucProfile(), btnNavProfile)
    End Sub

    Private Sub btnNavLogout_Click(sender As Object, e As EventArgs)
        Dim confirmation As DialogResult = MessageBox.Show(
            "Are you sure you want to log out?",
            "Confirm logout",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2)

        If confirmation <> DialogResult.Yes Then Return

        Try
            LoginActivityService.RecordLogout(Session.CurrentUser)
            If Session.CurrentUser IsNot Nothing Then
                SecurityLogService.Record(Session.CurrentUser.UserId, Session.CurrentUser.Username,
                                          "Logout", "The account signed out normally.")
            End If
        Catch ex As Exception
            MessageBox.Show("Your logout could not be recorded, but the session will still be closed." &
                            Environment.NewLine & ex.Message,
                            "Logout Activity", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
        Session.CurrentUser = Nothing
        Session.SessionId = Nothing
        Dim loginForm As New frmLogin()
        loginForm.Show()
        Me.Close()
    End Sub

    Private Sub sessionTimer_Tick(sender As Object, e As EventArgs)
        Try
            If Not ActiveSessionService.IsActive(Session.SessionId) Then
                sessionTimer.Stop()
                MessageBox.Show("This session was signed out from another device.", "Session ended",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                If Session.CurrentUser IsNot Nothing Then
                    SecurityLogService.Record(Session.CurrentUser.UserId, Session.CurrentUser.Username,
                                              "Session Expired", "The session was revoked from another device.")
                End If
                Session.CurrentUser = Nothing
                Session.SessionId = Nothing
                Dim loginForm As New frmLogin()
                loginForm.Show()
                Me.Close()
                Return
            End If
            ActiveSessionService.Touch(Session.SessionId)
        Catch ex As Exception
        End Try
    End Sub

End Class