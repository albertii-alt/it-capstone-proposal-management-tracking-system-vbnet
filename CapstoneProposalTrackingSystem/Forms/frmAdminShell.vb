Option Strict On
Option Explicit On

Public Class frmAdminShell
    Inherits Form

    Private ReadOnly _sidebarColor As Color = Color.FromArgb(122, 31, 46)
    Private ReadOnly _navTextColor As Color = Color.FromArgb(240, 217, 160)
    Private ReadOnly _activeColor As Color = Color.FromArgb(246, 201, 69)

    Private ReadOnly pnlSidebar As New Panel()
    Private ReadOnly pnlContent As New Panel()
    Private ReadOnly lblLogo As New Label()
    Private ReadOnly lblAppName As New Label()
    Private ReadOnly btnNavDashboard As New Button()
    Private ReadOnly btnNavManageProposals As New Button()
    Private ReadOnly btnNavTitleArchive As New Button()
    Private ReadOnly btnNavReports As New Button()
    Private ReadOnly btnNavManageAccounts As New Button()
    Private ReadOnly btnNavActivityLogs As New Button()
    Private ReadOnly btnNavLoginActivity As New Button()
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
        AddHandler btnNavActivityLogs.Click, AddressOf btnNavActivityLogs_Click
        AddHandler btnNavLoginActivity.Click, AddressOf btnNavLoginActivity_Click
        AddHandler btnNavProfile.Click, AddressOf btnNavProfile_Click
        AddHandler btnNavLogout.Click, AddressOf btnNavLogout_Click
        AddHandler MyBase.Load, AddressOf frmAdminShell_Load
        AddHandler sessionTimer.Tick, AddressOf sessionTimer_Tick
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
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
        pnlSidebar.Width = 180
        pnlSidebar.BackColor = _sidebarColor

        pnlContent.Name = "pnlContent"
        pnlContent.Dock = DockStyle.Fill
        pnlContent.BackColor = SystemColors.Control

        lblLogo.Name = "lblLogo"
        lblLogo.Text = "CCS"
        lblLogo.Dock = DockStyle.Top
        lblLogo.Height = 52
        lblLogo.TextAlign = ContentAlignment.MiddleCenter
        lblLogo.Font = New Font("Segoe UI", 20.0F, FontStyle.Bold)
        lblLogo.ForeColor = _activeColor
        lblLogo.BackColor = _sidebarColor

        lblAppName.Name = "lblAppName"
        lblAppName.Text = "Capstone System"
        lblAppName.Dock = DockStyle.Top
        lblAppName.Height = 32
        lblAppName.TextAlign = ContentAlignment.MiddleCenter
        lblAppName.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblAppName.ForeColor = _activeColor
        lblAppName.BackColor = _sidebarColor

        ConfigureNavButton(btnNavDashboard, "btnNavDashboard", "Dashboard")
        ConfigureNavButton(btnNavManageProposals, "btnNavManageProposals", "Manage Proposals")
        ConfigureNavButton(btnNavTitleArchive, "btnNavTitleArchive", "Title Archive")
        ConfigureNavButton(btnNavReports, "btnNavReports", "Reports")
        ConfigureNavButton(btnNavManageAccounts, "btnNavManageAccounts", "Manage Accounts")
        ConfigureNavButton(btnNavActivityLogs, "btnNavActivityLogs", "Activity Logs")
        ConfigureNavButton(btnNavLoginActivity, "btnNavLoginActivity", "My Login Activity")
        ConfigureNavButton(btnNavProfile, "btnNavProfile", "Profile")

        btnNavLogout.Name = "btnNavLogout"
        btnNavLogout.Text = "Logout"
        btnNavLogout.Dock = DockStyle.Bottom
        btnNavLogout.Height = 40
        btnNavLogout.FlatStyle = FlatStyle.Flat
        btnNavLogout.FlatAppearance.BorderSize = 0
        btnNavLogout.TextAlign = ContentAlignment.MiddleLeft
        btnNavLogout.Padding = New Padding(18, 0, 0, 0)
        btnNavLogout.BackColor = _sidebarColor
        btnNavLogout.ForeColor = Color.FromArgb(242, 166, 166)

        pnlSidebar.Controls.Add(btnNavLogout)
        pnlSidebar.Controls.Add(btnNavProfile)
        pnlSidebar.Controls.Add(btnNavManageAccounts)
        pnlSidebar.Controls.Add(btnNavActivityLogs)
        pnlSidebar.Controls.Add(btnNavLoginActivity)
        pnlSidebar.Controls.Add(btnNavReports)
        pnlSidebar.Controls.Add(btnNavTitleArchive)
        pnlSidebar.Controls.Add(btnNavManageProposals)
        pnlSidebar.Controls.Add(btnNavDashboard)
        pnlSidebar.Controls.Add(lblAppName)
        pnlSidebar.Controls.Add(lblLogo)

        Me.Controls.Add(pnlContent)
        Me.Controls.Add(pnlSidebar)
    End Sub

    Private Sub ConfigureNavButton(navButton As Button, controlName As String, buttonText As String)
        navButton.Name = controlName
        navButton.Text = buttonText
        navButton.Dock = DockStyle.Top
        navButton.Height = 40
        navButton.FlatStyle = FlatStyle.Flat
        navButton.FlatAppearance.BorderSize = 0
        navButton.TextAlign = ContentAlignment.MiddleLeft
        navButton.Padding = New Padding(18, 0, 0, 0)
        navButton.BackColor = _sidebarColor
        navButton.ForeColor = _navTextColor
    End Sub

    Private Sub SetActiveNavButton(activeButton As Button)
        Dim navButtons As Button() = {
            btnNavDashboard,
            btnNavManageProposals,
            btnNavTitleArchive,
            btnNavReports,
            btnNavManageAccounts,
            btnNavActivityLogs,
            btnNavLoginActivity,
            btnNavProfile
        }

        For Each navButton As Button In navButtons
            navButton.BackColor = _sidebarColor
            navButton.ForeColor = _navTextColor
        Next

        activeButton.BackColor = _activeColor
        activeButton.ForeColor = _sidebarColor
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
        LoadContent(dashboard, btnNavDashboard)
    End Sub

    Private Sub ucDashboard_ProfileRequested(sender As Object, e As EventArgs)
        btnNavProfile.PerformClick()
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

    Private Sub btnNavActivityLogs_Click(sender As Object, e As EventArgs)
        LoadContent(New ucAdminActivityLogs(), btnNavActivityLogs)
    End Sub

    Private Sub btnNavLoginActivity_Click(sender As Object, e As EventArgs)
        LoadContent(New ucMyLoginActivity(), btnNavLoginActivity)
    End Sub

    Private Sub btnNavProfile_Click(sender As Object, e As EventArgs)
        LoadContent(New ucProfile(), btnNavProfile)
    End Sub

    Private Sub btnNavLogout_Click(sender As Object, e As EventArgs)
        Try
            LoginActivityService.RecordLogout(Session.CurrentUser)
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