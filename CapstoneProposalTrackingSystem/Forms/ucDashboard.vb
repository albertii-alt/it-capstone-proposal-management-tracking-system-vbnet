Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Drawing.Drawing2D
Imports System.Linq
Imports ScottPlot.WinForms

Public Class ucDashboard
    Inherits UserControl

    Public Event DuplicateReviewRequested As EventHandler
    Public Event ActivityRequested As EventHandler

    Private ReadOnly pnlDuplicateRisks As New Panel()
    Private ReadOnly pnlRecentActivity As New Panel()
    Private ReadOnly plotStatusTrend As New FormsPlot()
    Friend ReadOnly lblPendingCount As New Label()
    Friend ReadOnly lblApprovedCount As New Label()
    Friend ReadOnly lblRevisionCount As New Label()
    Friend ReadOnly lblRejectedCount As New Label()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim dashboardLayout As New TableLayoutPanel()
        Dim cardsLayout As New TableLayoutPanel()
        Dim sectionCardsLayout As New TableLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Name = "ucDashboard"
        Me.Size = New Size(820, 520)

        dashboardLayout.Name = "tlpDashboard"
        dashboardLayout.Dock = DockStyle.Fill
        dashboardLayout.Padding = New Padding(16)
        dashboardLayout.ColumnCount = 1
        dashboardLayout.RowCount = 3
        dashboardLayout.GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        dashboardLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        dashboardLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 140.0F))
        dashboardLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 300.0F))
        dashboardLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        lblPendingCount.Name = "lblPendingCount"
        lblApprovedCount.Name = "lblApprovedCount"
        lblRevisionCount.Name = "lblRevisionCount"
        lblRejectedCount.Name = "lblRejectedCount"

        cardsLayout.Name = "tlpSummaryCards"
        cardsLayout.Dock = DockStyle.Fill
        cardsLayout.Padding = New Padding(16, 0, 16, 0)
        cardsLayout.ColumnCount = 4
        cardsLayout.RowCount = 1
        cardsLayout.GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        cardsLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        cardsLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        cardsLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        cardsLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        cardsLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        cardsLayout.Controls.Add(CreateSummaryCard("Pending", lblPendingCount, Color.FromArgb(246, 201, 69)), 0, 0)
        cardsLayout.Controls.Add(CreateSummaryCard("Approved", lblApprovedCount, Color.FromArgb(76, 154, 106)), 1, 0)
        cardsLayout.Controls.Add(CreateSummaryCard("For Revision", lblRevisionCount, Color.FromArgb(201, 138, 43)), 2, 0)
        cardsLayout.Controls.Add(CreateSummaryCard("Rejected", lblRejectedCount, Color.FromArgb(166, 58, 58)), 3, 0)

        ConfigureStatusTrendChart()

        pnlDuplicateRisks.Name = "pnlDuplicateRisks"
        pnlDuplicateRisks.Dock = DockStyle.Fill
        pnlDuplicateRisks.AutoSize = False
        pnlDuplicateRisks.AutoScroll = True
        pnlDuplicateRisks.Padding = New Padding(12, 0, 12, 8)

        pnlRecentActivity.Name = "pnlRecentActivity"
        pnlRecentActivity.Dock = DockStyle.Fill
        pnlRecentActivity.AutoScroll = True
        pnlRecentActivity.Padding = New Padding(12, 6, 12, 0)

        sectionCardsLayout.Name = "tlpSectionCards"
        sectionCardsLayout.Dock = DockStyle.Fill
        sectionCardsLayout.ColumnCount = 2
        sectionCardsLayout.RowCount = 1
        sectionCardsLayout.Padding = New Padding(0)
        sectionCardsLayout.GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        sectionCardsLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 62.0F))
        sectionCardsLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 38.0F))
        sectionCardsLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim duplicateRiskCard As Panel = CreateSectionCard(
            "Pending Proposals Needing Duplicate Review",
            Color.FromArgb(201, 138, 43),
            pnlDuplicateRisks,
            "View all",
            AddressOf DuplicateReviewViewAll_Click)
        duplicateRiskCard.Margin = New Padding(0, 0, 12, 0)
        sectionCardsLayout.Controls.Add(duplicateRiskCard, 0, 0)

        Dim recentActivityCard As Panel = CreateSectionCard(
            "Recent Activity",
            Color.FromArgb(122, 31, 46),
            pnlRecentActivity,
            "View all",
            AddressOf ActivityViewAll_Click)
        recentActivityCard.Margin = New Padding(12, 0, 0, 0)
        sectionCardsLayout.Controls.Add(recentActivityCard, 1, 0)

        dashboardLayout.Controls.Add(cardsLayout, 0, 0)
        dashboardLayout.Controls.Add(CreateSectionCard(
            "Proposal Status Trend",
            Color.FromArgb(122, 31, 46),
            plotStatusTrend), 0, 1)
        dashboardLayout.Controls.Add(sectionCardsLayout, 0, 2)

        Me.Controls.Add(dashboardLayout)
        Me.ResumeLayout(False)
    End Sub

    Private Function CreateSummaryCard(title As String, countLabel As Label, accentColor As Color) As Panel
        Dim card As New RoundedCardPanel()
        Dim accentStrip As New Panel()
        Dim titleLabel As New Label()

        card.BackColor = SystemColors.Window
        card.Dock = DockStyle.Fill
        card.Margin = New Padding(8, 8, 8, 8)

        accentStrip.Dock = DockStyle.Left
        accentStrip.Width = 4
        accentStrip.BackColor = accentColor

        titleLabel.AutoSize = True
        titleLabel.Location = New Point(16, 13)
        titleLabel.Text = title
        titleLabel.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        titleLabel.ForeColor = Color.DimGray

        countLabel.AutoSize = True
        countLabel.Location = New Point(16, 39)
        countLabel.Font = New Font("Segoe UI", 20.0F, FontStyle.Bold)
        countLabel.ForeColor = Color.FromArgb(45, 45, 45)
        countLabel.Text = "0"

        card.Controls.Add(accentStrip)
        card.Controls.Add(titleLabel)
        card.Controls.Add(countLabel)

        Return card
    End Function

    Private Function CreateSectionCard(
            title As String,
            accentColor As Color,
            contentPanel As Control,
            Optional actionText As String = Nothing,
            Optional actionHandler As EventHandler = Nothing) As Panel
        Dim card As New RoundedCardPanel()
        Dim accentStrip As New Panel()
        Dim headerLayout As New TableLayoutPanel()
        Dim headerLabel As New Label()

        card.BackColor = SystemColors.Window
        card.Dock = DockStyle.Fill

        accentStrip.Dock = DockStyle.Left
        accentStrip.Width = 4
        accentStrip.BackColor = accentColor

        headerLayout.Dock = DockStyle.Top
        headerLayout.Height = 36
        headerLayout.Padding = New Padding(16, 0, 10, 0)
        headerLayout.ColumnCount = 2
        headerLayout.RowCount = 1
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        headerLayout.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        headerLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        headerLabel.AutoSize = False
        headerLabel.Dock = DockStyle.Fill
        headerLabel.Text = title
        headerLabel.Font = New Font("Segoe UI", 10.5F, FontStyle.Bold)
        headerLabel.ForeColor = Color.FromArgb(40, 40, 40)
        headerLabel.TextAlign = ContentAlignment.MiddleLeft

        headerLayout.Controls.Add(headerLabel, 0, 0)
        If actionHandler IsNot Nothing Then
            Dim actionButton As New Button()
            actionButton.AutoSize = True
            actionButton.AutoSizeMode = AutoSizeMode.GrowAndShrink
            actionButton.Dock = DockStyle.Fill
            actionButton.FlatStyle = FlatStyle.Flat
            actionButton.FlatAppearance.BorderSize = 0
            actionButton.BackColor = Color.Transparent
            actionButton.Cursor = Cursors.Hand
            actionButton.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
            actionButton.ForeColor = Color.FromArgb(122, 31, 46)
            actionButton.Padding = New Padding(6, 0, 6, 0)
            actionButton.Text = actionText
            AddHandler actionButton.Click, actionHandler
            headerLayout.Controls.Add(actionButton, 1, 0)
        End If

        contentPanel.Dock = DockStyle.Fill

        card.Controls.Add(contentPanel)
        card.Controls.Add(headerLayout)
        card.Controls.Add(accentStrip)

        Return card
    End Function

    Private Sub DuplicateReviewViewAll_Click(sender As Object, e As EventArgs)
        RaiseEvent DuplicateReviewRequested(Me, EventArgs.Empty)
    End Sub

    Private Sub ActivityViewAll_Click(sender As Object, e As EventArgs)
        RaiseEvent ActivityRequested(Me, EventArgs.Empty)
    End Sub

    Private Function CreateRiskHeader() As TableLayoutPanel
        Dim header As New TableLayoutPanel()
        Dim teamHeader As New Label()
        Dim similarityHeader As New Label()
        Dim statusHeader As New Label()

        header.Name = "tlpDuplicateRiskHeader"
        header.Dock = DockStyle.Top
        header.Height = 32
        header.ColumnCount = 3
        header.RowCount = 1
        header.Padding = New Padding(8, 0, 8, 0)
        header.BackColor = Color.FromArgb(248, 248, 248)
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 54.0F))
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 22.0F))
        header.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 24.0F))
        header.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        teamHeader.Text = "TEAM / PROJECT"
        similarityHeader.Text = "SIMILARITY"
        statusHeader.Text = "STATUS"

        For Each columnHeader As Label In {teamHeader, similarityHeader, statusHeader}
            columnHeader.AutoSize = False
            columnHeader.Dock = DockStyle.Fill
            columnHeader.Font = New Font("Segoe UI", 8.0F, FontStyle.Bold)
            columnHeader.ForeColor = Color.DimGray
            columnHeader.TextAlign = ContentAlignment.MiddleLeft
        Next

        header.Controls.Add(teamHeader, 0, 0)
        header.Controls.Add(similarityHeader, 1, 0)
        header.Controls.Add(statusHeader, 2, 0)

        Return header
    End Function

    Private Function CreateDuplicateRiskRow(risk As PendingDuplicateRisk, displayIndex As Integer) As TableLayoutPanel
        Dim row As New TableLayoutPanel()
        Dim projectLayout As New TableLayoutPanel()
        Dim groupLabel As New Label()
        Dim projectLabel As New Label()
        Dim similarityLabel As New Label()
        Dim statusHost As New FlowLayoutPanel()
        Dim statusBadge As New Label()

        row.Name = "pnlDuplicateRiskRow" & displayIndex.ToString()
        row.Dock = DockStyle.Top
        row.Height = 62
        row.ColumnCount = 3
        row.RowCount = 1
        row.Padding = New Padding(8, 2, 8, 2)
        row.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 54.0F))
        row.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 22.0F))
        row.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 24.0F))
        row.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        projectLayout.Dock = DockStyle.Fill
        projectLayout.ColumnCount = 1
        projectLayout.RowCount = 2
        projectLayout.Padding = New Padding(0, 4, 4, 4)
        projectLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        projectLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        projectLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        groupLabel.AutoSize = False
        groupLabel.Dock = DockStyle.Fill
        groupLabel.Text = risk.GroupName
        groupLabel.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        groupLabel.ForeColor = Color.FromArgb(45, 45, 45)
        groupLabel.TextAlign = ContentAlignment.BottomLeft

        projectLabel.Name = "lblDuplicateRisk" & displayIndex.ToString()
        projectLabel.AutoSize = False
        projectLabel.Dock = DockStyle.Fill
        projectLabel.Text = risk.Title
        projectLabel.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
        projectLabel.ForeColor = Color.DimGray
        projectLabel.TextAlign = ContentAlignment.TopLeft
        projectLabel.AutoEllipsis = True

        projectLayout.Controls.Add(groupLabel, 0, 0)
        projectLayout.Controls.Add(projectLabel, 0, 1)

        similarityLabel.AutoSize = False
        similarityLabel.Dock = DockStyle.Fill
        similarityLabel.Text = Math.Round(risk.Score * 100.0).ToString("0") & "% similar"
        similarityLabel.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold)
        similarityLabel.ForeColor = Color.FromArgb(75, 75, 75)
        similarityLabel.TextAlign = ContentAlignment.MiddleLeft

        statusHost.Dock = DockStyle.Fill
        statusHost.FlowDirection = FlowDirection.LeftToRight
        statusHost.WrapContents = False
        statusHost.Padding = New Padding(0, 15, 0, 0)

        statusBadge.AutoSize = True
        statusBadge.Text = "Pending Review"
        statusBadge.Font = New Font("Segoe UI", 8.0F, FontStyle.Bold)
        statusBadge.ForeColor = Color.FromArgb(176, 104, 20)
        statusBadge.BackColor = Color.FromArgb(255, 244, 220)
        statusBadge.Padding = New Padding(8, 4, 8, 4)
        statusHost.Controls.Add(statusBadge)

        row.Controls.Add(projectLayout, 0, 0)
        row.Controls.Add(similarityLabel, 1, 0)
        row.Controls.Add(statusHost, 2, 0)

        Return row
    End Function

    Private Function CreateRecentActivityRow(entry As RecentActivityEntry, displayIndex As Integer) As TableLayoutPanel
        Dim row As New TableLayoutPanel()
        Dim marker As New Panel()
        Dim timelineLine As New Panel()
        Dim timelineDot As New Label()
        Dim activityLayout As New TableLayoutPanel()
        Dim dateLabel As New Label()
        Dim activityLabel As New Label()
        row.Name = "pnlRecentActivityRow" & displayIndex.ToString()
        row.Dock = DockStyle.Top
        row.Height = 74
        row.ColumnCount = 2
        row.RowCount = 1
        row.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 24.0F))
        row.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        row.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        marker.Dock = DockStyle.Fill
        timelineLine.BackColor = Color.FromArgb(225, 225, 225)
        timelineLine.Location = New Point(8, 0)
        timelineLine.Size = New Size(1, 74)

        timelineDot.AutoSize = True
        timelineDot.Text = "●"
        timelineDot.Location = New Point(1, 5)
        timelineDot.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)
        timelineDot.ForeColor = GetActivityColor(entry.NewStatus)
        marker.Controls.Add(timelineLine)
        marker.Controls.Add(timelineDot)

        activityLayout.Dock = DockStyle.Fill
        activityLayout.ColumnCount = 1
        activityLayout.RowCount = 2
        activityLayout.Padding = New Padding(4, 2, 4, 5)
        activityLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        activityLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        activityLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        dateLabel.AutoSize = False
        dateLabel.Dock = DockStyle.Fill
        dateLabel.Text = entry.ChangedAt.ToString("MMM d, h:mm tt")
        dateLabel.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular)
        dateLabel.ForeColor = Color.DimGray
        dateLabel.TextAlign = ContentAlignment.MiddleLeft

        activityLabel.Name = "lblRecentActivity" & displayIndex.ToString()
        activityLabel.AutoSize = False
        activityLabel.Dock = DockStyle.Fill
        activityLabel.Text = entry.ActivityText
        activityLabel.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        activityLabel.ForeColor = Color.FromArgb(60, 60, 60)
        activityLabel.TextAlign = ContentAlignment.TopLeft
        activityLabel.AutoEllipsis = True

        activityLayout.Controls.Add(dateLabel, 0, 0)
        activityLayout.Controls.Add(activityLabel, 0, 1)
        row.Controls.Add(marker, 0, 0)
        row.Controls.Add(activityLayout, 1, 0)

        Return row
    End Function

    Private Function GetActivityColor(status As String) As Color
        If String.Equals(status, "Submitted", StringComparison.OrdinalIgnoreCase) Then
            Return Color.FromArgb(76, 154, 106)
        End If

        If String.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase) Then
            Return Color.FromArgb(76, 154, 106)
        End If

        If String.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase) OrElse
           String.Equals(status, "Deleted", StringComparison.OrdinalIgnoreCase) Then
            Return Color.FromArgb(166, 58, 58)
        End If

        Return Color.FromArgb(201, 138, 43)
    End Function

    Public Sub LoadCounts()
        Try
            Dim counts As Dictionary(Of ProposalStatus, Integer) = ProposalService.GetStatusCounts()

            lblPendingCount.Text = counts(ProposalStatus.Pending).ToString()
            lblApprovedCount.Text = counts(ProposalStatus.Approved).ToString()
            lblRevisionCount.Text = counts(ProposalStatus.ForRevision).ToString()
            lblRejectedCount.Text = counts(ProposalStatus.Rejected).ToString()
        Catch ex As Exception
            MessageBox.Show("Could not load the summary counts: " & ex.Message)
        End Try
    End Sub

    Public Sub LoadDuplicateRisks()
        pnlDuplicateRisks.Controls.Clear()

        Try
            Dim risks As List(Of PendingDuplicateRisk) = ProposalService.GetPendingDuplicateRisks()

            If risks.Count = 0 Then
                Dim emptyMessage As New Label()
                emptyMessage.AutoSize = False
                emptyMessage.Dock = DockStyle.Top
                emptyMessage.Height = 44
                emptyMessage.Text = "No duplicate risks detected among pending proposals."
                emptyMessage.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
                emptyMessage.ForeColor = Color.DimGray
                emptyMessage.TextAlign = ContentAlignment.MiddleLeft
                emptyMessage.Padding = New Padding(8, 0, 0, 0)
                pnlDuplicateRisks.Controls.Add(emptyMessage)
                Return
            End If

            Dim displayCount As Integer = Math.Min(risks.Count, 5)

            If risks.Count > 5 Then
                Dim moreLabel As New Label()
                moreLabel.AutoSize = False
                moreLabel.Dock = DockStyle.Top
                moreLabel.Height = 38
                moreLabel.Text = "+" & (risks.Count - 5).ToString() & " more not shown — open Manage Proposals to review all."
                moreLabel.Font = New Font("Segoe UI", 8.5F, FontStyle.Italic)
                moreLabel.ForeColor = Color.FromArgb(95, 75, 40)
                moreLabel.BackColor = Color.FromArgb(255, 244, 220)
                moreLabel.Padding = New Padding(10, 0, 8, 0)
                moreLabel.TextAlign = ContentAlignment.MiddleLeft
                pnlDuplicateRisks.Controls.Add(moreLabel)
            End If

            For i As Integer = displayCount - 1 To 0 Step -1
                Dim risk As PendingDuplicateRisk = risks(i)
                pnlDuplicateRisks.Controls.Add(CreateDuplicateRiskRow(risk, i + 1))
            Next

            pnlDuplicateRisks.Controls.Add(CreateRiskHeader())
        Catch ex As Exception
            MessageBox.Show("Could not load duplicate risks: " & ex.Message)
        End Try
    End Sub

    Public Sub LoadRecentActivity()
        pnlRecentActivity.Controls.Clear()

        Try
            Dim entries As List(Of RecentActivityEntry) = ProposalService.GetRecentActivity(6)

            If entries.Count = 0 Then
                Dim emptyMessage As New Label()
                emptyMessage.AutoSize = False
                emptyMessage.Dock = DockStyle.Top
                emptyMessage.Height = 44
                emptyMessage.Text = "No recent activity yet."
                emptyMessage.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
                emptyMessage.ForeColor = Color.DimGray
                emptyMessage.Padding = New Padding(8, 0, 0, 0)
                emptyMessage.TextAlign = ContentAlignment.MiddleLeft
                pnlRecentActivity.Controls.Add(emptyMessage)
                Return
            End If

            For i As Integer = entries.Count - 1 To 0 Step -1
                Dim entry As RecentActivityEntry = entries(i)
                pnlRecentActivity.Controls.Add(CreateRecentActivityRow(entry, i + 1))
            Next
        Catch ex As Exception
            MessageBox.Show("Could not load recent activity: " & ex.Message)
        End Try
    End Sub

    Private Sub ucDashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCounts()
        LoadStatusTrend()
        LoadDuplicateRisks()
        LoadRecentActivity()
    End Sub

    Private Sub ConfigureStatusTrendChart()
        plotStatusTrend.Name = "plotStatusTrend"
        plotStatusTrend.Dock = DockStyle.Fill
        plotStatusTrend.Padding = New Padding(8, 2, 8, 8)
        plotStatusTrend.Margin = New Padding(10, 0, 10, 6)
        plotStatusTrend.Plot.FigureBackground.Color = ScottPlot.Color.FromColor(SystemColors.Window)
        plotStatusTrend.Plot.DataBackground.Color = ScottPlot.Color.FromColor(SystemColors.Window)
        plotStatusTrend.Plot.Axes.Left.Min = 0
        plotStatusTrend.Plot.Axes.Left.MajorTickStyle.Length = 0
        plotStatusTrend.Plot.Axes.Bottom.MajorTickStyle.Length = 0
        plotStatusTrend.Plot.Axes.Margins(0.02, 0.15)
    End Sub

    Private Sub LoadStatusTrend()
        Try
            Dim proposals As List(Of Proposal) = ProposalService.GetAllProposals()
            Dim firstMonth As DateTime = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-5)
            Dim months As DateTime() = Enumerable.Range(0, 6).Select(Function(index) firstMonth.AddMonths(index)).ToArray()
            Dim monthLabels As String() = months.Select(Function(month) month.ToString("MMM")).ToArray()
            Dim xValues As Double() = Enumerable.Range(0, 6).Select(Function(index) CDbl(index)).ToArray()

            plotStatusTrend.Plot.Clear()
            AddStatusTrendSeries(plotStatusTrend, proposals, months, xValues, ProposalStatus.Pending,
                                 "Pending", Color.FromArgb(201, 138, 43))
            AddStatusTrendSeries(plotStatusTrend, proposals, months, xValues, ProposalStatus.Approved,
                                 "Approved", Color.FromArgb(76, 154, 106))
            AddStatusTrendSeries(plotStatusTrend, proposals, months, xValues, ProposalStatus.ForRevision,
                                 "For Revision", Color.FromArgb(218, 169, 55))
            AddStatusTrendSeries(plotStatusTrend, proposals, months, xValues, ProposalStatus.Rejected,
                                 "Rejected", Color.FromArgb(166, 58, 58))
            plotStatusTrend.Plot.Axes.Bottom.SetTicks(xValues, monthLabels)
            plotStatusTrend.Plot.Axes.Left.Label.Text = "Proposals"
            plotStatusTrend.Plot.Legend.IsVisible = True
            plotStatusTrend.Plot.Legend.Alignment = ScottPlot.Alignment.UpperLeft
            plotStatusTrend.Refresh()
        Catch ex As Exception
            MessageBox.Show("Could not load the proposal status trend: " & ex.Message)
        End Try
    End Sub

    Private Shared Sub AddStatusTrendSeries(
            chart As FormsPlot,
            proposals As List(Of Proposal),
            months As DateTime(),
            xValues As Double(),
            status As ProposalStatus,
            label As String,
            color As Color)
        Dim yValues As Double() = months.Select(
            Function(month) CDbl(proposals.Where(
                Function(proposal) proposal.Status = status AndAlso
                    proposal.SubmittedAt.Year = month.Year AndAlso
                    proposal.SubmittedAt.Month = month.Month).Count())).ToArray()
        Dim scatter = chart.Plot.Add.Scatter(xValues, yValues)
        scatter.LegendText = label
        scatter.Color = ScottPlot.Color.FromColor(color)
        scatter.MarkerSize = 7
        scatter.LineWidth = 2.5F
    End Sub

End Class

Friend Class RoundedCardPanel
    Inherits Panel

    Private Const CornerRadius As Integer = 10

    Public Sub New()
        Me.BorderStyle = BorderStyle.None
        Me.SetStyle(
            ControlStyles.UserPaint Or
            ControlStyles.AllPaintingInWmPaint Or
            ControlStyles.OptimizedDoubleBuffer Or
            ControlStyles.ResizeRedraw,
            True)
    End Sub

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)

        If Me.ClientSize.Width <= 0 OrElse Me.ClientSize.Height <= 0 Then
            Return
        End If

        Using path As GraphicsPath = CreateRoundedPath(Me.ClientRectangle, CornerRadius)
            Dim previousRegion As Region = Me.Region
            Me.Region = New Region(path)

            If previousRegion IsNot Nothing Then
                previousRegion.Dispose()
            End If
        End Using
    End Sub

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        Using path As GraphicsPath = CreateRoundedPath(Me.ClientRectangle, CornerRadius)
            Using brush As New SolidBrush(Me.BackColor)
                e.Graphics.FillPath(brush, path)
            End Using
        End Using
    End Sub

    Private Shared Function CreateRoundedPath(bounds As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim diameter As Integer = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height))

        If diameter <= 0 Then
            path.AddRectangle(bounds)
            Return path
        End If

        path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180.0F, 90.0F)
        path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270.0F, 90.0F)
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0.0F, 90.0F)
        path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90.0F, 90.0F)
        path.CloseFigure()

        Return path
    End Function
End Class