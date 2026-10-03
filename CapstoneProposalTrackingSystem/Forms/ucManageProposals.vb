Option Strict On
Option Explicit On

Imports System.Collections.Generic

Public Class ucManageProposals
    Inherits UserControl

    Private _proposals As New List(Of Proposal)
    Private _isLoading As Boolean = False

    Private WithEvents lblHeader As New Label()
    Private WithEvents dgvProposals As New DataGridView()
    Private WithEvents lblDetailsField As New Label()
    Private WithEvents txtDetails As New TextBox()
    Private WithEvents lblStatusField As New Label()
    Private WithEvents cboStatus As New ComboBox()
    Private WithEvents lblRemarksField As New Label()
    Private WithEvents txtRemarks As New TextBox()
    Private WithEvents btnSaveDecision As New Button()
    Private WithEvents btnRefresh As New Button()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim layout As New TableLayoutPanel()
        Dim decisionLayout As New TableLayoutPanel()
        Dim actionLayout As New FlowLayoutPanel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.Name = "ucManageProposals"

        layout.Name = "tlpManageProposals"
        layout.Dock = DockStyle.Fill
        layout.Padding = New Padding(20)
        layout.ColumnCount = 1
        layout.RowCount = 6
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 32.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 45.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 30.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 25.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))

        lblHeader.Name = "lblHeader"
        lblHeader.Text = "Manage Proposals"
        lblHeader.Dock = DockStyle.Fill
        lblHeader.TextAlign = ContentAlignment.MiddleLeft
        lblHeader.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold)

        dgvProposals.Name = "dgvProposals"
        dgvProposals.Dock = DockStyle.Fill
        dgvProposals.AllowUserToAddRows = False
        dgvProposals.AllowUserToDeleteRows = False
        dgvProposals.AllowUserToResizeRows = False
        dgvProposals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvProposals.BackgroundColor = SystemColors.Window
        dgvProposals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvProposals.MultiSelect = False
        dgvProposals.ReadOnly = True
        dgvProposals.RowHeadersVisible = False
        dgvProposals.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        lblDetailsField.Name = "lblDetailsField"
        lblDetailsField.Text = "Proposal Details"
        lblDetailsField.Dock = DockStyle.Fill
        lblDetailsField.TextAlign = ContentAlignment.MiddleLeft

        txtDetails.Name = "txtDetails"
        txtDetails.Dock = DockStyle.Fill
        txtDetails.Multiline = True
        txtDetails.ReadOnly = True
        txtDetails.ScrollBars = ScrollBars.Vertical

        decisionLayout.Name = "tlpDecisionDetails"
        decisionLayout.Dock = DockStyle.Fill
        decisionLayout.ColumnCount = 2
        decisionLayout.RowCount = 2
        decisionLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30.0F))
        decisionLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 70.0F))
        decisionLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        decisionLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        lblStatusField.Name = "lblStatusField"
        lblStatusField.Text = "Set Status"
        lblStatusField.Dock = DockStyle.Fill
        lblStatusField.TextAlign = ContentAlignment.MiddleLeft

        cboStatus.Name = "cboStatus"
        cboStatus.Dock = DockStyle.Top
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList
        cboStatus.FormattingEnabled = True

        lblRemarksField.Name = "lblRemarksField"
        lblRemarksField.Text = "Remarks"
        lblRemarksField.Dock = DockStyle.Fill
        lblRemarksField.TextAlign = ContentAlignment.MiddleLeft

        txtRemarks.Name = "txtRemarks"
        txtRemarks.Dock = DockStyle.Fill
        txtRemarks.Multiline = True
        txtRemarks.ScrollBars = ScrollBars.Vertical

        decisionLayout.Controls.Add(lblStatusField, 0, 0)
        decisionLayout.Controls.Add(lblRemarksField, 1, 0)
        decisionLayout.Controls.Add(cboStatus, 0, 1)
        decisionLayout.Controls.Add(txtRemarks, 1, 1)

        actionLayout.Name = "flpProposalActions"
        actionLayout.Dock = DockStyle.Fill
        actionLayout.FlowDirection = FlowDirection.LeftToRight
        actionLayout.WrapContents = False

        btnSaveDecision.Name = "btnSaveDecision"
        btnSaveDecision.Text = "Save Decision"
        btnSaveDecision.Size = New Size(150, 32)
        btnSaveDecision.UseVisualStyleBackColor = True

        btnRefresh.Name = "btnRefresh"
        btnRefresh.Text = "Refresh"
        btnRefresh.Size = New Size(110, 32)
        btnRefresh.UseVisualStyleBackColor = True

        actionLayout.Controls.Add(btnSaveDecision)
        actionLayout.Controls.Add(btnRefresh)

        layout.Controls.Add(lblHeader, 0, 0)
        layout.Controls.Add(dgvProposals, 0, 1)
        layout.Controls.Add(lblDetailsField, 0, 2)
        layout.Controls.Add(txtDetails, 0, 3)
        layout.Controls.Add(decisionLayout, 0, 4)
        layout.Controls.Add(actionLayout, 0, 5)

        Me.Controls.Add(layout)
        Me.ResumeLayout(False)
    End Sub

    Private Sub ucManageProposals_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cboStatus.Items.AddRange({"Pending", "Approved", "For Revision", "Rejected"})

        SetupGridColumns()
        LoadProposals(0)
    End Sub

    Private Sub SetupGridColumns()
        dgvProposals.AutoGenerateColumns = False
        dgvProposals.Columns.Clear()
        dgvProposals.Columns.Add("colTitle", "Title")
        dgvProposals.Columns.Add("colYearLevel", "Year Level")
        dgvProposals.Columns.Add("colSemester", "Semester")
        dgvProposals.Columns.Add("colStatus", "Status")
        dgvProposals.Columns.Add("colSubmitted", "Submitted")
    End Sub

    Private Sub LoadProposals(selectProposalId As Integer)
        _isLoading = True

        Try
            dgvProposals.Rows.Clear()
            _proposals = ProposalService.GetAllProposals()

            For Each proposal As Proposal In _proposals
                Dim rowIndex As Integer = dgvProposals.Rows.Add(
                    proposal.Title,
                    proposal.YearLevel,
                    proposal.Semester,
                    FormatStatus(proposal.Status),
                    proposal.SubmittedAt.ToString("MM/dd/yyyy"))

                dgvProposals.Rows(rowIndex).Tag = proposal.ProposalId
            Next

        Catch ex As Exception
            MessageBox.Show("Could not load the proposals: " & ex.Message)
        Finally
            _isLoading = False
        End Try

        If dgvProposals.Rows.Count = 0 Then
            txtDetails.Text = "There are no proposals yet."
            txtRemarks.Clear()
            cboStatus.SelectedIndex = -1
            Return
        End If

        Dim rowToSelect As Integer = 0

        For i As Integer = 0 To dgvProposals.Rows.Count - 1
            If CInt(dgvProposals.Rows(i).Tag) = selectProposalId Then
                rowToSelect = i
                Exit For
            End If
        Next

        dgvProposals.ClearSelection()
        dgvProposals.Rows(rowToSelect).Selected = True
        ShowSelectedProposal()
    End Sub

    Private Sub dgvProposals_SelectionChanged(sender As Object, e As EventArgs) Handles dgvProposals.SelectionChanged
        If _isLoading Then
            Return
        End If

        ShowSelectedProposal()
    End Sub

    Private Function GetSelectedProposal() As Proposal
        If dgvProposals.SelectedRows.Count = 0 Then
            Return Nothing
        End If

        Dim proposalId As Integer = CInt(dgvProposals.SelectedRows(0).Tag)

        For Each proposal As Proposal In _proposals
            If proposal.ProposalId = proposalId Then
                Return proposal
            End If
        Next

        Return Nothing
    End Function

    Private Sub ShowSelectedProposal()
        Dim proposal As Proposal = GetSelectedProposal()

        If proposal Is Nothing Then
            Return
        End If

        Dim memberNames As New List(Of String)
        For Each member As ProposalMember In proposal.Members
            memberNames.Add(member.MemberName)
        Next

        Dim detailsText As String = proposal.Description & vbCrLf & vbCrLf &
                                    "Group members: " & String.Join(", ", memberNames)

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
                detailsText &= vbCrLf & vbCrLf & "Duplicate check: no similar titles found."
            Else
                detailsText &= vbCrLf & vbCrLf & "DUPLICATE CHECK (closest matches):"

                For Each item As TitleSimilarity In matches
                    Dim percent As String = (item.Score * 100).ToString("0") & "%"
                    detailsText &= vbCrLf & " - " & percent & " similar to """ & item.Title & """"
                Next
            End If

        Catch ex As Exception
            detailsText &= vbCrLf & vbCrLf & "Duplicate check unavailable."
        End Try

        txtDetails.Text = detailsText

        cboStatus.Text = FormatStatus(proposal.Status)
        txtRemarks.Clear()
    End Sub

    Private Sub btnSaveDecision_Click(sender As Object, e As EventArgs) Handles btnSaveDecision.Click
        Dim proposal As Proposal = GetSelectedProposal()

        If proposal Is Nothing Then
            MessageBox.Show("Select a proposal first.")
            Return
        End If

        If cboStatus.SelectedIndex = -1 Then
            MessageBox.Show("Choose a status first.")
            Return
        End If

        Dim newStatus As ProposalStatus = ToStatus(cboStatus.Text)

        Dim confirm As DialogResult = MessageBox.Show(
            "Set """ & proposal.Title & """ to " & cboStatus.Text & "?",
            "Confirm Decision",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If confirm = DialogResult.No Then
            Return
        End If

        Try
            ProposalService.ReviewProposal(proposal.ProposalId, newStatus, Session.CurrentUser.UserId, txtRemarks.Text)
            MessageBox.Show("Decision saved.")
            LoadProposals(proposal.ProposalId)

        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
        Catch ex As Exception
            MessageBox.Show("Something went wrong while saving. Please try again.")
        End Try
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

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Dim current As Proposal = GetSelectedProposal()
        LoadProposals(If(current IsNot Nothing, current.ProposalId, 0))
    End Sub

    Public Sub RefreshData()
        btnRefresh_Click(btnRefresh, EventArgs.Empty)
    End Sub

End Class