Option Strict On
Option Explicit On

Imports System.Windows.Forms

Public Module FormDisplayHelper

    Public Sub MaximizeFullScreen(targetForm As Form)
        targetForm.FormBorderStyle = FormBorderStyle.Sizable
        targetForm.WindowState = FormWindowState.Maximized
    End Sub

    Public Sub CenterCardOnResize(targetForm As Form, cardPanel As Panel)
        Dim repositionCard As EventHandler =
            Sub(sender As Object, e As EventArgs)
                cardPanel.Left = Math.Max(0, (targetForm.ClientSize.Width - cardPanel.Width) \ 2)
                cardPanel.Top = Math.Max(0, (targetForm.ClientSize.Height - cardPanel.Height) \ 2)
            End Sub

        AddHandler targetForm.Resize, repositionCard
        repositionCard(targetForm, EventArgs.Empty)
    End Sub

    Public Sub CenterCardInContainer(container As Control, cardPanel As Panel)
        Dim repositionCard As EventHandler =
            Sub(sender As Object, e As EventArgs)
                cardPanel.Left = Math.Max(0, (container.ClientSize.Width - cardPanel.Width) \ 2)
                cardPanel.Top = Math.Max(0, (container.ClientSize.Height - cardPanel.Height) \ 2)
            End Sub

        AddHandler container.Resize, repositionCard
        repositionCard(container, EventArgs.Empty)
    End Sub

End Module