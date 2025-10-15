Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class NotificationPopup
    Public Sub SetMessage(ByVal message As String, ByVal isSuccess As Boolean)
        messageText.Text = message

        If isSuccess Then
            border.Background = New SolidColorBrush(Color.FromArgb(200, 46, 204, 113)) ' Fond vert semi-transparent
            ' iconImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/success.png")) ' Optionnel : Icône de succès
        Else
            border.Background = New SolidColorBrush(Color.FromArgb(200, 231, 76, 60)) ' Fond rouge semi-transparent
            ' iconImage.Source = New BitmapImage(New Uri("pack://application:,,,/Resources/error.png")) ' Optionnel : Icône d'erreur
        End If
    End Sub
End Class