Option Strict On
Option Explicit On

Public Class loginScreen
    Private Sub LoginButton_Click(sender As Object, e As RoutedEventArgs) Handles loginButton.Click
        Dim email As String = emailTextBox.Text.ToString()
        Dim password As String = passwordTextBox.Password.ToString()
        If (email IsNot Nothing AndAlso password IsNot Nothing) Then
            Dim loggedIn As Boolean = Admin.Login(email, password)
            If (loggedIn = True) Then
                Dim home As New Home
                Me.Close()
                home.Show()
            Else
                MessageBox.Show("Email ou mot de passe incorrect")
            End If
            MessageBox.Show("Veuillez remplir tous les champs s'il vous plait!")
        End If
    End Sub

    Private Sub Mouse_Click(sender As Object, e As MouseButtonEventArgs) Handles registerLabel.MouseLeftButtonDown
        Dim signup As New SignupScreen
        Me.Close()
        signup.Show()
    End Sub
End Class
