<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMyProposals
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
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.dgvMyProposals = New System.Windows.Forms.DataGridView()
        Me.lblRemarksField = New System.Windows.Forms.Label()
        Me.txtRemarks = New System.Windows.Forms.TextBox()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.dgvMyProposals, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblHeader
        '
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblHeader.Location = New System.Drawing.Point(20, 15)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(105, 21)
        Me.lblHeader.TabIndex = 0
        Me.lblHeader.Text = "My Proposals"
        '
        'dgvMyProposals
        '
        Me.dgvMyProposals.AllowUserToAddRows = False
        Me.dgvMyProposals.AllowUserToDeleteRows = False
        Me.dgvMyProposals.AllowUserToResizeRows = False
        Me.dgvMyProposals.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMyProposals.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvMyProposals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMyProposals.Location = New System.Drawing.Point(20, 50)
        Me.dgvMyProposals.MultiSelect = False
        Me.dgvMyProposals.Name = "dgvMyProposals"
        Me.dgvMyProposals.ReadOnly = True
        Me.dgvMyProposals.RowHeadersVisible = False
        Me.dgvMyProposals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvMyProposals.Size = New System.Drawing.Size(560, 200)
        Me.dgvMyProposals.TabIndex = 1
        '
        'lblRemarksField
        '
        Me.lblRemarksField.AutoSize = True
        Me.lblRemarksField.Location = New System.Drawing.Point(20, 265)
        Me.lblRemarksField.Name = "lblRemarksField"
        Me.lblRemarksField.Size = New System.Drawing.Size(81, 13)
        Me.lblRemarksField.TabIndex = 2
        Me.lblRemarksField.Text = "Admin Remarks"
        '
        'txtRemarks
        '
        Me.txtRemarks.Location = New System.Drawing.Point(20, 283)
        Me.txtRemarks.Multiline = True
        Me.txtRemarks.Name = "txtRemarks"
        Me.txtRemarks.ReadOnly = True
        Me.txtRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtRemarks.Size = New System.Drawing.Size(560, 80)
        Me.txtRemarks.TabIndex = 3
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(20, 380)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(270, 30)
        Me.btnRefresh.TabIndex = 4
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(310, 380)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(270, 30)
        Me.btnClose.TabIndex = 5
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmMyProposals
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(600, 430)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.dgvMyProposals)
        Me.Controls.Add(Me.lblRemarksField)
        Me.Controls.Add(Me.txtRemarks)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmMyProposals"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "My Proposals"
        CType(Me.dgvMyProposals, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblHeader As Label
    Friend WithEvents dgvMyProposals As DataGridView
    Friend WithEvents lblRemarksField As Label
    Friend WithEvents txtRemarks As TextBox
    Friend WithEvents btnRefresh As Button
    Friend WithEvents btnClose As Button
End Class