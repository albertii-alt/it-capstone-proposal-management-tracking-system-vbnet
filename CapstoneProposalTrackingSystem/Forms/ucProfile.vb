Option Strict On
Option Explicit On

Imports System.IO

Public Class ucProfile
    Inherits UserControl

    Private ReadOnly txtFullName As New TextBox()
    Private ReadOnly txtUsername As New TextBox()
    Private ReadOnly txtEmail As New TextBox()
    Private ReadOnly picProfilePhoto As New PictureBox()
    Private ReadOnly lblPhotoPlaceholder As New Label()
    Private ReadOnly lblProfileStatus As New Label()
    Private ReadOnly lblPasswordStatus As New Label()
    Private _profilePhotoBytes As Byte()
    Private _originalProfile As User

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Dim pageLayout As New TableLayoutPanel()
        Dim headerPanel As New Panel()
        Dim lblHeader As New Label()
        Dim lblSubtitle As New Label()
        Dim editorLayout As New TableLayoutPanel()
        Dim profilePanel As New Panel()
        Dim passwordPanel As New Panel()
        Dim profileHeader As Label = CreateSectionHeader("Profile details")
        Dim passwordHeader As Label = CreateSectionHeader("Change password")
        Dim photoPanel As New Panel()
        Dim btnChoosePhoto As Button = CreateButton("Choose photo")
        Dim btnRemovePhoto As Button = CreateButton("Remove")
        Dim profileFields As New TableLayoutPanel()
        Dim profileFooter As New TableLayoutPanel()
        Dim btnSaveProfile As Button = CreatePrimaryButton("Save profile")
        Dim btnChangePassword As Button = CreatePrimaryButton("Change password")
        Dim lblPasswordHint As New Label()

        Me.SuspendLayout()
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = SystemColors.Control
        Me.Name = "ucProfile"

        pageLayout.Dock = DockStyle.Fill
        pageLayout.AutoScroll = True
        pageLayout.Padding = New Padding(22)
        pageLayout.ColumnCount = 1
        pageLayout.RowCount = 2
        pageLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        pageLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 76.0F))
        pageLayout.RowStyles.Add(New RowStyle(SizeType.Absolute, 520.0F))

        lblHeader.Dock = DockStyle.Top
        lblHeader.Height = 40
        lblHeader.Text = "Profile"
        lblHeader.Font = New Font("Segoe UI", 23.0F, FontStyle.Bold)
        lblHeader.ForeColor = Color.FromArgb(45, 45, 45)
        lblHeader.TextAlign = ContentAlignment.MiddleLeft

        lblSubtitle.Dock = DockStyle.Fill
        lblSubtitle.Text = "Manage your account details and sign-in password."
        lblSubtitle.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblSubtitle.ForeColor = Color.DimGray
        lblSubtitle.TextAlign = ContentAlignment.MiddleLeft

        headerPanel.Dock = DockStyle.Fill
        headerPanel.Controls.Add(lblSubtitle)
        headerPanel.Controls.Add(lblHeader)

        editorLayout.Dock = DockStyle.Fill
        editorLayout.ColumnCount = 2
        editorLayout.RowCount = 1
        editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 58.0F))
        editorLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 42.0F))
        editorLayout.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        profilePanel.Dock = DockStyle.Fill
        profilePanel.Size = New Size(450, 520)
        profilePanel.BackColor = SystemColors.Window
        profilePanel.Padding = New Padding(20)
        profilePanel.Margin = New Padding(0, 0, 10, 0)
        profilePanel.Controls.Add(profileHeader)

        photoPanel.Size = New Size(116, 116)
        photoPanel.Location = New Point(20, 64)

        picProfilePhoto.Name = "picProfilePhoto"
        picProfilePhoto.Size = New Size(104, 104)
        picProfilePhoto.Location = New Point(0, 0)
        picProfilePhoto.SizeMode = PictureBoxSizeMode.Zoom
        picProfilePhoto.BackColor = Color.FromArgb(244, 239, 231)

        lblPhotoPlaceholder.Dock = DockStyle.Fill
        lblPhotoPlaceholder.Text = "PHOTO"
        lblPhotoPlaceholder.TextAlign = ContentAlignment.MiddleCenter
        lblPhotoPlaceholder.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblPhotoPlaceholder.ForeColor = Color.FromArgb(122, 31, 46)
        lblPhotoPlaceholder.BackColor = Color.FromArgb(244, 239, 231)

        photoPanel.Controls.Add(picProfilePhoto)
        photoPanel.Controls.Add(lblPhotoPlaceholder)

        btnChoosePhoto.Location = New Point(146, 76)
        btnChoosePhoto.Size = New Size(122, 32)
        AddHandler btnChoosePhoto.Click, AddressOf btnChoosePhoto_Click

        btnRemovePhoto.Location = New Point(146, 116)
        btnRemovePhoto.Size = New Size(92, 30)
        AddHandler btnRemovePhoto.Click, AddressOf btnRemovePhoto_Click

        profileFields.Name = "tlpProfileFields"
        profileFields.Location = New Point(20, 198)
        profileFields.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        profileFields.Size = New Size(410, 198)
        profileFields.ColumnCount = 1
        profileFields.RowCount = 6
        profileFields.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
        profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
        profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 22.0F))
        profileFields.RowStyles.Add(New RowStyle(SizeType.Absolute, 36.0F))
        ConfigureTextBox(txtFullName, "txtFullName", 100)
        ConfigureTextBox(txtUsername, "txtUsername", 50)
        ConfigureTextBox(txtEmail, "txtEmail", 100)
        profileFields.Controls.Add(CreateFieldLabel("Full name"), 0, 0)
        profileFields.Controls.Add(txtFullName, 0, 1)
        profileFields.Controls.Add(CreateFieldLabel("Username"), 0, 2)
        profileFields.Controls.Add(txtUsername, 0, 3)
        profileFields.Controls.Add(CreateFieldLabel("Email"), 0, 4)
        profileFields.Controls.Add(txtEmail, 0, 5)

        profileFooter.Name = "tlpProfileFooter"
        profileFooter.Dock = DockStyle.Bottom
        profileFooter.Height = 82
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

        profilePanel.Controls.Add(photoPanel)
        profilePanel.Controls.Add(btnChoosePhoto)
        profilePanel.Controls.Add(btnRemovePhoto)
        profilePanel.Controls.Add(profileFields)
        profilePanel.Controls.Add(profileFooter)

        passwordPanel.Dock = DockStyle.Fill
        passwordPanel.Size = New Size(326, 520)
        passwordPanel.BackColor = SystemColors.Window
        passwordPanel.Padding = New Padding(20)
        passwordPanel.Margin = New Padding(10, 0, 0, 0)
        passwordPanel.Controls.Add(passwordHeader)

        lblPasswordHint.Location = New Point(20, 66)
        lblPasswordHint.Size = New Size(286, 42)
        lblPasswordHint.Text = "Enter your current password to confirm the change."
        lblPasswordHint.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        lblPasswordHint.ForeColor = Color.DimGray

        btnChangePassword.Location = New Point(20, 126)
        btnChangePassword.Size = New Size(160, 36)
        AddHandler btnChangePassword.Click, AddressOf btnChangePassword_Click

        lblPasswordStatus.Name = "lblPasswordStatus"
        lblPasswordStatus.AutoSize = False
        lblPasswordStatus.Location = New Point(20, 174)
        lblPasswordStatus.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lblPasswordStatus.Size = New Size(286, 48)
        lblPasswordStatus.ForeColor = Color.DimGray

        passwordPanel.Controls.Add(lblPasswordHint)
        passwordPanel.Controls.Add(btnChangePassword)
        passwordPanel.Controls.Add(lblPasswordStatus)

        editorLayout.Controls.Add(profilePanel, 0, 0)
        editorLayout.Controls.Add(passwordPanel, 1, 0)
        pageLayout.Controls.Add(headerPanel, 0, 0)
        pageLayout.Controls.Add(editorLayout, 0, 1)
        Me.Controls.Add(pageLayout)

        AddHandler btnSaveProfile.Click, AddressOf btnSaveProfile_Click
        AddHandler MyBase.Load, AddressOf ucProfile_Load
        Me.ResumeLayout(False)
    End Sub

    Private Shared Function CreateSectionHeader(text As String) As Label
        Return New Label With {
            .AutoSize = True,
            .Text = text,
            .Font = New Font("Segoe UI", 13.0F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(45, 45, 45),
            .Location = New Point(20, 18)
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
                _profilePhotoBytes)

            LoadUser(UserService.GetOwnProfile(Session.CurrentUser.UserId))
            Session.CurrentUser.Username = txtUsername.Text.Trim()
            Session.CurrentUser.FullName = txtFullName.Text.Trim()
            Session.CurrentUser.Email = txtEmail.Text.Trim()
            Session.CurrentUser.ProfilePhoto = _profilePhotoBytes
            SetProfileStatus(String.Empty, False)
            ShowSuccessMessage("Profile saved", "Your profile changes have been saved.")
        Catch ex As Exception
            SetProfileStatus("Could not save your profile: " & ex.Message, True)
        End Try
    End Sub

    Private Sub btnChangePassword_Click(sender As Object, e As EventArgs)
        If Session.CurrentUser Is Nothing Then Return

        Using passwordDialog As New frmChangePassword(Session.CurrentUser.UserId)
            If passwordDialog.ShowDialog(Me.FindForm()) = DialogResult.OK Then
                SetPasswordStatus(String.Empty, False)
                ShowSuccessMessage("Password changed", "Your password has been changed successfully.")
            End If
        End Using
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