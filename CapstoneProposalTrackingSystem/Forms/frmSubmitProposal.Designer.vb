<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSubmitProposal
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
        Me.lblGroupNameField = New System.Windows.Forms.Label()
        Me.txtGroupName = New System.Windows.Forms.TextBox()
        Me.lblTitleField = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.lblDescriptionField = New System.Windows.Forms.Label()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.lblYearLevelField = New System.Windows.Forms.Label()
        Me.cboYearLevel = New System.Windows.Forms.ComboBox()
        Me.lblSemesterField = New System.Windows.Forms.Label()
        Me.cboSemester = New System.Windows.Forms.ComboBox()
        Me.lblMembersField = New System.Windows.Forms.Label()
        Me.txtMemberName = New System.Windows.Forms.TextBox()
        Me.btnAddMember = New System.Windows.Forms.Button()
        Me.lstMembers = New System.Windows.Forms.ListBox()
        Me.btnRemoveMember = New System.Windows.Forms.Button()
        Me.lblDuplicateStatus = New System.Windows.Forms.Label()
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblHeader
        '
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblHeader.Location = New System.Drawing.Point(25, 15)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(140, 21)
        Me.lblHeader.TabIndex = 0
        Me.lblHeader.Text = "Submit Proposal"
        '
        'lblGroupNameField
        '
        Me.lblGroupNameField.AutoSize = True
        Me.lblGroupNameField.Location = New System.Drawing.Point(25, 55)
        Me.lblGroupNameField.Name = "lblGroupNameField"
        Me.lblGroupNameField.Size = New System.Drawing.Size(70, 13)
        Me.lblGroupNameField.TabIndex = 1
        Me.lblGroupNameField.Text = "Group Name"
        '
        'txtGroupName
        '
        Me.txtGroupName.Location = New System.Drawing.Point(25, 73)
        Me.txtGroupName.Name = "txtGroupName"
        Me.txtGroupName.Size = New System.Drawing.Size(370, 20)
        Me.txtGroupName.TabIndex = 2
        '
        'lblTitleField
        '
        Me.lblTitleField.AutoSize = True
        Me.lblTitleField.Location = New System.Drawing.Point(25, 95)
        Me.lblTitleField.Name = "lblTitleField"
        Me.lblTitleField.Size = New System.Drawing.Size(75, 13)
        Me.lblTitleField.TabIndex = 3
        Me.lblTitleField.Text = "Proposal Title"
        '
        'txtTitle
        '
        Me.txtTitle.Location = New System.Drawing.Point(25, 113)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(370, 20)
        Me.txtTitle.TabIndex = 4
        '
        'lblDescriptionField
        '
        Me.lblDescriptionField.AutoSize = True
        Me.lblDescriptionField.Location = New System.Drawing.Point(25, 145)
        Me.lblDescriptionField.Name = "lblDescriptionField"
        Me.lblDescriptionField.Size = New System.Drawing.Size(63, 13)
        Me.lblDescriptionField.TabIndex = 5
        Me.lblDescriptionField.Text = "Description"
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(25, 163)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDescription.Size = New System.Drawing.Size(370, 60)
        Me.txtDescription.TabIndex = 6
        '
        'lblYearLevelField
        '
        Me.lblYearLevelField.AutoSize = True
        Me.lblYearLevelField.Location = New System.Drawing.Point(25, 236)
        Me.lblYearLevelField.Name = "lblYearLevelField"
        Me.lblYearLevelField.Size = New System.Drawing.Size(58, 13)
        Me.lblYearLevelField.TabIndex = 7
        Me.lblYearLevelField.Text = "Year Level"
        '
        'cboYearLevel
        '
        Me.cboYearLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboYearLevel.FormattingEnabled = True
        Me.cboYearLevel.Location = New System.Drawing.Point(25, 254)
        Me.cboYearLevel.Name = "cboYearLevel"
        Me.cboYearLevel.Size = New System.Drawing.Size(175, 21)
        Me.cboYearLevel.TabIndex = 8
        '
        'lblSemesterField
        '
        Me.lblSemesterField.AutoSize = True
        Me.lblSemesterField.Location = New System.Drawing.Point(220, 236)
        Me.lblSemesterField.Name = "lblSemesterField"
        Me.lblSemesterField.Size = New System.Drawing.Size(52, 13)
        Me.lblSemesterField.TabIndex = 9
        Me.lblSemesterField.Text = "Semester"
        '
        'cboSemester
        '
        Me.cboSemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSemester.FormattingEnabled = True
        Me.cboSemester.Location = New System.Drawing.Point(220, 254)
        Me.cboSemester.Name = "cboSemester"
        Me.cboSemester.Size = New System.Drawing.Size(175, 21)
        Me.cboSemester.TabIndex = 10
        '
        'lblMembersField
        '
        Me.lblMembersField.AutoSize = True
        Me.lblMembersField.Location = New System.Drawing.Point(25, 290)
        Me.lblMembersField.Name = "lblMembersField"
        Me.lblMembersField.Size = New System.Drawing.Size(80, 13)
        Me.lblMembersField.TabIndex = 11
        Me.lblMembersField.Text = "Group Members"
        '
        'txtMemberName
        '
        Me.txtMemberName.Location = New System.Drawing.Point(25, 308)
        Me.txtMemberName.Name = "txtMemberName"
        Me.txtMemberName.Size = New System.Drawing.Size(280, 20)
        Me.txtMemberName.TabIndex = 12
        '
        'btnAddMember
        '
        Me.btnAddMember.Location = New System.Drawing.Point(315, 306)
        Me.btnAddMember.Name = "btnAddMember"
        Me.btnAddMember.Size = New System.Drawing.Size(80, 23)
        Me.btnAddMember.TabIndex = 13
        Me.btnAddMember.Text = "Add"
        Me.btnAddMember.UseVisualStyleBackColor = True
        '
        'lstMembers
        '
        Me.lstMembers.FormattingEnabled = True
        Me.lstMembers.Location = New System.Drawing.Point(25, 336)
        Me.lstMembers.Name = "lstMembers"
        Me.lstMembers.Size = New System.Drawing.Size(370, 69)
        Me.lstMembers.TabIndex = 14
        '
        'btnRemoveMember
        '
        Me.btnRemoveMember.Location = New System.Drawing.Point(290, 411)
        Me.btnRemoveMember.Name = "btnRemoveMember"
        Me.btnRemoveMember.Size = New System.Drawing.Size(105, 23)
        Me.btnRemoveMember.TabIndex = 15
        Me.btnRemoveMember.Text = "Remove Selected"
        Me.btnRemoveMember.UseVisualStyleBackColor = True
        '
        'lblDuplicateStatus
        '
        Me.lblDuplicateStatus.AutoSize = True
        Me.lblDuplicateStatus.ForeColor = System.Drawing.Color.DarkOrange
        Me.lblDuplicateStatus.Location = New System.Drawing.Point(25, 445)
        Me.lblDuplicateStatus.MaximumSize = New System.Drawing.Size(370, 0)
        Me.lblDuplicateStatus.Name = "lblDuplicateStatus"
        Me.lblDuplicateStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblDuplicateStatus.TabIndex = 16
        '
        'btnSubmit
        '
        Me.btnSubmit.Location = New System.Drawing.Point(25, 475)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(370, 30)
        Me.btnSubmit.TabIndex = 17
        Me.btnSubmit.Text = "Submit Proposal"
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'frmSubmitProposal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(420, 525)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.lblGroupNameField)
        Me.Controls.Add(Me.txtGroupName)
        Me.Controls.Add(Me.lblTitleField)
        Me.Controls.Add(Me.txtTitle)
        Me.Controls.Add(Me.lblDescriptionField)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.lblYearLevelField)
        Me.Controls.Add(Me.cboYearLevel)
        Me.Controls.Add(Me.lblSemesterField)
        Me.Controls.Add(Me.cboSemester)
        Me.Controls.Add(Me.lblMembersField)
        Me.Controls.Add(Me.txtMemberName)
        Me.Controls.Add(Me.btnAddMember)
        Me.Controls.Add(Me.lstMembers)
        Me.Controls.Add(Me.btnRemoveMember)
        Me.Controls.Add(Me.lblDuplicateStatus)
        Me.Controls.Add(Me.btnSubmit)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmSubmitProposal"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Submit Proposal"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblHeader As Label
    Friend WithEvents lblGroupNameField As Label
    Friend WithEvents txtGroupName As TextBox
    Friend WithEvents lblTitleField As Label
    Friend WithEvents txtTitle As TextBox
    Friend WithEvents lblDescriptionField As Label
    Friend WithEvents txtDescription As TextBox
    Friend WithEvents lblYearLevelField As Label
    Friend WithEvents cboYearLevel As ComboBox
    Friend WithEvents lblSemesterField As Label
    Friend WithEvents cboSemester As ComboBox
    Friend WithEvents lblMembersField As Label
    Friend WithEvents txtMemberName As TextBox
    Friend WithEvents btnAddMember As Button
    Friend WithEvents lstMembers As ListBox
    Friend WithEvents btnRemoveMember As Button
    Friend WithEvents lblDuplicateStatus As Label
    Friend WithEvents btnSubmit As Button
End Class