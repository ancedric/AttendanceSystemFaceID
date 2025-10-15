Option Strict On
Option Explicit On
Imports System.Windows.Controls.Primitives
Imports System.Windows.Threading

Public Class SignupScreen

    Private Sub SignupButton_Click(sender As Object, e As RoutedEventArgs) Handles signupButton.Click
        Dim nom As String = nomTextBox.Text.ToString()
        Dim prenom As String = prenomTextBox.Text.ToString()
        Dim email As String = emailTextBox.Text.ToString()
        Dim pwd As String = passwordTextBox.Password.ToString()

        Dim admin As Admin = New Admin(nom, prenom, email, pwd)

        If (email IsNot Nothing AndAlso password IsNot Nothing) Then
            Dim loggedIn As Boolean = admin.login(email, pwd)
            If (loggedIn = True) Then
                ShowNotification("Connexion réussie !", True)
            Else
                ShowNotification("Identifiants incorrects !", False)
            End If
        Else
            ShowNotification("Veuillez remplir tous les champs s'il vous plait !", False)
        End If
    End Sub
    Private Sub Mouse_Click(sender As Object, e As MouseButtonEventArgs)
        Dim login As loginScreen = New loginScreen
        Me.Close()
        login.Show()
    End Sub

    Private Sub ShowNotification(ByVal message As String, ByVal isSuccess As Boolean)
        ' 1. Créer le contenu
        Dim notificationContent As New NotificationPopup()
        notificationContent.SetMessage(message, isSuccess)

        ' 2. Créer le Popup
        Dim notificationPopup As New Popup()
        notificationPopup.AllowsTransparency = True
        notificationPopup.PlacementTarget = Me.MainGrid ' Cibler la grille principale
        notificationPopup.Placement = PlacementMode.Center ' Placer au centre
        notificationPopup.StaysOpen = False

        ' 3. Attacher le contenu au Popup
        notificationPopup.Child = notificationContent

        ' 4. Afficher le Popup
        notificationPopup.IsOpen = True

        ' 5. Démarre un minuteur pour fermer le pop-up
        Dim timer As New DispatcherTimer()
        timer.Interval = TimeSpan.FromSeconds(3) ' 3 secondes

        ' On utilise une lambda ou un gestionnaire anonyme pour fermer 
        ' ce popup spécifique, on passe donc le popup au ClosePopup
        AddHandler timer.Tick, Sub()
                                   timer.Stop()
                               End Sub
        notificationPopup.IsOpen = False
    End Sub
End Class
