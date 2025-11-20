Public Class HomeControl1
    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        adminNameLabel.Content = loggedAdmin.GetInfos()
    End Sub

    Private Sub OpenSession_Click(sender As Object, e As MouseButtonEventArgs) Handles openSessionBtn.MouseLeftButtonDown
        Dim password As String = sessionPasswordTextBox.Password
        If (password = "") Then
            MsgBox("Veuillez entrer un mot de passe pour la nouvelle session")
        Else
            sessionPassword = password

            Dim scan As New ScanWindow
            scan.Show()
        End If
    End Sub
End Class
