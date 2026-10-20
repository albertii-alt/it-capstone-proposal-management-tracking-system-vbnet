Option Strict On
Option Explicit On

Public NotInheritable Class AdminFilterToolbar
    Private Sub New()
    End Sub

    Public Shared Function Create(actionButtons As FlowLayoutPanel,
                                  filterControls As FlowLayoutPanel,
                                  refreshButton As Button,
                                  height As Integer,
                                  Optional buttonBesideRefresh As Button = Nothing) As TableLayoutPanel
        If actionButtons.Controls.Contains(refreshButton) Then
            actionButtons.Controls.Remove(refreshButton)
        End If

        Dim actionWidth As Integer = 8
        For Each actionButton As Control In actionButtons.Controls
            actionWidth += actionButton.Width + actionButton.Margin.Horizontal
        Next

        actionButtons.Dock = DockStyle.Fill
        actionButtons.FlowDirection = FlowDirection.LeftToRight
        actionButtons.WrapContents = False
        actionButtons.Padding = New Padding(0, 6, 4, 0)

        filterControls.Dock = DockStyle.Fill
        filterControls.FlowDirection = FlowDirection.LeftToRight
        filterControls.WrapContents = True
        filterControls.Padding = New Padding(0, 6, 4, 0)

        Dim refreshActions As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(0, 6, 0, 0)
        }
        refreshButton.Margin = New Padding(8, 0, 0, 0)
        refreshActions.Controls.Add(refreshButton)
        Dim rightActionsWidth As Integer = refreshButton.Width + refreshButton.Margin.Horizontal
        If buttonBesideRefresh IsNot Nothing Then
            buttonBesideRefresh.Margin = New Padding(0)
            refreshActions.Controls.Add(buttonBesideRefresh)
            rightActionsWidth += buttonBesideRefresh.Width + buttonBesideRefresh.Margin.Horizontal
        End If

        Dim toolbar As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 3,
            .RowCount = 1,
            .Height = height,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }
        toolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, actionWidth))
        toolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        toolbar.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, rightActionsWidth))
        toolbar.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        toolbar.Controls.Add(actionButtons, 0, 0)
        toolbar.Controls.Add(filterControls, 1, 0)
        toolbar.Controls.Add(refreshActions, 2, 0)

        Return toolbar
    End Function
End Class
