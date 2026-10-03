<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmManageProposals
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
        Me.dgvProposals = New System.Windows.Forms.DataGridView()
        Me.lblDetailsField = New System.Windows.Forms.Label()
        Me.txtDetails = New System.Windows.Forms.TextBox()
        Me.lblStatusField = New System.Windows.Forms.Label()
        Me.cboStatus = New System.Windows.Forms.ComboBox()
        Me.lblRemarksField = New System.Windows.Forms.Label()
        Me.txtRemarks = New System.Windows.Forms.TextBox()
        Me.btnSaveDecision = New System.Windows.Forms.Button()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.dgvProposals, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblHeader
        '
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblHeader.Location = New System.Drawing.Point(20, 15)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(140, 21)
        Me.lblHeader.TabIndex = 0
        Me.lblHeader.Text = "Manage Proposals"
        '
        'dgvProposals
        '
        Me.dgvProposals.AllowUserToAddRows = False
        Me.dgvProposals.AllowUserToDeleteRows = False
        Me.dgvProposals.AllowUserToResizeRows = False
        Me.dgvProposals.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvProposals.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvProposals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProposals.Location = New System.Drawing.Point(20, 50)
        Me.dgvProposals.MultiSelect = False
        Me.dgvProposals.Name = "dgvProposals"
        Me.dgvProposals.ReadOnly = True
        Me.dgvProposals.RowHeadersVisible = False
        Me.dgvProposals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvProposals.Size = New System.Drawing.Size(640, 180)
        Me.dgvProposals.TabIndex = 1
        '
        'lblDetailsField
        '
        Me.lblDetailsField.AutoSize = True
        Me.lblDetailsField.Location = New System.Drawing.Point(20, 242)
        Me.lblDetailsField.Name = "lblDetailsField"
        Me.lblDetailsField.Size = New System.Drawing.Size(84, 13)
        Me.lblDetailsField.TabIndex = 2
        Me.lblDetailsField.Text = "Proposal Details"
        '
        'txtDetails
        '
        Me.txtDetails.Location = New System.Drawing.Point(20, 260)
        Me.txtDetails.Multiline = True
        Me.txtDetails.Name = "txtDetails"
        Me.txtDetails.ReadOnly = True
        Me.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDetails.Size = New System.Drawing.Size(640, 90)
        Me.txtDetails.TabIndex = 3
        '
        'lblStatusField
        '
        Me.lblStatusField.AutoSize = True
        Me.lblStatusField.Location = New System.Drawing.Point(20, 362)
        Me.lblStatusField.Name = "lblStatusField"
        Me.lblStatusField.Size = New System.Drawing.Size(58, 13)
        Me.lblStatusField.TabIndex = 4
        Me.lblStatusField.Text = "Set Status"
        '
        'cboStatus
        '
        Me.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatus.FormattingEnabled = True
        Me.cboStatus.Location = New System.Drawing.Point(20, 380)
        Me.cboStatus.Name = "cboStatus"
        Me.cboStatus.Size = New System.Drawing.Size(170, 21)
        Me.cboStatus.TabIndex = 5
        '
        'lblRemarksField
        '
        Me.lblRemarksField.AutoSize = True
        Me.lblRemarksField.Location = New System.Drawing.Point(210, 362)
        Me.lblRemarksField.Name = "lblRemarksField"
        Me.lblRemarksField.Size = New System.Drawing.Size(49, 13)
        Me.lblRemarksField.TabIndex = 6
        Me.lblRemarksField.Text = "Remarks"
        '
        'txtRemarks
        '
        Me.txtRemarks.Location = New System.Drawing.Point(210, 380)
        Me.txtRemarks.Multiline = True
        Me.txtRemarks.Name = "txtRemarks"
        Me.txtRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtRemarks.Size = New System.Drawing.Size(450, 60)
        Me.txtRemarks.TabIndex = 7
        '
        'btnSaveDecision
        '
        Me.btnSaveDecision.Location = New System.Drawing.Point(20, 455)
        Me.btnSaveDecision.Name = "btnSaveDecision"
        Me.btnSaveDecision.Size = New System.Drawing.Size(640, 32)
        Me.btnSaveDecision.TabIndex = 8
        Me.btnSaveDecision.Text = "Save Decision"
        Me.btnSaveDecision.UseVisualStyleBackColor = True
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(20, 497)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(315, 30)
        Me.btnRefresh.TabIndex = 9
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(345, 497)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(315, 30)
        Me.btnClose.TabIndex = 10
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmManageProposals
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(680, 545)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.dgvProposals)
        Me.Controls.Add(Me.lblDetailsField)
        Me.Controls.Add(Me.txtDetails)
        Me.Controls.Add(Me.lblStatusField)
        Me.Controls.Add(Me.cboStatus)
        Me.Controls.Add(Me.lblRemarksField)
        Me.Controls.Add(Me.txtRemarks)
        Me.Controls.Add(Me.btnSaveDecision)
        Me.Controls.Add(Me.btnRefresh)
        Me.Controls.Add(Me.btnClose)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmManageProposals"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Manage Proposals"
        CType(Me.dgvProposals, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblHeader As Label
    Friend WithEvents dgvProposals As DataGridView
    Friend WithEvents lblDetailsField As Label
    Friend WithEvents txtDetails As TextBox
    Friend WithEvents lblStatusField As Label
    Friend WithEvents cboStatus As ComboBox
    Friend WithEvents lblRemarksField As Label
    Friend WithEvents txtRemarks As TextBox
    Friend WithEvents btnSaveDecision As Button
    Friend WithEvents btnRefresh As Button
    Friend WithEvents btnClose As Button
End Class