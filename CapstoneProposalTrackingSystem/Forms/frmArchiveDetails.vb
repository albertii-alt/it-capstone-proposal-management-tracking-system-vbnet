Option Strict On
Option Explicit On

Imports System.Collections.Generic

Public Class frmArchiveDetails
    Inherits Form

    Private ReadOnly _archivedTitle As ArchivedTitle

    Private ReadOnly pnlHeader As New Panel()
    Private WithEvents lblHeader As New Label()
    Private WithEvents txtTitle As New TextBox()
    Private WithEvents txtGroupName As New TextBox()
    Private WithEvents txtYearLevel As New TextBox()
    Private WithEvents txtSemester As New TextBox()
    Private WithEvents txtSubmittedOn As New TextBox()
    Private WithEvents txtApprovedOn As New TextBox()
    Private WithEvents txtDescription As New TextBox()
    Private WithEvents txtMembers As New TextBox()
    Private WithEvents btnDelete As New Button()
    Private WithEvents btnClose As New Button()

    Public Sub New(archivedTitle As ArchivedTitle)
        _archivedTitle = archivedTitle
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim footer As New FlowLayoutPanel()
        Dim proposalFields As New TableLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.ClientSize = New Size(620, 720)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmArchiveDetails"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Title Details"

        pnlHeader.Name = "pnlHeader"
        pnlHeader.Dock = DockStyle.Top
        pnlHeader.Height = 56
        pnlHeader.BackColor = Color.FromArgb(122, 31, 46)

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Title Details"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.Padding = New Padding(20, 0, 0, 0)
        lblHeader.Font = New Font("Segoe UI", 13.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.White
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        pnlHeader.Controls.Add(lblHeader)

        layout.Name = "tlpArchiveDetails"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20, 12, 20, 12)
        layout.ColumnCount = 1
        layout.RowCount = 9
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 92.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 52.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))

        ConfigureReadOnlyTextBox(txtTitle, "txtTitle")
        txtTitle.Multiline = True
        txtTitle.WordWrap = True
        txtTitle.ScrollBars = ScrollBars.Vertical

        ConfigureReadOnlyTextBox(txtGroupName, "txtGroupName")
        ConfigureReadOnlyTextBox(txtYearLevel, "txtYearLevel")
        ConfigureReadOnlyTextBox(txtSemester, "txtSemester")
        ConfigureReadOnlyTextBox(txtSubmittedOn, "txtSubmittedOn")
        ConfigureReadOnlyTextBox(txtApprovedOn, "txtApprovedOn")

        txtDescription.Name = "txtDescription"
        txtDescription.Dock = DockStyle.Fill
        txtDescription.Multiline = True
        txtDescription.WordWrap = True
        txtDescription.ReadOnly = True
        txtDescription.ScrollBars = ScrollBars.Vertical
        txtDescription.BackColor = SystemColors.Window

        proposalFields.Name = "tlpArchiveProposalFields"
        proposalFields.Dock = DockStyle.Fill
        proposalFields.ColumnCount = 4
        proposalFields.RowCount = 1
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        proposalFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        proposalFields.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        proposalFields.Controls.Add(CreateValueField("Year Level", txtYearLevel), 0, 0)
        proposalFields.Controls.Add(CreateValueField("Semester", txtSemester), 1, 0)
        proposalFields.Controls.Add(CreateValueField("Submitted", txtSubmittedOn), 2, 0)
        proposalFields.Controls.Add(CreateValueField("Approved On", txtApprovedOn), 3, 0)

        txtMembers.Name = "txtMembers"
        txtMembers.Dock = DockStyle.Fill
        txtMembers.Multiline = True
        txtMembers.ReadOnly = True
        txtMembers.ScrollBars = ScrollBars.Vertical
        txtMembers.BackColor = SystemColors.Window

        footer.Name = "flpArchiveDetailsFooter"
        footer.Dock = DockStyle.Fill
        footer.FlowDirection = FlowDirection.RightToLeft
        footer.WrapContents = False
        footer.Padding = New Padding(0, 8, 0, 0)

        btnClose.Name = "btnClose"
        btnClose.Text = "Close"
        btnClose.Size = New Size(100, 32)
        btnClose.DialogResult = DialogResult.Cancel
        btnClose.UseVisualStyleBackColor = True
        footer.Controls.Add(btnClose)

        btnDelete.Name = "btnDelete"
        btnDelete.Text = "Delete Archive Record"
        btnDelete.Size = New Size(160, 32)
        btnDelete.FlatStyle = FlatStyle.Flat
        btnDelete.FlatAppearance.BorderSize = 0
        btnDelete.BackColor = Color.FromArgb(166, 58, 58)
        btnDelete.ForeColor = Color.White
        btnDelete.UseVisualStyleBackColor = False
        footer.Controls.Add(btnDelete)

        layout.Controls.Add(CreateSectionLabel("Proposal Details"), 0, 0)
        layout.Controls.Add(CreateValueField("Title", txtTitle), 0, 1)
        layout.Controls.Add(CreateValueField("Group Name", txtGroupName), 0, 2)
        layout.Controls.Add(proposalFields, 0, 3)
        layout.Controls.Add(CreateSectionLabel("Description"), 0, 4)
        layout.Controls.Add(txtDescription, 0, 5)
        layout.Controls.Add(CreateSectionLabel("Group Members"), 0, 6)
        layout.Controls.Add(txtMembers, 0, 7)
        layout.Controls.Add(footer, 0, 8)

        Me.CancelButton = btnClose
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

    Private Function CreateValueField(captionText As String, valueControl As Control) As Panel
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

        valueControl.Dock = DockStyle.Fill
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

    Private Sub frmArchiveDetails_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtTitle.Text = _archivedTitle.Title
        txtGroupName.Text = _archivedTitle.GroupName
        txtYearLevel.Text = _archivedTitle.YearLevel
        txtSemester.Text = _archivedTitle.SemesterApproved
        txtSubmittedOn.Text = If(_archivedTitle.SubmittedAt = DateTime.MinValue, "", _archivedTitle.SubmittedAt.ToString("MM/dd/yyyy h:mm:ss tt"))
        txtApprovedOn.Text = If(_archivedTitle.ApprovedAt = DateTime.MinValue, "", _archivedTitle.ApprovedAt.ToString("MM/dd/yyyy"))

        Dim proposal As Proposal = Nothing
        If _archivedTitle.ProposalId > 0 Then
            Try
                proposal = ProposalService.GetProposalById(_archivedTitle.ProposalId)
            Catch ex As Exception
                MessageBox.Show("Could not load the archived proposal details: " & ex.Message)
                Me.Close()
                Return
            End Try
        End If

        If proposal Is Nothing Then
            txtDescription.Text = "The proposal was deleted. Only its archived title details are retained."
            txtMembers.Text = "Member details are unavailable after deletion."
            Return
        End If

        txtTitle.Text = proposal.Title
        txtGroupName.Text = proposal.GroupName
        txtYearLevel.Text = proposal.YearLevel
        txtSemester.Text = proposal.Semester
        txtSubmittedOn.Text = proposal.SubmittedAt.ToString("MM/dd/yyyy h:mm:ss tt")
        txtDescription.Text = proposal.Description

        Dim memberNames As New List(Of String)
        For Each member As ProposalMember In proposal.Members
            memberNames.Add(member.MemberName)
        Next

        txtMembers.Text = String.Join(vbCrLf, memberNames)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Using confirmation As New frmDeleteConfirmation("archived title", _archivedTitle.Title)
            If confirmation.ShowDialog(Me) <> DialogResult.OK Then
                Return
            End If
        End Using

        Try
            Dim deletedCount As Integer = ProposalService.DeleteArchivedTitles(New List(Of Integer) From {_archivedTitle.ArchiveId})
            If deletedCount = 0 Then
                MessageBox.Show("That archived title could not be found. It may have already been deleted.")
                Return
            End If

            MessageBox.Show("Archived title deleted successfully.")
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Could not delete the archived title: " & ex.Message)
        End Try
    End Sub
End Class