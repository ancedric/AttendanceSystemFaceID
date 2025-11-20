Imports Emgu.CV
Imports Emgu.CV.Face
Imports Emgu.CV.Structure

Public Class RegisterWindow
    Private ReadOnly _faceImage As Mat
    Public Sub New(ByVal faceImage As Mat)

        ' Cet appel est requis par le concepteur.
        InitializeComponent()

        _faceImage = faceImage

    End Sub

    Private Sub AddEmploye_Click(sender As Object, e As MouseButtonEventArgs) Handles addEmployeBtn.MouseLeftButtonUp
        Dim nom As String = nomTextBox.Text.ToString()
        Dim prenom As String = prenomTextBox.Text.ToString()

        If nom IsNot Nothing AndAlso prenom IsNot Nothing Then

            Dim employeManager As New Employe()
            Dim newUserId As Integer = employeManager.createUser(nom, prenom, _faceImage) ' <-- Récupère l'ID

            If newUserId > 0 Then

                Try
                    ' **********************************************
                    ' * APPEL RÉEL AU GESTIONNAIRE D'ENTRAÎNEMENT *
                    ' **********************************************
                    FaceRecognizerManager.UpdateAndSaveModel(_faceImage, newUserId)

                    MsgBox($"Employé ajouté (ID: {newUserId}) et modèle entraîné avec succès!")
                    Me.Close()

                Catch ex As Exception
                    MsgBox($"Succès de la BD, mais erreur d'entraînement : {ex.Message}")
                    ' Devrait idéalement supprimer l'entrée de la BD si l'entraînement échoue
                End Try

            Else
                MsgBox("Erreur lors de l'ajout de l'employé en base de données!")
            End If
        End If
    End Sub
End Class
