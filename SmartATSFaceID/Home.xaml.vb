Public Class Home
    ' Dictionnaire pour stocker les instances de pages et éviter de les recréer
    Private PageCache As New Dictionary(Of String, UserControl)
    Private Sub NavigateTo(ByVal pageName As String)
        ' Vérifier si la page est déjà en cache
        If Not PageCache.ContainsKey(pageName) Then

            ' 1. Trouver le Type de la page par son nom
            Dim pageType As Type = Type.GetType(Me.GetType().Namespace & "." & pageName)

            If pageType IsNot Nothing Then
                ' 2. Créer une nouvelle instance de la page
                Dim newPage As UserControl = DirectCast(Activator.CreateInstance(pageType), UserControl)

                ' 3. Ajouter la page au cache
                PageCache.Add(pageName, newPage)
            Else
                ' Gérer l'erreur si le nom de page est incorrect
                MessageBox.Show($"Erreur: La page '{pageName}' est introuvable.", "Erreur de Navigation")
                Return
            End If
        End If

        ' 4. Charger la page mise en cache dans le ContentControl
        ContentArea.Content = PageCache(pageName)

    End Sub

    Private Sub MenuButton_Click(sender As Object, e As RoutedEventArgs)
        ' Le Tag du bouton contient le nom de la page à charger
        Dim clickedButton As Button = DirectCast(sender, Button)
        Dim pageName As String = clickedButton.Tag.ToString()

        NavigateTo(pageName)
    End Sub

    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        ' Charger la page par défaut au démarrage
        NavigateTo("HomeControl1")
    End Sub

    Private Sub EmployeButton_Click(sender As Object, e As RoutedEventArgs) Handles employeBtn.Click
        ' Le Tag du bouton contient le nom de la page à charger
        Dim clickedButton As Button = DirectCast(sender, Button)
        Dim pageName As String = clickedButton.Tag.ToString()

        NavigateTo(pageName)
    End Sub

    Private Sub ReportButton_Click(sender As Object, e As RoutedEventArgs) Handles rapportBtn.Click
        ' Le Tag du bouton contient le nom de la page à charger
        Dim clickedButton As Button = DirectCast(sender, Button)
        Dim pageName As String = clickedButton.Tag.ToString()

        NavigateTo(pageName)
    End Sub
End Class
