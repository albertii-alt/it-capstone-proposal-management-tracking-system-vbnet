Option Strict On
Option Explicit On

Imports System.IO
Imports System.Drawing.Drawing2D

Public Class ucProfile
    Inherits UserControl

    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly txtSchoolId As New TextBox()
    Private ReadOnly picProfilePhoto As New PictureBox()
    Private ReadOnly lblPhotoPlaceholder As New Label()
    Private ReadOnly lblProfileStatus As New Label()
    Private ReadOnly lblPasswordStatus As New Label()
    Private ReadOnly txtCurrentPassword As New TextBox()
    Private ReadOnly txtNewPassword As New TextBox()
    Private ReadOnly txtConfirmPassword As New TextBox()
    Private _profilePhotoBytes As Byte()
    Private _originalProfile As User

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim pageLayout As New TableLayoutPanel()
        Dim editorLayout As New TableLayoutPanel()
        Dim profilePanel As New TableLayoutPanel()
        Dim passwordPanel As New TableLayoutPanel()
        Dim profileHeader As Label = CreateSectionHeader("Profile details")
        Dim passwordHeader As Label = CreateSectionHeader("Change password")
        Dim photoPanel As New Panel()
        Dim photoLayout As New TableLayoutPanel()
        Dim photoActions As New FlowLayoutPanel()
        Dim btnChoosePhoto As Button = CreateButton("Choose photo")
        Dim btnRemovePhoto As Button = CreateButton("Remove")
        Dim profileFields As New TableLayoutPanel()
        Dim profileFooter As New TableLayoutPanel()
        Dim btnSaveProfile As Button = CreatePrimaryButton("Save profile")
        Dim btnChangePassword As Button = CreatePrimaryButton("Change password")
        Dim lblPasswordHint As New Label()
        Dim passwordContent As New TableLayoutPanel()
        Dim passwordFields As New TableLayoutPanel()
        Dim passwordStatusPanel As New Panel()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Dpi
        Me.BackColor = SystemColors.Control
        Me.Name = "ucProfile"

        pageLayout.Dock = DockStyle.Fill
        pageLayout.AutoScroll = True
        pageLayout.Padding = New Padding(20)
        pageLayout.ColumnCount = 1
        pageLayout.RowCount = 1
        pageLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        pageLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        editorLayout.Dock = DockStyle.Fill
        editorLayout.ColumnCount = 2
        editorLayout.RowCount = 1
        editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0F))
        editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0F))
        editorLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        profilePanel.Dock = DockStyle.Fill
        profilePanel.ColumnCount = 1
        profilePanel.RowCount = 4
        profilePanel.BackColor = SystemColors.Window
        profilePanel.Padding = New Padding(20)
        profilePanel.Margin = New Padding(0, 0, 8, 0)
        profilePanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        profilePanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        profilePanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 228.0F))
        profilePanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        profilePanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 82.0F))
        profileHeader.Dock = DockStyle.Fill
        profilePanel.Controls.Add(profileHeader, 0, 0)

        photoPanel.Dock = DockStyle.Fill
        photoPanel.BackColor = Color.FromArgb(250, 248, 245)

        picProfilePhoto.Name = "picProfilePhoto"
        picProfilePhoto.Size = New Size(160, 160)
        picProfilePhoto.Margin = New Padding(0)
        picProfilePhoto.SizeMode = PictureBoxSizeMode.Zoom
        picProfilePhoto.BackColor = Color.FromArgb(244, 239, 231)

        lblPhotoPlaceholder.Size = picProfilePhoto.Size
        lblPhotoPlaceholder.Text = "PHOTO"
        lblPhotoPlaceholder.TextAlign = ContentAlignment.MiddleCenter
        lblPhotoPlaceholder.Font = New Font("Segoe UI", 11.0F, FontStyle.Bold)
        lblPhotoPlaceholder.ForeColor = Color.FromArgb(122, 31, 46)
        lblPhotoPlaceholder.BackColor = Color.FromArgb(244, 239, 231)

        photoPanel.Controls.Add(picProfilePhoto)
        photoPanel.Controls.Add(lblPhotoPlaceholder)
        AddHandler photoPanel.Resize, Sub(sender, e) CenterProfilePhoto(photoPanel)
        AddHandler picProfilePhoto.SizeChanged, Sub(sender, e) ApplyCircularPhotoRegion(picProfilePhoto)
        AddHandler lblPhotoPlaceholder.SizeChanged, Sub(sender, e) ApplyCircularPhotoRegion(lblPhotoPlaceholder)
        CenterProfilePhoto(photoPanel)

        photoActions.Dock = DockStyle.None
        photoActions.FlowDirection = FlowDirection.LeftToRight
        photoActions.WrapContents = False
        photoActions.Anchor = AnchorStyles.None
        photoActions.AutoSize = True
        photoActions.AutoSizeMode = AutoSizeMode.GrowAndShrink
        btnChoosePhoto.Size = New Size(150, 34)
        btnChoosePhoto.Margin = New Padding(0, 0, 10, 0)
        btnChoosePhoto.BackColor = Color.FromArgb(122, 31, 46)
        btnChoosePhoto.ForeColor = Color.White
        btnChoosePhoto.FlatAppearance.BorderSize = 0
        btnChoosePhoto.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        btnChoosePhoto.Cursor = Cursors.Hand
        AddHandler btnChoosePhoto.Click, AddressOf btnChoosePhoto_Click

        btnRemovePhoto.Text = "Remove photo"
        btnRemovePhoto.Size = New Size(130, 34)
        btnRemovePhoto.Margin = New Padding(0)
        btnRemovePhoto.BackColor = Color.White
        btnRemovePhoto.ForeColor = Color.FromArgb(166, 58, 58)
        btnRemovePhoto.FlatAppearance.BorderColor = Color.FromArgb(166, 58, 58)
        btnRemovePhoto.FlatAppearance.BorderSize = 1
        btnRemovePhoto.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        btnRemovePhoto.Cursor = Cursors.Hand
        AddHandler btnRemovePhoto.Click, AddressOf btnRemovePhoto_Click
        photoActions.Controls.Add(btnChoosePhoto)
        photoActions.Controls.Add(btnRemovePhoto)

        photoLayout.Dock = DockStyle.Fill
        photoLayout.ColumnCount = 1
        photoLayout.RowCount = 2
        photoLayout.Padding = New Padding(0, 4, 0, 4)
        photoLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        photoLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        photoLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        photoLayout.Controls.Add(photoPanel, 0, 0)
        photoLayout.Controls.Add(photoActions, 0, 1)
        profilePanel.Controls.Add(photoLayout, 0, 1)

        profileFields.Name = "tlpProfileFields"
        profileFields.Dock = DockStyle.Fill
        profileFields.ColumnCount = 1
        profileFields.RowCount = 4
        profileFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        For index As Integer = 0 To 3
            profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 78.0F))
        Next
        ConfigureTextBox(txtFullName, "txtFullName", 100)
        ConfigureTextBox(txtUsername, "txtUsername", 50)
        ConfigureTextBox(txtEmail, "txtEmail", 100)
        ConfigureTextBox(txtSchoolId, "txtSchoolId", 30)
        profileFields.Controls.Add(CreateProfileField("Full name", txtFullName), 0, 0)
        profileFields.Controls.Add(CreateProfileField("Username", txtUsername), 0, 1)
        profileFields.Controls.Add(CreateProfileField("Email", txtEmail), 0, 2)
        profileFields.Controls.Add(CreateProfileField("School ID", txtSchoolId), 0, 3)
        profileFields.AutoSize = True
        profileFields.AutoSizeMode = AutoSizeMode.GrowAndShrink
        profilePanel.Controls.Add(profileFields, 0, 2)

        profileFooter.Name = "tlpProfileFooter"
        profileFooter.Dock = DockStyle.Fill
        profileFooter.ColumnCount = 1
        profileFooter.RowCount = 2
        profileFooter.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        profileFooter.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        profileFooter.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        btnSaveProfile.Anchor = AnchorStyles.Left
        btnSaveProfile.Margin = New Padding(0)
        profileFooter.Controls.Add(btnSaveProfile, 0, 0)

        lblProfileStatus.Name = "lblProfileStatus"
        lblProfileStatus.AutoSize = False
        lblProfileStatus.Dock = DockStyle.Fill
        lblProfileStatus.TextAlign = ContentAlignment.MiddleLeft
        lblProfileStatus.AutoEllipsis = True
        lblProfileStatus.ForeColor = Color.DimGray
        profileFooter.Controls.Add(lblProfileStatus, 0, 1)
        profilePanel.Controls.Add(profileFooter, 0, 3)

        passwordPanel.Dock = DockStyle.Fill
        passwordPanel.ColumnCount = 1
        passwordPanel.RowCount = 3
        passwordPanel.BackColor = SystemColors.Window
        passwordPanel.Padding = New Padding(20)
        passwordPanel.Margin = New Padding(8, 0, 0, 0)
        passwordPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        passwordPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        passwordPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        passwordPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 74.0F))
        passwordHeader.Dock = DockStyle.Fill
        passwordPanel.Controls.Add(passwordHeader, 0, 0)

        passwordContent.Dock = DockStyle.Fill
        passwordContent.ColumnCount = 1
        passwordContent.RowCount = 4
        passwordContent.Padding = New Padding(0, 12, 0, 0)
        passwordContent.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        passwordContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        passwordContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 200.0F))
        passwordContent.RowStyles.Add(New RowStyle(SizeType.Absolute, 48.0F))
        passwordContent.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        lblPasswordHint.Dock = DockStyle.Fill
        lblPasswordHint.Text = "Confirm your current password, then choose a new password."
        lblPasswordHint.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblPasswordHint.ForeColor = Color.DimGray
        lblPasswordHint.TextAlign = ContentAlignment.TopLeft

        passwordFields.Dock = DockStyle.Top
        passwordFields.ColumnCount = 1
        passwordFields.RowCount = 6
        passwordFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        passwordFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        passwordFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        passwordFields.Height = 186
        passwordFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        passwordFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        passwordFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        passwordFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 38.0F))
        ConfigurePasswordTextBox(txtCurrentPassword, "txtCurrentPassword")
        ConfigurePasswordTextBox(txtNewPassword, "txtNewPassword")
        ConfigurePasswordTextBox(txtConfirmPassword, "txtConfirmPassword")
        passwordFields.Controls.Add(CreateFieldLabel("Current password"), 0, 0)
        passwordFields.Controls.Add(txtCurrentPassword, 0, 1)
        passwordFields.Controls.Add(CreateFieldLabel("New password"), 0, 2)
        passwordFields.Controls.Add(txtNewPassword, 0, 3)
        passwordFields.Controls.Add(CreateFieldLabel("Confirm new password"), 0, 4)
        passwordFields.Controls.Add(txtConfirmPassword, 0, 5)

        btnChangePassword.Anchor = AnchorStyles.Left
        btnChangePassword.Size = New Size(175, 38)
        passwordContent.Controls.Add(lblPasswordHint, 0, 0)
        passwordContent.Controls.Add(passwordFields, 0, 1)
        passwordContent.Controls.Add(btnChangePassword, 0, 2)
        AddHandler btnChangePassword.Click, AddressOf btnChangePassword_Click

        lblPasswordStatus.Name = "lblPasswordStatus"
        lblPasswordStatus.AutoSize = False
        lblPasswordStatus.Dock = DockStyle.Fill
        lblPasswordStatus.ForeColor = Color.DimGray
        lblPasswordStatus.TextAlign = ContentAlignment.MiddleLeft

        passwordStatusPanel.Dock = DockStyle.Fill
        passwordStatusPanel.Controls.Add(lblPasswordStatus)
        passwordPanel.Controls.Add(passwordContent, 0, 1)
        passwordPanel.Controls.Add(passwordStatusPanel, 0, 2)

        editorLayout.Controls.Add(profilePanel, 0, 0)
        editorLayout.Controls.Add(passwordPanel, 1, 0)
        pageLayout.Controls.Add(editorLayout, 0, 0)
        Me.Controls.Add(pageLayout)

        AddHandler editorLayout.Resize, Sub(sender, e) UpdateResponsiveLayout(editorLayout)
        AddHandler Me.Resize, Sub(sender, e) UpdateResponsiveLayout(editorLayout)
        AddHandler btnSaveProfile.Click, AddressOf btnSaveProfile_Click
        AddHandler MyBase.Load, AddressOf ucProfile_Load
        UpdateResponsiveLayout(editorLayout)
        Me.ResumeLayout(False)
    End Sub

    Private Shared Function CreateProfileField(labelText As String, textBox As TextBox) As TableLayoutPanel
        Dim field As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2,
            .Margin = New Padding(0, 3, 0, 7)
        }
        field.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        field.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        field.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        field.Controls.Add(CreateFieldLabel(labelText), 0, 0)
        field.Controls.Add(textBox, 0, 1)
        Return field
    End Function

    Private Sub CenterProfilePhoto(photoPanel As Panel)
        picProfilePhoto.Location = New Point(
            Math.Max(0, (photoPanel.ClientSize.Width - picProfilePhoto.Width) \ 2),
            Math.Max(0, (photoPanel.ClientSize.Height - picProfilePhoto.Height) \ 2))
        lblPhotoPlaceholder.Location = picProfilePhoto.Location
        ApplyCircularPhotoRegion(picProfilePhoto)
        ApplyCircularPhotoRegion(lblPhotoPlaceholder)
    End Sub

    Private Shared Sub ApplyCircularPhotoRegion(control As Control)
        If control.Width <= 0 OrElse control.Height <= 0 Then Return
        Dim previousRegion As Region = control.Region
        Using path As New GraphicsPath()
            path.AddEllipse(0, 0, control.Width - 1, control.Height - 1)
            control.Region = New Region(path)
        End Using
        If previousRegion IsNot Nothing Then previousRegion.Dispose()
    End Sub

    Private Shared Sub UpdateResponsiveLayout(editorLayout As TableLayoutPanel)
        If editorLayout.ClientSize.Width < 720 Then
            If editorLayout.ColumnCount = 1 Then Return
            editorLayout.ColumnCount = 1
            editorLayout.RowCount = 2
            editorLayout.ColumnStyles.Clear()
            editorLayout.RowStyles.Clear()
            editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
            editorLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 700.0F))
            editorLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 360.0F))
            editorLayout.Height = 1060
            editorLayout.Dock = DockStyle.Top
            editorLayout.Controls(0).Margin = New Padding(0, 0, 0, 8)
            editorLayout.Controls(1).Margin = New Padding(0, 8, 0, 0)
            editorLayout.SetCellPosition(editorLayout.Controls(0), New TableLayoutPanelCellPosition(0, 0))
            editorLayout.SetCellPosition(editorLayout.Controls(1), New TableLayoutPanelCellPosition(0, 1))
        Else
            If editorLayout.ColumnCount = 2 Then Return
            editorLayout.Dock = DockStyle.Fill
            editorLayout.Height = 0
            editorLayout.ColumnCount = 2
            editorLayout.RowCount = 1
            editorLayout.ColumnStyles.Clear()
            editorLayout.RowStyles.Clear()
            editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0F))
            editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0F))
            editorLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
            editorLayout.Controls(0).Margin = New Padding(0, 0, 8, 0)
            editorLayout.Controls(1).Margin = New Padding(8, 0, 0, 0)
            editorLayout.SetCellPosition(editorLayout.Controls(0), New TableLayoutPanelCellPosition(0, 0))
            editorLayout.SetCellPosition(editorLayout.Controls(1), New TableLayoutPanelCellPosition(1, 0))
        End If
    End Sub

    Private Shared Function CreateSectionHeader(text As String) As Label
        Return New Label With {
            .AutoSize = False,
            .Text = text,
            .Font = New Font("Segoe UI", 13.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(45, 45, 45),
            .TextAlign = ContentAlignment.MiddleLeft
        }
    End Function

    Private Shared Function CreateFieldLabel(text As String) As Label
        Return New Label With {
            .Dock = DockStyle.Fill,
            .Text = text,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(75, 75, 75)
        }
    End Function

    Private Shared Function CreateButton(text As String) As Button
        Return New Button With {
            .Text = text,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(246, 246, 246),
            .ForeColor = Color.FromArgb(65, 65, 65),
            .UseVisualStyleBackColor = False
        }
    End Function

    Private Shared Function CreatePrimaryButton(text As String) As Button
        Dim button As Button = CreateButton(text)
        button.FlatAppearance.BorderSize = 0
        button.BackColor = Color.FromArgb(122, 31, 46)
        button.ForeColor = Color.White
        button.Size = New Size(150, 36)
        Return button
    End Function

    Private Shared Sub ConfigureTextBox(textBox As TextBox, controlName As String, maxLength As Integer)
        textBox.Name = controlName
        textBox.Dock = DockStyle.Fill
        textBox.BorderStyle = BorderStyle.FixedSingle
        textBox.Margin = New Padding(0, 2, 0, 5)
        textBox.MaxLength = maxLength
    End Sub

    Private Shared Sub ConfigurePasswordTextBox(textBox As TextBox, controlName As String)
        textBox.Name = controlName
        textBox.Dock = DockStyle.Fill
        textBox.BorderStyle = BorderStyle.FixedSingle
        textBox.Margin = New Padding(0, 2, 0, 5)
        textBox.UseSystemPasswordChar = True
        textBox.MaxLength = 256
    End Sub

    Private Sub ucProfile_Load(sender As Object, e As EventArgs)
        If Session.CurrentUser Is Nothing Then
            SetProfileStatus("Sign in to manage your profile.", True)
            Return
        End If

        Try
            LoadUser(UserService.GetOwnProfile(Session.CurrentUser.UserId))
        Catch ex As Exception
            SetProfileStatus("Could not load your profile: " & ex.Message, True)
        End Try
    End Sub

    Private Sub LoadUser(user As User)
        _originalProfile = user
        txtFullName.Text = user.FullName
        txtUsername.Text = user.Username
        txtEmail.Text = user.Email
        txtSchoolId.Text = user.SchoolId
        _profilePhotoBytes = user.ProfilePhoto
        ShowProfilePhoto(_profilePhotoBytes)
        lblPhotoPlaceholder.Visible = _profilePhotoBytes Is Nothing
    End Sub

    Private Sub btnChoosePhoto_Click(sender As Object, e As EventArgs)
        Using dialog As New OpenFileDialog()
            dialog.Title = "Choose a profile photo"
            dialog.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            dialog.CheckFileExists = True
            If dialog.ShowDialog() <> DialogResult.OK Then Return

            Try
                Dim photoBytes As Byte() = File.ReadAllBytes(dialog.FileName)
                If photoBytes.Length > 5242880 Then
                    Throw New ApplicationException("Choose an image smaller than 5 MB.")
                End If

                Using stream As New MemoryStream(photoBytes)
                    Using image As Image = Image.FromStream(stream)
                        Using preview As New Bitmap(image)
                            ReplacePreview(New Bitmap(preview))
                        End Using
                    End Using
                End Using

                _profilePhotoBytes = photoBytes
                lblPhotoPlaceholder.Visible = False
                SetProfileStatus("Photo selected. Save your profile to apply it.", False)
            Catch ex As Exception
                SetProfileStatus("Could not use that image: " & ex.Message, True)
            End Try
        End Using
    End Sub

    Private Sub btnRemovePhoto_Click(sender As Object, e As EventArgs)
        _profilePhotoBytes = Nothing
        ReplacePreview(Nothing)
        lblPhotoPlaceholder.Visible = True
        SetProfileStatus("Photo removed. Save your profile to apply the change.", False)
    End Sub

    Private Sub btnSaveProfile_Click(sender As Object, e As EventArgs)
        If Session.CurrentUser Is Nothing Then Return

        Dim changes As List(Of String) = GetProfileChanges()
        If changes.Count = 0 Then
            SetProfileStatus("There are no profile changes to save.", False)
            Return
        End If

        Using confirmation As New frmProfileSaveConfirmation(changes)
            If confirmation.ShowDialog(Me.FindForm()) <> DialogResult.OK Then Return
        End Using

        Try
            UserService.UpdateOwnProfile(
                Session.CurrentUser.UserId,
                txtUsername.Text,
                txtFullName.Text,
                txtEmail.Text,
                txtSchoolId.Text,
                _profilePhotoBytes)

            LoadUser(UserService.GetOwnProfile(Session.CurrentUser.UserId))
            Session.CurrentUser.Username = txtUsername.Text.Trim()
            Session.CurrentUser.FullName = txtFullName.Text.Trim()
            Session.CurrentUser.Email = txtEmail.Text.Trim()
            Session.CurrentUser.SchoolId = txtSchoolId.Text.Trim()
            Session.CurrentUser.ProfilePhoto = _profilePhotoBytes
            SetProfileStatus(String.Empty, False)
            ShowSuccessMessage("Profile saved", "Your profile changes have been saved.")
        Catch ex As Exception
            SetProfileStatus("Could not save your profile: " & ex.Message, True)
        End Try
    End Sub

    Private Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        If Session.CurrentUser Is Nothing Then Return

        If txtNewPassword.Text <> txtConfirmPassword.Text Then
            SetPasswordStatus("The new password and confirmation do not match.", True)
            txtConfirmPassword.Focus()
            Return
        End If

        Try
            UserService.ChangeOwnPassword(Session.CurrentUser.UserId, txtCurrentPassword.Text, txtNewPassword.Text)
            txtCurrentPassword.Clear()
            txtNewPassword.Clear()
            txtConfirmPassword.Clear()
            SetPasswordStatus("Your password has been changed successfully.", False)
        Catch ex As ApplicationException
            SetPasswordStatus(ex.Message, True)
            txtCurrentPassword.Clear()
            txtCurrentPassword.Focus()
        Catch ex As Exception
            SetPasswordStatus("Could not update your password. Please try again.", True)
            txtCurrentPassword.Clear()
            txtCurrentPassword.Focus()
        End Try
    End Sub

    Private Sub ShowProfilePhoto(photoBytes As Byte())
        If photoBytes Is Nothing Then
            ReplacePreview(Nothing)
            Return
        End If

        Using stream As New MemoryStream(photoBytes)
            Using image As Image = Image.FromStream(stream)
                ReplacePreview(New Bitmap(image))
            End Using
        End Using
    End Sub

    Private Sub ReplacePreview(image As Image)
        Dim previousImage As Image = picProfilePhoto.Image
        picProfilePhoto.Image = image
        If previousImage IsNot Nothing Then previousImage.Dispose()
    End Sub

    Private Sub SetProfileStatus(message As String, isError As Boolean)
        lblProfileStatus.Text = message
        lblProfileStatus.ForeColor = If(isError, Color.FromArgb(166, 58, 58), Color.FromArgb(76, 116, 86))
    End Sub

    Private Sub SetPasswordStatus(message As String, isError As Boolean)
        lblPasswordStatus.Text = message
        lblPasswordStatus.ForeColor = If(isError, Color.FromArgb(166, 58, 58), Color.FromArgb(76, 116, 86))
    End Sub

    Private Sub ShowSuccessMessage(title As String, message As String)
        Using successDialog As New frmProfileSuccess(title, message)
            successDialog.ShowDialog(Me.FindForm())
        End Using
    End Sub

    Private Function GetProfileChanges() As List(Of String)
        Dim changes As New List(Of String)()
        If _originalProfile Is Nothing Then Return changes

        If Not String.Equals(txtFullName.Text.Trim(), _originalProfile.FullName, StringComparison.Ordinal) Then
            changes.Add("Full name: " & _originalProfile.FullName & " -> " & txtFullName.Text.Trim())
        End If

        If Not String.Equals(txtUsername.Text.Trim(), _originalProfile.Username, StringComparison.Ordinal) Then
            changes.Add("Username: " & _originalProfile.Username & " -> " & txtUsername.Text.Trim())
        End If

        If Not String.Equals(txtEmail.Text.Trim(), _originalProfile.Email, StringComparison.Ordinal) Then
            changes.Add("Email: " & _originalProfile.Email & " -> " & txtEmail.Text.Trim())
        End If

        If Not String.Equals(txtSchoolId.Text.Trim(), If(_originalProfile.SchoolId, ""), StringComparison.Ordinal) Then
            changes.Add("School ID: " & If(_originalProfile.SchoolId, "") & " -> " & txtSchoolId.Text.Trim())
        End If

        If Not PhotosMatch(_profilePhotoBytes, _originalProfile.ProfilePhoto) Then
            Dim photoChange As String = If(_profilePhotoBytes Is Nothing, "removed", If(_originalProfile.ProfilePhoto Is Nothing, "added", "updated"))
            changes.Add("Profile photo: " & photoChange)
        End If

        Return changes
    End Function

    Private Shared Function PhotosMatch(firstPhoto As Byte(), secondPhoto As Byte()) As Boolean
        If firstPhoto Is Nothing OrElse secondPhoto Is Nothing Then
            Return firstPhoto Is Nothing AndAlso secondPhoto Is Nothing
        End If

        If firstPhoto.Length <> secondPhoto.Length Then Return False
        For index As Integer = 0 To firstPhoto.Length - 1
            If firstPhoto(index) <> secondPhoto(index) Then Return False
        Next

        Return True
    End Function
End Class