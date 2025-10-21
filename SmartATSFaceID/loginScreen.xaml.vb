Option Strict On
Option Explicit On

Public Class loginScreen
    Private Sub loginButton_Click(sender As Object, e As RoutedEventArgs) Handles loginButton.Click
        Dim email As String = emailTextBox.Text.ToString()
        Dim password As String = passwordTextBox.Text.ToString()
        Dim admin As Admin = New Admin("", "", email, password)
        If (email IsNot Nothing AndAlso password IsNot Nothing) Then
            admin.login(email, password)
        Else
            MessageBox.Show("Veuillez remplir tous les champs s'il vous plait!")
        End If
    End Sub

    Private Sub Mouse_Click(sender As Object, e As MouseButtonEventArgs) Handles registerLabel.MouseLeftButtonDown
        Dim signup As New SignupScreen
        Me.Close()
        signup.Show()
    End Sub
End Class
