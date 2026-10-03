<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmAdminDashboard
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblWelcome = New System.Windows.Forms.Label()
        Me.lblSubheading = New System.Windows.Forms.Label()
        Me.lblPendingTitle = New System.Windows.Forms.Label()
        Me.lblPendingCount = New System.Windows.Forms.Label()
        Me.lblApprovedTitle = New System.Windows.Forms.Label()
        Me.lblApprovedCount = New System.Windows.Forms.Label()
        Me.lblRevisionTitle = New System.Windows.Forms.Label()
        Me.lblRevisionCount = New System.Windows.Forms.Label()
        Me.lblRejectedTitle = New System.Windows.Forms.Label()
        Me.lblRejectedCount = New System.Windows.Forms.Label()
        Me.btnManageProposals = New System.Windows.Forms.Button()
        Me.btnTitleArchive = New System.Windows.Forms.Button()
        Me.btnReports = New System.Windows.Forms.Button()
        Me.btnManageAccounts = New System.Windows.Forms.Button()
        Me.btnLogout = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblWelcome
        '
        Me.lblWelcome.AutoSize = True
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Bold)
        Me.lblWelcome.Location = New System.Drawing.Point(25, 20)
        Me.lblWelcome.Name = "lblWelcome"
        Me.lblWelcome.Size = New System.Drawing.Size(120, 20)
        Me.lblWelcome.TabIndex = 0
        Me.lblWelcome.Text = "Welcome, ..."
        '
        'lblSubheading
        '
        Me.lblSubheading.AutoSize = True
        Me.lblSubheading.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblSubheading.Location = New System.Drawing.Point(25, 48)
        Me.lblSubheading.Name = "lblSubheading"
        Me.lblSubheading.Size = New System.Drawing.Size(85, 13)
        Me.lblSubheading.TabIndex = 1
        Me.lblSubheading.Text = "Admin Dashboard"
        '
        'lblPendingTitle
        '
        Me.lblPendingTitle.AutoSize = True
        Me.lblPendingTitle.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblPendingTitle.Location = New System.Drawing.Point(25, 85)
        Me.lblPendingTitle.Name = "lblPendingTitle"
        Me.lblPendingTitle.Size = New System.Drawing.Size(45, 13)
        Me.lblPendingTitle.TabIndex = 2
        Me.lblPendingTitle.Text = "Pending"
        '
        'lblPendingCount
        '
        Me.lblPendingCount.AutoSize = True
        Me.lblPendingCount.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblPendingCount.Location = New System.Drawing.Point(25, 100)
        Me.lblPendingCount.Name = "lblPendingCount"
        Me.lblPendingCount.Size = New System.Drawing.Size(24, 30)
        Me.lblPendingCount.TabIndex = 3
        Me.lblPendingCount.Text = "0"
        '
        'lblApprovedTitle
        '
        Me.lblApprovedTitle.AutoSize = True
        Me.lblApprovedTitle.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblApprovedTitle.Location = New System.Drawing.Point(115, 85)
        Me.lblApprovedTitle.Name = "lblApprovedTitle"
        Me.lblApprovedTitle.Size = New System.Drawing.Size(51, 13)
        Me.lblApprovedTitle.TabIndex = 4
        Me.lblApprovedTitle.Text = "Approved"
        '
        'lblApprovedCount
        '
        Me.lblApprovedCount.AutoSize = True
        Me.lblApprovedCount.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblApprovedCount.Location = New System.Drawing.Point(115, 100)
        Me.lblApprovedCount.Name = "lblApprovedCount"
        Me.lblApprovedCount.Size = New System.Drawing.Size(24, 30)
        Me.lblApprovedCount.TabIndex = 5
        Me.lblApprovedCount.Text = "0"
        '
        'lblRevisionTitle
        '
        Me.lblRevisionTitle.AutoSize = True
        Me.lblRevisionTitle.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblRevisionTitle.Location = New System.Drawing.Point(205, 85)
        Me.lblRevisionTitle.Name = "lblRevisionTitle"
        Me.lblRevisionTitle.Size = New System.Drawing.Size(66, 13)
        Me.lblRevisionTitle.TabIndex = 6
        Me.lblRevisionTitle.Text = "For Revision"
        '
        'lblRevisionCount
        '
        Me.lblRevisionCount.AutoSize = True
        Me.lblRevisionCount.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblRevisionCount.Location = New System.Drawing.Point(205, 100)
        Me.lblRevisionCount.Name = "lblRevisionCount"
        Me.lblRevisionCount.Size = New System.Drawing.Size(24, 30)
        Me.lblRevisionCount.TabIndex = 7
        Me.lblRevisionCount.Text = "0"
        '
        'lblRejectedTitle
        '
        Me.lblRejectedTitle.AutoSize = True
        Me.lblRejectedTitle.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblRejectedTitle.Location = New System.Drawing.Point(295, 85)
        Me.lblRejectedTitle.Name = "lblRejectedTitle"
        Me.lblRejectedTitle.Size = New System.Drawing.Size(50, 13)
        Me.lblRejectedTitle.TabIndex = 8
        Me.lblRejectedTitle.Text = "Rejected"
        '
        'lblRejectedCount
        '
        Me.lblRejectedCount.AutoSize = True
        Me.lblRejectedCount.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold)
        Me.lblRejectedCount.Location = New System.Drawing.Point(295, 100)
        Me.lblRejectedCount.Name = "lblRejectedCount"
        Me.lblRejectedCount.Size = New System.Drawing.Size(24, 30)
        Me.lblRejectedCount.TabIndex = 9
        Me.lblRejectedCount.Text = "0"
        '
        'btnManageProposals
        '
        Me.btnManageProposals.Location = New System.Drawing.Point(25, 155)
        Me.btnManageProposals.Name = "btnManageProposals"
        Me.btnManageProposals.Size = New System.Drawing.Size(340, 36)
        Me.btnManageProposals.TabIndex = 10
        Me.btnManageProposals.Text = "Manage Proposals"
        Me.btnManageProposals.UseVisualStyleBackColor = True
        '
        'btnTitleArchive
        '
        Me.btnTitleArchive.Location = New System.Drawing.Point(25, 200)
        Me.btnTitleArchive.Name = "btnTitleArchive"
        Me.btnTitleArchive.Size = New System.Drawing.Size(340, 36)
        Me.btnTitleArchive.TabIndex = 11
        Me.btnTitleArchive.Text = "Title Archive"
        Me.btnTitleArchive.UseVisualStyleBackColor = True
        '
        'btnReports
        '
        Me.btnReports.Location = New System.Drawing.Point(25, 245)
        Me.btnReports.Name = "btnReports"
        Me.btnReports.Size = New System.Drawing.Size(340, 36)
        Me.btnReports.TabIndex = 12
        Me.btnReports.Text = "Reports"
        Me.btnReports.UseVisualStyleBackColor = True
        '
        'btnManageAccounts
        '
        Me.btnManageAccounts.Location = New System.Drawing.Point(25, 290)
        Me.btnManageAccounts.Name = "btnManageAccounts"
        Me.btnManageAccounts.Size = New System.Drawing.Size(340, 36)
        Me.btnManageAccounts.TabIndex = 13
        Me.btnManageAccounts.Text = "Manage Accounts"
        Me.btnManageAccounts.UseVisualStyleBackColor = True
        '
        'btnLogout
        '
        Me.btnLogout.ForeColor = System.Drawing.Color.Firebrick
        Me.btnLogout.Location = New System.Drawing.Point(25, 345)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(340, 30)
        Me.btnLogout.TabIndex = 14
        Me.btnLogout.Text = "Logout"
        Me.btnLogout.UseVisualStyleBackColor = True
        '
        'frmAdminDashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(390, 400)
        Me.Controls.Add(Me.lblWelcome)
        Me.Controls.Add(Me.lblSubheading)
        Me.Controls.Add(Me.lblPendingTitle)
        Me.Controls.Add(Me.lblPendingCount)
        Me.Controls.Add(Me.lblApprovedTitle)
        Me.Controls.Add(Me.lblApprovedCount)
        Me.Controls.Add(Me.lblRevisionTitle)
        Me.Controls.Add(Me.lblRevisionCount)
        Me.Controls.Add(Me.lblRejectedTitle)
        Me.Controls.Add(Me.lblRejectedCount)
        Me.Controls.Add(Me.btnManageProposals)
        Me.Controls.Add(Me.btnTitleArchive)
        Me.Controls.Add(Me.btnReports)
        Me.Controls.Add(Me.btnManageAccounts)
        Me.Controls.Add(Me.btnLogout)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmAdminDashboard"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Admin Dashboard"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblWelcome As Label
    Friend WithEvents lblSubheading As Label
    Friend WithEvents lblPendingTitle As Label
    Friend WithEvents lblPendingCount As Label
    Friend WithEvents lblApprovedTitle As Label
    Friend WithEvents lblApprovedCount As Label
    Friend WithEvents lblRevisionTitle As Label
    Friend WithEvents lblRevisionCount As Label
    Friend WithEvents lblRejectedTitle As Label
    Friend WithEvents lblRejectedCount As Label
    Friend WithEvents btnManageProposals As Button
    Friend WithEvents btnTitleArchive As Button
    Friend WithEvents btnReports As Button
    Friend WithEvents btnManageAccounts As Button
    Friend WithEvents btnLogout As Button
End Class