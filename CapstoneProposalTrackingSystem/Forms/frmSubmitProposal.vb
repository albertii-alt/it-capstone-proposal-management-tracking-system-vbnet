Public Class frmSubmitProposal
    Private Sub frmSubmitProposal_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cboYearLevel.Items.Clear()
        cboYearLevel.Items.AddRange({"1st Year", "2nd Year", "3rd Year", "4th Year"})
        cboYearLevel.SelectedIndex = 0

        cboSemester.Items.Clear()
        cboSemester.Items.AddRange({"1st Semester", "2nd Semester"})
        cboSemester.SelectedIndex = 0

        lblDuplicateStatus.Text = ""
    End Sub

    Private Sub btnAddMember_Click(sender As Object, e As EventArgs) Handles btnAddMember.Click
        Dim memberName As String = txtMemberName.Text.Trim()

        If String.IsNullOrEmpty(memberName) Then
            MessageBox.Show("Please enter a member name.")
            Return
        End If

        lstMembers.Items.Add(memberName)
        txtMemberName.Clear()
        txtMemberName.Focus()
    End Sub

    Private Sub btnRemoveMember_Click(sender As Object, e As EventArgs) Handles btnRemoveMember.Click
        If lstMembers.SelectedIndex = -1 Then
            MessageBox.Show("Select a member to remove first.")
            Return
        End If

        lstMembers.Items.RemoveAt(lstMembers.SelectedIndex)
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        lblDuplicateStatus.Text = ""

        Dim title As String = txtTitle.Text.Trim()
        Dim description As String = txtDescription.Text.Trim()
        Dim groupName As String = txtGroupName.Text.Trim()

        If String.IsNullOrEmpty(title) Then
            MessageBox.Show("Please enter a proposal title.")
            Return
        End If

        If String.IsNullOrEmpty(description) Then
            MessageBox.Show("Please enter a description.")
            Return
        End If

        If String.IsNullOrEmpty(groupName) Then
            MessageBox.Show("Please enter a group name.")
            Return
        End If

        If lstMembers.Items.Count = 0 Then
            MessageBox.Show("Please add at least one group member.")
            Return
        End If

        Dim duplicateResult As DuplicateCheckResult = ProposalService.CheckDuplicate(title)

        If duplicateResult.IsWarning Then
            Dim percent As String = (duplicateResult.HighestSimilarity * 100).ToString("0") & "%"
            lblDuplicateStatus.Text = "Warning: " & percent & " similar to """ & duplicateResult.MatchedTitle & """"

            Dim confirm As DialogResult = MessageBox.Show(
                "This title is " & percent & " similar to an existing title (""" & duplicateResult.MatchedTitle & """). Submit anyway?",
                "Possible Duplicate Title",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning)

            If confirm = DialogResult.No Then
                Return
            End If
        End If

        Dim proposal As New Proposal With {
            .Title = title,
            .GroupName = groupName,
            .Description = description,
            .Course = "BSIT",
            .YearLevel = cboYearLevel.Text,
            .Semester = cboSemester.Text,
            .SubmittedBy = Session.CurrentUser.UserId
        }

        For Each memberName As String In lstMembers.Items
            proposal.Members.Add(New ProposalMember With {.MemberName = memberName})
        Next

        Try
            ProposalService.SubmitProposal(proposal, Session.CurrentUser)
            MessageBox.Show("Proposal submitted successfully! Status: Pending.")
            Me.Close()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class