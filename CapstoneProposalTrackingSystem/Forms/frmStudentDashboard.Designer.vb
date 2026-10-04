<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmStudentDashboard
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
        Me.pnlDashboardCard = New System.Windows.Forms.Panel()
        Me.lblWelcome = New System.Windows.Forms.Label()
        Me.lblSubheading = New System.Windows.Forms.Label()
        Me.btnSubmitProposal = New System.Windows.Forms.Button()
        Me.btnMyProposals = New System.Windows.Forms.Button()
        Me.btnLogout = New System.Windows.Forms.Button()
        Me.pnlDashboardCard.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlDashboardCard
        '
        Me.pnlDashboardCard.BackColor = System.Drawing.SystemColors.Window
        Me.pnlDashboardCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlDashboardCard.Controls.Add(Me.lblWelcome)
        Me.pnlDashboardCard.Controls.Add(Me.lblSubheading)
        Me.pnlDashboardCard.Controls.Add(Me.btnSubmitProposal)
        Me.pnlDashboardCard.Controls.Add(Me.btnMyProposals)
        Me.pnlDashboardCard.Controls.Add(Me.btnLogout)
        Me.pnlDashboardCard.Location = New System.Drawing.Point(0, 0)
        Me.pnlDashboardCard.Name = "pnlDashboardCard"
        Me.pnlDashboardCard.Size = New System.Drawing.Size(360, 280)
        Me.pnlDashboardCard.TabIndex = 0
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
        Me.lblSubheading.Size = New System.Drawing.Size(94, 13)
        Me.lblSubheading.TabIndex = 1
        Me.lblSubheading.Text = "Student Dashboard"
        '
        'btnSubmitProposal
        '
        Me.btnSubmitProposal.Location = New System.Drawing.Point(25, 90)
        Me.btnSubmitProposal.Name = "btnSubmitProposal"
        Me.btnSubmitProposal.Size = New System.Drawing.Size(280, 40)
        Me.btnSubmitProposal.TabIndex = 2
        Me.btnSubmitProposal.Text = "Submit New Proposal"
        Me.btnSubmitProposal.UseVisualStyleBackColor = True
        '
        'btnMyProposals
        '
        Me.btnMyProposals.Location = New System.Drawing.Point(25, 140)
        Me.btnMyProposals.Name = "btnMyProposals"
        Me.btnMyProposals.Size = New System.Drawing.Size(280, 40)
        Me.btnMyProposals.TabIndex = 3
        Me.btnMyProposals.Text = "My Proposals"
        Me.btnMyProposals.UseVisualStyleBackColor = True
        '
        'btnLogout
        '
        Me.btnLogout.ForeColor = System.Drawing.Color.Firebrick
        Me.btnLogout.Location = New System.Drawing.Point(25, 220)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(280, 30)
        Me.btnLogout.TabIndex = 4
        Me.btnLogout.Text = "Logout"
        Me.btnLogout.UseVisualStyleBackColor = True
        '
        'frmStudentDashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.FromArgb(250, 249, 246)
        Me.ClientSize = New System.Drawing.Size(330, 275)
        Me.Controls.Add(Me.pnlDashboardCard)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmStudentDashboard"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Student Dashboard"
        Me.pnlDashboardCard.ResumeLayout(False)
        Me.pnlDashboardCard.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlDashboardCard As Panel
    Friend WithEvents lblWelcome As Label
    Friend WithEvents lblSubheading As Label
    Friend WithEvents btnSubmitProposal As Button
    Friend WithEvents btnMyProposals As Button
    Friend WithEvents btnLogout As Button
End Class