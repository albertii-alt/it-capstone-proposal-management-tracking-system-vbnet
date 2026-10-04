Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.Drawing.Drawing2D

Public Class frmProposalReview
    Inherits Form

    Private ReadOnly _proposalId As Integer
    Private _proposal As Proposal

    Private ReadOnly pnlHeader As New Panel()
    Private WithEvents lblHeader As New Label()
    Private WithEvents txtProposalTitle As New TextBox()
    Private WithEvents txtYearLevel As New TextBox()
    Private WithEvents txtSemester As New TextBox()
    Private WithEvents lblStatusBadge As New ReviewStatusBadgeLabel()
    Private WithEvents txtGroupName As New TextBox()
    Private WithEvents txtDescription As New TextBox()
    Private WithEvents txtMembers As New TextBox()
    Private WithEvents txtDuplicateCheck As New TextBox()
    Private WithEvents cboStatus As New ComboBox()
    Private WithEvents txtRemarks As New TextBox()
    Private WithEvents lblRemarksCounter As New Label()
    Private WithEvents btnSaveDecision As New Button()
    Private WithEvents btnCancel As New Button()

    Public Sub New(proposalId As Integer)
        _proposalId = proposalId
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()
        Dim proposalFields As New TableLayoutPanel()
        Dim groupNameField As Panel

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(620, 780)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmProposalReview"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Review Proposal"

        pnlHeader.Name = "pnlHeader"
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 56
        pnlHeader.BackColor = Color.FromArgb(122, 31, 46)

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Review Proposal"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Padding = New Padding(20, 0, 0, 0)
        lblHeader.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblHeader)

        layout.Name = "tlpProposalReview"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20, 12, 20, 12)
        layout.ColumnCount = 1
        layout.RowCount = 16
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 82.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 46.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 46.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 32.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 18.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 22.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 16.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 18.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        proposalFields.Name = "tlpProposalFields"
        proposalFields.Dock = DockStyle.Fill
        proposalFields.ColumnCount = 3
        proposalFields.RowCount = 1
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 38.0F))
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 31.0F))
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 31.0F))
        proposalFields.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        ConfigureReadOnlyTextBox(txtProposalTitle, "txtProposalTitle")
        txtProposalTitle.Multiline = True
        txtProposalTitle.WordWrap = True
        txtProposalTitle.ScrollBars = ScrollBars.Vertical
        ConfigureReadOnlyTextBox(txtYearLevel, "txtYearLevel")
        ConfigureReadOnlyTextBox(txtSemester, "txtSemester")

        lblStatusBadge.Name = "lblStatusBadge"
        lblStatusBadge.Dock = DockStyle.Left
        lblStatusBadge.Width = 110
        lblStatusBadge.Height = 30
        lblStatusBadge.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold)
        lblStatusBadge.TextAlign = ContentAlignment.MiddleCenter
        lblStatusBadge.Padding = New Padding(8, 0, 8, 0)

        proposalFields.Controls.Add(CreateValueField("Year Level", txtYearLevel), 0, 0)
        proposalFields.Controls.Add(CreateValueField("Semester", txtSemester), 1, 0)
        proposalFields.Controls.Add(CreateValueField("Status", lblStatusBadge, False), 2, 0)

        ConfigureReadOnlyTextBox(txtGroupName, "txtGroupName")
        groupNameField = CreateValueField("Group Name", txtGroupName)

        txtDescription.Name = "txtDescription"
        txtDescription.Dock = DockStyle.Fill
        txtDescription.Multiline = True
        txtDescription.WordWrap = True
        txtDescription.ReadOnly = True
        txtDescription.ScrollBars = ScrollBars.Vertical
        txtDescription.BackColor = SystemColors.Window

        txtMembers.Name = "txtMembers"
        txtMembers.Dock = DockStyle.Fill
        txtMembers.Multiline = True
        txtMembers.ReadOnly = True
        txtMembers.ScrollBars = ScrollBars.Vertical
        txtMembers.BackColor = SystemColors.Window

        txtDuplicateCheck.Name = "txtDuplicateCheck"
        txtDuplicateCheck.Dock = DockStyle.Fill
        txtDuplicateCheck.Multiline = True
        txtDuplicateCheck.ReadOnly = True
        txtDuplicateCheck.ScrollBars = ScrollBars.Vertical
        txtDuplicateCheck.BackColor = SystemColors.Window

        cboStatus.Name = "cboStatus"
        cboStatus.Dock = DockStyle.Fill
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList
        cboStatus.Items.AddRange({"Pending", "Approved", "For Revision", "Rejected"})

        txtRemarks.Name = "txtRemarks"
        txtRemarks.Dock = DockStyle.Fill
        txtRemarks.Multiline = True
        txtRemarks.ScrollBars = ScrollBars.Vertical
        txtRemarks.MaxLength = 500

        lblRemarksCounter.Name = "lblRemarksCounter"
        lblRemarksCounter.Dock = DockStyle.Fill
        lblRemarksCounter.Text = "0/500"
        lblRemarksCounter.ForeColor = Color.DimGray
        lblRemarksCounter.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
        lblRemarksCounter.TextAlign = ContentAlignment.MiddleRight

        footer.Name = "flpProposalReviewActions"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 0, 0)

        btnSaveDecision.Name = "btnSaveDecision"
        btnSaveDecision.Text = "Save Decision"
        btnSaveDecision.Size = New Size(140, 32)
        btnSaveDecision.FlatStyle = FlatStyle.Flat
        btnSaveDecision.FlatAppearance.BorderSize = 0
        btnSaveDecision.BackColor = Color.FromArgb(246, 201, 69)
        btnSaveDecision.ForeColor = Color.FromArgb(122, 31, 46)
        btnSaveDecision.UseVisualStyleBackColor = False

        btnCancel.Name = "btnCancel"
        btnCancel.Text = "Cancel"
        btnCancel.Size = New Size(100, 32)
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.UseVisualStyleBackColor = True

        footer.Controls.Add(btnSaveDecision)
        footer.Controls.Add(btnCancel)

        layout.Controls.Add(CreateSectionLabel("Proposal Details"), 0, 0)
        layout.Controls.Add(CreateValueField("Title", txtProposalTitle), 0, 1)
        layout.Controls.Add(proposalFields, 0, 2)
        layout.Controls.Add(groupNameField, 0, 3)
        layout.Controls.Add(CreateSectionLabel("Description"), 0, 4)
        layout.Controls.Add(txtDescription, 0, 5)
        layout.Controls.Add(CreateSectionLabel("Group Members"), 0, 6)
        layout.Controls.Add(txtMembers, 0, 7)
        layout.Controls.Add(CreateSectionLabel("Duplicate Check"), 0, 8)
        layout.Controls.Add(txtDuplicateCheck, 0, 9)
        layout.Controls.Add(CreateSectionLabel("Remarks"), 0, 10)
        layout.Controls.Add(txtRemarks, 0, 11)
        layout.Controls.Add(lblRemarksCounter, 0, 12)
        layout.Controls.Add(CreateSectionLabel("Update Status"), 0, 13)
        layout.Controls.Add(cboStatus, 0, 14)
        layout.Controls.Add(footer, 0, 15)

        Me.AcceptButton = btnSaveDecision
        Me.CancelButton = btnCancel
        Me.Controls.Add(layout)
        Me.Controls.Add(pnlHeader)
        Me.ResumeLayout(False)
    End Sub

    Private Function CreateSectionLabel(text As String) As Label
        Dim sectionLabel As New Label()
        sectionLabel.AutoSize = False
        sectionLabel.Dock = DockStyle.Fill
        sectionLabel.Text = text
        sectionLabel.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        sectionLabel.ForeColor = Color.FromArgb(122, 31, 46)
        sectionLabel.TextAlign = ContentAlignment.MiddleLeft
        Return sectionLabel
    End Function

    Private Function CreateValueField(captionText As String, valueControl As Control, Optional stretchValue As Boolean = True) As Panel
        Dim fieldPanel As New Panel()
        Dim caption As New Label()

        fieldPanel.Dock = DockStyle.Fill
        fieldPanel.Padding = New Padding(0, 0, 8, 0)

        caption.AutoSize = False
        caption.Dock = DockStyle.Top
        caption.Height = 18
        caption.Text = captionText
        caption.Font = New Font("Segoe UI", 8.0F, FontStyle.Regular)
        caption.ForeColor = Color.DimGray
        caption.TextAlign = ContentAlignment.MiddleLeft

        If stretchValue Then
            valueControl.Dock = DockStyle.Fill
        Else
            valueControl.Dock = DockStyle.Left
        End If

        fieldPanel.Controls.Add(valueControl)
        fieldPanel.Controls.Add(caption)
        Return fieldPanel
    End Function

    Private Sub ConfigureReadOnlyTextBox(textBox As TextBox, controlName As String)
        textBox.Name = controlName
        textBox.Dock = DockStyle.Fill
        textBox.ReadOnly = True
        textBox.BackColor = SystemColors.Window
        textBox.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Sub SetStatusBadge(status As ProposalStatus)
        lblStatusBadge.Text = FormatStatus(status)

        Select Case status
            Case ProposalStatus.Pending
                lblStatusBadge.BackColor = Color.FromArgb(246, 201, 69)
                lblStatusBadge.ForeColor = Color.FromArgb(45, 45, 45)
            Case ProposalStatus.Approved
                lblStatusBadge.BackColor = Color.FromArgb(76, 154, 106)
                lblStatusBadge.ForeColor = Color.White
            Case ProposalStatus.ForRevision
                lblStatusBadge.BackColor = Color.FromArgb(201, 138, 43)
                lblStatusBadge.ForeColor = Color.White
            Case ProposalStatus.Rejected
                lblStatusBadge.BackColor = Color.FromArgb(166, 58, 58)
                lblStatusBadge.ForeColor = Color.White
        End Select
    End Sub

    Private Sub frmProposalReview_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim proposal As Proposal = Nothing

        Try
            For Each item As Proposal In ProposalService.GetAllProposals()
                If item.ProposalId = _proposalId Then
                    proposal = item
                    Exit For
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Could not load the proposal: " & ex.Message)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            Return
        End Try

        If proposal Is Nothing Then
            MessageBox.Show("The proposal could not be found.")
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            Return
        End If

        _proposal = proposal
        txtProposalTitle.Text = proposal.Title
        txtDescription.Text = proposal.Description
        txtYearLevel.Text = proposal.YearLevel
        txtSemester.Text = proposal.Semester
        txtGroupName.Text = proposal.GroupName
        SetStatusBadge(proposal.Status)

        Dim memberNames As New List(Of String)
        For Each member As ProposalMember In proposal.Members
            memberNames.Add(member.MemberName)
        Next

        txtMembers.Text = String.Join(vbCrLf, memberNames)
        Dim duplicateCheckText As String = String.Empty

        Try
            Dim similarities As List(Of TitleSimilarity) = ProposalService.GetSimilarityBreakdown(proposal.ProposalId, proposal.Title)
            Dim matches As New List(Of TitleSimilarity)

            For Each item As TitleSimilarity In similarities
                If item.Score > 0 Then
                    matches.Add(item)
                End If

                If matches.Count = 5 Then
                    Exit For
                End If
            Next

            If matches.Count = 0 Then
                duplicateCheckText = "No similar titles found."
            Else
                duplicateCheckText = "DUPLICATE CHECK (closest matches):"

                For Each item As TitleSimilarity In matches
                    Dim percent As String = (item.Score * 100).ToString("0") & "%"
                    duplicateCheckText &= vbCrLf & percent & " similar to """ & item.Title & """ by " & item.GroupName
                Next
            End If

        Catch ex As Exception
            duplicateCheckText = "Duplicate check unavailable."
        End Try

        txtDuplicateCheck.Text = duplicateCheckText
        cboStatus.Text = FormatStatus(proposal.Status)
    End Sub

    Private Sub btnSaveDecision_Click(sender As Object, e As EventArgs) Handles btnSaveDecision.Click
        If _proposal Is Nothing Then
            MessageBox.Show("Select a proposal first.")
            Return
        End If

        If cboStatus.SelectedIndex = -1 Then
            MessageBox.Show("Choose a status first.")
            Return
        End If

        Dim newStatus As ProposalStatus = ToStatus(cboStatus.Text)

        Dim confirm As DialogResult = MessageBox.Show(
            "Set """ & _proposal.Title & """ to " & cboStatus.Text & "?",
            "Confirm Decision",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If confirm = DialogResult.No Then
            Return
        End If

        Try
            ProposalService.ReviewProposal(_proposalId, newStatus, Session.CurrentUser.UserId, txtRemarks.Text)
            MessageBox.Show("Decision saved.")
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
        Catch ex As Exception
            MessageBox.Show("Something went wrong while saving. Please try again.")
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub txtRemarks_TextChanged(sender As Object, e As EventArgs) Handles txtRemarks.TextChanged
        lblRemarksCounter.Text = txtRemarks.Text.Length.ToString() & "/500"
    End Sub

    Private Function FormatStatus(status As ProposalStatus) As String
        If status = ProposalStatus.ForRevision Then
            Return "For Revision"
        End If

        Return status.ToString()
    End Function

    Private Function ToStatus(text As String) As ProposalStatus
        If text = "For Revision" Then
            Return ProposalStatus.ForRevision
        End If

        Return CType([Enum].Parse(GetType(ProposalStatus), text), ProposalStatus)
    End Function
End Class

Friend Class ReviewStatusBadgeLabel
    Inherits Label

    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)

        If Me.ClientSize.Width <= 0 OrElse Me.ClientSize.Height <= 0 Then
            Return
        End If

        Dim bounds As Rectangle = New Rectangle(0, 0, Me.ClientSize.Width, Me.ClientSize.Height)
        Dim diameter As Integer = Math.Min(bounds.Height, bounds.Width)

        Using path As New GraphicsPath()
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180.0F, 90.0F)
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270.0F, 90.0F)
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0.0F, 90.0F)
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90.0F, 90.0F)
            path.CloseFigure()

            Dim previousRegion As Region = Me.Region
            Me.Region = New Region(path)

            If previousRegion IsNot Nothing Then
                previousRegion.Dispose()
            End If
        End Using
    End Sub
End Class