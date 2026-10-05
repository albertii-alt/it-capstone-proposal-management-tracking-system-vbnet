Imports System.Collections.Generic

Public Class ucStudentRegistrations
    Inherits UserControl

    Private ReadOnly dgvRegistrations As New DataGridView()
    Private _registrations As New List(Of User)()

    Public Sub New()
        InitializeLayout()
        AddHandler Me.Load, AddressOf ucStudentRegistrations_Load
    End Sub

    Private Sub InitializeLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Padding = New Padding(20)

        Dim layout As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 1, .RowCount = 3}
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 82.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        layout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim header As New Panel With {.Dock = DockStyle.Fill}
        Dim title As New Label With {.Text = "Student Registrations", .Font = New Font("Segoe UI", 23.0F, FontStyle.Bold), .ForeColor = Color.FromArgb(45, 45, 45), .Dock = DockStyle.Top, .Height = 48}
        Dim subtitle As New Label With {.Text = "Review student accounts before granting access to the system.", .Font = New Font("Segoe UI", 9.0F), .ForeColor = Color.DimGray, .Dock = DockStyle.Bottom, .Height = 26}
        header.Controls.Add(subtitle)
        header.Controls.Add(title)

        Dim toolbar As New Panel With {.Dock = DockStyle.Fill}
        Dim refreshButton As New Button With {.Text = "Refresh", .Dock = DockStyle.Right, .Width = 90}
        AddHandler refreshButton.Click, Sub(sender, e) LoadRegistrations()
        toolbar.Controls.Add(refreshButton)

        ConfigureGrid()
        layout.Controls.Add(header, 0, 0)
        layout.Controls.Add(toolbar, 0, 1)
        layout.Controls.Add(dgvRegistrations, 0, 2)
        Me.Controls.Add(layout)
    End Sub

    Private Sub ConfigureGrid()
        dgvRegistrations.Dock = DockStyle.Fill
        dgvRegistrations.AllowUserToAddRows = False
        dgvRegistrations.AllowUserToDeleteRows = False
        dgvRegistrations.AllowUserToResizeRows = False
        dgvRegistrations.AutoGenerateColumns = False
        dgvRegistrations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
        dgvRegistrations.BackgroundColor = SystemColors.Window
        dgvRegistrations.BorderStyle = BorderStyle.None
        dgvRegistrations.ColumnHeadersHeight = 38
        dgvRegistrations.EnableHeadersVisualStyles = False
        dgvRegistrations.Font = New Font("Segoe UI", 9.5F)
        dgvRegistrations.RowHeadersVisible = False
        dgvRegistrations.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvRegistrations.MultiSelect = False
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "schoolId", .HeaderText = "SCHOOL ID", .DataPropertyName = "SchoolId", .Width = 110})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "fullName", .HeaderText = "FULL NAME", .DataPropertyName = "FullName", .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "email", .HeaderText = "EMAIL", .DataPropertyName = "Email", .Width = 210})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "username", .HeaderText = "USERNAME", .DataPropertyName = "Username", .Width = 130})
        dgvRegistrations.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "registeredAt", .HeaderText = "REGISTERED", .DataPropertyName = "RegisteredAt", .Width = 145})
        dgvRegistrations.Columns.Add(New DataGridViewButtonColumn With {.Name = "approve", .HeaderText = "", .Text = "Approve", .UseColumnTextForButtonValue = True, .Width = 85})
        dgvRegistrations.Columns.Add(New DataGridViewButtonColumn With {.Name = "reject", .HeaderText = "", .Text = "Reject", .UseColumnTextForButtonValue = True, .Width = 75})
        AddHandler dgvRegistrations.CellContentClick, AddressOf dgvRegistrations_CellContentClick
    End Sub

    Private Sub ucStudentRegistrations_Load(sender As Object, e As EventArgs)
        LoadRegistrations()
    End Sub

    Private Sub LoadRegistrations()
        Try
            _registrations = UserService.GetPendingStudentRegistrations()
            dgvRegistrations.DataSource = Nothing
            dgvRegistrations.DataSource = _registrations
            For Each row As DataGridViewRow In dgvRegistrations.Rows
                If row.DataBoundItem IsNot Nothing Then
                    Dim registration As User = DirectCast(row.DataBoundItem, User)
                    row.Cells("registeredAt").Value = If(registration.RegisteredAt.HasValue, registration.RegisteredAt.Value.ToString("MMM d, yyyy h:mm tt"), String.Empty)
                End If
            Next
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Student Registrations", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvRegistrations_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.RowIndex >= _registrations.Count Then Return
        Dim registration As User = _registrations(e.RowIndex)
        If dgvRegistrations.Columns(e.ColumnIndex).Name = "approve" Then
            Review(registration, True)
        ElseIf dgvRegistrations.Columns(e.ColumnIndex).Name = "reject" Then
            Review(registration, False)
        End If
    End Sub

    Private Sub Review(registration As User, approve As Boolean)
        Dim actionText As String = If(approve, "approve", "reject")
        Dim prompt As String = actionText.Substring(0, 1).ToUpperInvariant() & actionText.Substring(1) &
                               " the student account for " & registration.FullName & " (" & registration.SchoolId & ")?"
        If MessageBox.Show(prompt, "Confirm Registration Review", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Dim remarks As String = String.Empty
        If Not approve Then
            remarks = InputBox("Optional rejection remarks:", "Reject Registration")
        End If

        Try
            UserService.ReviewStudentRegistration(registration.UserId, approve, Session.CurrentUser.UserId, remarks)
            LoadRegistrations()
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Student Registrations", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
